using SisEUs.Domain.Comum.Excecoes;

namespace SisEUs.Domain.ContextoDeEvento.Excecoes
{
    public class ParticipanteNaoEncontradoExcecao()
        : ExcecaoDeDominio("O participante especificado não foi encontrado no evento.");
}
