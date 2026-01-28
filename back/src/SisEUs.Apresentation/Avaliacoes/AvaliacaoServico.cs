using Microsoft.Extensions.Logging;
using SisEUs.Application.Avaliacoes.Abstracoes;
using SisEUs.Application.Avaliacoes.DTOs.Respostas;
using SisEUs.Application.Avaliacoes.DTOs.Solicitacoes;
using SisEUs.Application.Comum.Resultados;
using SisEUs.Application.Comum.UoW;
using SisEUs.Domain.Comum.Excecoes;
using SisEUs.Domain.Comum.LoggedUser;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Enumeracoes;
using SisEUs.Domain.ContextoDeEvento.Interfaces;
using SisEUs.Domain.ContextoDeUsuario.Interfaces;

namespace SisEUs.Application.Avaliacoes
{
    public class AvaliacaoServico(
        IAvaliacaoRepositorio avaliacaoRepositorio,
        IApresentacaoRepositorio apresentacaoRepositorio,
        IEventoRepositorio eventoRepositorio,
        IUsuarioRepositorio usuarioRepositorio,
        ILoggedUser loggedUser,
        ILogger<AvaliacaoServico> logger,
        IUoW uow) : IAvaliacaoServico
    {
        public async Task<Resultado<AvaliacaoResposta>> IniciarAvaliacaoAsync(int apresentacaoId, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Iniciando avaliação para apresentação {ApresentacaoId}", apresentacaoId);

            try
            {
                var usuarioAtual = await loggedUser.User();

                var apresentacao = await apresentacaoRepositorio.ObterPorIdAsync(apresentacaoId, cancellationToken);
                if (apresentacao is null)
                {
                    logger.LogWarning("Apresentação {ApresentacaoId} não encontrada", apresentacaoId);
                    return Resultado<AvaliacaoResposta>.Falha(TipoDeErro.NaoEncontrado, "Apresentação não encontrada.");
                }

                var avaliacaoExistente = await avaliacaoRepositorio.ObterPorApresentacaoEAvaliadorAsync(apresentacaoId, usuarioAtual.Id, cancellationToken);
                if (avaliacaoExistente is not null)
                {
                    logger.LogWarning("Avaliador {AvaliadorId} já possui avaliação para apresentação {ApresentacaoId}", usuarioAtual.Id, apresentacaoId);
                    
                    var dtoExistente = MapearParaResposta(avaliacaoExistente);
                    return Resultado<AvaliacaoResposta>.Ok(dtoExistente);
                }

                var avaliacao = Avaliacao.Iniciar(apresentacaoId, usuarioAtual.Id);

                await avaliacaoRepositorio.AdicionarAsync(avaliacao, cancellationToken);
                await uow.CommitAsync(cancellationToken);

                avaliacao = await avaliacaoRepositorio.ObterPorIdAsync(avaliacao.Id, cancellationToken);

                logger.LogInformation("Avaliação {AvaliacaoId} iniciada com sucesso para apresentação {ApresentacaoId}", avaliacao!.Id, apresentacaoId);

                var dto = MapearParaResposta(avaliacao);
                return Resultado<AvaliacaoResposta>.Ok(dto);
            }
            catch (ExcecaoDeDominio ex)
            {
                logger.LogError(ex, "Erro de domínio ao iniciar avaliação para apresentação {ApresentacaoId}", apresentacaoId);
                return Resultado<AvaliacaoResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro inesperado ao iniciar avaliação para apresentação {ApresentacaoId}", apresentacaoId);
                throw;
            }
        }

        public async Task<Resultado<AvaliacaoResposta>> EnviarAvaliacaoAsync(int avaliacaoId, decimal nota, string? parecer, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Enviando avaliação {AvaliacaoId} com nota {Nota}", avaliacaoId, nota);

            try
            {
                var usuarioAtual = await loggedUser.User();

                var avaliacao = await avaliacaoRepositorio.ObterPorIdAsync(avaliacaoId, cancellationToken);
                if (avaliacao is null)
                {
                    logger.LogWarning("Avaliação {AvaliacaoId} não encontrada", avaliacaoId);
                    return Resultado<AvaliacaoResposta>.Falha(TipoDeErro.NaoEncontrado, "Avaliação não encontrada.");
                }

                // Verificar se o avaliador logado é o dono da avaliação
                if (avaliacao.AvaliadorId != usuarioAtual.Id)
                {
                    logger.LogWarning("Avaliador {AvaliadorId} não autorizado a enviar avaliação {AvaliacaoId}", usuarioAtual.Id, avaliacaoId);
                    return Resultado<AvaliacaoResposta>.Falha(TipoDeErro.AcessoNegado, "Você não tem permissão para enviar esta avaliação.");
                }

                // Aplicar a avaliação (o método Avaliar já valida se está concluída)
                avaliacao.Avaliar(nota, parecer ?? string.Empty);

                await uow.CommitAsync(cancellationToken);

                logger.LogInformation("Avaliação {AvaliacaoId} enviada com sucesso. Nota: {Nota}", avaliacaoId, nota);

                var dto = MapearParaResposta(avaliacao);
                return Resultado<AvaliacaoResposta>.Ok(dto);
            }
            catch (ExcecaoDeDominio ex)
            {
                logger.LogError(ex, "Erro de domínio ao enviar avaliação {AvaliacaoId}", avaliacaoId);
                return Resultado<AvaliacaoResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro inesperado ao enviar avaliação {AvaliacaoId}", avaliacaoId);
                throw;
            }
        }

        public async Task<Resultado<AvaliacaoResposta>> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                var avaliacao = await avaliacaoRepositorio.ObterPorIdAsync(id, cancellationToken);
                if (avaliacao is null)
                {
                    logger.LogWarning("Avaliação {AvaliacaoId} não encontrada", id);
                    return Resultado<AvaliacaoResposta>.Falha(TipoDeErro.NaoEncontrado, "Avaliação não encontrada.");
                }

                var dto = MapearParaResposta(avaliacao);
                return Resultado<AvaliacaoResposta>.Ok(dto);
            }
            catch (ExcecaoDeDominio ex)
            {
                logger.LogError(ex, "Erro ao obter avaliação {AvaliacaoId}", id);
                return Resultado<AvaliacaoResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }

