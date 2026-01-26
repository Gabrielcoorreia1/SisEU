using SisEUs.Domain.Comum.Excecoes;

namespace SisEUs.Domain.Checkin.Excecoes
{
    public class CheckOutJaRealizadoException : ExcecaoDeDominio
    {
        public CheckOutJaRealizadoException() 
            : base("O check-out já foi realizado para este registro.")
        {
        }
    }
}
