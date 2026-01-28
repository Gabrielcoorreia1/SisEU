using Microsoft.AspNetCore.Mvc;
using SisEUs.API.Attributes;
using SisEUs.Application.Checkin.Abstraction;
using SisEUs.Application.Checkin.DTOs.Resposta;
using SisEUs.Application.Checkin.DTOs.Solicitacoes;
using SisEUs.Domain.ContextoDeUsuario.Enumeracoes;

namespace SisEUs.API.Controllers
{
    /// <summary>
    /// Gerenciamento de check-in e check-out com PIN
    /// </summary>
    [Tags("Check-in")]
    public class CheckinController(IPinService pinService) : BaseController
    {
        /// <summary>
        /// Obtém o PIN ativo atual para check-in
        /// </summary>
        /// <returns>PIN ativo</returns>
        [HttpGet("pin-ativo")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(PinResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPinAtivo()
        {
            var resultado = await pinService.ObterPinAtivoAsync();
            return HandleResult(resultado);
        }

        /// <summary>
        /// Gera um novo PIN para check-in (apenas administradores)
        /// </summary>
        /// <returns>Novo PIN gerado</returns>
        [HttpPost("pin")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(PinResposta), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GerarNovoPin()
        {
            var resultado = await pinService.GerarNovoPinAsync();

            if (resultado.Sucesso)
            {
                return Created($"/api/checkin/pin-ativo", resultado.Valor);
            }

            return HandleResult(resultado);
        }

        /// <summary>
        /// Valida se um PIN está ativo e correto
        /// </summary>
        /// <param name="request">PIN a ser validado</param>
        /// <returns>Sem conteúdo se válido</returns>
        [HttpPost("validar-pin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidarPin([FromBody] ValidarPinSolicitacao request)
        {
            var resultado = await pinService.ValidarApenasPinAsync(request.Pin);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Registra o check-in do usuário com PIN e localização
        /// </summary>
        /// <param name="request">PIN e coordenadas de localização</param>
        /// <returns>Confirmação de check-in registrado</returns>
        [HttpPost("registrar")]
        [AuthenticatedUser]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegistrarCheckin([FromBody] RegistrarCheckinSolicitacao request)
        {
            var resultado = await pinService.ValidarCheckinCompletoAsync(
                request.Pin,
                request.Latitude,
                request.Longitude
            );

            if (resultado.Sucesso)
            {
                return Created("/api/checkin/status", new { mensagem = "Check-in registrado com sucesso!" });
            }

            return HandleResult(resultado);
        }

        /// <summary>
        /// Registra o check-out do usuário com localização
        /// </summary>
        /// <param name="request">Coordenadas de localização</param>
        /// <returns>Confirmação de check-out registrado</returns>
        [HttpPost("checkout")]
        [AuthenticatedUser]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RegistrarCheckOut([FromBody] RegistrarCheckoutSolicitacao request)
        {

            var resultado = await pinService.RegistrarCheckOutAsync(
                request.Latitude,
                request.Longitude
            );

            if (resultado.Sucesso)
            {
                return Ok(new { mensagem = "Check-out registrado com sucesso!" });
            }

            return HandleResult(resultado);
        }

        /// <summary>
        /// Obtém o relatório completo de todos os check-ins e check-outs
        /// </summary>
        /// <returns>Relatório de check-ins</returns>
        [HttpGet("relatorio")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(IEnumerable<RelatorioCheckinResposta>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ObterRelatorioCheckin()
        {
            var resultado = await pinService.ObterDadosRelatorioCheckinAsync();
            return HandleResult(resultado);
        }
    }
}