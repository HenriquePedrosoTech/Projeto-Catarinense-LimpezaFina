using Catarinense.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catarinense.Infrastructure.Data.Configurations;

public class EtapaItemExecucaoConfiguration : IEntityTypeConfiguration<EtapaItemExecucao>
{
    public void Configure(EntityTypeBuilder<EtapaItemExecucao> builder)
    {
        builder.ToTable("etapas_itens_execucao");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        
        builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(50);
        builder.Property(e => e.Funcionalidade).HasConversion<string>().HasMaxLength(50);
        
        builder.Property(e => e.RelatoProblema).HasMaxLength(1000);
        builder.Property(e => e.FotoUrl).HasMaxLength(1000);

        builder.HasOne(e => e.LimpezaEtapaExecucao)
            .WithMany(e => e.Itens)
            .HasForeignKey(e => e.LimpezaEtapaExecucaoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Optional navigation
        builder.HasOne(e => e.EtapaItemPadrao)
            .WithMany()
            .HasForeignKey(e => e.EtapaItemPadraoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
