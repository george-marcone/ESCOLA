using System.Collections.Generic;

namespace form_API.Models
{
    public class Perfil
    {
        public int IdPerfil { get; set; }
        public string DescricaoPerfil { get; set; } = string.Empty;
        public List<Usuario> Usuarios { get; set; } = new();
    }
}
