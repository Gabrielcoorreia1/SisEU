namespace SisEUs.Application.Eventos.DTOs.Solicitacoes
{
    public class CriarLocalSolicitacao
    {
        public Campus Campus { get; set; }
        public string Departamento { get; set; }
        public string Bloco { get; set; }
        public string Sala { get; set; }
    }

    public enum Campus{
        Fortaleza = 0,
        Crateus = 1,
    }
}
