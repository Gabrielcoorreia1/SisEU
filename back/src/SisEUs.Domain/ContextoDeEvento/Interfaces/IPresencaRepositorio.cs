using SisEUs.Domain.ContextoDeEvento.Entidades;

namespace SisEUs.Domain.ContextoDeEvento.Interfaces
{
    /// <summary>
    /// Define o contrato para operações de persistência relacionadas à entidade Presenca.
    /// </summary>
    public interface IPresencaRepositorio
    {
        /// <summary>
        /// Adiciona um novo registro de presença ao banco de dados de forma assíncrona.
        /// </summary>
        /// <param name="presenca">A entidade de presença a ser criada.</param>
        /// <param name="cancellationToken">Token para monitorar solicitações de cancelamento.</param>
        Task CriarPresenca(Presenca presenca, CancellationToken cancellationToken = default!);

        /// <summary>
        /// Obtém todos os registros de presença de um usuário específico.
        /// </summary>
        /// <param name="usuarioId">O ID do usuário.</param>
        /// <param name="cancellationToken">Token para monitorar solicitações de cancelamento.</param>
        /// <returns>Uma coleção de registros de presença do usuário.</returns>
       Task<IEnumerable<Presenca>?> ObterPresencaDeUsuario(int usuarioId, CancellationToken cancellationToken = default!);

        /// <summary>
        /// Obtém todos os registros de presença do sistema.
        /// </summary>
        /// <returns>Uma coleção com todos os registros de presença.</returns>
        /// <remarks>
        /// Evite usar esse metodo caso o volume de presenças tenha crescido muito, pois ele pode impactar a performance do sistema.
        /// recomenda-se o metodo de paginação para obter presenças de forma mais eficiente.
        /// </remarks>
        Task<IEnumerable<Presenca>> ObterPresencas();

        /// <summary>
        /// Obtém um registro de presença específico pelo seu ID.
        /// </summary>
        /// <param name="presencaId">O ID do registro de presença.</param>
        /// <param name="cancellationToken">Token para monitorar solicitações de cancelamento.</param>
        /// <returns>A entidade de presença encontrada ou nulo se não existir.</returns>
        Task<Presenca?> ObterPresencaPorIdAsync(int presencaId, CancellationToken cancellationToken = default!);

        /// <summary>
        /// Busca por um registro de presença específico combinando o ID do evento e o ID do usuário.
        /// </summary>
        /// <param name="eventoId">O ID do evento.</param>
        /// <param name="usuarioId">O ID do usuário.</param>
        /// <param name="cancellationToken">Token para monitorar solicitações de cancelamento.</param>
        /// <returns>A entidade de presença encontrada ou nulo se não existir a combinação.</returns>
        Task<Presenca?> BuscarPorUsuarioEEventoAsync(int eventoId, int usuarioId, CancellationToken cancellationToken = default!);
        Task<Presenca?> ObterStatusPresencaPorEvento(int eventoId, int usuarioId, CancellationToken cancellationToken = default!);
        Task<Presenca?> ObterPresencaEventoEmAndamentoAsync(CancellationToken cancellationToken = default!);
    }
}
