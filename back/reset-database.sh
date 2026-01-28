#!/bin/bash

# Script para recriar o banco de dados e aplicar migrations
# Uso: ./reset-database.sh

echo "========================================"
echo "  Recriando Banco de Dados SisEUs"
echo "========================================"
echo ""

# Configurações do MySQL (ajuste conforme seu ambiente)
MYSQL_HOST="localhost"
MYSQL_PORT="3306"
MYSQL_USER="Faca"
MYSQL_PASSWORD="Gol050219581"
MYSQL_DATABASE="siseus"

echo "1. Removendo banco de dados existente..."

# Comando SQL para dropar e criar o banco
SQL_COMMANDS="DROP DATABASE IF EXISTS $MYSQL_DATABASE; CREATE DATABASE $MYSQL_DATABASE;"

# Executa comando MySQL
if mysql -h "$MYSQL_HOST" -P "$MYSQL_PORT" -u "$MYSQL_USER" -p"$MYSQL_PASSWORD" -e "$SQL_COMMANDS" 2>/dev/null; then
    echo "   ? Banco de dados recriado com sucesso!"
else
    echo "   ? Erro ao recriar banco de dados"
    echo "   Execute manualmente no MySQL:"
    echo "   DROP DATABASE IF EXISTS siseus;"
    echo "   CREATE DATABASE siseus;"
    echo ""
    read -p "Pressione ENTER após executar os comandos manualmente"
fi

echo ""
echo "2. Aplicando migrations..."

# Navega para o diretório da API
cd src/SisEUs.API || exit 1

# Aplica migrations
if dotnet ef database update; then
    echo "   ? Migrations aplicadas com sucesso!"
else
    echo "   ? Erro ao aplicar migrations"
    echo "   Execute manualmente: dotnet ef database update"
    exit 1
fi

# Volta para o diretório raiz
cd ../..

echo ""
echo "========================================"
echo "  Banco de dados pronto para uso!"
echo "========================================"
echo ""
echo "Execute a aplicação com: dotnet run --project src/SisEUs.API"
echo ""
