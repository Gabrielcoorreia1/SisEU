namespace SisEUs.Application.Comum.DTOs
{
    /// <summary>
    /// DTO padrão para representar coordenadas de localização.
    /// </summary>
    public record LocalizacaoResposta(
        string Latitude,
        string Longitude
    );
}
