using Catarinense.Domain.Entities;
using Catarinense.Domain.Interfaces;
using Catarinense.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catarinense.Infrastructure.Repositories;

public class OnibusRepository : RepositorioBase<Onibus>, IOnibusRepository
{
    public OnibusRepository(AppDbContext contexto) : base(contexto) { }

    public async Task<Onibus?> ObterPorPrefixoAsync(string prefixo) =>
        await DbSet.FirstOrDefaultAsync(o => o.Prefixo == prefixo.Trim().ToUpper());
}
