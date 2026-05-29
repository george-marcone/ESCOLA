using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace ESCOLA_API.Services
{
    public class LocalUsuarioArquivoStorage : IUsuarioArquivoStorage
    {
        private readonly string _uploadRoot;

        public LocalUsuarioArquivoStorage(IHostEnvironment environment, IConfiguration configuration)
        {
            _uploadRoot = ResolveUploadRoot(environment, configuration);
        }

        public async Task<ArquivoSalvo> SalvarAsync(int usuarioId, string categoria, IFormFile arquivo)
        {
            var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
            var nomeArquivo = $"{Guid.NewGuid():N}{extensao}";
            var relativeDirectory = Path.Combine("usuarios", usuarioId.ToString(), categoria);
            var targetDirectory = Path.Combine(_uploadRoot, relativeDirectory);
            Directory.CreateDirectory(targetDirectory);

            var targetPath = Path.Combine(targetDirectory, nomeArquivo);
            await using var stream = File.Create(targetPath);
            await arquivo.CopyToAsync(stream);

            var nomeBlob = $"{relativeDirectory.Replace('\\', '/')}/{nomeArquivo}";
            return new ArquivoSalvo
            {
                NomeBlob = nomeBlob,
                Url = $"/uploads/{nomeBlob}"
            };
        }

        public Task<ArquivoDownload?> AbrirAsync(string? nomeBlob, string? url, string? nomeOriginal, string? contentType)
        {
            var relative = !string.IsNullOrWhiteSpace(nomeBlob)
                ? nomeBlob
                : ObterCaminhoRelativo(url);

            if (string.IsNullOrWhiteSpace(relative))
            {
                return Task.FromResult<ArquivoDownload?>(null);
            }

            var fullPath = ResolveSafePath(relative);
            if (fullPath == null || !File.Exists(fullPath))
            {
                return Task.FromResult<ArquivoDownload?>(null);
            }

            var contentTypeFinal = contentType;
            if (string.IsNullOrWhiteSpace(contentTypeFinal))
            {
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(fullPath, out contentTypeFinal))
                {
                    contentTypeFinal = "application/octet-stream";
                }
            }

            return Task.FromResult<ArquivoDownload?>(new ArquivoDownload
            {
                Stream = File.OpenRead(fullPath),
                ContentType = contentTypeFinal,
                NomeArquivo = string.IsNullOrWhiteSpace(nomeOriginal) ? Path.GetFileName(fullPath) : nomeOriginal
            });
        }

        public Task RemoverAsync(string? nomeBlob, string? url)
        {
            var relative = !string.IsNullOrWhiteSpace(nomeBlob)
                ? nomeBlob
                : ObterCaminhoRelativo(url);

            if (string.IsNullOrWhiteSpace(relative))
            {
                return Task.CompletedTask;
            }

            var fullPath = ResolveSafePath(relative);
            if (fullPath != null && File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }

        private static string? ObterCaminhoRelativo(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            if (url.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            {
                return url["/uploads/".Length..];
            }

            if (Uri.TryCreate(url, UriKind.Absolute, out var uri)
                && uri.AbsolutePath.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            {
                return uri.AbsolutePath["/uploads/".Length..];
            }

            return null;
        }

        private string? ResolveSafePath(string relative)
        {
            var fullPath = Path.GetFullPath(Path.Combine(_uploadRoot, relative.Replace('/', Path.DirectorySeparatorChar)));
            var fullRoot = Path.GetFullPath(_uploadRoot).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var safePrefix = fullRoot + Path.DirectorySeparatorChar;

            return fullPath.StartsWith(safePrefix, StringComparison.OrdinalIgnoreCase)
                ? fullPath
                : null;
        }

        private static string ResolveUploadRoot(IHostEnvironment environment, IConfiguration configuration)
        {
            var configured = configuration["Uploads:RootPath"];
            if (!string.IsNullOrWhiteSpace(configured))
            {
                return Path.IsPathRooted(configured)
                    ? configured
                    : Path.Combine(environment.ContentRootPath, configured);
            }

            return Path.Combine(environment.ContentRootPath, "uploads");
        }
    }
}
