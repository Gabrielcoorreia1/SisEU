using SisEUs.Domain.Comum.Sementes;
using System;
using System.ComponentModel.DataAnnotations.Schema;

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
            return new Checkin
            {
                UsuarioId = usuarioId,
                PinId = pinId,
                DataHoraCheckIn = DateTime.Now,
                Latitude = latitude,
                Longitude = longitude
            };
        }
        
        public void RegistrarCheckOut(double latitude, double longitude)
        {
            if (DataHoraCheckOut.HasValue)
            {
                return; 
            }
            DataHoraCheckOut = DateTime.Now;
        }
    }

}