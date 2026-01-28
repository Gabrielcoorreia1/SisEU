#!/bin/bash
# Script para rebuild completo do Docker

echo "?? Limpando containers e volumes antigos..."
docker-compose down -v

echo ""
echo "??? Removendo volumes órfãos..."
docker volume prune -f

echo ""
echo "?? Reconstruindo imagens sem cache..."
docker-compose build --no-cache

echo ""
echo "?? Iniciando containers..."
docker-compose up -d

echo ""
echo "?? Aguardando 10 segundos para os containers iniciarem..."
sleep 10

echo ""
echo "?? Status dos containers:"
docker-compose ps

echo ""
echo "?? Logs da API:"
docker logs siseus-api --tail=50

echo ""
echo "? Para ver os logs em tempo real, use:"
echo "   docker logs -f siseus-api"
echo ""
echo "? Para verificar as tabelas criadas, use:"
echo "   docker exec -it siseus-db mysql -uFaca -pGol050219581 siseus -e 'SHOW TABLES;'"
