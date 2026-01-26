using SisEUs.Domain.ContextoDeUsuario.Entidades;
using SisEUs.Domain.ContextoDeUsuario.ObjetosDeValor;

namespace SisEUs.Domain.ContextoDeUsuario.Interfaces
{
    public interface IUsuarioRepositorio
    {
        Task<Usuario?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Usuario>> ObterPorIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
        Task<Usuario?> ObterPorEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<bool> CpfJaExisteAsync(Cpf cpf, CancellationToken cancellationToken = default);
        Task<bool> EmailJaExisteAsync(Email email, CancellationToken cancellationToken = default);
        Task AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default);
        Task<Usuario?> ObterPorCpfAsync(Cpf cpf, CancellationToken cancellationToken = default);
        Task<IEnumerable<Usuario>> BuscarPorNomeProfessorAsync(string nome, CancellationToken cancellationToken = default);
        Task<IEnumerable<Usuario>> ObterTodosUsuariosAsync();
        Task<Usuario?> ObterPorUserIdentifierAsync(Guid userIdentifier, CancellationToken cancellationToken = default);
    }
}