# Guia de Solução de Problemas - Docker

## Problema Resolvido

Este projeto tinha problemas ao executar via Docker onde o banco de dados (MySQL) não estava completamente pronto quando a API tentava se conectar, resultando em erros de tabelas não encontradas ou migrações falhadas.

## Soluções Implementadas

### 1. **docker-compose.yml**
- ? Adicionado `start_period: 30s` ao healthcheck do MySQL
- ? Aumentado o número de `retries` de 5 para 10
- ? Exposta a porta 8080 da API no docker-compose

### 2. **Program.cs**
- ? Implementado retry logic com 30 tentativas
- ? Aguarda 3 segundos entre cada tentativa
- ? Aguarda mais 2 segundos após conexão bem-sucedida para garantir que o banco está completamente pronto
- ? Usa `CanConnectAsync()` para testar a conexão antes de executar migrations
- ? Logs detalhados para facilitar o debug

### 3. **Dockerfile**
- ? Dockerfile simplificado e otimizado
- ? Confia no retry logic robusto implementado no código C#

## Como Usar

### Limpar containers e volumes antigos (IMPORTANTE)
```bash
docker-compose down -v
```

### Construir e iniciar os containers (sem cache)
```bash
docker-compose build --no-cache
docker-compose up
```

### Ou construir e iniciar em um único comando
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

### Verificar se as tabelas foram criadas
```bash
docker exec -it siseus-db mysql -uFaca -pGol050219581 siseus -e "SHOW TABLES;"
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

## O Que Foi Corrigido

### Problema Original
- A API tentava conectar ao MySQL imediatamente após o container subir
- O MySQL precisava de alguns segundos extras para aceitar conexões e processar comandos DDL
- Erros de "tabela não existe" ou "migração falhou"

### Solução Implementada
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
