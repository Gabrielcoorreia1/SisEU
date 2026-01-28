using SisEUs.Application.Apresentacoes.DTOs.Solicitacoes;
using SisEUs.Domain.ContextoDeEvento.Enumeracoes;

namespace SisEUs.Application.Eventos.DTOs.Solicitacoes
{
    public record AtualizarEventoSolicitacao
    (
        int Id,
        string Titulo,
        DateTime DataInicio,
        DateTime DataFim,
        CriarLocalSolicitacao Local,
        ETipoEvento ETipoEvento,
        string ImgUrl,
        string CodigoUnico,
        List<string>? CpfsAvaliadores,
        List<CriarApresentacaoSolicitacao> Apresentacoes
    );
}
