using Catarinense.Application.Common;
using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Enums;
using Catarinense.Domain.Exceptions;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class RegistrarItemExecucaoUseCase : IRegistrarItemExecucaoUseCase
{
    private readonly ILimpezaFinaRepository _limpezaFinaRepository;
    private readonly IOnibusRepository _onibusRepository;
    private readonly IArmazenamentoArquivoService _armazenamentoArquivoService;
    private readonly DetalhesDtoBuilder _detalhesDtoBuilder;

    public RegistrarItemExecucaoUseCase(
        ILimpezaFinaRepository limpezaFinaRepository,
        IOnibusRepository onibusRepository,
        IArmazenamentoArquivoService armazenamentoArquivoService,
        DetalhesDtoBuilder detalhesDtoBuilder)
    {
        _limpezaFinaRepository = limpezaFinaRepository;
        _onibusRepository = onibusRepository;
        _armazenamentoArquivoService = armazenamentoArquivoService;
        _detalhesDtoBuilder = detalhesDtoBuilder;
    }

    public async Task<LimpezaFinaDetalhesDto> ExecutarAsync(RegistrarItemExecucaoRequest request)
    {
        var limpeza = await _limpezaFinaRepository.ObterComEtapasAsync(request.LimpezaFinaId)
            ?? throw new NotFoundException("Registro de limpeza fina não encontrado.");

        var onibus = await _onibusRepository.ObterPorIdAsync(limpeza.OnibusId);
        var prefixo = onibus?.Prefixo ?? "desconhecido";

        if (!Enum.TryParse<StatusItemChecklist>(request.Status, true, out var statusEnum))
            throw new DomainException("Status inválido.");
            
        if (!Enum.TryParse<StatusFuncionalidade>(request.Funcionalidade, true, out var funcEnum))
            throw new DomainException("Funcionalidade inválida.");

        string? urlFoto = null;
        if (request.ConteudoArquivo != null && request.ConteudoArquivo.Length > 0)
        {
            urlFoto = await _armazenamentoArquivoService.SalvarFotoAsync(
                request.ConteudoArquivo, request.NomeArquivoOriginal, request.ContentType, prefixo);
        }

        limpeza.RegistrarItemExecucao(request.EtapaPadraoId, request.EtapaItemPadraoId, statusEnum, funcEnum, request.RelatoProblema, urlFoto);

        _limpezaFinaRepository.Atualizar(limpeza);
        await _limpezaFinaRepository.SalvarAlteracoesAsync();

        return await _detalhesDtoBuilder.ConstruirAsync(limpeza);
    }
}
