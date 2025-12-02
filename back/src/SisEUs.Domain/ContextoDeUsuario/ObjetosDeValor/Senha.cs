using SisEUs.Domain.Comum.Excecoes;
using SisEUs.Domain.Comum.Sementes;

namespace SisEUs.Domain.ContextoDeUsuario.ObjetosDeValor
{
    public record Senha : ObjetoDeValor
    {
        public string Valor { get; }

        private Senha(string valor)
        {
            Valor = valor;
        }

        public static Senha Criar(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ExcecaoDeDominioGenerica("Senha não pode ser nula ou vazia.");
            if (valor.Length < 6 || valor.Length > 100) 
                throw new ExcecaoDeDominioGenerica("Senha deve ter entre 6 e 100 caracteres.");
            
            return new Senha(valor);
        }
    }
}