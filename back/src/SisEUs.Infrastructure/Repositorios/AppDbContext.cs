using Microsoft.EntityFrameworkCore;
using SisEUs.Domain.Checkin.Entidades;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeUsuario.Entidades;

namespace SisEUs.Infrastructure.Repositorios
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Presenca> Presencas { get; set; }
        public DbSet<Apresentacao> Apresentacoes { get; set; }
        public DbSet<Avaliacao> Avaliacoes { get; set; }
        public DbSet<CheckinPin> CheckinPins { get; set; }
        public DbSet<Checkin> Checkins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            
            modelBuilder.Entity<Presenca>()
                .HasIndex(p => new { p.EventoId, p.UsuarioId })
                .HasDatabaseName("IX_Presencas_Evento_Usuario")
                .IsUnique();

            modelBuilder.Entity<Presenca>()
                .HasIndex(p => p.UsuarioId)
                .HasDatabaseName("IX_Presencas_Usuario");

            modelBuilder.Entity<Presenca>()
                .HasIndex(p => new { p.CheckInValido, p.CheckOutValido })
                .HasDatabaseName("IX_Presencas_Status");

            modelBuilder.Entity<Presenca>()
                .HasIndex(p => p.CheckIn)
                .HasDatabaseName("IX_Presencas_CheckIn");

            modelBuilder.Entity<Evento>()
                .HasIndex(e => e.CodigoUnico)
                .HasDatabaseName("IX_Eventos_CodigoUnico")
                .IsUnique();

            modelBuilder.Entity<Evento>()
                .HasIndex(e => e.DataInicio)
                .HasDatabaseName("IX_Eventos_DataInicio");

            modelBuilder.Entity<Apresentacao>()
                .HasIndex(a => a.EventoId)
                .HasDatabaseName("IX_Apresentacoes_EventoId");

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.EUserType)
                .HasDatabaseName("IX_Usuarios_Tipo");

            // Configuração básica de Avaliacao
            modelBuilder.Entity<Avaliacao>(builder =>
            {
                builder.ToTable("Avaliacoes");
                builder.HasKey(a => a.Id);
                builder.Property(a => a.ApresentacaoId).IsRequired();
                builder.Property(a => a.AvaliadorId).IsRequired();
                builder.Property(a => a.Nota).HasPrecision(4, 2);
                builder.Property(a => a.Parecer).HasMaxLength(2000);
                builder.Property(a => a.Estado).HasConversion<string>().IsRequired();
                
                builder.HasIndex(a => new { a.ApresentacaoId, a.AvaliadorId })
                    .HasDatabaseName("IX_Avaliacoes_Apresentacao_Avaliador")
                    .IsUnique();
            });
        }
    }
}