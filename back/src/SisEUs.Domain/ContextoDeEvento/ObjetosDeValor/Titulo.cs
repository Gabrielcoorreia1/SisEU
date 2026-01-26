using SisEUs.Domain.Comum.Excecoes;
using SisEUs.Domain.Comum.Sementes;

namespace SisEUs.Domain.ContextoDeEvento.ObjetosDeValor
{

    public record Titulo : ObjetoDeValor
    {
        public const int TamanhoMax = 100;
        public const int TamanhoMin = 3;
        private Titulo(string valor)
        {
            Valor = valor;
        }
        private Titulo() { }
        public string Valor { get; } = null!;

        public static Titulo Criar(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ExcecaoDeDominioGenerica("Título não pode ser vazio ou nulo.");
            if (valor.Length < TamanhoMin || valor.Length > TamanhoMax)
                throw new ExcecaoDeDominioGenerica($"Título deve ter entre {TamanhoMin} e {TamanhoMax} caracteres.");
            return new Titulo(valor);
        }
        public override string ToString() => Valor;
    }
}
