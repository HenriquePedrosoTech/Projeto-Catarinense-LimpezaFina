using Catarinense.Domain.Enums;
using Catarinense.Domain.Exceptions;

namespace Catarinense.Domain.Entities;

public class EtapaItemExecucao : EntidadeBase
{
    public Guid LimpezaEtapaExecucaoId { get; private set; }
    public Guid EtapaItemPadraoId { get; private set; }
    
    public StatusItemChecklist? Status { get; private set; }
    public StatusFuncionalidade? Funcionalidade { get; private set; }
    public string? RelatoProblema { get; private set; }
    public string? FotoUrl { get; private set; }
    
    public DateTime? ConcluidaEm { get; private set; }

    protected EtapaItemExecucao() { }

    public EtapaItemExecucao(Guid limpezaEtapaExecucaoId, Guid etapaItemPadraoId)
    {
        if (limpezaEtapaExecucaoId == Guid.Empty)
            throw new DomainException("Execução de Etapa inválida.");
        
        if (etapaItemPadraoId == Guid.Empty)
            throw new DomainException("Item padrão inválido.");

        LimpezaEtapaExecucaoId = limpezaEtapaExecucaoId;
        EtapaItemPadraoId = etapaItemPadraoId;
    }

    public void RegistrarExecucao(StatusItemChecklist status, StatusFuncionalidade funcionalidade, string? relatoProblema, string? fotoUrl)
    {
        if ((status == StatusItemChecklist.NaoConforme || funcionalidade == StatusFuncionalidade.ComDefeito) && string.IsNullOrWhiteSpace(relatoProblema))
        {
            throw new DomainException("O relato de problema é obrigatório quando há não conformidade ou defeito.");
        }

        // if (string.IsNullOrWhiteSpace(fotoUrl) && status != StatusItemChecklist.NaoSeAplica)
        // {
        //    throw new DomainException("A foto de evidência é obrigatória.");
        // }

        Status = status;
        Funcionalidade = funcionalidade;
        RelatoProblema = relatoProblema?.Trim();
        FotoUrl = fotoUrl;
        ConcluidaEm = DateTime.UtcNow;
    }

    public bool EstaConcluida() => Status.HasValue;
}
