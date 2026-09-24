using System;
using System.IO;
using System.Threading.Tasks;
using Catarinense.Application.Interfaces;
using Firebase.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Catarinense.Infrastructure.Services;

public class FirebaseStorageService : IArmazenamentoArquivoService
{
    private readonly string _bucket;
    private readonly ILogger<FirebaseStorageService> _logger;

    public FirebaseStorageService(IConfiguration configuration, ILogger<FirebaseStorageService> logger)
    {
        _bucket = configuration["Firebase:StorageBucket"]?.Replace("gs://", "") ?? throw new ArgumentNullException("Firebase:StorageBucket");
        _logger = logger;
    }

    public async Task<string> SalvarFotoAsync(Stream conteudo, string nomeArquivoOriginal, string contentType, string prefixo = "")
    {
        var nomePasta = string.IsNullOrWhiteSpace(prefixo) ? "fotos-limpeza" : $"fotos-limpeza/{prefixo}";
        var extensao = Path.GetExtension(nomeArquivoOriginal);
        var novoNome = $"{Guid.NewGuid()}{extensao}";
        
        var task = new FirebaseStorage(_bucket)
            .Child(nomePasta)
            .Child(novoNome)
            .PutAsync(conteudo);
            
        var url = await task;
        _logger.LogInformation("Foto salva no Firebase Storage: {Url}", url);
        return url;
    }

    public Task ExcluirFotoAsync(string url)
    {
        _logger.LogInformation("A exclusao automatica de arquivos no Firebase precisa ser feita extraindo o caminho da URL. Ignorado por enquanto: {Url}", url);
        return Task.CompletedTask;
    }
}