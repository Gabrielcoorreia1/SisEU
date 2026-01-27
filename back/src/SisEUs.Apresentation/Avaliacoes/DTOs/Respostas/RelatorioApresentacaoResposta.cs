using SisEUs.Application.Comum.DTOs;
using SisEUs.Domain.ContextoDeEvento.Enumeracoes;

namespace SisEUs.Application.Avaliacoes.DTOs.Respostas
{
    /// <summary>
    /// Relatório de notas e pareceres de uma apresentação
    /// </summary>
    public record RelatorioApresentacaoResposta(
        int ApresentacaoId,
        string TituloApresentacao,
        UsuarioResposta Autor,
        UsuarioResposta Orientador,
        EModalidadeApresentacao Modalidade,
        int TotalAvaliacoes,
        int AvaliacoesConcluidas,
        int AvaliacoesPendentes,
        decimal? MediaNotas,
        decimal? MaiorNota,
        decimal? MenorNota,
        IEnumerable<AvaliacaoDetalhadaResposta> Avaliacoes
    );

    /// <summary>
    /// Detalhes de uma avaliação individual no relatório
    /// </summary>
    public record AvaliacaoDetalhadaResposta(
        int AvaliacaoId,
        int AvaliadorId,
        string NomeAvaliador,
        decimal? Nota,
        string? Parecer,
        string Estado,
        DateTime? DataInicio,
        DateTime? DataConclusao
    );
}
