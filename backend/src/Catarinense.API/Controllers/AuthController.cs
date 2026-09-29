using Catarinense.Application.DTOs;
using Catarinense.Application.Interfaces.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace Catarinense.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAutenticarUsuarioUseCase _autenticarUsuarioUseCase;
    private readonly ITrocarSenhaUsuarioUseCase _trocarSenhaUsuarioUseCase;

    public AuthController(IAutenticarUsuarioUseCase autenticarUsuarioUseCase, ITrocarSenhaUsuarioUseCase trocarSenhaUsuarioUseCase)
    {
        _autenticarUsuarioUseCase = autenticarUsuarioUseCase;
        _trocarSenhaUsuarioUseCase = trocarSenhaUsuarioUseCase;
    }

    /// <summary>Login por matrícula + senha. Retorna o token JWT usado nas demais rotas.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var resposta = await _autenticarUsuarioUseCase.ExecutarAsync(request);
        return Ok(resposta);
    }

    [HttpPost("trocar-senha")]
    [Authorize]
    public async Task<ActionResult> TrocarSenha([FromBody] TrocarSenhaRequest request)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier) ?? User.FindFirst(JwtRegisteredClaimNames.Sub) ?? User.FindFirst("UsuarioId");
        var usuarioIdClaim = claim?.Value;
        
        if (string.IsNullOrEmpty(usuarioIdClaim) || !Guid.TryParse(usuarioIdClaim, out var usuarioId))
            return Unauthorized(new { erro = "Token inválido." });

        await _trocarSenhaUsuarioUseCase.ExecutarAsync(usuarioId, request);
        return Ok(new { mensagem = "Senha alterada com sucesso." });
    }
}
