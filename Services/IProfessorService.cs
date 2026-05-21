using System.Threading.Tasks;
using form_API.ViewModels;

namespace form_API.Services
{
    public interface IProfessorService
    {
        Task<ProfessorViewModel[]> GetAllAsync(bool includeAlunos = false);
        Task<ProfessorViewModel?> GetByIdAsync(int professorId, bool includeAlunos = false);
        Task<ProfessorViewModel> AddAsync(ProfessorCreateEditViewModel viewModel);
        Task<ProfessorViewModel?> UpdateAsync(int professorId, ProfessorCreateEditViewModel viewModel);
        Task<bool> DeleteAsync(int professorId);
    }
}
