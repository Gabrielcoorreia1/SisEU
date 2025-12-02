using SisEUs.Application.Apresentacoes.Abstractions;
using SisEUs.Application.Apresentacoes.DTOs.Respostas;
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
using SisEUs.Domain.ContextoDeUsuario.Interfaces;
using SisEUs.Domain.ContextoDeUsuario.ObjetosDeValor;
using System;
using System.Linq;
using System.Collections.Generic;
using SisEUs.Application.Apresentacoes.DTOs.Solicitacoes;

namespace SisEUs.Application.Eventos
{
    public class EventoServico : IEventoServico
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IEventoRepositorio _eventoRepositorio;
        public readonly IApresentacaoServico _servico;
        private readonly IUoW _uow;

        public EventoServico(IEventoRepositorio eventoRepositorio, IUsuarioRepositorio usuarioRepositorio, IUoW uow, IApresentacaoServico servico)
        {
            _eventoRepositorio = eventoRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
            _uow = uow;
            _servico = servico;
        }

        public async Task<Resultado<EventoResposta>> CriarEventoAsync(CriarEventoSolicitacao request, CancellationToken cancellationToken)
        {
            List<Localizacao> localizacoesUFC = Localizacao.ObterLocalizacoesUFC();
            Localizacao? localizacaoDoEvento = (request.Local.Campus == Campus.Fortaleza) ? localizacoesUFC[0] : localizacoesUFC[1];

            try
            {
                var LocalizacaoVO = Localizacao.Criar(
                    localizacaoDoEvento.Latitude,
                    localizacaoDoEvento.Longitude
                );
                var local = Local.Criar(request.Local.Campus.ToString(), request.Local.Departamento, request.Local.Bloco, request.Local.Sala);
                var titulo = Titulo.Criar(request.Titulo);
                
                string avaliadoresConcatenados = string.Join(";", request.Avaliadores ?? new List<string>());

                var evento = Evento.Criar(
                    titulo,
                    request.DataInicio,
                    request.DataFim,
                    local,
                    new List<int>(),
                    avaliadoresConcatenados,
                    LocalizacaoVO,
                    request.ImgUrl,
                    request.CodigoUnico,
                    request.ETipoEvento
                );

                Random random = new Random();
                string pin = random.Next(100000, 1000000).ToString("D6"); 

                evento.DefinirPinCheckin(pin);
                await _eventoRepositorio.CriarEventoAsync(evento, cancellationToken);

                await _uow.CommitAsync(cancellationToken);

                var apresentacoes = request.Apresentacoes ?? new List<CriarApresentacaoSolicitacao>();

                if (apresentacoes.Any())
                {
                    foreach (var apresentacao in apresentacoes)
                    {
                        apresentacao.EventoId = evento.Id;
                        var novaApresentacao = await _servico.CriarApresentacaoAsync(apresentacao, cancellationToken);
                        if (!novaApresentacao.Sucesso)
                            return Resultado<EventoResposta>.Falha(TipoDeErro.Inesperado, "Não foi possivel criar nova apresentação para o evento");
                    }
                }

                var eventoDto = await evento.ToResponseDtoAsync(_usuarioRepositorio, cancellationToken);

                return Resultado<EventoResposta>.Ok(eventoDto);
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado<EventoResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }
        
        public async Task<Resultado> AtualizarEventoAsync(int id, AtualizarEventoSolicitacao request, CancellationToken cancellationToken)
        {
            try
            {
                if (id != request.Id)
                    return Resultado.Falha(TipoDeErro.Validacao, "O ID do evento não corresponde ao ID fornecido na solicitação.");

                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(request.Id, cancellationToken);
                if (evento is null)
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");

                evento.AtualizarLocal(
                    bloco: request.Local.Bloco,
                    campus: request.Local.Campus.ToString(),
                    departamento: request.Local.Departamento,
                    sala: request.Local.Sala);

                string avaliadoresConcatenados = string.Join(";", request.Avaliadores ?? new List<string>());

                evento.AtualizarTitulo(request.Titulo);
                evento.AtualizarDataInicio(request.DataInicio);
                evento.AtualizarDataFim(request.DataFim);
                evento.AtualizarImg(request.ImgUrl);
                evento.AtualizarCodigoUnico(request.CodigoUnico);
                evento.AtualizarTipoEvento(request.ETipoEvento);
                evento.AtualizarAvaliadores(avaliadoresConcatenados);

                await _uow.CommitAsync(cancellationToken);
                return Resultado.Ok();
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }
        
        public async Task<Resultado> ExcluirEventoAsync(int eventoId, CancellationToken cancellationToken)
        {
            try
            {
                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken);
                if (evento is null)
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");

                _eventoRepositorio.ExcluirEvento(evento);

                await _uow.CommitAsync(cancellationToken);
                return Resultado.Ok();
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado.Falha(TipoDeErro.Conflito, ex.Message);
            }
        }

        public async Task<Resultado<EventoResposta>> ObterEventoPorIdAsync(int eventoId, CancellationToken cancellationToken)
        {
            try
            {
                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken);
                if (evento is null)
                    return Resultado<EventoResposta>.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");

                var eventoDto = await evento.ToResponseDtoAsync(_usuarioRepositorio, cancellationToken);

                Resultado<IEnumerable<ApresentacaoResposta>> resultado = await _servico.ObterApresentacoesPorEventoAsync(evento.Id, cancellationToken);
                List<ApresentacaoResposta> apresentacaoRespostas = resultado.Valor.ToList();
                eventoDto.Apresentacoes = apresentacaoRespostas;

                return Resultado<EventoResposta>.Ok(eventoDto);
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado<EventoResposta>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }

        public async Task<Resultado<IEnumerable<EventoResposta>>> ListarEventosAsync(int pagina, int tamanho, CancellationToken cancellationToken)
        {
            try
            {
                var eventos = await _eventoRepositorio.ObterEventosPaginadosAsync(pagina, tamanho, cancellationToken);

                var dtos = new List<EventoResposta>();

                foreach (var evento in eventos)
                {
                    var eventoDto = await evento.ToResponseDtoAsync(_usuarioRepositorio, cancellationToken);
                    dtos.Add(eventoDto);
                }

                return Resultado<IEnumerable<EventoResposta>>.Ok(dtos);
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado<IEnumerable<EventoResposta>>.Falha(TipoDeErro.Validacao, ex.Message);
            }
        }

        public async Task<Resultado> AdicionarParticipanteAsync(int participanteId, int eventoId, CancellationToken cancellationToken)
        {
            try
            {
                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken);
                if (evento is null)
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");

                evento.AdicionarParticipante(participanteId);

                await _uow.CommitAsync(cancellationToken);
                return Resultado.Ok();
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado.Falha(TipoDeErro.Conflito, ex.Message);
            }
        }

