using SisEUs.Application.Authenticacoes.DTOs.Resposta;
using SisEUs.Application.Authenticacoes.DTOs.Solicitacoes;
using SisEUs.Application.Comum.DTOs;
using SisEUs.Application.Comum.Resultados;
using SisEUs.Application.Eventos.DTOs.Resposta;

namespace SisEUs.Application.Authenticacoes.Abstractions
{
    public interface IAuthService
    {
        Task<Resultado<LoginResposta>> LogarAsync(LogarSolicitacao request, CancellationToken cancellationToken);
        Task<Resultado<UsuarioResposta>> RegistrarAsync(RegistrarSolicitacao request, CancellationToken cancellationToken);
        Task<Resultado<BuscarUsuariosResposta>> BuscarPorNomeProfessorAsync(string nome, CancellationToken cancellationToken);
        Task<Resultado<UsuarioResposta>> BuscarPorIdAsync(int id, CancellationToken cancellationToken);
        Task<Resultado<UsuarioResposta>> TornarProfessorAsync(int cpf, CancellationToken cancellationToken);
        Task<Resultado<UsuarioResposta>> TornarAvaliadorAsync(int cpf, CancellationToken cancellationToken);
    }
}
