using SisEUs.Domain.ContextoDeUsuario.Enumeracoes;

namespace SisEUs.Application.Authenticacoes.DTOs.Resposta
{
    public record LoginResposta
    (
        string Token,
        ETipoUsuario TipoUsuario,
        string NomeCompleto,
        string Cpf,
        int UsuarioId
    );
}