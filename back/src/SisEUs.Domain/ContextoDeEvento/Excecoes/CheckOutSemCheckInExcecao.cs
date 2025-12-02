using SisEUs.Domain.Comum.Excecoes;

namespace SisEUs.Domain.ContextoDeEvento.Excecoes
{
    public class CheckOutSemCheckInExcecao()
        : ExcecaoDeDominio("Não é possível realizar o check-out sem um check-in prévio.");
}
