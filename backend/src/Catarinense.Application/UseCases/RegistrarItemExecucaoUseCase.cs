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
    private readonly IFotoHashRepository _fotoHashRepository;
    private readonly Catarinense.Application.Interfaces.IArmazenamentoArquivoService _armazenamentoArquivoService;

    public RegistrarItemExecucaoUseCase(
        ILimpezaFinaRepository limpezaFinaRepository,
        IOnibusRepository onibusRepository,
        IUsuarioRepository usuarioRepository,
        IEtapaPadraoRepository etapaPadraoRepository, IFotoHashRepository fotoHashRepository, Catarinense.Application.Interfaces.IArmazenamentoArquivoService armazenamentoArquivoService)
    {
        _limpezaFinaRepository = limpezaFinaRepository;
        _onibusRepository = onibusRepository;
        _usuarioRepository = usuarioRepository;
        _etapaPadraoRepository = etapaPadraoRepository;
        _fotoHashRepository = fotoHashRepository;
        _armazenamentoArquivoService = armazenamentoArquivoService;
    }

    public async Task<LimpezaFinaDetalhesDto> ExecutarAsync(Guid limpezaFinaId, Guid etapaPadraoId, Guid itemId, string status, string funcionalidade, string? relatoProblema, byte[]? fotoBytes, string? fotoNome, string? fotoContentType)
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

        string? fotoUrlFinal = null;
        if (fotoBytes != null && fotoBytes.Length > 0)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hashBytes = sha256.ComputeHash(fotoBytes);
            var hashStr = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();

            if (await _fotoHashRepository.ExisteHashAsync(hashStr))
                throw new DomainException("Esta foto ja foi enviada anteriormente. O reuso de fotos nao e permitido.");

            fotoUrlFinal = await _armazenamentoArquivoService.SalvarFotoAsync(new MemoryStream(fotoBytes), fotoNome!, fotoContentType!, "evidencia");
            await _fotoHashRepository.AdicionarAsync(new Catarinense.Domain.Entities.FotoHash(hashStr, fotoUrlFinal));
        }

        itemExecucao.RegistrarExecucao(statusEnum, funcEnum, relatoProblema, fotoUrlFinal);
        etapaExecucao.VerificarConclusao();

        _limpezaFinaRepository.Atualizar(limpeza);
        await _limpezaFinaRepository.SalvarAlteracoesAsync();

        var onibus = await _onibusRepository.ObterPorIdAsync(limpeza.OnibusId);
        var operador = await _usuarioRepository.ObterPorIdAsync(limpeza.OperadorId);
        var etapasPadraoDict = await LimpezaFinaMapper.CarregarEtapasPadraoAsync(_etapaPadraoRepository, limpeza);

        return LimpezaFinaMapper.ParaDetalhesDto(limpeza, onibus!, operador!, etapasPadraoDict);
    }
}

