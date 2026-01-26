using SisEUs.Application.Eventos.DTOs.Resposta;

namespace SisEUs.Application.Apresentacoes.DTOs.Respostas
{
    public record ApresentacaoResposta
    (
        int Id,
        EventoResposta Evento,
        string Titulo,
        string NomeAutor,
        string NomeOrientador
    );
}
