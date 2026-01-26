using Microsoft.AspNetCore.Mvc;
using SisEUs.API.Attributes;
using SisEUs.API.Controllers;
using SisEUs.Application.Apresentacoes.Abstractions;
using SisEUs.Application.Apresentacoes.DTOs.Respostas;
using SisEUs.Application.Apresentacoes.DTOs.Solicitacoes;
using SisEUs.Domain.ContextoDeUsuario.Enumeracoes;

namespace SisEUs.API.Controllers
{
    [AuthenticatedUser]
    public class ApresentacoesController(
        IApresentacaoServico servico) : BaseController
    {
        [HttpPost("{eventoId:int}")]
        [ProducesResponseType(typeof(ApresentacaoResposta), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AdicionarApresentacao(int eventoId, [FromBody] CriarApresentacaoSolicitacao request, CancellationToken cancellationToken)
        {
            var requestSeguro = request with { EventoId = eventoId };

            var resultado = await servico.CriarApresentacaoAsync(requestSeguro, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet("evento/{eventoId:int}")]
        [ProducesResponseType(typeof(IEnumerable<ApresentacaoResposta>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListarApresentacoes(int eventoId, CancellationToken cancellationToken)
        {
            var resultado = await servico.ObterApresentacoesPorEventoAsync(eventoId, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApresentacaoResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterApresentacaoPorId(int id, CancellationToken cancellationToken)
        {
            var resultado = await servico.ObterApresentacaoPorIdAsync(id, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AtualizarApresentacao(int id, [FromBody] AtualizarApresentacaoSolicitacao request, CancellationToken cancellationToken)
        {
            var resultado = await servico.AtualizarApresentacaoAsync(id, request, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ExcluirApresentacao(int id, CancellationToken cancellationToken)
        {
            var resultado = await servico.ExcluirApresentacaoAsync(id, cancellationToken);
            return HandleResult(resultado);
        }
    }
}