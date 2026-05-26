namespace ESCOLA_API.Models
{
    public class UsuarioArquivo
    {
        public int IdUsuarioArquivo { get; set; }
        public int IdUsuario { get; set; }
        public Usuario? Usuario { get; set; }
        public string TipoArquivo { get; set; } = string.Empty;
        public string NomeOriginal { get; set; } = string.Empty;
        public string CaminhoRelativo { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long TamanhoBytes { get; set; }
        public DateTime CriadoEmUtc { get; set; } = DateTime.UtcNow;
    }
}
