using Catarinense.Application.DTOs;
using Catarinense.Application.Interfaces.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
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
    
    [HttpPost("trocar-senha")]
    [Authorize]
    public async Task<IActionResult> TrocarSenha([FromBody] TrocarSenhaRequest request, [FromServices] ITrocarSenhaUsuarioUseCase useCase)
    {
        var usuarioIdStr = User.FindFirstValue("usuarioId");
        if (!Guid.TryParse(usuarioIdStr, out var usuarioId))
            return Unauthorized();
            
        await useCase.ExecutarAsync(usuarioId, request);
        return NoContent();
    }
}

    /// <summary>Login por matrícula + senha. Retorna o token JWT usado nas demais rotas.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var resposta = await _autenticarUsuarioUseCase.ExecutarAsync(request);
        return Ok(resposta);
    
    [HttpPost("trocar-senha")]
    [Authorize]
    public async Task<IActionResult> TrocarSenha([FromBody] TrocarSenhaRequest request, [FromServices] ITrocarSenhaUsuarioUseCase useCase)
    {
        var usuarioIdStr = User.FindFirstValue("usuarioId");
        if (!Guid.TryParse(usuarioIdStr, out var usuarioId))
            return Unauthorized();
            
        await useCase.ExecutarAsync(usuarioId, request);
        return NoContent();
    }
}

    [HttpPost("trocar-senha")]
    [Authorize]
    public async Task<IActionResult> TrocarSenha([FromBody] TrocarSenhaRequest request, [FromServices] ITrocarSenhaUsuarioUseCase useCase)
    {
        var usuarioIdStr = User.FindFirstValue("usuarioId");
        if (!Guid.TryParse(usuarioIdStr, out var usuarioId))
            return Unauthorized();
            
        await useCase.ExecutarAsync(usuarioId, request);
        return NoContent();
    }
}

