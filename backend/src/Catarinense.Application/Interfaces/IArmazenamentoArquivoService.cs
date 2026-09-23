namespace Catarinense.Application.Interfaces;

/// <summary>
/// Abstrai onde as fotos são armazenadas (disco local, S3, Azure Blob, etc.).
/// A Infrastructure implementa. A Application/Domain não sabem nem se importam com o destino.
/// </summary>
public interface IArmazenamentoArquivoService
{
    /// <summary>
    /// Salva o arquivo e retorna a URL (ou caminho) pública/acessível para exibição posterior.
    /// </summary>
    Task<string> SalvarFotoAsync(Stream conteudo, string nomeArquivoOriginal, string contentType);

    /// <summary>
    /// Remove um arquivo previamente salvo (chamado quando um registro de limpeza é
    /// excluído). Não deve lançar exceção se o arquivo já não existir.
    /// </summary>
    Task ExcluirFotoAsync(string url);
}
