# Skill: new-module

Gera um módulo completo seguindo todos os padrões do projeto Financii.

## Como invocar

```
/new-module <NomeEntidade> [campos...]
```

**Exemplo:**
```
/new-module Transaction amount:decimal type:TransactionType description:string? date:datetime
```

---

## Antes de começar

Leia `/architecture` para garantir que os padrões estão frescos. Se tiver dúvida sobre onde cada arquivo vai, consulte lá.

---

## O que este skill cria (em ordem)

---

## 1. Entidade — `Financii.Domain/Entities/<Nome>.cs`

- Herda `EntityBase` (que implementa `IAggregateRoot` e `IEntity`)
- Campos com `private set` — domínio protege o próprio estado
- Construtor privado + factory method estático `Create()` que retorna `Result<<Nome>>`
- Métodos de negócio retornam `Result`
- Emite Domain Events nas operações importantes

```csharp
using Financii.Domain.Contracts;
using Financii.Domain.Events.<Nome>;
using FluentResults;

namespace Financii.Domain.Entities
{
    public class <Nome> : EntityBase
    {
        // campos com private set
        public long UserId { get; private set; }

        private <Nome>() { } // EF Core

        public static Result<<Nome>> Create(/* campos */, long userId)
        {
            // validações de domínio
            if (/* regra */)
                return Result.Fail("Mensagem em português.");

            var entity = new <Nome>
            {
                // atribuições
                UserId = userId
            };

            entity.AddDomainEvent(new <Nome>CreatedEvent(entity.Id));
            return Result.Ok(entity);
        }

        public Result Update(/* campos */)
        {
            // validação + mutação
            return Result.Ok();
        }
    }
}
```

---

## 2. Domain Event — `Financii.Domain/Events/<Nome>/<Nome>CreatedEvent.cs`

```csharp
using Financii.Domain.Events;

namespace Financii.Domain.Events.<Nome>
{
    public record <Nome>CreatedEvent(long <Nome>Id) : IDomainEvent;
}
```

Crie um evento por operação relevante (`Created`, `Updated`, `Deleted`, operações de negócio).

---

## 3. Interface do repositório — `Financii.Application/Interfaces/Repositories/I<Nome>Repository.cs`

```csharp
using Financii.Application.DataTransferObject.Responses.<Nome>;
using Financii.Domain.Entities;

namespace Financii.Application.Interfaces.Repositories
{
    public interface I<Nome>Repository : IRepositoryBase<<Nome>>
    {
        // Leitura: projeção com campos necessários
        Task<List<<Nome>Response>> GetAllAsync(long userId);
        Task<<Nome>Response?> GetProjectedByIdAsync(long id, long userId);

        // Escrita: entidade completa para o domínio operar
        Task<<Nome>?> GetByIdAsync(long id, long userId);
    }
}
```

---

## 4. Implementação do repositório — `Financii.Infra.Data/Repositories/<Nome>Repository.cs`

```csharp
using Financii.Application.DataTransferObject.Responses.<Nome>;
using Financii.Application.Interfaces.Repositories;
using Financii.Domain.Entities;
using Financii.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Financii.Infra.Data.Repositories
{
    public class <Nome>Repository : RepositoryBase<<Nome>>, I<Nome>Repository
    {
        public <Nome>Repository(FinanciiDbContext context) : base(context) { }

        // Leitura: Select com campos necessários — sem Include desnecessário
        public async Task<List<<Nome>Response>> GetAllAsync(long userId)
            => await Get()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Id)
                .Select(x => new <Nome>Response
                {
                    Id = x.Id,
                    // mapear campos
                    UserId = x.UserId
                })
                .ToListAsync();

        public async Task<<Nome>Response?> GetProjectedByIdAsync(long id, long userId)
            => await Get()
                .Where(x => x.Id == id && x.UserId == userId)
                .Select(x => new <Nome>Response
                {
                    Id = x.Id,
                    // mapear campos
                    UserId = x.UserId
                })
                .FirstOrDefaultAsync();

        // Escrita: entidade completa para o domínio modificar
        public async Task<<Nome>?> GetByIdAsync(long id, long userId)
            => await Get()
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
    }
}
```

---

## 5. DTOs

### Request — `Financii.Application/DataTransferObject/Requests/<Nome>/Create<Nome>Request.cs`
```csharp
namespace Financii.Application.DataTransferObject.Requests.<Nome>
{
    public class Create<Nome>Request
    {
        // campos editáveis, sem Id e UserId
    }
}
```

Crie um Request por operação (`Create`, `Update`, operações específicas).

### Response — `Financii.Application/DataTransferObject/Responses/<Nome>/<Nome>Response.cs`
```csharp
namespace Financii.Application.DataTransferObject.Responses.<Nome>
{
    public class <Nome>Response
    {
        public long Id { get; set; }
        // campos para exibição
        public long UserId { get; set; }
    }
}
```

---

## 6. Validators (FluentValidation) — `Financii.Application/Validators/<Nome>/Create<Nome>RequestValidator.cs`

```csharp
using Financii.Application.DataTransferObject.Requests.<Nome>;
using FluentValidation;

namespace Financii.Application.Validators.<Nome>
{
    public class Create<Nome>RequestValidator : AbstractValidator<Create<Nome>Request>
    {
        public Create<Nome>RequestValidator()
        {
            // validações de input — formato, obrigatoriedade, limites
            RuleFor(x => x./* campo */).NotEmpty().WithMessage("Campo obrigatório.");
        }
    }
}
```

Crie um validator por Request.

