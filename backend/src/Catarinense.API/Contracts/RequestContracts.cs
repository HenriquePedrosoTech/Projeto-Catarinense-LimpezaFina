namespace Catarinense.API.Contracts;

/// <summary>
/// Representa o corpo do formulÃ¡rio multipart usado no upload de foto de uma etapa.
/// O ASP.NET Core faz o model binding automÃ¡tico de IFormFile a partir do form-data.
/// </summary>
public class EnviarFotoEtapaForm
{
    public IFormFile Arquivo { get; set; } = default!;
}

public record ReprovarLimpezaBody(string Motivo);

public record NotificarLimpezaBody(IReadOnlyList<string> DestinatariosEmail);

public class RegistrarItemExecucaoForm
{
    public string Status { get; set; } = default!;
    public string Funcionalidade { get; set; } = default!;
    public string? RelatoProblema { get; set; }
    public IFormFile? Arquivo { get; set; }
}
