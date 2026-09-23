namespace Catarinense.Application.Interfaces;

/// <summary>
/// Representa uma mensagem de e-mail a ser enviada. Mantido simples e sem
/// dependência de nenhuma biblioteca de SMTP específica.
/// </summary>
public record EmailMensagem(
    IReadOnlyList<string> Destinatarios,
    string Assunto,
    string CorpoHtml
);
