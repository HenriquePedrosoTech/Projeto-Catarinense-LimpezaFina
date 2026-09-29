using System.Threading.Tasks;
using Catarinense.Domain.Entities;

namespace Catarinense.Domain.Interfaces;

public interface IFotoHashRepository : IRepositorioBase<FotoHash>
{
    Task<bool> ExisteHashAsync(string hash);
}
