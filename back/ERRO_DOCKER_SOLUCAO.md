# ?? SOLUÇÃO RÁPIDA: Erro "Table 'siseus.Usuarios' doesn't exist" no Docker

## ? Solução em 1 Comando

### Windows (PowerShell):
```powershell
.\reset-docker.ps1
```

### Linux/Mac (Bash):
```bash
chmod +x reset-docker.sh
./reset-docker.sh
```

### Comando Manual Direto:
```bash
docker-compose down -v && docker-compose up --build -d && sleep 40 && docker exec siseus-api dotnet ef database update && docker-compose restart api
```

## ?? O que o script faz:

1. ? Para e remove todos os containers e volumes
2. ? Reconstrói as imagens Docker
3. ? Aguarda o MySQL inicializar (40 segundos)
4. ? Aplica as migrations automaticamente
5. ? Reinicia a API
6. ? Mostra os logs e status

## ?? Resultado Esperado

Após executar, você verá:

```
? Conexão estabelecida com sucesso!
? Migrations aplicadas com sucesso!
? Seed concluído com sucesso!
```

## ?? Acesso

- **API Swagger**: http://localhost:8080/swagger
- **Frontend**: http://localhost

## ?? Usuários Padrão

| Email | Senha | Tipo |
|-------|-------|------|
| admin@siseus.com | Admin@123 | Admin |
| professor@siseus.com | Prof@123 | Professor |
| aluno@siseus.com | Aluno@123 | Aluno |

## ?? Se o Problema Persistir

### Opção 1: Aplicar Migrations Manualmente
```bash
# Aguardar MySQL estar pronto
docker-compose up -d mysql
sleep 30

# Aplicar migrations
docker exec -it siseus-api dotnet ef database update

# Reiniciar API
docker-compose restart api
```

### Opção 2: Verificar Logs
```bash
# Ver logs do MySQL
docker-compose logs mysql

# Ver logs da API
docker-compose logs api

# Ver status
docker-compose ps
```

### Opção 3: Limpar Cache Docker
```bash
docker system prune -a -f
docker-compose up --build -d
```

## ?? Documentação Completa

Para mais detalhes, consulte:
- **GUIA_DOCKER.md** - Guia completo de Docker
- **INICIO_RAPIDO.md** - Guia de início rápido
- **SOLUCAO_ERRO_MIGRATIONS.md** - Soluções avançadas

## ?? Importante

- O script `reset-docker.ps1` / `reset-docker.sh` **apaga o banco de dados** (por usar `-v`)
- Se você tem dados importantes, faça backup antes
- Aguarde sempre 30-40 segundos após iniciar o MySQL antes de aplicar migrations

## ?? Comandos Úteis

```bash
# Ver todos os containers
docker-compose ps

# Ver logs em tempo real
docker-compose logs -f api

# Reiniciar apenas a API
docker-compose restart api

# Parar tudo
docker-compose down

# Entrar no container da API
docker exec -it siseus-api bash

# Entrar no MySQL
docker exec -it siseus-db mysql -u Faca -pGol050219581
```

## ? Checklist de Verificação

Após executar o script, verifique:

- [ ] MySQL está "healthy" (`docker-compose ps`)
- [ ] API está "running"
- [ ] Logs mostram "Seed concluído com sucesso"
- [ ] Swagger acessível em http://localhost:8080/swagger
- [ ] Consegue fazer login com usuários padrão

## ?? Dica

Para evitar este problema no futuro, considere:
1. Adicionar script de inicialização ao Dockerfile (veja GUIA_DOCKER.md)
2. Usar wait-for-it para aguardar MySQL
3. Aplicar migrations como parte do ENTRYPOINT
