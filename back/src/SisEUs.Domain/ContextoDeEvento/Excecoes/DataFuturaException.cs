using SisEUs.Domain.Comum.Excecoes;

namespace SisEUs.Domain.ContextoDeEvento.Excecoes
{
    public class DataFuturaException(string acao)
        : ExcecaoDeDominio($"A data para {acao} não pode ser no futuro. Por favor, forneça uma data válida.");
}
