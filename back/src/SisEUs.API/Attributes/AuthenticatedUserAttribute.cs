using Microsoft.AspNetCore.Mvc;
using SisEUs.API.Filter;

namespace SisEUs.API.Attributes
{
    public class AuthenticatedUserAttribute : TypeFilterAttribute
    {
        public AuthenticatedUserAttribute() : base(typeof(AuthenticatedUserFilter)) { }
    }
}
