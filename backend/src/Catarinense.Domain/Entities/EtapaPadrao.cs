using Catarinense.Domain.Exceptions;

namespace Catarinense.Domain.Entities;

public class EtapaPadrao : EntidadeBase
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public string? LinkVideo { get; private set; }
    public int Ordem { get; private set; }
    public bool Ativo { get; private set; } = true;

    private readonly List<EtapaItemPadrao> _itens = new();
    public IReadOnlyCollection<EtapaItemPadrao> Itens => _itens.AsReadOnly();

    protected EtapaPadrao() { }

    public EtapaPadrao(string nome, int ordem, string? descricao = null, string? linkVideo = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome da etapa e obrigatorio.");

        if (ordem < 0)
            throw new DomainException("A ordem da etapa nao pode ser negativa.");

        Nome = nome.Trim();
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        LinkVideo = string.IsNullOrWhiteSpace(linkVideo) ? null : linkVideo.Trim();
        Ordem = ordem;
    }

    public void Desativar() => Ativo = false;

    public void RenomearOuReordenar(string nome, int ordem, string? descricao = null, string? linkVideo = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome da etapa e obrigatorio.");

        Nome = nome.Trim();
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        LinkVideo = string.IsNullOrWhiteSpace(linkVideo) ? null : linkVideo.Trim();
        Ordem = ordem;
    }

    public void AdicionarItem(string texto, int ordem, string? enquadramentoFoto = null)
    {
        _itens.Add(new EtapaItemPadrao(Id, texto, ordem, enquadramentoFoto));
    }

    public void LimparItens()
    {
        _itens.Clear();
    }
}