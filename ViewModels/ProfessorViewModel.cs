using System.Collections.Generic;

namespace form_API.ViewModels
{
    public class ProfessorViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public IEnumerable<AlunoSummaryViewModel> Alunos { get; set; } = new List<AlunoSummaryViewModel>();
    }
}
