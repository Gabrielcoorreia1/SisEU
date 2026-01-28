using Microsoft.AspNetCore.Mvc;
using SisEUs.API.Attributes;
using SisEUs.API.Controllers;
using SisEUs.Application.Eventos.Abstracoes;
using SisEUs.Application.Eventos.DTOs.Resposta;
using SisEUs.Application.Eventos.DTOs.Solicitacoes;
using SisEUs.Domain.Comum.LoggedUser;
using SisEUs.Domain.ContextoDeUsuario.Enumeracoes;

namespace SisEUs.Api.Controllers
{
    [AuthenticatedUser]
    public class EventosController(IEventoServico servico, ILoggedUser loggedUser) : BaseController
    {
        [HttpPost]
        [ProducesResponseType(typeof(EventoResposta), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CriarEvento(
            [FromBody] CriarEventoSolicitacao request,
            CancellationToken cancellationToken)
        {
            var resultado = await servico.CriarEventoAsync(request, cancellationToken);

            if (resultado.Sucesso)
            {
                return CreatedAtAction(
                    nameof(ObterEventoPorId),
                    new { id = resultado.Valor.Id },
                    resultado.Valor);
            }

            return HandleResult(resultado);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EventoResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterEventoPorId(int id, CancellationToken cancellationToken)
        {
            var resultado = await servico.ObterEventoPorIdAsync(id, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EventoResposta>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterEventos(
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanho = 10,
            CancellationToken cancellationToken = default)
        {
            var resultado = await servico.ListarEventosAsync(pagina, tamanho, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AtualizarEvento(int id, [FromBody] AtualizarEventoSolicitacao request, CancellationToken cancellationToken)
        {
            var resultado = await servico.AtualizarEventoAsync(id, request, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ExcluirEvento(int id, CancellationToken cancellationToken)
        {
            var resultado = await servico.ExcluirEventoAsync(id, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpPost("{eventoId:int}/participantes")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AdicionarParticipante(int eventoId, [FromBody] int participanteId, CancellationToken cancellationToken)
        {
            var resultado = await servico.AdicionarParticipanteAsync(participanteId, eventoId, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpDelete("{eventoId:int}/participantes/{participanteId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RemoverParticipante(int eventoId, int participanteId, CancellationToken cancellationToken)
        {
            var resultado = await servico.RemoverParticipanteAsync(participanteId, eventoId, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpPost("{eventoId:int}/avaliadores")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AdicionarAvaliador(int eventoId, [FromBody] string cpf, CancellationToken cancellationToken)
        {
            var resultado = await servico.AdicionarAvaliadorPorCpfAsync(cpf, eventoId, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpDelete("{eventoId:int}/avaliadores/{avaliadorId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RemoverAvaliador(int eventoId, int avaliadorId, CancellationToken cancellationToken)
        {
            var resultado = await servico.RemoverAvaliadorAsync(avaliadorId, eventoId, cancellationToken);
            return HandleResult(resultado);
        }

        [HttpGet("por-codigo")]
        [ProducesResponseType(typeof(EventoResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterEventoPorCodigo([FromQuery] string codigo, CancellationToken cancellationToken)
        {
            var resultado = await servico.ObterPorCodigoEvento(codigo, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Obtém os eventos em que o usuário logado é avaliador
        /// </summary>
        [HttpGet("meus-eventos-avaliar")]
        [ProducesResponseType(typeof(IEnumerable<EventoResposta>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterMeusEventosParaAvaliar(CancellationToken cancellationToken)
        {
            var usuario = await loggedUser.User();
            var resultado = await servico.ObterEventosPorAvaliadorAsync(usuario.Id, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Obtém os eventos em que um avaliador específico deve avaliar
        /// </summary>
        [HttpGet("avaliador/{avaliadorId:int}/eventos")]
        [AuthorizeRoles(ETipoUsuario.Admin, ETipoUsuario.Professor)]
        [ProducesResponseType(typeof(IEnumerable<EventoResposta>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterEventosPorAvaliador(int avaliadorId, CancellationToken cancellationToken)
        {
            var resultado = await servico.ObterEventosPorAvaliadorAsync(avaliadorId, cancellationToken);
            return HandleResult(resultado);
        }
    }
}