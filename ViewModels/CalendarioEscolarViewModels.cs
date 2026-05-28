namespace ESCOLA_API.ViewModels
{
    /// <summary>
    /// Calendario escolar anual com meses, dias e feriados nacionais brasileiros.
    /// </summary>
    public class CalendarioEscolarAnoViewModel
    {
        /// <summary>
        /// Ano do calendario.
        /// </summary>
        public int Ano { get; set; }

        /// <summary>
        /// Mes recomendado para destaque inicial no front.
        /// </summary>
        public int MesSelecionado { get; set; }

        /// <summary>
        /// Meses do ano.
        /// </summary>
        public CalendarioEscolarMesViewModel[] Meses { get; set; } = [];

        /// <summary>
        /// Lista consolidada de feriados nacionais do ano.
        /// </summary>
        public FeriadoNacionalViewModel[] FeriadosNacionais { get; set; } = [];

        /// <summary>
        /// Eventos escolares cadastrados para o ano.
        /// </summary>
        public CalendarioEscolarEventoViewModel[] Eventos { get; set; } = [];
    }

    /// <summary>
    /// Mes do calendario escolar.
    /// </summary>
    public class CalendarioEscolarMesViewModel
    {
        /// <summary>
        /// Numero do mes, de 1 a 12.
        /// </summary>
        public int Mes { get; set; }

        /// <summary>
        /// Nome do mes em portugues do Brasil.
        /// </summary>
        public string NomeMes { get; set; } = string.Empty;

        /// <summary>
        /// Dias do mes.
        /// </summary>
        public CalendarioEscolarDiaViewModel[] Dias { get; set; } = [];
    }

    /// <summary>
    /// Dia do calendario escolar.
    /// </summary>
    public class CalendarioEscolarDiaViewModel
    {
        /// <summary>
        /// Data no formato ISO yyyy-MM-dd.
        /// </summary>
        public DateOnly Data { get; set; }

        /// <summary>
        /// Dia do mes.
        /// </summary>
        public int Dia { get; set; }

        /// <summary>
        /// Nome do dia da semana em portugues do Brasil.
        /// </summary>
        public string DiaSemana { get; set; } = string.Empty;

        /// <summary>
        /// Indica sabado ou domingo.
        /// </summary>
        public bool FinalDeSemana { get; set; }

        /// <summary>
        /// Indica se a data e feriado nacional brasileiro.
        /// </summary>
        public bool FeriadoNacional { get; set; }

        /// <summary>
        /// Nome do feriado, quando houver.
        /// </summary>
        public string? NomeFeriado { get; set; }

        /// <summary>
        /// Eventos escolares marcados para a data.
        /// </summary>
        public CalendarioEscolarEventoViewModel[] Eventos { get; set; } = [];
    }

    /// <summary>
    /// Feriado nacional brasileiro.
    /// </summary>
    public class FeriadoNacionalViewModel
    {
        /// <summary>
        /// Data do feriado.
        /// </summary>
        public DateOnly Data { get; set; }

        /// <summary>
        /// Nome oficial ou usual do feriado.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Tipo do dia no calendario.
        /// </summary>
        public string Tipo { get; set; } = "Feriado Nacional";
    }

    /// <summary>
    /// Evento geral do calendario escolar, como festa, reuniao com professores ou reuniao de pais e mestres.
    /// </summary>
    public class CalendarioEscolarEventoViewModel
    {
        /// <summary>
        /// Identificador do evento.
        /// </summary>
        public int IdEventoCalendarioEscolar { get; set; }

        /// <summary>
        /// Data do evento.
        /// </summary>
        public DateOnly Data { get; set; }

        /// <summary>
        /// Tipo do evento, como FestaEscola, ReuniaoProfessores ou ReuniaoPaisMestres.
        /// </summary>
        public string Tipo { get; set; } = string.Empty;

        /// <summary>
        /// Titulo do evento.
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Descricao opcional.
        /// </summary>
        public string? Descricao { get; set; }

        /// <summary>
        /// Publico alvo normalizado do evento.
        /// </summary>
        public string PublicoAlvo { get; set; } = string.Empty;

        /// <summary>
        /// Perfis que devem receber notificacao sobre o evento.
        /// </summary>
        public string[] PerfisDestinatarios { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Identificador do administrador que cadastrou o evento.
        /// </summary>
        public int IdUsuarioCriador { get; set; }

        /// <summary>
        /// Nome do administrador que cadastrou o evento.
        /// </summary>
        public string NomeUsuarioCriador { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade de notificacoes criadas no cadastro do evento.
        /// </summary>
        public int TotalNotificados { get; set; }

        /// <summary>
        /// Data e hora UTC de criacao.
        /// </summary>
        public DateTime CriadoEmUtc { get; set; }
    }

    /// <summary>
    /// Payload para o administrador cadastrar evento geral no calendario escolar.
    /// </summary>
    public class CalendarioEscolarEventoCreateViewModel
    {
        /// <summary>
        /// Data do evento.
        /// </summary>
        public DateOnly Data { get; set; }

        /// <summary>
        /// Tipo do evento. Quando informado como ReuniaoProfessores, notifica professores; quando ReuniaoPaisMestres, notifica alunos e professores; demais tipos usam Todos por padrao.
        /// </summary>
        public string Tipo { get; set; } = "Evento";

        /// <summary>
        /// Titulo do evento.
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Descricao opcional.
        /// </summary>
        public string? Descricao { get; set; }

        /// <summary>
        /// Publico alvo opcional: Todos, Professores, Alunos, Administradores ou AlunosEProfessores.
        /// </summary>
        public string? PublicoAlvo { get; set; }
    }
}
