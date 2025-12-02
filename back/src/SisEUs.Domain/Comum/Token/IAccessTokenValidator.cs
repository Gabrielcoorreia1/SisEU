using System.Security.Claims;

namespace SisEUs.Domain.Comum.Token
{
    public interface IAccessTokenValidator
    {
        ClaimsPrincipal ValidateAndGetUserPrincipal(string token);
    }
}