namespace SisEUs.Application.Comum.Resultados
{
    public class Resultado
    {
        public bool Sucesso { get; }
        public TipoDeErro? TipoDeErro { get; }
        public IReadOnlyCollection<string> Erros { get; }

        protected Resultado(bool sucesso, string[] erros, TipoDeErro? tipoDeErro = null)
        {
            Sucesso = sucesso;
            TipoDeErro = tipoDeErro;
            Erros = erros ?? [];
        }

        public static Resultado Ok() => new(true, [], null);
        public static Resultado Falha(TipoDeErro tipoDeErro, params string[] erros) => new(false, erros, tipoDeErro);
    }

    public class Resultado<T> : Resultado
    {
        public T Valor { get; }

        private Resultado(T valor) : base(true, [], null) { Valor = valor; }
        private Resultado(TipoDeErro tipoDeErro, string[] erros) : base(false, erros, tipoDeErro) { Valor = default!; }

        public static Resultado<T> Ok(T valor) => new(valor);
        public new static Resultado<T> Falha(TipoDeErro tipoDeErro, params string[] erros) => new(tipoDeErro, erros);
    }
}
