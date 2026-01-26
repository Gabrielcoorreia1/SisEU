using SisEUs.Domain.Comum.Sementes;
using SisEUs.Domain.ContextoDeEvento.Enumeracoes;
using SisEUs.Domain.ContextoDeEvento.Excecoes;
using SisEUs.Domain.ContextoDeEvento.ObjetosDeValor;
using System.ComponentModel.DataAnnotations;

namespace SisEUs.Domain.ContextoDeEvento.Entidades
{
    public class Evento : Entidade
    {
        private Evento(
            Titulo titulo,
            DateTime dataInicio,
            DateTime dataFim,
            Local local,
            IEnumerable<int> participantes,
            IEnumerable<int> avaliadores,
            Localizacao localizacao,
            string imgUrl,
            string codigoUnico,
            ETipoEvento eTipoEvento)
        {
            Titulo = titulo;
            DataInicio = dataInicio;
            DataFim = dataFim;
            Local = local;
            Localizacao = localizacao;
            ParticipantesIds = participantes?.Distinct().ToList() ?? [];
            AvaliadoresIds = avaliadores?.Distinct().ToList() ?? [];
            ImgUrl = imgUrl;
            CodigoUnico = codigoUnico;
            TipoEvento = eTipoEvento;
        }

        private Evento() { }
        
        [MaxLength(10)]
        public string? PinCheckin { get; private set; }
        public Titulo Titulo { get; private set; } = null!;
        public DateTime DataInicio { get; private set; }
        public DateTime DataFim { get; private set; }
        public Local Local { get; private set; } = null!;
        public Localizacao Localizacao { get; private set; } = null!;
        public ETipoEvento TipoEvento { get; private set; } = ETipoEvento.Nenhum;
        public List<int> AvaliadoresIds { get; private set; } = [];
        public List<int> ParticipantesIds { get; private set; } = [];
        public string ImgUrl { get; private set; } = string.Empty;
        public string CodigoUnico { get; private set; } = null!;
        public ICollection<Apresentacao> Apresentacoes { get; private set; } = [];
        
        public void DefinirPinCheckin(string pin)
        {
            PinCheckin = pin;
        }

        public static Evento Criar(
            Titulo titulo,
            DateTime dataInicio,
            DateTime dataFim,
            Local local,
            IEnumerable<int> participantes,
            IEnumerable<int> avaliadores,
            Localizacao localizacao,
            string imgUrl,
            string codigoUnico,
            ETipoEvento eTipoEvento)
        {
            if (dataInicio >= dataFim)
                throw new DataInvalidaExcecao();

            return new Evento(titulo, dataInicio, dataFim, local, participantes, avaliadores, localizacao, imgUrl, codigoUnico, eTipoEvento);
        }

        public void AdicionarParticipante(int participanteId)
        {
            if (ParticipantesIds.Contains(participanteId))
                throw new ParticipanteJaAdicionadoExcecao(participanteId);

            ParticipantesIds.Add(participanteId);
        }

        public void RemoverParticipante(int usuarioId)
        {
            bool removido = ParticipantesIds.Remove(usuarioId);
            if (!removido)
                throw new ParticipanteNaoEncontradoExcecao();
        }
        
        public void AtualizarDataInicio(DateTime novaDataInicio)
        {
            if (this.DataInicio < DateTime.Now)
                throw new EventoJaComecouExcecao();

            if (novaDataInicio >= this.DataFim)
                throw new DataInvalidaExcecao();

            this.DataInicio = novaDataInicio;
        }

        public void AtualizarDataFim(DateTime novaDataFim)
        {
            if (novaDataFim <= this.DataInicio)
                throw new DataInvalidaExcecao();

            this.DataFim = novaDataFim;
        }

        public void AtualizarTitulo(string novoTitulo) => Titulo = Titulo.Criar(novoTitulo);

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
        
        public void AdicionarAvaliador(int usuarioId)
        {
            if (AvaliadoresIds.Contains(usuarioId))
                throw new AvaliadorJaAdicionadoExcecao();

            AvaliadoresIds.Add(usuarioId);
        }

        public void RemoverAvaliador(int usuarioId)
        {
            if (!AvaliadoresIds.Remove(usuarioId))
                throw new AvaliadorNaoEncontradoExcecao();
        }

        public void AtualizarAvaliadores(IEnumerable<int> novosAvaliadores)
        {
            AvaliadoresIds = [.. novosAvaliadores.Distinct()];
        }
    }
}