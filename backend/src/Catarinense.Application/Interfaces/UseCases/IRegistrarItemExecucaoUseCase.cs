using Catarinense.Application.DTOs;

namespace Catarinense.Application.Interfaces.UseCases;

public interface IRegistrarItemExecucaoUseCase
{
    Task<LimpezaFinaDetalhesDto> ExecutarAsync(Guid limpezaFinaId, Guid etapaPadraoId, Guid itemId, string status, string funcionalidade, string? relatoProblema, byte[]? fotoBytes, string? fotoNome, string? fotoContentType);
}
