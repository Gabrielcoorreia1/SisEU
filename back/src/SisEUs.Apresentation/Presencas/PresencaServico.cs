using Microsoft.Extensions.Logging;
using SisEUs.Application.Comum.Resultados;
using SisEUs.Application.Comum.UoW;
using SisEUs.Application.Eventos.Mappers;
using SisEUs.Application.Presencas.Abstracoes;
using SisEUs.Application.Presencas.DTOs.Respostas;
using SisEUs.Application.Presencas.DTOs.Solicitacoes;
using SisEUs.Application.Presencas.Mapper;
using SisEUs.Domain.Comum.Excecoes;
using SisEUs.Domain.Comum.LoggedUser;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Excecoes;
using SisEUs.Domain.ContextoDeEvento.Interfaces;
using SisEUs.Domain.ContextoDeUsuario.Interfaces;
using System.Globalization;
using System.Threading.Tasks;

namespace SisEUs.Application.Presencas
{
    public class PresencaServico : IPresencaServico
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IPresencaRepositorio _presencaRepositorio;
        private readonly IEventoRepositorio _eventoRepositorio;
        private readonly ILoggedUser _loggedUser;
        private readonly IUoW _uow;
        public PresencaServico(
            IUsuarioRepositorio usuarioRepositorio,
            IPresencaRepositorio presencaRepositorio,
            IEventoRepositorio eventoRepositorio,
            ILoggedUser loggedUser,
            IUoW uow)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _presencaRepositorio = presencaRepositorio;
            _eventoRepositorio = eventoRepositorio;
            _loggedUser = loggedUser;
            _uow = uow;
        }

        public async Task<Resultado<PresencaResposta>> EfetuarCheckInAsync(EfetuarCheckInSolicitacao request, CancellationToken cancellationToken)
        {
            var dataCheckIn = DateTime.Now;

            try
            {
                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(request.EventoId, cancellationToken);
                if (evento is null)
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");

                var distrancia = CalcularDistancia(request.Latitude, request.Longitude, evento.Localizacao.Latitude, evento.Localizacao.Longitude);
                if (!distrancia)
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.Validacao, "Usuário fora do raio permitido para check-in.");

                var usuario = await _usuarioRepositorio.ObterPorIdAsync(request.UsuarioId, cancellationToken);
                if (usuario is null)
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.NaoEncontrado, "Usuário não encontrado.");
                var presencaExistente = await _presencaRepositorio.BuscarPorUsuarioEEventoAsync(request.EventoId, request.UsuarioId, cancellationToken);
                if (presencaExistente != null)
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.Conflito, "Usuário já está presente neste evento.");

                if (dataCheckIn < evento.DataInicio)
                    throw new EventoNaoComecouExcecao();
                if (dataCheckIn > evento.DataFim)
                    throw new EventoFinalizadoExcecao("Check-in");

                var presenca = Presenca.Criar(usuario.Id, evento.Id, request.Latitude, request.Longitude);

                presenca.RealizarCheckIn(dataCheckIn);

                await _presencaRepositorio.CriarPresenca(presenca, cancellationToken);
                await _uow.CommitAsync(cancellationToken);

                var dto = await presenca.ToResponseDtoAsync(_usuarioRepositorio, _eventoRepositorio, cancellationToken);
                return Resultado<PresencaResposta>.Ok(dto);

            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado<PresencaResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }


        }

        public async Task<Resultado<PresencaResposta>> EfetuarCheckOutAsync(EfetuarCheckOutSolicitacao request, CancellationToken cancellationToken)
        {
            try
            {
                var dataCheckOut = DateTime.Now;
                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(request.EventoId, cancellationToken);
                if (evento is null)
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");

                var distrancia = CalcularDistancia(request.Latitude, request.Longitude, evento.Localizacao.Latitude, evento.Localizacao.Longitude);
                if (!distrancia)
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.Validacao, "Usuário fora do raio permitido para check-out.");

                var usuario = await _usuarioRepositorio.ObterPorIdAsync(request.UsuarioId, cancellationToken);
                if (usuario is null)
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.NaoEncontrado, "Usuário não encontrado.");

                var presenca = await _presencaRepositorio.BuscarPorUsuarioEEventoAsync(request.EventoId, request.UsuarioId, cancellationToken);
                if (presenca is null || !presenca.CheckInValido)
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.NaoEncontrado, "Nenhum check-in ativo encontrado para este usuário no evento.");

                if (dataCheckOut < evento.DataInicio || dataCheckOut > evento.DataFim)
                    throw new EventoFinalizadoExcecao("Check-out");

                if (request.UsuarioId != presenca.UsuarioId)
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.Validacao, "Usuário não autorizado a realizar check-out neste evento.");

                presenca.RealizarCheckOut(dataCheckOut);

                await _uow.CommitAsync(cancellationToken);

                var dto = await presenca.ToResponseDtoAsync(_usuarioRepositorio, _eventoRepositorio, cancellationToken);

                return Resultado<PresencaResposta>.Ok(dto);
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado<PresencaResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }

        public async Task<Resultado<PresencaResposta>> ObterPorIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var presenca = await _presencaRepositorio.ObterPresencaPorIdAsync(id, cancellationToken);
                if (presenca is null)
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.NaoEncontrado, "Registro de presença não encontrado.");

                var dto = await presenca.ToResponseDtoAsync(_usuarioRepositorio, _eventoRepositorio, cancellationToken);

                return Resultado<PresencaResposta>.Ok(dto);

            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado<PresencaResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }

        public async Task<Resultado<IEnumerable<PresencaResposta>>> ListarPorEventoAsync(int eventoId, CancellationToken cancellationToken)
        {
            try
            {
                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken);
                if (evento is null)
                    return Resultado<IEnumerable<PresencaResposta>>.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");

                var presencas = await _presencaRepositorio.ObterPresencas();
                if (presencas is null || !presencas.Any())
                    return Resultado<IEnumerable<PresencaResposta>>.Falha(TipoDeErro.NaoEncontrado, "Nenhum registro de presença encontrado para este evento.");

                var dtosTask = presencas.Select(p => p.ToResponseDtoAsync(_usuarioRepositorio, _eventoRepositorio, cancellationToken));
                var dtos = await Task.WhenAll(dtosTask);

                return Resultado<IEnumerable<PresencaResposta>>.Ok(dtos);
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado<IEnumerable<PresencaResposta>>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }
        public async Task<Resultado<IEnumerable<PresencaResposta>>> ObterRelatorioAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var presencas = await _presencaRepositorio.ObterPresencas();
                if (presencas is null || !presencas.Any())
                    return Resultado<IEnumerable<PresencaResposta>>.Falha(TipoDeErro.NaoEncontrado, "Nenhum registro de presença encontrado.");

                var dtosTask = presencas.Select(p => p.ToResponseDtoAsync(_usuarioRepositorio, _eventoRepositorio, cancellationToken));
                var dtos = await Task.WhenAll(dtosTask);
                return Resultado<IEnumerable<PresencaResposta>>.Ok(dtos);

            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado<IEnumerable<PresencaResposta>>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }

        public async Task<Resultado<StatusPresencaResposta>> ObterStatusPresencaEventoAsync(int eventoId, CancellationToken cancellationToken = default)
        {
            try
            {

                var usuarioAtual = await _loggedUser.User();

                var eventoExiste = await _eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken);

                if (eventoExiste is null)
                    return Resultado<StatusPresencaResposta>.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");

                var presencas = await _presencaRepositorio.ObterStatusPresencaPorEvento(eventoId, usuarioAtual.Id, cancellationToken);

                if (presencas is null)
                    return Resultado<StatusPresencaResposta>.Falha(TipoDeErro.NaoEncontrado, "Nenhum registro de presença encontrado para este usuário neste evento.");

                var status = new StatusPresencaResposta
                {
                    CheckInEfetuado = presencas.CheckInValido,
                    CheckOutEfetuado = presencas.CheckOutValido,
                };

                return Resultado<StatusPresencaResposta>.Ok(status);
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado<StatusPresencaResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }

        public async Task<Resultado<bool?>> ObterPresencaEventoEmAndamentoAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var usuarioAtual = await _loggedUser.User();

                var eventoEmAndamento = await _presencaRepositorio.ObterPresencaEventoEmAndamentoAsync(cancellationToken);
                if (eventoEmAndamento is null)
                    return Resultado<bool?>.Falha(TipoDeErro.NaoEncontrado, "Nenhum evento em andamento encontrado.");

                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(eventoEmAndamento.EventoId, cancellationToken);
                if (evento is null)
                    return Resultado<bool?>.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");

                if (evento.DataFim < DateTime.Now)
                    return Resultado<bool?>.Falha(TipoDeErro.NaoEncontrado, "Evento já finalizado.");

                return Resultado<bool?>.Ok(true);

            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado<bool?>.Falha(TipoDeErro.Validacao, ex.Message);

            }
        }
        private static double ParaRadianos(double anguloEmGraus)
        {
            return anguloEmGraus * Math.PI / 180;
        }
        private static bool CalcularDistancia(string l1, string l2, string r1, string r2)
        {
            var latitude1 = ConverterStringToDouble(l1);
            var longitude1 = ConverterStringToDouble(l2);
            var latitude2 = ConverterStringToDouble(r1);
            var longitude2 = ConverterStringToDouble(r2);

            var lat1Rad = ParaRadianos(latitude1);
            var lon1Rad = ParaRadianos(longitude1);
            var lat2Rad = ParaRadianos(latitude2);
            var lon2Rad = ParaRadianos(longitude2);

            var deltaLat = lat2Rad - lat1Rad;
            var deltaLon = lon2Rad - lon1Rad;

            var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                    Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                    Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var distancia = 6371000 * c;

            return distancia <= 100;
        }
        private static double ConverterStringToDouble(string a)
        {
            double result;
            double.TryParse(a, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
            return result;
        }
    }
}
