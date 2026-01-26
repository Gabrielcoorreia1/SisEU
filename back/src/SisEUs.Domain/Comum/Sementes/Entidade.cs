namespace SisEUs.Domain.Comum.Sementes
{
    public abstract class Entidade
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
