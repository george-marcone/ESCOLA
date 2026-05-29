using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace ESCOLA_API.Services
{
    public class AzureBlobUsuarioArquivoStorage : IUsuarioArquivoStorage
    {
        private readonly BlobContainerClient _containerClient;
        private readonly string? _publicBaseUrl;

        public AzureBlobUsuarioArquivoStorage(IConfiguration configuration)
        {
            var connectionString = configuration["AzureBlob:ConnectionString"]
                ?? configuration["AzureStorage:ConnectionString"];
            var containerName = configuration["AzureBlob:ContainerName"]
                ?? configuration["AzureStorage:ContainerName"]
                ?? "arquivos";

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("AzureBlob:ConnectionString nao configurada.");
            }

            _containerClient = new BlobContainerClient(connectionString, containerName);
            _publicBaseUrl = (configuration["AzureBlob:PublicBaseUrl"]
                ?? configuration["AzureStorage:PublicBaseUrl"])?.TrimEnd('/');
        }

        public async Task<ArquivoSalvo> SalvarAsync(int usuarioId, string categoria, IFormFile arquivo)
        {
            await _containerClient.CreateIfNotExistsAsync();

            var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            var nomeBlob = $"usuarios/{usuarioId}/{categoria}/{Guid.NewGuid():N}{extensao}";
            var blobClient = _containerClient.GetBlobClient(nomeBlob);

            await using var stream = arquivo.OpenReadStream();
            await blobClient.UploadAsync(stream, new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = arquivo.ContentType
                }
            });

            return new ArquivoSalvo
            {
                NomeBlob = nomeBlob,
                Url = BuildPublicUrl(blobClient, nomeBlob)
            };
        }

        public async Task<ArquivoDownload?> AbrirAsync(string? nomeBlob, string? url, string? nomeOriginal, string? contentType)
        {
            var blobName = !string.IsNullOrWhiteSpace(nomeBlob)
                ? nomeBlob
                : TryGetBlobNameFromUrl(url);

            if (string.IsNullOrWhiteSpace(blobName))
            {
                return null;
            }

            var blobClient = _containerClient.GetBlobClient(blobName);
            if (!await blobClient.ExistsAsync())
            {
                return null;
            }

            var download = await blobClient.DownloadStreamingAsync();
            return new ArquivoDownload
            {
                Stream = download.Value.Content,
                ContentType = string.IsNullOrWhiteSpace(contentType)
                    ? download.Value.Details.ContentType ?? "application/octet-stream"
                    : contentType,
                NomeArquivo = string.IsNullOrWhiteSpace(nomeOriginal)
                    ? Path.GetFileName(blobName)
                    : nomeOriginal
            };
        }

        public async Task RemoverAsync(string? nomeBlob, string? url)
        {
            var blobName = !string.IsNullOrWhiteSpace(nomeBlob)
                ? nomeBlob
                : TryGetBlobNameFromUrl(url);

            if (string.IsNullOrWhiteSpace(blobName))
            {
                return;
            }

            await _containerClient.GetBlobClient(blobName).DeleteIfExistsAsync();
        }

        private string BuildPublicUrl(BlobClient blobClient, string nomeBlob)
        {
            if (!string.IsNullOrWhiteSpace(_publicBaseUrl))
            {
                return $"{_publicBaseUrl}/{Uri.EscapeDataString(nomeBlob).Replace("%2F", "/", StringComparison.OrdinalIgnoreCase)}";
            }

            return blobClient.Uri.ToString();
        }

        private string? TryGetBlobNameFromUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url) || !Uri.TryCreate(url, UriKind.Absolute, out var uri))
            {
                return null;
            }

            if (!string.IsNullOrWhiteSpace(_publicBaseUrl)
                && url.StartsWith($"{_publicBaseUrl}/", StringComparison.OrdinalIgnoreCase))
            {
                return Uri.UnescapeDataString(url[(_publicBaseUrl.Length + 1)..]);
            }

            var path = uri.AbsolutePath.Trim('/');
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length < 2 || !segments[0].Equals(_containerClient.Name, StringComparison.OrdinalIgnoreCase))
            {
                return path.StartsWith("usuarios/", StringComparison.OrdinalIgnoreCase)
                    ? Uri.UnescapeDataString(path)
                    : null;
            }

            return Uri.UnescapeDataString(string.Join('/', segments.Skip(1)));
        }
    }
}
