# Skill: architecture

Exibe ou consulta as decisões arquiteturais do projeto Financii. Use este skill como referência antes de implementar qualquer coisa.

## Como invocar

```
/architecture
/architecture <topico>    # ex: /architecture ddd, /architecture validacao
```

---

## Visão Geral

**Projeto:** Financii — Software de controle financeiro pessoal, com plano de escalar para Open Finance.

**Stack:**
- ASP.NET Core — entry point e controllers
- Entity Framework Core — ORM
- SQL Server — banco de dados
- ASP.NET Identity — autenticação (User/Role com chave `long`)
- JWT Bearer — autorização stateless
- Scrutor — DI por convenção
- FluentValidation — validação de input
- FluentResults — Result Pattern para regras de negócio
- xUnit — testes unitários

---

## Estrutura de Projetos e Solution Folders

```
/0-Presentation/        → Financii.Api
/1-Application/         → Financii.Application
/2-Services/            → (reservado para Domain Services futuros)
/3-Domain/              → Financii.Domain
/4-Infrastructure/
  4.1-Data/             → Financii.Infra.Data
  4.2-Configurations/   → Financii.Infra.Configurations
  4.3-Services/         → Financii.Infra.Services
/Testes/                → Financii.Tests
```

### Responsabilidade de cada projeto

| Projeto | Responsabilidade |
|---|---|
| `Financii.Domain` | Entidades, Value Objects, Domain Events, contratos base |
| `Financii.Application` | AppServices, Controllers, DTOs, Validators, interfaces |
| `Financii.Infra.Data` | DbContext, Repositories, Migrations |
| `Financii.Infra.Configurations` | DI (Scrutor), setup de Identity e JWT no pipeline |
| `Financii.Infra.Services` | Serviços de infraestrutura: JwtService, EmailService, etc. |
| `Financii.Api` | Entry point — Program.cs, appsettings |
| `Financii.Tests` | Testes unitários xUnit |

**Regra de dependência (arquitetura em camadas):**
- Domain não conhece ninguém
- Application conhece Domain e Infra.Data (Service enxerga Repository — válido em camadas)
- Infra.Data conhece apenas Domain (interfaces de repositório vivem no próprio Infra.Data)
- Infra.Services conhece Application (para implementar IJwtService, etc.)
- Infra.Configurations conhece Application, Infra.Data e Infra.Services (orquestra o DI)
- Api conhece Application e Configurations

### Grafo de referências

```
Domain ← Application ← Infra.Configurations → Api
  ↑           ↑                ↑
  └─ Infra.Data ──────────────┘
       ↑
  Infra.Services → Application
```

Resumindo o fluxo de dependência por projeto:
| Projeto | Referencia |
|---|---|
| `Financii.Domain` | — (ninguém) |
| `Financii.Application` | Domain, Infra.Data |
| `Financii.Infra.Data` | Domain |
| `Financii.Infra.Services` | Application |
| `Financii.Infra.Configurations` | Application, Infra.Data, Infra.Services |
| `Financii.Api` | Application, Infra.Configurations |
| `Financii.Tests` | Domain, Application |

---

## Domain (DDD)

### Entidades

Todas as entidades — inclusive as do Identity — vivem em `Financii.Domain/Entities/`.

- Entidades de domínio próprio implementam `EntityBase` (que implementa `IAggregateRoot` e `IEntity`)
- Entidades do Identity (`User`, `Role`) estendem `IdentityUser<long>` / `IdentityRole<long>` — ficam em `Financii.Domain/Entities/` porque são entidades do banco de dados, mesmo sendo gerenciadas pelo Identity
- Campos com `private set` — domínio protege o próprio estado
- Nunca são anêmicas (sem setters públicos para campos de negócio)

#### Padrão de Id — `long Id` + `Guid PublicId`

Toda entidade tem **dois identificadores**:
- `long Id` — chave primária interna, usada em FKs e queries no banco
- `Guid PublicId` — identificador externo, exposto nas respostas da API (nunca expor o `long` diretamente)

`EntityBase` e `IEntity` já carregam ambos. `User` (Identity) os declara diretamente por não herdar `EntityBase`.

```csharp
// Entidade de domínio próprio — herda EntityBase
public class Transaction : EntityBase
{
    public long UserId { get; private set; }
    public decimal Amount { get; private set; }

    private Transaction() { } // EF Core

    public static Result<Transaction> Create(decimal amount, long userId)
    {
        var entity = new Transaction
        {
            PublicId = Guid.NewGuid(),
            Amount = amount,
            UserId = userId
        };
        return Result.Ok(entity);
    }
}

// Entidade Identity — também em Domain/Entities/
public class User : IdentityUser<long>, IEntity
{
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid PublicId { get; set; }
}
```

**Nota:** `Financii.Domain.csproj` usa `<FrameworkReference Include="Microsoft.AspNetCore.App" />` para acessar tipos do Identity sem instalar pacote NuGet separado.

### Value Objects
- Imutáveis, sem Id, igualdade por valor
- Encapsulam regras de formato/validação do próprio dado

```csharp
public record Money
{
    public decimal Value { get; }
    public Money(decimal value)
    {
        if (value < 0) throw new DomainException("Value cannot be negative.");
        Value = value;
    }
}
```

