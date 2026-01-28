using SisEUs.Application.Apresentacoes.DTOs.Respostas;
using SisEUs.Application.Comum.DTOs;
using SisEUs.Application.Eventos.DTOs.Resposta;
using SisEUs.Application.Presencas.DTOs.Respostas;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeUsuario.Entidades;

namespace SisEUs.Application.Comum.Mapeamento
{
    public interface IMapeadorDeEntidades
    {
        Task<EventoResposta> MapearEventoAsync(Evento evento, CancellationToken cancellationToken = default);

        Task<IEnumerable<EventoResposta>> MapearEventosAsync(
            IEnumerable<Evento> eventos, 
            Dictionary<int, Usuario> usuariosCache,
            CancellationToken cancellationToken = default);

        Task<PresencaResposta> MapearPresencaAsync(Presenca presenca, CancellationToken cancellationToken = default);

        Task<IEnumerable<PresencaResposta>> MapearPresencasAsync(
            IEnumerable<Presenca> presencas, 
            CancellationToken cancellationToken = default);

        Task<ApresentacaoResposta> MapearApresentacaoAsync(Apresentacao apresentacao, CancellationToken cancellationToken = default);

        UsuarioResposta MapearUsuario(Usuario usuario);
    }
}
