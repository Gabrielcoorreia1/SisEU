using Microsoft.AspNetCore.Mvc;
using SisEUs.Application.Comum.Resultados;

namespace SisEUs.API.Controllers
{
    /// <summary>
    /// Controlador base para todos os controladores da API.
    /// Fornece métodos padronizados para converter objetos de resultado da camada de aplicação
    /// em respostas HTTP (IActionResult), garantindo consistência no tratamento de sucessos e erros.
    /// </summary>
    /// <remarks>
    /// Para utilizar, basta herdar desta classe em seu controlador e invocar os métodos HandleResult,
    /// passando o objeto de resultado retornado pelo seu serviço.
    /// </remarks>
    [ApiController]
    [Route("api/[controller]")]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// Manipula um <see cref="Resultado{T}"/> de operações que retornam um valor em caso de sucesso.
        /// </summary>
        /// <typeparam name="T">O tipo do valor retornado no resultado.</typeparam>
        /// <param name="resultado">O resultado da operação contendo o valor ou os erros.</param>
        /// <returns>
        /// Retorna <see cref="OkObjectResult"/> (200) com o valor em caso de sucesso.
        /// Para falhas, retorna <see cref="NotFoundObjectResult"/> (404), <see cref="BadRequestObjectResult"/> (400),
        /// <see cref="ForbidResult"/> (403), <see cref="ConflictObjectResult"/> (409) ou um erro 500.
        /// </returns>
        protected IActionResult HandleResult<T>(Resultado<T> resultado)
        {
            if (resultado.Sucesso)
                return Ok(resultado.Valor);

            /// <remarks>Caso for adicionar um novo tipo de erro, basta adicionar no enum TipoDeErro e especificar qual tipo de erro é.</remarks>
            return resultado.TipoDeErro switch
            {
                TipoDeErro.NaoEncontrado => NotFound(new { resultado.Erros }),
                TipoDeErro.Validacao => BadRequest(new { resultado.Erros }),
                TipoDeErro.AcessoNegado => Forbid(),
                TipoDeErro.Conflito => Conflict(new { resultado.Erros }),
                _ => StatusCode(StatusCodes.Status500InternalServerError, new { resultado.Erros })
            };
        }

        /// <summary>
        /// Manipula um <see cref="Resultado"/> de operações que não retornam valor em caso de sucesso (ex: exclusão).
        /// </summary>
        /// <param name="resultado">O resultado da operação contendo informações de sucesso ou erro.</param>
        /// <returns>
        /// Retorna <see cref="NoContentResult"/> (204) em caso de sucesso.
        /// Para falhas, retorna <see cref="NotFoundObjectResult"/> (404), <see cref="ConflictObjectResult"/> (409) ou <see cref="BadRequestObjectResult"/> (400).
        /// </returns>
        protected IActionResult HandleResult(Resultado resultado)
        {
            if (resultado.Sucesso) return NoContent();

            /// <remarks>Caso for adicionar um novo tipo de erro, basta adicionar no enum TipoDeErro e especificar qual tipo de erro.</remarks>
            return resultado.TipoDeErro switch
            {
                TipoDeErro.NaoEncontrado => NotFound(new { resultado.Erros }),
                TipoDeErro.Conflito => Conflict(new { resultado.Erros }),
                TipoDeErro.Validacao => BadRequest(new { resultado.Erros }),
                _ => StatusCode(StatusCodes.Status500InternalServerError, new { resultado.Erros })
            };
        }
    }
}