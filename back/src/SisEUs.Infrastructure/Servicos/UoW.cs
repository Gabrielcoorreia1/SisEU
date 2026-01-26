using SisEUs.Application.Comum.UoW;
using SisEUs.Infrastructure.Repositorios;

namespace SisEUs.Infrastructure.Servicos;

public class UoW(AppDbContext context) : IUoW
{
    public async Task CommitAsync(CancellationToken cancellationToken = default!)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
