namespace ESCOLA_API.ViewModels
{
    public class DisciplinaViewModel
    {
        public int IdDisciplina { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int IdProfessorUsuario { get; set; }
        public string NomeProfessor { get; set; } = string.Empty;
    }

    public class DisciplinaCreateUpdateViewModel
    {
        public string Nome { get; set; } = string.Empty;
    }

    public class CadernetaDigitalViewModel
    {
        public int IdCadernetaDigital { get; set; }
        public int IdAlunoUsuario { get; set; }
        public string NomeAluno { get; set; } = string.Empty;
        public string EmailAluno { get; set; } = string.Empty;
        public int IdDisciplina { get; set; }
        public string NomeDisciplina { get; set; } = string.Empty;
        public int IdProfessorUsuario { get; set; }
        public string NomeProfessor { get; set; } = string.Empty;
        public decimal[] Notas { get; set; } = [];
        public int Presencas { get; set; }
        public int Faltas { get; set; }
    }

    public class CadernetaDigitalCreateUpdateViewModel
    {
        public int IdAlunoUsuario { get; set; }
        public int IdDisciplina { get; set; }
        public decimal[] Notas { get; set; } = [];
        public int Presencas { get; set; }
        public int Faltas { get; set; }
    }
}
