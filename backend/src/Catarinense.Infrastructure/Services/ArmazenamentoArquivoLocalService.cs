using Catarinense.Application.Interfaces;
using Catarinense.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Catarinense.Infrastructure.Services;

/// <summary>
/// Salva as fotos em disco local (pasta configurável, ex.: wwwroot/uploads).
/// Pode ser trocada depois por uma implementação em S3/Azure Blob sem afetar
/// nenhum caso de uso, pois eles dependem só de IArmazenamentoArquivoService.
/// </summary>
public class ArmazenamentoArquivoLocalService : IArmazenamentoArquivoService
{
    private readonly ArmazenamentoOptions _opcoes;
    private static readonly HashSet<string> ExtensoesPermitidas = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp", ".heic"
    };

    public ArmazenamentoArquivoLocalService(IOptions<ArmazenamentoOptions> opcoes)
    {
        _opcoes = opcoes.Value;
    }

    public async Task<string> SalvarFotoAsync(Stream conteudo, string nomeArquivoOriginal, string contentType)
    {
        var extensao = Path.GetExtension(nomeArquivoOriginal);
        if (string.IsNullOrWhiteSpace(extensao) || !ExtensoesPermitidas.Contains(extensao))
            throw new InvalidOperationException("Formato de imagem não suportado. Envie JPG, PNG, WEBP ou HEIC.");

        // Carrega em memória pra poder inspecionar os bytes iniciais (assinatura do
        // arquivo) antes de gravar em disco — a extensão do nome do arquivo é só o
        // que o cliente disse que é; quem garante é o próprio conteúdo binário.
        using var memoria = new MemoryStream();
        await conteudo.CopyToAsync(memoria);

        if (!ConteudoPareceImagemValida(memoria))
            throw new InvalidOperationException(
                "O arquivo enviado não é uma imagem válida (o conteúdo não corresponde a um formato de imagem reconhecido).");

        Directory.CreateDirectory(_opcoes.PastaDestino);

        var nomeUnico = $"{Guid.NewGuid()}{extensao}";
        var caminhoCompleto = Path.Combine(_opcoes.PastaDestino, nomeUnico);

        memoria.Position = 0;
        await using (var arquivoDestino = File.Create(caminhoCompleto))
        {
            await memoria.CopyToAsync(arquivoDestino);
        }

        return $"{_opcoes.UrlPublicaBase.TrimEnd('/')}/{nomeUnico}";
    }

    public Task ExcluirFotoAsync(string url)
    {
        // A URL é algo como "/uploads/fotos-limpeza/{arquivo}" — só nos interessa o nome do arquivo.
        var nomeArquivo = Path.GetFileName(url);
        if (string.IsNullOrWhiteSpace(nomeArquivo))
            return Task.CompletedTask;

        var caminhoCompleto = Path.Combine(_opcoes.PastaDestino, nomeArquivo);
        if (File.Exists(caminhoCompleto))
            File.Delete(caminhoCompleto);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Verifica a assinatura binária (magic bytes) do arquivo, não a extensão do
    /// nome — impede que um arquivo qualquer disfarçado de ".jpg" seja aceito.
    /// </summary>
    private static bool ConteudoPareceImagemValida(MemoryStream memoria)
    {
        if (memoria.Length < 12) return false;

        var cabecalho = new byte[12];
        memoria.Position = 0;
        _ = memoria.Read(cabecalho, 0, 12);

        // JPEG: FF D8 FF
        if (cabecalho[0] == 0xFF && cabecalho[1] == 0xD8 && cabecalho[2] == 0xFF)
            return true;

        // PNG: 89 50 4E 47 0D 0A 1A 0A
        byte[] assinaturaPng = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
        if (cabecalho.Take(8).SequenceEqual(assinaturaPng))
            return true;

        // WEBP: "RIFF" (bytes 0-3) + "WEBP" (bytes 8-11)
        if (cabecalho[0] == 'R' && cabecalho[1] == 'I' && cabecalho[2] == 'F' && cabecalho[3] == 'F' &&
            cabecalho[8] == 'W' && cabecalho[9] == 'E' && cabecalho[10] == 'B' && cabecalho[11] == 'P')
            return true;

        // HEIC/HEIF: caixa "ftyp" nos bytes 4-7 (formato ISO Base Media File Format)
        if (cabecalho[4] == 'f' && cabecalho[5] == 't' && cabecalho[6] == 'y' && cabecalho[7] == 'p')
            return true;

        return false;
    }
}
