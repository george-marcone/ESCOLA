using System.Linq;
using form_API.Data;
using form_API.ViewModels;

namespace form_API.Services
{
    public class DiretoriaService : IDiretoriaService
    {
        private readonly IRepository _repo;

        public DiretoriaService(IRepository repo)
        {
            _repo = repo;
        }

        public async Task<DiretoriaViewModel[]> GetAllAsync(bool includeUsuario = false)
        {
            var diretorias = await _repo.GetAllDiretoriasAsync(includeUsuario);
            return diretorias.Select(d => d.ToViewModel()).ToArray();
        }

        public async Task<DiretoriaViewModel?> GetByIdAsync(int diretoriaId, bool includeUsuario = false)
        {
            var diretoria = await _repo.GetDiretoriaAsyncById(diretoriaId, includeUsuario);
            return diretoria?.ToViewModel();
        }

        public async Task<DiretoriaViewModel> AddAsync(DiretoriaCreateEditViewModel viewModel)
        {
            var entity = viewModel.ToModel();
            _repo.Add(entity);
            await _repo.SaveChangesAsync();
            var created = await _repo.GetDiretoriaAsyncById(entity.Id, true);
            return created?.ToViewModel() ?? entity.ToViewModel();
        }

        public async Task<DiretoriaViewModel?> UpdateAsync(int diretoriaId, DiretoriaCreateEditViewModel viewModel)
        {
            var diretoria = await _repo.GetDiretoriaAsyncById(diretoriaId, false);
            if (diretoria == null)
            {
                return null;
            }

            diretoria.UpdateFrom(viewModel);
            _repo.Update(diretoria);
            await _repo.SaveChangesAsync();
            var updated = await _repo.GetDiretoriaAsyncById(diretoriaId, true);
            return updated?.ToViewModel();
        }

        public async Task<bool> DeleteAsync(int diretoriaId)
        {
            var diretoria = await _repo.GetDiretoriaAsyncById(diretoriaId, false);
            if (diretoria == null)
            {
                return false;
            }

            _repo.Delete(diretoria);
            return await _repo.SaveChangesAsync();
        }
    }
}
