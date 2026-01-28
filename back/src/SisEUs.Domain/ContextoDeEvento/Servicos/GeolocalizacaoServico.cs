namespace SisEUs.Domain.ContextoDeEvento.Servicos
{
    public static class GeolocalizacaoServico
    {
        private const double RaioTerraMetros = 6371000;

        public static double CalcularDistanciaEmMetros(double latitude1, double longitude1, double latitude2, double longitude2)
        {
            var lat1Rad = ParaRadianos(latitude1);
            var lon1Rad = ParaRadianos(longitude1);
            var lat2Rad = ParaRadianos(latitude2);
            var lon2Rad = ParaRadianos(longitude2);

            var deltaLat = lat2Rad - lat1Rad;
            var deltaLon = lon2Rad - lon1Rad;

            var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                    Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                    Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return RaioTerraMetros * c;
        }

        public static bool EstaDentroDoRaio(
            double latitudeOrigem,
            double longitudeOrigem,
            double latitudeDestino,
            double longitudeDestino,
            double raioEmMetros)
        {
            var distancia = CalcularDistanciaEmMetros(latitudeOrigem, longitudeOrigem, latitudeDestino, longitudeDestino);
            return distancia <= raioEmMetros;
        }

        private static double ParaRadianos(double anguloEmGraus)
        {
            return anguloEmGraus * Math.PI / 180;
        }
    }
}
