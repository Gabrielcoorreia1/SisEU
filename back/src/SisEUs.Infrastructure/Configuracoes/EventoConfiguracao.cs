using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.ObjetosDeValor;

namespace SisEUs.Infrastructure.Configuracoes
{
    public class EventoConfiguration : IEntityTypeConfiguration<Evento>
    {
        public void Configure(EntityTypeBuilder<Evento> builder)
        {
            builder.ToTable("Sessao");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.CodigoUnico)
                           .IsRequired()
                           .HasMaxLength(6);

            builder.Property(e => e.Titulo)
                   .HasConversion(
                       titulo => titulo.Valor,
                       valor => Titulo.Criar(valor)
                   )
                   .HasMaxLength(Titulo.TamanhoMax)
                   .IsRequired();

            builder.OwnsOne(e => e.Local, localBuilder =>
            {
                localBuilder.Property(l => l.Campus)
                            .HasColumnName("LocalCampus")
                            .HasMaxLength(Local.TamanhoMax)
                            .IsRequired();

                localBuilder.Property(l => l.Departamento)
                            .HasColumnName("LocalDepartamento")
                            .HasMaxLength(Local.TamanhoMax)
                            .IsRequired();

                localBuilder.Property(l => l.Bloco)
                            .HasColumnName("LocalBloco")
                            .HasMaxLength(Local.TamanhoMax)
                            .IsRequired();

                localBuilder.Property(l => l.Sala)
                            .HasColumnName("LocalSala")
                            .HasMaxLength(Local.TamanhoMax)
                            .IsRequired();
            });

            builder.OwnsOne(e => e.Localizacao, localBuilder =>
            {
                localBuilder.Property(l => l.Latitude).HasColumnName("LocalizacaoLatitude").IsRequired();
                localBuilder.Property(l => l.Longitude).HasColumnName("LocalizacaoLongitude").IsRequired();
            });

            builder.Property(e => e.ParticipantesIds)
                .HasColumnName("Participantes")
                .HasConversion(
                    v => string.Join(',', v),
                    v => string.IsNullOrWhiteSpace(v) ? new List<int>() : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .ToList()
                );

            builder.Property(e => e.ImgUrl)
                   .HasMaxLength(500);

            builder.Property(e => e.CodigoUnico)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasIndex(e => e.CodigoUnico).IsUnique();

            builder.Property(e => e.TipoEvento)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(e => e.AvaliadoresIds)
                .HasColumnName("Avaliadores")
                .HasConversion(
                    v => string.Join(',', v),
                    v => string.IsNullOrWhiteSpace(v) ? new List<int>() : v.Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(int.Parse)
                        .ToList()
                );
        }
    }
}