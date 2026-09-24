using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Catarinense.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Catarinense.Infrastructure.Services;

public class ResendEmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _remetenteEmail;
    private readonly ILogger<ResendEmailService> _logger;

    public ResendEmailService(HttpClient httpClient, IConfiguration configuration, ILogger<ResendEmailService> logger)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Resend:ApiKey"] ?? "";
        
        var dominioVerificado = configuration["Resend:DominioVerificado"] == "true";
        _remetenteEmail = dominioVerificado 
            ? (configuration["Smtp:RemetenteEmail"] ?? "sistema@seu-dominio.com") 
            : "onboarding@resend.dev";
            
        _logger = logger;
    }

    public async Task EnviarAsync(EmailMensagem mensagem)
    {
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        var payload = new
        {
            from = $"Catarinense Limpeza <{_remetenteEmail}>",
            to = mensagem.Destinatarios,
            subject = mensagem.Assunto,
            html = mensagem.CorpoHtml
        };

        var json = JsonSerializer.Serialize(payload);
        _logger.LogInformation("Enviando para Resend: {Json}", json);

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://api.resend.com/emails", content);
        
        if (!response.IsSuccessStatusCode)
        {
            var erro = await response.Content.ReadAsStringAsync();
            _logger.LogError("Resend API retornou erro {Status}: {Erro}", response.StatusCode, erro);
            response.EnsureSuccessStatusCode();
        }
        else
        {
            var result = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Resend respondeu SUCESSO: {Result}", result);
        }
    }
}