using System.Security.Claims;
using Catarinense.API.Contracts;
using Catarinense.Application.DTOs;
using Catarinense.Application.Interfaces.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catarinense.API.Controllers;

[ApiController]
[Route("api/limpezas-finas")]
[Authorize]
public class LimpezasFinasController : ControllerBase
{
    private readonly IIniciarLimpezaFinaUseCase _iniciarUseCase;
    private readonly IDefinirNumeroOSUseCase _definirNumeroOSUseCase;
    private readonly IRegistrarFotoEtapaUseCase _registrarFotoUseCase;
    private readonly IFinalizarLimpezaFinaUseCase _finalizarUseCase;
    private readonly IAprovarLimpezaFinaUseCase _aprovarUseCase;
    private readonly IReprovarLimpezaFinaUseCase _reprovarUseCase;
    private readonly IEnviarNotificacaoLimpezaUseCase _enviarNotificacaoUseCase;
    private readonly IConsultarLimpezaFinaUseCase _consultarUseCase;
    private readonly IExcluirLimpezaFinaUseCase _excluirUseCase;
    private readonly IConfiguration _configuration;

    public LimpezasFinasController(
        IIniciarLimpezaFinaUseCase iniciarUseCase,
        IDefinirNumeroOSUseCase definirNumeroOSUseCase,
        IRegistrarFotoEtapaUseCase registrarFotoUseCase,
        IFinalizarLimpezaFinaUseCase finalizarUseCase,
        IAprovarLimpezaFinaUseCase aprovarUseCase,
        IReprovarLimpezaFinaUseCase reprovarUseCase,
        IEnviarNotificacaoLimpezaUseCase enviarNotificacaoUseCase,
        IConsultarLimpezaFinaUseCase consultarUseCase,
        IExcluirLimpezaFinaUseCase excluirUseCase,
        IConfiguration configuration)
    {
        _iniciarUseCase = iniciarUseCase;
        _definirNumeroOSUseCase = definirNumeroOSUseCase;
        _registrarFotoUseCase = registrarFotoUseCase;
        _finalizarUseCase = finalizarUseCase;
        _aprovarUseCase = aprovarUseCase;
        _reprovarUseCase = reprovarUseCase;
        _enviarNotificacaoUseCase = enviarNotificacaoUseCase;
        _consultarUseCase = consultarUseCase;
        _excluirUseCase = excluirUseCase;
        _configuration = configuration;
    }

    /// <summary>
    /// Operador inicia um novo registro de limpeza fina informando sÃ³ o prefixo do
    /// Ã´nibus. A O.S. Ã© aberta no Protheus (sem integraÃ§Ã£o automÃ¡tica por enquanto)
    /// e Ã© preenchida depois pelo administrador â€” ver <see cref="DefinirNumeroOS"/>.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<LimpezaFinaDetalhesDto>> Iniciar([FromBody] IniciarLimpezaFinaBody body)
    {
        var request = new IniciarLimpezaFinaRequest(body.PrefixoOnibus, OperadorLogadoId());
        var resultado = await _iniciarUseCase.ExecutarAsync(request);
        return CreatedAtAction(nameof(ObterDetalhes), new { id = resultado.Id }, resultado);
    }