        public async Task<Resultado<IEnumerable<AvaliacaoResposta>>> ListarPorApresentacaoAsync(int apresentacaoId, CancellationToken cancellationToken = default)
        {
            try
            {
                var apresentacao = await apresentacaoRepositorio.ObterPorIdAsync(apresentacaoId, cancellationToken);
                if (apresentacao is null)
                {
                    logger.LogWarning("Apresentação {ApresentacaoId} não encontrada", apresentacaoId);
                    return Resultado<IEnumerable<AvaliacaoResposta>>.Falha(TipoDeErro.NaoEncontrado, "Apresentação não encontrada.");
                }

                var avaliacoes = await avaliacaoRepositorio.ObterPorApresentacaoAsync(apresentacaoId, cancellationToken);

                var dtos = avaliacoes.Select(MapearParaResposta);

                logger.LogInformation("Listadas {Count} avaliações para apresentação {ApresentacaoId}", dtos.Count(), apresentacaoId);
                return Resultado<IEnumerable<AvaliacaoResposta>>.Ok(dtos);
            }
            catch (ExcecaoDeDominio ex)
            {
                logger.LogError(ex, "Erro ao listar avaliações da apresentação {ApresentacaoId}", apresentacaoId);
                return Resultado<IEnumerable<AvaliacaoResposta>>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }

        public async Task<Resultado<IEnumerable<AvaliacaoResposta>>> ListarMinhasAvaliacoesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var usuarioAtual = await loggedUser.User();

                var avaliacoes = await avaliacaoRepositorio.ObterPorAvaliadorAsync(usuarioAtual.Id, cancellationToken);

                var dtos = avaliacoes.Select(MapearParaResposta);

                logger.LogInformation("Listadas {Count} avaliações do avaliador {AvaliadorId}", dtos.Count(), usuarioAtual.Id);
                return Resultado<IEnumerable<AvaliacaoResposta>>.Ok(dtos);
            }
            catch (ExcecaoDeDominio ex)
            {
                logger.LogError(ex, "Erro ao listar avaliações do avaliador logado");
                return Resultado<IEnumerable<AvaliacaoResposta>>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }

        public async Task<Resultado<IEnumerable<AvaliacaoResposta>>> ListarPorEventoAsync(int eventoId, CancellationToken cancellationToken = default)
        {
            try
            {
                var evento = await eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken);
                if (evento is null)
                {
                    logger.LogWarning("Evento {EventoId} não encontrado", eventoId);
                    return Resultado<IEnumerable<AvaliacaoResposta>>.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");
                }

                var avaliacoes = await avaliacaoRepositorio.ObterPorEventoAsync(eventoId, cancellationToken);

                var dtos = avaliacoes.Select(MapearParaResposta);

                logger.LogInformation("Listadas {Count} avaliações para evento {EventoId}", dtos.Count(), eventoId);
                return Resultado<IEnumerable<AvaliacaoResposta>>.Ok(dtos);
            }
            catch (ExcecaoDeDominio ex)
            {
                logger.LogError(ex, "Erro ao listar avaliações do evento {EventoId}", eventoId);
                return Resultado<IEnumerable<AvaliacaoResposta>>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }

