using Microsoft.AspNetCore.Mvc;
using SisEUs.API.Controllers;
using SisEUs.API.Attributes;
using SisEUs.Apresentation.Checkin.Abstractions;
using System.Threading.Tasks;
using SisEUs.Apresentation.Checkin.DTOs.Solicitacoes;
using SisEUs.Apresentation.Checkin.DTOs.Resposta;
using System.Security.Claims;
using System.Security.Principal;
using SisEUs.Application.Comum.Resultados;
using System.Collections.Generic;

namespace SisEUs.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckinController : BaseController
    {
        private readonly IPinService _pinService;

        public CheckinController(IPinService pinService)
        {
            _pinService = pinService;
        }

        [HttpGet("pin-ativo")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(PinResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPinAtivo()
        {
            var resultado = await _pinService.ObterPinAtivoAsync();
            if (resultado.Sucesso)
            {
                return Ok(resultado.Valor);
            }

            return HandleResult(resultado);
        }

        [HttpPost("gerar-novo-pin")]
        [AuthenticatedUser]
        public async Task<IActionResult> GerarNovoPin()
        {
            var resultado = await _pinService.GerarNovoPinAsync();
            return HandleResult(resultado);
        }

        [HttpPost("validar-pin")]
        public async Task<IActionResult> ValidarPin([FromBody] ValidarPinSolicitacao request)
        {
            var resultado = await _pinService.ValidarApenasPinAsync(request.Pin);
            return HandleResult(resultado);
        }

        [HttpGet("validar-geolocalizacao")]
        [AuthenticatedUser]
        public async Task<IActionResult> ValidarGeolocalizacao([FromQuery] string pin, [FromQuery] string latitude, [FromQuery] string longitude)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                           ?? User.FindFirst("nameid") 
                           ?? User.FindFirst(ClaimTypes.Sid)
                           ?? User.FindFirst("sub")
                           ?? User.FindFirst("id");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int usuarioId))
            {
                return Unauthorized($"Usuário não autenticado. ID não encontrado no token.");
            }
            
            var resultado = await _pinService.ValidarCheckinCompletoAsync(
                pin, 
                latitude, 
                longitude, 
                usuarioId
            );

            return HandleResult(resultado);
        }
        [HttpGet("registrar-checkout")]
        [AuthenticatedUser]
        public async Task<IActionResult> EfetuarCheckOut([FromQuery] string latitude, [FromQuery] string longitude)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                           ?? User.FindFirst("nameid") 
                           ?? User.FindFirst(ClaimTypes.Sid)
                           ?? User.FindFirst("sub")
                           ?? User.FindFirst("id");

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int usuarioId))
            {
                return Unauthorized($"Usuário não autenticado. ID não encontrado no token.");
            }
            var resultado = await _pinService.RegistrarCheckOutAsync(
                latitude, 
                longitude, 
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
        public async Task<IActionResult> ObterRelatorioCheckin()
        {
            var resultado = await _pinService.ObterDadosRelatorioCheckinAsync();
            return HandleResult(resultado);
        }
    }
}