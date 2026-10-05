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
            .Include(l => l.Etapas).ThenInclude(e => e.Itens)
            .Include(l => l.Etapas).ThenInclude(e => e.Fotos)
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

        if (dataInicio.HasValue) { var inicioUtc = dataInicio.Value.Date.AddHours(3); query = query.Where(l => l.IniciadaEm >= inicioUtc); } if (dataFim.HasValue) { var fimUtc = dataFim.Value.Date.AddDays(1).AddHours(3); query = query.Where(l => l.IniciadaEm < fimUtc); }

        if (status.HasValue)
            query = query.Where(l => l.Status == status.Value);

        if (operadorId.HasValue)
            query = query.Where(l => l.OperadorId == operadorId.Value);

        return await query.OrderByDescending(l => l.IniciadaEm).ToListAsync();
    }

    // Sobrescreve ObterPorIdAsync para sempre trazer as etapas/fotos junto,
    // jÃ¡ que praticamente todo uso desse agregado precisa dessas coleÃ§Ãµes carregadas.
    public override async Task<LimpezaFina?> ObterPorIdAsync(Guid id) => await ObterComEtapasAsync(id);

    /// <summary>
    /// AlÃ©m do comportamento padrÃ£o (ver RepositorioBase.Atualizar), varre o
    /// agregado inteiro procurando por fotos que ainda nÃ£o estÃ£o sendo rastreadas
    /// pelo EF (foram criadas em memÃ³ria agora, dentro de uma coleÃ§Ã£o de uma
    /// entidade que JÃ era rastreada) e marca cada uma explicitamente como
    /// "Added". Isso Ã© necessÃ¡rio porque o EF Core, ao descobrir sozinho uma
    /// entidade nova dentro de uma coleÃ§Ã£o (em vez de via Add() explÃ­cito),
    /// decide Added vs. Modified olhando se a chave jÃ¡ tem valor â€” e como
    /// geramos o Guid no construtor, ele erra e assume Modified. Sendo
    /// explÃ­citos aqui, eliminamos essa ambiguidade de vez.
    /// </summary>
    public override void Atualizar(LimpezaFina entidade)
    {
        base.Atualizar(entidade);

        foreach (var etapa in entidade.Etapas)
        {
            foreach (var item in etapa.Itens)
            {
                var entrada = Contexto.Entry(item);
                if (entrada.State == EntityState.Detached)
                {
                    entrada.State = EntityState.Added;
                }
            }
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

