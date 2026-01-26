using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace SisEUs.Infrastructure.Token
{
    public abstract class JwtTokenHandler
    {
        protected static SymmetricSecurityKey SecurityKey(string signingKey)
        {
            var bytes = Encoding.UTF8.GetBytes(signingKey);

            return new SymmetricSecurityKey(bytes);
        }
    }
}
