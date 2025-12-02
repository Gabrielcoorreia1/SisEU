using SisEUs.Domain.Comum.Excecoes;

namespace SisEUs.Domain.ContextoDeEvento.Excecoes
{
    public class DataPassadaExcecao(string acao)
        : ExcecaoDeDominio($"A data para {acao} não pode ser no passado. Por favor, forneça uma data valida.");
}
