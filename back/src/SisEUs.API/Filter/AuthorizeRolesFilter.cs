using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SisEUs.Domain.ContextoDeUsuario.Enumeracoes;
using System.Security.Claims;

namespace SisEUs.API.Filter
{
    public class AuthorizeRolesFilter : IAuthorizationFilter
    {
        private readonly ETipoUsuario[] _allowedRoles;

        public AuthorizeRolesFilter(ETipoUsuario[] allowedRoles)
        {
            _allowedRoles = allowedRoles ?? throw new ArgumentNullException(nameof(allowedRoles));
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedObjectResult(new 
                { 
                    erro = "Usuário não autenticado." 
                });
                return;
            }

            var roleClaim = context.HttpContext.User.FindFirst(ClaimTypes.Role)
                         ?? context.HttpContext.User.FindFirst("role");

            if (roleClaim == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            if (!Enum.TryParse<ETipoUsuario>(roleClaim.Value, ignoreCase: true, out var userRole))
            {
                context.Result = new ForbidResult();
                return;
            }

            if (!_allowedRoles.Contains(userRole))
            {
                context.Result = new ObjectResult(new 
                { 
                    erro = "Acesso negado. Você não tem permissão para acessar este recurso.",
                    roleRequerida = string.Join(", ", _allowedRoles.Select(r => r.ToString())),
                    suaRole = userRole.ToString()
                })
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
                return;
            }
        }
    }
}
