using Catarinense.Domain.Entities;
using Catarinense.Domain.Interfaces;

namespace Catarinense.API.Startup;

/// <summary>
/// Cadastra automaticamente as etapas padrão do checklist de limpeza fina, na
/// ordem abaixo, SE ainda não existir nenhuma etapa cadastrada. Assim o operador
/// nunca precisa esperar o administrador configurar isso manualmente — é o
/// processo padrão da empresa, sempre o mesmo.
///
/// O administrador continua podendo adicionar, via Cadastros → Etapas do
/// checklist, outras etapas além dessas (ou uma filial futura pode ter um
/// processo diferente) — este seed só preenche o ponto de partida.
/// </summary>
public static class SeedEtapasPadraoIniciais
{
    private static readonly string[] EtapasPadrao =
    {
        "RETIRADA DAS CORTINAS",
        "LIMPEZA DO BANHEIRO",
        "LIMPEZA DA GELADEIRA",
        "LIMPEZA INTERNA",
        "LIMPEZA DA CABINE DO MOTORISTA",
        "LIMPEZA EXTERNA DA FROTA",
        "MÁQUINA DE OZÔNIO",
    };

    public static async Task ExecutarAsync(IServiceProvider services, ILogger logger)
    {
        using var scope = services.CreateScope();
        var etapaPadraoRepository = scope.ServiceProvider.GetRequiredService<IEtapaPadraoRepository>();

        var existentes = await etapaPadraoRepository.ListarAsync();
        if (existentes.Count > 0)
        {
            logger.LogInformation("SeedEtapasPadrao: já existem {Quantidade} etapa(s) cadastrada(s). Nada a fazer.", existentes.Count);
            return;
        }

        for (var ordem = 0; ordem < EtapasPadrao.Length; ordem++)
        {
            var etapa = new EtapaPadrao(EtapasPadrao[ordem], ordem);
            await etapaPadraoRepository.AdicionarAsync(etapa);
        }

        await etapaPadraoRepository.SalvarAlteracoesAsync();

        logger.LogWarning("SeedEtapasPadrao: {Quantidade} etapas padrão do checklist criadas automaticamente.", EtapasPadrao.Length);
    }
}
