using SisEUs.Application.Apresentacoes.Abstractions;
using SisEUs.Application.Apresentacoes.DTOs.Respostas;
using SisEUs.Application.Comum.Mapeamento;
using SisEUs.Application.Comum.Resultados;
using SisEUs.Application.Comum.UoW;
using SisEUs.Application.Eventos.Abstracoes;
using SisEUs.Application.Eventos.DTOs.Resposta;
using SisEUs.Application.Eventos.DTOs.Solicitacoes;
using SisEUs.Application.Eventos.Mappers;
using SisEUs.Domain.Comum.Excecoes;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Interfaces;
using SisEUs.Domain.ContextoDeEvento.ObjetosDeValor;
using SisEUs.Domain.ContextoDeEvento.Servicos;
using SisEUs.Domain.ContextoDeUsuario.Interfaces;
using SisEUs.Domain.ContextoDeUsuario.ObjetosDeValor;
using Microsoft.Extensions.Logging;
using SisEUs.Domain.Comum.LoggedUser;

namespace SisEUs.Application.Eventos
{
    public class EventoServico : IEventoServico
    {
        private readonly IEventoRepositorio _eventoRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IUoW _uow;
        private readonly IApresentacaoServico _servico;
        private readonly ILogger<EventoServico> _logger;
        private readonly ILoggedUser _loggedUser;
        private readonly IGeolocalizacaoValidador _geolocalizacaoValidador;
        private readonly IMapeadorDeEntidades _mapeador;

        public EventoServico(
            IEventoRepositorio eventoRepositorio,
            IUsuarioRepositorio usuarioRepositorio,
            IUoW uow,
            IApresentacaoServico servico,
            ILogger<EventoServico> logger,
            IGeolocalizacaoValidador geolocalizacaoValidador,
            IMapeadorDeEntidades mapeador)
        {
            _eventoRepositorio = eventoRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _uow = uow;
            _servico = servico;
            _logger = logger;
            _geolocalizacaoValidador = geolocalizacaoValidador;
            _mapeador = mapeador;
        }

        public async Task<Resultado<EventoResposta>> CriarEventoAsync(CriarEventoSolicitacao request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                _logger.LogWarning("Tentativa de criar evento com requisição nula");
                return Resultado<EventoResposta>.Falha(TipoDeErro.Validacao, "Requisição inválida.");
            }

            _logger.LogInformation("Iniciando criação de evento: {Titulo}", request.Titulo);

            try
            {
                if (await _eventoRepositorio.CodigoUnicoJaExisteAsync(request.CodigoUnico, null, cancellationToken))
                {
                    _logger.LogWarning("Tentativa de criar evento com código único duplicado: {CodigoUnico}", request.CodigoUnico);
                    return Resultado<EventoResposta>.Falha(TipoDeErro.Conflito, $"Já existe um evento com o código '{request.CodigoUnico}'.");
                }

                // Converter CPFs dos avaliadores para IDs
                var avaliadoresIds = new List<int>();
                if (request.CpfsAvaliadores is not null && request.CpfsAvaliadores.Count > 0)
                {
                    var resultadoConversao = await ConverterCpfsParaIdsAsync(request.CpfsAvaliadores, cancellationToken);
                    if (!resultadoConversao.Sucesso)
                    {
                        return Resultado<EventoResposta>.Falha(resultadoConversao.TipoDeErro!.Value, resultadoConversao.Erros.First());
                    }
                    avaliadoresIds = resultadoConversao.Valor.ToList();
                }

                var nomeCampus = request.Local.Campus.ToString();
                var coordenadasCampus = _geolocalizacaoValidador.ObterCoordenadasCampusPorNome(nomeCampus);

                if (coordenadasCampus is null)
                {
                    _logger.LogWarning("Campus não encontrado: {Campus}", nomeCampus);
                    return Resultado<EventoResposta>.Falha(TipoDeErro.Validacao, $"Campus '{nomeCampus}' não encontrado.");
                }

                var local = Local.Criar(
                    nomeCampus,
                    request.Local.Departamento,
                    request.Local.Bloco,
                    request.Local.Sala
                );

                var localizacao = Localizacao.Criar(
                    coordenadasCampus.Value.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture),
                    coordenadasCampus.Value.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture)
                );

