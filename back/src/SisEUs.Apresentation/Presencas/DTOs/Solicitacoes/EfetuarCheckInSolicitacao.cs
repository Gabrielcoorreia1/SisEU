namespace SisEUs.Application.Presencas.DTOs.Solicitacoes
{
    public class EfetuarCheckInSolicitacao
    {
        public int UsuarioId { get; set; }
        public int EventoId { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
