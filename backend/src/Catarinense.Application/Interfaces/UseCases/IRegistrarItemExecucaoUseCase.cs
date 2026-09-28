using Catarinense.Application.DTOs;

namespace Catarinense.Application.Interfaces.UseCases;

public interface IRegistrarItemExecucaoUseCase
{
    Task<LimpezaFinaDetalhesDto> ExecutarAsync(RegistrarItemExecucaoRequest request);
}
