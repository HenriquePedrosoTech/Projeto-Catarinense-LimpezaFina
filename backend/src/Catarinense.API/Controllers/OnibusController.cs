using Catarinense.API.Contracts;
using Catarinense.Application.DTOs;
using Catarinense.Application.Interfaces.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catarinense.API.Controllers;

[ApiController]
[Route("api/onibus")]
[Authorize]
public class OnibusController : ControllerBase
{
    private readonly ICadastrarOnibusUseCase _cadastrarOnibusUseCase;
    private readonly IExcluirOnibusUseCase _excluirOnibusUseCase;
    private readonly IImportarOnibusEmLoteUseCase _importarOnibusEmLoteUseCase;
    private readonly IConsultarDadosMestresUseCase _consultarDadosMestresUseCase;
    private readonly IConsultarLimpezaFinaUseCase _consultarLimpezaFinaUseCase;

    public OnibusController(
        ICadastrarOnibusUseCase cadastrarOnibusUseCase,
        IExcluirOnibusUseCase excluirOnibusUseCase,
        IImportarOnibusEmLoteUseCase importarOnibusEmLoteUseCase,
        IConsultarDadosMestresUseCase consultarDadosMestresUseCase,
        IConsultarLimpezaFinaUseCase consultarLimpezaFinaUseCase)
    {
        _cadastrarOnibusUseCase = cadastrarOnibusUseCase;
        _excluirOnibusUseCase = excluirOnibusUseCase;
        _importarOnibusEmLoteUseCase = importarOnibusEmLoteUseCase;
        _consultarDadosMestresUseCase = consultarDadosMestresUseCase;
        _consultarLimpezaFinaUseCase = consultarLimpezaFinaUseCase;
    }

    /// <summary>Cadastra um ônibus na frota. Somente administradores.</summary>
    [HttpPost]
    [Authorize(Roles = "Administrador")]
    public async Task<ActionResult<OnibusResumoDto>> Cadastrar([FromBody] CadastrarOnibusRequest request)
    {
        var onibus = await _cadastrarOnibusUseCase.ExecutarAsync(request);
        return CreatedAtAction(nameof(Listar), new { }, onibus);
    }

    /// <summary>
    /// Cadastra vários ônibus de uma vez a partir de uma planilha (CSV ou Excel .xlsx).
    /// A coluna de prefixo é identificada pelo NOME do cabeçalho (ex.: "Prefixo",
    /// "Veículo", "Nº"), então funciona mesmo com outras colunas no meio.
    /// Prefixos já cadastrados são apenas relatados; o restante do arquivo continua sendo processado.
    /// </summary>
    [HttpPost("importar")]
    [Authorize(Roles = "Administrador")]
    [RequestSizeLimit(10_000_000)]
    public async Task<ActionResult<ResultadoImportacaoOnibus>> Importar(IFormFile arquivo)
    {
        if (arquivo is null || arquivo.Length == 0)
            return BadRequest(new { erro = "Envie uma planilha CSV ou Excel (.xlsx) com uma coluna de prefixo do ônibus." });

        await using var stream = arquivo.OpenReadStream();
        var itens = PlanilhaOnibusParser.Ler(stream, arquivo.FileName);

        if (itens.Count == 0)
            return BadRequest(new { erro = "Nenhuma linha válida encontrada na planilha." });

        var resultado = await _importarOnibusEmLoteUseCase.ExecutarAsync(itens);
        return Ok(resultado);
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<OnibusResumoDto>>> Listar()
    {
        var onibus = await _consultarDadosMestresUseCase.ListarOnibusAsync();
        return Ok(onibus);
    }

    /// <summary>Histórico de limpezas finas de um ônibus específico, por prefixo.</summary>
    [HttpGet("{prefixo}/limpezas")]
    public async Task<ActionResult<IReadOnlyList<LimpezaFinaResumoDto>>> HistoricoLimpezas(string prefixo)
    {
        var historico = await _consultarLimpezaFinaUseCase.ListarPorOnibusAsync(prefixo);
        return Ok(historico);
    }

    /// <summary>Exclui um ônibus. Bloqueia se houver limpeza fina vinculada. Uso: remover dado de teste.</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Administrador")]
    public async Task<IActionResult> Excluir(Guid id)
    {
        await _excluirOnibusUseCase.ExecutarAsync(id);
        return NoContent();
    }
}
