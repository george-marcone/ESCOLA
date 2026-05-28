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
}
