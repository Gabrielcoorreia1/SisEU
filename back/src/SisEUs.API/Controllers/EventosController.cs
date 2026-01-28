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
    /// <summary>
    /// Gerenciamento de eventos acadêmicos (sessões de apresentação)
    /// </summary>
    [AuthenticatedUser]
    [Tags("Eventos")]
    public class EventosController(IEventoServico servico, ILoggedUser loggedUser) : BaseController
    {
        /// <summary>
        /// Cria um novo evento acadêmico
        /// </summary>
        /// <param name="request">Dados do evento a ser criado</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Evento criado</returns>
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

        /// <summary>
        /// Obtém um evento específico pelo ID
        /// </summary>
        /// <param name="id">ID do evento</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Dados do evento</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(EventoResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterEventoPorId(int id, CancellationToken cancellationToken)
        {
            var resultado = await servico.ObterEventoPorIdAsync(id, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Lista todos os eventos com paginação
        /// </summary>
        /// <param name="pagina">Número da página (padrão: 1)</param>
        /// <param name="tamanho">Tamanho da página (padrão: 10)</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista paginada de eventos</returns>
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

        /// <summary>
        /// Atualiza os dados de um evento existente
        /// </summary>
        /// <param name="id">ID do evento</param>
        /// <param name="request">Dados atualizados</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Sem conteúdo</returns>
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

        /// <summary>
        /// Remove um evento do sistema
        /// </summary>
        /// <param name="id">ID do evento</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Sem conteúdo</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ExcluirEvento(int id, CancellationToken cancellationToken)
        {
            var resultado = await servico.ExcluirEventoAsync(id, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Adiciona um participante a um evento
        /// </summary>
        /// <param name="eventoId">ID do evento</param>
        /// <param name="participanteId">ID do participante</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Sem conteúdo</returns>
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

        /// <summary>
        /// Remove um participante de um evento
        /// </summary>
        /// <param name="eventoId">ID do evento</param>
        /// <param name="participanteId">ID do participante</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Sem conteúdo</returns>
        [HttpDelete("{eventoId:int}/participantes/{participanteId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RemoverParticipante(int eventoId, int participanteId, CancellationToken cancellationToken)
        {
            var resultado = await servico.RemoverParticipanteAsync(participanteId, eventoId, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Adiciona um avaliador a um evento pelo CPF
        /// </summary>
        /// <param name="eventoId">ID do evento</param>
        /// <param name="cpf">CPF do avaliador</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Sem conteúdo</returns>
        [HttpPost("{eventoId:int}/avaliadores")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AdicionarAvaliador(int eventoId, [FromBody] string cpf, CancellationToken cancellationToken)
        {
            var resultado = await servico.AdicionarAvaliadorPorCpfAsync(cpf, eventoId, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Remove um avaliador de um evento
        /// </summary>
        /// <param name="eventoId">ID do evento</param>
        /// <param name="avaliadorId">ID do avaliador</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Sem conteúdo</returns>
        [HttpDelete("{eventoId:int}/avaliadores/{avaliadorId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RemoverAvaliador(int eventoId, int avaliadorId, CancellationToken cancellationToken)
        {
            var resultado = await servico.RemoverAvaliadorAsync(avaliadorId, eventoId, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Busca um evento pelo código único
        /// </summary>
        /// <param name="codigo">Código único do evento</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Dados do evento</returns>
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
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de eventos para avaliar</returns>
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
        /// <param name="avaliadorId">ID do avaliador</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de eventos do avaliador</returns>
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