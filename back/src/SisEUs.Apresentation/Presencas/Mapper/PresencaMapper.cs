using SisEUs.Application.Eventos.Mappers;
using SisEUs.Application.Presencas.DTOs.Respostas;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Interfaces;
using SisEUs.Domain.ContextoDeUsuario.Interfaces;

namespace SisEUs.Application.Presencas.Mapper
{
    public static class PresencaMapper
    {
        public static async Task<PresencaResposta> ToResponseDtoAsync(
            this Presenca presenca,
            IUsuarioRepositorio usuarioRepositorio,
            IEventoRepositorio eventoRepositorio,
            CancellationToken cancellationToken)
        {

            var usuario = await usuarioRepositorio.ObterPorIdAsync(presenca.UsuarioId, cancellationToken);
            var evento = await eventoRepositorio.ObterEventoPorIdAsync(presenca.EventoId, cancellationToken);

            var usuarioDto = new UsuarioResposta
            {
                Id = usuario?.Id ?? 0,
                NomeCompleto = usuario?.Nome.ToString() ?? "Usuário não encontrado",
                Cpf = usuario?.Cpf.Valor ?? "Sem cpf",
                Email = usuario?.Email.Valor ?? "Sem email",
            };

            var eventoDto = (evento != null)
                ? await evento.ToResponseDtoAsync(usuarioRepositorio, cancellationToken)
                : null;

            return new PresencaResposta
            {
                Id = presenca.Id,
                Usuario = usuarioDto,
                Evento = eventoDto,
                DataCheckIn = presenca.CheckIn,
                DataCheckOut = presenca.CheckOut,
                Localizacao = new LocalizacaoResposta
                {
                    Latitude = presenca.Localizacao.Latitude,
                    Longitude = presenca.Localizacao.Longitude
                }
            };
        }
    }
}
