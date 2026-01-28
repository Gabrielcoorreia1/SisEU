using SisEUs.Application.Comum.Resultados;
using SisEUs.Application.Eventos.DTOs.Resposta;
using SisEUs.Application.Eventos.DTOs.Solicitacoes;

namespace SisEUs.Application.Eventos.Abstracoes
{
    public interface IEventoServico
    {
        Task<Resultado<EventoResposta>> CriarEventoAsync(CriarEventoSolicitacao request, CancellationToken cancellationToken);

        Task<Resultado> AtualizarEventoAsync(int id, AtualizarEventoSolicitacao request, CancellationToken cancellationToken);

        Task<Resultado> ExcluirEventoAsync(int eventoId, CancellationToken cancellationToken);

        Task<Resultado<EventoResposta>> ObterEventoPorIdAsync(int eventoId, CancellationToken cancellationToken);

        Task<Resultado<IEnumerable<EventoResposta>>> ListarEventosAsync(int pagina, int tamanho, CancellationToken cancellationToken);

        Task<Resultado> AdicionarParticipanteAsync(int participanteId, int eventoId, CancellationToken cancellationToken);

        Task<Resultado> RemoverParticipanteAsync(int participanteId, int eventoId, CancellationToken cancellationToken);
        
        Task<Resultado> AdicionarAvaliadorPorCpfAsync(string cpf, int eventoId, CancellationToken cancellationToken);
        
        Task<Resultado> AdicionarAvaliadorAsync(int avaliadorId, int eventoId, CancellationToken cancellationToken);
        
        Task<Resultado> RemoverAvaliadorAsync(int avaliadorId, int eventoId, CancellationToken cancellationToken);
        
        Task<Resultado<EventoResposta>> ObterPorCodigoEvento(string codigoEvento, CancellationToken cancellationToken);
        
        Task<Resultado<IEnumerable<EventoResposta>>> ObterEventosPorAvaliadorAsync(int avaliadorId, CancellationToken cancellationToken);
    }
}
