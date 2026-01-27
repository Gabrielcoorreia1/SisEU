using SisEUs.Domain.Comum.Excecoes;

namespace SisEUs.Domain.Comum.Validacao
{
    public class ExcecaoDeValidacaoMultipla : ExcecaoDeDominio
    {
        public IReadOnlyCollection<string> Erros { get; }

        public ExcecaoDeValidacaoMultipla(IEnumerable<string> erros)
            : base(string.Join(" | ", erros))
        {
            Erros = erros.ToList().AsReadOnly();
        }
    }
}
