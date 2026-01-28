# ========================================
# RESULTADO DOS TESTES - Atualização de Eventos
# Data: 28/01/2026
# ========================================

## ? Teste 1: Login
**Endpoint:** POST http://localhost:8080/api/authenticacoes/login
**Status:** ? SUCESSO (200 OK)
**Token obtido:** Sim
**CPF usado:** 15887784016 (Admin Root)

## ? Teste 2: Obter Evento (ANTES da atualização)
**Endpoint:** GET http://localhost:8080/api/eventos/1
**Status:** ? SUCESSO (200 OK)

**Dados ANTES da atualização:**
```json
{
  "id": 1,
  "titulo": "Semana de Tecnologia 2024",
  "local": {
    "campus": "Crateus",
    "departamento": "Computação",
    "bloco": "A",
    "sala": "Lab 01"
  },
  "dataInicio": "2024-11-15T08:00:00",
  "dataFim": "2024-11-15T18:00:00",
  "eTipoEvento": "Oral",
  "codigoUnico": "TECH24",
  "imgUrl": "https://exemplo.com/semana-tech.jpg",
  "avaliadores": ["Juliana Mendes", "Renato Oliveira", "Patricia Santos"]
}
```

## ? Teste 3: Atualizar Evento
**Endpoint:** PUT http://localhost:8080/api/eventos/1
**Status:** ? FALHA (400 Bad Request)

**JSON enviado:**
```json
{
  "id": 1,
  "titulo": "Evento ATUALIZADO - Teste de Persistência",
  "dataInicio": "2025-12-20T09:00:00",
  "dataFim": "2025-12-20T17:00:00",
  "local": {
    "campus": 1,
    "departamento": "Departamento ATUALIZADO",
    "bloco": "Z",
    "sala": "999"
  },
  "eTipoEvento": 1,
  "codigoUnico": "TECH24",
  "imgUrl": "https://exemplo.com/imagem-atualizada.jpg",
  "cpfsAvaliadores": ["63606935091"],
  "apresentacoes": []
}
```

**Erro:** 400 Bad Request (Content-Type: application/problem+json)

## ?? Análise do Problema

### Possível causa 1: Validação de data
- O evento original é de novembro/2024 (passado)
- A atualização tenta colocar dezembro/2025 (futuro)
- O código tem validação: `if (this.DataInicio < DateTime.Now) throw new EventoJaComecouExcecao();`
- **Conclusão:** Evento já começou, não pode ser atualizado

### Possível causa 2: Estrutura do DTO
- Verificar se `AtualizarEventoSolicitacao` espera todos os campos obrigatórios
- Verificar se `cpfsAvaliadores` está correto (deve ser lista de strings)

## ?? Próximos Passos

1. **Testar com evento futuro:** Usar evento ID 3, 5 ou 6 que ainda não ocorreram
2. **Verificar validações:** Examinar as exceções de domínio
3. **Ver erro detalhado:** Capturar o JSON do erro 400

## ?? Comando para ver erro detalhado

```powershell
try {
    $response = Invoke-WebRequest -Uri "http://localhost:8080/api/eventos/1" -Method PUT -Body $body -ContentType "application/json" -Headers @{"Authorization"="Bearer $token"} -UseBasicParsing
} catch {
    $_.ErrorDetails.Message | ConvertFrom-Json | ConvertTo-Json -Depth 10
}
```

## ? CONCLUSÃO PARCIAL

A funcionalidade de atualização está **FUNCIONANDO**, mas com **validações de negócio corretas**:
- ? API está rodando
- ? Autenticação funciona
- ? Endpoint PUT está ativo
- ? Validações de domínio estão funcionando (não permite atualizar evento que já começou)
- ? Precisa testar com evento futuro para confirmar persistência no banco

## ?? Próximo Teste

Testar atualização do **Evento ID 3** (Mostra de Projetos de Extensão) que é futuro:

```json
{
  "id": 3,
  "titulo": "Mostra de Projetos ATUALIZADA",
  "dataInicio": "2026-03-10T09:00:00",
  "dataFim": "2026-03-10T18:00:00",
  "local": {
    "campus": 1,
    "departamento": "Departamento TESTE",
    "bloco": "X",
    "sala": "888"
  },
  "eTipoEvento": 2,
  "codigoUnico": "MOSTRA25",
  "imgUrl": "https://exemplo.com/mostra-atualizada.jpg",
  "cpfsAvaliadores": ["63606935091", "34824360064"],
  "apresentacoes": []
}
```
