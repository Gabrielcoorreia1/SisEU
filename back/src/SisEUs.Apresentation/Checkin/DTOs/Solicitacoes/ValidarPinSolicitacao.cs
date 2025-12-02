// SisEUs/src/SisEUs.Apresentation/Checkin/DTOs/Solicitacoes/ValidarPinSolicitacao.cs

namespace SisEUs.Apresentation.Checkin.DTOs.Solicitacoes
{
    public class ValidarPinSolicitacao
    {
        /// <summary>
        /// O PIN global inserido pelo usuário.
        /// </summary>
        public string Pin { get; set; } = null!;
        
        /// <summary>
        /// Latitude atual do dispositivo do usuário.
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Longitude atual do dispositivo do usuário.
        /// </summary>
        public double Longitude { get; set; }

    }
}