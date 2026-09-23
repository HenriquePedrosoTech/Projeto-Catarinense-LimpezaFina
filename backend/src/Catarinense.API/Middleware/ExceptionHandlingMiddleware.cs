using Catarinense.Application.Exceptions;
using Catarinense.Domain.Exceptions;
using System.Text.Json;

namespace Catarinense.API.Middleware;

/// <summary>
/// Converte exceções de negócio em respostas HTTP padronizadas, para que os
/// Controllers não precisem de try/catch repetido em cada action.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _ambiente;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment ambiente)
    {
        _next = next;
        _logger = logger;
        _ambiente = ambiente;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            var (status, mensagem) = ex switch
            {
                DomainException => (StatusCodes.Status400BadRequest, ex.Message),
                NotFoundException => (StatusCodes.Status404NotFound, ex.Message),
                NaoAutorizadoException => (StatusCodes.Status403Forbidden, ex.Message),
                InvalidOperationException => (StatusCodes.Status400BadRequest, ex.Message),
                _ => (StatusCodes.Status500InternalServerError, "Ocorreu um erro inesperado. Tente novamente.")
            };

            if (status == StatusCodes.Status500InternalServerError)
                _logger.LogError(ex, "Erro não tratado ao processar {Path}", context.Request.Path);

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = status;

            // Em Development, manda o detalhe real do erro na resposta (mais fácil de
            // depurar sem precisar ficar catando no console). Em produção, nunca —
            // detalhe de exceção pode vazar informação sensível (nomes de tabela, etc.).
            object corpo = _ambiente.IsDevelopment() && status == StatusCodes.Status500InternalServerError
                ? new { erro = mensagem, detalhe = ex.ToString() }
                : new { erro = mensagem };

            await context.Response.WriteAsync(JsonSerializer.Serialize(corpo));
        }
    }
}
