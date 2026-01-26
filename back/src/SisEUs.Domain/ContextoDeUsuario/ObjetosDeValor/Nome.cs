using SisEUs.Domain.Comum.Excecoes;
using SisEUs.Domain.Comum.Sementes;

namespace SisEUs.Domain.ContextoDeUsuario.ObjetosDeValor
{
    public record NomeCompleto : ObjetoDeValor
    {
        public const int MaxLength = 100;
        public const int MinLength = 3;

        private NomeCompleto(string nome, string sobrenome)
        {
            Nome = nome;
            Sobrenome = sobrenome;
        }
        public string Nome { get; } = null!;
        public string Sobrenome { get; } = null!;

        public static NomeCompleto Criar(string nome, string sobrenome)
        {
            if (!Validar(nome, sobrenome))
            {
                throw new ExcecaoDeDominioGenerica("Nome e sobrenome inválidos.");
            }
            return new NomeCompleto(nome, sobrenome);
        }
        private NomeCompleto() { }
        private static bool Validar(string nome, string sobrenome)
        {
            if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(sobrenome))
            {
                throw new ExcecaoDeDominioGenerica("Nome e sobrenome não podem ser nulos ou vazios.");
            }
            if (nome.Length < MinLength || nome.Length > MaxLength)
            {
                throw new ExcecaoDeDominioGenerica($"O nome deve ter entre {MinLength} e {MaxLength} caracteres.");
            }
            if (sobrenome.Length < MinLength || sobrenome.Length > MaxLength)
            {
                throw new ExcecaoDeDominioGenerica($"O sobrenome deve ter entre {MinLength} e {MaxLength} caracteres.");
            }
            return true;
        }
    }
}
