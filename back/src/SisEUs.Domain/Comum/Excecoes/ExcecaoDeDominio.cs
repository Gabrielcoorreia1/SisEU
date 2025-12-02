namespace SisEUs.Domain.Comum.Excecoes
{
    /// <summary>
    /// Exceção base para todas as exceções customizadas relacionadas a regras de negócio do domínio.
    /// </summary>
    public abstract class ExcecaoDeDominio : Exception
    {
        protected ExcecaoDeDominio() : base() { }
        protected ExcecaoDeDominio(string mensagem) : base(mensagem) { }
        protected ExcecaoDeDominio(string mensagem, Exception? innerException) : base(mensagem, innerException) { }
    }
}
