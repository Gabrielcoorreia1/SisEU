using SisEUs.Application.Comum.UoW;
using SisEUs.Infrastructure.Repositorios;

namespace SisEUs.Infrastructure.Servicos;

public class UoW : IUoW
{
    private readonly AppDbContext _context;
    public UoW(AppDbContext contexto)
    {
        _context = contexto;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default!)
    {
        await _context.SaveChangesAsync();
    }
}
