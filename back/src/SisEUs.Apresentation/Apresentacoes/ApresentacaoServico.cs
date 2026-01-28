using SisEUs.Application.Apresentacoes.Abstractions;
using SisEUs.Application.Apresentacoes.DTOs.Respostas;
using SisEUs.Application.Apresentacoes.DTOs.Solicitacoes;
using SisEUs.Application.Comum.Mapeamento;
using SisEUs.Application.Comum.Resultados;
using SisEUs.Application.Comum.UoW;
using SisEUs.Domain.Comum.Excecoes;
using SisEUs.Domain.Comum.LoggedUser;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Interfaces;
using SisEUs.Domain.ContextoDeEvento.ObjetosDeValor;
using SisEUs.Domain.ContextoDeUsuario.Interfaces;
using SisEUs.Domain.ContextoDeUsuario.ObjetosDeValor;
using Microsoft.Extensions.Logging;

namespace SisEUs.Application.Apresentacoes
{
    public class ApresentacaoServico(
        IEventoRepositorio eventoRepositorio,
        IApresentacaoRepositorio apresentacaoRepositorio,
        IUsuarioRepositorio usuarioRepositorio,
        ILogger<ApresentacaoServico> logger,
        IUoW uow,
        IMapeadorDeEntidades mapeador,
        ILoggedUser loggedUser) : IApresentacaoServico
    {
        public async Task<Resultado<ApresentacaoResposta>> CriarApresentacaoAsync(CriarApresentacaoSolicitacao request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Criando apresentação: {Titulo} para evento {EventoId}", request.Titulo, request.EventoId);

            var resultadoConstrucao = await AdicionarAoContextoAsync(request, cancellationToken);

            if (!resultadoConstrucao.Sucesso)
            {
                var mensagemErro = resultadoConstrucao.Erros.FirstOrDefault() ?? "Erro desconhecido";
                logger.LogWarning("Falha ao criar apresentação: {Titulo}. Erro: {Erro}", request.Titulo, mensagemErro);
                return Resultado<ApresentacaoResposta>.Falha(resultadoConstrucao.TipoDeErro!.Value, mensagemErro);
            }

            await uow.CommitAsync(cancellationToken);

            var novaApresentacao = resultadoConstrucao.Valor;
            var dto = await mapeador.MapearApresentacaoAsync(novaApresentacao, cancellationToken);

            logger.LogInformation("Apresentação {ApresentacaoId} criada com sucesso", novaApresentacao.Id);
            return Resultado<ApresentacaoResposta>.Ok(dto);
        }

        public async Task<Resultado<Apresentacao>> AdicionarAoContextoAsync(CriarApresentacaoSolicitacao request, CancellationToken cancellationToken)
        {
            try
            {
                // Buscar autor por CPF
                var cpfAutor = Cpf.Criar(request.CpfAutor);
                var autor = await usuarioRepositorio.ObterPorCpfAsync(cpfAutor, cancellationToken);
                if (autor is null)
                {
                    logger.LogWarning("Autor com CPF {CpfAutor} não encontrado", request.CpfAutor);
                    return Resultado<Apresentacao>.Falha(TipoDeErro.NaoEncontrado, $"Autor com CPF '{request.CpfAutor}' não encontrado.");
                }

                // Buscar orientador por CPF
                var cpfOrientador = Cpf.Criar(request.CpfOrientador);
                var orientador = await usuarioRepositorio.ObterPorCpfAsync(cpfOrientador, cancellationToken);
                if (orientador is null)
                {
                    logger.LogWarning("Orientador com CPF {CpfOrientador} não encontrado", request.CpfOrientador);
                    return Resultado<Apresentacao>.Falha(TipoDeErro.NaoEncontrado, $"Orientador com CPF '{request.CpfOrientador}' não encontrado.");
                }

                var titulo = Titulo.Criar(request.Titulo);

                var novaApresentacao = Apresentacao.Criar(
                    request.EventoId,
                    titulo,
                    autor.Id,
                    orientador.Id,
                    request.Modalidade
                );

                await apresentacaoRepositorio.AdicionarAsync(novaApresentacao, cancellationToken);

                return Resultado<Apresentacao>.Ok(novaApresentacao);
            }
            catch (ExcecaoDeDominio ex)
            {
                logger.LogError(ex, "Erro de domínio ao adicionar apresentação ao contexto: {Titulo}", request.Titulo);
                return Resultado<Apresentacao>.Falha(TipoDeErro.Validacao, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro inesperado ao adicionar apresentação ao contexto: {Titulo}", request.Titulo);
                throw;
            }
        }

        public async Task<Resultado<ApresentacaoResposta>> ObterApresentacaoPorIdAsync(int apresentacaoId, CancellationToken cancellationToken)
        {
            logger.LogInformation("Obtendo apresentação {ApresentacaoId}", apresentacaoId);

            try
            {
                var apresentacao = await apresentacaoRepositorio.ObterPorIdAsync(apresentacaoId, cancellationToken);
                if (apresentacao is null)
                {
                    logger.LogWarning("Apresentação {ApresentacaoId} não encontrada", apresentacaoId);
                    return Resultado<ApresentacaoResposta>.Falha(TipoDeErro.NaoEncontrado, "Apresentação não encontrada.");
                }

                var dto = await mapeador.MapearApresentacaoAsync(apresentacao, cancellationToken);
                return Resultado<ApresentacaoResposta>.Ok(dto);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao obter apresentação {ApresentacaoId}", apresentacaoId);
                throw;
            }
        }

        public async Task<Resultado<IEnumerable<ApresentacaoResposta>>> ObterApresentacoesPorEventoAsync(int eventoId, CancellationToken cancellationToken)
        {
            logger.LogInformation("Obtendo apresentações do evento {EventoId}", eventoId);

            try
            {
                var eventoExiste = await eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken) is not null;
                if (!eventoExiste)
                {
                    logger.LogWarning("Evento {EventoId} não encontrado ao buscar apresentações", eventoId);
                    return Resultado<IEnumerable<ApresentacaoResposta>>.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");
                }

                var apresentacoes = await apresentacaoRepositorio.ObterPorEventoIdAsync(eventoId, cancellationToken);

                if (!apresentacoes.Any())
                {
                    logger.LogInformation("Nenhuma apresentação encontrada para o evento {EventoId}", eventoId);
                    return Resultado<IEnumerable<ApresentacaoResposta>>.Ok(Enumerable.Empty<ApresentacaoResposta>());
                }

                var dtos = new List<ApresentacaoResposta>();
                foreach (var apresentacao in apresentacoes)
                {
                    var dto = await mapeador.MapearApresentacaoAsync(apresentacao, cancellationToken);
                    dtos.Add(dto);
                }

                logger.LogInformation("Encontradas {Count} apresentações para o evento {EventoId}", dtos.Count, eventoId);
                return Resultado<IEnumerable<ApresentacaoResposta>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao obter apresentações do evento {EventoId}", eventoId);
                throw;
            }
        }

        public async Task<Resultado> ExcluirApresentacaoAsync(int apresentacaoId, CancellationToken cancellationToken)
        {
            logger.LogInformation("Excluindo apresentação {ApresentacaoId}", apresentacaoId);

            try
            {
                var apresentacao = await apresentacaoRepositorio.ObterPorIdAsync(apresentacaoId, cancellationToken);
                if (apresentacao is null)
                {
                    logger.LogWarning("Apresentação {ApresentacaoId} não encontrada para exclusão", apresentacaoId);
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, "Apresentação não encontrada.");
                }

                apresentacaoRepositorio.Remover(apresentacao);
                await uow.CommitAsync(cancellationToken);

                logger.LogInformation("Apresentação {ApresentacaoId} excluída com sucesso", apresentacaoId);
                return Resultado.Ok();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao excluir apresentação {ApresentacaoId}", apresentacaoId);
                throw;
            }
        }

