using ESCOLA_API.ViewModels;

namespace ESCOLA_API.Services
{
    public interface IDiretoriaService
    {
        Task<DiretoriaViewModel[]> GetAllAsync(bool includeUsuario = false);
        Task<DiretoriaViewModel?> GetByIdAsync(int diretoriaId, bool includeUsuario = false);
        Task<DiretoriaViewModel> AddAsync(DiretoriaCreateEditViewModel viewModel);
        Task<DiretoriaViewModel?> UpdateAsync(int diretoriaId, DiretoriaCreateEditViewModel viewModel);
        Task<bool> DeleteAsync(int diretoriaId);
    }
}
