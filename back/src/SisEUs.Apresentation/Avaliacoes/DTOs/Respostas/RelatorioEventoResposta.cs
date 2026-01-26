using SisEUs.Domain.ContextoDeEvento.Enumeracoes;

namespace SisEUs.Application.Avaliacoes.DTOs.Respostas
{
    /// <summary>
    /// Relatório consolidado de notas e pareceres de um evento
    /// </summary>
    public record RelatorioEventoResposta(
        int EventoId,
        string TituloEvento,
        DateTime DataInicio,
        DateTime DataFim,
        int TotalApresentacoes,
        int TotalAvaliacoes,
        int AvaliacoesConcluidas,
        decimal? MediaGeralNotas,
        IEnumerable<ResumoApresentacaoResposta> Apresentacoes
    );

    /// <summary>
    /// Resumo de uma apresentação no relatório do evento
    /// </summary>
    public record ResumoApresentacaoResposta(
        int ApresentacaoId,
        string TituloApresentacao,
        string NomeAutor,
        EModalidadeApresentacao Modalidade,
        int TotalAvaliacoes,
        int AvaliacoesConcluidas,
        decimal? MediaNotas,
        decimal? MaiorNota,
        decimal? MenorNota,
        string? Classificacao
    );
}
