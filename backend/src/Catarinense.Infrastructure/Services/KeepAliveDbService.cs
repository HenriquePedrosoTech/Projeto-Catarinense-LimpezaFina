using System;
using System.Threading;
using System.Threading.Tasks;
using Catarinense.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Catarinense.Infrastructure.Services;

public class KeepAliveDbService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<KeepAliveDbService> _logger;
    private static readonly HttpClient _httpClient = new() { Timeout = TimeSpan.FromSeconds(15) };

    public KeepAliveDbService(IServiceProvider serviceProvider, ILogger<KeepAliveDbService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("KeepAliveDbService iniciado. Mantendo o Render e o TiDB Cloud acordados a cada 5 minutos...");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                // Fazemos um SELECT 1 leve direto no banco para evitar que a conexão entre em suspensão (cold start)
                await dbContext.Database.ExecuteSqlRawAsync("SELECT 1", stoppingToken);
                
                // Fazemos também um auto-ping HTTP na URL pública do Render para evitar que o container durma após 15 min
                var renderUrl = Environment.GetEnvironmentVariable("RENDER_EXTERNAL_URL")
                    ?? "https://catarinense-limpezafina.onrender.com";
                await _httpClient.GetAsync($"{renderUrl.TrimEnd('/')}/api/health", stoppingToken);
                
                _logger.LogDebug("Ping no TiDB Cloud e auto-ping no Render realizados com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Falha no keep-alive (ignorando, tentará novamente em breve): {Msg}", ex.Message);
            }

            // Aguarda 5 minutos para o próximo ping
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}