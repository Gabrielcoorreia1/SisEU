using Microsoft.EntityFrameworkCore;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeUsuario.Entidades;
using System.Reflection;
using SisEUs.Domain.Checkin.Entidades;

namespace SisEUs.Infrastructure.Repositorios
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Evento> Sessoes { get; set; }
        public DbSet<Presenca> Presencas { get; set; }
        public DbSet<Apresentacao> Apresentacoes { get; set; }
        public DbSet<CheckinPin> CheckinPins { get; set; }
        public DbSet<Checkin> Checkins { get; set; } 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}