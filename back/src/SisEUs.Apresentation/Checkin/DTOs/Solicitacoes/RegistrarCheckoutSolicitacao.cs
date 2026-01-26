namespace SisEUs.Application.Checkin.DTOs.Solicitacoes
{
    /// <summary>
    /// DTO para registro de check-out com localização.
    /// </summary>
    public record RegistrarCheckoutSolicitacao(
        string Latitude,
        string Longitude
    );
}
