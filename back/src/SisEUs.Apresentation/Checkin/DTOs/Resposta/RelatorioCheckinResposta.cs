namespace SisEUs.Application.Checkin.DTOs.Resposta
{
    public record RelatorioCheckinResposta
    (
        string NomeCompleto,
        string Cpf,
        string Email,
        string PinUsado,
        string DataCheckin,
        string HoraCheckin,
        string DataCheckout,
        string HoraCheckout,
        string? Matricula,
        double Latitude,
        double Longitude
    );
}