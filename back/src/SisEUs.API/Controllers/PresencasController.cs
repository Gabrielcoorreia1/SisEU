using Microsoft.AspNetCore.Mvc;
using SisEUs.API.Attributes;
using SisEUs.Application.Presencas.Abstracoes;
using SisEUs.Application.Presencas.DTOs.Respostas;
using SisEUs.Application.Presencas.DTOs.Solicitacoes;
using SisEUs.Domain.ContextoDeUsuario.Enumeracoes;

namespace SisEUs.API.Controllers
{
    [AuthenticatedUser]
    public class PresencasController(IPresencaServico servico) : BaseController
    {
        [HttpPost("check-in")]
        [ProducesResponseType(typeof(PresencaResposta), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> EfetuarCheckIn(
           [FromBody] EfetuarCheckInEventoSolicitacao request,
           CancellationToken cancellationToken)
        {
            if (!TryGetUsuarioIdFromToken(out int usuarioId))
            {
                return Unauthorized(new { erro = "Usuário não autenticado." });
            }

            var solicitacao = new EfetuarCheckInSolicitacao
            {
                UsuarioId = usuarioId,
                EventoId = request.EventoId,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };

            var resultado = await servico.EfetuarCheckInAsync(solicitacao, cancellationToken);

            if (resultado.Sucesso)
            {
                return CreatedAtAction(
                    nameof(ObterPorId),
                    new { id = resultado.Valor.Id },
                    resultado.Valor);
            }

            return HandleResult(resultado);
        }

        [HttpPost("check-out")]
        [ProducesResponseType(typeof(PresencaResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EfetuarCheckOut(
           [FromBody] EfetuarCheckOutEventoSolicitacao request,
           CancellationToken cancellationToken)
        {
            if (!TryGetUsuarioIdFromToken(out int usuarioId))
            {
                return Unauthorized(new { erro = "Usuário não autenticado." });
            }

            var solicitacao = new EfetuarCheckOutSolicitacao
            {
                UsuarioId = usuarioId,
                EventoId = request.EventoId,
                Latitude = request.Latitude,
                Longitude = request.Longitude
            };

            var resultado = await servico.EfetuarCheckOutAsync(solicitacao, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PresencaResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId(int id, CancellationToken cancellationToken)
        {
            var resultado = await servico.ObterPorIdAsync(id, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet("evento/{eventoId:int}")]
        [AuthorizeRoles(ETipoUsuario.Admin, ETipoUsuario.Professor, ETipoUsuario.Avaliador)]
        [ProducesResponseType(typeof(IEnumerable<PresencaResposta>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ListarPorEvento(int eventoId, CancellationToken cancellationToken)
        {
            var resultado = await servico.ListarPorEventoAsync(eventoId, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet("relatorio")]
        [AuthorizeRoles(ETipoUsuario.Admin, ETipoUsuario.Professor)]
        [ProducesResponseType(typeof(IEnumerable<PresencaResposta>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ObterRelatorio(CancellationToken cancellationToken)
        {
            var resultado = await servico.ObterRelatorioAsync(cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet("status/evento/{eventoId:int}")]
        [ProducesResponseType(typeof(StatusPresencaResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterStatusEvento(int eventoId, CancellationToken cancellationToken)
        {
            var resultado = await servico.ObterStatusPresencaEventoAsync(eventoId, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet("evento-em-andamento")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPresencaEventoEmAndamento(CancellationToken cancellationToken)
        {
            var resultado = await servico.ObterPresencaEventoEmAndamentoAsync(cancellationToken);
            return HandleResult(resultado);
        }
    }
}