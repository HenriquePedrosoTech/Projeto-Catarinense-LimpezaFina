using System.Threading.Tasks;
using Catarinense.Domain.Entities;
using Catarinense.Domain.Interfaces;
using Catarinense.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catarinense.Infrastructure.Repositories;

public class FotoHashRepository : RepositorioBase<FotoHash>, IFotoHashRepository
{
    public FotoHashRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<bool> ExisteHashAsync(string hash)
    {
        return await DbSet.AnyAsync(f => f.Hash == hash);
    }
}
