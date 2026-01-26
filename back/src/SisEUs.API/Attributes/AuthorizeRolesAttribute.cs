using Microsoft.AspNetCore.Mvc;
using SisEUs.API.Filter;
using SisEUs.Domain.ContextoDeUsuario.Enumeracoes;

namespace SisEUs.API.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class AuthorizeRolesAttribute : TypeFilterAttribute
    {
        public AuthorizeRolesAttribute(params ETipoUsuario[] roles) : base(typeof(AuthorizeRolesFilter))
        {
            Arguments = [roles];
        }
    }
}
