using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SisEUs.Domain.ContextoDeEvento.Entidades;

namespace SisEUs.Infrastructure.Configuracoes
{
    public class PresencaConfiguracao : IEntityTypeConfiguration<Presenca>
    {
        public void Configure(EntityTypeBuilder<Presenca> builder)
        {
            builder.ToTable("Presencas");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.UsuarioId).IsRequired();
            builder.Property(p => p.EventoId).IsRequired();

            builder.Property(p => p.CheckIn);
            builder.Property(p => p.CheckOut);

            builder.Property(p => p.CheckInValido)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(p => p.CheckOutValido)
                .IsRequired()
                .HasDefaultValue(false);

            builder.OwnsOne(p => p.Localizacao, localBuilder =>
            {
                localBuilder.Property(l => l.Latitude).HasColumnName("LocalizacaoLatitude").IsRequired();
                localBuilder.Property(l => l.Longitude).HasColumnName("LocalizacaoLongitude").IsRequired();
            });

            builder.HasIndex(p => new { p.UsuarioId, p.EventoId }).IsUnique();
        }
    }
}