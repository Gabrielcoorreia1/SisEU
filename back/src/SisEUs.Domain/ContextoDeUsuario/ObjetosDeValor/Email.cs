using SisEUs.Domain.Comum.Excecoes;
using SisEUs.Domain.Comum.Sementes;

namespace SisEUs.Domain.ContextoDeUsuario.ObjetosDeValor
{

    public record Email : ObjetoDeValor
    {
        private Email(string valor)
        {
            Valor = valor;
        }
        public string Valor { get; }
        public static Email Criar(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ExcecaoDeDominioGenerica("Email não pode ser nulo ou vazio.");
            }
            if (!IsValidEmail(value))
            {
                throw new ExcecaoDeDominioGenerica("Formato de email inválido.");
            }
            return new Email(value);
        }
        private Email() { }
        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public override string ToString() => Valor;
    }
}
