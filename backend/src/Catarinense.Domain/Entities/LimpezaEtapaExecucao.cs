using Catarinense.Domain.Exceptions;

namespace Catarinense.Domain.Entities;

public class LimpezaEtapaExecucao : EntidadeBase
{
    public Guid LimpezaFinaId { get; private set; }
    public Guid EtapaPadraoId { get; private set; }
    public DateTime? ConcluidaEm { get; private set; }

    private readonly List<FotoEtapa> _fotos = new();
    public IReadOnlyCollection<FotoEtapa> Fotos => _fotos.AsReadOnly();

    private readonly List<EtapaItemExecucao> _itens = new();
    public IReadOnlyCollection<EtapaItemExecucao> Itens => _itens.AsReadOnly();

    public EtapaPadrao? EtapaPadrao { get; private set; }

    protected LimpezaEtapaExecucao() { }

    public LimpezaEtapaExecucao(Guid limpezaFinaId, Guid etapaPadraoId)
    {
        if (limpezaFinaId == Guid.Empty)
            throw new DomainException("Registro de limpeza fina invalido.");

        if (etapaPadraoId == Guid.Empty)
            throw new DomainException("Etapa padrao invalida.");

        LimpezaFinaId = limpezaFinaId;
        EtapaPadraoId = etapaPadraoId;
    }

    public void InicializarItem(Guid etapaItemPadraoId, bool obrigatorio = true)
    {
        _itens.Add(new EtapaItemExecucao(Id, etapaItemPadraoId, obrigatorio));
    }

    public void AdicionarFoto(string urlArquivo)
    {
        var foto = new FotoEtapa(Id, urlArquivo);
        _fotos.Add(foto);
        VerificarConclusao();
    }

    public void RemoverFoto(string urlArquivo)
    {
        var foto = _fotos.FirstOrDefault(f => f.UrlArquivo == urlArquivo);
        if (foto is not null)
        {
            _fotos.Remove(foto);
            VerificarConclusao();
        }
    }

    public void VerificarConclusao()
    {
        if (EstaConcluida())
        {
            ConcluidaEm = DateTime.UtcNow;
        }
        else
        {
            ConcluidaEm = null;
        }
    }

    public bool EstaConcluida() 
    {
        if (_itens.Count > 0)
        {
            return _itens.Where(i => i.Obrigatorio).All(i => i.Concluida);
        }
        
        return _fotos.Count > 0;
    }
}
