namespace form_API.ViewModels
{
    public class UsuarioSummaryViewModel
    {
        public int IdUsuario { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Telefone { get; set; } = string.Empty;
        public int IdPerfil { get; set; }
        public string DescricaoPerfil { get; set; } = string.Empty;
    }
}
