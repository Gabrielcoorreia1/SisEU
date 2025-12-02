using SisEUs.Application.Eventos.DTOs.Resposta;
using SisEUs.Application.Eventos.DTOs.Solicitacoes;
using SisEUs.Application.Presencas.DTOs.Respostas;
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

            if (evento.Participantes.Any())
            {
                var usuarios = await usuarioRepositorio.ObterPorIdsAsync(evento.Participantes, cancellationToken);
                organizadoresDto = usuarios.Select(u => new ParticipanteResposta
                {
                    Id = u.Id,
                    NomeCompleto = (u.Nome.Nome + " " + u.Nome.Sobrenome),
                }).ToList();
            }

            string avaliadoresString = evento.Avaliadores; 
            string[] avaliadoresArray = avaliadoresString.Split(';', StringSplitOptions.RemoveEmptyEntries);

            return new EventoResposta
            {
                Id = evento.Id,
                Titulo = evento.Titulo.Valor,
                Localizacao = new LocalizacaoResposta
                {
                    Latitude = evento.Localizacao.Latitude,
                    Longitude = evento.Localizacao.Longitude
                },
                Local = new CriarLocalSolicitacao
                {
                    Campus = Enum.Parse<Campus>(evento.Local.Campus, ignoreCase: true),
                    Departamento = evento.Local.Departamento,
                    Bloco = evento.Local.Bloco,
                    Sala = evento.Local.Sala
                },
                DataInicio = FormatarDataPorExtenso(evento.DataInicio),
                DataFim = FormatarDataPorExtenso(evento.DataFim),
                Organizadores = organizadoresDto,
                ImgUrl = evento.ImgUrl,
                CodigoUnico = evento.CodigoUnico,
                PinCheckin = evento.PinCheckin,
                ETipoEvento = evento.TipoEvento.ToString(),
                NomeCampus = evento.Local.Campus.ToString(),
                DataDateTime = evento.DataInicio,
                Avaliadores = avaliadoresArray.ToList(),
                EventType = evento.TipoEvento,
            };
        }

        private static Data FormatarDataPorExtenso(DateTime data)
        {
            var dataExtenso = data.ToString("dddd, d 'de' MMMM", new System.Globalization.CultureInfo("pt-BR"));
            dataExtenso = char.ToUpper(dataExtenso[0]) + dataExtenso.Substring(1);

            return new Data
            {
                DataPorExtenso = dataExtenso,
                Hora = ExtrairHora(data)
            };
        }

        private static string ExtrairHora(DateTime data)
        {
            return data.ToString("HH:mm");
        }
    }
}
