namespace ESCOLA_API.ViewModels
{
    public class AlunoQrCodeBancarioViewModel
    {
        public int IdUsuario { get; set; }
        public string NomeAluno { get; set; } = string.Empty;
        public string EmailAluno { get; set; } = string.Empty;
        public DadosBancariosFicticiosViewModel DadosBancarios { get; set; } = new();
        public string ConteudoQrCode { get; set; } = string.Empty;
        public string QrCodeBase64 { get; set; } = string.Empty;
        public string QrCodeDataUrl { get; set; } = string.Empty;
        public string TextoCompartilhamento { get; set; } = string.Empty;
        public string EmailCompartilhamentoUrl { get; set; } = string.Empty;
        public string WhatsAppCompartilhamentoUrl { get; set; } = string.Empty;
        public DateTime GeradoEmUtc { get; set; }
    }

    public class DadosBancariosFicticiosViewModel
    {
        public string Banco { get; set; } = string.Empty;
        public string Agencia { get; set; } = string.Empty;
        public string Conta { get; set; } = string.Empty;
        public string TipoConta { get; set; } = string.Empty;
        public string Favorecido { get; set; } = string.Empty;
        public string DocumentoFicticio { get; set; } = string.Empty;
        public string ChavePixFicticia { get; set; } = string.Empty;
        public string Aviso { get; set; } = string.Empty;
    }
}
