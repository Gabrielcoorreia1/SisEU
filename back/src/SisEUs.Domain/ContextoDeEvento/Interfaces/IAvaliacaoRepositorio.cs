using SisEUs.Domain.ContextoDeEvento.Entidades;

namespace SisEUs.Domain.ContextoDeEvento.Interfaces
{
    public interface IAvaliacaoRepositorio
    {
        Task AdicionarAsync(Avaliacao avaliacao, CancellationToken cancellationToken = default);
        Task<Avaliacao?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task<Avaliacao?> ObterPorApresentacaoEAvaliadorAsync(int apresentacaoId, int avaliadorId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Avaliacao>> ObterPorApresentacaoAsync(int apresentacaoId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Avaliacao>> ObterPorAvaliadorAsync(int avaliadorId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Avaliacao>> ObterPorEventoAsync(int eventoId, CancellationToken cancellationToken = default);
    }
}
