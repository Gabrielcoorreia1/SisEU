# Guia de Solução de Problemas - Docker

## ?? PROBLEMA PRINCIPAL RESOLVIDO

**O problema principal era que o código estava configurado para usar SQLite em vez de MySQL!**

### Correções Aplicadas:

1. ? **InjecaoDependencia.cs** - Alterado de `UseSqlite` para `UseMySql` com Pomelo
2. ? **AppDbContextFactory.cs** - Alterado para usar MySQL
3. ? **SisEUs.Infrastructure.csproj** - Substituído `Microsoft.EntityFrameworkCore.Sqlite` por `Pomelo.EntityFrameworkCore.MySql`
4. ? **Connection String** - Agora usa `DefaultConnection` em vez de `conexao` (que não existia)

---

## Problema Original

Este projeto tinha problemas ao executar via Docker onde:
- O código estava configurado para **SQLite** mas o Docker usa **MySQL**
- A connection string estava errada (`conexao` em vez de `DefaultConnection`)
- O banco de dados (MySQL) não estava completamente pronto quando a API tentava se conectar
- Erros de tabelas não encontradas ou migrações falhadas

## Soluções Implementadas

### 1. **Correção do Provedor de Banco de Dados** ? PRINCIPAL
- ? Alterado de SQLite para MySQL (Pomelo) em `InjecaoDependencia.cs`
- ? Corrigido `AppDbContextFactory.cs` para usar MySQL
- ? Adicionado pacote `Pomelo.EntityFrameworkCore.MySql` versão 8.0.0
- ? Removido pacote `Microsoft.EntityFrameworkCore.Sqlite`
- ? Connection string corrigida para usar `DefaultConnection`

### 2. **docker-compose.yml**
- ? Adicionado `start_period: 30s` ao healthcheck do MySQL
- ? Aumentado o número de `retries` de 5 para 10
- ? Exposta a porta 8080 da API no docker-compose

### 3. **Program.cs**
- ? Implementado retry logic com 30 tentativas
- ? Aguarda 3 segundos entre cada tentativa
- ? Aguarda mais 2 segundos após conexão bem-sucedida para garantir que o banco está completamente pronto
- ? Usa `CanConnectAsync()` para testar a conexão antes de executar migrations
- ? Logs detalhados para facilitar o debug

### 4. **Dockerfile**
- ? Dockerfile simplificado e otimizado
- ? Confia no retry logic robusto implementado no código C#

## Como Usar

### Opção 1: Usar o Script Automatizado (RECOMENDADO)

**Windows:**
```bash
rebuild-docker.bat
```

**Linux/Mac:**
```bash
chmod +x rebuild-docker.sh
./rebuild-docker.sh
```

### Opção 2: Comandos Manuais

#### Limpar containers e volumes antigos (IMPORTANTE)
```bash
docker-compose down -v
```

#### Construir e iniciar os containers (sem cache)
```bash
docker-compose build --no-cache
docker-compose up
```

#### Ou construir e iniciar em um único comando
```bash
docker-compose up --build --force-recreate
```

### Ver os logs da API
```bash
docker logs -f siseus-api
```

### Ver os logs do MySQL
```bash
docker logs -f siseus-db
```

## Verificar Status

### Verificar se o MySQL está rodando
```bash
docker exec -it siseus-db mysql -uFaca -pGol050219581 -e "SHOW DATABASES;"
```

### Verificar se as tabelas foram criadas ?
```bash
docker exec -it siseus-db mysql -uFaca -pGol050219581 siseus -e "SHOW TABLES;"
```

Você deve ver algo como:
```
+-------------------+
| Tables_in_siseus  |
+-------------------+
| Apresentacoes     |
| CheckinPins       |
| Checkins          |
| Presencas         |
| Sessoes           |
| Usuarios          |
+-------------------+
```

### Verificar dados mockados
```bash
docker exec -it siseus-db mysql -uFaca -pGol050219581 siseus -e "SELECT * FROM Usuarios;"
```

### Acessar a API
A API estará disponível em: http://localhost:8080

### Acessar o Swagger
http://localhost:8080/swagger

## Troubleshooting

### Se ainda houver problemas:

