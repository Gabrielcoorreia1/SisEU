using Microsoft.EntityFrameworkCore;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Interfaces;

namespace SisEUs.Infrastructure.Repositorios
{
    public class AvaliacaoRepositorio(AppDbContext dbContext) : IAvaliacaoRepositorio
    {
        public async Task AdicionarAsync(Avaliacao avaliacao, CancellationToken cancellationToken = default)
        {
            await dbContext.Avaliacoes.AddAsync(avaliacao, cancellationToken);
        }

        public async Task<Avaliacao?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await dbContext.Avaliacoes
                .Include(a => a.Apresentacao)
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<Avaliacao?> ObterPorApresentacaoEAvaliadorAsync(int apresentacaoId, int avaliadorId, CancellationToken cancellationToken = default)
        {
            return await dbContext.Avaliacoes
                .Include(a => a.Apresentacao)
                .FirstOrDefaultAsync(a => a.ApresentacaoId == apresentacaoId && a.AvaliadorId == avaliadorId, cancellationToken);
        }

        public async Task<IEnumerable<Avaliacao>> ObterPorApresentacaoAsync(int apresentacaoId, CancellationToken cancellationToken = default)
        {
            return await dbContext.Avaliacoes
                .AsNoTracking()
                .Include(a => a.Apresentacao)
                .Where(a => a.ApresentacaoId == apresentacaoId)
                .OrderByDescending(a => a.DataConclusao)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Avaliacao>> ObterPorAvaliadorAsync(int avaliadorId, CancellationToken cancellationToken = default)
        {
            return await dbContext.Avaliacoes
                .AsNoTracking()
                .Include(a => a.Apresentacao)
                .Where(a => a.AvaliadorId == avaliadorId)
                .OrderByDescending(a => a.DataInicio)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Avaliacao>> ObterPorEventoAsync(int eventoId, CancellationToken cancellationToken = default)
        {
            return await dbContext.Avaliacoes
                .AsNoTracking()
                .Include(a => a.Apresentacao)
                .Where(a => a.Apresentacao.EventoId == eventoId)
                .OrderByDescending(a => a.DataConclusao)
                .ToListAsync(cancellationToken);
        }
    }
}
