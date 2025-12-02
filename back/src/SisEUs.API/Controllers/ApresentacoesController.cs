using Microsoft.AspNetCore.Mvc;
using SisEUs.API.Attributes;
using SisEUs.Application.Apresentacoes.Abstractions;
using SisEUs.Application.Apresentacoes.DTOs.Respostas;
using SisEUs.Application.Apresentacoes.DTOs.Solicitacoes;

namespace SisEUs.API.Controllers
{
    public class ApresentacoesController : BaseController
    {
        public readonly IApresentacaoServico _servico;
        public ApresentacoesController(IApresentacaoServico servico)
        {
            _servico = servico;
        }
        [HttpPost("{eventoId:int}/apresentacoes")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(ApresentacaoResposta), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> AdicionarApresentacao(int eventoId, [FromBody] CriarApresentacaoSolicitacao request, CancellationToken cancellationToken)
        {
            var resultado = await _servico.CriarApresentacaoAsync(request, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet("{eventoId:int}/apresentacoes")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(IEnumerable<ApresentacaoResposta>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListarApresentacoes(int eventoId, CancellationToken cancellationToken)
        {
            var resultado = await _servico.ObterApresentacoesPorEventoAsync(eventoId, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet("/apresentacoes/{id:int}")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(ApresentacaoResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterApresentacaoPorId(int id, CancellationToken cancellationToken)
        {
            var resultado = await _servico.ObterApresentacaoPorIdAsync(id, cancellationToken);
            return HandleResult(resultado);
        }
        [HttpPut("/apresentacoes/{id:int}")]
        [AuthenticatedUser]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AtualizarApresentacao(int id, [FromBody] AtualizarApresentacaoSolicitacao request, CancellationToken cancellationToken)
        {
            var resultado = await _servico.AtualizarApresentacaoAsync(id, request, cancellationToken);
            return HandleResult(resultado);
        }
        [HttpDelete("/apresentacoes/{id:int}")]
        [AuthenticatedUser]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExcluirApresentacao(int id, CancellationToken cancellationToken)
        {
            var resultado = await _servico.ExcluirApresentacaoAsync(id, cancellationToken);
            return HandleResult(resultado);
        }
    }
}
