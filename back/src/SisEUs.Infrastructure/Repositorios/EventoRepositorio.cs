// src/SisEUs.Infrastructure/Repositorios/EventoRepositorio.cs

using Microsoft.EntityFrameworkCore;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Interfaces;

namespace SisEUs.Infrastructure.Repositorios
{
    public class EventoRepositorio : IEventoRepositorio
    {
        private readonly AppDbContext _context;
        public EventoRepositorio(AppDbContext context)
        {
            _context = context;
        }

        public async Task CriarEventoAsync(Evento evento, CancellationToken cancellationToken = default)
        {
            await _context.Sessoes.AddAsync(evento, cancellationToken);
        }

        public async Task<Evento?> ObterEventoPorIdAsync(int eventoId, CancellationToken cancellationToken = default)
        {

            return await _context.Sessoes
                .Include(s => s.Apresentacoes)
                .FirstOrDefaultAsync(e => e.Id == eventoId, cancellationToken);
        }

        public async Task<IEnumerable<Evento>> ObterEventosAsync()
        {

            return await _context.Sessoes
                .Include(s => s.Apresentacoes)
                .ToListAsync();
        }

        public async Task<IEnumerable<Evento>> ObterEventosPaginadosAsync(int skip, int take, CancellationToken cancellationToken = default!)
        {
            return await _context.Sessoes
                .Include(s => s.Apresentacoes)
                .AsNoTracking()
                .Skip((skip - 1) * take)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public void ExcluirEvento(Evento evento)
        {
            _context.Sessoes.Remove(evento);
        }

        public async Task<Evento?> ObterEventoPorCodigoAsync(string codigo, CancellationToken cancellationToken = default)
        {
            return await _context.Sessoes
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.CodigoUnico == codigo, cancellationToken);
        }
    }
}