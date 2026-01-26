using SisEUs.Application.Comum.Resultados;

namespace SisEUs.Application.Comum.Servicos
{
    public interface IValidadorDeCoordenadas
    {
        Resultado TryConverterCoordenadas(string latitude, string longitude, out double latDouble, out double lonDouble);

        Resultado ValidarDistanciaParaEvento(
            string latitudeUsuario,
            string longitudeUsuario,
            string latitudeEvento,
            string longitudeEvento);

        Resultado ValidarLocalizacaoCampus(string latitude, string longitude);

        Resultado ValidarLocalizacaoCampus(double latitude, double longitude);
    }
}
