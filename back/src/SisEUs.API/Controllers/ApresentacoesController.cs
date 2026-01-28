using Microsoft.AspNetCore.Mvc;
using SisEUs.API.Attributes;
using SisEUs.API.Controllers;
using SisEUs.Application.Apresentacoes.Abstractions;
using SisEUs.Application.Apresentacoes.DTOs.Respostas;
using SisEUs.Application.Apresentacoes.DTOs.Solicitacoes;
using SisEUs.Domain.ContextoDeUsuario.Enumeracoes;

namespace SisEUs.API.Controllers
{
    /// <summary>
    /// Gerenciamento de apresentações de trabalhos em eventos
    /// </summary>
    [AuthenticatedUser]
    [Tags("Apresentações")]
    public class ApresentacoesController(
        IApresentacaoServico servico) : BaseController
    {
        /// <summary>
        /// Adiciona uma nova apresentação a um evento
        /// </summary>
        /// <param name="eventoId">ID do evento</param>
        /// <param name="request">Dados da apresentação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Apresentação criada</returns>
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

        /// <summary>
        /// Lista todas as apresentações de um evento
        /// </summary>
        /// <param name="eventoId">ID do evento</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de apresentações do evento</returns>
        [HttpGet("evento/{eventoId:int}")]
        [ProducesResponseType(typeof(IEnumerable<ApresentacaoResposta>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ListarApresentacoes(int eventoId, CancellationToken cancellationToken)
        {
            var resultado = await servico.ObterApresentacoesPorEventoAsync(eventoId, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Obtém uma apresentação específica pelo ID
        /// </summary>
        /// <param name="id">ID da apresentação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Dados da apresentação</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApresentacaoResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterApresentacaoPorId(int id, CancellationToken cancellationToken)
        {
            var resultado = await servico.ObterApresentacaoPorIdAsync(id, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Atualiza os dados de uma apresentação existente
        /// </summary>
        /// <param name="id">ID da apresentação</param>
        /// <param name="request">Dados atualizados</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Sem conteúdo</returns>
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

        /// <summary>
        /// Remove uma apresentação do sistema
        /// </summary>
        /// <param name="id">ID da apresentação</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Sem conteúdo</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ExcluirApresentacao(int id, CancellationToken cancellationToken)
        {
            var resultado = await servico.ExcluirApresentacaoAsync(id, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Obtém as apresentações em que o usuário logado é autor
        /// </summary>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de apresentações do usuário</returns>
        [HttpGet("minhas-apresentacoes")]
        [ProducesResponseType(typeof(IEnumerable<ApresentacaoResposta>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterMinhasApresentacoes(CancellationToken cancellationToken)
        {
            var resultado = await servico.ObterMinhasApresentacoesAsync(cancellationToken);
            return HandleResult(resultado);
        }
    }
}