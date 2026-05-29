namespace ESCOLA_API.ViewModels
{
    /// <summary>
    /// Resposta com dados bancarios ficticios do aluno logado, imagem de QR Code e links de compartilhamento.
    /// </summary>
    public class AlunoQrCodeBancarioViewModel
    {
        /// <summary>
        /// Identificador do usuario aluno.
        /// </summary>
        public int IdUsuario { get; set; }

        /// <summary>
        /// Nome do aluno.
        /// </summary>
        public string NomeAluno { get; set; } = string.Empty;

        /// <summary>
        /// E-mail do aluno.
        /// </summary>
        public string EmailAluno { get; set; } = string.Empty;

        /// <summary>
        /// Dados bancarios ficticios usados no QR Code.
        /// </summary>
        public DadosBancariosFicticiosViewModel DadosBancarios { get; set; } = new();

        /// <summary>
        /// Texto bruto codificado no QR Code.
        /// </summary>
        public string ConteudoQrCode { get; set; } = string.Empty;

        /// <summary>
        /// Imagem PNG do QR Code em base64.
        /// </summary>
        public string QrCodeBase64 { get; set; } = string.Empty;

        /// <summary>
        /// Data URL da imagem PNG do QR Code.
        /// </summary>
        public string QrCodeDataUrl { get; set; } = string.Empty;

        /// <summary>
        /// Texto pronto para compartilhamento.
        /// </summary>
        public string TextoCompartilhamento { get; set; } = string.Empty;

        /// <summary>
        /// Link mailto com assunto e corpo preenchidos.
        /// </summary>
        public string EmailCompartilhamentoUrl { get; set; } = string.Empty;

        /// <summary>
        /// Link do WhatsApp com texto preenchido.
        /// </summary>
        public string WhatsAppCompartilhamentoUrl { get; set; } = string.Empty;

        /// <summary>
        /// Data e hora UTC em que os dados foram gerados.
        /// </summary>
        public DateTime GeradoEmUtc { get; set; }
    }

    /// <summary>
    /// Dados bancarios ficticios para uso escolar, sem validade financeira real.
    /// </summary>
    public class DadosBancariosFicticiosViewModel
    {
        /// <summary>
        /// Nome ficticio do banco.
        /// </summary>
        public string Banco { get; set; } = string.Empty;

        /// <summary>
        /// Agencia ficticia.
        /// </summary>
        public string Agencia { get; set; } = string.Empty;

        /// <summary>
        /// Conta ficticia.
        /// </summary>
        public string Conta { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de conta ficticia.
        /// </summary>
        public string TipoConta { get; set; } = string.Empty;

        /// <summary>
        /// Nome do favorecido ficticio.
        /// </summary>
        public string Favorecido { get; set; } = string.Empty;

        /// <summary>
        /// Documento ficticio do favorecido.
        /// </summary>
        public string DocumentoFicticio { get; set; } = string.Empty;

        /// <summary>
        /// Chave Pix ficticia, sem validade bancaria real.
        /// </summary>
        public string ChavePixFicticia { get; set; } = string.Empty;

        /// <summary>
        /// Aviso de que os dados sao ficticios.
        /// </summary>
        public string Aviso { get; set; } = string.Empty;
    }
}
