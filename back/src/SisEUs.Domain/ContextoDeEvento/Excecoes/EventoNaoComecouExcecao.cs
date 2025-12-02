using SisEUs.Domain.Comum.Excecoes;

namespace SisEUs.Domain.ContextoDeEvento.Excecoes
{
    public class EventoNaoComecouExcecao()
        : ExcecaoDeDominio("O Evento ainda não começou");
}
