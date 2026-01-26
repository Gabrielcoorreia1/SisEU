namespace SisEUs.Domain.Comum.Excecoes
{
    public class ExcecaoDeDominioGenerica : ExcecaoDeDominio
    {
        public ExcecaoDeDominioGenerica() : base() { }
        public ExcecaoDeDominioGenerica(string mensagem) : base(mensagem) { }
        public ExcecaoDeDominioGenerica(string mensagem, Exception innerException) : base(mensagem, innerException) { }
    }
}
