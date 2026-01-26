using Microsoft.AspNetCore.Mvc;
using SisEUs.API.Attributes;
using SisEUs.Application.Checkin.Abstraction;
using SisEUs.Application.Checkin.DTOs.Resposta;
using SisEUs.Application.Checkin.DTOs.Solicitacoes;
using SisEUs.Domain.ContextoDeUsuario.Enumeracoes;

namespace SisEUs.API.Controllers
{
    public class CheckinController(IPinService pinService) : BaseController
    {
        [HttpGet("pin-ativo")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(PinResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPinAtivo()
        {
            var resultado = await pinService.ObterPinAtivoAsync();
            return HandleResult(resultado);
        }

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

        [HttpPost("validar-pin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ValidarPin([FromBody] ValidarPinSolicitacao request)
        {
            var resultado = await pinService.ValidarApenasPinAsync(request.Pin);
            return HandleResult(resultado);
        }

        [HttpPost("registrar")]
        [AuthenticatedUser]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegistrarCheckin([FromBody] RegistrarCheckinSolicitacao request)
        {
            if (!TryGetUsuarioIdFromToken(out int usuarioId))
            {
                return Unauthorized(new { erro = "Usuário não autenticado. ID não encontrado no token." });
            }

            var resultado = await pinService.ValidarCheckinCompletoAsync(
                request.Pin,
                request.Latitude,
                request.Longitude,
                usuarioId
            );

            if (resultado.Sucesso)
            {
                return Created("/api/checkin/status", new { mensagem = "Check-in registrado com sucesso!" });
            }

            return HandleResult(resultado);
        }

        [HttpPost("checkout")]
        [AuthenticatedUser]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RegistrarCheckOut([FromBody] RegistrarCheckoutSolicitacao request)
        {
            if (!TryGetUsuarioIdFromToken(out int usuarioId))
            {
                return Unauthorized(new { erro = "Usuário não autenticado. ID não encontrado no token." });
            }

            var resultado = await pinService.RegistrarCheckOutAsync(
                request.Latitude,
                request.Longitude,
                usuarioId
            );

            if (resultado.Sucesso)
            {
                return Ok(new { mensagem = "Check-out registrado com sucesso!" });
            }

            return HandleResult(resultado);
        }

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