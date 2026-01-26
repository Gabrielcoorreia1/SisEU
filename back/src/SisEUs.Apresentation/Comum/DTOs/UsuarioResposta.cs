namespace SisEUs.Application.Comum.DTOs
{
    /// <summary>
    /// DTO padrão para representar informações de um usuário em respostas.
    /// </summary>
    public record UsuarioResposta(
        int Id,
        string NomeCompleto,
        string Cpf,
        string Email
    );
}
