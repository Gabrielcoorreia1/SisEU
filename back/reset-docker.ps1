#!/usr/bin/env pwsh
# Script para resetar ambiente Docker e aplicar migrations
# Uso: .\reset-docker.ps1

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  ?? Reset Docker + Migrations SisEUs" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Navegar para o diretório correto (raiz onde está docker-compose.yml)
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptPath

Write-Host "?? Diretório atual: $(Get-Location)" -ForegroundColor Gray
Write-Host ""

# Verificar se docker-compose.yml existe
if (-not (Test-Path "docker-compose.yml")) {
    if (Test-Path "..\docker-compose.yml") {
        Write-Host "?? docker-compose.yml encontrado na pasta pai" -ForegroundColor Yellow
        Set-Location ".."
    } else {
        Write-Host "? docker-compose.yml não encontrado!" -ForegroundColor Red
        Write-Host "   Execute este script do diretório que contém o docker-compose.yml" -ForegroundColor Yellow
        exit 1
    }
}

Write-Host "1??  Parando e removendo containers..." -ForegroundColor Yellow
docker-compose down -v

if ($LASTEXITCODE -ne 0) {
    Write-Host "   ? Erro ao parar containers" -ForegroundColor Red
    exit 1
}

Write-Host "   ? Containers removidos" -ForegroundColor Green
Write-Host ""

Write-Host "2??  Rebuilding imagens..." -ForegroundColor Yellow
docker-compose up --build -d

if ($LASTEXITCODE -ne 0) {
    Write-Host "   ? Erro no build" -ForegroundColor Red
    exit 1
}

Write-Host "   ? Build concluído" -ForegroundColor Green
Write-Host ""

Write-Host "3??  Aguardando MySQL inicializar (40 segundos)..." -ForegroundColor Yellow
Start-Sleep -Seconds 40

Write-Host "   ? MySQL deve estar pronto" -ForegroundColor Green
Write-Host ""

Write-Host "4??  Verificando status dos containers..." -ForegroundColor Yellow
docker-compose ps

Write-Host ""

Write-Host "5??  Aplicando migrations..." -ForegroundColor Yellow

# Tentar aplicar migrations
$maxRetries = 3
$retry = 0
$success = $false

while (-not $success -and $retry -lt $maxRetries) {
    $retry++
    Write-Host "   Tentativa $retry de $maxRetries..." -ForegroundColor Gray
    
    docker exec siseus-api dotnet ef database update
    
    if ($LASTEXITCODE -eq 0) {
        $success = $true
        Write-Host "   ? Migrations aplicadas com sucesso!" -ForegroundColor Green
    } else {
        Write-Host "   ??  Falha na tentativa $retry" -ForegroundColor Yellow
        if ($retry -lt $maxRetries) {
            Write-Host "   Aguardando 10 segundos antes de tentar novamente..." -ForegroundColor Gray
            Start-Sleep -Seconds 10
        }
    }
}

if (-not $success) {
    Write-Host "   ? Não foi possível aplicar migrations" -ForegroundColor Red
    Write-Host "   Tente executar manualmente:" -ForegroundColor Yellow
    Write-Host "   docker exec -it siseus-api dotnet ef database update" -ForegroundColor White
}

Write-Host ""

Write-Host "6??  Reiniciando API..." -ForegroundColor Yellow
docker-compose restart api

if ($LASTEXITCODE -ne 0) {
    Write-Host "   ??  Erro ao reiniciar API" -ForegroundColor Yellow
} else {
    Write-Host "   ? API reiniciada" -ForegroundColor Green
}

Write-Host ""

Write-Host "7??  Aguardando API inicializar (15 segundos)..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  ?? Status Final" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

docker-compose ps

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  ?? Logs Recentes da API" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

docker-compose logs --tail=30 api

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  ? Processo Concluído!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "?? Acesse:" -ForegroundColor White
Write-Host "   • API Swagger: " -NoNewline; Write-Host "http://localhost:8080/swagger" -ForegroundColor Cyan
Write-Host "   • Frontend: " -NoNewline; Write-Host "http://localhost" -ForegroundColor Cyan
Write-Host ""
Write-Host "?? Usuários padrão (após seed):" -ForegroundColor White
Write-Host "   • admin@siseus.com / Admin@123" -ForegroundColor Gray
Write-Host "   • professor@siseus.com / Prof@123" -ForegroundColor Gray
Write-Host "   • aluno@siseus.com / Aluno@123" -ForegroundColor Gray
Write-Host ""
Write-Host "?? Comandos úteis:" -ForegroundColor White
Write-Host "   • Ver logs: " -NoNewline; Write-Host "docker-compose logs -f api" -ForegroundColor Gray
Write-Host "   • Status: " -NoNewline; Write-Host "docker-compose ps" -ForegroundColor Gray
Write-Host "   • Parar: " -NoNewline; Write-Host "docker-compose down" -ForegroundColor Gray
Write-Host ""
