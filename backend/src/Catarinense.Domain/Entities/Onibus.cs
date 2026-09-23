using Catarinense.Domain.Exceptions;

namespace Catarinense.Domain.Entities;

/// <summary>
/// Representa um ônibus da frota, identificado pelo prefixo.
/// </summary>
public class Onibus : EntidadeBase
{
    public string Prefixo { get; private set; } = string.Empty;
    public string? Placa { get; private set; }
    public bool Ativo { get; private set; } = true;

    protected Onibus() { }

    public Onibus(string prefixo, string? placa = null)
    {
        if (string.IsNullOrWhiteSpace(prefixo))
            throw new DomainException("O prefixo do ônibus é obrigatório.");

        Prefixo = prefixo.Trim().ToUpperInvariant();
        Placa = placa?.Trim().ToUpperInvariant();
    }

    public void Desativar() => Ativo = false;
}
