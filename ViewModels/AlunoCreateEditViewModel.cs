namespace form_API.ViewModels
{
    public class AlunoCreateEditViewModel
    {
        public string Nome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;
        public string DataNasc { get; set; } = string.Empty;
        public int ProfessorId { get; set; }
        public int? IdUsuario { get; set; }
    }
}
