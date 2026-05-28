using System.Security.Claims;
using ESCOLA_API.ViewModels;

namespace ESCOLA_API.Services
{
    public interface IAlunoQrCodeBancarioService
    {
        Task<AlunoQrCodeBancarioViewModel?> GerarParaAlunoLogadoAsync(ClaimsPrincipal principal);
    }
}
