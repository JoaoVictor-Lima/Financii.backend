# Skill: check-pattern

Audita o código do projeto para verificar conformidade com os padrões do Financii.

## Como invocar

```
/check-pattern
/check-pattern <NomeModulo>    # audita apenas um módulo específico
```

---

## O que auditar

### Domain — Entidades

| # | Regra |
|---|-------|
| 1 | Herda `EntityBase` |
| 2 | Campos com `private set` — sem setters públicos para dados de negócio |
| 3 | Construtor privado (para EF) + factory `Create()` estático |
| 4 | `Create()` retorna `Result<T>` |
| 5 | Métodos de negócio retornam `Result` |
| 6 | Domain Events emitidos nas operações relevantes |
| 7 | Sem regras de persistência ou infraestrutura |

### Domain — Value Objects

| # | Regra |
|---|-------|
| 1 | Declarados como `record` |
| 2 | Imutáveis |
| 3 | Validam seu próprio dado no construtor via `DomainException` |

### Application — Validators

| # | Regra |
|---|-------|
| 1 | Um validator por Request |
| 2 | Herda `AbstractValidator<TRequest>` |
| 3 | Localizado em `Financii.Application/Validators/<Modulo>/` |

### Application — AppServices

| # | Regra |
|---|-------|
| 1 | Implementa `IAppService` (para Scrutor) |
| 2 | Retorna `Result<T>` ou `Result` sempre |
| 3 | Sem regra de negócio — chama método da entidade |
| 4 | Chama `_uow.CommitAsync()` uma única vez por operação de escrita |
| 5 | Operações de leitura usam método projetado do repositório |
| 6 | Operações de escrita buscam entidade completa via `GetByIdAsync` |

### Application — Controllers

| # | Regra |
|---|-------|
| 1 | Herda `BaseController` |
| 2 | Tem `[Authorize]` na classe |
| 3 | Retorna `ActionResult<ApiResponse<T>>` |
| 4 | Usa `HandleResult()` para mapear Result → ApiResponse |
| 5 | Chama `GetUserId()` em todo endpoint que acessa dados |
| 6 | Sem lógica de negócio — apenas delega ao AppService |

### Infra — Repositórios

| # | Regra |
|---|-------|
| 1 | Herda `RepositoryBase<T>` |
| 2 | Implementa `IRepositoryBase<T>` via interface específica |
| 3 | Sem `SaveChangesAsync()` — quem salva é o UoW |
| 4 | Métodos de leitura usam `.Select()` com projeção para Response |
| 5 | Métodos de escrita retornam entidade completa (sem projeção) |

### Infra — DI (Scrutor)

| # | Regra |
|---|-------|
| 1 | Nenhum AppService registrado manualmente |
| 2 | Nenhum repositório registrado manualmente |
| 3 | `IUnitOfWork` registrado explicitamente (é caso especial) |

---

## Formato do relatório

```
## Resultado: check-pattern [<Modulo>]

### ✅ Conformes
- [lista]

### ⚠️ Violações

#### <NomeArquivo>
- [Regra N]: descrição do problema
  Sugestão: como corrigir

### 📋 DI / Scrutor
- [OK ou itens faltando]
```

Se tudo conforme:
```
✅ Todos os padrões estão sendo seguidos corretamente.
```
