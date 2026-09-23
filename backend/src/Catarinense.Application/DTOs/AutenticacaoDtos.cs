namespace Catarinense.Application.DTOs;

public record LoginRequest(string Matricula, string Senha);

public record LoginResponse(Guid UsuarioId, string Nome, string Matricula, string Perfil, string Token);
