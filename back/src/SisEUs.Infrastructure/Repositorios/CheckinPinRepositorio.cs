using Microsoft.EntityFrameworkCore;
using SisEUs.Domain.Checkin.Entidades;
using SisEUs.Domain.Checkin.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace SisEUs.Infrastructure.Repositorios
{
    public class CheckinPinRepositorio : ICheckinPinRepositorio
    {
        private readonly AppDbContext _context;

        public CheckinPinRepositorio(AppDbContext context)
        {
            _context = context;
        }

        public void Adicionar(CheckinPin pin)
        {
            _context.CheckinPins.Add(pin);
        }

        public void Atualizar(CheckinPin pin)
        {
            _context.CheckinPins.Update(pin);
        }

        public async Task<CheckinPin?> ObterPinAtivoAsync()
        {
            return await _context.CheckinPins
                .FirstOrDefaultAsync(p => p.IsAtivo);
        }

        public async Task<IEnumerable<CheckinPin>> ObterTodosPinsAsync()
        {
            return await _context.CheckinPins.AsNoTracking().ToListAsync();
        }
    }
}