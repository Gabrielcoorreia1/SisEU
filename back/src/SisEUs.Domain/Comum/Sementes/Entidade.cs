namespace SisEUs.Domain.Comum.Sementes
{
    /// <summary>
    /// Entidade base para todas as entidades do domínio.
    /// </summary>
    /// <remarks>
    /// Ao implementar uma nova entidade, herde desta classe por padrão.
    /// Propriedades comuns a todas as entidades devem ser adicionadas aqui.
    /// </remarks>
    public abstract class Entidade
    {
        public int Id { get; set; } = new Random().Next();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
