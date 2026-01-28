using SisEUs.Domain.Comum.Excecoes;

namespace SisEUs.Domain.ContextoDeUsuario.Excecoes
{
    public class UsuarioJaEAvaliadorExcecao : ExcecaoDeDominio
    {
        public UsuarioJaEAvaliadorExcecao() 
            : base("O usuário já é um avaliador.")
        {
        }
    }
}