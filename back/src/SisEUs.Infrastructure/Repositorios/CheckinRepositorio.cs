using Microsoft.EntityFrameworkCore;
using SisEUs.Domain.Checkin.Interfaces;
using EntidadeCheckin = SisEUs.Domain.Checkin.Entidades.Checkin;

namespace SisEUs.Infrastructure.Repositorios
{
    public class CheckinRepositorio(AppDbContext context) : ICheckinRepositorio
    {
        public void Adicionar(EntidadeCheckin checkin)
        {
            context.Checkins.Add(checkin);
        }

        public void Atualizar(EntidadeCheckin checkin)
        {
            context.Checkins.Update(checkin);
        }

        public async Task<bool> VerificarCheckinExistenteAsync(int usuarioId, int pinId)
        {
            return await context.Checkins
                .AnyAsync(c => c.UsuarioId == usuarioId && c.PinId == pinId);
        }

        public async Task<IEnumerable<EntidadeCheckin>> ObterTodosCheckinsAsync()
        {
            return await context.Checkins.AsNoTracking().ToListAsync();
        }

        public async Task<EntidadeCheckin?> ObterCheckinAbertoAsync(int usuarioId)
        {
            return await context.Checkins
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.DataHoraCheckOut == null);
        }
    }
}