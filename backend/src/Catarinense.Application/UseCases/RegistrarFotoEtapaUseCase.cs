using System.Threading.Tasks;
using Catarinense.Application.Common;
using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class RegistrarFotoEtapaUseCase : IRegistrarFotoEtapaUseCase
{
    private readonly ILimpezaFinaRepository _limpezaFinaRepository;
    private readonly IOnibusRepository _onibusRepository;
    private readonly IArmazenamentoArquivoService _armazenamentoArquivoService;
    private readonly DetalhesDtoBuilder _detalhesDtoBuilder;

    public RegistrarFotoEtapaUseCase(
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

    public async Task<LimpezaFinaDetalhesDto> ExecutarAsync(RegistrarFotoEtapaRequest request)
    {
        var limpeza = await _limpezaFinaRepository.ObterComEtapasAsync(request.LimpezaFinaId)
            ?? throw new NotFoundException("Registro de limpeza fina nao encontrado.");

        var onibus = await _onibusRepository.ObterPorIdAsync(limpeza.OnibusId);
        var prefixo = onibus?.Prefixo ?? "desconhecido";

        var urlFoto = await _armazenamentoArquivoService.SalvarFotoAsync(
            request.ConteudoArquivo, request.NomeArquivoOriginal, request.ContentType, prefixo);

        limpeza.RegistrarFotoDaEtapa(request.EtapaPadraoId, urlFoto);

        _limpezaFinaRepository.Atualizar(limpeza);
        await _limpezaFinaRepository.SalvarAlteracoesAsync();

        return await _detalhesDtoBuilder.ConstruirAsync(limpeza);
    }
}