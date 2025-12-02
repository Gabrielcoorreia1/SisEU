using SisEUs.Application.Presencas.DTOs.Respostas;
using SisEUs.Domain.ContextoDeUsuario.Entidades;

namespace SisEUs.Application.Authenticacoes.Mappers
{
    public static class UsuarioMapper
    {
        public static UsuarioResposta ToResponseDto(this Usuario usuario)
        {
            if (usuario is null) return null;

            return new UsuarioResposta
            {
                Id = usuario.Id,
                NomeCompleto = usuario.Nome.ToString(),
                Cpf = usuario.Cpf.Valor,
                Email = usuario.Email.Valor,
            };
        }
    }
}
