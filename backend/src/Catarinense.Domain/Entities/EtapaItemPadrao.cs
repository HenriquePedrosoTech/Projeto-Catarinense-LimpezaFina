using Catarinense.Domain.Exceptions;

namespace Catarinense.Domain.Entities;

public class EtapaItemPadrao : EntidadeBase
{
    public Guid EtapaPadraoId { get; private set; }
    public string Texto { get; private set; } = string.Empty;
    public string? EnquadramentoFoto { get; private set; }
    public int Ordem { get; private set; }
    public bool Ativo { get; private set; } = true;

    protected EtapaItemPadrao() { }

    public EtapaItemPadrao(Guid etapaPadraoId, string texto, int ordem, string? enquadramentoFoto = null)
    {
        if (etapaPadraoId == Guid.Empty)
            throw new DomainException("Etapa Padrão inválida.");

        if (string.IsNullOrWhiteSpace(texto))
            throw new DomainException("O texto do item é obrigatório.");

        EtapaPadraoId = etapaPadraoId;
        Texto = texto.Trim();
        Ordem = ordem;
        EnquadramentoFoto = string.IsNullOrWhiteSpace(enquadramentoFoto) ? null : enquadramentoFoto.Trim();
    }

    public void Desativar() => Ativo = false;
    
    public void Atualizar(string texto, int ordem, string? enquadramentoFoto = null)
    {
        if (string.IsNullOrWhiteSpace(texto))
            throw new DomainException("O texto do item é obrigatório.");

        Texto = texto.Trim();
        Ordem = ordem;
        EnquadramentoFoto = string.IsNullOrWhiteSpace(enquadramentoFoto) ? null : enquadramentoFoto.Trim();
    }
}
