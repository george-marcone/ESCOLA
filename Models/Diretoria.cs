namespace form_API.Models
{
    public class Diretoria
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int? IdUsuario { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
