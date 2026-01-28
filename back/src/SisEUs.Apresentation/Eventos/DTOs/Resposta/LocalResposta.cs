namespace SisEUs.Application.Eventos.DTOs.Resposta
{
    /// <summary>
    /// DTO para representar informações de local de um evento.
    /// </summary>
    public record LocalResposta(
        string Campus,
        string Departamento,
        string Bloco,
        string Sala
    );
}
