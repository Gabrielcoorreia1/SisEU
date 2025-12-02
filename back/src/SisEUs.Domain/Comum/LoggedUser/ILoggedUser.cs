using SisEUs.Domain.ContextoDeUsuario.Entidades;

namespace SisEUs.Domain.Comum.LoggedUser
{
    public interface ILoggedUser
    {
        public Task<Usuario> User();
    }
}