---

## 7. Interface do AppService — `Financii.Application/Interfaces/AppServices/I<Nome>AppService.cs`

```csharp
using Financii.Application.DataTransferObject.Requests.<Nome>;
using Financii.Application.DataTransferObject.Responses.<Nome>;
using Financii.Application.Interfaces.AppServices;
using FluentResults;

namespace Financii.Application.Interfaces.AppServices
{
    public interface I<Nome>AppService : IAppService
    {
        Task<Result<List<<Nome>Response>>> GetAllAsync(long userId);
        Task<Result<<Nome>Response>> GetByIdAsync(long id, long userId);
        Task<Result<<Nome>Response>> CreateAsync(Create<Nome>Request request, long userId);
        Task<Result<<Nome>Response>> UpdateAsync(long id, Update<Nome>Request request, long userId);
        Task<Result> DeleteAsync(long id, long userId);
    }
}
```

---

## 8. AppService — `Financii.Application/AppServices/<Nome>/<Nome>AppService.cs`

```csharp
using Financii.Application.DataTransferObject.Requests.<Nome>;
using Financii.Application.DataTransferObject.Responses.<Nome>;
using Financii.Application.Interfaces.AppServices;
using Financii.Application.Interfaces.Repositories;
using Financii.Domain.Entities;
using FluentResults;

namespace Financii.Application.AppServices.<Nome>
{
    public class <Nome>AppService : I<Nome>AppService
    {
        private readonly I<Nome>Repository _repository;
        private readonly IUnitOfWork _uow;

        public <Nome>AppService(I<Nome>Repository repository, IUnitOfWork uow)
        {
            _repository = repository;
            _uow = uow;
        }

        public async Task<Result<List<<Nome>Response>>> GetAllAsync(long userId)
            => Result.Ok(await _repository.GetAllAsync(userId));

        public async Task<Result<<Nome>Response>> GetByIdAsync(long id, long userId)
        {
            var item = await _repository.GetProjectedByIdAsync(id, userId);
            if (item is null) return Result.Fail("<Nome> não encontrado(a).");
            return Result.Ok(item);
        }

        public async Task<Result<<Nome>Response>> CreateAsync(Create<Nome>Request request, long userId)
        {
            var result = <Nome>.Create(/* request campos */, userId);
            if (result.IsFailed) return result.ToResult();

            await _repository.AddAsync(result.Value);
            await _uow.CommitAsync();

            return await GetByIdAsync(result.Value.Id, userId);
        }

        public async Task<Result<<Nome>Response>> UpdateAsync(long id, Update<Nome>Request request, long userId)
        {
            var entity = await _repository.GetByIdAsync(id, userId);
            if (entity is null) return Result.Fail("<Nome> não encontrado(a).");

            var result = entity.Update(/* request campos */);
            if (result.IsFailed) return result.ToResult();

            _repository.Update(entity);
            await _uow.CommitAsync();

            return await GetByIdAsync(id, userId);
        }

        public async Task<Result> DeleteAsync(long id, long userId)
        {
            var entity = await _repository.GetByIdAsync(id, userId);
            if (entity is null) return Result.Fail("<Nome> não encontrado(a).");

            _repository.Delete(entity);
            await _uow.CommitAsync();
            return Result.Ok();
        }
    }
}
```

---

## 9. Controller — `Financii.Application/Controllers/<Nome>Controller.cs`

```csharp
using Financii.Application.DataTransferObject.Requests.<Nome>;
using Financii.Application.DataTransferObject.Responses;
using Financii.Application.DataTransferObject.Responses.<Nome>;
using Financii.Application.Interfaces.AppServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Financii.Application.Controllers
{
    [Authorize]
    public class <Nome>Controller : BaseController
    {
        private readonly I<Nome>AppService _appService;

        public <Nome>Controller(I<Nome>AppService appService)
        {
            _appService = appService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<<Nome>Response>>>> GetAll()
            => HandleResult(await _appService.GetAllAsync(GetUserId()));

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<<Nome>Response>>> GetById(long id)
            => HandleResult(await _appService.GetByIdAsync(id, GetUserId()));

        [HttpPost]
        public async Task<ActionResult<ApiResponse<<Nome>Response>>> Create([FromBody] Create<Nome>Request request)
            => HandleResult(await _appService.CreateAsync(request, GetUserId()), successStatusCode: 201);

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<<Nome>Response>>> Update(long id, [FromBody] Update<Nome>Request request)
            => HandleResult(await _appService.UpdateAsync(id, request, GetUserId()));

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(long id)
            => HandleResult(await _appService.DeleteAsync(id, GetUserId()));
    }
}
```

---

## 10. DbSet no DbContext

Adicione em `Financii.Infra.Data/Context/FinanciiDbContext.cs`:

```csharp
public DbSet<<Nome>> <Nome>s { get; set; }
```

---

## 11. Testes — `Financii.Tests/Domain/<Nome>Tests.cs`

```csharp
using Financii.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Financii.Tests.Domain
{
    public class <Nome>Tests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var result = <Nome>.Create(/* dados válidos */, userId: 1);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Create_WithInvalidData_ShouldFail()
        {
            var result = <Nome>.Create(/* dados inválidos */, userId: 1);
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message == "mensagem esperada");
        }

        // teste por regra de negócio da entidade
    }
}
```

---

## 12. Após criar os arquivos

Execute `/add-migration Add<Nome>` para criar e aplicar a migration.
Execute `/review-module <Nome>` para validar que tudo está conforme os padrões.
