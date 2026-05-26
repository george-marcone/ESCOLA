namespace ESCOLA_API.ViewModels
{
    public class NotificacaoViewModel
    {
        public int IdNotificacao { get; set; }
        public int IdUsuario { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public string? Link { get; set; }
        public int? IdCadernetaDigital { get; set; }
        public int? IdDisciplina { get; set; }
        public string? NomeDisciplina { get; set; }
        public decimal? MediaAritmetica { get; set; }
        public string? Situacao { get; set; }
        public string? CorSituacao { get; set; }
        public bool Lida { get; set; }
        public DateTime CriadaEmUtc { get; set; }
        public DateTime? LidaEmUtc { get; set; }
    }

    public class NotificacaoCreateViewModel
    {
        public int IdUsuario { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;
        public string Tipo { get; set; } = "Geral";
        public string? Link { get; set; }
    }
}
