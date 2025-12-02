namespace SisEUs.Application.Eventos.DTOs.Resposta
{
    public class NomeCompletoResposta
    {
        public string Nome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;
        public override string ToString()
        {
            return $"{Nome} {Sobrenome}";
        }
    }
}
