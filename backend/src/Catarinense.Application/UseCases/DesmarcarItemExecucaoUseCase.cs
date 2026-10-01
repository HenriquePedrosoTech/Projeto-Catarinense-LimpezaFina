using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Interfaces;
using Catarinense.Domain.Entities;
using Catarinense.Application.Common;

namespace Catarinense.Application.UseCases;

public interface IDesmarcarItemExecucaoUseCase
{
    Task<LimpezaFinaDetalhesDto> ExecutarAsync(Guid limpezaFinaId, Guid etapaPadraoId, Guid itemId);
}

public class DesmarcarItemExecucaoUseCase : IDesmarcarItemExecucaoUseCase
{
    private readonly ILimpezaFinaRepository _limpezaFinaRepository;
    private readonly IOnibusRepository _onibusRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEtapaPadraoRepository _etapaPadraoRepository;

    public DesmarcarItemExecucaoUseCase(
        ILimpezaFinaRepository limpezaFinaRepository,
        IOnibusRepository onibusRepository,
        IUsuarioRepository usuarioRepository,
        IEtapaPadraoRepository etapaPadraoRepository)
    {
        _limpezaFinaRepository = limpezaFinaRepository;
        _onibusRepository = onibusRepository;
        _usuarioRepository = usuarioRepository;
        _etapaPadraoRepository = etapaPadraoRepository;
    }

    public async Task<LimpezaFinaDetalhesDto> ExecutarAsync(Guid limpezaFinaId, Guid etapaPadraoId, Guid itemId)
    {
        var limpeza = await _limpezaFinaRepository.ObterComEtapasAsync(limpezaFinaId)
            ?? throw new NotFoundException("Limpeza fina nao encontrada.");

        var etapaExecucao = limpeza.Etapas.FirstOrDefault(e => e.EtapaPadraoId == etapaPadraoId)
            ?? throw new NotFoundException("Etapa nao encontrada na limpeza atual.");

        var itemExecucao = etapaExecucao.Itens.FirstOrDefault(i => i.Id == itemId)
            ?? throw new NotFoundException("Item de checklist nao encontrado nesta etapa.");

        itemExecucao.DesfazerExecucao();
        etapaExecucao.VerificarConclusao();

        _limpezaFinaRepository.Atualizar(limpeza);
        await _limpezaFinaRepository.SalvarAlteracoesAsync();

        var onibus = await _onibusRepository.ObterPorIdAsync(limpeza.OnibusId);
        var operador = await _usuarioRepository.ObterPorIdAsync(limpeza.OperadorId);
        var etapasPadraoDict = await LimpezaFinaMapper.CarregarEtapasPadraoAsync(_etapaPadraoRepository, limpeza);

        return LimpezaFinaMapper.ParaDetalhesDto(limpeza, onibus!, operador!, etapasPadraoDict);
    }
}
