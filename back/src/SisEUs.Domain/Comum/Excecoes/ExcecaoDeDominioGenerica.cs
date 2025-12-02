namespace SisEUs.Domain.Comum.Excecoes
{
    /// <summary>
    /// Exceção para violações de regras de domínio genéricas que não possuem uma exceção específica.
    /// </summary>
    /// <remarks>
    /// Use com moderação. Prefira sempre criar uma exceção específica para a regra de negócio.
    /// </remarks>
    public class ExcecaoDeDominioGenerica : ExcecaoDeDominio
    {
        public ExcecaoDeDominioGenerica() : base() { }
        public ExcecaoDeDominioGenerica(string mensagem) : base(mensagem) { }
        public ExcecaoDeDominioGenerica(string mensagem, Exception innerException) : base(mensagem, innerException) { }
    }
}
