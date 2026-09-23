using Catarinense.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catarinense.Infrastructure.Data.Configurations;

public class OnibusConfiguration : IEntityTypeConfiguration<Onibus>
{
    public void Configure(EntityTypeBuilder<Onibus> builder)
    {
        builder.ToTable("onibus");

        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id).ValueGeneratedNever(); // Guid gerado pela aplicação (Domain), nunca pelo banco

        builder.Property(o => o.Prefixo).HasMaxLength(20).IsRequired();
        builder.HasIndex(o => o.Prefixo).IsUnique();

        builder.Property(o => o.Placa).HasMaxLength(10);
        builder.Property(o => o.Ativo);
        builder.Property(o => o.CriadoEm);
    }
}
