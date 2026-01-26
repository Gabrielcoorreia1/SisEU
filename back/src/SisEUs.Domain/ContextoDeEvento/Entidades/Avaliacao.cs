using SisEUs.Domain.Comum.Sementes;
using SisEUs.Domain.ContextoDeEvento.Enumeracoes;
using SisEUs.Domain.ContextoDeEvento.Excecoes;

namespace SisEUs.Domain.ContextoDeEvento.Entidades
{
    public class Avaliacao : Entidade
    {
        public int ApresentacaoId { get; private set; }
        public int AvaliadorId { get; private set; }
        public decimal? Nota { get; private set; }
        public string? Parecer { get; private set; }
        public EEstadoAvaliacao Estado { get; private set; }
        public DateTime? DataInicio { get; private set; }
        public DateTime? DataConclusao { get; private set; }

        // Navigation properties
        public Apresentacao Apresentacao { get; private set; } = null!;

        private Avaliacao() { }

        public static Avaliacao Criar(int apresentacaoId, int avaliadorId)
        {
            return new Avaliacao
            {
                ApresentacaoId = apresentacaoId,
                AvaliadorId = avaliadorId,
                Estado = EEstadoAvaliacao.Pendente
            };
        }

        public static Avaliacao Iniciar(int apresentacaoId, int avaliadorId)
        {
            return new Avaliacao
            {
                ApresentacaoId = apresentacaoId,
                AvaliadorId = avaliadorId,
                Estado = EEstadoAvaliacao.EmAvaliacao,
                DataInicio = DateTime.Now
            };
        }

        public void IniciarAvaliacao()
        {
            if (Estado == EEstadoAvaliacao.Concluido)
                throw new AvaliacaoJaConcluidaExcecao();

            Estado = EEstadoAvaliacao.EmAvaliacao;
            DataInicio = DateTime.Now;
        }

        public void Avaliar(decimal nota, string parecer)
        {
            if (Estado == EEstadoAvaliacao.Concluido)
                throw new AvaliacaoJaConcluidaExcecao();

            if (nota < 0 || nota > 10)
                throw new NotaInvalidaExcecao();

            Nota = nota;
            Parecer = parecer;
            Estado = EEstadoAvaliacao.Concluido;
            DataConclusao = DateTime.Now;
        }
    }
}
