using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisEUs.API.Attributes;
using SisEUs.API.Controllers;
using SisEUs.Application.Eventos.Abstracoes;
using SisEUs.Application.Eventos.DTOs.Resposta;
using SisEUs.Application.Eventos.DTOs.Solicitacoes;

namespace SisEUs.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventosController : BaseController
    {
        private readonly IEventoServico _servico;

        public EventosController(IEventoServico servico)
        {
            _servico = servico;
        }

        [HttpPost]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(EventoResposta), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CriarEvento(
            [FromBody] CriarEventoSolicitacao request,
            CancellationToken cancellationToken)
        {
            var resultado = await _servico.CriarEventoAsync(request, cancellationToken);

            if (resultado.Sucesso)
            {
                return CreatedAtAction(
                    nameof(ObterEventoPorId),
                    new { id = resultado.Valor.Id },
                    resultado.Valor);
            }

            return HandleResult(resultado);
        }
        [HttpGet("/api/Eventos/pin-global/{id:int}")] 
        [AuthenticatedUser]
        [ProducesResponseType(typeof(EventoResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterEventoPrincipalPorId(int id, CancellationToken cancellationToken)
        {
            var resultado = await _servico.ObterEventoPorIdAsync(id, cancellationToken);
            return HandleResult(resultado);
        }


        [AuthenticatedUser]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EventoResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterEventoPorId(int id, CancellationToken cancellationToken)
        {
            var resultado = await _servico.ObterEventoPorIdAsync(id, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet("ObterEventos")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(IEnumerable<EventoResposta>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterEventos(
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 10,
            CancellationToken cancellationToken = default)
        {
            var resultado = await _servico.ListarEventosAsync(pagina, tamanho, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpPut("{id:int}")]
        [AuthenticatedUser]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AtualizarEvento(int id, [FromBody] AtualizarEventoSolicitacao request, CancellationToken cancellationToken)
        {
            var resultado = await _servico.AtualizarEventoAsync(id, request, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpDelete("{id:int}")]
        [AuthenticatedUser]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExcluirEvento(int id, CancellationToken cancellationToken)
        {
            var resultado = await _servico.ExcluirEventoAsync(id, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpPost("{eventoId:int}/avaliadores")]
        [AuthenticatedUser]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> AdicionarAvaliador(int eventoId, [FromBody] NomeCompletoResposta request, CancellationToken cancellationToken)
        {
            var resultado = await _servico.AdicionarAvaliadorAsync(request.Nome, request.Sobrenome, eventoId, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpDelete("{eventoId:int}/avaliadores")]
        [AuthenticatedUser]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemoverAvaliador(int eventoId, [FromBody] NomeCompletoResposta request, CancellationToken cancellationToken)
        {
            var resultado = await _servico.RemoverAvaliadorAsync(request.Nome, request.Sobrenome, eventoId, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet("ObterPorCodigo")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(EventoResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterEventoPorCodigo(string codigo, CancellationToken cancellationToken)
        {
            var resultado = await _servico.ObterPorCodigoEvento(codigo, cancellationToken);
            return HandleResult(resultado);
        }
    }
}