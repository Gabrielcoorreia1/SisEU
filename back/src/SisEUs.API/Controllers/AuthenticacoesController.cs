using Microsoft.AspNetCore.Mvc;
using SisEUs.API.Attributes;
using SisEUs.Application.Authenticacoes.Abstractions;
using SisEUs.Application.Authenticacoes.DTOs.Resposta;
using SisEUs.Application.Authenticacoes.DTOs.Solicitacoes;
using SisEUs.Application.Comum.DTOs;
using SisEUs.Application.Eventos.DTOs.Resposta;
using SisEUs.Domain.ContextoDeUsuario.Enumeracoes;

namespace SisEUs.API.Controllers
{
    /// <summary>
    /// Autenticação e gerenciamento de usuários
    /// </summary>
    [Tags("Autenticação")]
    public class AuthenticacoesController(
        IAuthService authService) : BaseController
    {
        /// <summary>
        /// Realiza o login do usuário no sistema
        /// </summary>
        /// <param name="request">Credenciais de login (email/CPF e senha)</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Token JWT e dados do usuário</returns>
        [HttpPost("login")]
        [ProducesResponseType(typeof(LoginResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LogarAsync([FromBody] LogarSolicitacao request, CancellationToken cancellationToken)
        {
            var resultado = await authService.LogarAsync(request, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Registra um novo usuário no sistema
        /// </summary>
        /// <param name="request">Dados do novo usuário</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Usuário criado</returns>
        [HttpPost("registrar")]
        [ProducesResponseType(typeof(UsuarioResposta), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegistrarAsync([FromBody] RegistrarSolicitacao request, CancellationToken cancellationToken)
        {
            var resultado = await authService.RegistrarAsync(request, cancellationToken);

            if (resultado.Sucesso)
            {
                return CreatedAtAction(
                    nameof(BuscarUsuarioLogado),
                    new { id = resultado.Valor.Id },
                    resultado.Valor);
            }

            return HandleResult(resultado);
        }

        /// <summary>
        /// Busca professores por nome
        /// </summary>
        /// <param name="nome">Nome ou parte do nome do professor</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Lista de professores encontrados</returns>
        [HttpGet("buscar")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(BuscarUsuariosResposta), StatusCodes.Status200OK)]
        public async Task<IActionResult> BuscarPorNome([FromQuery] string nome, CancellationToken cancellationToken)
        {
            var resultado = await authService.BuscarPorNomeProfessorAsync(nome, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Obtém os dados de um usuário específico
        /// </summary>
        /// <param name="id">ID do usuário</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Dados do usuário</returns>
        [HttpGet("{id:int}")]
        [AuthenticatedUser]
        [ProducesResponseType(typeof(UsuarioResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BuscarUsuarioLogado(int id, CancellationToken cancellationToken)
        {
            var resultado = await authService.BuscarPorIdAsync(id, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Promove um usuário para o tipo Professor
        /// </summary>
        /// <param name="cpf">CPF do usuário</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Usuário atualizado</returns>
        [HttpPut("{cpf:int}/tornar-professor")]
        [ProducesResponseType(typeof(UsuarioResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> TornarProfessorAsync(int cpf, CancellationToken cancellationToken)
        {
            var resultado = await authService.TornarProfessorAsync(cpf, cancellationToken);
            return HandleResult(resultado);
        }

        /// <summary>
        /// Promove um usuário para o tipo Avaliador
        /// </summary>
        /// <param name="cpf">CPF do usuário</param>
        /// <param name="cancellationToken">Token de cancelamento</param>
        /// <returns>Usuário atualizado</returns>
        [HttpPut("{cpf:int}/tornar-avaliador")]
        [ProducesResponseType(typeof(UsuarioResposta), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> TornarAvaliadorAsync(int cpf, CancellationToken cancellationToken)
        {
            var resultado = await authService.TornarAvaliadorAsync(cpf, cancellationToken);
            return HandleResult(resultado);
        }
    }
}