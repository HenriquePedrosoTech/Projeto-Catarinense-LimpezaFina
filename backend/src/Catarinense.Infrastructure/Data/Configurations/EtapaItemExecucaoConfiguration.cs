using Catarinense.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catarinense.Infrastructure.Data.Configurations;

public class EtapaItemExecucaoConfiguration : IEntityTypeConfiguration<EtapaItemExecucao>
{
    public void Configure(EntityTypeBuilder<EtapaItemExecucao> builder)
    {
        builder.ToTable("etapas_itens_execucao");

        builder.HasKey(ei => ei.Id);
        builder.Property(ei => ei.Id).ValueGeneratedNever();

        builder.Property(ei => ei.RelatoProblema).HasMaxLength(1000);
        builder.Property(ei => ei.FotoUrl).HasMaxLength(1000);
        builder.Property(ei => ei.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(ei => ei.Funcionalidade).HasConversion<string>().HasMaxLength(50);
        
        builder.HasOne<LimpezaEtapaExecucao>()
               .WithMany(l => l.Itens)
               .HasForeignKey(ei => ei.LimpezaEtapaExecucaoId)
               .OnDelete(DeleteBehavior.Cascade);
               
        builder.HasOne<EtapaItemPadrao>()
               .WithMany()
               .HasForeignKey(ei => ei.EtapaItemPadraoId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
