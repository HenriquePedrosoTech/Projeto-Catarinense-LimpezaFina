namespace Catarinense.Application.DTOs;

public record RegistrarFotoEtapaRequest(
    Guid LimpezaFinaId,
    Guid EtapaPadraoId,
    Stream ConteudoArquivo,
    string NomeArquivoOriginal,
    string ContentType
);

public record RemoverFotoEtapaRequest(Guid LimpezaFinaId, Guid EtapaPadraoId, string UrlArquivo);

public record FinalizarLimpezaFinaRequest(Guid LimpezaFinaId, string UrlBaseDetalhes = "");

public record AprovarLimpezaFinaRequest(Guid LimpezaFinaId, Guid AvaliadorId);

public record ReprovarLimpezaFinaRequest(Guid LimpezaFinaId, Guid AvaliadorId, string Motivo);

public record EnviarNotificacaoLimpezaRequest(Guid LimpezaFinaId, IReadOnlyList<string> DestinatariosEmail, string UrlBaseDetalhes);

