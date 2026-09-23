using System.Threading.Tasks;
using Catarinense.Application.DTOs;

namespace Catarinense.Application.Interfaces.UseCases;

public interface IEditarUsuarioUseCase
{
    Task<UsuarioResumoDto> ExecutarAsync(EditarUsuarioRequest request);
}