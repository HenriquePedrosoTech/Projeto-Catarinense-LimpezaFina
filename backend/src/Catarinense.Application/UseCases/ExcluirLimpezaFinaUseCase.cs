using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class ExcluirLimpezaFinaUseCase : IExcluirLimpezaFinaUseCase
{
    private readonly ILimpezaFinaRepository _limpezaFinaRepository;
    private readonly IArmazenamentoArquivoService _armazenamentoArquivoService;

    public ExcluirLimpezaFinaUseCase(
        ILimpezaFinaRepository limpezaFinaRepository,
        IArmazenamentoArquivoService armazenamentoArquivoService)
    {
        _limpezaFinaRepository = limpezaFinaRepository;
        _armazenamentoArquivoService = armazenamentoArquivoService;
    }

    public async Task ExecutarAsync(Guid limpezaFinaId)
    {
        var limpeza = await _limpezaFinaRepository.ObterComEtapasAsync(limpezaFinaId)
            ?? throw new NotFoundException("Registro de limpeza fina não encontrado.");

        // Apaga as fotos do disco antes de excluir o registro do banco — senão
        // ficam arquivos órfãos ocupando espaço, sem nenhum registro apontando pra eles.
        foreach (var etapa in limpeza.Etapas)
        {
            foreach (var foto in etapa.Fotos)
            {
                await _armazenamentoArquivoService.ExcluirFotoAsync(foto.UrlArquivo);
            }
        }

        // As etapas e fotos no banco têm DeleteBehavior.Cascade configurado no EF —
        // remover o registro principal já remove tudo que pertence a ele.
        _limpezaFinaRepository.Remover(limpeza);
        await _limpezaFinaRepository.SalvarAlteracoesAsync();
    }
}
