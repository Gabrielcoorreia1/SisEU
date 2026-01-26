// src/SisEUs.Infrastructure/Repositorios/EventoRepositorio.cs

using Microsoft.EntityFrameworkCore;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Interfaces;

namespace SisEUs.Infrastructure.Repositorios
{
    public class EventoRepositorio(AppDbContext context) : IEventoRepositorio
    {
        public async Task CriarEventoAsync(Evento evento, CancellationToken cancellationToken = default)
        {
            await context.Eventos.AddAsync(evento, cancellationToken);
        }

        public async Task<Evento?> ObterEventoPorIdAsync(int eventoId, CancellationToken cancellationToken = default)
        {
            return await context.Eventos
                .Include(e => e.Apresentacoes)
                .FirstOrDefaultAsync(e => e.Id == eventoId, cancellationToken);
        }

        public async Task<IEnumerable<Evento>> ObterEventosAsync()
        {
            return await context.Eventos
                .Include(e => e.Apresentacoes)
                .ToListAsync();
        }

        public async Task<IEnumerable<Evento>> ObterEventosPaginadosAsync(int skip, int take, CancellationToken cancellationToken = default!)
        {
            return await context.Eventos
                .AsNoTracking()
                .OrderByDescending(e => e.DataInicio)
                .Skip(skip)
                .Take(take)
                .ToListAsync(cancellationToken);
        }

        public void ExcluirEvento(Evento evento)
        {
            context.Eventos.Remove(evento);
        }

        public async Task<Evento?> ObterEventoPorCodigoAsync(string codigo, CancellationToken cancellationToken = default)
        {
            return await context.Eventos
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.CodigoUnico == codigo, cancellationToken);
        }

        public async Task<bool> CodigoUnicoJaExisteAsync(string codigoUnico, int? eventoIdExcluir = null, CancellationToken cancellationToken = default)
        {
            var query = context.Eventos.AsNoTracking().Where(e => e.CodigoUnico == codigoUnico);
            
            if (eventoIdExcluir.HasValue)
            {
                query = query.Where(e => e.Id != eventoIdExcluir.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }
    }
}