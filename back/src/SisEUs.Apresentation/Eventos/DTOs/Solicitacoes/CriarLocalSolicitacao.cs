using SisEUs.Application.Comum.Enumeracoes;

namespace SisEUs.Application.Eventos.DTOs.Solicitacoes
{
    /// <summary>
    /// DTO para criação de local de um evento.
    /// </summary>
    public record CriarLocalSolicitacao(
        Campus Campus,
        string Departamento,
        string Bloco,
        string Sala
    );
}
