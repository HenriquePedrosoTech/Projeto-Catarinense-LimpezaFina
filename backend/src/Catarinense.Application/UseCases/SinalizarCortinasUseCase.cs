using Catarinense.Application.Common;
using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public interface ISinalizarCortinasUseCase
{
    Task<LimpezaFinaDetalhesDto> ExecutarAsync(Guid limpezaFinaId, bool retiradas);
}

public class SinalizarCortinasUseCase : ISinalizarCortinasUseCase
{
    private readonly ILimpezaFinaRepository _limpezaFinaRepository;
    private readonly DetalhesDtoBuilder _detalhesDtoBuilder;

    public SinalizarCortinasUseCase(
        ILimpezaFinaRepository limpezaFinaRepository,
        DetalhesDtoBuilder detalhesDtoBuilder)
    {
        _limpezaFinaRepository = limpezaFinaRepository;
        _detalhesDtoBuilder = detalhesDtoBuilder;
    }

    public async Task<LimpezaFinaDetalhesDto> ExecutarAsync(Guid limpezaFinaId, bool retiradas)
    {
        var limpeza = await _limpezaFinaRepository.ObterComEtapasAsync(limpezaFinaId)
            ?? throw new NotFoundException("Registro não encontrado.");

        limpeza.SinalizarCortinas(retiradas);

        _limpezaFinaRepository.Atualizar(limpeza);
        await _limpezaFinaRepository.SalvarAlteracoesAsync();

        return await _detalhesDtoBuilder.ConstruirAsync(limpeza);
    }
}

