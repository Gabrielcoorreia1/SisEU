using SisEUs.Application.Comum.Mapeamento;
using SisEUs.Application.Comum.Resultados;
using SisEUs.Application.Comum.Servicos;
using SisEUs.Application.Comum.UoW;
using SisEUs.Application.Presencas.Abstracoes;
using SisEUs.Application.Presencas.DTOs.Respostas;
using SisEUs.Application.Presencas.DTOs.Solicitacoes;
using SisEUs.Domain.Comum.Excecoes;
using SisEUs.Domain.Comum.LoggedUser;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Excecoes;
using SisEUs.Domain.ContextoDeEvento.Interfaces;
using SisEUs.Domain.ContextoDeUsuario.Interfaces;
using Microsoft.Extensions.Logging;

namespace SisEUs.Application.Presencas
{
    public class PresencaServico(
        IUsuarioRepositorio usuarioRepositorio,
        IPresencaRepositorio presencaRepositorio,
        IEventoRepositorio eventoRepositorio,
        ILoggedUser loggedUser,
        ILogger<PresencaServico> logger,
        IUoW uow,
        IValidadorDeCoordenadas validadorDeCoordenadas,
        IMapeadorDeEntidades mapeador) : IPresencaServico
    {
        public async Task<Resultado<PresencaResposta>> EfetuarCheckInAsync(EfetuarCheckInSolicitacao request, CancellationToken cancellationToken)
        {
            var dataCheckIn = DateTime.Now;

            logger.LogInformation("Iniciando check-in para usuário {UsuarioId} no evento {EventoId}", request.UsuarioId, request.EventoId);

            try
            {
                var evento = await eventoRepositorio.ObterEventoPorIdAsync(request.EventoId, cancellationToken);
                if (evento is null)
                {
                    logger.LogWarning("Evento {EventoId} não encontrado para check-in", request.EventoId);
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");
                }

                var resultadoDistancia = validadorDeCoordenadas.ValidarDistanciaParaEvento(
                    request.Latitude, 
                    request.Longitude, 
                    evento.Localizacao.Latitude, 
                    evento.Localizacao.Longitude);

                if (!resultadoDistancia.Sucesso)
                {
                    logger.LogWarning("Usuário {UsuarioId} fora do raio permitido para check-in no evento {EventoId}", request.UsuarioId, request.EventoId);
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.Validacao, resultadoDistancia.Erros.First());
                }

                var usuario = await usuarioRepositorio.ObterPorIdAsync(request.UsuarioId, cancellationToken);
                if (usuario is null)
                {
                    logger.LogWarning("Usuário {UsuarioId} não encontrado para check-in", request.UsuarioId);
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.NaoEncontrado, "Usuário não encontrado.");
                }

                var presencaExistente = await presencaRepositorio.BuscarPorUsuarioEEventoAsync(request.EventoId, request.UsuarioId, cancellationToken);
                if (presencaExistente != null)
                {
                    logger.LogWarning("Usuário {UsuarioId} já possui presença no evento {EventoId}", request.UsuarioId, request.EventoId);
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.Conflito, "Usuário já está presente neste evento.");
                }

                if (dataCheckIn < evento.DataInicio)
                    throw new EventoNaoComecouExcecao();
                if (dataCheckIn > evento.DataFim)
                    throw new EventoFinalizadoExcecao("Check-in");

                var presenca = Presenca.Criar(usuario.Id, evento.Id, request.Latitude, request.Longitude);
                presenca.RealizarCheckIn(dataCheckIn);

                presencaRepositorio.CriarPresenca(presenca, cancellationToken);
                await uow.CommitAsync(cancellationToken);

                logger.LogInformation("Check-in realizado com sucesso para usuário {UsuarioId} no evento {EventoId}", request.UsuarioId, request.EventoId);

