namespace ESCOLA_API.Models
{
    public class CadernetaDigital
    {
        public int IdCadernetaDigital { get; set; }
        public int IdAlunoUsuario { get; set; }
        public Usuario? AlunoUsuario { get; set; }
        public int IdDisciplina { get; set; }
        public Disciplina? Disciplina { get; set; }
        public string Notas { get; set; } = string.Empty;
        public int Presencas { get; set; }
        public int Faltas { get; set; }
    }
}
