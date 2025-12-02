using SisEUs.Application.Apresentacoes.DTOs.Respostas;
using SisEUs.Application.Eventos.DTOs.Solicitacoes;
using SisEUs.Application.Presencas.DTOs.Respostas;
using SisEUs.Domain.ContextoDeEvento.Enumeracoes;

namespace SisEUs.Application.Eventos.DTOs.Resposta
{
    public class EventoResposta
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public CriarLocalSolicitacao Local { get; set; }
        public LocalizacaoResposta Localizacao { get; set; }
        public Data DataInicio { get; set; }
        public Data DataFim { get; set; }
        public IReadOnlyCollection<ParticipanteResposta> Organizadores { get; set; }
        public List<string> Avaliadores { get; set; }
        public string ETipoEvento { get; set; }
        public ETipoEvento EventType { get; set; }
        public string ImgUrl { get; set; }
        public string CodigoUnico { get; set; }
        public string NomeCampus { get; set; }
        public DateTime DataDateTime { get; set; }

        public string? PinCheckin { get; set; }
        public List<ApresentacaoResposta> Apresentacoes { get; set; }
    }
}
