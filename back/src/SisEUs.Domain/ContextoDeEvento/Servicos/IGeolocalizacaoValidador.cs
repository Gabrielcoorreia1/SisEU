namespace SisEUs.Domain.ContextoDeEvento.Servicos
{
    public interface IGeolocalizacaoValidador
    {
        bool EstaDentroDoRaioPermitido(double latitudeUsuario, double longitudeUsuario, double latitudeEvento, double longitudeEvento);
        double CalcularDistanciaEmMetros(double latitude1, double longitude1, double latitude2, double longitude2);
        double RaioMaximoCheckinMetros { get; }
        bool EstaDentroDeAlgumCampus(double latitudeUsuario, double longitudeUsuario);
        (double Latitude, double Longitude)? ObterCoordenadasCampusPorNome(string nomeCampus);
    }
}