        public async Task<Resultado> RemoverParticipanteAsync(int participanteId, int eventoId, CancellationToken cancellationToken)
        {
            try
            {
                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken);
                if (evento is null)
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");

                evento.RemoverParticipante(participanteId);

                await _uow.CommitAsync(cancellationToken);
                return Resultado.Ok();
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado.Falha(TipoDeErro.NaoEncontrado, ex.Message);
            }
        }
        public async Task<Resultado> AdicionarAvaliadorAsync(string avaliadorNome, string avaliadorSobrenome, int eventoId, CancellationToken cancellationToken)
        {
            try
            {
                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken);
                if (evento is null)
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");

                var nome = NomeCompleto.Criar(avaliadorNome, avaliadorSobrenome);

                await _uow.CommitAsync(cancellationToken);
                return Resultado.Ok();
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado.Falha(TipoDeErro.Conflito, ex.Message);
            }
        }
        public async Task<Resultado> RemoverAvaliadorAsync(string avaliadorNome, string avaliadorSobrenome, int eventoId, CancellationToken cancellationToken)
        {
            try
            {
                var evento = await _eventoRepositorio.ObterEventoPorIdAsync(eventoId, cancellationToken);
                if (evento is null)
                    return Resultado.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");

                var nome = NomeCompleto.Criar(avaliadorNome, avaliadorSobrenome);

                await _uow.CommitAsync(cancellationToken);
                return Resultado.Ok();
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado.Falha(TipoDeErro.Conflito, ex.Message);
            }
        }

        public async Task<Resultado<EventoResposta>> ObterPorCodigoEvento(string codigoEvento, CancellationToken cancellationToken)
        {
            try
            {
                var evento = await _eventoRepositorio.ObterEventoPorCodigoAsync(codigoEvento, cancellationToken);
                if (evento is null)
                    return Resultado<EventoResposta>.Falha(TipoDeErro.NaoEncontrado, "Evento não encontrado.");

                var eventoDto = await evento.ToResponseDtoAsync(_usuarioRepositorio, cancellationToken);

                return Resultado<EventoResposta>.Ok(eventoDto);
            }
            catch (ExcecaoDeDominio ex)
            {
                return Resultado<EventoResposta>.Falha(TipoDeErro.Conflito, ex.Message);
            }
        }
    }
}