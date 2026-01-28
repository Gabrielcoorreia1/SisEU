using SisEUs.Application.Apresentacoes.DTOs.Solicitacoes;
using SisEUs.Domain.ContextoDeEvento.Enumeracoes;

namespace SisEUs.Application.Eventos.DTOs.Solicitacoes
{
    public record CriarEventoSolicitacao
    (
        string Titulo,
        DateTime DataInicio,
        DateTime DataFim,
        CriarLocalSolicitacao Local,
        ETipoEvento ETipoEvento,
        List<string>? AvaliadoresCpfs,
        string ImgUrl,
        string CodigoUnico,
        List<CriarApresentacaoSolicitacao> Apresentacoes
    );
}
