using SisEUs.Application.Comum.Resultados;
using SisEUs.Apresentation.Authenticacoes.DTOs.Resposta;
using SisEUs.Apresentation.Authenticacoes.DTOs.Solicitacoes;
using SisEUs.Application.Eventos.DTOs.Resposta;
using SisEUs.Application.Presencas.DTOs.Respostas;
using System.Threading;
using System.Threading.Tasks;

namespace SisEUs.Apresentation.Authenticacoes.Abstractions
{
    public interface IAuthService
    {
        Task<Resultado<LoginResposta>> LogarAsync(LogarSolicitacao request, CancellationToken cancellationToken);
        Task<Resultado<LoginResposta>> RegistrarAsync(RegistrarSolicitacao request, CancellationToken cancellationToken);
        Task<Resultado<BuscarUsuariosResposta>> BuscarPorNomeProfessorAsync(string nome, CancellationToken cancellationToken);
        Task<Resultado<UsuarioResposta>> BuscarPorIdAsync(int id, CancellationToken cancellationToken);
        Task<Resultado> TornarProfessorAsync(int id, CancellationToken cancellationToken);
    }
}
