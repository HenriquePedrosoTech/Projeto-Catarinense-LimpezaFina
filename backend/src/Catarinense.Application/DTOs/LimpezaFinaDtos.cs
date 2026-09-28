namespace Catarinense.Application.DTOs;

public record IniciarLimpezaFinaRequest(string PrefixoOnibus, Guid OperadorId);

public record DefinirNumeroOSRequest(Guid LimpezaFinaId, string NumeroOS);

public record EtapaItemResumoDto(Guid Id, string Texto, string? EnquadramentoFoto, int Ordem, string? Status, string? Funcionalidade, string? RelatoProblema, string? FotoUrl, bool Concluida);
public record EtapaResumoDto(Guid EtapaPadraoId, string Nome, string? Descricao, string? LinkVideo, int Ordem, bool Concluida, IReadOnlyList<string> Fotos, IReadOnlyList<EtapaItemResumoDto> Itens);

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
