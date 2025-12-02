using Microsoft.EntityFrameworkCore;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Interfaces;

namespace SisEUs.Infrastructure.Repositorios
{
    public class PresencaRepositorio : IPresencaRepositorio
    {
        private readonly AppDbContext _context;
        public PresencaRepositorio(AppDbContext dbContext)
        {
            _context = dbContext;
        }
        public async Task<Presenca?> BuscarPorUsuarioEEventoAsync(int eventoId, int usuarioId, CancellationToken cancellationToken = default)
        {
            return await _context.Presencas
                .FirstOrDefaultAsync(p => p.EventoId == eventoId && p.UsuarioId == usuarioId, cancellationToken);
        }

        public async Task CriarPresenca(Presenca presenca, CancellationToken cancellationToken = default)
        {
            await _context.Presencas.AddAsync(presenca, cancellationToken);
        }

        public async Task<IEnumerable<Presenca>>? ObterPresencaDeUsuario(int usuarioId, CancellationToken cancellationToken = default)
        {
            return await _context.Presencas
                .Where(p => p.UsuarioId == usuarioId)
                .ToListAsync(cancellationToken);
        }

        public Task<Presenca?> ObterPresencaEventoEmAndamentoAsync(CancellationToken cancellationToken = default)
        {
            return _context.Presencas
                .FirstOrDefaultAsync(e => e.CheckInValido && !e.CheckOutValido, cancellationToken);
        }

        public async Task<Presenca?> ObterPresencaPorIdAsync(int presencaId, CancellationToken cancellationToken = default)
        {
            return await _context.Presencas
                .FirstOrDefaultAsync(p => p.Id == presencaId, cancellationToken);
        }

        public async Task<IEnumerable<Presenca>> ObterPresencas()
        {
            return await _context.Presencas.ToListAsync();
        }

        public Task<Presenca?> ObterStatusPresencaPorEvento(int eventoId, int usuarioId, CancellationToken cancellationToken = default)
        {
            return _context.Presencas
                .FirstOrDefaultAsync(p => p.EventoId == eventoId && p.UsuarioId == usuarioId, cancellationToken);
        }
    }
}

