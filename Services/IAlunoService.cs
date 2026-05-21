using System.Threading.Tasks;
using form_API.ViewModels;

namespace form_API.Services
{
    public interface IAlunoService
    {
        Task<AlunoViewModel[]> GetAllAsync(bool includeProfessor = false);
        Task<AlunoViewModel?> GetByIdAsync(int alunoId, bool includeProfessor = false);
        Task<AlunoViewModel[]> GetByProfessorIdAsync(int professorId, bool includeProfessor = false);
        Task<AlunoViewModel> AddAsync(AlunoCreateEditViewModel viewModel);
        Task<AlunoViewModel?> UpdateAsync(int alunoId, AlunoCreateEditViewModel viewModel);
        Task<bool> DeleteAsync(int alunoId);
    }
}
