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

            var identifier = jwtSecurityToken.Claims.First(c => c.Type == ClaimTypes.Sid).Value;

            var userIdentifier = Guid.Parse(identifier);

            return await context
                .Usuarios
                .AsNoTracking()
                .FirstAsync(user => user.UserIdentifier == userIdentifier);
        }
    }
}
