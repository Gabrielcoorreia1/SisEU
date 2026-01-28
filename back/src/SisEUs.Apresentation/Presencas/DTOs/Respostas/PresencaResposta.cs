using SisEUs.Application.Comum.DTOs;
using SisEUs.Application.Eventos.DTOs.Resposta;

namespace SisEUs.Application.Presencas.DTOs.Respostas
{
    /// <summary>
    /// DTO para representar uma presença completa com check-in/check-out.
    /// </summary>
    public record PresencaResposta(
        int Id,
        UsuarioResposta Usuario,
        EventoResposta? Evento,
        DateTime? DataCheckIn,
        DateTime? DataCheckOut,
        LocalizacaoResposta Localizacao
    );
}
