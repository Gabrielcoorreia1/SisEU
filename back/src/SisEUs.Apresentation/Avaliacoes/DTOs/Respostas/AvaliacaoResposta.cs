using SisEUs.Domain.ContextoDeEvento.Enumeracoes;

namespace SisEUs.Application.Avaliacoes.DTOs.Respostas
{
    public record AvaliacaoResposta(
        int Id,
        int ApresentacaoId,
        string TituloApresentacao,
        string NomeAutor,
        EModalidadeApresentacao Modalidade,
        int AvaliadorId,
        decimal? Nota,
        string? Parecer,
        string Estado,
        DateTime? DataInicio,
        DateTime? DataConclusao
    );
}
