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
    private readonly IEditarUsuarioUseCase _editarUseCase;
    private readonly IConsultarDadosMestresUseCase _consultarDadosMestresUseCase;

    public UsuariosController(
        ICadastrarUsuarioUseCase cadastrarUsuarioUseCase,
        IExcluirUsuarioUseCase excluirUsuarioUseCase, IEditarUsuarioUseCase editarUseCase,
        IConsultarDadosMestresUseCase consultarDadosMestresUseCase)
    {
        _cadastrarUsuarioUseCase = cadastrarUsuarioUseCase;
        _excluirUsuarioUseCase = excluirUsuarioUseCase;
        _editarUseCase = editarUseCase;
        _consultarDadosMestresUseCase = consultarDadosMestresUseCase;
    }

    /// <summary>Cadastra um novo operador ou administrador. Somente Administrador Master.</summary>
    [HttpPost]
    [Authorize(Roles = "AdminMaster")]
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

    /// <summary>Edita ou exclui um usuário. Somente Administrador Master.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "AdminMaster")]
    public async Task<ActionResult<UsuarioResumoDto>> Editar(Guid id, [FromBody] EditarUsuarioRequest request)
    {
        if (id != request.Id) return BadRequest("ID da rota difere do corpo.");
        var resultado = await _editarUseCase.ExecutarAsync(request);
        return Ok(resultado);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "AdminMaster")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        await _excluirUsuarioUseCase.ExecutarAsync(id);
        return NoContent();
    }
}


