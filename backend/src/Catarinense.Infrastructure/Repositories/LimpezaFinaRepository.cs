using Catarinense.Domain.Entities;
using Catarinense.Domain.Enums;
using Catarinense.Domain.Interfaces;
using Catarinense.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Catarinense.Infrastructure.Repositories;

public class LimpezaFinaRepository : RepositorioBase<LimpezaFina>, ILimpezaFinaRepository
{
    public LimpezaFinaRepository(AppDbContext contexto) : base(contexto) { }

    public async Task<LimpezaFina?> ObterComEtapasAsync(Guid id) =>
        await DbSet
            .Include(l => l.Etapas)
                .ThenInclude(e => e.Fotos)
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<IReadOnlyList<LimpezaFina>> ListarPorOnibusAsync(Guid onibusId) =>
        await DbSet.Where(l => l.OnibusId == onibusId)
            .OrderByDescending(l => l.IniciadaEm)
            .ToListAsync();

    public async Task<IReadOnlyList<LimpezaFina>> ListarPorOperadorAsync(Guid operadorId) =>
        await DbSet.Where(l => l.OperadorId == operadorId)
            .OrderByDescending(l => l.IniciadaEm)
            .ToListAsync();

    public async Task<IReadOnlyList<LimpezaFina>> ListarPorStatusAsync(StatusLimpeza status) =>
        await DbSet.Where(l => l.Status == status)
            .OrderByDescending(l => l.IniciadaEm)
            .ToListAsync();

    public async Task<IReadOnlyList<LimpezaFina>> ListarParaRelatorioAsync(DateTime? dataInicio, DateTime? dataFim, StatusLimpeza? status, Guid? operadorId)
    {
        var query = DbSet.AsQueryable();

        if (dataInicio.HasValue)
            query = query.Where(l => l.IniciadaEm >= dataInicio.Value);

        if (dataFim.HasValue)
            query = query.Where(l => l.IniciadaEm <= dataFim.Value);

        if (status.HasValue)
            query = query.Where(l => l.Status == status.Value);

        if (operadorId.HasValue)
            query = query.Where(l => l.OperadorId == operadorId.Value);

        return await query.OrderByDescending(l => l.IniciadaEm).ToListAsync();
    }

    // Sobrescreve ObterPorIdAsync para sempre trazer as etapas/fotos junto,
    // já que praticamente todo uso desse agregado precisa dessas coleções carregadas.
    public override async Task<LimpezaFina?> ObterPorIdAsync(Guid id) => await ObterComEtapasAsync(id);

    /// <summary>
    /// Além do comportamento padrão (ver RepositorioBase.Atualizar), varre o
    /// agregado inteiro procurando por fotos que ainda não estão sendo rastreadas
    /// pelo EF (foram criadas em memória agora, dentro de uma coleção de uma
    /// entidade que JÁ era rastreada) e marca cada uma explicitamente como
    /// "Added". Isso é necessário porque o EF Core, ao descobrir sozinho uma
    /// entidade nova dentro de uma coleção (em vez de via Add() explícito),
    /// decide Added vs. Modified olhando se a chave já tem valor — e como
    /// geramos o Guid no construtor, ele erra e assume Modified. Sendo
    /// explícitos aqui, eliminamos essa ambiguidade de vez.
    /// </summary>
    public override void Atualizar(LimpezaFina entidade)
    {
        base.Atualizar(entidade);

        foreach (var etapa in entidade.Etapas)
        {
            foreach (var foto in etapa.Fotos)
            {
                var entrada = Contexto.Entry(foto);
                if (entrada.State == EntityState.Detached)
                {
                    entrada.State = EntityState.Added;
                }
            }
        }
    }
}