1. **Remova todos os volumes e recrie (RECOMENDADO):**
   ```bash
   docker-compose down -v
   docker volume prune -f
   docker-compose build --no-cache
   docker-compose up
   ```

2. **Verifique os logs da API em tempo real:**
   ```bash
   docker logs -f siseus-api
   ```
   
   Você deve ver mensagens como:
   ```
   Tentando conectar ao banco de dados... (Tentativa 1/30)
   Conexão com o banco de dados estabelecida com sucesso!
   Recriando o banco de dados...
   Populando dados iniciais...
   Banco de dados inicializado com sucesso!
   ```

3. **Verifique se o MySQL está saudável:**
   ```bash
   docker inspect siseus-db | grep -A 20 Health
   ```

4. **Conecte-se manualmente ao MySQL:**
   ```bash
   docker exec -it siseus-db mysql -uFaca -pGol050219581
   ```

5. **Reinicie apenas o container da API (se o MySQL já estiver rodando):**
   ```bash
   docker-compose restart api
   ```

6. **Verifique se a API está usando MySQL:**
   ```bash
   docker logs siseus-api 2>&1 | grep -i mysql
   ```

## O Que Foi Corrigido

### Problema Original #1 - SQLite vs MySQL ? PRINCIPAL
- O código estava usando `UseSqlite` mas o Docker tinha MySQL
- Connection string errada (`conexao` não existia)
- Pacote errado instalado (SQLite em vez de Pomelo MySQL)

**Solução:**
- Substituído SQLite por MySQL (Pomelo) em todos os lugares
- Corrigida a connection string para usar `DefaultConnection`
- Instalado o pacote correto `Pomelo.EntityFrameworkCore.MySql`

### Problema Original #2 - Timing de Inicialização
- A API tentava conectar ao MySQL imediatamente após o container subir
- O MySQL precisava de alguns segundos extras para aceitar conexões e processar comandos DDL
- Erros de "tabela não existe" ou "migração falhou"

**Solução:**
- **30 tentativas** com intervalo de 3 segundos (até 90 segundos de espera)
- Delay adicional de 2 segundos após conexão bem-sucedida
- Healthcheck robusto no docker-compose
- Logs detalhados para monitorar o processo

## Notas Importantes

- O banco de dados é **recriado** toda vez que a API inicia (via `EnsureDeleted()` e `EnsureCreated()`)
- Os dados são populados via `InitBD.SeedAsync()`
- Em produção, considere usar Migrations do Entity Framework em vez de `EnsureCreated()`
- O MySQL está exposto na porta **3307** do host (não 3306) para evitar conflitos com MySQL local
- A API aguarda até **90 segundos** para o MySQL ficar pronto antes de falhar
- **IMPORTANTE**: Agora o projeto usa MySQL em TODOS os ambientes (local e Docker)

## Portas Utilizadas

- **MySQL:** 3307:3306 (host:container)
- **API:** 8080:8080
- **Frontend:** 80:80

## Comandos Úteis

```bash
# Parar tudo
docker-compose down

# Parar e remover volumes
docker-compose down -v

# Ver todos os containers rodando
docker ps

# Ver logs de um container específico
docker logs siseus-api
docker logs siseus-db

# Entrar no container da API
docker exec -it siseus-api bash

# Entrar no MySQL
docker exec -it siseus-db mysql -uFaca -pGol050219581 siseus

# Rebuild completo
docker-compose down -v && docker-compose build --no-cache && docker-compose up

# Ver query de criação de uma tabela
docker exec -it siseus-db mysql -uFaca -pGol050219581 siseus -e "SHOW CREATE TABLE Usuarios;"

# Contar registros em uma tabela
docker exec -it siseus-db mysql -uFaca -pGol050219581 siseus -e "SELECT COUNT(*) FROM Usuarios;"
```

## Diferenças entre Ambiente Local e Docker

### Antes da Correção:
- ? Local: SQLite
- ? Docker: MySQL
- ? Problema: Inconsistência causava erros

### Depois da Correção:
- ? Local: MySQL (localhost:3306)
- ? Docker: MySQL (mysql:3306 dentro da rede Docker)
- ? Ambos usam o mesmo código e configurações
