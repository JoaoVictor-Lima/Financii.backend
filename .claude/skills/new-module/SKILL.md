# Skill: new-module

Gera um módulo completo seguindo todos os padrões do projeto Financii.

## Como invocar

```
/new-module <EntityName> [fields...]
```

**Exemplo:**
```
/new-module Transaction amount:decimal type:TransactionType description:string? date:datetime
```

---

## Antes de começar

Leia `/architecture` para garantir que os padrões estão frescos. Se tiver dúvida sobre onde cada arquivo vai, consulte lá.

**Nomenclatura:** English only — todos os identificadores, nomes de arquivo, pastas e mensagens de validator em inglês.

---

## O que este skill cria (em ordem)

---

## 1. Entidade — `Financii.Domain/Entities/<Name>.cs`

- Herda `EntityBase` (que implementa `IAggregateRoot` e `IEntity`)
- Campos com `private set` — domínio protege o próprio estado
- Construtor privado + factory method estático `Create()` que retorna `Result<<Name>>`
- Métodos de negócio retornam `Result`
- Emite Domain Events nas operações importantes

```csharp
using Financii.Domain.Contracts;
using Financii.Domain.Events.<Name>;
using FluentResults;

namespace Financii.Domain.Entities
{
    public class <Name> : EntityBase
    {
        // fields with private set
        public long UserId { get; private set; }

        private <Name>() { } // EF Core

        public static Result<<Name>> Create(/* fields */, long userId)
        {
            // domain validations
            if (/* rule */)
                return Result.Fail("Error message.");

            var entity = new <Name>
            {
                // assignments
                UserId = userId
            };

            entity.AddDomainEvent(new <Name>CreatedEvent(entity.Id));
            return Result.Ok(entity);
        }

        public Result Update(/* fields */)
        {
            // validate + mutate
            return Result.Ok();
        }
    }
}
```

---

## 2. Domain Event — `Financii.Domain/Events/<Name>/<Name>CreatedEvent.cs`

```csharp
namespace Financii.Domain.Events.<Name>
{
    public record <Name>CreatedEvent(long <Name>Id) : IDomainEvent;
}
```

Crie um evento por operação relevante (`Created`, `Updated`, `Deleted`, operações de negócio).

---

## 3. Interface do repositório — `Financii.Infra.Data/Interfaces/Repositories/I<Name>Repository.cs`

Repositório **nunca retorna DTOs ou models da Application** — apenas entidades do Domain. Projeção e mapping são responsabilidade do AppService.

```csharp
using Financii.Domain.Entities;

namespace Financii.Infra.Data.Interfaces.Repositories
{
    public interface I<Name>Repository : IRepositoryBase<<Name>>
    {
        Task<List<<Name>>> GetAllAsync(long userId);
        Task<<Name>?> GetByIdAsync(long id, long userId);
    }
}
```

---

## 4. Implementação do repositório — `Financii.Infra.Data/Repositories/<Name>Repository.cs`

Repositório só tem queries, creates, updates e deletes. Nunca retorna DTOs — só entidades. Mapping acontece no AppService.

```csharp
using Financii.Domain.Entities;
using Financii.Infra.Data.Context;
using Financii.Infra.Data.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Financii.Infra.Data.Repositories
{
    public class <Name>Repository : RepositoryBase<<Name>>, I<Name>Repository
    {
        public <Name>Repository(FinanciiDbContext context) : base(context) { }

        public async Task<List<<Name>>> GetAllAsync(long userId)
            => await Get()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

        public async Task<<Name>?> GetByIdAsync(long id, long userId)
            => await Get()
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);
    }
}
```

---

## 5. DTOs

### Request — `Financii.Application/DataTransferObject/Requests/<Name>/Create<Name>Request.cs`
```csharp
namespace Financii.Application.DataTransferObject.Requests.<Name>
{
    public class Create<Name>Request
    {
        // editable fields, no Id or UserId
    }
}
```

Crie um Request por operação (`Create`, `Update`, operações específicas).

### Response — `Financii.Application/DataTransferObject/Responses/<Name>/<Name>Response.cs`
```csharp
namespace Financii.Application.DataTransferObject.Responses.<Name>
{
    public class <Name>Response
    {
        public long Id { get; set; }
        // display fields
        public long UserId { get; set; }
    }
}
```

---

## 6. Validators (FluentValidation) — `Financii.Application/Validators/<Name>/Create<Name>RequestValidator.cs`

```csharp
using Financii.Application.DataTransferObject.Requests.<Name>;
using FluentValidation;

namespace Financii.Application.Validators.<Name>
{
    public class Create<Name>RequestValidator : AbstractValidator<Create<Name>Request>
    {
        public Create<Name>RequestValidator()
        {
            // input validations — format, required, limits
            RuleFor(x => x./* field */).NotEmpty().WithMessage("Field is required.");
        }
    }
}
```

Crie um validator por Request. Mensagens de erro em **inglês**.

---

## 7. Interface do AppService — `Financii.Application/Interfaces/AppServices/I<Name>AppService.cs`

