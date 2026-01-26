# Arquitetura do Sistema SisEUs

## Visão Geral

O sistema segue os princípios de **Domain-Driven Design (DDD)** com uma arquitetura em camadas:

```
???????????????????????????????????????
?            SisEUs.API               ?  ? Apresentação (Controllers)
???????????????????????????????????????
?        SisEUs.Application           ?  ? Aplicação (Serviços, DTOs)
???????????????????????????????????????
?          SisEUs.Domain              ?  ? Domínio (Entidades, VOs)
???????????????????????????????????????
?       SisEUs.Infrastructure         ?  ? Infraestrutura (EF, Repos)
???????????????????????????????????????
```

## Camadas

### 1. Domain (SisEUs.Domain)

Contém as regras de negócio puras, sem dependências de frameworks externos.

#### Contextos Delimitados

- **ContextoDeEvento**: Eventos, Apresentações, Presenças em eventos
- **ContextoDeUsuario**: Usuários, autenticação
- **Checkin**: Check-in diário por PIN (independente de eventos)

#### Entidades Principais

| Entidade | Descrição |
|----------|-----------|
| `Evento` | Raiz do agregado de eventos |
| `Apresentacao` | Apresentação vinculada a um evento |
| `Presenca` | Presença de usuário em um evento específico |
| `Checkin` | Check-in diário no campus usando PIN |
| `Usuario` | Usuário do sistema |

#### Diferença entre Presenca e Checkin

| Característica | Presenca | Checkin |
|----------------|----------|---------|
| Vinculado a evento | ? Sim | ? Não |
| Usa PIN | ? Não | ? Sim |
| Validação de localização | Do evento | Do campus |
| Contexto | Eventos | Check-in diário |

### 2. Application (SisEUs.Apresentation)

Orquestra os casos de uso da aplicação.

#### Serviços de Aplicação

- `EventoServico`: CRUD de eventos
- `ApresentacaoServico`: CRUD de apresentações  
- `PresencaServico`: Check-in/out em eventos
- `PinService`: Check-in/out diário por PIN
- `AuthService`: Autenticação

#### Serviços de Infraestrutura de Aplicação

- `GeolocalizacaoValidador`: Validação de localização
- `MapeadorDeEntidades`: Mapeamento de entidades para DTOs

### 3. Infrastructure (SisEUs.Infrastructure)

Implementações de persistência e serviços externos.

- Repositórios (Entity Framework Core)
- Configurações de entidades
- Geração/validação de tokens JWT

### 4. API (SisEUs.API)

Exposição de endpoints REST.

## Padrões Utilizados

### Repository Pattern
Interfaces definidas no Domain, implementações na Infrastructure.

### Unit of Work
`IUoW` gerencia transações de banco de dados.

### Factory Method
Entidades usam métodos `Criar()` para garantir estado válido.

### Value Objects
Objetos imutáveis como `Localizacao`, `Titulo`, `Email`, `Cpf`.

### Serviço de Mapeamento
`IMapeadorDeEntidades` centraliza conversão de entidades para DTOs.

## Configurações

### Geolocalização (appsettings.json)

```json
{
  "Geolocalizacao": {
    "RaioCheckinEventoMetros": 100,
    "Campus": [
      {
        "Nome": "Fortaleza",
        "Latitude": -3.7436587246947785,
        "Longitude": -38.5410718062838,
        "RaioPermitidoMetros": 2000
      }
    ]
  }
}
```

## Fluxos Principais

### Check-in em Evento (Presenca)

1. Usuário envia localização + ID do evento
2. Sistema valida se usuário está no raio do evento
3. Sistema valida se evento está em andamento
4. Cria registro de `Presenca` com check-in

### Check-in Diário (PIN)

1. Administrador gera PIN
2. Usuário envia PIN + localização
3. Sistema valida se usuário está no campus
4. Sistema valida PIN ativo
5. Cria registro de `Checkin`

## Boas Práticas

1. **Entidades**: Sempre usar métodos de fábrica (`Criar()`)
2. **Validações**: Regras de negócio no Domain
3. **Mapeamento**: Usar `IMapeadorDeEntidades` para conversões
4. **Transações**: Usar `IUoW.CommitAsync()` ao final das operações
5. **Logs**: Usar `ILogger` nos serviços de aplicação
