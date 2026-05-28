using System.Security.Claims;
using ESCOLA_API.ViewModels;
using Microsoft.AspNetCore.Http;

namespace ESCOLA_API.Services
{
    public interface IHoleriteService
    {
        Task<HoleriteViewModel[]> GetMeusHoleritesAsync(ClaimsPrincipal principal);
        Task<ArquivoDownload?> DownloadMeuHoleriteAsync(int holeriteId, ClaimsPrincipal principal);
        Task<HoleriteViewModel[]> GetHoleritesUsuarioAsync(int usuarioId, ClaimsPrincipal principal);
        Task<ArquivoDownload?> DownloadHoleriteUsuarioAsync(int usuarioId, int holeriteId, ClaimsPrincipal principal);
        Task<HoleriteViewModel> UploadHoleriteAsync(int usuarioId, int competenciaMes, int competenciaAno, IFormFile arquivo, ClaimsPrincipal principal);
        Task<bool> DeleteHoleriteAsync(int usuarioId, int holeriteId, ClaimsPrincipal principal);
    }
}
