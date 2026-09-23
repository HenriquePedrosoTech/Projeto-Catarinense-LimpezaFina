namespace Catarinense.Application.DTOs;

public record IniciarLimpezaFinaRequest(string PrefixoOnibus, Guid OperadorId);

public record DefinirNumeroOSRequest(Guid LimpezaFinaId, string NumeroOS);

public record EtapaResumoDto(Guid EtapaPadraoId, string Nome, string? Descricao, string? LinkVideo, int Ordem, bool Concluida, IReadOnlyList<string> Fotos);

public record LimpezaFinaResumoDto(
    Guid Id,
    string Prefixo,
    string? NumeroOS,
    string Status,
    string NomeOperador,
    DateTime IniciadaEm,
    DateTime? FinalizadaEm,
    bool NotificacaoEnviada,
    bool CortinasRetiradas
);

public record LimpezaFinaDetalhesDto(
    Guid Id,
    string Prefixo,
    string? NumeroOS,
    string Status,
    string NomeOperador,
    DateTime IniciadaEm,
    DateTime? FinalizadaEm,
    DateTime? AvaliadaEm,
    string? ObservacaoAvaliacao,
    bool NotificacaoEnviada,
    bool CortinasRetiradas,
    IReadOnlyList<EtapaResumoDto> Etapas
);
