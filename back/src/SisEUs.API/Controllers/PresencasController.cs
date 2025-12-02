using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisEUs.API.Attributes;
using SisEUs.Application.Presencas.Abstracoes;
using SisEUs.Application.Presencas.DTOs.Respostas;
using SisEUs.Application.Presencas.DTOs.Solicitacoes;
using System.Security.Claims;

namespace SisEUs.API.Controllers
{
    /// <summary>
    /// Gerencia o registro de presenças (check-in e check-out) e a consulta de participantes em eventos.
    /// </summary> 
    [ApiController]
    [Route("api/[controller]")]
    public class PresencasController : BaseController
    {
        private readonly IPresencaServico _servico;

        /// <summary>
        /// Inicializa uma nova instância do controlador de Presenças.
        /// </summary>
        /// <param name="servico">O serviço de presença injetado via DI.</param>
        public PresencasController(IPresencaServico servico)
        {
            _servico = servico;
        }

        /// <summary>
        /// Realiza o check-in de um participante em um evento específico. E criado uma presença com a hora de entrada.
        /// </summary>
        /// <param name="request">Dados da solicitação contendo os identificadores do participante e do evento.</param>
        /// <param name="cancellationToken">Token para cancelamento da operação assíncrona.</param>
        /// <returns>Retorna a presença criada.</returns>
        [HttpPost("check-in")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(PresencaResposta), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> EfetuarCheckIn(
           [FromBody] EfetuarCheckInSolicitacao request,
           CancellationToken cancellationToken)
        {
            //var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            var resultado = await _servico.EfetuarCheckInAsync(request, cancellationToken);

            if (resultado.Sucesso)
            {
                return CreatedAtAction(
                    nameof(ObterPorId),
                    new { id = resultado.Valor.Id },
                    resultado.Valor);
            }

            return HandleResult(resultado);
        }

        /// <summary>
        /// Realiza o check-out de um participante de um evento, registrando a hora de saída.
        /// </summary>
        /// <param name="request">Dados da solicitação contendo os identificadores do participante e do evento.</param>
        /// <param name="cancellationToken">Token para cancelamento da operação assíncrona.</param>
        /// <returns>Retorna os dados do registro de presença atualizado com a hora de saída.</returns>
        [HttpPost("check-out")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(PresencaResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EfetuarCheckOut(
           [FromBody] EfetuarCheckOutSolicitacao request,
           CancellationToken cancellationToken)
        {
            var resultado = await _servico.EfetuarCheckOutAsync(request, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Obtém um registro de presença específico pelo seu identificador único.
        /// </summary>
        /// <param name="id">O ID do registro de presença.</param>
        /// <param name="cancellationToken">Token para cancelamento da operação assíncrona.</param>
        /// <returns>Os dados do registro de presença solicitado.</returns>
        [HttpGet("{id:int}")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(PresencaResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(int id, CancellationToken cancellationToken)
        {
            var resultado = await _servico.ObterPorIdAsync(id, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Lista todos os registros de presença associados a um evento específico.
        /// </summary>
        /// <param name="eventoId">O ID do evento para o qual as presenças serão listadas.</param>
        /// <param name="cancellationToken">Token para cancelamento da operação assíncrona.</param>
        /// <returns>Uma lista com os registros de presença do evento.</returns>
        [HttpGet("/api/eventos/{eventoId:int}/presencas")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(IEnumerable<PresencaResposta>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListarPorEvento(
           int eventoId,
           CancellationToken cancellationToken = default)
        {
            var resultado = await _servico.ListarPorEventoAsync(eventoId, cancellationToken);
            return HandleResult(resultado);
        }
        [HttpGet("/api/eventos/presencas/relatorio")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(IEnumerable<PresencaResposta>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterRelatorio(CancellationToken cancellationToken)
        {
            var resultado = await _servico.ObterRelatorioAsync(cancellationToken);
            return HandleResult(resultado);
        }
        [HttpGet("/status/evento{id:int}")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(StatusPresencaResposta), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterStatusEvento(int id, CancellationToken cancellationToken)
        {

            var resultado = await _servico.ObterStatusPresencaEventoAsync(id, cancellationToken);
            return HandleResult(resultado);
        }
        [HttpGet("/presenca/eventoEmAndamento")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(Boolean), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPresencaEventoEmAndamento(CancellationToken cancellationToken)
        {
            var resultado = await _servico.ObterPresencaEventoEmAndamentoAsync(cancellationToken);
            return HandleResult(resultado);
        }
    }
}