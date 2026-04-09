# Skill: setup-project

Gera a estrutura base completa do projeto Financii do zero, seguindo todos os padrões definidos em `/architecture`.

## Como invocar

```
/setup-project
```

Executar **uma única vez**, no início do projeto, com o diretório raiz vazio.

---

## O que este skill cria

Execute **em ordem**, sem pular etapas.

---

## Passo 1 — Criar a solução e projetos

```bash
dotnet new sln -n Financii
dotnet new classlib -n Financii.Domain
dotnet new classlib -n Financii.Application
dotnet new classlib -n Financii.Infra.Data
dotnet new classlib -n Financii.Infra.Configurations
dotnet new webapi -n Financii.Api
dotnet new xunit -n Financii.Tests

dotnet sln add Financii.Domain/Financii.Domain.csproj
dotnet sln add Financii.Application/Financii.Application.csproj
dotnet sln add Financii.Infra.Data/Financii.Infra.Data.csproj
dotnet sln add Financii.Infra.Configurations/Financii.Infra.Configurations.csproj
dotnet sln add Financii.Api/Financii.Api.csproj
dotnet sln add Financii.Tests/Financii.Tests.csproj
```

---

## Passo 2 — Referências entre projetos

```bash
# Application conhece Domain
dotnet add Financii.Application/Financii.Application.csproj reference Financii.Domain/Financii.Domain.csproj

# Infra.Data conhece Domain
dotnet add Financii.Infra.Data/Financii.Infra.Data.csproj reference Financii.Domain/Financii.Domain.csproj

# Infra.Configurations conhece todos
dotnet add Financii.Infra.Configurations/Financii.Infra.Configurations.csproj reference Financii.Application/Financii.Application.csproj
dotnet add Financii.Infra.Configurations/Financii.Infra.Configurations.csproj reference Financii.Infra.Data/Financii.Infra.Data.csproj

# Api conhece Configurations e Application
dotnet add Financii.Api/Financii.Api.csproj reference Financii.Infra.Configurations/Financii.Infra.Configurations.csproj
dotnet add Financii.Api/Financii.Api.csproj reference Financii.Application/Financii.Application.csproj

# Tests conhece Domain e Application
dotnet add Financii.Tests/Financii.Tests.csproj reference Financii.Domain/Financii.Domain.csproj
dotnet add Financii.Tests/Financii.Tests.csproj reference Financii.Application/Financii.Application.csproj
```

---

## Passo 3 — Instalar pacotes NuGet

```bash
# Infra.Data
dotnet add Financii.Infra.Data/Financii.Infra.Data.csproj package Microsoft.EntityFrameworkCore
dotnet add Financii.Infra.Data/Financii.Infra.Data.csproj package Microsoft.EntityFrameworkCore.SqlServer
dotnet add Financii.Infra.Data/Financii.Infra.Data.csproj package Microsoft.EntityFrameworkCore.Tools

# Application
dotnet add Financii.Application/Financii.Application.csproj package FluentValidation.AspNetCore
dotnet add Financii.Application/Financii.Application.csproj package FluentResults

# Domain
dotnet add Financii.Domain/Financii.Domain.csproj package FluentResults

# Api
dotnet add Financii.Api/Financii.Api.csproj package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add Financii.Api/Financii.Api.csproj package Microsoft.AspNetCore.Identity.EntityFrameworkCore
dotnet add Financii.Api/Financii.Api.csproj package Swashbuckle.AspNetCore

# Infra.Configurations
dotnet add Financii.Infra.Configurations/Financii.Infra.Configurations.csproj package Scrutor
dotnet add Financii.Infra.Configurations/Financii.Infra.Configurations.csproj package Microsoft.EntityFrameworkCore.Design

# Tests
dotnet add Financii.Tests/Financii.Tests.csproj package Moq
dotnet add Financii.Tests/Financii.Tests.csproj package FluentAssertions
dotnet add Financii.Tests/Financii.Tests.csproj package FluentResults
```

---

## Passo 4 — Estrutura de pastas

Crie as pastas abaixo (sem arquivos ainda, apenas a estrutura):