    /// <summary>
    /// Administrador preenche/atualiza o nÃºmero da O.S. (consultado manualmente no
    /// Protheus). ObrigatÃ³rio antes de aprovar o registro.
    /// </summary>
    [HttpPost("{id:guid}/os")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<LimpezaFinaDetalhesDto>> DefinirNumeroOS(Guid id, [FromBody] DefinirNumeroOSBody body)
    {
        var resultado = await _definirNumeroOSUseCase.ExecutarAsync(new DefinirNumeroOSRequest(id, body.NumeroOS));
        return Ok(resultado);
    }

    /// <summary>Envia a foto de evidÃªncia de uma etapa (multipart/form-data).</summary>
    [HttpPost("{id}/etapas/{etapaPadraoId}/itens/{itemId}")]
    [Authorize(Roles = "Operador,Administrador")]
    public async Task<ActionResult<LimpezaFinaDetalhesDto>> RegistrarItemExecucao(
        Guid id, Guid etapaPadraoId, Guid itemId,
        [FromForm] string status,
        [FromForm] string funcionalidade,
        [FromForm] string? relatoProblema,
        IFormFile? arquivo,
        [FromServices] IRegistrarItemExecucaoUseCase useCase)
    {
        byte[]? fotoBytes = null;
        string? fotoNome = null;
        string? fotoContentType = null;

        if (arquivo != null && arquivo.Length > 0)
        {
            using var memoryStream = new MemoryStream();
            await arquivo.CopyToAsync(memoryStream);
            fotoBytes = memoryStream.ToArray();
            fotoNome = arquivo.FileName;
            fotoContentType = arquivo.ContentType;
        }

        var resultado = await useCase.ExecutarAsync(
            id, etapaPadraoId, itemId, status, funcionalidade, relatoProblema, 
            fotoBytes, fotoNome, fotoContentType);
            
        return Ok(resultado);
    }

    [HttpPost("{id:guid}/etapas/{etapaPadraoId:guid}/foto")]
    [RequestSizeLimit(20_000_000)] // 20MB
    public async Task<ActionResult<LimpezaFinaDetalhesDto>> EnviarFotoEtapa(
        Guid id, Guid etapaPadraoId, [FromForm] EnviarFotoEtapaForm form)
    {
        if (form.Arquivo is null || form.Arquivo.Length == 0)
            return BadRequest(new { erro = "Nenhum arquivo enviado." });

        await using var stream = form.Arquivo.OpenReadStream();
        var request = new RegistrarFotoEtapaRequest(id, etapaPadraoId, stream, form.Arquivo.FileName, form.Arquivo.ContentType);
        var resultado = await _registrarFotoUseCase.ExecutarAsync(request);
        return Ok(resultado);
    }

    [HttpDelete("{id:guid}/etapas/{etapaPadraoId:guid}/foto")]
    public async Task<ActionResult<LimpezaFinaDetalhesDto>> RemoverFotoEtapa(
        Guid id, Guid etapaPadraoId, [FromQuery] string urlArquivo)
    {
        if (string.IsNullOrWhiteSpace(urlArquivo))
            return BadRequest(new { erro = "A URL da foto Ã© obrigatÃ³ria." });

        var useCase = HttpContext.RequestServices.GetRequiredService<IRemoverFotoEtapaUseCase>();
        var request = new RemoverFotoEtapaRequest(id, etapaPadraoId, urlArquivo);
        var resultado = await useCase.ExecutarAsync(request);
        return Ok(resultado);
    }

    [HttpPost("{id:guid}/cortinas")]
    public async Task<ActionResult<LimpezaFinaDetalhesDto>> SinalizarCortinas(
        Guid id, [FromQuery] bool retiradas)
    {
        var useCase = HttpContext.RequestServices.GetRequiredService<Catarinense.Application.UseCases.ISinalizarCortinasUseCase>();
        var resultado = await useCase.ExecutarAsync(id, retiradas);
        return Ok(resultado);
    }

    /// <summary>Operador finaliza o registro (sÃ³ permite se todas as etapas tiverem foto).</summary>
    [HttpPost("{id:guid}/finalizar")]
    public async Task<ActionResult<LimpezaFinaDetalhesDto>> Finalizar(Guid id)
    {
        var resultado = await _finalizarUseCase.ExecutarAsync(new FinalizarLimpezaFinaRequest(id));
        return Ok(resultado);
    }

    /// <summary>Administrador aprova o registro concluÃ­do.</summary>
    [HttpPost("{id:guid}/aprovar")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<LimpezaFinaDetalhesDto>> Aprovar(Guid id)
    {
        var resultado = await _aprovarUseCase.ExecutarAsync(new AprovarLimpezaFinaRequest(id, UsuarioLogadoId()));
        return Ok(resultado);
    }

    /// <summary>Administrador reprova o registro concluÃ­do, informando o motivo.</summary>
    [HttpPost("{id:guid}/reprovar")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<LimpezaFinaDetalhesDto>> Reprovar(Guid id, [FromBody] ReprovarLimpezaBody body)
    {
        var resultado = await _reprovarUseCase.ExecutarAsync(new ReprovarLimpezaFinaRequest(id, UsuarioLogadoId(), body.Motivo));
        return Ok(resultado);
    }

    /// <summary>
    /// BotÃ£o "Notificar": administrador dispara o e-mail avisando que a limpeza fina
    /// foi executada, com prefixo, O.S. e link para a pÃ¡gina com as fotos de cada etapa.
    /// </summary>
    [HttpPost("{id:guid}/notificar")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Notificar(Guid id, [FromBody] NotificarLimpezaBody body)
    {
        var urlBase = _configuration["Frontend:UrlBaseDetalhesLimpeza"]
            ?? "http://localhost:3000/limpezas";

        var request = new EnviarNotificacaoLimpezaRequest(id, body.DestinatariosEmail, urlBase);
        await _enviarNotificacaoUseCase.ExecutarAsync(request);
        return NoContent();
    }

    /// <summary>
    /// Detalhes completos de um registro, com as etapas e fotos â€” Ã© a pÃ¡gina que
    /// abre a partir do link enviado no e-mail de notificaÃ§Ã£o.
    /// </summary>
    [HttpGet("{id:guid}")]
    [AllowAnonymous] // acessÃ­vel pelo link do e-mail; o id Ã© um GUID nÃ£o sequencial
    public async Task<ActionResult<LimpezaFinaDetalhesDto>> ObterDetalhes(Guid id)
    {
        var resultado = await _consultarUseCase.ObterDetalhesAsync(id);
        return Ok(resultado);
    }

    /// <summary>Lista registros filtrando por operador (o prÃ³prio usuÃ¡rio logado) ou por status.</summary>
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<LimpezaFinaResumoDto>>> Listar(
        [FromQuery] Guid? operadorId, [FromQuery] string? status)
    {
        if (operadorId.HasValue)
            return Ok(await _consultarUseCase.ListarPorOperadorAsync(operadorId.Value));

        if (!string.IsNullOrWhiteSpace(status))
            return Ok(await _consultarUseCase.ListarPorStatusAsync(status));

        return BadRequest(new { erro = "Informe 'operadorId' ou 'status' para filtrar a listagem." });
    }

    [HttpGet("relatorio")]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<IReadOnlyList<LimpezaFinaResumoDto>>> Relatorio(
        [FromQuery] DateTime? dataInicio,
        [FromQuery] DateTime? dataFim,
        [FromQuery] string? status,
        [FromQuery] Guid? operadorId)
    {
        var resultado = await _consultarUseCase.ListarParaRelatorioAsync(dataInicio, dataFim, status, operadorId);
        return Ok(resultado);
    }

    /// <summary>Exclui um registro de limpeza fina (e suas fotos). Uso: remover dados de teste.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        await _excluirUseCase.ExecutarAsync(id);
        return NoContent();
    }

    private Guid OperadorLogadoId() => UsuarioLogadoId();

    private Guid UsuarioLogadoId()
    {
        var claim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
        return Guid.TryParse(claim, out var id) ? id : throw new UnauthorizedAccessException("Token invÃ¡lido.");
    }
}

public record IniciarLimpezaFinaBody(string PrefixoOnibus);
public record DefinirNumeroOSBody(string NumeroOS);



