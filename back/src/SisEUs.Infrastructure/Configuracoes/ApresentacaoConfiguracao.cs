using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SisEUs.Domain.ContextoDeEvento.Entidades;
using SisEUs.Domain.ContextoDeEvento.ObjetosDeValor;

namespace SisEUs.Infrastructure.Configuracoes
{
    public class ApresentacaoConfiguration : IEntityTypeConfiguration<Apresentacao>
    {
        public void Configure(EntityTypeBuilder<Apresentacao> builder)
        {
            builder.ToTable("Apresentacoes");
            builder.HasKey(a => a.Id);

            builder.Property(a => a.EventoId).IsRequired();
            builder.HasOne(a => a.Evento)
                   .WithMany(e => e.Apresentacoes)
                   .HasForeignKey(a => a.EventoId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(a => a.Titulo)
                   .HasConversion(
                       titulo => titulo.Valor,
                       valor => Titulo.Criar(valor)
                   )
                   .HasMaxLength(300)
                   .IsRequired();

            builder.Property(a => a.AutorId).IsRequired();
            builder.Property(a => a.OrientadorId).IsRequired();
        }
    }
}
