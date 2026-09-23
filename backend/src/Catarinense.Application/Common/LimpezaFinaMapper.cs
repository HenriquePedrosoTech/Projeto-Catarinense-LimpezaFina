using Catarinense.Application.DTOs;
using Catarinense.Domain.Entities;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.Common;

public static class LimpezaFinaMapper
{
    /// <summary>
    /// Carrega, num único lugar, o dicionário de EtapaPadrao usado pelas etapas de um registro.
    /// Evita repetir essa consulta em cada Use Case que precisa montar o DTO de detalhes.
    /// </summary>
    public static async Task<IReadOnlyDictionary<Guid, EtapaPadrao>> CarregarEtapasPadraoAsync(
        IEtapaPadraoRepository etapaPadraoRepository,
        LimpezaFina limpeza)
    {
        var ids = limpeza.Etapas.Select(e => e.EtapaPadraoId);
        var etapas = await etapaPadraoRepository.ListarPorIdsAsync(ids);
        return etapas.ToDictionary(e => e.Id);
    }

    public static LimpezaFinaDetalhesDto ParaDetalhesDto(
        LimpezaFina limpeza,
        Onibus onibus,
        Usuario operador,
        IReadOnlyDictionary<Guid, EtapaPadrao> etapasPadrao)
    {
        var etapas = limpeza.Etapas
            .OrderBy(e => etapasPadrao.TryGetValue(e.EtapaPadraoId, out var ep) ? ep.Ordem : int.MaxValue)
            .Select(e =>
            {
                etapasPadrao.TryGetValue(e.EtapaPadraoId, out var etapaPadrao);
                return new EtapaResumoDto(
                    e.EtapaPadraoId,
                    etapaPadrao?.Nome ?? "(etapa removida)",
                    etapaPadrao?.Descricao, etapaPadrao?.LinkVideo,
                    etapaPadrao?.Ordem ?? 0,
                    e.EstaConcluida(),
                    e.Fotos.Select(f => f.UrlArquivo).ToList()
                );
            })
            .ToList();

        return new LimpezaFinaDetalhesDto(
            limpeza.Id,
            onibus.Prefixo,
            limpeza.NumeroOS,
            limpeza.Status.ToString(),
            operador.Nome,
            limpeza.IniciadaEm,
            limpeza.FinalizadaEm,
            limpeza.AvaliadaEm,
            limpeza.ObservacaoAvaliacao,
            limpeza.NotificacaoEnviada,
            limpeza.CortinasRetiradas,
            etapas
        );
    }

    public static LimpezaFinaResumoDto ParaResumoDto(LimpezaFina limpeza, Onibus onibus, Usuario operador) =>
        new(
            limpeza.Id,
            onibus.Prefixo,
            limpeza.NumeroOS,
            limpeza.Status.ToString(),
            operador.Nome,
            limpeza.IniciadaEm,
            limpeza.FinalizadaEm,
            limpeza.NotificacaoEnviada,
            limpeza.CortinasRetiradas
        );
}
