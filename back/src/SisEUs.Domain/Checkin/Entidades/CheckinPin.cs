using SisEUs.Domain.Comum.Sementes;

namespace SisEUs.Domain.Checkin.Entidades
{
    public class CheckinPin : Entidade
    {
        public string Pin { get; private set; } = null!;
        public DateTime DataGeracao { get; private set; }
        public bool IsAtivo { get; private set; }

        private CheckinPin() { }

        public static CheckinPin Criar(string pin)
        {
            if (string.IsNullOrEmpty(pin) || pin.Length != 6)
            {
                throw new ArgumentException("O PIN deve ter exatamente 6 caracteres.", nameof(pin));
            }

            if (!pin.All(char.IsDigit))
            {
                throw new ArgumentException("O PIN deve conter apenas dígitos.", nameof(pin));
            }

            return new CheckinPin
            {
                Pin = pin,
                DataGeracao = DateTime.Now,
                IsAtivo = true
            };
        }

        public void Invalidar()
        {
            IsAtivo = false;
        }
    }
}