```csharp
using Financii.Application.DataTransferObject.Requests.<Name>;
using Financii.Application.DataTransferObject.Responses.<Name>;
using Financii.Application.Interfaces.AppServices;
using FluentResults;

namespace Financii.Application.Interfaces.AppServices
{
    public interface I<Name>AppService : IAppService
    {
        Task<Result<List<<Name>Response>>> GetAllAsync(long userId);
        Task<Result<<Name>Response>> GetByIdAsync(long id, long userId);
        Task<Result<<Name>Response>> CreateAsync(Create<Name>Request request, long userId);
        Task<Result<<Name>Response>> UpdateAsync(long id, Update<Name>Request request, long userId);
        Task<Result> DeleteAsync(long id, long userId);
    }
}
```

---

## 8. AppService — `Financii.Application/AppServices/<Name>/<Name>AppService.cs`

O AppService é responsável por buscar a entidade do repositório e fazer o mapping para DTO. Repositório nunca mapeia.

```csharp
using Financii.Application.DataTransferObject.Requests.<Name>;
using Financii.Application.DataTransferObject.Responses.<Name>;
using Financii.Application.Interfaces.AppServices;
using Financii.Domain.Entities;
using Financii.Infra.Data.Interfaces.Repositories;
using FluentResults;

namespace Financii.Application.AppServices.<Name>
{
    public class <Name>AppService : I<Name>AppService
    {
        private readonly I<Name>Repository _repository;
        private readonly IUnitOfWork _uow;

        public <Name>AppService(I<Name>Repository repository, IUnitOfWork uow)
        {
            _repository = repository;
            _uow = uow;
        }

        public async Task<Result<List<<Name>Response>>> GetAllAsync(long userId)
        {
            var entities = await _repository.GetAllAsync(userId);
            return Result.Ok(entities.Select(MapToResponse).ToList());
        }

        public async Task<Result<<Name>Response>> GetByIdAsync(long id, long userId)
        {
            var entity = await _repository.GetByIdAsync(id, userId);
            if (entity is null) return Result.Fail("<Name> not found.");
            return Result.Ok(MapToResponse(entity));
        }

        public async Task<Result<<Name>Response>> CreateAsync(Create<Name>Request request, long userId)
        {
            var result = <Name>.Create(/* request fields */, userId);
            if (result.IsFailed) return result.ToResult();

            await _repository.AddAsync(result.Value);
            await _uow.CommitAsync();

            return await GetByIdAsync(result.Value.Id, userId);
        }

        public async Task<Result<<Name>Response>> UpdateAsync(long id, Update<Name>Request request, long userId)
        {
            var entity = await _repository.GetByIdAsync(id, userId);
            if (entity is null) return Result.Fail("<Name> not found.");

            var result = entity.Update(/* request fields */);
            if (result.IsFailed) return result.ToResult();

            _repository.Update(entity);
            await _uow.CommitAsync();

            return await GetByIdAsync(id, userId);
        }

        public async Task<Result> DeleteAsync(long id, long userId)
        {
            var entity = await _repository.GetByIdAsync(id, userId);
            if (entity is null) return Result.Fail("<Name> not found.");

            _repository.Delete(entity);
            await _uow.CommitAsync();
            return Result.Ok();
        }

        private static <Name>Response MapToResponse(<Name> entity) => new()
        {
            Id = entity.Id,
            // map fields
            UserId = entity.UserId
        };
    }
}
```

---

## 9. Controller — `Financii.Application/Controllers/<Name>Controller.cs`

```csharp
using Financii.Application.DataTransferObject.Requests.<Name>;
using Financii.Application.DataTransferObject.Responses;
using Financii.Application.DataTransferObject.Responses.<Name>;
using Financii.Application.Interfaces.AppServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Financii.Application.Controllers
{
    [Authorize]
    public class <Name>Controller : BaseController
    {
        private readonly I<Name>AppService _appService;

        public <Name>Controller(I<Name>AppService appService)
        {
            _appService = appService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<<Name>Response>>>> GetAll()
            => HandleResult(await _appService.GetAllAsync(GetUserId()));

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<<Name>Response>>> GetById(long id)
            => HandleResult(await _appService.GetByIdAsync(id, GetUserId()));

        [HttpPost]
        public async Task<ActionResult<ApiResponse<<Name>Response>>> Create([FromBody] Create<Name>Request request)
            => HandleResult(await _appService.CreateAsync(request, GetUserId()), successStatusCode: 201);

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<<Name>Response>>> Update(long id, [FromBody] Update<Name>Request request)
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
public DbSet<<Name>> <Name>s { get; set; }
```

O `FinanciiDbContext` importa entidades de `Financii.Domain.Entities` — o using já deve estar presente.

---

## 11. Testes — `Financii.Tests/Domain/<Name>Tests.cs`

```csharp
using Financii.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Financii.Tests.Domain
{
    public class <Name>Tests
    {
        [Fact]
        public void Create_WithValidData_ShouldSucceed()
        {
            var result = <Name>.Create(/* valid data */, userId: 1);
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void Create_WithInvalidData_ShouldFail()
        {
            var result = <Name>.Create(/* invalid data */, userId: 1);
            result.IsFailed.Should().BeTrue();
            result.Errors.Should().ContainSingle(e => e.Message == "expected error message");
        }

        // one test per business rule
    }
}
```

---

## 12. Após criar os arquivos

Execute `/add-migration Add<Name>` para criar e aplicar a migration.
Execute `/review-module <Name>` para validar que tudo está conforme os padrões.
