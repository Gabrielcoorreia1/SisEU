using Microsoft.AspNetCore.Mvc;
using SisEUs.Application.Comum.Resultados;
using System.Security.Claims;

namespace SisEUs.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public abstract class BaseController : ControllerBase
    {
        protected IActionResult HandleResult<T>(Resultado<T> resultado)
        {
            if (resultado.Sucesso)
                return Ok(resultado.Valor);

            return resultado.TipoDeErro switch
            {
                TipoDeErro.NaoEncontrado => NotFound(new { resultado.Erros }),
                TipoDeErro.Validacao => BadRequest(new { resultado.Erros }),
                TipoDeErro.AcessoNegado => Forbid(),
                TipoDeErro.Conflito => Conflict(new { resultado.Erros }),
                _ => StatusCode(StatusCodes.Status500InternalServerError, new { resultado.Erros })
            };
        }

        protected IActionResult HandleResult(Resultado resultado)
        {
            if (resultado.Sucesso) return NoContent();

            return resultado.TipoDeErro switch
            {
                TipoDeErro.NaoEncontrado => NotFound(new { resultado.Erros }),
                TipoDeErro.Conflito => Conflict(new { resultado.Erros }),
                TipoDeErro.Validacao => BadRequest(new { resultado.Erros }),
                _ => StatusCode(StatusCodes.Status500InternalServerError, new { resultado.Erros })
            };
        }

        protected bool TryGetUsuarioIdFromToken(out int usuarioId)
        {
            usuarioId = 0;

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                           ?? User.FindFirst("nameid")
                           ?? User.FindFirst(ClaimTypes.Sid)
                           ?? User.FindFirst("sub")
                           ?? User.FindFirst("id");

            if (userIdClaim == null)
            {
                return false;
            }

            return int.TryParse(userIdClaim.Value, out usuarioId);
        }
    }
}