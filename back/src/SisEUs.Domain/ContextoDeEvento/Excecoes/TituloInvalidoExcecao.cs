using SisEUs.Domain.Comum.Excecoes;
using SisEUs.Domain.ContextoDeEvento.ObjetosDeValor;

namespace SisEUs.Domain.ContextoDeEvento.Excecoes
{
    public class TituloInvalidoExcecao(string titulo)
        : ExcecaoDeDominio($"O título '{titulo}' é inválido. Deve conter entre {Titulo.TamanhoMin} e {Titulo.TamanhoMax} caracteres.");
}
