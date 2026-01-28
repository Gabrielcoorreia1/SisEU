using Microsoft.Extensions.Options;
using SisEUs.Application.Comum.Configuracoes;
using SisEUs.Domain.ContextoDeEvento.Servicos;

namespace SisEUs.Application.Comum.Servicos
{
    public class GeolocalizacaoValidador : IGeolocalizacaoValidador
    {
        private readonly GeolocalizacaoConfig _config;

        public GeolocalizacaoValidador(IOptions<GeolocalizacaoConfig> config)
        {
            _config = config.Value;
        }

        public double RaioMaximoCheckinMetros => _config.RaioCheckinEventoMetros;

        public bool EstaDentroDoRaioPermitido(
            double latitudeUsuario, 
            double longitudeUsuario, 
            double latitudeEvento, 
            double longitudeEvento)
        {
            return GeolocalizacaoServico.EstaDentroDoRaio(
                latitudeUsuario, 
                longitudeUsuario,
                latitudeEvento, 
                longitudeEvento,
                _config.RaioCheckinEventoMetros);
        }

        public double CalcularDistanciaEmMetros(
            double latitude1, 
            double longitude1, 
            double latitude2, 
            double longitude2)
        {
            return GeolocalizacaoServico.CalcularDistanciaEmMetros(latitude1, longitude1, latitude2, longitude2);
        }

        public bool EstaDentroDeAlgumCampus(double latitudeUsuario, double longitudeUsuario)
        {
            foreach (var campus in _config.Campus)
            {
                if (GeolocalizacaoServico.EstaDentroDoRaio(
                    latitudeUsuario, longitudeUsuario,
                    campus.Latitude, campus.Longitude,
                    campus.RaioPermitidoMetros))
                {
                    return true;
                }
            }
            return false;
        }

        public (double Latitude, double Longitude)? ObterCoordenadasCampusPorNome(string nomeCampus)
        {
            var campus = _config.Campus.FirstOrDefault(c => 
                c.Nome.Equals(nomeCampus, StringComparison.OrdinalIgnoreCase));
            
            if (campus == null)
                return null;
                
            return (campus.Latitude, campus.Longitude);
        }

        public CampusConfig? ObterCampusPorNome(string nomeCampus)
        {
            return _config.Campus.FirstOrDefault(c => 
                c.Nome.Equals(nomeCampus, StringComparison.OrdinalIgnoreCase));
        }
    }
}
