using System;

namespace SisEUs.Apresentation.Checkin.DTOs.Resposta
{
    public class RelatorioCheckinResposta
    {
        public string NomeCompleto { get; set; } = null!;
        public string Cpf { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PinUsado { get; set; } = null!;
        
        public string DataCheckin { get; set; } = null!;
        public string HoraCheckin { get; set; } = null!;
        public string DataCheckout { get; set; } = null!;
        public string HoraCheckout { get; set; } = null!;

        public string? Matricula { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}