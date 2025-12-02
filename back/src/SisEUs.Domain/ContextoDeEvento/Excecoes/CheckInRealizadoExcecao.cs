using SisEUs.Domain.Comum.Excecoes;

namespace SisEUs.Domain.ContextoDeEvento.Excecoes
{
    public class CheckInRealizadoExcecao()
        : ExcecaoDeDominio("O check-in já foi realizado para este evento. Não é possível realizar novamente.");
}
