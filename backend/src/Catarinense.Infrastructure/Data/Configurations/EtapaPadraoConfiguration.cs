using Catarinense.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catarinense.Infrastructure.Data.Configurations;

public class EtapaPadraoConfiguration : IEntityTypeConfiguration<EtapaPadrao>
{
    public void Configure(EntityTypeBuilder<EtapaPadrao> builder)
    {
        builder.ToTable("etapas_padrao");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever(); // Guid gerado pela aplicação (Domain), nunca pelo banco

        builder.Property(e => e.Nome).HasMaxLength(100).IsRequired();
        builder.Property(e => e.Ordem);
        builder.Property(e => e.Ativo);
        builder.Property(e => e.CriadoEm);
    }
}
