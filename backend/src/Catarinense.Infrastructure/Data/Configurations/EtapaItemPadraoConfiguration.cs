using Catarinense.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catarinense.Infrastructure.Data.Configurations;

public class EtapaItemPadraoConfiguration : IEntityTypeConfiguration<EtapaItemPadrao>
{
    public void Configure(EntityTypeBuilder<EtapaItemPadrao> builder)
    {
        builder.ToTable("etapas_itens_padrao");

        builder.HasKey(ei => ei.Id);
        builder.Property(ei => ei.Id).ValueGeneratedNever();

        builder.Property(ei => ei.Texto).HasMaxLength(300).IsRequired();
        builder.Property(ei => ei.EnquadramentoFoto).HasMaxLength(300);
        
        builder.HasOne<EtapaPadrao>()
               .WithMany(e => e.Itens)
               .HasForeignKey(ei => ei.EtapaPadraoId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
