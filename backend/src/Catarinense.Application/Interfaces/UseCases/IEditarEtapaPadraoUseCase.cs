using Catarinense.Application.DTOs;

namespace Catarinense.Application.Interfaces.UseCases;

public interface IEditarEtapaPadraoUseCase
{
    Task<EtapaPadraoResumoDto> ExecutarAsync(EditarEtapaPadraoRequest request);
}

