#!/bin/bash

# Script para resetar ambiente Docker e aplicar migrations
# Uso: ./reset-docker.sh

# Cores
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;90m'
NC='\033[0m' # No Color

echo ""
echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}  ?? Reset Docker + Migrations SisEUs${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""

# Navegar para o diretório correto
cd "$(dirname "$0")"
echo -e "${GRAY}?? Diretório atual: $(pwd)${NC}"
echo ""

# Verificar se docker-compose.yml existe
if [ ! -f "docker-compose.yml" ]; then
    if [ -f "../docker-compose.yml" ]; then
        echo -e "${YELLOW}?? docker-compose.yml encontrado na pasta pai${NC}"
        cd ..
    else
        echo -e "${RED}? docker-compose.yml não encontrado!${NC}"
        echo -e "${YELLOW}   Execute este script do diretório que contém o docker-compose.yml${NC}"
        exit 1
    fi
fi

echo -e "${YELLOW}1??  Parando e removendo containers...${NC}"
docker-compose down -v

if [ $? -ne 0 ]; then
    echo -e "${RED}   ? Erro ao parar containers${NC}"
    exit 1
fi

echo -e "${GREEN}   ? Containers removidos${NC}"
echo ""

echo -e "${YELLOW}2??  Rebuilding imagens...${NC}"
docker-compose up --build -d

if [ $? -ne 0 ]; then
    echo -e "${RED}   ? Erro no build${NC}"
    exit 1
fi

echo -e "${GREEN}   ? Build concluído${NC}"
echo ""

echo -e "${YELLOW}3??  Aguardando MySQL inicializar (40 segundos)...${NC}"
sleep 40

echo -e "${GREEN}   ? MySQL deve estar pronto${NC}"
echo ""

echo -e "${YELLOW}4??  Verificando status dos containers...${NC}"
docker-compose ps

echo ""

echo -e "${YELLOW}5??  Aplicando migrations...${NC}"

# Tentar aplicar migrations
max_retries=3
retry=0
success=false

while [ "$success" != "true" ] && [ $retry -lt $max_retries ]; do
    retry=$((retry + 1))
    echo -e "${GRAY}   Tentativa $retry de $max_retries...${NC}"
    
    docker exec siseus-api dotnet ef database update
    
    if [ $? -eq 0 ]; then
        success=true
        echo -e "${GREEN}   ? Migrations aplicadas com sucesso!${NC}"
    else
        echo -e "${YELLOW}   ??  Falha na tentativa $retry${NC}"
        if [ $retry -lt $max_retries ]; then
            echo -e "${GRAY}   Aguardando 10 segundos antes de tentar novamente...${NC}"
            sleep 10
        fi
    fi
done

if [ "$success" != "true" ]; then
    echo -e "${RED}   ? Não foi possível aplicar migrations${NC}"
    echo -e "${YELLOW}   Tente executar manualmente:${NC}"
    echo -e "   docker exec -it siseus-api dotnet ef database update"
fi

echo ""

echo -e "${YELLOW}6??  Reiniciando API...${NC}"
docker-compose restart api

if [ $? -ne 0 ]; then
    echo -e "${YELLOW}   ??  Erro ao reiniciar API${NC}"
else
    echo -e "${GREEN}   ? API reiniciada${NC}"
fi

echo ""

echo -e "${YELLOW}7??  Aguardando API inicializar (15 segundos)...${NC}"
sleep 15

echo ""

echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}  ?? Status Final${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""

docker-compose ps

echo ""
echo -e "${CYAN}========================================${NC}"
echo -e "${CYAN}  ?? Logs Recentes da API${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""

docker-compose logs --tail=30 api

echo ""
echo -e "${CYAN}========================================${NC}"
echo -e "${GREEN}  ? Processo Concluído!${NC}"
echo -e "${CYAN}========================================${NC}"
echo ""
echo -e "?? Acesse:"
echo -e "   • API Swagger: ${CYAN}http://localhost:8080/swagger${NC}"
echo -e "   • Frontend: ${CYAN}http://localhost${NC}"
echo ""
echo -e "?? Usuários padrão (após seed):"
echo -e "${GRAY}   • admin@siseus.com / Admin@123${NC}"
echo -e "${GRAY}   • professor@siseus.com / Prof@123${NC}"
echo -e "${GRAY}   • aluno@siseus.com / Aluno@123${NC}"
echo ""
echo -e "?? Comandos úteis:"
echo -e "   • Ver logs: ${GRAY}docker-compose logs -f api${NC}"
echo -e "   • Status: ${GRAY}docker-compose ps${NC}"
echo -e "   • Parar: ${GRAY}docker-compose down${NC}"
echo ""
