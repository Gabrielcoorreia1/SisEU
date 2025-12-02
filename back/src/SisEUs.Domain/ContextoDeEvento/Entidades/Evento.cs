using SisEUs.Domain.Comum.Sementes;
using SisEUs.Domain.ContextoDeEvento.Enumeracoes;
using SisEUs.Domain.ContextoDeEvento.Excecoes;
using SisEUs.Domain.ContextoDeEvento.ObjetosDeValor;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq; // Adicionado para .Linq

namespace SisEUs.Domain.ContextoDeEvento.Entidades
{
    /// <summary>
    /// Representa um evento no sistema, contendo suas informações, datas e participantes.
    /// É a raiz do agregado de Evento.
    /// </summary>
    /// <remarks>
    /// A criação de instâncias desta classe deve ser feita através do método de fábrica estático <see cref="Criar"/>.
    /// </remarks>
    public class Evento : Entidade
    {
        /// <summary>
        /// Construtor privado para garantir que a entidade seja criada através do método de fábrica.
        /// </summary>
        private Evento(
            Titulo titulo,
            DateTime dataInicio,
            DateTime dataFim, 
            Local local, 
            IEnumerable<int> participantes, 
            string avaliadores,  
            Localizacao localizacao, 
            string imgUrl, 
            string codigoUnico, // <-- CORREÇÃO: Removido 'string' duplicado
            ETipoEvento eTipoEvento)
        {
            Titulo = titulo;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Local = local;
            Localizacao = localizacao;
            Participantes = participantes?.Distinct().ToList() ?? new List<int>();
            Avaliadores = avaliadores;
            ImgUrl = imgUrl;
            CodigoUnico = codigoUnico;
            TipoEvento = eTipoEvento;
        }
        /// <summary>
        /// Construtor privado sem parâmetros, exigido pelo Entity Framework Core para materialização de objetos.
        /// </summary>
        private Evento() { }
        [MaxLength(10)]
        public string? PinCheckin { get; private set; }
        public Titulo Titulo { get; private set; } = null!;
        public DateTime DataInicio { get; private set; }
        public DateTime DataFim { get; private set; }
        public Local Local { get; private set; } = null!;
        public Localizacao Localizacao { get; private set; } = null!;
        public ETipoEvento TipoEvento { get; set; } = ETipoEvento.Nenhum;
        public string Avaliadores { get; private set; } = null!;
        public List<int> Participantes { get; private set; } = new List<int>();
        public string ImgUrl { get; set; } = string.Empty;
        public string CodigoUnico { get; set; } = null!;
        public ICollection<Apresentacao> Apresentacoes { get; private set; } = new List<Apresentacao>();
        public void DefinirPinCheckin(string pin)
        {
            PinCheckin = pin;
        }

        /// <summary>
        /// Método de fábrica para criar uma nova instância de Evento.
        /// </summary>
        public static Evento Criar(
            Titulo titulo, 
            DateTime dataInicio, 
            DateTime dataFim, 
            Local local, 
            IEnumerable<int> participantes, 
            string avaliadores,
            Localizacao localizacao, 
            string imgUrl, 
            string codigoUnico, // <-- CORREÇÃO: Removido 'string' duplicado
            ETipoEvento eTipoEvento)
        {
            if (dataInicio >= dataFim)
                throw new DataInvalidaExcecao();


            return new Evento(titulo, dataInicio, dataFim, local, participantes, avaliadores, localizacao, imgUrl, codigoUnico, eTipoEvento);
        }

        /// <summary>
        /// Adiciona um novo participante à lista do evento.
        /// </summary>
        public void AdicionarParticipante(int participanteId)
        {
            if (Participantes.Contains(participanteId))
                throw new ParticipanteJaAdicionadoExcecao(participanteId);

            Participantes.Add(participanteId);
        }

        /// <summary>
        /// Remove um participante da lista do evento.
        /// </summary>
        public void RemoverParticipante(int usuarioId)
        {
            bool removido = Participantes.Remove(usuarioId);
            if (!removido)
                throw new ParticipanteNaoEncontradoExcecao();
        }
        /// <summary>
        /// Atualiza a data de início do evento, se o evento ainda não tiver começado.
        /// </summary>
        public void AtualizarDataInicio(DateTime novaDataInicio)
        {
            /*if (this.DataInicio < DateTime.Now)
                throw new EventoJaComecouExcecao();

            if (novaDataInicio >= this.DataFim)
                throw new DataInvalidaExcecao();
            */

            this.DataInicio = novaDataInicio;
        }

        /// <summary>
        /// Atualiza a data de fim do evento.
        /// </summary>
        public void AtualizarDataFim(DateTime novaDataFim)
        {
            /*if (novaDataFim <= this.DataInicio)
                throw new DataInvalidaExcecao();
                */

            this.DataFim = novaDataFim;
        }

        /// <summary>
        /// Atualiza o título do evento.
        /// </summary>
        public void AtualizarTitulo(string novoTitulo) => Titulo = Titulo.Criar(novoTitulo);

        /// <summary>
        /// Atualiza o local do evento, se o evento ainda não tiver começado.
        /// </summary>
        public void AtualizarLocal(string campus, string departamento, string bloco, string sala)
        {
            var local = Local.Criar(
                campus,
                departamento,
                bloco,
                sala
            );

            if (this.DataInicio < DateTime.Now)
                throw new EventoJaComecouExcecao();

            Local = local;
        }
        public void AtualizarImg(string imgUrl)
        {
            if (string.IsNullOrWhiteSpace(imgUrl))
                throw new ArgumentException("A URL da imagem não pode ser nula ou vazia.", nameof(imgUrl));
            ImgUrl = imgUrl;
        }
        public void AtualizarCodigoUnico(string codigoUnico)
        {
            if (string.IsNullOrWhiteSpace(codigoUnico))
                throw new ArgumentException("O código único não pode ser nulo ou vazio.", nameof(codigoUnico));
            CodigoUnico = codigoUnico;
        }
        public void AtualizarTipoEvento(ETipoEvento tipoEvento)
        {
            if (tipoEvento == ETipoEvento.Nenhum)
                throw new ArgumentException("O tipo de evento não pode ser 'Nenhum'.", nameof(tipoEvento));
            TipoEvento = tipoEvento;
        }
        public void AtualizarAvaliadores(string avaliadores)
        {
            if (string.IsNullOrWhiteSpace(avaliadores))
                throw new ArgumentException("Os avaliadores não podem ser nulos ou vazios.", nameof(avaliadores));
            Avaliadores = avaliadores;
        }
    }
}