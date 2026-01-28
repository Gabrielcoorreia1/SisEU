namespace SisEUs.Application.Eventos.DTOs.Resposta
{
    /// <summary>
    /// Representa uma data formatada por extenso com hora separada.
    /// </summary>
    public record DataFormatadaResposta(
        string DataPorExtenso,
        string Hora
    );
}
