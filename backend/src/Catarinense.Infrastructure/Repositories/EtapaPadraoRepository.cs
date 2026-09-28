using Catarinense.Domain.Entities;
using Catarinense.Domain.Interfaces;
using Catarinense.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catarinense.Infrastructure.Repositories;

public class EtapaPadraoRepository : RepositorioBase<EtapaPadrao>, IEtapaPadraoRepository
{
    public EtapaPadraoRepository(AppDbContext contexto) : base(contexto) { }

    public async Task<IReadOnlyList<EtapaPadrao>> ListarAtivasAsync() =>
        await DbSet.Include(e => e.Itens).Where(e => e.Ativo).OrderBy(e => e.Ordem).ToListAsync();

    public async Task<IReadOnlyList<EtapaPadrao>> ListarPorIdsAsync(IEnumerable<Guid> ids) =>
        await DbSet.Include(e => e.Itens).Where(e => ids.Contains(e.Id)).ToListAsync();

    public override async Task<IReadOnlyList<EtapaPadrao>> ListarAsync() =>
        await DbSet.Include(e => e.Itens).ToListAsync();
}
