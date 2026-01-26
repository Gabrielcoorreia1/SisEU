using SisEUs.Application.Apresentacoes.DTOs.Respostas;
using SisEUs.Application.Comum.DTOs;
using SisEUs.Application.Eventos.DTOs.Resposta;
using SisEUs.Application.Presencas.DTOs.Respostas;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Interfaces;
using SisEUs.Domain.ContextoDeUsuario.Entidades;
using SisEUs.Domain.ContextoDeUsuario.Interfaces;
using System.Globalization;

namespace SisEUs.Application.Comum.Mapeamento
{
    public class MapeadorDeEntidades : IMapeadorDeEntidades
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IEventoRepositorio _eventoRepositorio;

        public MapeadorDeEntidades(
            IUsuarioRepositorio usuarioRepositorio,
            IEventoRepositorio eventoRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _eventoRepositorio = eventoRepositorio;
        }

        public async Task<EventoResposta> MapearEventoAsync(Evento evento, CancellationToken cancellationToken = default)
        {
            var organizadoresDto = new List<ParticipanteResposta>();
            if (evento.ParticipantesIds.Count > 0)
            {
                var usuarios = await _usuarioRepositorio.ObterPorIdsAsync(evento.ParticipantesIds, cancellationToken);
                organizadoresDto = usuarios.Select(u => new ParticipanteResposta(
                    Id: u.Id,
                    NomeCompleto: $"{u.Nome.Nome} {u.Nome.Sobrenome}"
                )).ToList();
            }

            var avaliadoresNomes = new List<string>();
            if (evento.AvaliadoresIds.Count > 0)
            {
                var usuariosAvaliadores = await _usuarioRepositorio.ObterPorIdsAsync(evento.AvaliadoresIds, cancellationToken);
                avaliadoresNomes = usuariosAvaliadores.Select(u => $"{u.Nome.Nome} {u.Nome.Sobrenome}").ToList();
            }

            return CriarEventoResposta(evento, organizadoresDto, avaliadoresNomes);
        }

        public Task<IEnumerable<EventoResposta>> MapearEventosAsync(
            IEnumerable<Evento> eventos,
            Dictionary<int, Usuario> usuariosCache,
            CancellationToken cancellationToken = default)
        {
            var resultado = eventos.Select(evento =>
            {
                var organizadoresDto = evento.ParticipantesIds
                    .Where(id => usuariosCache.ContainsKey(id))
                    .Select(id => new ParticipanteResposta(
                        Id: id,
                        NomeCompleto: $"{usuariosCache[id].Nome.Nome} {usuariosCache[id].Nome.Sobrenome}"
                    )).ToList();

                var avaliadoresNomes = evento.AvaliadoresIds
                    .Where(id => usuariosCache.ContainsKey(id))
                    .Select(id => $"{usuariosCache[id].Nome.Nome} {usuariosCache[id].Nome.Sobrenome}")
                    .ToList();

                return CriarEventoResposta(evento, organizadoresDto, avaliadoresNomes);
            });

            return Task.FromResult(resultado);
        }

        public async Task<PresencaResposta> MapearPresencaAsync(Presenca presenca, CancellationToken cancellationToken = default)
        {
            var usuario = await _usuarioRepositorio.ObterPorIdAsync(presenca.UsuarioId, cancellationToken);
            var evento = await _eventoRepositorio.ObterEventoPorIdAsync(presenca.EventoId, cancellationToken);

            var usuarioDto = usuario != null 
                ? MapearUsuario(usuario)
                : new UsuarioResposta(
                    Id: 0,
                    NomeCompleto: "Usuário não encontrado",
                    Cpf: "Sem cpf",
                    Email: "Sem email"
                );

            var eventoDto = evento != null
                ? await MapearEventoAsync(evento, cancellationToken)
                : null;

            return new PresencaResposta(
                Id: presenca.Id,
                Usuario: usuarioDto,
                Evento: eventoDto,
                DataCheckIn: presenca.CheckIn,
                DataCheckOut: presenca.CheckOut,
                Localizacao: new LocalizacaoResposta(
                    Latitude: presenca.Localizacao.Latitude,
                    Longitude: presenca.Localizacao.Longitude
                )
            );
        }

        public async Task<IEnumerable<PresencaResposta>> MapearPresencasAsync(
            IEnumerable<Presenca> presencas,
            CancellationToken cancellationToken = default)
        {
            var usuarioIds = presencas.Select(p => p.UsuarioId).Distinct().ToList();
            var eventoIds = presencas.Select(p => p.EventoId).Distinct().ToList();

            var usuarios = await _usuarioRepositorio.ObterPorIdsAsync(usuarioIds, cancellationToken);
            var usuariosDict = usuarios.ToDictionary(u => u.Id);

            var resultados = new List<PresencaResposta>();
            
            foreach (var presenca in presencas)
            {
                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(presenca.EventoId, cancellationToken);

                var usuarioDto = usuariosDict.TryGetValue(presenca.UsuarioId, out var usuario)
                    ? MapearUsuario(usuario)
                    : new UsuarioResposta(
                        Id: 0,
                        NomeCompleto: "Usuário não encontrado",
                        Cpf: "Sem cpf",
                        Email: "Sem email"
                    );

                var eventoDto = evento != null
                    ? await MapearEventoAsync(evento, cancellationToken)
                    : null;

                resultados.Add(new PresencaResposta(
                    Id: presenca.Id,
                    Usuario: usuarioDto,
                    Evento: eventoDto,
                    DataCheckIn: presenca.CheckIn,
                    DataCheckOut: presenca.CheckOut,
                    Localizacao: new LocalizacaoResposta(
                        Latitude: presenca.Localizacao.Latitude,
                        Longitude: presenca.Localizacao.Longitude
                    )
                ));
            }

            return resultados;
        }

        public async Task<ApresentacaoResposta> MapearApresentacaoAsync(Apresentacao apresentacao, CancellationToken cancellationToken = default)
        {
            var evento = await _eventoRepositorio.ObterEventoPorIdAsync(apresentacao.EventoId, cancellationToken);
            
            var eventoDto = evento != null
                ? await MapearEventoAsync(evento, cancellationToken)
                : null!;

            return new ApresentacaoResposta(
                apresentacao.Id,
                eventoDto,
                apresentacao.Titulo.Valor,
                apresentacao.NomeAutor,
                apresentacao.NomeOrientador
            );
        }

        public UsuarioResposta MapearUsuario(Usuario usuario)
        {
            return new UsuarioResposta(
                Id: usuario.Id,
                NomeCompleto: $"{usuario.Nome.Nome} {usuario.Nome.Sobrenome}",
                Cpf: usuario.Cpf.Valor,
                Email: usuario.Email.Valor
            );
        }

        private static EventoResposta CriarEventoResposta(
            Evento evento,
            List<ParticipanteResposta> organizadores,
            List<string> avaliadores)
        {
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
                Organizadores: organizadores,
                Avaliadores: avaliadores,
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
            var culture = new CultureInfo("pt-BR");
            var dataExtenso = data.ToString("dddd, d 'de' MMMM", culture);

            if (!string.IsNullOrEmpty(dataExtenso))
                dataExtenso = char.ToUpper(dataExtenso[0]) + dataExtenso[1..];

            return new DataFormatadaResposta(dataExtenso, data.ToString("HH:mm"));
        }
    }
}
