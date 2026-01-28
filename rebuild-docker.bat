@echo off
REM Script para rebuild completo do Docker no Windows

echo ?? Limpando containers e volumes antigos...
docker-compose down -v

echo.
echo ??? Removendo volumes orfaos...
docker volume prune -f

echo.
echo ?? Reconstruindo imagens sem cache...
docker-compose build --no-cache

echo.
echo ?? Iniciando containers...
docker-compose up -d

echo.
echo ?? Aguardando 10 segundos para os containers iniciarem...
timeout /t 10 /nobreak

echo.
echo ?? Status dos containers:
docker-compose ps

echo.
echo ?? Logs da API:
docker logs siseus-api --tail=50

echo.
echo ? Para ver os logs em tempo real, use:
echo    docker logs -f siseus-api
echo.
echo ? Para verificar as tabelas criadas, use:
echo    docker exec -it siseus-db mysql -uFaca -pGol050219581 siseus -e "SHOW TABLES;"
