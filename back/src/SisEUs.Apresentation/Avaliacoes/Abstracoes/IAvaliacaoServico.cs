using SisEUs.Application.Avaliacoes.DTOs.Respostas;
using SisEUs.Application.Avaliacoes.DTOs.Solicitacoes;
using SisEUs.Application.Comum.Resultados;

namespace SisEUs.Application.Avaliacoes.Abstracoes
{
    public interface IAvaliacaoServico
    {
        /// <summary>
        /// Inicia uma nova avaliação para uma apresentação (Estado: EmAvaliacao)
        /// </summary>
        Task<Resultado<AvaliacaoResposta>> IniciarAvaliacaoAsync(int apresentacaoId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Envia a avaliação com nota e parecer (Estado: Concluido). Após envio, não pode ser editada.
        /// </summary>
        Task<Resultado<AvaliacaoResposta>> EnviarAvaliacaoAsync(int avaliacaoId, decimal nota, string? parecer, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém uma avaliação pelo ID
        /// </summary>
        Task<Resultado<AvaliacaoResposta>> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lista todas as avaliações de uma apresentação
        /// </summary>
        Task<Resultado<IEnumerable<AvaliacaoResposta>>> ListarPorApresentacaoAsync(int apresentacaoId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Lista todas as avaliações do avaliador logado
        /// </summary>
        Task<Resultado<IEnumerable<AvaliacaoResposta>>> ListarMinhasAvaliacoesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Lista todas as avaliações de um evento
        /// </summary>
        Task<Resultado<IEnumerable<AvaliacaoResposta>>> ListarPorEventoAsync(int eventoId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém o relatório detalhado de notas e pareceres de uma apresentação específica
        /// </summary>
        Task<Resultado<RelatorioApresentacaoResposta>> ObterRelatorioApresentacaoAsync(int apresentacaoId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtém o relatório consolidado de notas e pareceres de todas as apresentações de um evento
        /// </summary>
        Task<Resultado<RelatorioEventoResposta>> ObterRelatorioEventoAsync(int eventoId, CancellationToken cancellationToken = default);
        Task<Resultado<IEnumerable<IniciarAvaliacaoSolicitacao>>> ListarAvaliacoesPendentesAsync(CancellationToken cancellationToken = default);
    }
}
