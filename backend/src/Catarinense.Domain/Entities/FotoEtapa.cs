using Catarinense.Domain.Exceptions;

namespace Catarinense.Domain.Entities;

/// <summary>
/// Foto de evidência anexada a uma etapa executada.
/// </summary>
public class FotoEtapa : EntidadeBase
{
    public Guid LimpezaEtapaExecucaoId { get; private set; }
    public string UrlArquivo { get; private set; } = string.Empty;

    protected FotoEtapa() { }

    public FotoEtapa(Guid limpezaEtapaExecucaoId, string urlArquivo)
    {
        if (limpezaEtapaExecucaoId == Guid.Empty)
            throw new DomainException("Etapa de execução inválida para a foto.");

        if (string.IsNullOrWhiteSpace(urlArquivo))
            throw new DomainException("A URL do arquivo da foto é obrigatória.");

        LimpezaEtapaExecucaoId = limpezaEtapaExecucaoId;
        UrlArquivo = urlArquivo;
    }
}
