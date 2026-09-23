using Catarinense.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catarinense.Infrastructure.Data.Configurations;

public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
{
    public void Configure(EntityTypeBuilder<Usuario> builder)
    {
        builder.ToTable("usuarios");

        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id).ValueGeneratedNever(); // Guid gerado pela aplicação (Domain), nunca pelo banco

        builder.Property(u => u.Matricula)
            .HasMaxLength(20)
            .IsRequired();
        builder.HasIndex(u => u.Matricula).IsUnique();

        builder.Property(u => u.Nome).HasMaxLength(150).IsRequired();
        builder.Property(u => u.Email).HasMaxLength(150);
        builder.Property(u => u.SenhaHash).HasMaxLength(200).IsRequired();
        builder.Property(u => u.Perfil).HasConversion<string>().HasMaxLength(30);
        builder.Property(u => u.Ativo);
        builder.Property(u => u.TentativasFalhasLogin);
        builder.Property(u => u.BloqueadoAte);
        builder.Property(u => u.CriadoEm);
    }
}
