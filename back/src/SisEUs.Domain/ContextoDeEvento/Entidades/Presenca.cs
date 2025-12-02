using SisEUs.Domain.Comum.Sementes;
using SisEUs.Domain.ContextoDeEvento.Excecoes;
using SisEUs.Domain.ContextoDeEvento.ObjetosDeValor;

namespace SisEUs.Domain.ContextoDeEvento.Entidades
{
    /// <summary>
    /// Representa a presença de um usuário em um evento específico.
    /// </summary>
    /// <remarks>
    /// A criação de instâncias desta classe deve ser feita através do método de fábrica estático <see cref="Criar"/>.
    /// </remarks>
    public class Presenca : Entidade
    {
        // Construtor privado para garantir a criação controlada via método de fábrica.
        private Presenca(int usuarioId, int eventoId, Localizacao localizacao)
        {
            UsuarioId = usuarioId;
            EventoId = eventoId;
            Localizacao = localizacao;
        }
        // Construtor privado sem parâmetros para uso do EF Core.
        private Presenca() { }

        public int UsuarioId { get; }
        public int EventoId { get; }
        public Localizacao Localizacao { get; private set; } = null!;
        public DateTime? CheckIn { get; private set; }
        public DateTime? CheckOut { get; private set; }
        public bool CheckInValido => CheckIn.HasValue;
        public bool CheckOutValido => CheckOut.HasValue;

        /// <summary>
        /// Método de fábrica para criar uma nova instância de Presenca.
        /// </summary>
        /// 
        public static Presenca Criar(int usuarioId, int eventoId, string latitude, string longitude)
        {
            var localizacaoVO = Localizacao.Criar(latitude, longitude);
            return new Presenca(usuarioId, eventoId, localizacaoVO);
        }

        /// <summary>
        /// Registra o check-in, validando o estado interno da presença.
        /// </summary>
        public void RealizarCheckIn(DateTime checkIn)
        {
            if (CheckInValido)
            {
                throw new CheckInRealizadoExcecao();
            }
            CheckIn = checkIn;
        }

        /// <summary>
        /// Registra o check-out, validando o estado interno da presença.
        /// </summary>
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
        }
    }
}
