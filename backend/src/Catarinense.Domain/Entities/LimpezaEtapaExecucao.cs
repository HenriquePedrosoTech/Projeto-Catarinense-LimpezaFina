using Catarinense.Domain.Exceptions;

namespace Catarinense.Domain.Entities;

/// <summary>
/// Representa a execução de uma EtapaPadrao dentro de um registro específico de LimpezaFina,
/// contendo as fotos de evidência daquela etapa.
/// </summary>
public class LimpezaEtapaExecucao : EntidadeBase
{
    public Guid LimpezaFinaId { get; private set; }
    public Guid EtapaPadraoId { get; private set; }
    public DateTime? ConcluidaEm { get; private set; }

    private readonly List<FotoEtapa> _fotos = new();
    public IReadOnlyCollection<FotoEtapa> Fotos => _fotos.AsReadOnly();

    protected LimpezaEtapaExecucao() { }

    public LimpezaEtapaExecucao(Guid limpezaFinaId, Guid etapaPadraoId)
    {
        if (limpezaFinaId == Guid.Empty)
            throw new DomainException("Registro de limpeza fina inválido.");

        if (etapaPadraoId == Guid.Empty)
            throw new DomainException("Etapa padrão inválida.");

        LimpezaFinaId = limpezaFinaId;
        EtapaPadraoId = etapaPadraoId;
    }

    public void AdicionarFoto(string urlArquivo)
    {
        var foto = new FotoEtapa(Id, urlArquivo);
        _fotos.Add(foto);
        ConcluidaEm = DateTime.UtcNow;
    }

    public void RemoverFoto(string urlArquivo)
    {
        var foto = _fotos.FirstOrDefault(f => f.UrlArquivo == urlArquivo);
        if (foto is not null)
        {
            _fotos.Remove(foto);
            if (_fotos.Count == 0)
                ConcluidaEm = null;
        }
    }

    public bool EstaConcluida() => _fotos.Count > 0;
}
