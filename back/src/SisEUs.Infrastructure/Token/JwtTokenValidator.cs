using Microsoft.IdentityModel.Tokens;
using SisEUs.Domain.Comum.Token;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SisEUs.Infrastructure.Token
{
    public class JwtTokenValidator(string signingKey) : IAccessTokenValidator
    {
        public ClaimsPrincipal ValidateAndGetUserPrincipal(string token)
        {
            var validationParameter = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                ClockSkew = TimeSpan.FromMinutes(5),
                RequireExpirationTime = false,
                ValidateLifetime = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var principal = tokenHandler.ValidateToken(token, validationParameter, out _);

            return principal;
        }
    }
}