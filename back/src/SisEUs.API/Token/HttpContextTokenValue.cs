using SisEUs.Domain.Comum.Token;

namespace SisEUs.API.Token
{
    public class HttpContextTokenValue(IHttpContextAccessor contextAccessor) : ITokenProvider
    {
        public string Value()
        {
            var authorization = contextAccessor.HttpContext!.Request.Headers.Authorization.ToString();

            return authorization["Bearer ".Length..].Trim();
        }
    }
}
