using Microsoft.IdentityModel.Tokens;
using SisEUs.Domain.Comum.Token;
using SisEUs.Domain.ContextoDeUsuario.Entidades;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SisEUs.Infrastructure.Token
{
    public class JwtTokenGenerator(uint expirationTimeMinutes, string signinKey) : IAccessTokenGenerator
    {
        public string Generate(Usuario usuario)
        {
            var claims = new List<Claim>()
            {
                new(JwtRegisteredClaimNames.Sub, usuario.UserIdentifier.ToString()),
                new(ClaimTypes.Sid, usuario.UserIdentifier.ToString()),
                new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new(JwtRegisteredClaimNames.Email, usuario.Email.Valor),
                new(ClaimTypes.Role, usuario.EUserType.ToString()),
                new("role", usuario.EUserType.ToString()),
                new("nome", usuario.Nome.ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expirationTimeMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signinKey)), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(securityToken);
        }
    }
}