using System;
using System.IO;
using System.Text.RegularExpressions;
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
        var novoNome = $"{Guid.NewGuid()}"; 
        
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

    public async Task ExcluirFotoAsync(string url)
    {
        try
        {
            // The Cloudinary URL looks like: https://res.cloudinary.com/cloudname/image/upload/v1234/fotos-limpeza/1234/uuid.jpg
            // We need to extract: "fotos-limpeza/1234/uuid" (without extension and without the domain/upload/v.. parts)
            var match = Regex.Match(url, @"/upload/(?:v\d+/)?(.+)\.[a-zA-Z0-9]+$");
            if (match.Success)
            {
                var publicId = match.Groups[1].Value;
                var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId));
                _logger.LogInformation("Excluido do Cloudinary ({PublicId}): {Result}", publicId, result.Result);
            }
            else
            {
                _logger.LogWarning("Nao foi possivel extrair o PublicId da URL do Cloudinary: {Url}", url);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao tentar excluir foto do Cloudinary: {Url}", url);
        }
    }
}