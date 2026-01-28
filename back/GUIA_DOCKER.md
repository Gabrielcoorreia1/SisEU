# ?? Guia Completo: Docker + Migrations - SisEUs API

## ?? Problema Atual

Erro ao rodar em Docker: **"Table 'siseus.Usuarios' doesn't exist"**

**Causa:** As migrations não são aplicadas automaticamente quando o container inicia.

## ? Solução Rápida (1 Comando)

```bash
# Parar tudo, rebuild e aplicar migrations
docker-compose down -v && docker-compose up --build -d && sleep 40 && docker exec -it siseus-api dotnet ef database update && docker-compose restart api
```

## ?? Soluções Detalhadas

### Solução 1: Aplicar Migrations Manualmente (Mais Rápido)

```bash
# 1. Garantir que MySQL está rodando
docker-compose up -d mysql

# 2. Aguardar MySQL inicializar (30-60 segundos)
docker-compose logs -f mysql
# Pressione Ctrl+C quando ver "ready for connections"

# 3. Aplicar migrations no container
docker exec -it siseus-api dotnet ef database update

# 4. Reiniciar a API
docker-compose restart api

# 5. Verificar logs
docker-compose logs -f api
```

### Solução 2: Aplicar Migrations do Host

```bash
# 1. Certificar que MySQL do Docker está acessível
docker-compose up -d mysql
sleep 30

# 2. Aplicar migrations do seu computador
cd back
dotnet ef database update --project src/SisEUs.Infrastructure --startup-project src/SisEUs.API

# 3. Iniciar a API
docker-compose up -d api
```

### Solução 3: Recriar Tudo do Zero

```bash
# 1. Remover tudo (?? Apaga banco de dados!)
docker-compose down -v

# 2. Rebuild e iniciar
docker-compose up --build -d

# 3. Aguardar MySQL (importante!)
sleep 40

# 4. Aplicar migrations
docker exec -it siseus-api dotnet ef database update

# 5. Reiniciar API
docker-compose restart api
```

## ?? Solução Permanente: Modificar Dockerfile

### Opção A: Script de Inicialização (Recomendado)

#### 1. Criar arquivo `back/entrypoint.sh`:

```bash
#!/bin/bash
set -e

echo "?? Aguardando MySQL inicializar..."
sleep 30

echo "?? Aplicando migrations..."
dotnet ef database update --no-build || echo "??  Migrations já aplicadas ou erro ao aplicar"

echo "?? Iniciando aplicação..."
exec dotnet SisEUs.API.dll
```

#### 2. Atualizar `back/Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "SisEUs.sln"

# Instalar dotnet-ef
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet publish "src/SisEUs.API/SisEUs.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Copiar dotnet-ef tools
COPY --from=build /root/.dotnet/tools /root/.dotnet/tools
ENV PATH="$PATH:/root/.dotnet/tools"

# Copiar aplicação
COPY --from=build /app/publish .

# Copiar script de entrada
COPY entrypoint.sh .
RUN chmod +x entrypoint.sh

EXPOSE 8080

ENTRYPOINT ["./entrypoint.sh"]
```

#### 3. Rebuild:

```bash
docker-compose down
docker-compose up --build -d
```

### Opção B: Wait-for Script (Mais Robusto)

#### 1. Baixar wait-for-it:

```bash
# No diretório back/
curl -o wait-for-it.sh https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh
chmod +x wait-for-it.sh
```

#### 2. Atualizar Dockerfile:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY . .

RUN dotnet restore "SisEUs.sln"
RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

RUN dotnet publish "src/SisEUs.API/SisEUs.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Instalar bash para scripts
RUN apt-get update && apt-get install -y bash && rm -rf /var/lib/apt/lists/*

COPY --from=build /root/.dotnet/tools /root/.dotnet/tools
ENV PATH="$PATH:/root/.dotnet/tools"

COPY --from=build /app/publish .
COPY wait-for-it.sh .
RUN chmod +x wait-for-it.sh

EXPOSE 8080

CMD ["./wait-for-it.sh", "mysql:3306", "--timeout=60", "--", "sh", "-c", "dotnet ef database update --no-build && dotnet SisEUs.API.dll"]
```

#### 3. Rebuild:

```bash
docker-compose down -v
docker-compose up --build -d
```

## ?? Verificar se Funcionou

