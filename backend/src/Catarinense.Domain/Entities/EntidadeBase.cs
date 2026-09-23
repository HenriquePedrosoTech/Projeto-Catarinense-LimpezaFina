namespace Catarinense.Domain.Entities;

/// <summary>
/// Classe base para todas as entidades do domínio.
/// Centraliza Id e data de criação, evitando duplicação (SRP).
/// </summary>
public abstract class EntidadeBase
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CriadoEm { get; protected set; } = DateTime.UtcNow;
}
