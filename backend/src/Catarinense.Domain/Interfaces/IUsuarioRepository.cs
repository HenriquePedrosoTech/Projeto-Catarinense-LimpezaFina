using Catarinense.Domain.Entities;

namespace Catarinense.Domain.Interfaces;

public interface IUsuarioRepository : IRepositorioBase<Usuario>
{
    Task<Usuario?> ObterPorMatriculaAsync(string matricula);
}
