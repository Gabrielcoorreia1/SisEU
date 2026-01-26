using Microsoft.EntityFrameworkCore;
using SisEUs.Domain.Checkin.Entidades;
using SisEUs.Domain.Checkin.Interfaces;

namespace SisEUs.Infrastructure.Repositorios
{
    public class CheckinPinRepositorio(AppDbContext context) : ICheckinPinRepositorio
    {
        public void Adicionar(CheckinPin pin)
        {
            context.CheckinPins.Add(pin);
        }

        public void Atualizar(CheckinPin pin)
        {
            context.CheckinPins.Update(pin);
        }

        public async Task<CheckinPin?> ObterPinAtivoAsync()
        {
            return await context.CheckinPins
                .FirstOrDefaultAsync(p => p.IsAtivo);
        }

        public async Task<IEnumerable<CheckinPin>> ObterTodosPinsAsync()
        {
            return await context.CheckinPins.AsNoTracking().ToListAsync();
        }
    }
}