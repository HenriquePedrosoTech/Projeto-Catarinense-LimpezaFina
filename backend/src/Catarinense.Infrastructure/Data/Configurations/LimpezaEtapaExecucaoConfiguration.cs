using Catarinense.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catarinense.Infrastructure.Data.Configurations;

public class LimpezaEtapaExecucaoConfiguration : IEntityTypeConfiguration<LimpezaEtapaExecucao>
{
    public void Configure(EntityTypeBuilder<LimpezaEtapaExecucao> builder)
    {
        builder.ToTable("limpeza_etapa_execucoes");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever(); // Guid gerado pela aplicação (Domain), nunca pelo banco

        builder.Property(e => e.LimpezaFinaId).IsRequired();
        builder.Property(e => e.EtapaPadraoId).IsRequired();
        builder.Property(e => e.ConcluidaEm);
        builder.Property(e => e.CriadoEm);

        builder.Metadata.FindNavigation(nameof(LimpezaEtapaExecucao.Fotos))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(e => e.Fotos)
            .WithOne()
            .HasForeignKey(f => f.LimpezaEtapaExecucaoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.LimpezaFinaId);
        builder.HasIndex(e => e.EtapaPadraoId);
    }
}
