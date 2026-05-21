namespace form_API.ViewModels
{
    public class DiretoriaCreateEditViewModel
    {
        public string Nome { get; set; } = string.Empty;
        public int? IdUsuario { get; set; }
    }

    public class DiretoriaViewModel
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int? IdUsuario { get; set; }
        public UsuarioSummaryViewModel? Usuario { get; set; }
    }
}
