using System.Security.Claims;
using ESCOLA_API.ViewModels;

namespace ESCOLA_API.Services
{
    public interface ICalendarioEscolarService
    {
        Task<CalendarioEscolarAnoViewModel> GetCalendarioAnualAsync(int? ano, int? mesSelecionado);
        Task<CalendarioEscolarEventoViewModel> AddEventoAsync(
            CalendarioEscolarEventoCreateViewModel viewModel,
            ClaimsPrincipal principal);
    }
}
