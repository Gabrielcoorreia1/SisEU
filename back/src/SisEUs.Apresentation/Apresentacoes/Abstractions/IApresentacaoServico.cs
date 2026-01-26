using SisEUs.Application.Apresentacoes.DTOs.Respostas;
using SisEUs.Application.Apresentacoes.DTOs.Solicitacoes;
using SisEUs.Application.Comum.Resultados;
using SisEUs.Domain.ContextoDeEvento.Entidades;

namespace SisEUs.Application.Apresentacoes.Abstractions
{
    public interface IApresentacaoServico
    {
        Task<Resultado<Apresentacao>> AdicionarAoContextoAsync(CriarApresentacaoSolicitacao request, CancellationToken cancellationToken);
        Task<Resultado<ApresentacaoResposta>> CriarApresentacaoAsync(CriarApresentacaoSolicitacao request, CancellationToken cancellationToken);
        Task<Resultado<ApresentacaoResposta>> ObterApresentacaoPorIdAsync(int apresentacaoId, CancellationToken cancellationToken);
        Task<Resultado<IEnumerable<ApresentacaoResposta>>> ObterApresentacoesPorEventoAsync(int eventoId, CancellationToken cancellationToken);
        Task<Resultado> ExcluirApresentacaoAsync(int apresentacaoId, CancellationToken cancellationToken);
        Task<Resultado> AtualizarApresentacaoAsync(int apresentacaoId, AtualizarApresentacaoSolicitacao request, CancellationToken cancellationToken);
    }
}
