namespace SisEUs.Application.Presencas.DTOs.Solicitacoes
{
    /// <summary>
    /// DTO interno usado pelo serviço (contém UsuarioId).
    /// </summary>
    public class EfetuarCheckOutSolicitacao
    {
        public int UsuarioId { get; set; }
        public int EventoId { get; set; }
        public string Latitude { get; set; } = null!;
        public string Longitude { get; set; } = null!;
    }

    /// <summary>
    /// DTO exposto na API (não contém UsuarioId - pega do token).
    /// </summary>
    public class EfetuarCheckOutEventoSolicitacao
    {
        public int EventoId { get; set; }
        public string Latitude { get; set; } = null!;
        public string Longitude { get; set; } = null!;
    }
}
