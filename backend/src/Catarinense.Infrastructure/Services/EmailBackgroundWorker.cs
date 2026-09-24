using System;
using System.Threading;
using System.Threading.Tasks;
using Catarinense.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Catarinense.Infrastructure.Services;

public class EmailBackgroundWorker : BackgroundService
{
    private readonly IEmailQueueService _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EmailBackgroundWorker> _logger;

    public EmailBackgroundWorker(IEmailQueueService queue, IServiceScopeFactory scopeFactory, ILogger<EmailBackgroundWorker> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("EmailBackgroundWorker iniciado.");

        await foreach (var msg in _queue.ObterEmailsAsync(stoppingToken))
        {
            try
            {
                _logger.LogInformation("Processando e-mail da fila para: {Destinatarios}", string.Join(", ", msg.Destinatarios));
                using var scope = _scopeFactory.CreateScope();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                
                await emailService.EnviarAsync(msg);
                _logger.LogInformation("E-mail enviado com sucesso via fila de background!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail pela fila de background.");
            }
        }
    }
}