using Catarinense.Domain.Exceptions;

namespace Catarinense.Domain.Entities;

public class EtapaPadrao : EntidadeBase
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public string? LinkVideo { get; private set; }
    public int Ordem { get; private set; }
    public bool Obrigatoria { get; private set; } = true;
    public bool Ativo { get; private set; } = true;

    private readonly List<EtapaItemPadrao> _itens = new();
    public IReadOnlyCollection<EtapaItemPadrao> Itens => _itens.AsReadOnly();

    protected EtapaPadrao() { }

    public EtapaPadrao(string nome, int ordem, string? descricao = null, string? linkVideo = null, bool obrigatoria = true)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome da etapa e obrigatorio.");

        if (ordem < 0)
            throw new DomainException("A ordem da etapa nao pode ser negativa.");

        Nome = nome.Trim();
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        LinkVideo = string.IsNullOrWhiteSpace(linkVideo) ? null : linkVideo.Trim();
        Ordem = ordem;
        Obrigatoria = obrigatoria;
    }

    public void Desativar() => Ativo = false;

    public void RenomearOuReordenar(string nome, int ordem, string? descricao = null, string? linkVideo = null, bool obrigatoria = true)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome da etapa e obrigatorio.");

        Nome = nome.Trim();
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        LinkVideo = string.IsNullOrWhiteSpace(linkVideo) ? null : linkVideo.Trim();
        Ordem = ordem;
        Obrigatoria = obrigatoria;
    }

    public void AdicionarItem(string texto, int ordem, string? descricao = null, string? enquadramentoFoto = null, bool obrigatorio = true)
    {
        _itens.Add(new EtapaItemPadrao(Id, texto, ordem, descricao, enquadramentoFoto, obrigatorio));
    }

    public void SincronizarItens(IEnumerable<(Guid? Id, string Texto, int Ordem, string? Descricao, string? EnquadramentoFoto, bool Obrigatorio)> novosItens)
    {
        var idsParaManter = novosItens.Where(x => x.Id.HasValue && x.Id.Value != Guid.Empty).Select(x => x.Id).ToHashSet();

        foreach (var item in _itens.Where(i => i.Ativo && !idsParaManter.Contains(i.Id)))
        {
            item.Desativar();
        }

        foreach (var novo in novosItens)
        {
            if (novo.Id.HasValue && novo.Id.Value != Guid.Empty)
            {
                var existente = _itens.FirstOrDefault(i => i.Id == novo.Id.Value);
                if (existente != null)
                {
                    existente.Atualizar(novo.Texto, novo.Ordem, novo.Descricao, novo.EnquadramentoFoto, novo.Obrigatorio);
                }
            }
            else
            {
                AdicionarItem(novo.Texto, novo.Ordem, novo.Descricao, novo.EnquadramentoFoto, novo.Obrigatorio);
            }
        }
    }
}
