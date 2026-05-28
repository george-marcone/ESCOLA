namespace ESCOLA_API.ViewModels
{
    public class HoleriteViewModel
    {
        public int IdHolerite { get; set; }
        public int IdUsuario { get; set; }
        public string NomeUsuario { get; set; } = string.Empty;
        public string PerfilUsuario { get; set; } = string.Empty;
        public int CompetenciaMes { get; set; }
        public int CompetenciaAno { get; set; }
        public string Competencia { get; set; } = string.Empty;
        public string NomeOriginal { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long TamanhoBytes { get; set; }
        public DateTime CriadoEmUtc { get; set; }
    }
}
