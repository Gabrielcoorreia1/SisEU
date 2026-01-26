using Microsoft.AspNetCore.Mvc;
using SisEUs.API.Attributes;
using SisEUs.Application.Avaliacoes.Abstracoes;
using SisEUs.Application.Avaliacoes.DTOs.Respostas;
using SisEUs.Application.Avaliacoes.DTOs.Solicitacoes;
using SisEUs.Domain.ContextoDeUsuario.Enumeracoes;

namespace SisEUs.API.Controllers
{
    [AuthenticatedUser]
    public class AvaliacoesController(IAvaliacaoServico servico) : BaseController
    {
        [HttpPost("iniciar")]
        [ProducesResponseType(typeof(AvaliacaoResposta), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> IniciarAvaliacao(
            [FromBody] IniciarAvaliacaoSolicitacao request,
            CancellationToken cancellationToken)
        {
            var resultado = await servico.IniciarAvaliacaoAsync(request.ApresentacaoId, cancellationToken);

            if (resultado.Sucesso)
            {
                return CreatedAtAction(
                    nameof(ObterPorId),
                    new { id = resultado.Valor.Id },
                    resultado.Valor);
            }

            return HandleResult(resultado);
        }

        [HttpPost("{id:int}/enviar")]
        [ProducesResponseType(typeof(AvaliacaoResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> EnviarAvaliacao(
            int id,
            [FromBody] EnviarAvaliacaoSolicitacao request,
            CancellationToken cancellationToken)
        {
            var resultado = await servico.EnviarAvaliacaoAsync(id, request.Nota, request.Parecer, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AvaliacaoResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ObterPorId(int id, CancellationToken cancellationToken)
        {
            var resultado = await servico.ObterPorIdAsync(id, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet("apresentacao/{apresentacaoId:int}")]
        [ProducesResponseType(typeof(IEnumerable<AvaliacaoResposta>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ListarPorApresentacao(int apresentacaoId, CancellationToken cancellationToken)
        {
            var resultado = await servico.ListarPorApresentacaoAsync(apresentacaoId, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet("minhas")]
        [ProducesResponseType(typeof(IEnumerable<AvaliacaoResposta>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ListarMinhasAvaliacoes(CancellationToken cancellationToken)
        {
            var resultado = await servico.ListarMinhasAvaliacoesAsync(cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet("evento/{eventoId:int}")]
        [ProducesResponseType(typeof(IEnumerable<AvaliacaoResposta>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ListarPorEvento(int eventoId, CancellationToken cancellationToken)
        {
            var resultado = await servico.ListarPorEventoAsync(eventoId, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet("relatorio/apresentacao/{apresentacaoId:int}")]
        [ProducesResponseType(typeof(RelatorioApresentacaoResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ObterRelatorioApresentacao(int apresentacaoId, CancellationToken cancellationToken)
        {
            var resultado = await servico.ObterRelatorioApresentacaoAsync(apresentacaoId, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet("relatorio/evento/{eventoId:int}")]
        [ProducesResponseType(typeof(RelatorioEventoResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ObterRelatorioEvento(int eventoId, CancellationToken cancellationToken)
        {
            var resultado = await servico.ObterRelatorioEventoAsync(eventoId, cancellationToken);
            return HandleResult(resultado);
        }
    }
}
