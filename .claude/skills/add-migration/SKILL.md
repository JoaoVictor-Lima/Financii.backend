# Skill: add-migration

Automatiza o fluxo completo de criação e aplicação de migrations do Entity Framework Core.

## Como invocar

```
/add-migration <NomeDaMigration>
```

**Exemplo:**
```
/add-migration AddTransaction
```

---

## Fluxo de execução

Execute os passos **em ordem**, parando imediatamente se qualquer um falhar.

### Passo 1 — Build

```bash
cd Financii.Api && dotnet build
```

- Se falhar: exiba os erros de compilação e **não prossiga**.

### Passo 2 — Criar migration

```bash
dotnet ef migrations add <NomeDaMigration> --project ../Financii.Infra.Data --startup-project .
```

Executar a partir de `Financii.Api/`.

### Passo 3 — Revisar migration gerada

Leia o arquivo gerado em `Financii.Infra.Data/Migrations/` e exiba um resumo do que será alterado (tabelas, colunas, índices, FKs).

Peça confirmação antes de aplicar:
> "Migration gerada. Deseja aplicar ao banco agora?"

### Passo 4 — Aplicar migration (após confirmação)

```bash
dotnet ef database update --project ../Financii.Infra.Data --startup-project .
```

---

## Comandos de referência

```bash
# Listar migrations
dotnet ef migrations list --project ../Financii.Infra.Data --startup-project .

# Reverter para migration específica
dotnet ef database update <NomeDaMigration> --project ../Financii.Infra.Data --startup-project .

# Remover última migration (apenas se não aplicada ao banco)
dotnet ef migrations remove --project ../Financii.Infra.Data --startup-project .
```

---

## Notas

- Todos os comandos `dotnet ef` executar a partir de `Financii.Api/`
- Nunca remover migration já aplicada ao banco — criar nova para reverter
- Se `dotnet-ef` não encontrado: `dotnet tool install --global dotnet-ef`
