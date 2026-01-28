namespace SisEUs.Application.Avaliacoes.DTOs.Respostas
{
    public record AvaliacoesPendentesResposta
    (
        int AvaliacaoId,
        string NomeEvento,
        DateTime DataEvento,
        string LocalEvento,
        string NomeProfessor,
        int ProfessorId
    );
}
