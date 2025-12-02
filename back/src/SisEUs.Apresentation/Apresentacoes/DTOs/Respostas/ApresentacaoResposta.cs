using SisEUs.Application.Eventos.DTOs.Resposta;

namespace SisEUs.Application.Apresentacoes.DTOs.Respostas
{
    public class ApresentacaoResposta
    {
        public int Id { get; set; }
        public EventoResposta Evento { get; set; }
        public string Titulo { get; set; }
        public string NomeAutor { get; set; }
        public string NomeOrientador { get; set; }
    }
}
