using SisEUs.Application.Apresentacoes.DTOs.Respostas;
using SisEUs.Application.Comum.DTOs;
using SisEUs.Application.Eventos.DTOs.Resposta;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeUsuario.Interfaces;

namespace SisEUs.Application.Eventos.Mappers
{
    public static class EventoMapper
    {
        public static async Task<EventoResposta> ToResponseDtoAsync(
            this Evento evento,
            IUsuarioRepositorio usuarioRepositorio,
            CancellationToken cancellationToken)
        {
            var organizadoresDto = new List<ParticipanteResposta>();
            if (evento.ParticipantesIds.Count > 0)
            {
                var idsParticipantes = evento.ParticipantesIds.ToList();
                var usuarios = await usuarioRepositorio.ObterPorIdsAsync(idsParticipantes, cancellationToken);

                organizadoresDto = [.. usuarios.Select(u => new ParticipanteResposta
                (
                    Id: u.Id,
                    NomeCompleto: $"{u.Nome.Nome} {u.Nome.Sobrenome}"
                ))];
            }

            var avaliadoresNomes = new List<string>();
            if (evento.AvaliadoresIds.Count > 0)
            {
                var idsAvaliadores = evento.AvaliadoresIds.ToList();
                var usuariosAvaliadores = await usuarioRepositorio.ObterPorIdsAsync(idsAvaliadores, cancellationToken);

                avaliadoresNomes = [.. usuariosAvaliadores.Select(u => $"{u.Nome.Nome} {u.Nome.Sobrenome}")];
            }

            return new EventoResposta(
                Id: evento.Id,
                Titulo: evento.Titulo.Valor,
                Local: new LocalResposta(
                    evento.Local.Campus,
                    evento.Local.Departamento,
                    evento.Local.Bloco,
                    evento.Local.Sala
                ),
                Localizacao: new LocalizacaoResposta(
                    evento.Localizacao.Latitude,
                    evento.Localizacao.Longitude
                ),
                DataInicio: FormatarDataPorExtenso(evento.DataInicio),
                DataFim: FormatarDataPorExtenso(evento.DataFim),
                Organizadores: organizadoresDto,
                Avaliadores: avaliadoresNomes,
                ETipoEvento: evento.TipoEvento.ToString(),
                EventType: evento.TipoEvento,
                ImgUrl: evento.ImgUrl,
                CodigoUnico: evento.CodigoUnico,
                NomeCampus: evento.Local.Campus,
                DataDateTime: evento.DataInicio,
                PinCheckin: evento.PinCheckin,
                Apresentacoes: []
            );
        }

        public static EventoResposta ToResponseDto(this Evento evento, List<string> avaliadoresNomes)
        {
            var organizadoresDto = evento.ParticipantesIds
                .Select(id => new ParticipanteResposta(Id: id, NomeCompleto: ""))
                .ToList();

            return new EventoResposta(
                Id: evento.Id,
                Titulo: evento.Titulo.Valor,
                Local: new LocalResposta(
                    evento.Local.Campus,
                    evento.Local.Departamento,
                    evento.Local.Bloco,
                    evento.Local.Sala
                ),
                Localizacao: new LocalizacaoResposta(
                    evento.Localizacao.Latitude,
                    evento.Localizacao.Longitude
                ),
                DataInicio: FormatarDataPorExtenso(evento.DataInicio),
                DataFim: FormatarDataPorExtenso(evento.DataFim),
                Organizadores: organizadoresDto,
                Avaliadores: avaliadoresNomes,
                ETipoEvento: evento.TipoEvento.ToString(),
                EventType: evento.TipoEvento,
                ImgUrl: evento.ImgUrl,
                CodigoUnico: evento.CodigoUnico,
                NomeCampus: evento.Local.Campus,
                DataDateTime: evento.DataInicio,
                PinCheckin: evento.PinCheckin,
                Apresentacoes: []
            );
        }

        private static DataFormatadaResposta FormatarDataPorExtenso(DateTime data)
        {
            var culture = new System.Globalization.CultureInfo("pt-BR");
            var dataExtenso = data.ToString("dddd, d 'de' MMMM", culture);

            if (!string.IsNullOrEmpty(dataExtenso))
                dataExtenso = char.ToUpper(dataExtenso[0]) + dataExtenso[1..];

            return new DataFormatadaResposta(dataExtenso, data.ToString("HH:mm"));
        }
    }
}