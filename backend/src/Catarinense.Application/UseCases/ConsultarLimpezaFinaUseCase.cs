using Catarinense.Application.Common;
using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Enums;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class ConsultarLimpezaFinaUseCase : IConsultarLimpezaFinaUseCase
{
    private readonly ILimpezaFinaRepository _limpezaFinaRepository;
    private readonly IOnibusRepository _onibusRepository;
    private readonly DetalhesDtoBuilder _detalhesDtoBuilder;

    public ConsultarLimpezaFinaUseCase(
        ILimpezaFinaRepository limpezaFinaRepository,
        IOnibusRepository onibusRepository,
        DetalhesDtoBuilder detalhesDtoBuilder)
    {
        _limpezaFinaRepository = limpezaFinaRepository;
        _onibusRepository = onibusRepository;
        _detalhesDtoBuilder = detalhesDtoBuilder;
    }

    public async Task<LimpezaFinaDetalhesDto> ObterDetalhesAsync(Guid limpezaFinaId)
    {
        var limpeza = await _limpezaFinaRepository.ObterComEtapasAsync(limpezaFinaId)
            ?? throw new NotFoundException("Registro de limpeza fina não encontrado.");

        return await _detalhesDtoBuilder.ConstruirAsync(limpeza);
    }

    public async Task<IReadOnlyList<LimpezaFinaResumoDto>> ListarPorOnibusAsync(string prefixo)
    {
        var onibus = await _onibusRepository.ObterPorPrefixoAsync(prefixo)
            ?? throw new NotFoundException($"Ônibus de prefixo '{prefixo}' não encontrado.");

        var registros = await _limpezaFinaRepository.ListarPorOnibusAsync(onibus.Id);
        return await MapearListaAsync(registros);
    }

    public async Task<IReadOnlyList<LimpezaFinaResumoDto>> ListarPorOperadorAsync(Guid operadorId)
    {
        var registros = await _limpezaFinaRepository.ListarPorOperadorAsync(operadorId);
        return await MapearListaAsync(registros);
    }

    public async Task<IReadOnlyList<LimpezaFinaResumoDto>> ListarPorStatusAsync(string status)
    {
        if (!Enum.TryParse<StatusLimpeza>(status, ignoreCase: true, out var statusEnum))
            throw new NotFoundException($"Status '{status}' inválido.");

        var registros = await _limpezaFinaRepository.ListarPorStatusAsync(statusEnum);
        return await MapearListaAsync(registros);
    }

    public async Task<IReadOnlyList<LimpezaFinaResumoDto>> ListarParaRelatorioAsync(DateTime? dataInicio, DateTime? dataFim, string? status, Guid? operadorId)
    {
        StatusLimpeza? statusEnum = null;
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<StatusLimpeza>(status, ignoreCase: true, out var parsedStatus))
                throw new NotFoundException($"Status '{status}' inválido.");
            statusEnum = parsedStatus;
        }

        var registros = await _limpezaFinaRepository.ListarParaRelatorioAsync(dataInicio, dataFim, statusEnum, operadorId);
        return await MapearListaAsync(registros);
    }

    private async Task<IReadOnlyList<LimpezaFinaResumoDto>> MapearListaAsync(IReadOnlyList<Domain.Entities.LimpezaFina> registros)
    {
        var resultado = new List<LimpezaFinaResumoDto>(registros.Count);
        foreach (var registro in registros)
            resultado.Add(await _detalhesDtoBuilder.ConstruirResumoAsync(registro));

        return resultado;
    }
}
