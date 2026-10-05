using Catarinense.Application.DTOs;
using Catarinense.Application.Interfaces.UseCases;

namespace Catarinense.Application.Interfaces.UseCases;

public interface IReordenarEtapasPadraoUseCase
{
    Task ExecutarAsync(IEnumerable<ReordenarEtapaPadraoRequest> request);
}
