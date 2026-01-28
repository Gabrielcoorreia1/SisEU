# Script para recriar o banco de dados e aplicar migrations
# Uso: .\reset-database.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Recriando Banco de Dados SisEUs" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Configurações do MySQL (ajuste conforme seu ambiente)
$mysqlHost = "localhost"
$mysqlPort = "3306"
$mysqlUser = "Faca"
$mysqlPassword = "Gol050219581"
$mysqlDatabase = "siseus"

Write-Host "1. Removendo banco de dados existente..." -ForegroundColor Yellow

# Comando SQL para dropar e criar o banco
$sqlCommands = @"
DROP DATABASE IF EXISTS $mysqlDatabase;
CREATE DATABASE $mysqlDatabase;
"@

# Executa comando MySQL
$mysqlCommand = "mysql -h $mysqlHost -P $mysqlPort -u $mysqlUser -p$mysqlPassword -e `"$sqlCommands`""

try {
    Invoke-Expression $mysqlCommand
    Write-Host "   ? Banco de dados recriado com sucesso!" -ForegroundColor Green
} catch {
    Write-Host "   ? Erro ao recriar banco de dados" -ForegroundColor Red
    Write-Host "   Execute manualmente no MySQL:" -ForegroundColor Yellow
    Write-Host "   DROP DATABASE IF EXISTS siseus;" -ForegroundColor White
    Write-Host "   CREATE DATABASE siseus;" -ForegroundColor White
    Write-Host ""
    Read-Host "Pressione ENTER após executar os comandos manualmente"
}

Write-Host ""
Write-Host "2. Aplicando migrations..." -ForegroundColor Yellow

# Navega para o diretório da API
Set-Location -Path "src\SisEUs.API"

# Aplica migrations
try {
    dotnet ef database update
    Write-Host "   ? Migrations aplicadas com sucesso!" -ForegroundColor Green
} catch {
    Write-Host "   ? Erro ao aplicar migrations" -ForegroundColor Red
    Write-Host "   Execute manualmente: dotnet ef database update" -ForegroundColor Yellow
    exit 1
}

# Volta para o diretório raiz
Set-Location -Path "..\..\"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Banco de dados pronto para uso!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Execute a aplicação com: dotnet run --project src\SisEUs.API" -ForegroundColor White
Write-Host ""
