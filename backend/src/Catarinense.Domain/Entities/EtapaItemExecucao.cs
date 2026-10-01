using Catarinense.Domain.Enums;
using Catarinense.Domain.Exceptions;

namespace Catarinense.Domain.Entities;

public class EtapaItemExecucao : EntidadeBase
{
    public Guid LimpezaEtapaExecucaoId { get; private set; }
    public LimpezaEtapaExecucao LimpezaEtapaExecucao { get; private set; } = null!;

    public Guid EtapaItemPadraoId { get; private set; }
    public bool Obrigatorio { get; private set; } = true;
    
    // Nao precisamos mapear a navegacao pra EtapaItemPadrao se n quisermos, 
    // mas vamos deixar so os Ids e dados basicos por simplicidade, ou entao:
    public EtapaItemPadrao? EtapaItemPadrao { get; private set; }

    public StatusItemChecklist Status { get; private set; } = StatusItemChecklist.Pendente;
    public StatusFuncionalidade Funcionalidade { get; private set; } = StatusFuncionalidade.Pendente;
    
    public string? RelatoProblema { get; private set; }
    public string? FotoUrl { get; private set; }
    public bool Concluida { get; private set; }

    protected EtapaItemExecucao() { }

    public EtapaItemExecucao(Guid limpezaEtapaExecucaoId, Guid etapaItemPadraoId, bool obrigatorio = true)
    {
        LimpezaEtapaExecucaoId = limpezaEtapaExecucaoId;
        EtapaItemPadraoId = etapaItemPadraoId;
        Obrigatorio = obrigatorio;
    }

    public void RegistrarExecucao(StatusItemChecklist status, StatusFuncionalidade func, string? relato, string? fotoUrl)
    {
        if (status == StatusItemChecklist.Pendente)
            throw new DomainException("Status invalido para registro.");
            
        if (func == StatusFuncionalidade.Pendente)
            throw new DomainException("Funcionalidade invalida para registro.");

        if ((status == StatusItemChecklist.NaoConforme || func == StatusFuncionalidade.ComDefeito) && string.IsNullOrWhiteSpace(relato))
        {
            throw new DomainException("Relato de problema e obrigatorio para itens nao conformes ou com defeito.");
        }

        Status = status;
        Funcionalidade = func;
        RelatoProblema = string.IsNullOrWhiteSpace(relato) ? null : relato.Trim();
        
        if (!string.IsNullOrWhiteSpace(fotoUrl))
        {
            FotoUrl = fotoUrl;
        }

        Concluida = true;
    }

    public void DesfazerExecucao()
    {
        Status = StatusItemChecklist.Pendente;
        Funcionalidade = StatusFuncionalidade.Pendente;
        RelatoProblema = null;
        FotoUrl = null;
        Concluida = false;
    }
    }
}

