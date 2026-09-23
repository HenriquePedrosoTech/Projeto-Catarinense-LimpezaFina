using Catarinense.Application.DTOs;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Entities;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class CadastrarEtapaPadraoUseCase : ICadastrarEtapaPadraoUseCase
{
    private readonly IEtapaPadraoRepository _etapaPadraoRepository;

    public CadastrarEtapaPadraoUseCase(IEtapaPadraoRepository etapaPadraoRepository)
    {
        _etapaPadraoRepository = etapaPadraoRepository;
    }

    public async Task<EtapaPadraoResumoDto> ExecutarAsync(CadastrarEtapaPadraoRequest request)
    {
        var etapa = new EtapaPadrao(request.Nome, request.Ordem, request.Descricao, request.LinkVideo);

        await _etapaPadraoRepository.AdicionarAsync(etapa);
        await _etapaPadraoRepository.SalvarAlteracoesAsync();

        return new EtapaPadraoResumoDto(etapa.Id, etapa.Nome, etapa.Descricao, etapa.LinkVideo, etapa.Ordem, etapa.Ativo);
    }
}