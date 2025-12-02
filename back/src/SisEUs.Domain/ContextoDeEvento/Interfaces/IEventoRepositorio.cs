using SisEUs.Domain.ContextoDeEvento.Entidades;

namespace SisEUs.Domain.ContextoDeEvento.Interfaces
{
    /// <summary>
    /// Define o contrato para operações de persistência relacionadas à entidade Evento.
    /// </summary>
    public interface IEventoRepositorio
    {
        /// <summary>
        /// Obtém um evento pelo seu código único (GUID).
        /// </summary>
        /// <param name="codigo">O código único do evento.</param>
        /// <param name="cancellationToken">Token para monitorar solicitações de cancelamento.</param>
        /// <returns>A entidade de evento encontrada ou nulo se não existir.</returns>
        Task<Evento?> ObterEventoPorCodigoAsync(string codigo, CancellationToken cancellationToken = default!);

        /// <summary>
        /// Obtém um evento pelo seu ID de chave primária.
        /// </summary>
        /// <param name="eventoId">O ID do evento.</param>
        /// <param name="cancellationToken">Token para monitorar solicitações de cancelamento.</param>
        /// <returns>A entidade de evento encontrada ou nulo se não existir.</returns>
        Task<Evento?> ObterEventoPorIdAsync(int eventoId, CancellationToken cancellationToken = default!);

        /// <summary>
        /// Obtém todos os eventos do sistema.
        /// </summary>
        /// <remarks>
        /// ATENÇÃO: Use com cuidado em produção. Prefira o método <see cref="ObterEventosPaginadosAsync"/>.
        /// </remarks>
        /// <returns>Uma coleção de todos os eventos.</returns>
        Task<IEnumerable<Evento>> ObterEventosAsync();

        /// <summary>
        /// Obtém uma lista paginada de eventos.
        /// </summary>
        /// <param name="skip">O número de registros a serem ignorados (para paginação).</param>
        /// <param name="take">O número máximo de registros a serem retornados.</param>
        /// <param name="cancellationToken">Token para monitorar solicitações de cancelamento.</param>
        /// <returns>Uma coleção paginada de eventos.</returns>
        Task<IEnumerable<Evento>> ObterEventosPaginadosAsync(int skip, int take, CancellationToken cancellationToken = default!);

        /// <summary>
        /// Adiciona uma nova entidade de evento ao contexto para ser persistida.
        /// </summary>
        /// <param name="evento">A entidade de evento a ser criada.</param>
        /// <param name="cancellationToken">Token para monitorar solicitações de cancelamento.</param>
        Task CriarEventoAsync(Evento evento, CancellationToken cancellationToken = default!);

        /// <summary>
        /// Marca uma entidade de evento para exclusão. A exclusão será efetivada ao salvar as mudanças na unidade de trabalho.
        /// </summary>
        /// <param name="evento">A entidade de evento a ser excluída.</param>
        void ExcluirEvento(Evento evento);
    }
}
