using SisEUs.Domain.Comum.Sementes;
using SisEUs.Domain.ContextoDeEvento.ObjetosDeValor;
using SisEUs.Domain.ContextoDeUsuario.ObjetosDeValor;

namespace SisEUs.Domain.ContextoDeEvento.Entidades
{
    public class Apresentacao : Entidade
    {
        public Apresentacao(int eventoId, Titulo titulo, string nomeAutor, string nomeOrientador)
        {
            EventoId = eventoId;
            Titulo = titulo;
            NomeAutor = nomeAutor;
            NomeOrientador = nomeOrientador;
        }

        private Apresentacao() { }
        public int EventoId { get; set; }

        public Evento Evento { get; set; }
        
        public Titulo Titulo { get; private set; } = null!;
        public string NomeAutor { get; private set; } = null!;
        public string NomeOrientador { get; private set; } = null!;

        public static Apresentacao Criar(int eventoId, Titulo titulo, string nomeAutor, string nomeOrientador)
        {
            return new Apresentacao(eventoId, titulo, nomeAutor, nomeOrientador);
        }
        public void Atualizar(Titulo titulo, string nomeAutor, string nomeOrientador)
        {
            Titulo = titulo;
            NomeAutor = nomeAutor;
            NomeOrientador = nomeOrientador;
        }
    }
}