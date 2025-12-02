using SisEUs.Application.Apresentacoes.Abstractions;
using SisEUs.Application.Apresentacoes.DTOs.Respostas;
using SisEUs.Application.Apresentacoes.DTOs.Solicitacoes;
using SisEUs.Application.Apresentacoes.Mappers;
using SisEUs.Application.Comum.Resultados;
using SisEUs.Application.Comum.UoW;
using SisEUs.Domain.Comum.Excecoes;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Interfaces;
using SisEUs.Domain.ContextoDeEvento.ObjetosDeValor;
using SisEUs.Domain.ContextoDeUsuario.Interfaces;
using SisEUs.Domain.ContextoDeUsuario.ObjetosDeValor;

namespace SisEUs.Application.Apresentacoes
{
    public class ApresentacaoServico : IApresentacaoServico
    {
        private readonly IEventoRepositorio _eventoRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IApresentacaoRepositorio _apresentacaoRepositorio;
        private readonly IUoW _uow;

        public ApresentacaoServico(IEventoRepositorio eventoRepositorio, IUsuarioRepositorio usuarioRepositorio, IApresentacaoRepositorio apresentacaoRepositorio, IUoW uow)
        {
            _eventoRepositorio = eventoRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _apresentacaoRepositorio = apresentacaoRepositorio;
            _uow = uow;
        }

        public async Task<Resultado<ApresentacaoResposta>> CriarApresentacaoAsync(CriarApresentacaoSolicitacao request, CancellationToken cancellationToken)
        {
            try
            {
                var titulo = Titulo.Criar(request.Titulo);
                var nomeAutor = request.NomeAutor;
                var nomeOrientador = request.NomeOrientador;

                var novaApresentacao = Apresentacao.Criar(
                    request.EventoId,
                    titulo,
                    nomeAutor,
                    nomeOrientador
                );

                await _apresentacaoRepositorio.AdicionarAsync(novaApresentacao, cancellationToken);

                await _uow.CommitAsync(cancellationToken);

                var dto = await novaApresentacao.ToResponseDtoAsync(_eventoRepositorio, _usuarioRepositorio, cancellationToken);

                return Resultado<ApresentacaoResposta>.Ok(dto);
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado<ApresentacaoResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }

        public async Task<Resultado<ApresentacaoResposta>> ObterApresentacaoPorIdAsync(int apresentacaoId, CancellationToken cancellationToken)
        {
            var apresentacao = await _apresentacaoRepositorio.ObterPorIdAsync(apresentacaoId, cancellationToken);
            if (apresentacao is null)
                return Resultado<ApresentacaoResposta>.Falha(TipoDeErro.NaoEncontrado, "Apresentação não encontrada.");

            var dto = await apresentacao.ToResponseDtoAsync(_eventoRepositorio, _usuarioRepositorio, cancellationToken);
            return Resultado<ApresentacaoResposta>.Ok(dto);
        }

        public async Task<Resultado<IEnumerable<ApresentacaoResposta>>> ObterApresentacoesPorEventoAsync(int eventoId, CancellationToken cancellationToken)
        {
            var eventoExiste = await _eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken) is not null;
            if (!eventoExiste)
                return Resultado<IEnumerable<ApresentacaoResposta>>.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");

            var apresentacoes = await _apresentacaoRepositorio.ObterPorEventoIdAsync(eventoId, cancellationToken);

            var dtosTasks = apresentacoes.Select(ap => ap.ToResponseDtoAsync(_eventoRepositorio, _usuarioRepositorio, cancellationToken));
            var dtos = await Task.WhenAll(dtosTasks);

            return Resultado<IEnumerable<ApresentacaoResposta>>.Ok(dtos);
        }

        public async Task<Resultado> ExcluirApresentacaoAsync(int apresentacaoId, CancellationToken cancellationToken)
        {
            var evento = await _eventoRepositorio.ObterEventoPorIdAsync(apresentacaoId, cancellationToken);
            if (evento is null)
                return Resultado.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado para a apresentação informada.");

            var apresentacao = await _apresentacaoRepositorio.ObterPorIdAsync(apresentacaoId, cancellationToken);
            if (apresentacao is null)
                return Resultado.Falha(TipoDeErro.NaoEncontrado, "Apresentação não encontrada.");

            _apresentacaoRepositorio.Remover(apresentacao);

            await _uow.CommitAsync(cancellationToken);


            return Resultado.Ok();
        }

        public async Task<Resultado> AtualizarApresentacaoAsync(int apresentacaoId, AtualizarApresentacaoSolicitacao request, CancellationToken cancellationToken)
        {
            try
            {
                var apresentacao = await _apresentacaoRepositorio.ObterPorIdAsync(apresentacaoId, cancellationToken);
                if (apresentacao is null)
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, "Apresentação não encontrada.");

                var titulo = Titulo.Criar(request.Titulo);
                var nomeAutor = request.NomeAutor;
                var nomeOrientador = request.NomeOrientador;
                apresentacao.Atualizar(titulo, nomeAutor, nomeOrientador);

                await _uow.CommitAsync(cancellationToken);
                return Resultado.Ok();
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }
    }
}
