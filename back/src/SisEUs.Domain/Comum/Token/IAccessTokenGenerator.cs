using SisEUs.Domain.ContextoDeUsuario.Entidades;

namespace SisEUs.Domain.Comum.Token
{
    public interface IAccessTokenGenerator
    {
        string Generate(Usuario usuario);
    }
}