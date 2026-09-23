using Catarinense.Application.DTOs;
using Catarinense.Application.Interfaces.UseCases;
using Catarinense.Domain.Entities;
using Catarinense.Domain.Exceptions;
using Catarinense.Domain.Interfaces;

namespace Catarinense.Application.UseCases;

public class ImportarOnibusEmLoteUseCase : IImportarOnibusEmLoteUseCase
{
    private readonly IOnibusRepository _onibusRepository;

    public ImportarOnibusEmLoteUseCase(IOnibusRepository onibusRepository)
    {
        _onibusRepository = onibusRepository;
    }

    public async Task<ResultadoImportacaoOnibus> ExecutarAsync(IReadOnlyList<ItemImportacaoOnibus> itens)
    {
        var criados = new List<OnibusResumoDto>();
        var jaExistentes = new List<string>();
        var erros = new List<ItemImportacaoErro>();

        // Evita duplicar dentro do próprio arquivo (ex.: prefixo repetido duas vezes no CSV).
        var prefixosVistosNesteLote = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var item in itens)
        {
            var prefixo = item.Prefixo?.Trim().ToUpperInvariant() ?? "";

            if (string.IsNullOrWhiteSpace(prefixo))
            {
                erros.Add(new ItemImportacaoErro(item.Prefixo ?? "(vazio)", "Prefixo em branco."));
                continue;
            }

            if (!prefixosVistosNesteLote.Add(prefixo))
            {
                erros.Add(new ItemImportacaoErro(prefixo, "Prefixo duplicado dentro do próprio arquivo."));
                continue;
            }

            try
            {
                if (await _onibusRepository.ObterPorPrefixoAsync(prefixo) is not null)
                {
                    jaExistentes.Add(prefixo);
                    continue;
                }

                var onibus = new Onibus(prefixo, item.Placa);
                await _onibusRepository.AdicionarAsync(onibus);
                criados.Add(new OnibusResumoDto(onibus.Id, onibus.Prefixo, onibus.Placa, onibus.Ativo));
            }
            catch (DomainException ex)
            {
                erros.Add(new ItemImportacaoErro(prefixo, ex.Message));
            }
        }

        // Um único SaveChanges pro lote inteiro — muito mais eficiente que salvar linha a linha.
        await _onibusRepository.SalvarAlteracoesAsync();

        return new ResultadoImportacaoOnibus(itens.Count, criados, jaExistentes, erros);
    }
}
