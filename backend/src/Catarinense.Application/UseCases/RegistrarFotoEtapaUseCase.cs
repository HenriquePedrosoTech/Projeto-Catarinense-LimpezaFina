using Catarinense.Application.Common;
using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

/// <summary>
/// Faz upload da foto (via IArmazenamentoArquivoService) e registra a evidência
/// na etapa correspondente do registro de limpeza fina.
/// </summary>
public class RegistrarFotoEtapaUseCase : IRegistrarFotoEtapaUseCase
{
    private readonly ILimpezaFinaRepository _limpezaFinaRepository;
    private readonly IArmazenamentoArquivoService _armazenamentoArquivoService;
    private readonly DetalhesDtoBuilder _detalhesDtoBuilder;

    public RegistrarFotoEtapaUseCase(
        ILimpezaFinaRepository limpezaFinaRepository,
        IArmazenamentoArquivoService armazenamentoArquivoService,
        DetalhesDtoBuilder detalhesDtoBuilder)
    {
        _limpezaFinaRepository = limpezaFinaRepository;
        _armazenamentoArquivoService = armazenamentoArquivoService;
        _detalhesDtoBuilder = detalhesDtoBuilder;
    }

    public async Task<LimpezaFinaDetalhesDto> ExecutarAsync(RegistrarFotoEtapaRequest request)
    {
        var limpeza = await _limpezaFinaRepository.ObterComEtapasAsync(request.LimpezaFinaId)
            ?? throw new NotFoundException("Registro de limpeza fina não encontrado.");

        var urlFoto = await _armazenamentoArquivoService.SalvarFotoAsync(
            request.ConteudoArquivo, request.NomeArquivoOriginal, request.ContentType);

        // Regra de negócio (etapa pertence ao checklist, registro em andamento) é validada na própria entidade.
        limpeza.RegistrarFotoDaEtapa(request.EtapaPadraoId, urlFoto);

        _limpezaFinaRepository.Atualizar(limpeza);
        await _limpezaFinaRepository.SalvarAlteracoesAsync();

        return await _detalhesDtoBuilder.ConstruirAsync(limpeza);
    }
}
