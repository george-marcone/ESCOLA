namespace form_API.ViewModels
{
    public class LoginRequestViewModel
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }

    public class AuthResponseViewModel
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiraEm { get; set; }
        public UsuarioSummaryViewModel Usuario { get; set; } = new();
    }
}
