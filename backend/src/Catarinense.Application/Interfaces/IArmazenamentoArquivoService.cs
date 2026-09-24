using System.IO;
using System.Threading.Tasks;

namespace Catarinense.Application.Interfaces;

public interface IArmazenamentoArquivoService
{
    Task<string> SalvarFotoAsync(Stream conteudo, string nomeArquivoOriginal, string contentType, string prefixo = "");
    Task ExcluirFotoAsync(string url);
}