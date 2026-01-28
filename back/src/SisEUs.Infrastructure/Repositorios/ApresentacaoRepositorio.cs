using Microsoft.EntityFrameworkCore;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Interfaces;

namespace SisEUs.Infrastructure.Repositorios
{
    public class ApresentacaoRepositorio(AppDbContext context) : IApresentacaoRepositorio
    {
        public async Task AdicionarAsync(Apresentacao apresentacao, CancellationToken cancellationToken = default)
        {
            await context.Apresentacoes.AddAsync(apresentacao, cancellationToken);
        }

        public async Task<Apresentacao?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await context.Apresentacoes.FindAsync([id], cancellationToken);
        }

        public async Task<IEnumerable<Apresentacao>> ObterPorEventoIdAsync(int eventoId, CancellationToken cancellationToken = default)
        {
            return await context.Apresentacoes
                .AsNoTracking()
                .Where(a => a.EventoId == eventoId)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Apresentacao>> ObterPorNomeAutorAsync(string nomeAutor, CancellationToken cancellationToken = default)
        {
            var nomeBusca = nomeAutor.ToLower().Trim();
            return await context.Apresentacoes
                .AsNoTracking()
                .Include(a => a.Evento)
                .Where(a => a.NomeAutor.ToLower().Contains(nomeBusca))
                .ToListAsync(cancellationToken);
        }

        public void Remover(Apresentacao apresentacao)
        {
            context.Apresentacoes.Remove(apresentacao);
        }
    }
}
