namespace SisEUs.Application.Comum.Configuracoes
{
    public class CampusConfig
    {
        public string Nome { get; set; } = null!;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double RaioPermitidoMetros { get; set; } = 2000;
    }

    public class GeolocalizacaoConfig
    {
        public const string SectionName = "Geolocalizacao";

        public List<CampusConfig> Campus { get; set; } = new()
        {
            new CampusConfig 
            { 
                Nome = "Fortaleza", 
                Latitude = -3.7436587246947785, 
                Longitude = -38.5410718062838,
                RaioPermitidoMetros = 2000
            },
            new CampusConfig 
            { 
                Nome = "Crateus", 
                Latitude = -5.184846, 
                Longitude = -40.651807,
                RaioPermitidoMetros = 2000
            }
        };
        
        public double RaioCheckinEventoMetros { get; set; } = 100;
    }
}
