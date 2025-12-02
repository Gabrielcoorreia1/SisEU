using SisEUs.Domain.Comum.Excecoes;

namespace SisEUs.Domain.ContextoDeUsuario.Excecoes
{
    public class EmailInvalidoExcecao()
        : ExcecaoDeDominio("O e-mail informado é inválido. Verifique o formato e tente novamente.");
}