```
Financii.Domain/
  Contracts/
  Entities/
  Enums/
  Events/
  Exceptions/
  ValueObjects/

Financii.Application/
  Controllers/
  AppServices/
  DataTransferObject/
    Requests/
    Responses/
  Interfaces/
    AppServices/
    Repositories/
  Validators/

Financii.Infra.Data/
  Context/
  Migrations/
  Repositories/

Financii.Infra.Configurations/
  DependencyInjection/

Financii.Tests/
  Domain/
  Application/
```

---

## Passo 5 — Arquivos base do Domain

### `Financii.Domain/Contracts/IEntity.cs`
```csharp
namespace Financii.Domain.Contracts
{
    public interface IEntity
    {
        long Id { get; }
    }
}
```

### `Financii.Domain/Contracts/IAggregateRoot.cs`
```csharp
namespace Financii.Domain.Contracts
{
    public interface IAggregateRoot : IEntity
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void ClearDomainEvents();
    }
}
```

### `Financii.Domain/Events/IDomainEvent.cs`
```csharp
namespace Financii.Domain.Events
{
    public interface IDomainEvent { }
}
```

### `Financii.Domain/Exceptions/DomainException.cs`
```csharp
namespace Financii.Domain.Exceptions
{
    public class DomainException : Exception
    {
        public DomainException(string message) : base(message) { }
    }
}
```

### `Financii.Domain/Contracts/EntityBase.cs`
```csharp
using Financii.Domain.Events;

namespace Financii.Domain.Contracts
{
    public abstract class EntityBase : IAggregateRoot
    {
        public long Id { get; protected set; }

        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent)
            => _domainEvents.Add(domainEvent);

        public void ClearDomainEvents()
            => _domainEvents.Clear();
    }
}
```

---

## Passo 6 — Arquivos base da Application

### `Financii.Application/DataTransferObject/Responses/ApiResponse.cs`
```csharp
namespace Financii.Application.DataTransferObject.Responses
{
    public class ApiResponse<T>
    {
        public bool IsSuccessful { get; set; }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ApiResponse<T> Success(T data, string message = "Operação realizada com sucesso.", int statusCode = 200)
            => new() { IsSuccessful = true, StatusCode = statusCode, Message = message, Data = data };

        public static ApiResponse<T> Failure(string message, int statusCode = 400, List<string> errors = null)
            => new() { IsSuccessful = false, StatusCode = statusCode, Message = message, Errors = errors ?? new() };
    }
}
```

### `Financii.Application/Controllers/BaseController.cs`
```csharp
using Financii.Application.DataTransferObject.Responses;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Financii.Application.Controllers
{
    [ApiController]
    [Route("Api/v1/[controller]/[action]")]
    public abstract class BaseController : ControllerBase
    {
        protected long GetUserId()
        {
            var claim = User.FindFirst("id")?.Value;
            if (!long.TryParse(claim, out var userId))
                throw new Exception("Usuário inválido.");
            return userId;
        }

        protected ActionResult<ApiResponse<T>> HandleResult<T>(Result<T> result, int successStatusCode = 200)
        {
            if (result.IsSuccess)
                return Ok(ApiResponse<T>.Success(result.Value, statusCode: successStatusCode));

            var errors = result.Errors.Select(e => e.Message).ToList();
            return BadRequest(ApiResponse<T>.Failure("Operação não permitida.", errors: errors));
        }

        protected ActionResult<ApiResponse<object>> HandleResult(Result result)
        {
            if (result.IsSuccess)
                return Ok(ApiResponse<object>.Success(null));

            var errors = result.Errors.Select(e => e.Message).ToList();
            return BadRequest(ApiResponse<object>.Failure("Operação não permitida.", errors: errors));
        }
    }
}
```

---

## Passo 7 — Interfaces base de repositório

### `Financii.Application/Interfaces/Repositories/IRepositoryBase.cs`
```csharp
using Financii.Domain.Contracts;

namespace Financii.Application.Interfaces.Repositories
{
    public interface IRepositoryBase<TEntity> where TEntity : class, IEntity
    {
        IQueryable<TEntity> Get();
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
```

