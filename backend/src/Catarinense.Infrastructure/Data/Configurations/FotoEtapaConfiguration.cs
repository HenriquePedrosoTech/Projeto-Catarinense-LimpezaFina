using Catarinense.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catarinense.Infrastructure.Data.Configurations;

public class FotoEtapaConfiguration : IEntityTypeConfiguration<FotoEtapa>
{
    public void Configure(EntityTypeBuilder<FotoEtapa> builder)
    {
        builder.ToTable("fotos_etapa");

        builder.HasKey(f => f.Id);
        builder.Property(f => f.Id).ValueGeneratedNever(); // Guid gerado pela aplicação (Domain), nunca pelo banco

        builder.Property(f => f.LimpezaEtapaExecucaoId).IsRequired();
        builder.Property(f => f.UrlArquivo).HasMaxLength(500).IsRequired();
        builder.Property(f => f.CriadoEm);

        builder.HasIndex(f => f.LimpezaEtapaExecucaoId);
    }
}
