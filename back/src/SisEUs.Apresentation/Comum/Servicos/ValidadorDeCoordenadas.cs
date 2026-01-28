using SisEUs.Application.Comum.Resultados;
using SisEUs.Domain.ContextoDeEvento.Servicos;
using System.Globalization;

namespace SisEUs.Application.Comum.Servicos
{
    public class ValidadorDeCoordenadas : IValidadorDeCoordenadas
    {
        private readonly IGeolocalizacaoValidador _geolocalizacaoValidador;

        public ValidadorDeCoordenadas(IGeolocalizacaoValidador geolocalizacaoValidador)
        {
            _geolocalizacaoValidador = geolocalizacaoValidador;
        }

        public Resultado TryConverterCoordenadas(string latitude, string longitude, out double latDouble, out double lonDouble)
        {
            latDouble = 0;
            lonDouble = 0;

            if (string.IsNullOrWhiteSpace(latitude))
            {
                return Resultado.Falha(TipoDeErro.Validacao, "Latitude não pode ser vazia.");
            }

            if (string.IsNullOrWhiteSpace(longitude))
            {
                return Resultado.Falha(TipoDeErro.Validacao, "Longitude não pode ser vazia.");
            }

            if (!double.TryParse(latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out latDouble))
            {
                return Resultado.Falha(TipoDeErro.Validacao, "Formato de latitude inválido. Use '.' como separador decimal.");
            }

            if (!double.TryParse(longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out lonDouble))
            {
                return Resultado.Falha(TipoDeErro.Validacao, "Formato de longitude inválido. Use '.' como separador decimal.");
            }

            if (latDouble < -90 || latDouble > 90)
            {
                return Resultado.Falha(TipoDeErro.Validacao, "Latitude deve estar entre -90 e 90 graus.");
            }

            if (lonDouble < -180 || lonDouble > 180)
            {
                return Resultado.Falha(TipoDeErro.Validacao, "Longitude deve estar entre -180 e 180 graus.");
            }

            return Resultado.Ok();
        }

        public Resultado ValidarDistanciaParaEvento(
            string latitudeUsuario,
            string longitudeUsuario,
            string latitudeEvento,
            string longitudeEvento)
        {
            var resultadoUsuario = TryConverterCoordenadas(latitudeUsuario, longitudeUsuario, out double latUser, out double lonUser);
            if (!resultadoUsuario.Sucesso)
            {
                return resultadoUsuario;
            }

            var resultadoEvento = TryConverterCoordenadas(latitudeEvento, longitudeEvento, out double latEvento, out double lonEvento);
            if (!resultadoEvento.Sucesso)
            {
                return Resultado.Falha(TipoDeErro.Validacao, "Coordenadas do evento inválidas.");
            }

            if (!_geolocalizacaoValidador.EstaDentroDoRaioPermitido(latUser, lonUser, latEvento, lonEvento))
            {
                var distancia = _geolocalizacaoValidador.CalcularDistanciaEmMetros(latUser, lonUser, latEvento, lonEvento);
                return Resultado.Falha(TipoDeErro.Validacao,
                    $"Você está fora do raio permitido. Distância: {distancia:F0}m, máximo permitido: {_geolocalizacaoValidador.RaioMaximoCheckinMetros:F0}m.");
            }

            return Resultado.Ok();
        }

        public Resultado ValidarLocalizacaoCampus(string latitude, string longitude)
        {
            var resultado = TryConverterCoordenadas(latitude, longitude, out double latDouble, out double lonDouble);
            if (!resultado.Sucesso)
            {
                return resultado;
            }

            return ValidarLocalizacaoCampus(latDouble, lonDouble);
        }

        public Resultado ValidarLocalizacaoCampus(double latitude, double longitude)
        {
            if (!_geolocalizacaoValidador.EstaDentroDeAlgumCampus(latitude, longitude))
            {
                return Resultado.Falha(TipoDeErro.Validacao, "Você não está na área permitida do Campus.");
            }

            return Resultado.Ok();
        }
    }
}
