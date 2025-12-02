using Microsoft.EntityFrameworkCore;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Interfaces;

namespace SisEUs.Infrastructure.Repositorios
{
    public class ApresentacaoRepositorio : IApresentacaoRepositorio
    {
        private readonly AppDbContext _context;

        public ApresentacaoRepositorio(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Apresentacao apresentacao, CancellationToken cancellationToken = default)
        {
            await _context.Apresentacoes.AddAsync(apresentacao, cancellationToken);
        }

        public async Task<Apresentacao?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Apresentacoes.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IEnumerable<Apresentacao>> ObterPorEventoIdAsync(int eventoId, CancellationToken cancellationToken = default)
        {
            return await _context.Apresentacoes
                .AsNoTracking()
                .Where(a => a.EventoId == eventoId)
                .ToListAsync(cancellationToken);
        }

        public void Remover(Apresentacao apresentacao)
        {
            _context.Apresentacoes.Remove(apresentacao);
        }
    }
}
