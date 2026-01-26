using SisEUs.Domain.ContextoDeEvento.Enumeracoes;

namespace SisEUs.Application.Apresentacoes.DTOs.Solicitacoes
{
    public record CriarApresentacaoSolicitacao
    (
        long? Id,
        int EventoId,
        string Titulo,
        string NomeAutor,
        string NomeOrientador,
        EModalidadeApresentacao Modalidade
    );
}
