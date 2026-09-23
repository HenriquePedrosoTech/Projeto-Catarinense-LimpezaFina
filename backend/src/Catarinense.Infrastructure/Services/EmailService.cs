using System.Net;
using System.Net.Mail;
using Catarinense.Application.Interfaces;
using Catarinense.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Catarinense.Infrastructure.Services;

/// <summary>
/// Envia e-mail via SMTP usando System.Net.Mail (nativo do .NET, sem dependência extra).
/// Se a empresa preferir um provedor tipo SendGrid/SES no futuro, basta trocar esta
/// classe — o resto do sistema não muda, pois depende só de IEmailService.
/// </summary>
public class EmailService : IEmailService
{
    private readonly SmtpOptions _opcoes;

    public EmailService(IOptions<SmtpOptions> opcoes)
    {
        _opcoes = opcoes.Value;
    }

    public async Task EnviarAsync(EmailMensagem mensagem)
    {
        using var cliente = new SmtpClient(_opcoes.Host, _opcoes.Porta)
        {
            Credentials = new NetworkCredential(_opcoes.Usuario, _opcoes.Senha),
            EnableSsl = _opcoes.UsarSsl
        };

        using var email = new MailMessage
        {
            From = new MailAddress(_opcoes.RemetenteEmail, _opcoes.RemetenteNome),
            Subject = mensagem.Assunto,
            Body = mensagem.CorpoHtml,
            IsBodyHtml = true
        };

        foreach (var destinatario in mensagem.Destinatarios)
            email.To.Add(destinatario);

        await cliente.SendMailAsync(email);
    }
}
