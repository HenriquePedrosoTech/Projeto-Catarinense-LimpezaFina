using Catarinense.Domain.Exceptions;

namespace Catarinense.Domain.Entities;

public class EtapaItemPadrao : EntidadeBase
{
    public Guid EtapaPadraoId { get; private set; }
    public EtapaPadrao EtapaPadrao { get; private set; } = null!;
    
    public string Texto { get; private set; } = string.Empty;
    public int Ordem { get; private set; }
    public string? Descricao { get; private set; }
    public string? EnquadramentoFoto { get; private set; }
    public bool Obrigatorio { get; private set; } = true;
    public bool Ativo { get; private set; } = true;

    protected EtapaItemPadrao() { }

    public EtapaItemPadrao(Guid etapaPadraoId, string texto, int ordem, string? descricao = null, string? enquadramentoFoto = null, bool obrigatorio = true)
    {
        if (etapaPadraoId == Guid.Empty) throw new DomainException("Id da etapa padrao invalido.");
        if (string.IsNullOrWhiteSpace(texto)) throw new DomainException("O texto do item e obrigatorio.");

        EtapaPadraoId = etapaPadraoId;
        Texto = texto.Trim();
        Ordem = ordem;
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        EnquadramentoFoto = string.IsNullOrWhiteSpace(enquadramentoFoto) ? null : enquadramentoFoto.Trim();
        Obrigatorio = obrigatorio;
    }

    public void Desativar() => Ativo = false;

    public void Atualizar(string texto, int ordem, string? descricao, string? enquadramentoFoto, bool obrigatorio = true)
    {
        if (string.IsNullOrWhiteSpace(texto)) throw new DomainException("O texto do item e obrigatorio.");
        
        Texto = texto.Trim();
        Ordem = ordem;
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        EnquadramentoFoto = string.IsNullOrWhiteSpace(enquadramentoFoto) ? null : enquadramentoFoto.Trim();
        Obrigatorio = obrigatorio;
    }
}
