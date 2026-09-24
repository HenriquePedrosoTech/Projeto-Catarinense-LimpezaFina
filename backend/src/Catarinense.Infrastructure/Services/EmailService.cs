using Catarinense.Application.Interfaces;
using Catarinense.Infrastructure.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Catarinense.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly SmtpOptions _opcoes;

    public EmailService(IOptions<SmtpOptions> opcoes)
    {
        _opcoes = opcoes.Value;
    }

    public async Task EnviarAsync(EmailMensagem mensagem)
    {
        var mimeMessage = new MimeMessage();
        mimeMessage.From.Add(new MailboxAddress(_opcoes.RemetenteNome, _opcoes.RemetenteEmail));
        
        foreach (var destinatario in mensagem.Destinatarios)
        {
            mimeMessage.To.Add(MailboxAddress.Parse(destinatario));
        }

        mimeMessage.Subject = mensagem.Assunto;

        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = mensagem.CorpoHtml
        };
        mimeMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        
        // Timeout menor para não pendurar
        client.Timeout = 10000;

        // SecureSocketOptions.StartTls é o padrão correto para porta 587
        var secureOption = _opcoes.UsarSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None;

        // Se a porta for 465 explícita e pedir SSL, o MailKit entende Auto, mas SslOnConnect é mais seguro para porta 465
        if (_opcoes.Porta == 465 && _opcoes.UsarSsl)
        {
            secureOption = SecureSocketOptions.SslOnConnect;
        }

        await client.ConnectAsync(_opcoes.Host, _opcoes.Porta, secureOption);

        if (!string.IsNullOrEmpty(_opcoes.Usuario))
        {
            await client.AuthenticateAsync(_opcoes.Usuario, _opcoes.Senha);
        }

        await client.SendAsync(mimeMessage);
        await client.DisconnectAsync(true);
    }
}