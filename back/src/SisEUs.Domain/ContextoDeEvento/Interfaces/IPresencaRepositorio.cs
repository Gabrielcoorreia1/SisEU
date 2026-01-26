using SisEUs.Domain.ContextoDeEvento.Entidades;

namespace SisEUs.Domain.ContextoDeEvento.Interfaces
{
    public interface IPresencaRepositorio
    {
        void CriarPresenca(Presenca presenca, CancellationToken cancellationToken = default);
        Task<IEnumerable<Presenca>>? ObterPresencaDeUsuario(int usuarioId, CancellationToken cancellationToken = default);

        [Obsolete("Use ObterPresencasPaginadas para evitar problemas de performance com grandes volumes de dados.")]
        Task<IEnumerable<Presenca>> ObterPresencas();

        Task<IEnumerable<Presenca>> ObterPresencasPaginadas(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default);
        Task<Presenca?> ObterPresencaPorIdAsync(int presencaId, CancellationToken cancellationToken = default);
        Task<Presenca?> BuscarPorUsuarioEEventoAsync(int eventoId, int usuarioId, CancellationToken cancellationToken = default);
        Task<Presenca?> ObterStatusPresencaPorEvento(int eventoId, int usuarioId, CancellationToken cancellationToken = default);
        Task<Presenca?> ObterPresencaEventoEmAndamentoAsync(int usuarioId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Presenca>> ObterPresencasPorEventoAsync(int eventoId, CancellationToken cancellationToken = default);
    }
}
