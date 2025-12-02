using SisEUs.Application.Apresentacoes.DTOs.Solicitacoes;
using SisEUs.Domain.ContextoDeEvento.Enumeracoes;

namespace SisEUs.Application.Eventos.DTOs.Solicitacoes
{
    public class CriarEventoSolicitacao
    {
        public string Titulo { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataFim { get; set; }
        public CriarLocalSolicitacao Local { get; set; }
        public ETipoEvento ETipoEvento { get; set; }
        public List<string>? Avaliadores { get; set; }
        public string ImgUrl { get; set; }
        public string CodigoUnico { get; set; }
        public List<CriarApresentacaoSolicitacao> Apresentacoes { get; set; }
    }
}
