
namespace SisEUs.Apresentation.Authenticacoes.DTOs.Solicitacoes
{
    public class RegistrarSolicitacao
    {
        public string PrimeiroNome { get; set; } = null!;
        public string Sobrenome { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Senha { get; set; } = null!;
    }
}