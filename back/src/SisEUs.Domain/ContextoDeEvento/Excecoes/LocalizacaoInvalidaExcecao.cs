using SisEUs.Domain.Comum.Excecoes;

namespace SisEUs.Domain.ContextoDeEvento.Excecoes
{
    public class LocalizacaoInvalidaExcecao()
        : ExcecaoDeDominio("A localização fornecida é inválida. Deve conter latitude e longitude válidas.");
}
