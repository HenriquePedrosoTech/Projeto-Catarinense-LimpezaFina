using Catarinense.Application.Common;
using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

/// <summary>
/// Preenche o número da O.S. de um registro já iniciado. Existe porque a abertura
/// da O.S. acontece no Protheus (sem integração automática por enquanto) — o
/// operador só informa o prefixo do ônibus, e o administrador confere no Protheus
/// e preenche o número aqui, antes de aprovar.
/// </summary>
public class DefinirNumeroOSUseCase : IDefinirNumeroOSUseCase
{
    private readonly ILimpezaFinaRepository _limpezaFinaRepository;
    private readonly DetalhesDtoBuilder _detalhesDtoBuilder;

    public DefinirNumeroOSUseCase(ILimpezaFinaRepository limpezaFinaRepository, DetalhesDtoBuilder detalhesDtoBuilder)
    {
        _limpezaFinaRepository = limpezaFinaRepository;
        _detalhesDtoBuilder = detalhesDtoBuilder;
    }

    public async Task<LimpezaFinaDetalhesDto> ExecutarAsync(DefinirNumeroOSRequest request)
    {
        var limpeza = await _limpezaFinaRepository.ObterComEtapasAsync(request.LimpezaFinaId)
            ?? throw new NotFoundException("Registro de limpeza fina não encontrado.");

        limpeza.DefinirNumeroOS(request.NumeroOS);

        _limpezaFinaRepository.Atualizar(limpeza);
        await _limpezaFinaRepository.SalvarAlteracoesAsync();

        return await _detalhesDtoBuilder.ConstruirAsync(limpeza);
    }
}
