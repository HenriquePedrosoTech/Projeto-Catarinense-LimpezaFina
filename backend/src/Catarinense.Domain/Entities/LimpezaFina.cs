using Catarinense.Domain.Enums;
using Catarinense.Domain.Exceptions;

namespace Catarinense.Domain.Entities;

/// <summary>
/// Aggregate root que representa um registro de limpeza fina de um ônibus.
/// Concentra as regras de negócio: quais etapas precisam ser concluídas,
/// quando pode ser finalizado, aprovado ou reprovado.
/// </summary>
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

    /// <summary>
    /// Cria um novo registro de limpeza fina, já instanciando a execução de cada
    /// etapa padrão ativa (checklist obrigatório definido pelo administrador).
    ///
    /// O número da O.S. é OPCIONAL na abertura: como a O.S. é aberta no Protheus
    /// (sistema externo sem integração automática por enquanto), o operador só
    /// informa o prefixo do ônibus. O administrador preenche o número da O.S.
    /// depois, ao revisar o registro (ver <see cref="DefinirNumeroOS"/>) — e não
    /// é possível aprovar sem ela estar preenchida.
    /// </summary>
    public LimpezaFina(Guid onibusId, Guid operadorId, IEnumerable<Guid> etapasPadraoAtivasIds, string? numeroOS = null)
    {
        if (onibusId == Guid.Empty)
            throw new DomainException("O ônibus é obrigatório.");

        if (operadorId == Guid.Empty)
            throw new DomainException("O operador responsável é obrigatório.");

        var etapasIds = etapasPadraoAtivasIds?.ToList() ?? new List<Guid>();
        if (etapasIds.Count == 0)
            throw new DomainException("Não há etapas de checklist cadastradas. Cadastre as etapas padrão antes de iniciar uma limpeza.");

        OnibusId = onibusId;
        NumeroOS = string.IsNullOrWhiteSpace(numeroOS) ? null : numeroOS.Trim();
        OperadorId = operadorId;
        Status = StatusLimpeza.EmAndamento;
        IniciadaEm = DateTime.UtcNow;

        foreach (var etapaId in etapasIds)
            _etapas.Add(new LimpezaEtapaExecucao(Id, etapaId));
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
            throw new DomainException($"Existem {etapasPendentes.Count} etapa(s) sem foto de evidência. Finalize todas as etapas antes de concluir o registro.");

        Status = StatusLimpeza.Concluida;
        FinalizadaEm = DateTime.UtcNow;
    }

    public void SinalizarCortinas(bool retiradas)
    {
        GarantirEmAndamento();
        CortinasRetiradas = retiradas;
    }

    /// <summary>
    /// Preenche/atualiza o número da O.S. (normalmente feito pelo administrador,
    /// depois de consultar o Protheus). Só pode ser alterado enquanto a notificação
    /// por e-mail ainda não foi enviada — depois disso, o dado já saiu pra fora do
    /// sistema e não deveria mudar mais.
    /// </summary>
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