        public async Task<Resultado> AtualizarApresentacaoAsync(int apresentacaoId, AtualizarApresentacaoSolicitacao request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Atualizando apresentação {ApresentacaoId}", apresentacaoId);

            try
            {
                var apresentacao = await apresentacaoRepositorio.ObterPorIdAsync(apresentacaoId, cancellationToken);
                if (apresentacao is null)
                {
                    logger.LogWarning("Apresentação {ApresentacaoId} não encontrada para atualização", apresentacaoId);
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, "Apresentação não encontrada.");
                }

                // Buscar autor por CPF
                var cpfAutor = Cpf.Criar(request.CpfAutor);
                var autor = await usuarioRepositorio.ObterPorCpfAsync(cpfAutor, cancellationToken);
                if (autor is null)
                {
                    logger.LogWarning("Autor com CPF {CpfAutor} não encontrado", request.CpfAutor);
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, $"Autor com CPF '{request.CpfAutor}' não encontrado.");
                }

                // Buscar orientador por CPF
                var cpfOrientador = Cpf.Criar(request.CpfOrientador);
                var orientador = await usuarioRepositorio.ObterPorCpfAsync(cpfOrientador, cancellationToken);
                if (orientador is null)
                {
                    logger.LogWarning("Orientador com CPF {CpfOrientador} não encontrado", request.CpfOrientador);
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, $"Orientador com CPF '{request.CpfOrientador}' não encontrado.");
                }

                var titulo = Titulo.Criar(request.Titulo);
                apresentacao.Atualizar(titulo, autor.Id, orientador.Id);

                await uow.CommitAsync(cancellationToken);
                
                logger.LogInformation("Apresentação {ApresentacaoId} atualizada com sucesso", apresentacaoId);
                return Resultado.Ok();
            }
            catch (ExcecaoDeDominio ex)
            {
                logger.LogError(ex, "Erro de domínio ao atualizar apresentação {ApresentacaoId}", apresentacaoId);
                return Resultado.Falha(TipoDeErro.Validacao, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro inesperado ao atualizar apresentação {ApresentacaoId}", apresentacaoId);
                throw;
            }
        }

        public async Task<Resultado<IEnumerable<ApresentacaoResposta>>> ObterMinhasApresentacoesAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Obtendo apresentações do usuário logado");

            try
            {
                var usuarioLogado = await loggedUser.User();

                var apresentacoes = await apresentacaoRepositorio.ObterPorAutorIdAsync(usuarioLogado.Id, cancellationToken);

                if (!apresentacoes.Any())
                {
                    logger.LogInformation("Nenhuma apresentação encontrada para o autor {AutorId}", usuarioLogado.Id);
                    return Resultado<IEnumerable<ApresentacaoResposta>>.Ok(Enumerable.Empty<ApresentacaoResposta>());
                }

                var dtos = new List<ApresentacaoResposta>();
                foreach (var apresentacao in apresentacoes)
                {
                    var dto = await mapeador.MapearApresentacaoAsync(apresentacao, cancellationToken);
                    dtos.Add(dto);
                }

                logger.LogInformation("Encontradas {Count} apresentações para o autor {AutorId}", dtos.Count, usuarioLogado.Id);
                return Resultado<IEnumerable<ApresentacaoResposta>>.Ok(dtos);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro ao obter apresentações do autor logado");
                throw;
            }
        }
    }
}
