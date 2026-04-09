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

## Estrutura de Camadas (Clean Architecture)

```
Financii.Domain/                — núcleo, sem dependências externas
Financii.Application/           — casos de uso, DTOs, validators, controllers
Financii.Infra.Data/            — EF Core, repositórios, migrations
Financii.Infra.Configurations/  — DI, configurações de infraestrutura
Financii.Api/                   — entry point (Program.cs, appsettings)
Financii.Tests/                 — testes unitários xUnit
```

**Regra de dependência:** Domain não conhece ninguém. Application conhece Domain. Infra conhece Domain. Api conhece todos.

---

## Domain (DDD)

### Entidades
- Implementam `IEntity` (`long Id`)
- Têm **comportamento** — métodos que executam operações e retornam `Result`
- Nunca são anêmicas (sem setters públicos para campos de negócio)
- Protegem seu próprio estado

```csharp
public class Transaction : IEntity
{
    public long Id { get; private set; }
    public Money Amount { get; private set; }
    public TransactionType Type { get; private set; }

    public Result Categorize(long categoryId)
    {
        if (categoryId <= 0)
            return Result.Fail("Categoria inválida.");
        CategoryId = categoryId;
        AddDomainEvent(new TransactionCategorizedEvent(Id, categoryId));
        return Result.Ok();
    }
}
```

### Value Objects
- Imutáveis, sem Id, igualdade por valor
- Encapsulam regras de formato/validação do próprio dado

```csharp
public record Money
{
    public decimal Value { get; }
    public Money(decimal value)
    {
        if (value < 0) throw new DomainException("Valor não pode ser negativo.");
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
- Sempre `[Authorize]`
- Sempre `ActionResult<ApiResponse<T>>`
- Apenas mapeiam Result → ApiResponse, sem lógica

---

## Infra.Data

### Repository
- `RepositoryBase<T>` sem `SaveChangesAsync()` — quem salva é o UoW
- **Leitura:** retorna projeção com campos necessários (`.Select()`)
- **Escrita:** retorna entidade completa (para o domínio operar)
- Métodos específicos por caso de uso no repositório concreto

### Unit of Work
- `IUnitOfWork` com `CommitAsync()` e `RollbackAsync()`
- Após commit, dispara Domain Events pendentes

---

## Infra.Configurations

### DI (Scrutor)
- Convenção: tudo que implementa `IAppService` → Scoped
- Convenção: tudo que implementa `IRepository` → Scoped
- Sem registro manual de classes individuais

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

- Projetos: `Financii.<Camada>` (ex: `Financii.Domain`)
- Namespaces seguem a estrutura de pastas
- Entities: substantivo simples (`Transaction`, `Category`)
- Value Objects: substantivo descritivo (`Money`, `DateRange`)
- Domain Events: passado (`TransactionCreatedEvent`)
- Commands/Requests: verbo + substantivo (`CreateTransactionRequest`)
- Validators: Request + Validator (`CreateTransactionRequestValidator`)
- Repositórios: entidade + Repository (`ITransactionRepository`)
