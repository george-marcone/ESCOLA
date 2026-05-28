namespace ESCOLA_API.Models
{
    public class Holerite
    {
        public int IdHolerite { get; set; }
        public int IdUsuario { get; set; }
        public Usuario? Usuario { get; set; }
        public int CompetenciaMes { get; set; }
        public int CompetenciaAno { get; set; }
        public string NomeOriginal { get; set; } = string.Empty;
        public string NomeBlob { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long TamanhoBytes { get; set; }
        public DateTime CriadoEmUtc { get; set; } = DateTime.UtcNow;
    }
}
