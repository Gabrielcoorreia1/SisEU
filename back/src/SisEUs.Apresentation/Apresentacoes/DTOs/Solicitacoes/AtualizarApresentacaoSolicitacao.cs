namespace SisEUs.Application.Apresentacoes.DTOs.Solicitacoes
{
    public record AtualizarApresentacaoSolicitacao
    (
        int Id,
        string Titulo,
        string CpfAutor,
        string CpfOrientador
    );
}
