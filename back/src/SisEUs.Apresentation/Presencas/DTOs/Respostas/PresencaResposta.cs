using SisEUs.Application.Eventos.DTOs.Resposta;

namespace SisEUs.Application.Presencas.DTOs.Respostas
{
    public class PresencaResposta
    {
        public int Id { get; set; }
        public UsuarioResposta Usuario { get; set; }
        public EventoResposta Evento { get; set; }
        public DateTime? DataCheckIn { get; set; }
        public DateTime? DataCheckOut { get; set; }
        public LocalizacaoResposta Localizacao { get; set; }
    }
}
