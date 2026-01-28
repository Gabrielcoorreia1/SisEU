using SisEUs.Domain.Comum.Sementes;
using SisEUs.Domain.ContextoDeEvento.Enumeracoes;
using SisEUs.Domain.ContextoDeEvento.ObjetosDeValor;

namespace SisEUs.Domain.ContextoDeEvento.Entidades
{
    public class Apresentacao : Entidade
    {
        private Apresentacao(int eventoId, Titulo titulo, string nomeAutor, string nomeOrientador, EModalidadeApresentacao modalidade)
        {
            EventoId = eventoId;
            Titulo = titulo;
            NomeAutor = nomeAutor;
            NomeOrientador = nomeOrientador;
            Modalidade = modalidade;
        }

        private Apresentacao() { }
        
        public int EventoId { get; private set; }

        public Evento Evento { get; private set; } = null!;

        public Titulo Titulo { get; private set; } = null!;
        public string NomeAutor { get; private set; } = string.Empty;
        public string NomeOrientador { get; private set; } = string.Empty;
        public EModalidadeApresentacao Modalidade { get; private set; }

        public static Apresentacao Criar(int eventoId, Titulo titulo, string nomeAutor, string nomeOrientador, EModalidadeApresentacao modalidade)
        {
            return new Apresentacao(eventoId, titulo, nomeAutor, nomeOrientador, modalidade);
        }

        public void Atualizar(Titulo titulo, string nomeAutor, string nomeOrientador)
        {
            Titulo = titulo;
            NomeAutor = nomeAutor;
            NomeOrientador = nomeOrientador;
        }

        public void AtualizarModalidade(EModalidadeApresentacao modalidade)
        {
            Modalidade = modalidade;
        }
    }
}