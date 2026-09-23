namespace Catarinense.Application.Interfaces;

/// <summary>
/// Abstrai o envio de e-mail. A Infrastructure implementa (SMTP, SendGrid, etc.).
/// </summary>
public interface IEmailService
{
    Task EnviarAsync(EmailMensagem mensagem);
}
