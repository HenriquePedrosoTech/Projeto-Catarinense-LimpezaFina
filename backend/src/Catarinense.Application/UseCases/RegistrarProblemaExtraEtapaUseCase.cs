using Catarinense.Application.DTOs;
using Catarinense.Domain.Exceptions;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class RegistrarProblemaExtraEtapaUseCase
{
    private readonly ILimpezaFinaRepository _limpezaRepository;

    public RegistrarProblemaExtraEtapaUseCase(ILimpezaFinaRepository limpezaRepository)
    {
        _limpezaRepository = limpezaRepository;
    }

    public async Task ExecutarAsync(Guid limpezaFinaId, Guid etapaPadraoId, string descricao, string? fotoUrl)
    {
        var limpeza = await _limpezaRepository.ObterPorIdAsync(limpezaFinaId);
        if (limpeza == null)
            throw new DomainException("Limpeza não encontrada.");

        var etapa = limpeza.Etapas.FirstOrDefault(e => e.EtapaPadraoId == etapaPadraoId);
        if (etapa == null)
            throw new DomainException("Etapa não encontrada nesta limpeza.");

        etapa.RelatarProblemaExtra(descricao, fotoUrl);

        await _limpezaRepository.SalvarAlteracoesAsync();
    }
}
