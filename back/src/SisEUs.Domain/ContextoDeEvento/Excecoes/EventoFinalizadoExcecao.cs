using SisEUs.Domain.Comum.Excecoes;

namespace SisEUs.Domain.ContextoDeEvento.Excecoes
{
    public class EventoFinalizadoExcecao(string acao)
        : ExcecaoDeDominio($"Não é possível realizar a ação '{acao}' em um evento que já foi finalizado.");
}
