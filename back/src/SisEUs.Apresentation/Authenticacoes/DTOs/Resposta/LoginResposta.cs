using SisEUs.Domain.ContextoDeUsuario.Enumeracoes;

namespace SisEUs.Apresentation.Authenticacoes.DTOs.Resposta
{
    public class LoginResposta
    {
        public string Token { get; set; } = null!;
        public ETipoUsuario TipoUsuario { get; set; }
        public string NomeCompleto { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public int UsuarioId { get; set; }
    }
}