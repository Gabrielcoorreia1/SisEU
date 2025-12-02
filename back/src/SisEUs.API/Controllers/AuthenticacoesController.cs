using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SisEUs.API.Attributes;
using SisEUs.Apresentation.Authenticacoes.Abstractions;
using SisEUs.Apresentation.Authenticacoes.DTOs.Resposta;
using SisEUs.Application.Eventos.DTOs.Resposta;
using SisEUs.Application.Presencas.DTOs.Respostas;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Http;
using SisEUs.Domain.ContextoDeUsuario.Entidades;
using SisEUs.Apresentation.Authenticacoes.DTOs.Solicitacoes;

namespace SisEUs.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticacoesController : BaseController
    {
        private readonly IAuthService _authService;
        public AuthenticacoesController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LogarAsync([FromBody] LogarSolicitacao request, CancellationToken cancellationToken)
        {
            var resultado = await _authService.LogarAsync(request, cancellationToken);
            return HandleResult(resultado);
        }
        [HttpPost("register")]
        [ProducesResponseType(typeof(LoginResposta), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RegistrarAsync([FromBody] RegistrarSolicitacao request, CancellationToken cancellationToken)
        {
            var resultado = await _authService.RegistrarAsync(request, cancellationToken);
            return HandleResult(resultado);
        }
        [HttpGet("buscar")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(BuscarUsuariosResposta), StatusCodes.Status200OK)]
        public async Task<IActionResult> BuscarPorNome([FromQuery] string nome, CancellationToken cancellationToken)
        {
            var resultado = await _authService.BuscarPorNomeProfessorAsync(nome, cancellationToken);
            return HandleResult(resultado);
        }
        [HttpGet("{id:int}")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(UsuarioResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BuscarUsuarioLogado(int id, CancellationToken cancellationToken)
        {
            var resultado = await _authService.BuscarPorIdAsync(id, cancellationToken);
            return HandleResult(resultado);
        }
        [HttpPost("tornar-professor/{id:int}")]
        [AuthenticatedUser]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> TornarProfessor(int id, CancellationToken cancellationToken)
        {
            var resultado = await _authService.TornarProfessorAsync(id, cancellationToken);
            return HandleResult(resultado);
        }
    }
}