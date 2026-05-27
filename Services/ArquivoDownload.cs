namespace ESCOLA_API.Services
{
    public class ArquivoDownload
    {
        public Stream Stream { get; set; } = Stream.Null;
        public string ContentType { get; set; } = "application/octet-stream";
        public string NomeArquivo { get; set; } = "arquivo";
    }
}
