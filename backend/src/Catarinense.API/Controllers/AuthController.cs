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
}
