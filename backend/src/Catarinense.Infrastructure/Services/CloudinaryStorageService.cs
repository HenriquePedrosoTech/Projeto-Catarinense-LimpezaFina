using System;
using System.IO;
using System.Threading.Tasks;
using Catarinense.Application.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Catarinense.Infrastructure.Services;

public class CloudinaryStorageService : IArmazenamentoArquivoService
{
    private readonly Cloudinary _cloudinary;
    private readonly ILogger<CloudinaryStorageService> _logger;

    public CloudinaryStorageService(IConfiguration configuration, ILogger<CloudinaryStorageService> logger)
    {
        _logger = logger;
        
        var cloudName = configuration["Cloudinary:CloudName"] ?? throw new ArgumentNullException("Cloudinary:CloudName");
        var apiKey = configuration["Cloudinary:ApiKey"] ?? throw new ArgumentNullException("Cloudinary:ApiKey");
        var apiSecret = configuration["Cloudinary:ApiSecret"] ?? throw new ArgumentNullException("Cloudinary:ApiSecret");
        
        var account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<string> SalvarFotoAsync(Stream conteudo, string nomeArquivoOriginal, string contentType, string prefixo = "")
    {
        var nomePasta = string.IsNullOrWhiteSpace(prefixo) ? "fotos-limpeza" : $"fotos-limpeza/{prefixo}";
        var extensao = Path.GetExtension(nomeArquivoOriginal);
        var novoNome = $"{Guid.NewGuid()}"; // Cloudinary will append extension or format automatically, but we can also just use the guid as publicId
        
        var uploadParams = new ImageUploadParams()
        {
            File = new FileDescription(novoNome + extensao, conteudo),
            Folder = nomePasta,
            PublicId = novoNome
        };
        
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        
        if (uploadResult.Error != null)
        {
            throw new Exception($"Erro no Cloudinary: {uploadResult.Error.Message}");
        }
        
        _logger.LogInformation("Foto salva no Cloudinary: {Url}", uploadResult.SecureUrl);
        return uploadResult.SecureUrl.ToString();
    }

    public Task ExcluirFotoAsync(string url)
    {
        // For deletion, Cloudinary uses the PublicId (e.g. fotos-limpeza/8323/c43a4...). 
        // We can extract it from the URL if needed later.
        _logger.LogInformation("A exclusao automatica de arquivos no Cloudinary precisa do PublicId. Ignorado por enquanto: {Url}", url);
        return Task.CompletedTask;
    }
}