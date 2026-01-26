using Microsoft.EntityFrameworkCore;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.Interfaces;

namespace SisEUs.Infrastructure.Repositorios
{
    public class PresencaRepositorio(AppDbContext dbContext) : IPresencaRepositorio
    {
        public Task<Presenca?> BuscarPorUsuarioEEventoAsync(int eventoId, int usuarioId, CancellationToken cancellationToken = default)
        {
            return dbContext.Presencas
                .FirstOrDefaultAsync(p => p.EventoId == eventoId && p.UsuarioId == usuarioId, cancellationToken);
        }

        public void CriarPresenca(Presenca presenca, CancellationToken cancellationToken = default)
        {
            dbContext.Presencas.Add(presenca);
        }

        public Task<IEnumerable<Presenca>>? ObterPresencaDeUsuario(int usuarioId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IEnumerable<Presenca>>(
                dbContext.Presencas
                    .AsNoTracking()
                    .Where(p => p.UsuarioId == usuarioId)
                    .ToList()
            );
        }

        public Task<Presenca?> ObterPresencaEventoEmAndamentoAsync(int usuarioId, CancellationToken cancellationToken = default)
        {
            return dbContext.Presencas
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.UsuarioId == usuarioId && e.CheckInValido && !e.CheckOutValido, cancellationToken);
        }

        public Task<Presenca?> ObterPresencaPorIdAsync(int presencaId, CancellationToken cancellationToken = default)
        {
            return dbContext.Presencas.FindAsync(new object[] { presencaId }, cancellationToken).AsTask();
        }

        [Obsolete("Use ObterPresencasPaginadas para evitar problemas de performance com grandes volumes de dados.")]
        public async Task<IEnumerable<Presenca>> ObterPresencas()
        {
            return await dbContext.Presencas
                .AsNoTracking()
                .ToListAsync();
        }

        public Task<Presenca?> ObterStatusPresencaPorEvento(int eventoId, int usuarioId, CancellationToken cancellationToken = default)
        {
            return dbContext.Presencas
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.EventoId == eventoId && p.UsuarioId == usuarioId, cancellationToken);
        }

        public async Task<IEnumerable<Presenca>> ObterPresencasPaginadas(int pagina, int tamanhoPagina, CancellationToken cancellationToken = default)
        {
            if (pagina <= 0) pagina = 1;
            if (tamanhoPagina <= 0 || tamanhoPagina > 100) tamanhoPagina = 10;

            return await dbContext.Presencas
                .AsNoTracking()
                .OrderByDescending(p => p.CheckIn)
                .Skip((pagina - 1) * tamanhoPagina)
                .Take(tamanhoPagina)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Presenca>> ObterPresencasPorEventoAsync(int eventoId, CancellationToken cancellationToken = default)
        {
            return await dbContext.Presencas
                .AsNoTracking()
                .Where(p => p.EventoId == eventoId)
                .OrderByDescending(p => p.CheckIn)
                .ToListAsync(cancellationToken);
        }
    }
}

