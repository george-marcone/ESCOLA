using Microsoft.AspNetCore.Http;

namespace ESCOLA_API.Services
{
    public interface IUsuarioArquivoStorage
    {
        Task<ArquivoSalvo> SalvarAsync(int usuarioId, string categoria, IFormFile arquivo);
        Task<ArquivoDownload?> AbrirAsync(string? nomeBlob, string? url, string? nomeOriginal, string? contentType);
        Task RemoverAsync(string? nomeBlob, string? url);
    }
}
