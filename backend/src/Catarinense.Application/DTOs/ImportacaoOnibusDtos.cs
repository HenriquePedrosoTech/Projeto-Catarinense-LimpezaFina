namespace Catarinense.Application.DTOs;

public record ItemImportacaoOnibus(string Prefixo, string? Placa);

public record ItemImportacaoErro(string Prefixo, string Motivo);

public record ResultadoImportacaoOnibus(
    int TotalLinhasProcessadas,
    IReadOnlyList<OnibusResumoDto> Criados,
    IReadOnlyList<string> JaExistentes,
    IReadOnlyList<ItemImportacaoErro> Erros
);
