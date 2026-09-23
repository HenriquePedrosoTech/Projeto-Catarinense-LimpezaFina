using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class ExcluirEtapaPadraoUseCase : IExcluirEtapaPadraoUseCase
{
    private readonly IEtapaPadraoRepository _etapaPadraoRepository;

    public ExcluirEtapaPadraoUseCase(IEtapaPadraoRepository etapaPadraoRepository)
    {
        _etapaPadraoRepository = etapaPadraoRepository;
    }

    public async Task ExecutarAsync(Guid etapaPadraoId)
    {
        var etapa = await _etapaPadraoRepository.ObterPorIdAsync(etapaPadraoId)
            ?? throw new NotFoundException("Etapa não encontrada.");

        // Registros antigos que referenciam esta etapa continuam funcionando: o
        // mapper (LimpezaFinaMapper) já trata etapa ausente exibindo "(etapa removida)".
        _etapaPadraoRepository.Remover(etapa);
        await _etapaPadraoRepository.SalvarAlteracoesAsync();
    }
}
