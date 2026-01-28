using SisEUs.Domain.Comum.Sementes;
using SisEUs.Domain.ContextoDeEvento.Enumeracoes;
using SisEUs.Domain.ContextoDeEvento.ObjetosDeValor;

namespace SisEUs.Domain.ContextoDeEvento.Entidades
{
    public class Apresentacao : Entidade
    {
        private Apresentacao(int eventoId, Titulo titulo, int autorId, int orientadorId, EModalidadeApresentacao modalidade)
        {
            EventoId = eventoId;
            Titulo = titulo;
            AutorId = autorId;
            OrientadorId = orientadorId;
            Modalidade = modalidade;
        }

        private Apresentacao() { }
        
        public int EventoId { get; private set; }

        public Evento Evento { get; private set; } = null!;

        public Titulo Titulo { get; private set; } = null!;
        
        public int AutorId { get; private set; }
        
        public int OrientadorId { get; private set; }
        
        public EModalidadeApresentacao Modalidade { get; private set; }

        public static Apresentacao Criar(int eventoId, Titulo titulo, int autorId, int orientadorId, EModalidadeApresentacao modalidade)
        {
            return new Apresentacao(eventoId, titulo, autorId, orientadorId, modalidade);
        }

        public void Atualizar(Titulo titulo, int autorId, int orientadorId)
        {
            Titulo = titulo;
            AutorId = autorId;
            OrientadorId = orientadorId;
        }

        public void AtualizarModalidade(EModalidadeApresentacao modalidade)
        {
            Modalidade = modalidade;
        }
    }
}