namespace Catarinense.API.Contracts;

/// <summary>
/// Representa o corpo do formulário multipart usado no upload de foto de uma etapa.
/// O ASP.NET Core faz o model binding automático de IFormFile a partir do form-data.
/// </summary>
public class EnviarFotoEtapaForm
{
    public IFormFile Arquivo { get; set; } = default!;
}

public record ReprovarLimpezaBody(string Motivo);

public record NotificarLimpezaBody(IReadOnlyList<string> DestinatariosEmail);
