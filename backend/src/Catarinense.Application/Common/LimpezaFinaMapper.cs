using Catarinense.Application.DTOs;
using Catarinense.Domain.Entities;
using Catarinense.Domain.Interfaces;
using Catarinense.Domain.Enums;

namespace Catarinense.Application.Common;

public static class LimpezaFinaMapper
{
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
                
                var itensExecucao = e.Itens.Select(i => 
                {
                    var itemPadrao = etapaPadrao?.Itens.FirstOrDefault(ip => ip.Id == i.EtapaItemPadraoId);
                    return new EtapaItemResumoDto(
                        i.Id,
                        itemPadrao?.Texto ?? "(item removido)",
                        itemPadrao?.Descricao,
                        itemPadrao?.EnquadramentoFoto,
                        itemPadrao?.Ordem ?? 0,
                        i.Status == StatusItemChecklist.Pendente ? null : (i.Status == StatusItemChecklist.NaoConforme ? "NÃ£o Conforme" : (i.Status == StatusItemChecklist.Conforme ? "Conforme" : "N/A")),
                        i.Funcionalidade == StatusFuncionalidade.Pendente ? null : (i.Funcionalidade == StatusFuncionalidade.ComDefeito ? "Com Defeito" : "OK / Funcional"),
                        i.RelatoProblema,
                        i.FotoUrl,
                        i.Concluida, itemPadrao?.Obrigatorio ?? true
                    );
                }).OrderBy(i => i.Ordem).ToList();

                return new EtapaResumoDto(
                    e.EtapaPadraoId,
                    etapaPadrao?.Nome ?? "(etapa removida)",
                    etapaPadrao?.Descricao, 
                    etapaPadrao?.LinkVideo,
                    etapaPadrao?.Ordem ?? 0,
                    e.EstaConcluida(),
                    e.Fotos.Select(f => f.UrlArquivo).ToList(),
                    itensExecucao, etapaPadrao?.Obrigatoria ?? true, e.ProblemaExtraDescricao, e.ProblemaExtraFotoUrl);
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

