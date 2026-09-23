using System.Globalization;
using System.Text;
using Catarinense.Application.DTOs;
using ClosedXML.Excel;

namespace Catarinense.API.Contracts;

/// <summary>
/// Lê uma planilha de ônibus em CSV ou Excel (.xlsx) e identifica as colunas de
/// "prefixo" e "placa" pelo NOME do cabeçalho — não pela posição. Isso significa
/// que funciona mesmo se a planilha tiver várias outras colunas (modelo, garagem,
/// ano, etc.) em qualquer ordem, desde que exista uma coluna cujo cabeçalho
/// contenha uma palavra reconhecida (ver <see cref="PrefixoPalavrasChave"/>).
///
/// Se nenhum cabeçalho reconhecido for encontrado, cai para o comportamento
/// antigo (1ª coluna = prefixo, 2ª coluna = placa), pra continuar funcionando
/// com planilhas simples de duas colunas sem cabeçalho.
/// </summary>
public static class PlanilhaOnibusParser
{
    private static readonly string[] PrefixoPalavrasChave =
        { "prefixo", "prefix", "veiculo", "onibus", "numero", "num", "codigo", "frota", "carro" };

    private static readonly string[] PlacaPalavrasChave = { "placa", "plate" };

    public static IReadOnlyList<ItemImportacaoOnibus> Ler(Stream conteudo, string nomeArquivo)
    {
        var extensao = Path.GetExtension(nomeArquivo).ToLowerInvariant();

        return extensao switch
        {
            ".xlsx" or ".xlsm" => LerExcel(conteudo),
            _ => LerCsv(conteudo), // .csv ou desconhecido: tenta como texto
        };
    }

    private static IReadOnlyList<ItemImportacaoOnibus> LerExcel(Stream conteudo)
    {
        using var workbook = new XLWorkbook(conteudo);
        var planilha = workbook.Worksheets.First();
        var intervalo = planilha.RangeUsed();
        if (intervalo is null) return Array.Empty<ItemImportacaoOnibus>();

        var primeiraLinha = intervalo.FirstRow().RowNumber();
        var ultimaLinha = intervalo.LastRow().RowNumber();
        var primeiraColuna = intervalo.FirstColumn().ColumnNumber();
        var ultimaColuna = intervalo.LastColumn().ColumnNumber();

        var cabecalhos = new List<string>();
        for (var col = primeiraColuna; col <= ultimaColuna; col++)
            cabecalhos.Add(planilha.Cell(primeiraLinha, col).GetString());

        var (colPrefixo, colPlaca, temCabecalho) = IdentificarColunas(cabecalhos);
        var linhaInicial = temCabecalho ? primeiraLinha + 1 : primeiraLinha;

        var itens = new List<ItemImportacaoOnibus>();
        for (var linha = linhaInicial; linha <= ultimaLinha; linha++)
        {
            var prefixo = planilha.Cell(linha, primeiraColuna + colPrefixo).GetString().Trim();
            if (string.IsNullOrWhiteSpace(prefixo)) continue;

            string? placa = null;
            if (colPlaca is int idxPlaca)
                placa = planilha.Cell(linha, primeiraColuna + idxPlaca).GetString().Trim();

            itens.Add(new ItemImportacaoOnibus(prefixo, string.IsNullOrWhiteSpace(placa) ? null : placa));
        }

        return itens;
    }

    private static IReadOnlyList<ItemImportacaoOnibus> LerCsv(Stream conteudo)
    {
        using var leitor = new StreamReader(conteudo);
        var texto = leitor.ReadToEnd();

        var linhas = texto
            .Split('\n')
            .Select(l => l.Trim('\r', ' '))
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToList();

        if (linhas.Count == 0) return Array.Empty<ItemImportacaoOnibus>();

        var cabecalhos = SplitLinha(linhas[0]).Select(c => c.Trim().Trim('"')).ToList();
        var (colPrefixo, colPlaca, temCabecalho) = IdentificarColunas(cabecalhos);

        var itens = new List<ItemImportacaoOnibus>();
        foreach (var linha in linhas.Skip(temCabecalho ? 1 : 0))
        {
            var colunas = SplitLinha(linha);
            var prefixo = colunas.ElementAtOrDefault(colPrefixo)?.Trim().Trim('"');
            if (string.IsNullOrWhiteSpace(prefixo)) continue;

            var placa = colPlaca is int idxPlaca ? colunas.ElementAtOrDefault(idxPlaca)?.Trim().Trim('"') : null;
            itens.Add(new ItemImportacaoOnibus(prefixo, string.IsNullOrWhiteSpace(placa) ? null : placa));
        }

        return itens;
    }

    /// <summary>
    /// Procura, entre os cabeçalhos, uma coluna de prefixo e uma de placa pelo nome.
    /// Retorna também se algum cabeçalho reconhecido foi encontrado — se não, o
    /// chamador deve tratar a primeira linha como dado (não como cabeçalho) e usar
    /// as posições 0 e 1 como fallback.
    /// </summary>
    private static (int ColPrefixo, int? ColPlaca, bool TemCabecalho) IdentificarColunas(IReadOnlyList<string> cabecalhos)
    {
        var normalizados = cabecalhos.Select(Normalizar).ToList();

        var colPrefixo = EncontrarColuna(normalizados, PrefixoPalavrasChave);
        var colPlaca = EncontrarColuna(normalizados, PlacaPalavrasChave);

        if (colPrefixo is null)
            return (0, colPlaca ?? (cabecalhos.Count > 1 ? 1 : null), false);

        return (colPrefixo.Value, colPlaca, true);
    }

    private static int? EncontrarColuna(IReadOnlyList<string> cabecalhosNormalizados, string[] palavrasChave)
    {
        foreach (var palavra in palavrasChave)
        {
            for (var i = 0; i < cabecalhosNormalizados.Count; i++)
            {
                if (cabecalhosNormalizados[i].Contains(palavra))
                    return i;
            }
        }
        return null;
    }

    /// <summary>Minúsculo, sem acento, sem espaços/pontuação — pra comparar "Nº Prefixo" com "prefixo".</summary>
    private static string Normalizar(string texto)
    {
        var normalizado = texto.Normalize(NormalizationForm.FormD);
        var semAcento = new StringBuilder();
        foreach (var c in normalizado)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                semAcento.Append(c);
        }

        return new string(semAcento.ToString().ToLowerInvariant().Where(char.IsLetterOrDigit).ToArray());
    }

    private static string[] SplitLinha(string linha) =>
        linha.Contains(';') ? linha.Split(';') : linha.Split(',');
}
