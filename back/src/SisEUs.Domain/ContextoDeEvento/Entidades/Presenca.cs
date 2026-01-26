using SisEUs.Domain.Comum.Sementes;
using SisEUs.Domain.ContextoDeEvento.Excecoes;
using SisEUs.Domain.ContextoDeEvento.ObjetosDeValor;

namespace SisEUs.Domain.ContextoDeEvento.Entidades
{
    public class Presenca : Entidade
    {
        private Presenca(int usuarioId, int eventoId, Localizacao localizacao)
        {
            UsuarioId = usuarioId;
            EventoId = eventoId;
            Localizacao = localizacao;
        }

        private Presenca() { }

        public int UsuarioId { get; private set; }
        public int EventoId { get; private set; }
        public Localizacao Localizacao { get; private set; } = null!;
        public DateTime? CheckIn { get; private set; }
        public DateTime? CheckOut { get; private set; }
        public bool CheckInValido { get; private set; }
        public bool CheckOutValido { get; private set; }

        public static Presenca Criar(int usuarioId, int eventoId, string latitude, string longitude)
        {
            var localizacaoVO = Localizacao.Criar(latitude, longitude);
            return new Presenca(usuarioId, eventoId, localizacaoVO);
        }

        public void RealizarCheckIn(DateTime checkIn)
        {
            if (CheckInValido)
            {
                throw new CheckInRealizadoExcecao();
            }
            CheckIn = checkIn;
            CheckInValido = true;
        }

        public void RealizarCheckOut(DateTime checkOut)
        {
            if (!CheckInValido)
            {
                throw new CheckOutSemCheckInExcecao();
            }
            if (CheckOutValido)
            {
                throw new CheckOutRealizadoExcecao();
            }
            CheckOut = checkOut;
            CheckOutValido = true;
        }
    }
}
