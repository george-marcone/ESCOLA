namespace ESCOLA_API.Models
{
    public class CalendarioEscolarEvento
    {
        public int IdEventoCalendarioEscolar { get; set; }
        public DateOnly Data { get; set; }
        public string Tipo { get; set; } = "Evento";
        public string Titulo { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string PublicoAlvo { get; set; } = "Todos";
        public int IdUsuarioCriador { get; set; }
        public Usuario? UsuarioCriador { get; set; }
        public string NomeUsuarioCriador { get; set; } = string.Empty;
        public DateTime CriadoEmUtc { get; set; } = DateTime.UtcNow;
    }
}
