using System.ComponentModel.DataAnnotations;

namespace SisEUs.Application.Avaliacoes.DTOs.Solicitacoes
{
    public record EnviarAvaliacaoSolicitacao(
        [Range(0, 10, ErrorMessage = "A nota deve estar entre 0 e 10.")]
        decimal Nota,
        
        [MaxLength(2000, ErrorMessage = "O parecer deve ter no máximo 2000 caracteres.")]
        string? Parecer
    );
}
