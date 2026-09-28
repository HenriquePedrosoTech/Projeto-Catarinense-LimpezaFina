using Catarinense.Domain.Exceptions;
using Catarinense.Domain.Enums;

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

    protected LimpezaEtapaExecucao() { }

    public LimpezaEtapaExecucao(Guid limpezaFinaId, Guid etapaPadraoId, IEnumerable<Guid>? itensPadraoIds = null)
    {
        if (limpezaFinaId == Guid.Empty)
            throw new DomainException("Registro de limpeza fina inválido.");

        if (etapaPadraoId == Guid.Empty)
            throw new DomainException("Etapa padrão inválida.");

        LimpezaFinaId = limpezaFinaId;
        EtapaPadraoId = etapaPadraoId;
        
        if (itensPadraoIds != null)
        {
            foreach(var itemId in itensPadraoIds)
            {
                _itens.Add(new EtapaItemExecucao(Id, itemId));
            }
        }
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
            if (_fotos.Count == 0 && _itens.All(i => !i.EstaConcluida()))
                ConcluidaEm = null;
        }
    }

    public void RegistrarItemExecucao(Guid etapaItemPadraoId, StatusItemChecklist status, StatusFuncionalidade funcionalidade, string? relatoProblema, string? fotoUrl)
    {
        var item = _itens.FirstOrDefault(i => i.EtapaItemPadraoId == etapaItemPadraoId)
            ?? throw new DomainException("Item não pertence a esta etapa.");
            
        item.RegistrarExecucao(status, funcionalidade, relatoProblema, fotoUrl);
        ConcluidaEm = DateTime.UtcNow;
    }

    public bool EstaConcluida() => _itens.Count > 0 ? _itens.All(i => i.EstaConcluida()) : _fotos.Count > 0;
}
