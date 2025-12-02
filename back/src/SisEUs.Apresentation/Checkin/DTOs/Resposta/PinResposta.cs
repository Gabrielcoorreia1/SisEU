using System;

namespace SisEUs.Apresentation.Checkin.DTOs.Resposta
{
    /// <summary>
    /// Representa os dados de resposta do PIN de check-in global.
    /// </summary>
    public class PinResposta
    {
        public int Id { get; set; }

        /// <summary>
        /// O PIN numérico de 6 dígitos.
        /// </summary>
        public string Pin { get; set; } = null!;

        /// <summary>
        /// Data e hora em que o PIN foi gerado.
        /// </summary>
        public DateTime DataGeracao { get; set; }
    }
}