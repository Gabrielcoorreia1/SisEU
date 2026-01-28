namespace SisEUs.Application.Checkin.DTOs.Solicitacoes
{
    /// <summary>
    /// DTO para registro de check-in com PIN e localização.
    /// </summary>
    public record RegistrarCheckinSolicitacao(
        string Pin,
        string Latitude,
        string Longitude
    );
}
