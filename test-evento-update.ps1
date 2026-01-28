# Script para testar atualização de eventos e verificar persistência no banco de dados
# Uso: .\test-evento-update.ps1

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "TESTE DE ATUALIZAÇÃO DE EVENTO" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:8080/api"
$eventoId = 1

# Função para fazer requisições HTTP
function Invoke-ApiRequest {
    param(
        [string]$Method,
        [string]$Uri,
        [string]$Token = $null,
        [object]$Body = $null
    )
    
    $headers = @{
        "Content-Type" = "application/json"
    }
    
    if ($Token) {
        $headers["Authorization"] = "Bearer $Token"
    }
    
    try {
        if ($Body) {
            $jsonBody = $Body | ConvertTo-Json -Depth 10
            $response = Invoke-RestMethod -Uri $Uri -Method $Method -Headers $headers -Body $jsonBody -ErrorAction Stop
        } else {
            $response = Invoke-RestMethod -Uri $Uri -Method $Method -Headers $headers -ErrorAction Stop
        }
        return $response
    } catch {
        Write-Host "Erro na requisição: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.ErrorDetails.Message) {
            Write-Host "Detalhes: $($_.ErrorDetails.Message)" -ForegroundColor Red
        }
        return $null
    }
}

# Passo 1: Login
Write-Host "PASSO 1: Fazendo login..." -ForegroundColor Yellow
$loginBody = @{
    email = "ana.lima@siseus.com"
    senha = "Senha@123"
}

$loginResponse = Invoke-ApiRequest -Method POST -Uri "$baseUrl/auth/login" -Body $loginBody

if (-not $loginResponse) {
    Write-Host "Falha no login. Abortando teste." -ForegroundColor Red
    exit 1
}

$token = $loginResponse.token
Write-Host "? Login realizado com sucesso!" -ForegroundColor Green
Write-Host "Token: $($token.Substring(0, 20))..." -ForegroundColor Gray
Write-Host ""

# Passo 2: Obter evento ANTES da atualização
Write-Host "PASSO 2: Obtendo evento ANTES da atualização..." -ForegroundColor Yellow
$eventoBefore = Invoke-ApiRequest -Method GET -Uri "$baseUrl/eventos/$eventoId"

if (-not $eventoBefore) {
    Write-Host "Evento não encontrado. Abortando teste." -ForegroundColor Red
    exit 1
}

Write-Host "? Evento obtido:" -ForegroundColor Green
Write-Host "  ID: $($eventoBefore.id)" -ForegroundColor Gray
Write-Host "  Título: $($eventoBefore.titulo)" -ForegroundColor Gray
Write-Host "  Data Início: $($eventoBefore.dataInicio)" -ForegroundColor Gray
Write-Host "  Data Fim: $($eventoBefore.dataFim)" -ForegroundColor Gray
Write-Host "  Local: $($eventoBefore.local.departamento) - Bloco $($eventoBefore.local.bloco) - Sala $($eventoBefore.local.sala)" -ForegroundColor Gray
Write-Host "  Código Único: $($eventoBefore.codigoUnico)" -ForegroundColor Gray
Write-Host "  Avaliadores: $($eventoBefore.avaliadores.Count)" -ForegroundColor Gray
Write-Host ""

# Passo 3: Atualizar evento
Write-Host "PASSO 3: Atualizando evento..." -ForegroundColor Yellow
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
$updateBody = @{
    id = $eventoId
    titulo = "Evento Atualizado em $timestamp"
    dataInicio = "2025-04-20T09:00:00"
    dataFim = "2025-04-20T17:00:00"
    local = @{
        campus = 0
        departamento = "Departamento ATUALIZADO"
        bloco = "Z"
        sala = "999"
    }
    eTipoEvento = 1
    codigoUnico = $eventoBefore.codigoUnico
    imgUrl = "https://exemplo.com/imagem-atualizada-$timestamp.jpg"
    cpfsAvaliadores = @("12345678900", "98765432100")
    apresentacoes = @()
}

$updateResponse = Invoke-ApiRequest -Method PUT -Uri "$baseUrl/eventos/$eventoId" -Token $token -Body $updateBody

if ($null -eq $updateResponse) {
    Write-Host "? Atualização aceita (resposta vazia = sucesso)" -ForegroundColor Green
} else {
    Write-Host "? Evento atualizado!" -ForegroundColor Green
}
Write-Host ""

# Aguardar um pouco para garantir que salvou
Start-Sleep -Seconds 2

# Passo 4: Obter evento DEPOIS da atualização
Write-Host "PASSO 4: Obtendo evento DEPOIS da atualização..." -ForegroundColor Yellow
$eventoAfter = Invoke-ApiRequest -Method GET -Uri "$baseUrl/eventos/$eventoId"

if (-not $eventoAfter) {
    Write-Host "Erro ao obter evento após atualização." -ForegroundColor Red
    exit 1
}

Write-Host "? Evento obtido após atualização:" -ForegroundColor Green
Write-Host "  ID: $($eventoAfter.id)" -ForegroundColor Gray
Write-Host "  Título: $($eventoAfter.titulo)" -ForegroundColor Gray
Write-Host "  Data Início: $($eventoAfter.dataInicio)" -ForegroundColor Gray
Write-Host "  Data Fim: $($eventoAfter.dataFim)" -ForegroundColor Gray
Write-Host "  Local: $($eventoAfter.local.departamento) - Bloco $($eventoAfter.local.bloco) - Sala $($eventoAfter.local.sala)" -ForegroundColor Gray
Write-Host "  Código Único: $($eventoAfter.codigoUnico)" -ForegroundColor Gray
Write-Host "  Avaliadores: $($eventoAfter.avaliadores.Count)" -ForegroundColor Gray
Write-Host ""

