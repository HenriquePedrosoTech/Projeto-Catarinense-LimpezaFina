using Catarinense.Domain.Entities;
using Catarinense.Domain.Interfaces;
using Catarinense.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catarinense.Infrastructure.Repositories;

public class UsuarioRepository : RepositorioBase<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(AppDbContext contexto) : base(contexto) { }

    public async Task<Usuario?> ObterPorMatriculaAsync(string matricula) =>
        await DbSet.FirstOrDefaultAsync(u => u.Matricula == matricula.Trim());
}
