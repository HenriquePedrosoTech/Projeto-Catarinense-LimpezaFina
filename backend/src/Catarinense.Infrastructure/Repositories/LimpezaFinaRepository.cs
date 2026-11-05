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

    public async Task<LimpezaFina?> ObterEmAndamentoDoDiaPorOnibusAsync(Guid onibusId)
    {
        var hojeBrasil = DateTime.UtcNow.AddHours(-3).Date;
        var inicioHojeUtc = hojeBrasil.AddHours(3);

        return await DbSet
            .Include(l => l.Etapas).ThenInclude(e => e.Itens)
            .Include(l => l.Etapas).ThenInclude(e => e.Fotos)
            .OrderByDescending(l => l.IniciadaEm)
            .FirstOrDefaultAsync(l =>
                l.OnibusId == onibusId &&
                l.Status == StatusLimpeza.EmAndamento &&
                l.IniciadaEm >= inicioHojeUtc);
    }

    public async Task<IReadOnlyList<LimpezaFina>> ListarPorOnibusAsync(Guid onibusId) =>
        await DbSet.Where(l => l.OnibusId == onibusId)
            .OrderByDescending(l => l.IniciadaEm)
            .ToListAsync();

    public async Task<IReadOnlyList<LimpezaFina>> ListarPorOperadorAsync(Guid operadorId)
    {
        var idStr = operadorId.ToString();
        var hojeBrasil = DateTime.UtcNow.AddHours(-3).Date;
        var inicioHojeUtc = hojeBrasil.AddHours(3);

        return await DbSet.Where(l =>
                l.OperadorId == operadorId ||
                (l.ColaboradoresIds != null && l.ColaboradoresIds.Contains(idStr)) ||
                (l.Status == StatusLimpeza.EmAndamento && l.IniciadaEm >= inicioHojeUtc))
            .OrderByDescending(l => l.IniciadaEm)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<LimpezaFina>> ListarPorStatusAsync(StatusLimpeza status) =>
        await DbSet.Where(l => l.Status == status)
            .OrderByDescending(l => l.IniciadaEm)
            .ToListAsync();

    public async Task<IReadOnlyList<LimpezaFina>> ListarParaRelatorioAsync(DateTime? dataInicio, DateTime? dataFim, StatusLimpeza? status, Guid? operadorId)
    {
        var query = DbSet.AsQueryable();

        if (dataInicio.HasValue)
        {
            var inicioUtc = dataInicio.Value.Date.AddHours(3);
            query = query.Where(l => l.IniciadaEm >= inicioUtc);
        }
        if (dataFim.HasValue)
        {
            var fimUtc = dataFim.Value.Date.AddDays(1).AddHours(3);
            query = query.Where(l => l.IniciadaEm < fimUtc);
        }

        if (status.HasValue)
            query = query.Where(l => l.Status == status.Value);

        if (operadorId.HasValue)
        {
            var idStr = operadorId.Value.ToString();
            query = query.Where(l =>
                l.OperadorId == operadorId.Value ||
                (l.ColaboradoresIds != null && l.ColaboradoresIds.Contains(idStr)));
        }

        return await query.OrderByDescending(l => l.IniciadaEm).ToListAsync();
    }

    public override async Task<LimpezaFina?> ObterPorIdAsync(Guid id) => await ObterComEtapasAsync(id);

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
