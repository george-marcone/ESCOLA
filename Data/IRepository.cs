using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using form_API.Models;

namespace form_API.Data
{
    public interface IRepository
    {
        // Gerais
        void Add<T>(T entity) where T : class;

        void Update<T>(T entity) where T : class;

        void Delete<T>(T entity) where T : class;

        Task<bool> SaveChangesAsync();

        // Alunos
        Task<Aluno[]> GetAllAlunosAsync(bool includeProfessor);
        Task<Aluno[]> GetAlunoAsyncByProfessorId(int ProfessorId, bool includeProfessor);
        Task<Aluno?> GetAlunoAsyncById(int AlunoId, bool includeProfessor);

        // Professor
        Task<Professor[]> GetAllProfessoresAsync(bool includeAluno);
        Task<Professor?> GetProfessorAsyncById(int ProfessorId, bool includeAluno);
    }
}