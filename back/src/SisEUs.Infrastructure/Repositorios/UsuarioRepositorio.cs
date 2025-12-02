using Microsoft.EntityFrameworkCore;
using SisEUs.Domain.ContextoDeUsuario.Entidades;
using SisEUs.Domain.ContextoDeUsuario.Interfaces;
using SisEUs.Domain.ContextoDeUsuario.ObjetosDeValor;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System; // Adicionado para Guid

namespace SisEUs.Infrastructure.Repositorios
{
    public class UsuarioRepositorio : IUsuarioRepositorio
    {
        private readonly AppDbContext _context;

        public UsuarioRepositorio(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Usuario usuario, CancellationToken cancellationToken = default)
        {
            await _context.Usuarios.AddAsync(usuario, cancellationToken);
        }

        public async Task<bool> CpfJaExisteAsync(Cpf cpf, CancellationToken cancellationToken = default)
        {
            return await _context.Usuarios.AnyAsync(u => u.Cpf == cpf, cancellationToken);
        }
        public async Task<Usuario?> ObterPorCpfAsync(Cpf cpf, CancellationToken cancellationToken = default)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Cpf == cpf, cancellationToken);
        }

        public async Task<bool> EmailJaExisteAsync(Email email, CancellationToken cancellationToken = default)
        {
            return await _context.Usuarios.AnyAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<Usuario?> ObterPorEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<Usuario?> ObterPorIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Usuarios.FindAsync(new object[] { id }, cancellationToken);
        }

        public Task<IEnumerable<Usuario>> ObterPorIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
        {
            return _context.Usuarios
                .Where(u => ids.Contains(u.Id))
                .ToListAsync(cancellationToken)
                .ContinueWith(task => task.Result.AsEnumerable(), cancellationToken);
        }

        public async Task<IEnumerable<Usuario>> BuscarPorNomeProfessorAsync(string nome, CancellationToken cancellationToken = default)
        {
            var termoBusca = nome.ToLower().Trim();

            return await _context.Usuarios
                .AsNoTracking()
                .Where(u => u.Nome.Nome.ToLower().Contains(termoBusca) || 
                u.Nome.Sobrenome.ToLower().Contains(termoBusca) && 
                u.EUserType == Domain.ContextoDeUsuario.Enumeracoes.ETipoUsuario.Professor)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Usuario>> ObterTodosUsuariosAsync()
        {
            return await _context.Usuarios.AsNoTracking().ToListAsync();
        }
        public async Task<Usuario?> ObterPorUserIdentifierAsync(Guid userIdentifier, CancellationToken cancellationToken = default)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.UserIdentifier == userIdentifier, cancellationToken);
        }
    }
}