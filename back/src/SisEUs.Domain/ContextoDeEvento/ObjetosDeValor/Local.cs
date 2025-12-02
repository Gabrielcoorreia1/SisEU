using SisEUs.Domain.Comum.Sementes;

namespace SisEUs.Domain.ContextoDeEvento.ObjetosDeValor
{
    public record Local : ObjetoDeValor
    {
        public const int TamanhoMin = 1;
        public const int TamanhoMax = 100;
        private Local(string campus, string departamento, string bloco, string sala)
        {
            Campus = campus;
            Departamento = departamento;
            Bloco = bloco;
            Sala = sala;
        }
        private Local() { }
        public static Local Criar(string campus, string departamento, string bloco, string sala)
        {
            if (EValido(campus, departamento, bloco, sala))
            {
                return new Local(campus, departamento, bloco, sala);
            }
            throw new Excecoes.LocalInvalidoExcecao();
        }
        public static bool EValido(string campus, string departamento, string bloco, string sala)
        {
            return !string.IsNullOrWhiteSpace(campus) &&
                   !string.IsNullOrWhiteSpace(departamento) &&
                   !string.IsNullOrWhiteSpace(bloco) &&
                   !string.IsNullOrWhiteSpace(sala) &&
                   campus.Length >= TamanhoMin &&
                   campus.Length <= TamanhoMax &&
                   departamento.Length >= TamanhoMin &&
                   departamento.Length <= TamanhoMax &&
                   bloco.Length >= TamanhoMin &&
                   bloco.Length <= TamanhoMax &&
                   sala.Length >= TamanhoMin &&
                   sala.Length <= TamanhoMax;
        }
        public string Campus { get; }
        public string Departamento { get; }
        public string Bloco { get; }
        public string Sala { get; }
    }
}
