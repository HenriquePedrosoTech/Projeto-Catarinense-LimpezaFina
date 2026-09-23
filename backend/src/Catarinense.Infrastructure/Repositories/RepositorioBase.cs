using Catarinense.Domain.Interfaces;
using Catarinense.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catarinense.Infrastructure.Repositories;

/// <summary>
/// Implementação genérica do CRUD básico, reaproveitada por todos os repositórios
/// específicos (evita repetir AdicionarAsync/Atualizar/SalvarAlteracoesAsync em cada um).
/// </summary>
public class RepositorioBase<T> : IRepositorioBase<T> where T : class
{
    protected readonly AppDbContext Contexto;
    protected readonly DbSet<T> DbSet;

    public RepositorioBase(AppDbContext contexto)
    {
        Contexto = contexto;
        DbSet = contexto.Set<T>();
    }

    public virtual async Task<T?> ObterPorIdAsync(Guid id) => await DbSet.FindAsync(id);

    public virtual async Task<IReadOnlyList<T>> ListarAsync() => await DbSet.ToListAsync();

    public virtual async Task AdicionarAsync(T entidade) => await DbSet.AddAsync(entidade);

    /// <summary>
    /// Marca a entidade como alterada, SE ela ainda não estiver sendo rastreada
    /// pelo DbContext atual. Isso importa porque, no nosso fluxo (Use Case carrega
    /// a entidade, muda alguma coisa, chama Atualizar), a entidade já está sendo
    /// rastreada desde o carregamento — e o EF Core já detecta essas mudanças
    /// sozinho (change tracking automático).
    ///
    /// Chamar DbSet.Update(entidade) numa entidade já rastreada é problemático:
    /// nossas entidades geram o próprio Guid no construtor (antes de qualquer
    /// contato com o banco), e o Update() decide "é novo ou já existe?" olhando
    /// se a chave é o valor padrão (Guid.Empty) ou não. Como o Guid já vem
    /// preenchido, QUALQUER entidade nova dentro do grafo (ex.: uma FotoEtapa
    /// recém-criada numa coleção) é tratada como "já existe, é um UPDATE" — e
    /// como essa linha nunca foi inserida, o banco responde "0 linhas afetadas",
    /// gerando DbUpdateConcurrencyException.
    ///
    /// Só chamamos Update() de fato quando a entidade está genuinamente
    /// desconectada (Detached) — cenário em que ela não foi carregada nesta
    /// mesma requisição/DbContext.
    /// </summary>
    public virtual void Atualizar(T entidade)
    {
        var entrada = Contexto.Entry(entidade);
        if (entrada.State == EntityState.Detached)
        {
            DbSet.Update(entidade);
        }
    }

    public virtual void Remover(T entidade) => DbSet.Remove(entidade);

    public virtual async Task SalvarAlteracoesAsync() => await Contexto.SaveChangesAsync();
}
