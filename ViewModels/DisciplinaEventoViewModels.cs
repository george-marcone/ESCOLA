namespace ESCOLA_API.ViewModels
{
    /// <summary>
    /// Evento escolar vinculado a uma disciplina, como avaliacao ou entrega de trabalho.
    /// </summary>
    public class DisciplinaEventoViewModel
    {
        /// <summary>
        /// Identificador do evento da disciplina.
        /// </summary>
        public int IdEventoDisciplina { get; set; }

        /// <summary>
        /// Identificador da disciplina.
        /// </summary>
        public int IdDisciplina { get; set; }

        /// <summary>
        /// Nome da disciplina.
        /// </summary>
        public string NomeDisciplina { get; set; } = string.Empty;

        /// <summary>
        /// Identificador do usuario professor responsavel.
        /// </summary>
        public int IdProfessorUsuario { get; set; }

        /// <summary>
        /// Nome do professor responsavel.
        /// </summary>
        public string NomeProfessor { get; set; } = string.Empty;

        /// <summary>
        /// Tipo do evento: Avaliacao ou Trabalho.
        /// </summary>
        public string Tipo { get; set; } = string.Empty;

        /// <summary>
        /// Titulo do evento.
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Descricao opcional do evento.
        /// </summary>
        public string? Descricao { get; set; }

        /// <summary>
        /// Data do evento no formato ISO yyyy-MM-dd.
        /// </summary>
        public DateOnly Data { get; set; }

        /// <summary>
        /// Data e hora UTC de criacao.
        /// </summary>
        public DateTime CriadoEmUtc { get; set; }

        /// <summary>
        /// Data e hora UTC da ultima atualizacao.
        /// </summary>
        public DateTime? AtualizadoEmUtc { get; set; }
    }

    /// <summary>
    /// Payload para criar ou editar evento de disciplina.
    /// </summary>
    public class DisciplinaEventoCreateUpdateViewModel
    {
        /// <summary>
        /// Tipo do evento: Avaliacao ou Trabalho.
        /// </summary>
        public string Tipo { get; set; } = string.Empty;

        /// <summary>
        /// Titulo do evento.
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Descricao opcional do evento.
        /// </summary>
        public string? Descricao { get; set; }

        /// <summary>
        /// Data do evento no formato ISO yyyy-MM-dd.
        /// </summary>
        public DateOnly Data { get; set; }
    }
}
