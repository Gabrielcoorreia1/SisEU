using SisEUs.Application.Apresentacoes.DTOs.Respostas;
using SisEUs.Application.Comum.DTOs;
using SisEUs.Domain.ContextoDeEvento.Enumeracoes;

namespace SisEUs.Application.Eventos.DTOs.Resposta
{
    /// <summary>
    /// DTO completo para representar um evento com todas as suas informações.
    /// </summary>
    public record EventoResposta(
        int Id,
        string Titulo,
        LocalResposta Local,
        LocalizacaoResposta Localizacao,
        DataFormatadaResposta DataInicio,
        DataFormatadaResposta DataFim,
        IReadOnlyCollection<ParticipanteResposta> Organizadores,
        List<string> Avaliadores,
        string ETipoEvento,
        ETipoEvento EventType,
        string ImgUrl,
        string CodigoUnico,
        string NomeCampus,
        DateTime DataDateTime,
        string? PinCheckin,
        List<ApresentacaoResposta> Apresentacoes
    );
}