using System.Linq;
using form_API.Models;

namespace form_API.ViewModels
{
    public static class MappingExtensions
    {
        public static AlunoViewModel ToViewModel(this Aluno entity)
        {
            return new AlunoViewModel
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Sobrenome = entity.Sobrenome,
                DataNasc = entity.DataNasc,
                ProfessorId = entity.ProfessorId,
                Professor = entity.Professor == null ? null : new ProfessorSummaryViewModel
                {
                    Id = entity.Professor.Id,
                    Nome = entity.Professor.Nome
                }
            };
        }

        public static AlunoSummaryViewModel ToSummary(this Aluno entity)
        {
            return new AlunoSummaryViewModel
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Sobrenome = entity.Sobrenome,
                ProfessorId = entity.ProfessorId
            };
        }

        public static ProfessorViewModel ToViewModel(this Professor entity)
        {
            return new ProfessorViewModel
            {
                Id = entity.Id,
                Nome = entity.Nome,
                Alunos = entity.Alunos?.Select(a => a.ToSummary()).ToArray() ?? new AlunoSummaryViewModel[0]
            };
        }

        public static Aluno ToModel(this AlunoCreateEditViewModel viewModel)
        {
            return new Aluno
            {
                Nome = viewModel.Nome,
                Sobrenome = viewModel.Sobrenome,
                DataNasc = viewModel.DataNasc,
                ProfessorId = viewModel.ProfessorId
            };
        }

        public static void UpdateFrom(this Aluno entity, AlunoCreateEditViewModel viewModel)
        {
            entity.Nome = viewModel.Nome;
            entity.Sobrenome = viewModel.Sobrenome;
            entity.DataNasc = viewModel.DataNasc;
            entity.ProfessorId = viewModel.ProfessorId;
        }

        public static Professor ToModel(this ProfessorCreateEditViewModel viewModel)
        {
            return new Professor
            {
                Nome = viewModel.Nome
            };
        }
    }
}
