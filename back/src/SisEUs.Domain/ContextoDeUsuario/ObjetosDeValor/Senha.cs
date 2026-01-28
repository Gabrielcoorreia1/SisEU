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
                throw new ExcecaoDeDominioGenerica("A senha/hash não pode ser nula ou vazia.");

            if (valor.Length > 255)
                throw new ExcecaoDeDominioGenerica("A senha excede o tamanho máximo permitido.");

            return new Senha(valor);
        }
    }
}