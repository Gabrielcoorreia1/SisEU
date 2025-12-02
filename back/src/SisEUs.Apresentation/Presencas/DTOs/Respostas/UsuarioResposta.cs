using SisEUs.Domain.ContextoDeUsuario.Enumeracoes;
using SisEUs.Domain.ContextoDeUsuario.ObjetosDeValor;

namespace SisEUs.Application.Presencas.DTOs.Respostas
{
    public class UsuarioResposta
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string Email { get; set; } = null!;
    }
}
