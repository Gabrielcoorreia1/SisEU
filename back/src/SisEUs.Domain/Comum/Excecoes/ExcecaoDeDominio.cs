namespace SisEUs.Domain.Comum.Excecoes
{
    public abstract class ExcecaoDeDominio : Exception
    {
        protected ExcecaoDeDominio() : base() { }
        protected ExcecaoDeDominio(string mensagem) : base(mensagem) { }
        protected ExcecaoDeDominio(string mensagem, Exception? innerException) : base(mensagem, innerException) { }
    }
}
