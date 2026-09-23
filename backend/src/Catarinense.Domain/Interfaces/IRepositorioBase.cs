namespace Catarinense.Domain.Interfaces;

/// <summary>
/// Contrato genérico de repositório. Evita repetir os mesmos métodos de CRUD
/// em cada repositório específico (DRY), mantendo o Domain independente de EF Core (DIP).
/// </summary>
public interface IRepositorioBase<T>
{
    Task<T?> ObterPorIdAsync(Guid id);
    Task<IReadOnlyList<T>> ListarAsync();
    Task AdicionarAsync(T entidade);
    void Atualizar(T entidade);
    void Remover(T entidade);
    Task SalvarAlteracoesAsync();
}