# Passo 5: Comparar mudanças
Write-Host "PASSO 5: Comparando mudanças..." -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan

$mudancas = @()

if ($eventoBefore.titulo -ne $eventoAfter.titulo) {
    $mudancas += "  Título: '$($eventoBefore.titulo)' ? '$($eventoAfter.titulo)'"
}

if ($eventoBefore.dataInicio -ne $eventoAfter.dataInicio) {
    $mudancas += "  Data Início: '$($eventoBefore.dataInicio)' ? '$($eventoAfter.dataInicio)'"
}

if ($eventoBefore.dataFim -ne $eventoAfter.dataFim) {
    $mudancas += "  Data Fim: '$($eventoBefore.dataFim)' ? '$($eventoAfter.dataFim)'"
}

if ($eventoBefore.local.departamento -ne $eventoAfter.local.departamento) {
    $mudancas += "  Departamento: '$($eventoBefore.local.departamento)' ? '$($eventoAfter.local.departamento)'"
}

if ($eventoBefore.local.bloco -ne $eventoAfter.local.bloco) {
    $mudancas += "  Bloco: '$($eventoBefore.local.bloco)' ? '$($eventoAfter.local.bloco)'"
}

if ($eventoBefore.local.sala -ne $eventoAfter.local.sala) {
    $mudancas += "  Sala: '$($eventoBefore.local.sala)' ? '$($eventoAfter.local.sala)'"
}

if ($eventoBefore.imgUrl -ne $eventoAfter.imgUrl) {
    $mudancas += "  ImgUrl: '$($eventoBefore.imgUrl)' ? '$($eventoAfter.imgUrl)'"
}

if ($eventoBefore.avaliadores.Count -ne $eventoAfter.avaliadores.Count) {
    $mudancas += "  Avaliadores: $($eventoBefore.avaliadores.Count) ? $($eventoAfter.avaliadores.Count)"
}

if ($mudancas.Count -gt 0) {
    Write-Host "? Mudanças detectadas:" -ForegroundColor Green
    foreach ($mudanca in $mudancas) {
        Write-Host $mudanca -ForegroundColor Cyan
    }
} else {
    Write-Host "? NENHUMA MUDANÇA DETECTADA!" -ForegroundColor Red
    Write-Host "Isso pode indicar que as alterações não foram salvas no banco." -ForegroundColor Red
}
Write-Host ""

# Passo 6: Verificar persistência (conectar ao banco MySQL)
Write-Host "PASSO 6: Verificando persistência no banco de dados..." -ForegroundColor Yellow
Write-Host "Consultando diretamente o MySQL via Docker..." -ForegroundColor Gray

# Verifica se o Docker está rodando
$dockerRunning = docker ps --filter "name=siseus-mysql" --format "{{.Names}}" 2>$null

if ($dockerRunning) {
    Write-Host "? Container MySQL encontrado: $dockerRunning" -ForegroundColor Green
    
    # Consulta SQL para verificar o evento
    $sqlQuery = "SELECT Id, Titulo, DataInicio, DataFim, CodigoUnico FROM Evento WHERE Id = $eventoId;"
    
    Write-Host "Executando query no banco..." -ForegroundColor Gray
    $dbResult = docker exec siseus-mysql mysql -u root -proot siseus -e "$sqlQuery" 2>$null
    
    if ($dbResult) {
        Write-Host "? Resultado da consulta no banco:" -ForegroundColor Green
        Write-Host $dbResult -ForegroundColor Cyan
    } else {
        Write-Host "? Não foi possível consultar o banco diretamente." -ForegroundColor Yellow
    }
} else {
    Write-Host "? Container MySQL não encontrado ou não está rodando." -ForegroundColor Yellow
    Write-Host "Execute 'docker-compose up -d' para iniciar os containers." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "RESUMO DO TESTE" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if ($mudancas.Count -gt 0) {
    Write-Host "? TESTE BEM-SUCEDIDO!" -ForegroundColor Green
    Write-Host "  - Evento foi atualizado via API" -ForegroundColor Green
    Write-Host "  - Mudanças foram detectadas na consulta subsequente" -ForegroundColor Green
    Write-Host "  - $($mudancas.Count) campo(s) foram modificado(s)" -ForegroundColor Green
} else {
    Write-Host "? TESTE FALHOU!" -ForegroundColor Red
    Write-Host "  - Nenhuma mudança foi detectada após a atualização" -ForegroundColor Red
    Write-Host "  - Verifique se o método AtualizarEventoAsync está chamando _uow.CommitAsync()" -ForegroundColor Red
}

Write-Host ""
Write-Host "Para verificar a persistência completa, reinicie os containers:" -ForegroundColor Yellow
Write-Host "  docker-compose down && docker-compose up -d" -ForegroundColor Cyan
Write-Host "E então execute novamente:" -ForegroundColor Yellow
Write-Host "  GET $baseUrl/eventos/$eventoId" -ForegroundColor Cyan
Write-Host ""
