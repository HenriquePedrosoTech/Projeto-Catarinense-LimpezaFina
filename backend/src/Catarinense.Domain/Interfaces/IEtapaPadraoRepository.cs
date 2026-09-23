using Catarinense.Domain.Entities;

namespace Catarinense.Domain.Interfaces;

public interface IEtapaPadraoRepository : IRepositorioBase<EtapaPadrao>
{
    Task<IReadOnlyList<EtapaPadrao>> ListarAtivasAsync();

    /// <summary>
    /// Retorna as etapas padrão correspondentes aos ids informados (inclusive inativas),
    /// necessário para exibir o histórico de registros antigos mesmo que a etapa
    /// tenha sido desativada depois.
    /// </summary>
    Task<IReadOnlyList<EtapaPadrao>> ListarPorIdsAsync(IEnumerable<Guid> ids);
}
