namespace SisEUs.Application.Presencas.DTOs.Respostas
{
    /// <summary>
    /// DTO para representar o status de presença (check-in/check-out).
    /// </summary>
    public record StatusPresencaResposta(
        bool CheckInEfetuado,
        bool CheckOutEfetuado
    );
}