                var dto = await mapeador.MapearPresencaAsync(presenca, cancellationToken);
                return Resultado<PresencaResposta>.Ok(dto);
            }
            catch (ExcecaoDeDominio ex)
            {
                logger.LogError(ex, "Erro de domínio ao efetuar check-in para usuário {UsuarioId} no evento {EventoId}", request.UsuarioId, request.EventoId);
                return Resultado<PresencaResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro inesperado ao efetuar check-in para usuário {UsuarioId} no evento {EventoId}", request.UsuarioId, request.EventoId);
                throw;
            }
        }

        public async Task<Resultado<PresencaResposta>> EfetuarCheckOutAsync(EfetuarCheckOutSolicitacao request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Iniciando check-out para usuário {UsuarioId} no evento {EventoId}", request.UsuarioId, request.EventoId);

            try
            {
                var dataCheckOut = DateTime.Now;
                var evento = await eventoRepositorio.ObterEventoPorIdAsync(request.EventoId, cancellationToken);
                if (evento is null)
                {
                    logger.LogWarning("Evento {EventoId} não encontrado para check-out", request.EventoId);
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");
                }

                var resultadoDistancia = validadorDeCoordenadas.ValidarDistanciaParaEvento(
                    request.Latitude, 
                    request.Longitude, 
                    evento.Localizacao.Latitude, 
                    evento.Localizacao.Longitude);

                if (!resultadoDistancia.Sucesso)
                {
                    logger.LogWarning("Usuário {UsuarioId} fora do raio permitido para check-out no evento {EventoId}", request.UsuarioId, request.EventoId);
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.Validacao, resultadoDistancia.Erros.First());
                }

                var usuario = await usuarioRepositorio.ObterPorIdAsync(request.UsuarioId, cancellationToken);
                if (usuario is null)
                {
                    logger.LogWarning("Usuário {UsuarioId} não encontrado para check-out", request.UsuarioId);
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.NaoEncontrado, "Usuário não encontrado.");
                }

                var presenca = await presencaRepositorio.BuscarPorUsuarioEEventoAsync(request.EventoId, request.UsuarioId, cancellationToken);
                if (presenca is null || !presenca.CheckInValido)
                {
                    logger.LogWarning("Check-in ativo não encontrado para usuário {UsuarioId} no evento {EventoId}", request.UsuarioId, request.EventoId);
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.NaoEncontrado, "Nenhum check-in ativo encontrado para este usuário no evento.");
                }

                if (dataCheckOut < evento.DataInicio || dataCheckOut > evento.DataFim)
                    throw new EventoFinalizadoExcecao("Check-out");

                if (request.UsuarioId != presenca.UsuarioId)
                {
                    logger.LogWarning("Tentativa de check-out não autorizada: usuário {UsuarioId} tentou fazer check-out da presença {PresencaId}", request.UsuarioId, presenca.Id);
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.Validacao, "Usuário não autorizado a realizar check-out neste evento.");
                }

                presenca.RealizarCheckOut(dataCheckOut);
                await uow.CommitAsync(cancellationToken);

                logger.LogInformation("Check-out realizado com sucesso para usuário {UsuarioId} no evento {EventoId}", request.UsuarioId, request.EventoId);

                var dto = await mapeador.MapearPresencaAsync(presenca, cancellationToken);
                return Resultado<PresencaResposta>.Ok(dto);
            }
            catch (ExcecaoDeDominio ex)
            {
                logger.LogError(ex, "Erro de domínio ao efetuar check-out para usuário {UsuarioId} no evento {EventoId}", request.UsuarioId, request.EventoId);
                return Resultado<PresencaResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro inesperado ao efetuar check-out para usuário {UsuarioId} no evento {EventoId}", request.UsuarioId, request.EventoId);
                throw;
            }
        }

        public async Task<Resultado<PresencaResposta>> ObterPorIdAsync(int id, CancellationToken cancellationToken)
        {
            try
            {
                var presenca = await presencaRepositorio.ObterPresencaPorIdAsync(id, cancellationToken);
                if (presenca is null)
                {
                    logger.LogWarning("Presença {PresencaId} não encontrada", id);
                    return Resultado<PresencaResposta>.Falha(TipoDeErro.NaoEncontrado, "Registro de presença não encontrado.");
                }

                var dto = await mapeador.MapearPresencaAsync(presenca, cancellationToken);
                return Resultado<PresencaResposta>.Ok(dto);
            }
            catch (ExcecaoDeDominio ex)
            {
                logger.LogError(ex, "Erro ao obter presença {PresencaId}", id);
                return Resultado<PresencaResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }

        public async Task<Resultado<IEnumerable<PresencaResposta>>> ListarPorEventoAsync(int eventoId, CancellationToken cancellationToken)
        {
            try
            {
                var evento = await eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken);
                if (evento is null)
                {
                    logger.LogWarning("Evento {EventoId} não encontrado ao listar presenças", eventoId);
                    return Resultado<IEnumerable<PresencaResposta>>.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");
                }

                var presencas = await presencaRepositorio.ObterPresencasPorEventoAsync(eventoId, cancellationToken);
                if (!presencas.Any())
                {
                    logger.LogInformation("Nenhuma presença encontrada para o evento {EventoId}", eventoId);
                    return Resultado<IEnumerable<PresencaResposta>>.Falha(TipoDeErro.NaoEncontrado, "Nenhum registro de presença encontrado para este evento.");
                }

                var dtos = await mapeador.MapearPresencasAsync(presencas, cancellationToken);

                logger.LogInformation("Listadas {Count} presenças para o evento {EventoId}", dtos.Count(), eventoId);
                return Resultado<IEnumerable<PresencaResposta>>.Ok(dtos);
            }
            catch (ExcecaoDeDominio ex)
            {
                logger.LogError(ex, "Erro ao listar presenças do evento {EventoId}", eventoId);
                return Resultado<IEnumerable<PresencaResposta>>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }

        public async Task<Resultado<IEnumerable<PresencaResposta>>> ObterRelatorioAsync(CancellationToken cancellationToken = default)
        {
            logger.LogWarning("Método ObterRelatorioAsync foi chamado - considere usar paginação");

            try
            {
#pragma warning disable CS0618
                var presencas = await presencaRepositorio.ObterPresencas();
#pragma warning restore CS0618

                if (!presencas.Any())
                {
                    logger.LogInformation("Nenhuma presença encontrada no sistema");
                    return Resultado<IEnumerable<PresencaResposta>>.Falha(TipoDeErro.NaoEncontrado, "Nenhum registro de presença encontrado.");
                }

                var dtos = await mapeador.MapearPresencasAsync(presencas, cancellationToken);

                logger.LogInformation("Relatório gerado com {Count} presenças", dtos.Count());
                return Resultado<IEnumerable<PresencaResposta>>.Ok(dtos);
            }
            catch (ExcecaoDeDominio ex)
            {
                logger.LogError(ex, "Erro ao gerar relatório de presenças");
                return Resultado<IEnumerable<PresencaResposta>>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }

        public async Task<Resultado<StatusPresencaResposta>> ObterStatusPresencaEventoAsync(int eventoId, CancellationToken cancellationToken = default)
        {
            try
            {
                var usuarioAtual = await loggedUser.User();
                var eventoExiste = await eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken);

                if (eventoExiste is null)
                {
                    logger.LogWarning("Evento {EventoId} não encontrado ao obter status de presença", eventoId);
                    return Resultado<StatusPresencaResposta>.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");
                }

                var presenca = await presencaRepositorio.ObterStatusPresencaPorEvento(eventoId, usuarioAtual.Id, cancellationToken);

                if (presenca is null)
                {
                    logger.LogInformation("Nenhuma presença encontrada para usuário {UsuarioId} no evento {EventoId}", usuarioAtual.Id, eventoId);
                    return Resultado<StatusPresencaResposta>.Falha(TipoDeErro.NaoEncontrado, "Nenhum registro de presença encontrado para este usuário neste evento.");
                }

                var status = new StatusPresencaResposta(
                    CheckInEfetuado: presenca.CheckInValido,
                    CheckOutEfetuado: presenca.CheckOutValido
                );

                return Resultado<StatusPresencaResposta>.Ok(status);
            }
            catch (ExcecaoDeDominio ex)
            {
                logger.LogError(ex, "Erro ao obter status de presença do evento {EventoId}", eventoId);
                return Resultado<StatusPresencaResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }

        public async Task<Resultado<bool?>> ObterPresencaEventoEmAndamentoAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var usuarioAtual = await loggedUser.User();

                var eventoEmAndamento = await presencaRepositorio.ObterPresencaEventoEmAndamentoAsync(usuarioAtual.Id, cancellationToken);
                if (eventoEmAndamento is null)
                {
                    logger.LogInformation("Nenhum evento em andamento para usuário {UsuarioId}", usuarioAtual.Id);
                    return Resultado<bool?>.Falha(TipoDeErro.NaoEncontrado, "Nenhum evento em andamento encontrado.");
                }

                var evento = await eventoRepositorio.ObterEventoPorIdAsync(eventoEmAndamento.EventoId, cancellationToken);
                if (evento is null)
                {
                    logger.LogWarning("Evento {EventoId} não encontrado para presença em andamento", eventoEmAndamento.EventoId);
                    return Resultado<bool?>.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");
                }

                if (evento.DataFim < DateTime.Now)
                {
                    logger.LogInformation("Evento {EventoId} já finalizado", evento.Id);
                    return Resultado<bool?>.Falha(TipoDeErro.NaoEncontrado, "Evento já finalizado.");
                }

                return Resultado<bool?>.Ok(true);
            }
            catch (ExcecaoDeDominio ex)
            {
                logger.LogError(ex, "Erro ao obter presença em andamento");
                return Resultado<bool?>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }
    }
}
