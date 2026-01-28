using SisEUs.Domain.ContextoDeEvento.Entidades;

namespace SisEUs.Domain.ContextoDeEvento.Interfaces
{
    public interface IEventoRepositorio
    {
        Task<Evento?> ObterEventoPorCodigoAsync(string codigo, CancellationToken cancellationToken = default!);
        Task<Evento?> ObterEventoPorIdAsync(int eventoId, CancellationToken cancellationToken = default!);
        Task<IEnumerable<Evento>> ObterEventosAsync();
        Task<IEnumerable<Evento>> ObterEventosPaginadosAsync(int skip, int take, CancellationToken cancellationToken = default!);
        Task<IEnumerable<Evento>> ObterEventosPorAvaliadorIdAsync(int avaliadorId, CancellationToken cancellationToken = default!);
        Task CriarEventoAsync(Evento evento, CancellationToken cancellationToken = default!);
        void ExcluirEvento(Evento evento);
        Task<bool> CodigoUnicoJaExisteAsync(string codigoUnico, int? eventoIdExcluir = null, CancellationToken cancellationToken = default!);
    }
}
