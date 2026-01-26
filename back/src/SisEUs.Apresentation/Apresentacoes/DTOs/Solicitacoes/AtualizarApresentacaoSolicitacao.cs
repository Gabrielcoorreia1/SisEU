namespace SisEUs.Application.Apresentacoes.DTOs.Solicitacoes
{
    public record AtualizarApresentacaoSolicitacao
    (
        int Id,
        string Titulo,
        string NomeAutor,
        string NomeOrientador
    );
}
