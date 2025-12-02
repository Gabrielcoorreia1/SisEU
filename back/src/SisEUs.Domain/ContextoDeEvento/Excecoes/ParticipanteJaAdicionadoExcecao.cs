using SisEUs.Domain.Comum.Excecoes;

namespace SisEUs.Domain.ContextoDeEvento.Excecoes
{
    public class ParticipanteJaAdicionadoExcecao(int participanteId)
        : ExcecaoDeDominio($"O participante {participanteId} já foi adicionado ao evento.");
}
