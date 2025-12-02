using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SisEUs.Domain.ContextoDeUsuario.Entidades;
using SisEUs.Domain.ContextoDeUsuario.ObjetosDeValor;

namespace SisEUs.Infrastructure.Configuracoes
{
    public class UsuarioConfiguracao : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.EUserType)
                   .HasConversion<string>()
                   .HasMaxLength(50);

            builder.OwnsOne(u => u.Nome, nomeBuilder =>
            {
                nomeBuilder.Property(n => n.Nome).HasColumnName("Nome").IsRequired();
                nomeBuilder.Property(n => n.Sobrenome).HasColumnName("Sobrenome").IsRequired();
            });

            builder.Property(u => u.Cpf)
                   .HasConversion(
                       cpf => cpf.Valor,
                       valor => Cpf.Criar(valor)
                   )
                   .HasMaxLength(11)
                   .IsRequired();

            builder.Property(u => u.Email)
                   .HasConversion(
                       email => email.Valor,
                       valor => Email.Criar(valor)
                   )
                   .HasMaxLength(255)
                   .IsRequired();

            builder.Property(u => u.Senha)
                   .HasConversion(
                       senha => senha.Valor,
                       valor => Senha.Criar(valor)
                   )
                   .IsRequired();

            builder.HasIndex(u => u.Cpf).IsUnique();
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.UserIdentifier);
        }
    }
}

