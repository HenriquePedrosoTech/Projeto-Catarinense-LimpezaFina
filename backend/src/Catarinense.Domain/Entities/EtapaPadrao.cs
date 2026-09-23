using Catarinense.Domain.Exceptions;

namespace Catarinense.Domain.Entities;

public class EtapaPadrao : EntidadeBase
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public string? LinkVideo { get; private set; }
    public int Ordem { get; private set; }
    public bool Ativo { get; private set; } = true;

    protected EtapaPadrao() { }

    public EtapaPadrao(string nome, int ordem, string? descricao = null, string? linkVideo = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome da etapa é obrigatório.");

        if (ordem < 0)
            throw new DomainException("A ordem da etapa não pode ser negativa.");

        Nome = nome.Trim();
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        LinkVideo = string.IsNullOrWhiteSpace(linkVideo) ? null : linkVideo.Trim();
        Ordem = ordem;
    }

    public void Desativar() => Ativo = false;

    public void RenomearOuReordenar(string nome, int ordem, string? descricao = null, string? linkVideo = null)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome da etapa é obrigatório.");

        Nome = nome.Trim();
        Descricao = string.IsNullOrWhiteSpace(descricao) ? null : descricao.Trim();
        LinkVideo = string.IsNullOrWhiteSpace(linkVideo) ? null : linkVideo.Trim();
        Ordem = ordem;
    }
}