```bash
# Ver logs em tempo real
docker-compose logs -f api

# Deve aparecer:
# ? Conexão estabelecida com sucesso!
# ? Migrations aplicadas com sucesso!
# ? Seed concluído com sucesso!

# Testar API
curl http://localhost:8080/swagger

# Ver tabelas criadas
docker exec -it siseus-db mysql -u Faca -pGol050219581 -e "USE siseus; SHOW TABLES;"
```

## ?? Problemas Comuns

### "Cannot connect to MySQL"

**Causa:** MySQL ainda não está pronto

**Solução:**
```bash
# Verificar status
docker-compose ps

# Aguardar até ficar "healthy"
docker-compose logs mysql | grep "ready for connections"
```

### "dotnet-ef: command not found"

**Causa:** EF Core tools não estão instalados no container

**Solução Imediata:**
```bash
docker exec -it siseus-api sh -c "dotnet tool install --global dotnet-ef"
docker exec -it siseus-api sh -c "export PATH=\"\$PATH:/root/.dotnet/tools\" && dotnet ef database update"
docker-compose restart api
```

**Solução Permanente:** Use o Dockerfile com dotnet-ef incluído (Opção A ou B acima)

### "Access denied for user 'Faca'"

**Causa:** Credenciais incorretas

**Solução:** Verifique `docker-compose.yml`:
```yaml
mysql:
  environment:
    MYSQL_USER: Faca
    MYSQL_PASSWORD: Gol050219581
    MYSQL_DATABASE: siseus

api:
  environment:
    - ConnectionStrings__DefaultConnection=server=mysql;port=3306;database=siseus;user=Faca;password=Gol050219581
```

### "Port 3306 already in use"

**Causa:** MySQL local rodando na mesma porta

**Solução:**
```bash
# Parar MySQL local (Windows)
net stop MySQL80

# Ou mudar porta no docker-compose.yml
ports:
  - "3307:3306"  # Usa porta 3307 no host
```

## ?? Comandos Úteis Docker

```bash
# Ver status de todos os serviços
docker-compose ps

# Ver logs
docker-compose logs -f             # Todos
docker-compose logs -f api         # Apenas API
docker-compose logs -f mysql       # Apenas MySQL

# Reiniciar serviço
docker-compose restart api
docker-compose restart mysql

# Entrar no container
docker exec -it siseus-api bash
docker exec -it siseus-db mysql -u Faca -pGol050219581

# Rebuild específico
docker-compose up -d --build api

# Ver recursos usados
docker stats

# Limpar tudo
docker-compose down -v              # Remove volumes (banco)
docker system prune -a              # Remove cache
```

## ?? Checklist de Solução

- [ ] MySQL rodando e healthy (`docker-compose ps`)
- [ ] Aguardou 30-60 segundos após iniciar MySQL
- [ ] String de conexão usa `server=mysql` (não localhost)
- [ ] Migrations aplicadas (manualmente ou via script)
- [ ] API reiniciada após migrations
- [ ] Logs mostram "Seed concluído com sucesso"
- [ ] Swagger acessível em http://localhost:8080/swagger

## ? Script PowerShell Completo

Crie `reset-docker.ps1`:

```powershell
Write-Host "?? Resetando ambiente Docker..." -ForegroundColor Cyan

Write-Host "1. Parando containers..." -ForegroundColor Yellow
docker-compose down -v

Write-Host "2. Rebuilding..." -ForegroundColor Yellow
docker-compose up --build -d

Write-Host "3. Aguardando MySQL (40s)..." -ForegroundColor Yellow
Start-Sleep -Seconds 40

Write-Host "4. Aplicando migrations..." -ForegroundColor Yellow
docker exec -it siseus-api dotnet ef database update

Write-Host "5. Reiniciando API..." -ForegroundColor Yellow
docker-compose restart api

Write-Host "6. Aguardando API iniciar (10s)..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

Write-Host "? Pronto! Acesse: http://localhost:8080/swagger" -ForegroundColor Green
docker-compose logs --tail=50 api
```

Execute:
```powershell
.\reset-docker.ps1
```

## ?? Após Resolver

**Endpoints:**
- API: http://localhost:8080/swagger
- Frontend: http://localhost

**Usuários padrão:**
- admin@siseus.com / Admin@123
- professor@siseus.com / Prof@123  
- aluno@siseus.com / Aluno@123
