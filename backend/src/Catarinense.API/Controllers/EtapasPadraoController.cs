using Catarinense.Application.DTOs;
using Catarinense.Application.Interfaces.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catarinense.API.Controllers;

[ApiController]
[Route("api/etapas-padrao")]
[Authorize]
public class EtapasPadraoController : ControllerBase
{
    private readonly ICadastrarEtapaPadraoUseCase _cadastrarEtapaPadraoUseCase;
    private readonly IExcluirEtapaPadraoUseCase _excluirEtapaPadraoUseCase;
    private readonly IConsultarDadosMestresUseCase _consultarDadosMestresUseCase;

    public EtapasPadraoController(
        ICadastrarEtapaPadraoUseCase cadastrarEtapaPadraoUseCase,
        IExcluirEtapaPadraoUseCase excluirEtapaPadraoUseCase,
        IConsultarDadosMestresUseCase consultarDadosMestresUseCase)
    {
        _cadastrarEtapaPadraoUseCase = cadastrarEtapaPadraoUseCase;
        _excluirEtapaPadraoUseCase = excluirEtapaPadraoUseCase;
        _consultarDadosMestresUseCase = consultarDadosMestresUseCase;
    }

    /// <summary>Cadastra uma etapa do checklist de limpeza fina (ex.: "Bancos", "Piso"). Somente administradores.</summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<EtapaPadraoResumoDto>> Cadastrar([FromBody] CadastrarEtapaPadraoRequest request)
    {
        var etapa = await _cadastrarEtapaPadraoUseCase.ExecutarAsync(request);
        return CreatedAtAction(nameof(Listar), new { }, etapa);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EtapaPadraoResumoDto>>> Listar()
    {
        var etapas = await _consultarDadosMestresUseCase.ListarEtapasPadraoAsync();
        return Ok(etapas);
    }

    /// <summary>Edita uma etapa existente. Somente administradores.</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<EtapaPadraoResumoDto>> Editar(Guid id, [FromBody] CadastrarEtapaPadraoRequest request)
    {
        var useCase = HttpContext.RequestServices.GetRequiredService<IEditarEtapaPadraoUseCase>();
        var editRequest = new EditarEtapaPadraoRequest(id, request.Nome, request.Ordem, request.Descricao, request.LinkVideo);
        var etapa = await useCase.ExecutarAsync(editRequest);
        return Ok(etapa);
    }

    /// <summary>Exclui uma etapa do checklist. Uso: remover dado de teste.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        await _excluirEtapaPadraoUseCase.ExecutarAsync(id);
        return NoContent();
    }
}
