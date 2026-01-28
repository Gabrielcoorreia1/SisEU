# Solução para Erro: Table 'siseus.Usuarios' doesn't exist

## Problema
O erro ocorre porque as migrations não foram aplicadas corretamente ao banco de dados.

## Soluções

### Solução 1: Recriar o banco de dados (Recomendado para desenvolvimento)

#### No MySQL Workbench ou linha de comando:
```sql
DROP DATABASE IF EXISTS siseus;
CREATE DATABASE siseus;
```

#### Depois, no terminal do projeto:
```bash
cd src/SisEUs.API
dotnet ef database update
```

### Solução 2: Aplicar migrations manualmente

#### No terminal do projeto:
```bash
cd src/SisEUs.API
dotnet ef database update
```

### Solução 3: Limpar e reconstruir migrations (se o problema persistir)

#### 1. Remover o banco existente:
```sql
DROP DATABASE IF EXISTS siseus;
CREATE DATABASE siseus;
```

#### 2. Verificar migrations disponíveis:
```bash
cd src/SisEUs.API
dotnet ef migrations list
```

#### 3. Aplicar todas as migrations:
```bash
dotnet ef database update
```

### Solução 4: Recriar migrations do zero (último recurso)

**?? ATENÇÃO: Isso remove todas as migrations existentes!**

#### 1. Remover pasta de migrations:
```bash
Remove-Item -Recurse -Force src\SisEUs.Infrastructure\Migrations
```

#### 2. Criar nova migration inicial:
```bash
cd src/SisEUs.API
dotnet ef migrations add InitialCreate --project ..\SisEUs.Infrastructure\SisEUs.Infrastructure.csproj
```

#### 3. Aplicar ao banco:
```bash
dotnet ef database update
```

## Verificar se o problema foi resolvido

Após aplicar uma das soluções, execute a aplicação:

```bash
cd src/SisEUs.API
dotnet run
```

A aplicação deve iniciar sem erros e aplicar o seed automaticamente se o banco estiver vazio.

## Logs úteis

Ao executar a aplicação, você verá logs informativos:
- ? "Conexão estabelecida com sucesso!"
- ? "Encontradas X migrations pendentes"
- ? "Migrations aplicadas com sucesso!"
- ? "Seed concluído com sucesso!"

## Comandos úteis do EF Core

```bash
# Listar migrations
dotnet ef migrations list

# Ver SQL de uma migration
dotnet ef migrations script

# Remover última migration (se não aplicada)
dotnet ef migrations remove

# Aplicar migration específica
dotnet ef database update <NomeDaMigration>

# Ver informações do contexto
dotnet ef dbcontext info
```

## Mais informações

Se o problema persistir, verifique:
1. A string de conexão está correta no `appsettings.json`
2. O MySQL está rodando
3. O usuário tem permissões adequadas
4. As migrations estão na pasta `src/SisEUs.Infrastructure/Migrations`
