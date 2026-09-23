using Catarinense.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catarinense.Infrastructure.Data.Configurations;

public class LimpezaFinaConfiguration : IEntityTypeConfiguration<LimpezaFina>
{
    public void Configure(EntityTypeBuilder<LimpezaFina> builder)
    {
        builder.ToTable("limpezas_finas");

        builder.HasKey(l => l.Id);
        builder.Property(l => l.Id).ValueGeneratedNever(); // Guid gerado pela aplicação (Domain), nunca pelo banco

        builder.Property(l => l.OnibusId).IsRequired();
        builder.Property(l => l.NumeroOS).HasMaxLength(50);
        builder.Property(l => l.OperadorId).IsRequired();
        builder.Property(l => l.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(l => l.IniciadaEm);
        builder.Property(l => l.FinalizadaEm);
        builder.Property(l => l.AvaliadaEm);
        builder.Property(l => l.AvaliadorId);
        builder.Property(l => l.ObservacaoAvaliacao).HasMaxLength(500);
        builder.Property(l => l.NotificacaoEnviada);
        builder.Property(l => l.CriadoEm);

        // A entidade expõe "Etapas" como IReadOnlyCollection, mas o EF acessa
        // diretamente o campo privado "_etapas" (List<T>) para ler/gravar.
        builder.Metadata.FindNavigation(nameof(LimpezaFina.Etapas))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);

        builder.HasMany(l => l.Etapas)
            .WithOne()
            .HasForeignKey(e => e.LimpezaFinaId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(l => l.NumeroOS);
        builder.HasIndex(l => l.OnibusId);
        builder.HasIndex(l => l.OperadorId);
        builder.HasIndex(l => l.Status);
    }
}
