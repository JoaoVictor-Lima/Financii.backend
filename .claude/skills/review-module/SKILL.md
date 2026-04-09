# Skill: review-module

Valida que um módulo está completamente implementado e pronto antes de considerá-lo concluído.

## Como invocar

```
/review-module <NomeModulo>
```

**Exemplo:**
```
/review-module Transaction
```

---

## Checklist de completude

Execute cada verificação lendo os arquivos correspondentes.

### Arquivos que devem existir

```
Financii.Domain/
  Entities/<Nome>.cs                              ← entidade com comportamento
  Events/<Nome>/<Nome>CreatedEvent.cs             ← ao menos um evento

Financii.Application/
  DataTransferObject/Requests/<Nome>/Create<Nome>Request.cs
  DataTransferObject/Requests/<Nome>/Update<Nome>Request.cs
  DataTransferObject/Responses/<Nome>/<Nome>Response.cs
  Interfaces/Repositories/I<Nome>Repository.cs
  Interfaces/AppServices/I<Nome>AppService.cs
  AppServices/<Nome>/<Nome>AppService.cs
  Controllers/<Nome>Controller.cs
  Validators/<Nome>/Create<Nome>RequestValidator.cs
  Validators/<Nome>/Update<Nome>RequestValidator.cs

Financii.Infra.Data/
  Repositories/<Nome>Repository.cs

Financii.Tests/
  Domain/<Nome>Tests.cs                          ← testes das regras da entidade
```

---

### Verificações de qualidade

**Entidade**
- [ ] Campos com `private set`
- [ ] `Create()` retorna `Result<<Nome>>`
- [ ] Ao menos um Domain Event emitido
- [ ] Regras de negócio testadas em `<Nome>Tests.cs`

**Repositório**
- [ ] `GetAllAsync()` usa `.Select()` com projeção
- [ ] `GetByIdAsync()` retorna entidade completa
- [ ] Sem `SaveChangesAsync()`

**AppService**
- [ ] Retorna `Result<T>` em todos os métodos
- [ ] Operações de escrita chamam `_uow.CommitAsync()` uma vez
- [ ] Implementa `IAppService`

**Controller**
- [ ] `[Authorize]` presente
- [ ] Usa `HandleResult()` em todos os endpoints
- [ ] Sem try/catch — erros tratados via Result

**Validators**
- [ ] Existe validator para Create e Update
- [ ] Valida campos obrigatórios, formatos e limites

**DbContext**
- [ ] DbSet adicionado em `FinanciiDbContext`

**Migration**
- [ ] Migration criada e aplicada ao banco

---

## Formato do relatório

```
## Review: <NomeModulo>

### ✅ Pronto
- [itens completos]

### ❌ Pendente
- [arquivo ou verificação faltando]
  Ação: o que fazer para resolver

### 📊 Status
Pronto para uso: SIM / NÃO
```
