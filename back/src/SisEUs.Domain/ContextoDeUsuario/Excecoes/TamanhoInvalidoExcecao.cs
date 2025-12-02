using SisEUs.Domain.Comum.Excecoes;

namespace SisEUs.Domain.ContextoDeUsuario.Excecoes
{
    public class TamanhoInvalidoExcecao(int min, int max)
        : ExcecaoDeDominio($"O tamanho invalido. Deve ter entre {min} e {max} caracteres.");
}
