using SisEUs.Domain.Comum.Excecoes;

namespace SisEUs.Domain.ContextoDeEvento.Excecoes
{
    public class CheckOutRealizadoExcecao()
        : ExcecaoDeDominio("O check-out já foi realizado para este evento. Não é possível realizar novamente.");
}
