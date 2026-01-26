using SisEUs.Application.Apresentacoes.DTOs.Respostas;
using SisEUs.Application.Eventos.Mappers;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Interfaces;
using SisEUs.Domain.ContextoDeUsuario.Interfaces;

namespace SisEUs.Application.Apresentacoes.Mappers
{
    public static class ApresentacaoMapper
    {
        public static async Task<ApresentacaoResposta> ToResponseDtoAsync(
            this Apresentacao apresentacao,
            IEventoRepositorio eventoRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            CancellationToken cancellationToken)
        {
            var evento = await eventoRepositorio.ObterEventoPorIdAsync(apresentacao.EventoId, cancellationToken);

            var eventoDto = (evento != null)
                ? await evento.ToResponseDtoAsync(usuarioRepositorio, cancellationToken)
                : null;

            return new ApresentacaoResposta
            (
                apresentacao.Id,
                eventoDto!,
                apresentacao.Titulo.Valor,
                apresentacao.NomeAutor,
                apresentacao.NomeOrientador
            );
        }
    }
}
