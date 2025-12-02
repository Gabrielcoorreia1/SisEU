using SisEUs.Domain.Checkin.Entidades;
using SisEUs.Domain.Checkin.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

// Alias usado na interface ICheckinRepositorio.cs
using EntidadeCheckin = SisEUs.Domain.Checkin.Entidades.Checkin;

namespace SisEUs.Infrastructure.Repositorios
{
    public class CheckinRepositorio : ICheckinRepositorio
    {
        private readonly AppDbContext _context;

        public CheckinRepositorio(AppDbContext context)
        {
            _context = context;
        }

        public void Adicionar(EntidadeCheckin checkin)
        {
            _context.Checkins.Add(checkin);
        }

        public void Atualizar(EntidadeCheckin checkin)
        {
            // O Entity Framework rastreia a entidade, mas o Update é usado para garantir a persistência em cenários complexos.
            _context.Checkins.Update(checkin);
        }

        public async Task<bool> VerificarCheckinExistenteAsync(int usuarioId, int pinId)
        {
            return await _context.Checkins
                .AnyAsync(c => c.UsuarioId == usuarioId && c.PinId == pinId);
        }

        public async Task<IEnumerable<EntidadeCheckin>> ObterTodosCheckinsAsync()
        {
            return await _context.Checkins.AsNoTracking().ToListAsync();
        }

        public async Task<EntidadeCheckin?> ObterCheckinAbertoAsync(int usuarioId)
        {
            return await _context.Checkins
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId && c.DataHoraCheckOut == null);
        }
    }
}