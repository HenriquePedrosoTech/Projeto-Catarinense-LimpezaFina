using Catarinense.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catarinense.Infrastructure.Data.Configurations;

public class EtapaItemPadraoConfiguration : IEntityTypeConfiguration<EtapaItemPadrao>
{
    public void Configure(EntityTypeBuilder<EtapaItemPadrao> builder)
    {
        builder.ToTable("etapas_itens_padrao");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Texto).IsRequired().HasMaxLength(255);
        builder.Property(e => e.EnquadramentoFoto).HasMaxLength(500);

        builder.HasOne(e => e.EtapaPadrao)
            .WithMany(e => e.Itens)
            .HasForeignKey(e => e.EtapaPadraoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
