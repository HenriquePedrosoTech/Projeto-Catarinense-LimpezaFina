using Catarinense.Application.DTOs;
using Catarinense.Application.Interfaces.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catarinense.API.Controllers;

[ApiController]
[Route("api/usuarios")]
[Authorize(Roles = "Administrador")]
public class UsuariosController : ControllerBase
{
    private readonly ICadastrarUsuarioUseCase _cadastrarUsuarioUseCase;
    private readonly IExcluirUsuarioUseCase _excluirUsuarioUseCase;
    private readonly IConsultarDadosMestresUseCase _consultarDadosMestresUseCase;

    public UsuariosController(
        ICadastrarUsuarioUseCase cadastrarUsuarioUseCase,
        IExcluirUsuarioUseCase excluirUsuarioUseCase,
        IConsultarDadosMestresUseCase consultarDadosMestresUseCase)
    {
        _cadastrarUsuarioUseCase = cadastrarUsuarioUseCase;
        _excluirUsuarioUseCase = excluirUsuarioUseCase;
        _consultarDadosMestresUseCase = consultarDadosMestresUseCase;
    }

    /// <summary>Cadastra um novo operador ou administrador. Somente administradores.</summary>
    [HttpPost]
    public async Task<ActionResult<UsuarioResumoDto>> Cadastrar([FromBody] CadastrarUsuarioRequest request)
    {
        var usuario = await _cadastrarUsuarioUseCase.ExecutarAsync(request);
        return CreatedAtAction(nameof(Listar), new { }, usuario);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UsuarioResumoDto>>> Listar()
    {
        var usuarios = await _consultarDadosMestresUseCase.ListarUsuariosAsync();
        return Ok(usuarios);
    }

    /// <summary>Exclui um usuário. Bloqueia se for o único admin ou se tiver limpeza vinculada. Uso: remover dado de teste.</summary>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        await _excluirUsuarioUseCase.ExecutarAsync(id);
        return NoContent();
    }
}
