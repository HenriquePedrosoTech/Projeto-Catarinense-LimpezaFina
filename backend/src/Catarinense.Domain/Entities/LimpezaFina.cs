using Catarinense.Domain.Enums;
using Catarinense.Domain.Exceptions;

namespace Catarinense.Domain.Entities;

public class LimpezaFina : EntidadeBase
{
    public Guid OnibusId { get; private set; }
    public string? NumeroOS { get; private set; }
    public Guid OperadorId { get; private set; }
    public StatusLimpeza Status { get; private set; }
    public DateTime IniciadaEm { get; private set; }
    public DateTime? FinalizadaEm { get; private set; }
    public DateTime? AvaliadaEm { get; private set; }
    public Guid? AvaliadorId { get; private set; }
    public string? ObservacaoAvaliacao { get; private set; }
    public bool NotificacaoEnviada { get; private set; }
    public bool CortinasRetiradas { get; private set; }

    private readonly List<LimpezaEtapaExecucao> _etapas = new();
    public IReadOnlyCollection<LimpezaEtapaExecucao> Etapas => _etapas.AsReadOnly();

    protected LimpezaFina() { }

    public LimpezaFina(Guid onibusId, Guid operadorId, IEnumerable<EtapaPadrao> etapasPadraoAtivas, string? numeroOS = null)
    {
        if (onibusId == Guid.Empty)
            throw new DomainException("O ônibus é obrigatório.");

        if (operadorId == Guid.Empty)
            throw new DomainException("O operador responsável é obrigatório.");

        var etapas = etapasPadraoAtivas?.ToList() ?? new List<EtapaPadrao>();
        if (etapas.Count == 0)
            throw new DomainException("Não há etapas de checklist cadastradas. Cadastre as etapas padrão antes de iniciar uma limpeza.");

        OnibusId = onibusId;
        NumeroOS = string.IsNullOrWhiteSpace(numeroOS) ? null : numeroOS.Trim();
        OperadorId = operadorId;
        Status = StatusLimpeza.EmAndamento;
        IniciadaEm = DateTime.UtcNow;

        foreach (var etapa in etapas)
            _etapas.Add(new LimpezaEtapaExecucao(Id, etapa.Id, etapa.Itens.Select(i => i.Id)));
    }

    public void RegistrarItemExecucao(Guid etapaPadraoId, Guid etapaItemPadraoId, StatusItemChecklist status, StatusFuncionalidade funcionalidade, string? relatoProblema, string? fotoUrl)
    {
        GarantirEmAndamento();

        var execucao = _etapas.FirstOrDefault(e => e.EtapaPadraoId == etapaPadraoId)
            ?? throw new DomainException("Esta etapa não faz parte do checklist deste registro.");

        execucao.RegistrarItemExecucao(etapaItemPadraoId, status, funcionalidade, relatoProblema, fotoUrl);
    }
    
    public void RegistrarFotoDaEtapa(Guid etapaPadraoId, string urlArquivo)
    {
        GarantirEmAndamento();

        var execucao = _etapas.FirstOrDefault(e => e.EtapaPadraoId == etapaPadraoId)
            ?? throw new DomainException("Esta etapa não faz parte do checklist deste registro.");

        execucao.AdicionarFoto(urlArquivo);
    }

    public void RemoverFotoDaEtapa(Guid etapaPadraoId, string urlArquivo)
    {
        GarantirEmAndamento();

        var execucao = _etapas.FirstOrDefault(e => e.EtapaPadraoId == etapaPadraoId)
            ?? throw new DomainException("Esta etapa não faz parte do checklist deste registro.");

        execucao.RemoverFoto(urlArquivo);
    }

    public void Finalizar(Guid? etapaCortinaId = null)
    {
        GarantirEmAndamento();

        var etapasPendentes = _etapas.Where(e => !e.EstaConcluida() && !(e.EtapaPadraoId == etapaCortinaId && !CortinasRetiradas)).ToList();
        if (etapasPendentes.Count > 0)
            throw new DomainException($"Existem {etapasPendentes.Count} etapa(s) incompletas. Finalize todas as etapas antes de concluir o registro.");

        Status = StatusLimpeza.Concluida;
        FinalizadaEm = DateTime.UtcNow;
    }

    public void SinalizarCortinas(bool retiradas)
    {
        GarantirEmAndamento();
        CortinasRetiradas = retiradas;
    }

    public void DefinirNumeroOS(string numeroOS)
    {
        if (string.IsNullOrWhiteSpace(numeroOS))
            throw new DomainException("O número da O.S. não pode ficar em branco.");

        if (NotificacaoEnviada)
            throw new DomainException("Não é possível alterar a O.S. de um registro que já foi notificado por e-mail.");

        NumeroOS = numeroOS.Trim();
    }

    public void Aprovar(Guid avaliadorId)
    {
        GarantirConcluida();

        if (string.IsNullOrWhiteSpace(NumeroOS))
            throw new DomainException("Defina o número da O.S. antes de aprovar este registro.");

        Status = StatusLimpeza.Aprovada;
        AvaliadorId = avaliadorId;
        AvaliadaEm = DateTime.UtcNow;
        ObservacaoAvaliacao = null;
    }

    public void Reprovar(Guid avaliadorId, string motivo)
    {
        GarantirConcluida();

        if (string.IsNullOrWhiteSpace(motivo))
            throw new DomainException("É obrigatório informar o motivo da reprovação.");

        Status = StatusLimpeza.Reprovada;
        AvaliadorId = avaliadorId;
        AvaliadaEm = DateTime.UtcNow;
        ObservacaoAvaliacao = motivo.Trim();
    }

    public void MarcarNotificacaoEnviada()
    {
        if (Status != StatusLimpeza.Aprovada)
            throw new DomainException("Só é possível notificar uma limpeza que já foi aprovada.");

        NotificacaoEnviada = true;
    }

    private void GarantirEmAndamento()
    {
        if (Status != StatusLimpeza.EmAndamento)
            throw new DomainException("Este registro de limpeza não está em andamento.");
    }

    private void GarantirConcluida()
    {
        if (Status != StatusLimpeza.Concluida)
            throw new DomainException("Este registro precisa estar concluído antes de ser avaliado.");
    }
}
