namespace SisEUs.Application.Comum.UoW
{
    public interface IUoW
    {
        Task CommitAsync(CancellationToken cancellationToken = default!);
    }
}
