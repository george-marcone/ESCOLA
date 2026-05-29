using System.Linq;
using System.Threading.Tasks;
using ESCOLA_API.Data;
using ESCOLA_API.ViewModels;

namespace ESCOLA_API.Services
{
    public class AlunoService : IAlunoService
    {
        private readonly IRepository _repo;

        public AlunoService(IRepository repo)
        {
            _repo = repo;
        }

        public async Task<AlunoViewModel[]> GetAllAsync(bool includeProfessor = false)
        {
            var alunos = await _repo.GetAllAlunosAsync(includeProfessor);
            return alunos.Select(a => a.ToViewModel()).ToArray();
        }

        public async Task<AlunoViewModel?> GetByIdAsync(int alunoId, bool includeProfessor = false)
        {
            var aluno = await _repo.GetAlunoAsyncById(alunoId, includeProfessor);
            return aluno?.ToViewModel();
        }

        public async Task<AlunoViewModel[]> GetByProfessorIdAsync(int professorId, bool includeProfessor = false)
        {
            var alunos = await _repo.GetAlunoAsyncByProfessorId(professorId, includeProfessor);
            return alunos.Select(a => a.ToViewModel()).ToArray();
        }

        public async Task<AlunoViewModel> AddAsync(AlunoCreateEditViewModel viewModel)
        {
            var entity = viewModel.ToModel();
            _repo.Add(entity);
            await _repo.SaveChangesAsync();
            var created = await _repo.GetAlunoAsyncById(entity.Id, true);
            return created?.ToViewModel() ?? entity.ToViewModel();
        }

        public async Task<AlunoViewModel?> UpdateAsync(int alunoId, AlunoCreateEditViewModel viewModel)
        {
            var aluno = await _repo.GetAlunoAsyncById(alunoId, false);
            if (aluno == null)
            {
                return null;
            }

            aluno.UpdateFrom(viewModel);
            _repo.Update(aluno);
            await _repo.SaveChangesAsync();
            var updated = await _repo.GetAlunoAsyncById(alunoId, true);
            return updated?.ToViewModel();
        }

        public async Task<bool> DeleteAsync(int alunoId)
        {
            var aluno = await _repo.GetAlunoAsyncById(alunoId, false);
            if (aluno == null)
            {
                return false;
            }

            _repo.Delete(aluno);
            return await _repo.SaveChangesAsync();
        }
    }
}