                var titulo = Titulo.Criar(request.Titulo);

                var evento = Evento.Criar(
                    titulo,
                    request.DataInicio,
                    request.DataFim,
                    local,
                    [],
                    avaliadoresIds,
                    localizacao,
                    request.ImgUrl,
                    request.CodigoUnico,
                    request.ETipoEvento
                );

                await _eventoRepositorio.CriarEventoAsync(evento, cancellationToken);
                await _uow.CommitAsync(cancellationToken);

                if (request.Apresentacoes is not null && request.Apresentacoes.Count > 0)
                {
                    foreach (var apresentacao in request.Apresentacoes)
                    {
                        var novaSolicitacao = apresentacao with { EventoId = evento.Id };
                        await _servico.CriarApresentacaoAsync(novaSolicitacao, cancellationToken);
                    }
                }

                var resposta = await evento.ToResponseDtoAsync(_usuarioRepositorio, cancellationToken);

                _logger.LogInformation("Evento criado com sucesso: {EventoId}", evento.Id);
                return Resultado<EventoResposta>.Ok(resposta);
            }
            catch (ExcecaoDeDominio ex)
            {
                _logger.LogWarning(ex, "Erro de domínio ao criar evento");
                return Resultado<EventoResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar evento");
                return Resultado<EventoResposta>.Falha(TipoDeErro.Inesperado, "Erro interno ao criar evento.");
            }
        }

        public async Task<Resultado> AtualizarEventoAsync(int id, AtualizarEventoSolicitacao request, CancellationToken cancellationToken)
        {
            if (request is null)
            {
                _logger.LogWarning("Tentativa de atualizar evento com requisição nula");
                return Resultado.Falha(TipoDeErro.Validacao, "Requisição inválida.");
            }

            _logger.LogInformation("Iniciando atualização do evento: {EventoId}", id);

            try
            {
                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(id, cancellationToken);

                if (evento is null)
                {
                    _logger.LogWarning("Evento não encontrado: {EventoId}", id);
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");
                }

                if (await _eventoRepositorio.CodigoUnicoJaExisteAsync(request.CodigoUnico, id, cancellationToken))
                {
                    _logger.LogWarning("Tentativa de atualizar evento com código único duplicado: {CodigoUnico}", request.CodigoUnico);
                    return Resultado.Falha(TipoDeErro.Conflito, $"Já existe outro evento com o código '{request.CodigoUnico}'.");
                }

                // Converter CPFs dos avaliadores para IDs
                var avaliadoresIds = new List<int>();
                if (request.CpfsAvaliadores is not null && request.CpfsAvaliadores.Count > 0)
                {
                    var resultadoConversao = await ConverterCpfsParaIdsAsync(request.CpfsAvaliadores, cancellationToken);
                    if (!resultadoConversao.Sucesso)
                    {
                        return Resultado.Falha(resultadoConversao.TipoDeErro!.Value, resultadoConversao.Erros.First());
                    }
                    avaliadoresIds = resultadoConversao.Valor.ToList();
                }

                evento.AtualizarTitulo(request.Titulo);
                evento.AtualizarDataInicio(request.DataInicio);
                evento.AtualizarDataFim(request.DataFim);
                evento.AtualizarLocal(
                    request.Local.Campus.ToString(),
                    request.Local.Departamento,
                    request.Local.Bloco,
                    request.Local.Sala
                );
                evento.AtualizarImg(request.ImgUrl);
                evento.AtualizarCodigoUnico(request.CodigoUnico);
                evento.AtualizarTipoEvento(request.ETipoEvento);
                evento.AtualizarAvaliadores(avaliadoresIds);

                await _uow.CommitAsync(cancellationToken);

                _logger.LogInformation("Evento atualizado com sucesso: {EventoId}", id);
                return Resultado.Ok();
            }
            catch (ExcecaoDeDominio ex)
            {
                _logger.LogWarning(ex, "Erro de domínio ao atualizar evento");
                return Resultado.Falha(TipoDeErro.Validacao, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao atualizar evento");
                return Resultado.Falha(TipoDeErro.Inesperado, "Erro interno ao atualizar evento.");
            }
        }

        public async Task<Resultado> ExcluirEventoAsync(int eventoId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando exclusão do evento: {EventoId}", eventoId);

            try
            {
                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken);

                if (evento is null)
                {
                    _logger.LogWarning("Evento não encontrado para exclusão: {EventoId}", eventoId);
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");
                }

                _eventoRepositorio.ExcluirEvento(evento);
                await _uow.CommitAsync(cancellationToken);

                _logger.LogInformation("Evento excluído com sucesso: {EventoId}", eventoId);
                return Resultado.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao excluir evento");
                return Resultado.Falha(TipoDeErro.Inesperado, "Erro interno ao excluir evento.");
            }
        }

        public async Task<Resultado<EventoResposta>> ObterEventoPorIdAsync(int eventoId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Buscando evento por ID: {EventoId}", eventoId);

            try
            {
                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken);

                if (evento is null)
                {
                    _logger.LogWarning("Evento não encontrado: {EventoId}", eventoId);
                    return Resultado<EventoResposta>.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");
                }

                var resposta = await evento.ToResponseDtoAsync(_usuarioRepositorio, cancellationToken);

                var apresentacoesDto = new List<ApresentacaoResposta>();
                foreach (var apresentacao in evento.Apresentacoes)
                {
                    var apresentacaoDto = await _mapeador.MapearApresentacaoAsync(apresentacao, cancellationToken);
                    apresentacoesDto.Add(apresentacaoDto);
                }

                resposta = resposta with { Apresentacoes = apresentacoesDto };

                return Resultado<EventoResposta>.Ok(resposta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao buscar evento por ID");
                return Resultado<EventoResposta>.Falha(TipoDeErro.Inesperado, "Erro interno ao buscar evento.");
            }
        }

        public async Task<Resultado<IEnumerable<EventoResposta>>> ListarEventosAsync(int pagina, int tamanho, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Listando eventos - Página: {Pagina}, Tamanho: {Tamanho}", pagina, tamanho);

            try
            {
                var skip = (pagina - 1) * tamanho;
                var eventos = await _eventoRepositorio.ObterEventosPaginadosAsync(skip, tamanho, cancellationToken);

                var respostas = new List<EventoResposta>();

                foreach (var evento in eventos)
                {
                    var resposta = await evento.ToResponseDtoAsync(_usuarioRepositorio, cancellationToken);
                    respostas.Add(resposta);
                }

                return Resultado<IEnumerable<EventoResposta>>.Ok(respostas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao listar eventos");
                return Resultado<IEnumerable<EventoResposta>>.Falha(TipoDeErro.Inesperado, "Erro interno ao listar eventos.");
            }
        }

        public async Task<Resultado> AdicionarParticipanteAsync(int participanteId, int eventoId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adicionando participante {ParticipanteId} ao evento {EventoId}", participanteId, eventoId);

            try
            {
                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken);

                if (evento is null)
                {
                    _logger.LogWarning("Evento não encontrado: {EventoId}", eventoId);
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");
                }

                var usuario = await _usuarioRepositorio.ObterPorIdAsync(participanteId, cancellationToken);

                if (usuario is null)
                {
                    _logger.LogWarning("Usuário não encontrado: {ParticipanteId}", participanteId);
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, "Usuário não encontrado.");
                }

                evento.AdicionarParticipante(participanteId);
                await _uow.CommitAsync(cancellationToken);

                _logger.LogInformation("Participante {ParticipanteId} adicionado ao evento {EventoId} com sucesso", participanteId, eventoId);
                return Resultado.Ok();
            }
            catch (ExcecaoDeDominio ex)
            {
                _logger.LogWarning(ex, "Erro de domínio ao adicionar participante");
                return Resultado.Falha(TipoDeErro.Validacao, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao adicionar participante");
                return Resultado.Falha(TipoDeErro.Inesperado, "Erro interno ao adicionar participante.");
            }
        }

        public async Task<Resultado> RemoverParticipanteAsync(int participanteId, int eventoId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Removendo participante {ParticipanteId} do evento {EventoId}", participanteId, eventoId);

            try
            {
                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken);

                if (evento is null)
                {
                    _logger.LogWarning("Evento não encontrado: {EventoId}", eventoId);
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");
                }

                evento.RemoverParticipante(participanteId);
                await _uow.CommitAsync(cancellationToken);

                _logger.LogInformation("Participante {ParticipanteId} removido do evento {EventoId} com sucesso", participanteId, eventoId);
                return Resultado.Ok();
            }
            catch (ExcecaoDeDominio ex)
            {
                _logger.LogWarning(ex, "Erro de domínio ao remover participante");
                return Resultado.Falha(TipoDeErro.Validacao, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao remover participante");
                return Resultado.Falha(TipoDeErro.Inesperado, "Erro interno ao remover participante.");
            }
        }

        public async Task<Resultado> AdicionarAvaliadorPorCpfAsync(string cpf, int eventoId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adicionando avaliador com CPF {Cpf} ao evento {EventoId}", cpf, eventoId);

            try
            {
                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken);

                if (evento is null)
                {
                    _logger.LogWarning("Evento não encontrado: {EventoId}", eventoId);
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");
                }

                var cpfObjeto = Cpf.Criar(cpf);
                var usuario = await _usuarioRepositorio.ObterPorCpfAsync(cpfObjeto, cancellationToken);

                if (usuario is null)
                {
                    _logger.LogWarning("Usuário não encontrado com CPF: {Cpf}", cpf);
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, $"Usuário com CPF '{cpf}' não encontrado.");
                }

                evento.AdicionarAvaliador(usuario.Id);
                await _uow.CommitAsync(cancellationToken);

                _logger.LogInformation("Avaliador {AvaliadorId} adicionado ao evento {EventoId} com sucesso", usuario.Id, eventoId);
                return Resultado.Ok();
            }
            catch (ExcecaoDeDominio ex)
            {
                _logger.LogWarning(ex, "Erro de domínio ao adicionar avaliador por CPF");
                return Resultado.Falha(TipoDeErro.Validacao, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao adicionar avaliador");
                return Resultado.Falha(TipoDeErro.Inesperado, "Erro interno ao adicionar avaliador.");
            }
        }

        public async Task<Resultado> RemoverAvaliadorAsync(int avaliadorId, int eventoId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Removendo avaliador {AvaliadorId} do evento {EventoId}", avaliadorId, eventoId);

            try
            {
                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken);

                if (evento is null)
                {
                    _logger.LogWarning("Evento não encontrado: {EventoId}", eventoId);
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");
                }

                evento.RemoverAvaliador(avaliadorId);
                await _uow.CommitAsync(cancellationToken);

                _logger.LogInformation("Avaliador {AvaliadorId} removido do evento {EventoId} com sucesso", avaliadorId, eventoId);
                return Resultado.Ok();
            }
            catch (ExcecaoDeDominio ex)
            {
                _logger.LogWarning(ex, "Erro de domínio ao remover avaliador");
                return Resultado.Falha(TipoDeErro.Validacao, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao remover avaliador");
                return Resultado.Falha(TipoDeErro.Inesperado, "Erro interno ao remover avaliador.");
            }
        }

        public async Task<Resultado<EventoResposta>> ObterPorCodigoEvento(string codigoEvento, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Buscando evento por código: {CodigoEvento}", codigoEvento);

            try
            {
                if (string.IsNullOrWhiteSpace(codigoEvento))
                {
                    _logger.LogWarning("Código de evento inválido");
                    return Resultado<EventoResposta>.Falha(TipoDeErro.Validacao, "Código de evento inválido.");
                }

                var evento = await _eventoRepositorio.ObterEventoPorCodigoAsync(codigoEvento, cancellationToken);

                if (evento is null)
                {
                    _logger.LogWarning("Evento não encontrado com código: {CodigoEvento}", codigoEvento);
                    return Resultado<EventoResposta>.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");
                }

                var resposta = await evento.ToResponseDtoAsync(_usuarioRepositorio, cancellationToken);

                return Resultado<EventoResposta>.Ok(resposta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao buscar evento por código");
                return Resultado<EventoResposta>.Falha(TipoDeErro.Inesperado, "Erro interno ao buscar evento.");
            }
        }

        public async Task<Resultado<IEnumerable<EventoResposta>>> ObterEventosPorAvaliadorAsync(int avaliadorId, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Buscando eventos para o avaliador: {AvaliadorId}", avaliadorId);

            try
            {
                var usuario = await _usuarioRepositorio.ObterPorIdAsync(avaliadorId, cancellationToken);

                if (usuario is null)
                {
                    _logger.LogWarning("Usuário não encontrado: {AvaliadorId}", avaliadorId);
                    return Resultado<IEnumerable<EventoResposta>>.Falha(TipoDeErro.NaoEncontrado, "Usuário não encontrado.");
                }

                var eventos = await _eventoRepositorio.ObterEventosPorAvaliadorIdAsync(avaliadorId, cancellationToken);

                var respostas = new List<EventoResposta>();

                foreach (var evento in eventos)
                {
                    var resposta = await evento.ToResponseDtoAsync(_usuarioRepositorio, cancellationToken);
                    
                    var apresentacoesDto = new List<ApresentacaoResposta>();
                    foreach (var apresentacao in evento.Apresentacoes)
                    {
                        var apresentacaoDto = await _mapeador.MapearApresentacaoAsync(apresentacao, cancellationToken);
                        apresentacoesDto.Add(apresentacaoDto);
                    }

                    resposta = resposta with { Apresentacoes = apresentacoesDto };
                    respostas.Add(resposta);
                }

                _logger.LogInformation("Encontrados {Count} eventos para o avaliador {AvaliadorId}", respostas.Count, avaliadorId);
                return Resultado<IEnumerable<EventoResposta>>.Ok(respostas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao buscar eventos por avaliador");
                return Resultado<IEnumerable<EventoResposta>>.Falha(TipoDeErro.Inesperado, "Erro interno ao buscar eventos.");
            }
        }

        public async Task<Resultado<IEnumerable<EventoResposta>>> ObterMeusEventosComoAvaliadorAsync(CancellationToken cancellationToken)
        {
            try
            {
                var usuarioLogado = await _loggedUser.User();
                
                if (usuarioLogado is null)
                {
                    return Resultado<IEnumerable<EventoResposta>>.Falha(TipoDeErro.NaoEncontrado, "Usuário não encontrado.");
                }

                return await ObterEventosPorAvaliadorAsync(usuarioLogado.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter eventos do avaliador logado");
                return Resultado<IEnumerable<EventoResposta>>.Falha(TipoDeErro.Inesperado, "Erro interno ao buscar eventos.");
            }
        }

        private async Task<Resultado<IEnumerable<int>>> ConverterCpfsParaIdsAsync(List<string> cpfs, CancellationToken cancellationToken)
        {
            var usuarios = await _usuarioRepositorio.ObterPorCpfsAsync(cpfs, cancellationToken);
            var usuariosList = usuarios.ToList();

            var cpfsEncontrados = usuariosList.Select(u => u.Cpf.Valor).ToHashSet();
            var cpfsNaoEncontrados = cpfs.Where(cpf => !cpfsEncontrados.Contains(cpf)).ToList();

            if (cpfsNaoEncontrados.Any())
            {
                var cpfsString = string.Join(", ", cpfsNaoEncontrados);
                _logger.LogWarning("CPFs não encontrados: {Cpfs}", cpfsString);
                return Resultado<IEnumerable<int>>.Falha(TipoDeErro.NaoEncontrado, $"Usuários com os seguintes CPFs não foram encontrados: {cpfsString}");
            }

            return Resultado<IEnumerable<int>>.Ok(usuariosList.Select(u => u.Id));
        }
    }
}