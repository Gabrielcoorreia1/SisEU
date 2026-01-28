using Microsoft.AspNetCore.Mvc;
using SisEUs.API.Attributes;
using SisEUs.Application.Avaliacoes.Abstracoes;
using SisEUs.Application.Avaliacoes.DTOs.Respostas;
using SisEUs.Application.Avaliacoes.DTOs.Solicitacoes;
using SisEUs.Domain.ContextoDeUsuario.Enumeracoes;

namespace SisEUs.API.Controllers
{
    /// <summary>
    /// Gerenciamento de avaliações de apresentações
    /// </summary>
    [AuthenticatedUser]
    [Tags("Avaliações")]
    public class AvaliacoesController(IAvaliacaoServico servico) : BaseController
    {
        /// <summary>
        /// Inicia uma nova avaliação para uma apresentação
        /// </summary>
        /// <param name="request">Dados da avaliação a ser iniciada</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Avaliação criada</returns>
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

        /// <summary>
        /// Envia uma avaliação já iniciada com nota e parecer
        /// </summary>
        /// <param name="id">ID da avaliação</param>
        /// <param name="request">Nota e parecer da avaliação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Avaliação atualizada</returns>
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

        /// <summary>
        /// Obtém uma avaliação específica pelo ID
        /// </summary>
        /// <param name="id">ID da avaliação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Dados da avaliação</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AvaliacaoResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ObterPorId(int id, CancellationToken cancellationToken)
        {
            var resultado = await servico.ObterPorIdAsync(id, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Lista todas as avaliações de uma apresentação
        /// </summary>
        /// <param name="apresentacaoId">ID da apresentação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de avaliações</returns>
        [HttpGet("apresentacao/{apresentacaoId:int}")]
        [ProducesResponseType(typeof(IEnumerable<AvaliacaoResposta>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ListarPorApresentacao(int apresentacaoId, CancellationToken cancellationToken)
        {
            var resultado = await servico.ListarPorApresentacaoAsync(apresentacaoId, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Lista todas as avaliações do usuário logado
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de avaliações do usuário</returns>
        [HttpGet("minhas")]
        [ProducesResponseType(typeof(IEnumerable<AvaliacaoResposta>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ListarMinhasAvaliacoes(CancellationToken cancellationToken)
        {
            var resultado = await servico.ListarMinhasAvaliacoesAsync(cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Lista todas as avaliações de um evento
        /// </summary>
        /// <param name="eventoId">ID do evento</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de avaliações do evento</returns>
        [HttpGet("evento/{eventoId:int}")]
        [ProducesResponseType(typeof(IEnumerable<AvaliacaoResposta>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ListarPorEvento(int eventoId, CancellationToken cancellationToken)
        {
            var resultado = await servico.ListarPorEventoAsync(eventoId, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Obtém o relatório consolidado de uma apresentação com todas as avaliações
        /// </summary>
        /// <param name="apresentacaoId">ID da apresentação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Relatório com média e todas as avaliações</returns>
        [HttpGet("relatorio/apresentacao/{apresentacaoId:int}")]
        [ProducesResponseType(typeof(RelatorioApresentacaoResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ObterRelatorioApresentacao(int apresentacaoId, CancellationToken cancellationToken)
        {
            var resultado = await servico.ObterRelatorioApresentacaoAsync(apresentacaoId, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Obtém o relatório consolidado de um evento com todas as apresentações e avaliações
        /// </summary>
        /// <param name="eventoId">ID do evento</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Relatório completo do evento</returns>
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