        public async Task<Resultado<RelatorioApresentacaoResposta>> ObterRelatorioApresentacaoAsync(int apresentacaoId, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Gerando relatório de notas e pareceres para apresentação {ApresentacaoId}", apresentacaoId);

            try
            {
                var apresentacao = await apresentacaoRepositorio.ObterPorIdAsync(apresentacaoId, cancellationToken);
                if (apresentacao is null)
                {
                    logger.LogWarning("Apresentação {ApresentacaoId} não encontrada", apresentacaoId);
                    return Resultado<RelatorioApresentacaoResposta>.Falha(TipoDeErro.NaoEncontrado, "Apresentação não encontrada.");
                }

                var avaliacoes = await avaliacaoRepositorio.ObterPorApresentacaoAsync(apresentacaoId, cancellationToken);
                var listaAvaliacoes = avaliacoes.ToList();

                // Obter nomes dos avaliadores
                var avaliadoresIds = listaAvaliacoes.Select(a => a.AvaliadorId).Distinct();
                var avaliadores = await usuarioRepositorio.ObterPorIdsAsync(avaliadoresIds, cancellationToken);
                var avaliadoresDict = avaliadores.ToDictionary(
                    u => u.Id, 
                    u => u.Nome != null ? $"{u.Nome.Nome} {u.Nome.Sobrenome}" : "Avaliador"
                );

                // Calcular estatísticas
                var avaliacoesConcluidas = listaAvaliacoes.Where(a => a.Estado == EEstadoAvaliacao.Concluido).ToList();
                var notasConcluidas = avaliacoesConcluidas.Where(a => a.Nota.HasValue).Select(a => a.Nota!.Value).ToList();

                var avaliacoesDetalhadas = listaAvaliacoes.Select(a => new AvaliacaoDetalhadaResposta(
                    AvaliacaoId: a.Id,
                    AvaliadorId: a.AvaliadorId,
                    NomeAvaliador: avaliadoresDict.GetValueOrDefault(a.AvaliadorId, "Avaliador"),
                    Nota: a.Nota,
                    Parecer: a.Parecer,
                    Estado: a.Estado.ToString(),
                    DataInicio: a.DataInicio,
                    DataConclusao: a.DataConclusao
                )).ToList();

                var relatorio = new RelatorioApresentacaoResposta(
                    ApresentacaoId: apresentacao.Id,
                    TituloApresentacao: apresentacao.Titulo?.Valor ?? string.Empty,
                    NomeAutor: apresentacao.NomeAutor,
                    NomeOrientador: apresentacao.NomeOrientador,
                    Modalidade: apresentacao.Modalidade,
                    TotalAvaliacoes: listaAvaliacoes.Count,
                    AvaliacoesConcluidas: avaliacoesConcluidas.Count,
                    AvaliacoesPendentes: listaAvaliacoes.Count - avaliacoesConcluidas.Count,
                    MediaNotas: notasConcluidas.Any() ? Math.Round(notasConcluidas.Average(), 2) : null,
                    MaiorNota: notasConcluidas.Any() ? notasConcluidas.Max() : null,
                    MenorNota: notasConcluidas.Any() ? notasConcluidas.Min() : null,
                    Avaliacoes: avaliacoesDetalhadas
                );

                logger.LogInformation("Relatório gerado para apresentação {ApresentacaoId} com {TotalAvaliacoes} avaliações", apresentacaoId, relatorio.TotalAvaliacoes);
                return Resultado<RelatorioApresentacaoResposta>.Ok(relatorio);
            }
            catch (ExcecaoDeDominio ex)
            {
                logger.LogError(ex, "Erro ao gerar relatório da apresentação {ApresentacaoId}", apresentacaoId);
                return Resultado<RelatorioApresentacaoResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro inesperado ao gerar relatório da apresentação {ApresentacaoId}", apresentacaoId);
                throw;
            }
        }

