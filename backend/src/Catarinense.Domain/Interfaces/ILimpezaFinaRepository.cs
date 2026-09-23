using Catarinense.Domain.Entities;
using Catarinense.Domain.Enums;

namespace Catarinense.Domain.Interfaces;

public interface ILimpezaFinaRepository : IRepositorioBase<LimpezaFina>
{
    /// <summary>
    /// Deve retornar o registro com as etapas (e fotos) já carregadas (Include),
    /// pois as regras de negócio da entidade dependem dessas coleções.
    /// </summary>
    Task<LimpezaFina?> ObterComEtapasAsync(Guid id);

    Task<IReadOnlyList<LimpezaFina>> ListarPorOnibusAsync(Guid onibusId);
    Task<IReadOnlyList<LimpezaFina>> ListarPorOperadorAsync(Guid operadorId);
    Task<IReadOnlyList<LimpezaFina>> ListarPorStatusAsync(StatusLimpeza status);
    Task<IReadOnlyList<LimpezaFina>> ListarParaRelatorioAsync(DateTime? dataInicio, DateTime? dataFim, StatusLimpeza? status, Guid? operadorId);
}
