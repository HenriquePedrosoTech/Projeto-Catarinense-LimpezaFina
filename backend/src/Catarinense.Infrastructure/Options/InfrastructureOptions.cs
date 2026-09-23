namespace Catarinense.Infrastructure.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Chave { get; set; } = string.Empty;
    public string Emissor { get; set; } = string.Empty;
    public string Audiencia { get; set; } = string.Empty;
    public int ExpiracaoHoras { get; set; } = 8;
}

public class SmtpOptions
{
    public const string SectionName = "Smtp";

    public string Host { get; set; } = string.Empty;
    public int Porta { get; set; } = 587;
    public string Usuario { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
    public string RemetenteEmail { get; set; } = string.Empty;
    public string RemetenteNome { get; set; } = "Catarinense — Limpeza Fina";
    public bool UsarSsl { get; set; } = true;
}

public class ArmazenamentoOptions
{
    public const string SectionName = "Armazenamento";

    /// <summary>Pasta local onde as fotos serão salvas.</summary>
    public string PastaDestino { get; set; } = "wwwroot/uploads/fotos-limpeza";

    /// <summary>Prefixo de URL pública usado para montar o link da foto (ex.: https://api.catarinense.com/uploads).</summary>
    public string UrlPublicaBase { get; set; } = "/uploads/fotos-limpeza";
}
