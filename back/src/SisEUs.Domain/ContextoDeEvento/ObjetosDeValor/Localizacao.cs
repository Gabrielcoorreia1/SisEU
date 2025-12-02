using SisEUs.Domain.Comum.Sementes;
using SisEUs.Domain.ContextoDeEvento.Excecoes;
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
        public string Latitude { get; }
        public string Longitude { get; }

        public static Localizacao Criar(string latitude, string longitude)
        {
            return EValido(latitude, longitude)
                ? new Localizacao(latitude, longitude)
                : throw new LocalizacaoInvalidaExcecao();
        }
        public static bool EValido(string latitude, string longitude)
        {
            if (string.IsNullOrWhiteSpace(latitude) || string.IsNullOrWhiteSpace(longitude))
                return false;

            if (latitude.Length < -90 || latitude.Length > 90 || longitude.Length < -180 || longitude.Length > 180)
                return false;


            string pattern = @"^[-+]?\d+(\.\d+)?$";

            Regex regex = new(pattern);

            if (!regex.IsMatch(latitude) || !regex.IsMatch(longitude))
            {
                return false;
            }

            if (!double.TryParse(latitude, out double lat) || !double.TryParse(longitude, out double lng))
            {
                return false;
            }
            return true;
        }

        public static List<Localizacao> ObterLocalizacoesUFC()
        {
            return new List<Localizacao>
            {
                // UFC - Crateús
                new Localizacao("-5.178645", "-40.667794"),

                // UFC - Fortaleza (Campus do Pici)
                new Localizacao("-3.745749", "-38.574452")
            };
        }
    }
}
