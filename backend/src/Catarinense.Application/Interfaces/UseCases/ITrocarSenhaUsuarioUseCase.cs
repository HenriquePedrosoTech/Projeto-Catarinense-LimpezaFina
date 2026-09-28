using Catarinense.Application.DTOs;

namespace Catarinense.Application.Interfaces.UseCases;

public interface ITrocarSenhaUsuarioUseCase
{
    Task ExecutarAsync(Guid usuarioId, TrocarSenhaRequest request);
}
