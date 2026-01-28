# ?? Guia Rápido: Resolver Erro de Tabelas não Encontradas

## ?? Solução Rápida (Escolha uma)

### ? Opção 1: Usar o Script Automático (Recomendado)

#### Windows (PowerShell):
```powershell
.\reset-database.ps1
```

#### Linux/Mac (Bash):
```bash
chmod +x reset-database.sh
./reset-database.sh
```

### ? Opção 2: Manual no MySQL Workbench

1. Abra o MySQL Workbench
2. Execute:
```sql
DROP DATABASE IF EXISTS siseus;
CREATE DATABASE siseus;
```
3. No terminal do projeto:
```bash
cd src/SisEUs.API
dotnet ef database update
```

### ? Opção 3: Linha de Comando Direto

```bash
# 1. Recriar banco
mysql -u Faca -pGol050219581 -e "DROP DATABASE IF EXISTS siseus; CREATE DATABASE siseus;"

# 2. Aplicar migrations
cd src/SisEUs.API
dotnet ef database update
```

## ?? Verificar se Funcionou

Execute a aplicação:
```bash
cd src/SisEUs.API
dotnet run
```

Você deve ver:
```
? Conexão estabelecida com sucesso!
? Encontradas X migrations pendentes
? Migrations aplicadas com sucesso!
? Seed concluído com sucesso!
```

## ?? Executar a Aplicação

```bash
cd src/SisEUs.API
dotnet run
```

Acesse: `https://localhost:7000/swagger`

## ?? Usuários Padrão (Após Seed)

| Email | Senha | Tipo |
|-------|-------|------|
| admin@siseus.com | Admin@123 | Admin |
| professor@siseus.com | Prof@123 | Professor |
| aluno@siseus.com | Aluno@123 | Aluno |

## ? Problemas Comuns

### "Command 'dotnet ef' not found"
```bash
dotnet tool install --global dotnet-ef
```

### "Cannot connect to MySQL server"
Verifique se o MySQL está rodando:
```bash
# Windows
net start MySQL80

# Linux
sudo systemctl start mysql
```

### "Access denied for user"
Verifique o usuário e senha no arquivo:
- `src/SisEUs.API/appsettings.json`
- `src/SisEUs.API/appsettings.Development.json`

## ?? Mais Informações

Consulte: `SOLUCAO_ERRO_MIGRATIONS.md` para soluções avançadas.
