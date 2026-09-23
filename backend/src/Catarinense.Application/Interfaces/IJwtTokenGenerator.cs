using Catarinense.Domain.Entities;

namespace Catarinense.Application.Interfaces;

/// <summary>
/// Abstrai a geração do token de autenticação. A Infrastructure implementa (JWT).
/// </summary>
public interface IJwtTokenGenerator
{
    string GerarToken(Usuario usuario);
}
