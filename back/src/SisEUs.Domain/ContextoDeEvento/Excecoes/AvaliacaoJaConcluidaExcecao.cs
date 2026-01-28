using SisEUs.Domain.Comum.Excecoes;

namespace SisEUs.Domain.ContextoDeEvento.Excecoes
{
    public class AvaliacaoJaConcluidaExcecao()
        : ExcecaoDeDominio("A avaliação já foi concluída e não pode ser editada.");
}
