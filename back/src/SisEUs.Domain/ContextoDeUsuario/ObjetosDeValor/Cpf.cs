using SisEUs.Domain.Comum.Excecoes;
using SisEUs.Domain.Comum.Sementes;

namespace SisEUs.Domain.ContextoDeUsuario.ObjetosDeValor
{
    public record Cpf : ObjetoDeValor
    {
        private Cpf(string valor)
        {
            Valor = valor;
        }
        public string Valor { get; } = null!;

        public static Cpf Criar(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ExcecaoDeDominioGenerica("CPF não pode ser nulo ou vazio.");
            }
            if (!ECpf(value))
            {
                throw new ExcecaoDeDominioGenerica("Formato de CPF inválido.");
            }
            return new Cpf(value);
        }
        private Cpf() { }
        private static bool ECpf(string cpf)
        {
            cpf = cpf.Trim().Replace(".", "").Replace("-", "");

            if (cpf.Length != 11)
                return false;
            if (!cpf.All(char.IsDigit) || cpf.Distinct().Count() == 1)
                return false;

            return true;
        }

        public override string ToString() => Valor;
    }
}