### `Financii.Application/Interfaces/Repositories/IUnitOfWork.cs`
```csharp
namespace Financii.Application.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        Task CommitAsync();
    }
}
```

---

## Passo 8 — RepositoryBase e UnitOfWork

### `Financii.Infra.Data/Repositories/RepositoryBase.cs`
```csharp
using Financii.Application.Interfaces.Repositories;
using Financii.Domain.Contracts;
using Financii.Infra.Data.Context;

namespace Financii.Infra.Data.Repositories
{
    public class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
        where TEntity : class, IEntity
    {
        protected readonly FinanciiDbContext _context;

        public RepositoryBase(FinanciiDbContext context)
        {
            _context = context;
        }

        public IQueryable<TEntity> Get() => _context.Set<TEntity>();

        public async Task AddAsync(TEntity entity)
            => await _context.Set<TEntity>().AddAsync(entity);

        public void Update(TEntity entity)
            => _context.Set<TEntity>().Update(entity);

        public void Delete(TEntity entity)
            => _context.Set<TEntity>().Remove(entity);
    }
}
```

### `Financii.Infra.Data/Repositories/UnitOfWork.cs`
```csharp
using Financii.Application.Interfaces.Repositories;
using Financii.Infra.Data.Context;

namespace Financii.Infra.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FinanciiDbContext _context;

        public UnitOfWork(FinanciiDbContext context)
        {
            _context = context;
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
            // TODO: disparar Domain Events após commit
        }
    }
}
```

---

## Passo 9 — DbContext base

### `Financii.Infra.Data/Context/FinanciiDbContext.cs`
```csharp
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Financii.Infra.Data.Context
{
    public class FinanciiDbContext : IdentityDbContext<User, Role, long>
    {
        public FinanciiDbContext(DbContextOptions<FinanciiDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
```

---

## Passo 10 — DI com Scrutor

### `Financii.Infra.Configurations/DependencyInjection/InfrastructureDI.cs`
```csharp
using Financii.Application.Interfaces.Repositories;
using Financii.Infra.Data.Context;
using Financii.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Financii.Infra.Configurations.DependencyInjection
{
    public static class InfrastructureDI
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FinanciiDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repositórios — convenção: tudo que implementa IRepositoryBase
            services.Scan(scan => scan
                .FromAssemblyOf<UnitOfWork>()
                .AddClasses(c => c.AssignableTo(typeof(IRepositoryBase<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }

        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // AppServices — convenção: tudo que implementa IAppService
            services.Scan(scan => scan
                .FromAssemblyOf<BaseController>()
                .AddClasses(c => c.AssignableTo<IAppService>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}
```

---

## Passo 11 — Interfaces marcadoras para Scrutor

### `Financii.Application/Interfaces/AppServices/IAppService.cs`
```csharp
namespace Financii.Application.Interfaces.AppServices
{
    // Interface marcadora — todo AppService implementa esta para registro automático via Scrutor
    public interface IAppService { }
}
```

---

## Passo 12 — Program.cs

Configure o entry point com autenticação JWT, Swagger com suporte a Bearer token e FluentValidation:

### `Financii.Api/Program.cs`
```csharp
using Financii.Infra.Configurations.DependencyInjection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Controllers + FluentValidation automático
builder.Services.AddControllers();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<BaseController>();

// Swagger com suporte a JWT Bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Financii API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe: Bearer {seu token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"];
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
    };
});

builder.Services.AddAuthorization();

// Infraestrutura e AppServices (Scrutor)
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Financii API v1");
    c.RoutePrefix = string.Empty; // Swagger na raiz
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

### `Financii.Api/appsettings.json` — adicionar seção JWT
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=FinanciiDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "sua-chave-secreta-minimo-32-caracteres-aqui",
    "Issuer": "Financii",
    "Audience": "Financii"
  }
}
```

---

## Passo 13 — Verificação final

```bash
dotnet build
```

Se compilar sem erros, a estrutura base está pronta. Execute `/new-module` para criar o primeiro módulo.
