using Catarinense.Application.Common;
using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class RemoverFotoEtapaUseCase : IRemoverFotoEtapaUseCase
{
    private readonly ILimpezaFinaRepository _limpezaFinaRepository;
    private readonly IArmazenamentoArquivoService _armazenamentoService;
    private readonly DetalhesDtoBuilder _detalhesDtoBuilder;

    public RemoverFotoEtapaUseCase(
        ILimpezaFinaRepository limpezaFinaRepository,
        IArmazenamentoArquivoService armazenamentoService,
        DetalhesDtoBuilder detalhesDtoBuilder)
    {
        _limpezaFinaRepository = limpezaFinaRepository;
        _armazenamentoService = armazenamentoService;
        _detalhesDtoBuilder = detalhesDtoBuilder;
    }

    public async Task<LimpezaFinaDetalhesDto> ExecutarAsync(RemoverFotoEtapaRequest request)
    {
        var limpeza = await _limpezaFinaRepository.ObterComEtapasAsync(request.LimpezaFinaId)
            ?? throw new NotFoundException("Registro de limpeza fina não encontrado.");

        limpeza.RemoverFotoDaEtapa(request.EtapaPadraoId, request.UrlArquivo);

        // Se a entidade validou a remoção sem erros, vamos excluir o arquivo físico.
        // Nosso Storage salva com o formato /uploads/nome-do-arquivo.jpg
        var fileName = Path.GetFileName(request.UrlArquivo);
        if (!string.IsNullOrEmpty(fileName))
        {
            await _armazenamentoService.ExcluirFotoAsync(fileName);
        }

        _limpezaFinaRepository.Atualizar(limpeza);
        await _limpezaFinaRepository.SalvarAlteracoesAsync();

        return await _detalhesDtoBuilder.ConstruirAsync(limpeza);
    }
}