        public async Task<Resultado<RelatorioEventoResposta>> ObterRelatorioEventoAsync(int eventoId, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Gerando relatório consolidado de notas e pareceres para evento {EventoId}", eventoId);

            try
            {
                var evento = await eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken);
                if (evento is null)
                {
                    logger.LogWarning("Evento {EventoId} não encontrado", eventoId);
                    return Resultado<RelatorioEventoResposta>.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");
                }

                var apresentacoes = await apresentacaoRepositorio.ObterPorEventoIdAsync(eventoId, cancellationToken);
                var listaApresentacoes = apresentacoes.ToList();

                var avaliacoes = await avaliacaoRepositorio.ObterPorEventoAsync(eventoId, cancellationToken);
                var listaAvaliacoes = avaliacoes.ToList();

                var todasNotasConcluidas = listaAvaliacoes
                    .Where(a => a.Estado == EEstadoAvaliacao.Concluido && a.Nota.HasValue)
                    .Select(a => a.Nota!.Value)
                    .ToList();

                var resumosApresentacoes = new List<ResumoApresentacaoResposta>();

                foreach (var apresentacao in listaApresentacoes)
                {
                    var avaliacoesDaApresentacao = listaAvaliacoes.Where(a => a.ApresentacaoId == apresentacao.Id).ToList();
                    var avaliacoesConcluidas = avaliacoesDaApresentacao.Where(a => a.Estado == EEstadoAvaliacao.Concluido).ToList();
                    var notasConcluidas = avaliacoesConcluidas.Where(a => a.Nota.HasValue).Select(a => a.Nota!.Value).ToList();

                    var media = notasConcluidas.Any() ? Math.Round(notasConcluidas.Average(), 2) : (decimal?)null;

                    var resumo = new ResumoApresentacaoResposta(
                        ApresentacaoId: apresentacao.Id,
                        TituloApresentacao: apresentacao.Titulo?.Valor ?? string.Empty,
                        NomeAutor: apresentacao.NomeAutor,
                        Modalidade: apresentacao.Modalidade,
                        TotalAvaliacoes: avaliacoesDaApresentacao.Count,
                        AvaliacoesConcluidas: avaliacoesConcluidas.Count,
                        MediaNotas: media,
                        MaiorNota: notasConcluidas.Any() ? notasConcluidas.Max() : null,
                        MenorNota: notasConcluidas.Any() ? notasConcluidas.Min() : null,
                        Classificacao: ObterClassificacao(media)
                    );

                    resumosApresentacoes.Add(resumo);
                }

                // Ordenar por média de notas (decrescente)
                resumosApresentacoes = resumosApresentacoes
                    .OrderByDescending(r => r.MediaNotas ?? 0)
                    .ToList();

                var relatorio = new RelatorioEventoResposta(
                    EventoId: evento.Id,
                    TituloEvento: evento.Titulo?.Valor ?? string.Empty,
                    DataInicio: evento.DataInicio,
                    DataFim: evento.DataFim,
                    TotalApresentacoes: listaApresentacoes.Count,
                    TotalAvaliacoes: listaAvaliacoes.Count,
                    AvaliacoesConcluidas: listaAvaliacoes.Count(a => a.Estado == EEstadoAvaliacao.Concluido),
                    MediaGeralNotas: todasNotasConcluidas.Any() ? Math.Round(todasNotasConcluidas.Average(), 2) : null,
                    Apresentacoes: resumosApresentacoes
                );

                logger.LogInformation("Relatório gerado para evento {EventoId} com {TotalApresentacoes} apresentações e {TotalAvaliacoes} avaliações",
                    eventoId, relatorio.TotalApresentacoes, relatorio.TotalAvaliacoes);
                return Resultado<RelatorioEventoResposta>.Ok(relatorio);
            }
            catch (ExcecaoDeDominio ex)
            {
                logger.LogError(ex, "Erro ao gerar relatório do evento {EventoId}", eventoId);
                return Resultado<RelatorioEventoResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Erro inesperado ao gerar relatório do evento {EventoId}", eventoId);
                throw;
            }
        }

        private static string? ObterClassificacao(decimal? media)
        {
            if (!media.HasValue) return null;

            return media.Value switch
            {
                >= 9 => "Excelente",
                >= 7 => "Bom",
                >= 5 => "Regular",
                >= 3 => "Insuficiente",
                _ => "Muito Insuficiente"
            };
        }

        private static AvaliacaoResposta MapearParaResposta(Avaliacao avaliacao)
        {
            return new AvaliacaoResposta(
                Id: avaliacao.Id,
                ApresentacaoId: avaliacao.ApresentacaoId,
                TituloApresentacao: avaliacao.Apresentacao?.Titulo?.Valor ?? string.Empty,
                NomeAutor: avaliacao.Apresentacao?.NomeAutor ?? string.Empty,
                Modalidade: avaliacao.Apresentacao?.Modalidade ?? default,
                AvaliadorId: avaliacao.AvaliadorId,
                Nota: avaliacao.Nota,
                Parecer: avaliacao.Parecer,
                Estado: avaliacao.Estado.ToString(),
                DataInicio: avaliacao.DataInicio,
                DataConclusao: avaliacao.DataConclusao
            );
        }

        public Task<Resultado<IEnumerable<IniciarAvaliacaoSolicitacao>>> ListarAvaliacoesPendentesAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
