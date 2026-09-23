using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Exceptions;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class ExcluirOnibusUseCase : IExcluirOnibusUseCase
{
    private readonly IOnibusRepository _onibusRepository;
    private readonly ILimpezaFinaRepository _limpezaFinaRepository;

    public ExcluirOnibusUseCase(IOnibusRepository onibusRepository, ILimpezaFinaRepository limpezaFinaRepository)
    {
        _onibusRepository = onibusRepository;
        _limpezaFinaRepository = limpezaFinaRepository;
    }

    public async Task ExecutarAsync(Guid onibusId)
    {
        var onibus = await _onibusRepository.ObterPorIdAsync(onibusId)
            ?? throw new NotFoundException("Ônibus não encontrado.");

        var limpezasVinculadas = await _limpezaFinaRepository.ListarPorOnibusAsync(onibusId);
        if (limpezasVinculadas.Count > 0)
            throw new DomainException(
                $"Não é possível excluir: existem {limpezasVinculadas.Count} registro(s) de limpeza fina vinculados a este ônibus. Exclua-os primeiro.");

        _onibusRepository.Remover(onibus);
        await _onibusRepository.SalvarAlteracoesAsync();
    }
}
