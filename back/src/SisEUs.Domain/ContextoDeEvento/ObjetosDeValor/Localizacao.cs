using SisEUs.Domain.Comum.Sementes;
using SisEUs.Domain.ContextoDeEvento.Excecoes;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SisEUs.Domain.ContextoDeEvento.ObjetosDeValor
{
    public record Localizacao : ObjetoDeValor
    {
        private Localizacao(string latitude, string longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        private Localizacao() { }

        public string Latitude { get; } = null!;
        public string Longitude { get; } = null!;

        public static Localizacao Criar(string latitude, string longitude)
        {
            if (!EValido(latitude, longitude))
                throw new LocalizacaoInvalidaExcecao();

            return new Localizacao(latitude, longitude);
        }

        public static bool EValido(string latitude, string longitude)
        {
            if (string.IsNullOrWhiteSpace(latitude) || string.IsNullOrWhiteSpace(longitude))
                return false;

            string pattern = @"^[-+]?\d+(\.\d+)?$";
            var regex = new Regex(pattern);

            if (!regex.IsMatch(latitude) || !regex.IsMatch(longitude))
                return false;

            if (!double.TryParse(latitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var lat) ||
                !double.TryParse(longitude, NumberStyles.Any, CultureInfo.InvariantCulture, out var lon))
                return false;

            if (lat < -90 || lat > 90)
                return false;

            if (lon < -180 || lon > 180)
                return false;

            return true;
        }

        public double LatitudeDecimal => double.Parse(Latitude, CultureInfo.InvariantCulture);
        public double LongitudeDecimal => double.Parse(Longitude, CultureInfo.InvariantCulture);
    }
}
