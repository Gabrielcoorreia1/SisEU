using System;
using System.Security.Claims;

namespace SisEUs.Apresentation.Comum.Utils
{
    public static class GeolocalizacaoUtils
    {
        private const double RAIO_DA_TERRA_METROS = 6371000; 
        private const double TOLERANCIA_METROS = 2000; 

        
        private const double LAT_CAMPUS = -5.184846;
        private const double LON_CAMPUS = -40.651807;

        public static double CalcularDistanciaEmMetros(double lat1, double lon1, double lat2, double lon2)
        {
            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            lat1 = ToRadians(lat1);
            lat2 = ToRadians(lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1) * Math.Cos(lat2);
            
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            
            return RAIO_DA_TERRA_METROS * c;
        }

        public static bool EstaDentroDoRaioDoCampus(double latUsuario, double lonUsuario)
        {
            var distancia = CalcularDistanciaEmMetros(latUsuario, lonUsuario, LAT_CAMPUS, LON_CAMPUS);
            return distancia <= TOLERANCIA_METROS;
        }

        private static double ToRadians(double angle)
        {
            return Math.PI * angle / 180.0;
        }
    }
}