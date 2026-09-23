using Catarinense.Domain.Entities;

namespace Catarinense.Domain.Interfaces;

public interface IOnibusRepository : IRepositorioBase<Onibus>
{
    Task<Onibus?> ObterPorPrefixoAsync(string prefixo);
}
