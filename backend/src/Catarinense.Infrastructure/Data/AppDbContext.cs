using Catarinense.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catarinense.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Onibus> Onibus => Set<Onibus>();
    public DbSet<EtapaPadrao> EtapasPadrao => Set<EtapaPadrao>();
    public DbSet<LimpezaFina> LimpezasFinas => Set<LimpezaFina>();
    public DbSet<LimpezaEtapaExecucao> LimpezaEtapaExecucoes => Set<LimpezaEtapaExecucao>();
    public DbSet<FotoEtapa> FotosEtapa => Set<FotoEtapa>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Aplica todas as classes IEntityTypeConfiguration<T> da pasta Data/Configurations.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // O Pomelo usa "ascii_general_ci" por padrão nas colunas char(36) que guardam
        // Guid (Id, chaves estrangeiras). O TiDB Cloud não suporta essa collation —
        // só "ascii_bin" pro charset ascii. Forçamos ascii_bin em todo Guid do modelo,
        // o que é equivalente pra esse caso (comparação exata de string, sem acento).
        foreach (var entidade in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var propriedade in entidade.GetProperties())
            {
                if (propriedade.ClrType == typeof(Guid) || propriedade.ClrType == typeof(Guid?))
                {
                    propriedade.SetCollation("ascii_bin");
                }
            }
        }

        base.OnModelCreating(modelBuilder);
    }
}
