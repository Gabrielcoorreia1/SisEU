using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SisEUs.Domain.Comum.Token;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;

namespace SisEUs.API.Filter
{
    public class AuthenticatedUserFilter : Attribute, IAsyncAuthorizationFilter
    {

        public AuthenticatedUserFilter() { } 

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                var token = TokenOnRequest(context);

                if (token == null)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }
                var validator = context.HttpContext.RequestServices.GetRequiredService<IAccessTokenValidator>();
                
                var userPrincipal = validator.ValidateAndGetUserPrincipal(token);

                context.HttpContext.User = userPrincipal;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERRO AUTH FILTER]: {ex.Message}");
                context.Result = new UnauthorizedResult();
            }
        }

        private static string TokenOnRequest(AuthorizationFilterContext context)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            return authHeader.Substring("Bearer ".Length).Trim();
        }
    }
}