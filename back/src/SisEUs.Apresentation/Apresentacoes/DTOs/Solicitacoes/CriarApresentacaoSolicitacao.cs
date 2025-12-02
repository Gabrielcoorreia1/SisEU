namespace SisEUs.Application.Apresentacoes.DTOs.Solicitacoes
{
    public class CriarApresentacaoSolicitacao
    {
        public long? Id { get; set; }
        public int EventoId { get; set; }
        public string Titulo { get; set; }
        public string NomeAutor { get; set; }
        public string NomeOrientador { get; set; }
    }
}
