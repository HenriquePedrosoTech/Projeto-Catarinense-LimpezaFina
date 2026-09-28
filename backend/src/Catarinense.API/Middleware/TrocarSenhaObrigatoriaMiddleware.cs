using Microsoft.AspNetCore.Http;

namespace Catarinense.API.Middleware;

public class TrocarSenhaObrigatoriaMiddleware
{
    private readonly RequestDelegate _next;

    public TrocarSenhaObrigatoriaMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var precisaTrocar = context.User.Claims.FirstOrDefault(c => c.Type == "precisaTrocarSenha")?.Value;
            
            if (precisaTrocar == "true")
            {
                var path = context.Request.Path.Value?.ToLower();
                if (path != null && !path.Contains("/api/auth/trocar-senha") && !path.Contains("/api/auth/login"))
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"erro\": \"Troca de senha obrigatória.\", \"codigo\": \"TROCA_SENHA_OBRIGATORIA\"}");
                    return;
                }
            }
        }

        await _next(context);
    }
}
