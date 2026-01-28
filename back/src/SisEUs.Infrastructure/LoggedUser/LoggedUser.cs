using Microsoft.EntityFrameworkCore;
using SisEUs.Domain.Comum.LoggedUser;
using SisEUs.Domain.Comum.Token;
using SisEUs.Domain.ContextoDeUsuario.Entidades;
using SisEUs.Infrastructure.Repositorios;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SisEUs.Infrastructure.LoggedUser
{
    public class LoggedUser(AppDbContext context, ITokenProvider tokenProvider) : ILoggedUser
    {
        public async Task<Usuario> User()
        {
            var token = tokenProvider.Value();

            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtSecurityToken = tokenHandler.ReadJwtToken(token);

            var identifierClaim = jwtSecurityToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);
            if (identifierClaim == null)
            {
                throw new UnauthorizedAccessException("Token inválido: Claim SID não encontrado.");
            }

            var identifier = identifierClaim.Value;
            var userIdentifier = Guid.Parse(identifier);

            var user = await context
                .Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(user => user.UserIdentifier == userIdentifier);

            if (user == null)
            {
                throw new UnauthorizedAccessException($"Usuário não encontrado para o identificador: {userIdentifier}");
            }

            return user;
        }
    }
}
