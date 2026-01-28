using SisEUs.Application.Comum.DTOs;

namespace SisEUs.Application.Eventos.DTOs.Resposta
{
    /// <summary>
    /// DTO para resposta de busca de usuários organizadores.
    /// </summary>
    public record BuscarUsuariosResposta(
        List<UsuarioResposta> Organizadores
    );
}
