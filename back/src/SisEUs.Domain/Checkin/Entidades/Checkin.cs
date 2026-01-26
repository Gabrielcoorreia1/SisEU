using SisEUs.Domain.Comum.Sementes;
using SisEUs.Domain.Checkin.Excecoes;

namespace SisEUs.Domain.Checkin.Entidades
{
    public class Checkin : Entidade
    {
        public int UsuarioId { get; private set; }
        public int PinId { get; private set; }
        public DateTime DataHoraCheckIn { get; private set; }
        public DateTime? DataHoraCheckOut { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        private Checkin() { }

        public static Checkin Criar(int usuarioId, int pinId, double latitude, double longitude)
        {
            if (usuarioId <= 0)
                throw new ArgumentException("ID do usuário inválido.", nameof(usuarioId));

            if (pinId <= 0)
                throw new ArgumentException("ID do PIN inválido.", nameof(pinId));

            return new Checkin
            {
                UsuarioId = usuarioId,
                PinId = pinId,
                DataHoraCheckIn = DateTime.Now,
                Latitude = latitude,
                Longitude = longitude
            };
        }

        public void RegistrarCheckOut()
        {
            if (DataHoraCheckOut.HasValue)
            {
                throw new CheckOutJaRealizadoException();
            }
            DataHoraCheckOut = DateTime.Now;
        }

        public bool CheckOutRealizado => DataHoraCheckOut.HasValue;
    }
}
