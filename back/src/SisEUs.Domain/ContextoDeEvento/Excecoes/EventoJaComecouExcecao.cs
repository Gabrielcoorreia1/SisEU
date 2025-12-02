using SisEUs.Domain.Comum.Excecoes;

namespace SisEUs.Domain.ContextoDeEvento.Excecoes
{
    public class EventoJaComecouExcecao()
        : ExcecaoDeDominio("O evento já começou e não pode ser modificado ou cancelado.");
}
