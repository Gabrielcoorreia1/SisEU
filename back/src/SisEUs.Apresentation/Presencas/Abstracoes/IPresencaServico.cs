using SisEUs.Application.Comum.Resultados;
using SisEUs.Application.Presencas.DTOs.Respostas;
using SisEUs.Application.Presencas.DTOs.Solicitacoes;

namespace SisEUs.Application.Presencas.Abstracoes
{
    public interface IPresencaServico
    {
        Task<Resultado<PresencaResposta>> EfetuarCheckInAsync(EfetuarCheckInSolicitacao request, CancellationToken cancellationToken);
        Task<Resultado<PresencaResposta>> EfetuarCheckOutAsync(EfetuarCheckOutSolicitacao request, CancellationToken cancellationToken);
        Task<Resultado<PresencaResposta>> ObterPorIdAsync(int id, CancellationToken cancellationToken);
        Task<Resultado<IEnumerable<PresencaResposta>>> ListarPorEventoAsync(int eventoId, CancellationToken cancellationToken);
        Task<Resultado<IEnumerable<PresencaResposta>>> ObterRelatorioAsync(CancellationToken cancellationToken = default!);
        Task<Resultado<StatusPresencaResposta>> ObterStatusPresencaEventoAsync(int eventoId, CancellationToken cancellationToken = default!);
        Task<Resultado<bool?>> ObterPresencaEventoEmAndamentoAsync(CancellationToken cancellationToken = default!);
    }
}
