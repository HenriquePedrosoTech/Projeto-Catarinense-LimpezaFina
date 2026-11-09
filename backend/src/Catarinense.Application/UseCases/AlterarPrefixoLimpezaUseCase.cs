using Catarinense.Application.Common;
using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Domain.Exceptions;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public interface IAlterarPrefixoLimpezaUseCase
{
    Task<LimpezaFinaDetalhesDto> ExecutarAsync(Guid limpezaFinaId, string novoPrefixo);
}

public class AlterarPrefixoLimpezaUseCase : IAlterarPrefixoLimpezaUseCase
{
    private readonly ILimpezaFinaRepository _limpezaFinaRepository;
    private readonly IOnibusRepository _onibusRepository;
    private readonly DetalhesDtoBuilder _detalhesDtoBuilder;

    public AlterarPrefixoLimpezaUseCase(
        ILimpezaFinaRepository limpezaFinaRepository,
        IOnibusRepository onibusRepository,
        DetalhesDtoBuilder detalhesDtoBuilder)
    {
        _limpezaFinaRepository = limpezaFinaRepository;
        _onibusRepository = onibusRepository;
        _detalhesDtoBuilder = detalhesDtoBuilder;
    }

    public async Task<LimpezaFinaDetalhesDto> ExecutarAsync(Guid limpezaFinaId, string novoPrefixo)
    {
        if (string.IsNullOrWhiteSpace(novoPrefixo))
            throw new DomainException("Informe o novo prefixo do ônibus.");

        var prefixoNormalizado = novoPrefixo.Trim().ToUpperInvariant();

        var limpeza = await _limpezaFinaRepository.ObterComEtapasAsync(limpezaFinaId)
            ?? throw new NotFoundException("Registro de limpeza fina não encontrado.");

        var novoOnibus = await _onibusRepository.ObterPorPrefixoAsync(prefixoNormalizado)
            ?? throw new NotFoundException($"Ônibus de prefixo '{prefixoNormalizado}' não encontrado na frota.");

        limpeza.AlterarOnibus(novoOnibus.Id);

        _limpezaFinaRepository.Atualizar(limpeza);
        await _limpezaFinaRepository.SalvarAlteracoesAsync();

        return await _detalhesDtoBuilder.ConstruirAsync(limpeza);
    }
}
