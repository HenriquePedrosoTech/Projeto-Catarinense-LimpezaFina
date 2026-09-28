using Catarinense.Application.Common;
using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Enums;
using Catarinense.Domain.Interfaces;
using Catarinense.Domain.Exceptions;

namespace Catarinense.Application.UseCases;

public class RegistrarItemExecucaoUseCase : IRegistrarItemExecucaoUseCase
{
    private readonly ILimpezaFinaRepository _limpezaFinaRepository;
    private readonly IOnibusRepository _onibusRepository;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IEtapaPadraoRepository _etapaPadraoRepository;

    public RegistrarItemExecucaoUseCase(
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

    public async Task<LimpezaFinaDetalhesDto> ExecutarAsync(Guid limpezaFinaId, Guid etapaPadraoId, Guid itemId, string status, string funcionalidade, string? relatoProblema, string? fotoUrl)
    {
        var limpeza = await _limpezaFinaRepository.ObterComEtapasAsync(limpezaFinaId)
            ?? throw new NotFoundException("Limpeza fina nao encontrada.");

        var etapaExecucao = limpeza.Etapas.FirstOrDefault(e => e.EtapaPadraoId == etapaPadraoId)
            ?? throw new NotFoundException("Etapa nao encontrada na limpeza atual.");

        var itemExecucao = etapaExecucao.Itens.FirstOrDefault(i => i.Id == itemId)
            ?? throw new NotFoundException("Item de checklist nao encontrado nesta etapa.");

        if (!Enum.TryParse<StatusItemChecklist>(status, out var statusEnum))
            throw new DomainException("Status do item invalido.");
            
        var funcEnumStr = funcionalidade == "Com Defeito" ? "ComDefeito" : (funcionalidade == "OK / Funcional" || funcionalidade == "OK" ? "Ok" : funcionalidade);

        if (!Enum.TryParse<StatusFuncionalidade>(funcEnumStr, out var funcEnum))
            throw new DomainException("Funcionalidade do item invalida.");

        itemExecucao.RegistrarExecucao(statusEnum, funcEnum, relatoProblema, fotoUrl);
        etapaExecucao.VerificarConclusao();

        _limpezaFinaRepository.Atualizar(limpeza);
        await _limpezaFinaRepository.SalvarAlteracoesAsync();

        var onibus = await _onibusRepository.ObterPorIdAsync(limpeza.OnibusId);
        var operador = await _usuarioRepository.ObterPorIdAsync(limpeza.OperadorId);
        var etapasPadraoDict = await LimpezaFinaMapper.CarregarEtapasPadraoAsync(_etapaPadraoRepository, limpeza);

        return LimpezaFinaMapper.ParaDetalhesDto(limpeza, onibus!, operador!, etapasPadraoDict);
    }
}
