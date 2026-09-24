using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Catarinense.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Catarinense.Infrastructure.Services;

public class ResendEmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _remetenteEmail;

    public ResendEmailService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiKey = configuration["Resend:ApiKey"] ?? "";
        
        // O Resend exige que o domínio do remetente seja validado na plataforma deles.
        // Se usar o domínio @jcatlm.com.br, ele precisa estar validado lá.
        // Senão, para testes, o Resend usa o "onboarding@resend.dev" enviando só pro seu email cadastrado.
        _remetenteEmail = "onboarding@resend.dev";
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

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync("https://api.resend.com/emails", content);
        response.EnsureSuccessStatusCode();
    }
}