using Catarinense.Application.DTOs;
using Catarinense.Application.Interfaces.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Catarinense.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAutenticarUsuarioUseCase _autenticarUsuarioUseCase;

    public AuthController(IAutenticarUsuarioUseCase autenticarUsuarioUseCase)
    {
        _autenticarUsuarioUseCase = autenticarUsuarioUseCase;
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

    /// <summary>Troca a senha provisria no primeiro acesso ou se o admin resetar.</summary>
    [HttpPost("trocar-senha")]
    [Authorize]
    public async Task<IActionResult> TrocarSenha([FromBody] TrocarSenhaRequest request, [FromServices] ITrocarSenhaUsuarioUseCase trocarSenhaUseCase)
    {
        var usuarioIdClaim = User.Claims.FirstOrDefault(c => c.Type == "usuarioId")?.Value;
        if (!Guid.TryParse(usuarioIdClaim, out var usuarioId))
            return Unauthorized("Token inválido.");

        await trocarSenhaUseCase.ExecutarAsync(usuarioId, request);
        return Ok(new { mensagem = "Senha alterada com sucesso. Você já pode usar o sistema." });
    }
}
