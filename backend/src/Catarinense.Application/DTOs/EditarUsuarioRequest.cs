using System;
namespace Catarinense.Application.DTOs;
public record EditarUsuarioRequest(Guid Id, string Nome, string? Email, string? Senha, string? Matricula = null, string? LoginAcesso = null);
