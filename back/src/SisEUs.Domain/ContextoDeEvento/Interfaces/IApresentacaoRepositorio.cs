using SisEUs.Domain.ContextoDeEvento.Entidades;

namespace SisEUs.Domain.ContextoDeEvento.Interfaces
{
    public interface IApresentacaoRepositorio
    {
        Task AdicionarAsync(Apresentacao apresentacao, CancellationToken cancellationToken = default);
        void Remover(Apresentacao apresentacao);
        Task<Apresentacao?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Apresentacao>> ObterPorEventoIdAsync(int eventoId, CancellationToken cancellationToken = default);
    }
}
