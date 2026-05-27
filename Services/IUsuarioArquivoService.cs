using System.Security.Claims;
using ESCOLA_API.ViewModels;
using Microsoft.AspNetCore.Http;

namespace ESCOLA_API.Services
{
    public interface IUsuarioArquivoService
    {
        Task<UsuarioSummaryViewModel?> UploadFotoAsync(int usuarioId, IFormFile arquivo, ClaimsPrincipal principal);
        Task<ArquivoDownload?> DownloadFotoAsync(int usuarioId, ClaimsPrincipal principal);
        Task<UsuarioArquivoViewModel[]> GetArquivosAsync(int usuarioId, ClaimsPrincipal principal);
        Task<ArquivoDownload?> DownloadArquivoAsync(int usuarioId, int arquivoId, ClaimsPrincipal principal);
        Task<UsuarioArquivoViewModel> UploadCertificadoAsync(int usuarioId, IFormFile arquivo, ClaimsPrincipal principal);
        Task<bool> DeleteArquivoAsync(int usuarioId, int arquivoId, ClaimsPrincipal principal);
    }
}