### Domain Events
- Emitidos pelas entidades via `AddDomainEvent()`
- Disparados pelo UnitOfWork após o `CommitAsync()`
- Permitem reações entre Aggregates sem acoplamento direto

### Aggregate Root
- Entidade raiz que controla o ciclo de vida das filhas
- Só se acessa filhas através do Aggregate Root
- Um repositório por Aggregate Root

### 2-Services — Domain Services
A solution folder `2-Services` é reservada para **Domain Services** — lógica de negócio pura que não pertence a nenhuma entidade específica (ex: `TransferService`, `BudgetCalculationService`). Não confundir com `Infra.Services`.

---

## Application

### AppService
- Orquestrador: busca entidade → chama operação → commita → retorna Result
- Não contém regra de negócio
- Retorna `Result<TResponse>` sempre

```csharp
public async Task<Result<TransactionResponse>> CreateAsync(CreateTransactionRequest request, long userId)
{
    var transaction = Transaction.Create(request.Amount, request.Type, userId);
    if (transaction.IsFailed) return transaction.ToResult();

    await _transactionRepository.AddAsync(transaction.Value);
    await _uow.CommitAsync();

    return Result.Ok(MapToResponse(transaction.Value));
}
```

### Validators (FluentValidation)
- Um validator por Request
- Roda automaticamente antes do controller — sem chegar no AppService se inválido
- Retorna **todos** os erros de uma vez

### Controllers
- Herdam `BaseController`
- Sempre `[Authorize]` (exceto endpoints públicos como Login/Register que usam `[AllowAnonymous]`)
- Sempre `ActionResult<ApiResponse<T>>`
- Apenas mapeiam Result → ApiResponse, sem lógica

---

## Infra.Data

### Interfaces de repositório

As interfaces de repositório (`IRepositoryBase`, `IUnitOfWork`, `IXxxRepository`) vivem em `Financii.Infra.Data/Interfaces/Repositories/`. Application referencia Infra.Data para usá-las — isso é válido na arquitetura em camadas (Service enxerga Repository).

### Repository

**Regra absoluta: repositório só tem queries, creates, updates e deletes — nada mais.**

- `RepositoryBase<T>` fornece `Get()`, `AddAsync()`, `Update()`, `Delete()` — sem `SaveChangesAsync()`
- **Repositórios nunca retornam DTOs ou models da Application** — retornam apenas entidades do Domain
- Projeções e mapeamentos para DTO são responsabilidade exclusiva do AppService
- Métodos específicos por caso de uso no repositório concreto (ex: `GetByUserIdAsync`)
- Repositórios que usam Identity (`UserRepository`) não estendem `IRepositoryBase` — usam `UserManager<User>` diretamente no AppService

```csharp
// ✅ Correto — repositório retorna entidade
public async Task<Transaction?> GetByIdAsync(long id, long userId)
    => await Get().FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

public async Task<List<Transaction>> GetAllAsync(long userId)
    => await Get().Where(x => x.UserId == userId).ToListAsync();

// ❌ Errado — repositório mapeando para DTO
public async Task<List<TransactionResponse>> GetAllAsync(long userId)
    => await Get().Select(x => new TransactionResponse { ... }).ToListAsync();
```

### Unit of Work
- `IUnitOfWork` com `CommitAsync()`
- Após commit, dispara Domain Events pendentes

---

## Infra.Services

Serviços de **infraestrutura técnica** — sem lógica de negócio, sem acesso ao banco:

| Serviço | Responsabilidade |
|---|---|
| `JwtService` | Gera e valida tokens JWT |
| `EmailService` *(futuro)* | Envio de e-mails |
| `StorageService` *(futuro)* | Upload de arquivos |

Todos registrados via `AddScoped<IXyzService, XyzService>()` no `InfrastructureDI`.

---

## Infra.Configurations

- Registra Identity (`AddIdentityCore<User>`)
- Registra DbContext
- Registra repositórios via Scrutor (`IRepositoryBase<>`)
- Registra repositórios fora do padrão (ex: `IUserRepository`)
- Registra serviços de infraestrutura (`IJwtService`)
- Registra AppServices via Scrutor (`IAppService`)

---

## Padrões obrigatórios em todo o projeto

| O quê | Como |
|---|---|
| Validação de input | FluentValidation — antes do AppService |
| Regra de negócio | Result na entidade — método retorna `Result` |
| Orquestração | AppService — busca, chama, commita |
| Persistência atômica | UnitOfWork — um `CommitAsync()` por caso de uso |
| Resposta HTTP | `ApiResponse<T>` em todos os endpoints |
| Multi-tenancy | `GetUserId()` em todo endpoint que acessa dados |
| Testes | xUnit — entidades testadas isoladamente, AppServices com mocks |

---

## Convenções de nomenclatura

- **Idioma:** English only — todos os identificadores de código em inglês
- Projetos: `Financii.<Camada>` (ex: `Financii.Domain`)
- Namespaces seguem a estrutura de pastas
- Entities: substantivo simples (`Transaction`, `Category`, `User`)
- Value Objects: substantivo descritivo (`Money`, `DateRange`)
- Domain Events: passado (`TransactionCreatedEvent`)
- Commands/Requests: verbo + substantivo (`CreateTransactionRequest`)
- Validators: Request + Validator (`CreateTransactionRequestValidator`)
- Repositórios: entidade + Repository (`ITransactionRepository`)
- Mensagens de erro em validators: English (`"Name is required."`)
