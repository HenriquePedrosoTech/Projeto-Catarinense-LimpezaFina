using Catarinense.Application.DTOs;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class ReordenarEtapasPadraoUseCase : IReordenarEtapasPadraoUseCase
{
    private readonly IEtapaPadraoRepository _etapaPadraoRepository;

    public ReordenarEtapasPadraoUseCase(IEtapaPadraoRepository etapaPadraoRepository)
    {
        _etapaPadraoRepository = etapaPadraoRepository;
    }

    public async Task ExecutarAsync(IEnumerable<ReordenarEtapaPadraoRequest> request)
    {
        var todasEtapas = await _etapaPadraoRepository.ListarAtivasAsync();
        foreach (var r in request)
        {
            var etapa = todasEtapas.FirstOrDefault(e => e.Id == r.Id);
            if (etapa != null)
            {
                etapa.RenomearOuReordenar(etapa.Nome, r.Ordem, etapa.Descricao, etapa.LinkVideo, etapa.Obrigatoria);
                _etapaPadraoRepository.Atualizar(etapa);
            }
        }
        await _etapaPadraoRepository.SalvarAlteracoesAsync();
    }
}
