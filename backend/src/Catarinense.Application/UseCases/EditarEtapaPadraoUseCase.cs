using Catarinense.Application.DTOs;
using Catarinense.Application.Exceptions;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class EditarEtapaPadraoUseCase : IEditarEtapaPadraoUseCase
{
    private readonly IEtapaPadraoRepository _etapaPadraoRepository;

    public EditarEtapaPadraoUseCase(IEtapaPadraoRepository etapaPadraoRepository)
    {
        _etapaPadraoRepository = etapaPadraoRepository;
    }

    public async Task<EtapaPadraoResumoDto> ExecutarAsync(EditarEtapaPadraoRequest request)
    {
        var etapa = await _etapaPadraoRepository.ObterPorIdAsync(request.Id)
            ?? throw new NotFoundException("Etapa padrao nao encontrada.");

        etapa.RenomearOuReordenar(request.Nome, request.Ordem, request.Descricao, request.LinkVideo);

        _etapaPadraoRepository.Atualizar(etapa);
        await _etapaPadraoRepository.SalvarAlteracoesAsync();

        return new EtapaPadraoResumoDto(etapa.Id, etapa.Nome, etapa.Descricao, etapa.LinkVideo, etapa.Ordem, etapa.Ativo);
    }
}