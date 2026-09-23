using Catarinense.Application.Common;
using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Entities;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

/// <summary>
/// Inicia um novo registro de limpeza fina para um ônibus, já criando as
/// execuções pendentes de cada etapa padrão ativa (checklist obrigatório).
/// </summary>
public class IniciarLimpezaFinaUseCase : IIniciarLimpezaFinaUseCase
{
    private readonly IOnibusRepository _onibusRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEtapaPadraoRepository _etapaPadraoRepository;
    private readonly ILimpezaFinaRepository _limpezaFinaRepository;

    public IniciarLimpezaFinaUseCase(
        IOnibusRepository onibusRepository,
        IUsuarioRepository usuarioRepository,
        IEtapaPadraoRepository etapaPadraoRepository,
        ILimpezaFinaRepository limpezaFinaRepository)
    {
        _onibusRepository = onibusRepository;
        _usuarioRepository = usuarioRepository;
        _etapaPadraoRepository = etapaPadraoRepository;
        _limpezaFinaRepository = limpezaFinaRepository;
    }

    public async Task<LimpezaFinaDetalhesDto> ExecutarAsync(IniciarLimpezaFinaRequest request)
    {
        var onibus = await _onibusRepository.ObterPorPrefixoAsync(request.PrefixoOnibus)
            ?? throw new NotFoundException($"Ônibus de prefixo '{request.PrefixoOnibus}' não encontrado.");

        var operador = await _usuarioRepository.ObterPorIdAsync(request.OperadorId)
            ?? throw new NotFoundException("Operador não encontrado.");

        var etapasAtivas = await _etapaPadraoRepository.ListarAtivasAsync();

        var limpeza = new LimpezaFina(
            onibus.Id,
            operador.Id,
            etapasAtivas.Select(e => e.Id)
        );

        await _limpezaFinaRepository.AdicionarAsync(limpeza);
        await _limpezaFinaRepository.SalvarAlteracoesAsync();

        var etapasPadraoDict = etapasAtivas.ToDictionary(e => e.Id);
        return LimpezaFinaMapper.ParaDetalhesDto(limpeza, onibus, operador, etapasPadraoDict);
    }
}
