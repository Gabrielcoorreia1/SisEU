namespace SisEUs.Application.Authenticacoes.DTOs.Solicitacoes
{
    public record RegistrarSolicitacao
    (
        string PrimeiroNome,
        string Sobrenome,
        string Cpf,
        string Email,
        string Senha
    );
}