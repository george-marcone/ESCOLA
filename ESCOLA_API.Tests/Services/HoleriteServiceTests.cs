using System.Security.Claims;
using ESCOLA_API.Data;
using ESCOLA_API.Models;
using ESCOLA_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace ESCOLA_API.Tests.Services
{
    public class HoleriteServiceTests
    {
        [Fact]
        public async Task UploadHoleriteAsync_WhenAdminUploadsForProfessor_SavesAndAllowsProfessorDownload()
        {
            var uploadRoot = CreateUploadRoot();

            try
            {
                await using var connection = new SqliteConnection("DataSource=:memory:");
                await connection.OpenAsync();
                await using var context = CreateContext(connection);
                await context.Database.EnsureCreatedAsync();

                var service = CreateService(context, uploadRoot);
                var arquivo = CreateFormFile("holerite.pdf", "application/pdf", new byte[] { 37, 80, 68, 70 });

                var created = await service.UploadHoleriteAsync(2, 5, 2026, arquivo, CreatePrincipal(1, PerfilSistema.Administrador));

                Assert.True(created.IdHolerite > 0);
                Assert.Equal(2, created.IdUsuario);
                Assert.Equal("05/2026", created.Competencia);
                Assert.Equal("Professor Vinicius", created.NomeUsuario);
                Assert.True(File.Exists(ToPhysicalPath(uploadRoot, created.Url)));

                var professoresEsperados = await context.Usuarios
                    .CountAsync(usuario => usuario.IdPerfil == PerfilSistema.ProfessorId);
                var notificacoes = await context.Notificacoes
                    .Include(item => item.Usuario)
                    .Where(item => item.Tipo == "HoleriteLancado")
                    .ToArrayAsync();
                Assert.Equal(professoresEsperados, notificacoes.Length);
                Assert.All(notificacoes, notificacao =>
                {
                    Assert.Equal(PerfilSistema.ProfessorId, notificacao.Usuario!.IdPerfil);
                    Assert.Contains("Administrador Sistema", notificacao.Mensagem);
                    Assert.Contains("05/2026", notificacao.Mensagem);
                    Assert.Contains("holerite.pdf", notificacao.Mensagem);
                    Assert.Contains("Professor Vinicius", notificacao.Mensagem);
                });

                var notificacaoFuncionario = Assert.Single(notificacoes, item => item.IdUsuario == 2);
                Assert.Contains("para voce", notificacaoFuncionario.Mensagem);
                Assert.Equal($"/holerite?holeriteId={created.IdHolerite}", notificacaoFuncionario.Link);

                var notificacaoOutroProfessor = Assert.Single(notificacoes, item => item.IdUsuario == 3);
                Assert.DoesNotContain("para voce", notificacaoOutroProfessor.Mensagem);
                Assert.Equal("/holerite", notificacaoOutroProfessor.Link);

                var meusHolerites = await service.GetMeusHoleritesAsync(CreatePrincipal(2, PerfilSistema.Professor));
                Assert.Single(meusHolerites);

                var download = await service.DownloadMeuHoleriteAsync(created.IdHolerite, CreatePrincipal(2, PerfilSistema.Professor));
                Assert.NotNull(download);
                Assert.Equal("application/pdf", download!.ContentType);
                Assert.Equal("holerite.pdf", download.NomeArquivo);
                await using (download.Stream)
                {
                    Assert.True(download.Stream.Length > 0);
                }
            }
            finally
            {
                DeleteDirectory(uploadRoot);
            }
        }

        [Fact]
        public async Task UploadHoleriteAsync_WhenUsuarioIsAluno_ThrowsInvalidOperationException()
        {
            var uploadRoot = CreateUploadRoot();

            try
            {
                await using var connection = new SqliteConnection("DataSource=:memory:");
                await connection.OpenAsync();
                await using var context = CreateContext(connection);
                await context.Database.EnsureCreatedAsync();

                var service = CreateService(context, uploadRoot);
                var arquivo = CreateFormFile("holerite.pdf", "application/pdf", new byte[] { 37, 80, 68, 70 });

                var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                    service.UploadHoleriteAsync(12, 5, 2026, arquivo, CreatePrincipal(1, PerfilSistema.Administrador)));

                Assert.Equal("Holerites podem ser vinculados apenas a professores e administradores.", exception.Message);
            }
            finally
            {
                DeleteDirectory(uploadRoot);
            }
        }

        [Fact]
        public async Task GetMeusHoleritesAsync_WhenAlunoTriesToList_ThrowsUnauthorizedAccessException()
        {
            var uploadRoot = CreateUploadRoot();

            try
            {
                await using var connection = new SqliteConnection("DataSource=:memory:");
                await connection.OpenAsync();
                await using var context = CreateContext(connection);
                await context.Database.EnsureCreatedAsync();

                var service = CreateService(context, uploadRoot);

                await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                    service.GetMeusHoleritesAsync(CreatePrincipal(12, PerfilSistema.Aluno)));
            }
            finally
            {
                DeleteDirectory(uploadRoot);
            }
        }

        [Fact]
        public async Task CriarCompartilhamentoMeuHoleriteAsync_WhenProfessorOwnsHolerite_AllowsSharedDownload()
        {
            var uploadRoot = CreateUploadRoot();

            try
            {
                await using var connection = new SqliteConnection("DataSource=:memory:");
                await connection.OpenAsync();
                await using var context = CreateContext(connection);
                await context.Database.EnsureCreatedAsync();

                var service = CreateService(context, uploadRoot);
                var arquivo = CreateFormFile("holerite.pdf", "application/pdf", new byte[] { 37, 80, 68, 70 });
                var created = await service.UploadHoleriteAsync(2, 5, 2026, arquivo, CreatePrincipal(1, PerfilSistema.Administrador));

                var compartilhamento = await service.CriarCompartilhamentoMeuHoleriteAsync(
                    created.IdHolerite,
                    CreatePrincipal(2, PerfilSistema.Professor));

                Assert.NotNull(compartilhamento);
                Assert.False(string.IsNullOrWhiteSpace(compartilhamento!.Token));
                Assert.True(compartilhamento.ExpiraEmUtc > DateTime.UtcNow);

                var download = await service.DownloadHoleriteCompartilhadoAsync(compartilhamento.Token);

                Assert.NotNull(download);
                Assert.Equal("application/pdf", download!.ContentType);
                Assert.Equal("holerite.pdf", download.NomeArquivo);
                await using (download.Stream)
                {
                    Assert.True(download.Stream.Length > 0);
                }
            }
            finally
            {
                DeleteDirectory(uploadRoot);
            }
        }

        private static IFormFile CreateFormFile(string fileName, string contentType, byte[] content)
        {
            var stream = new MemoryStream(content);
            return new FormFile(stream, 0, content.Length, "arquivo", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
        }

        private static ClaimsPrincipal CreatePrincipal(int usuarioId, string perfil)
        {
            var identity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString()),
                new Claim(ClaimTypes.Role, perfil)
            }, "Test");

            return new ClaimsPrincipal(identity);
        }

        private static DataContext CreateContext(SqliteConnection connection)
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseSqlite(connection)
                .Options;

            return new DataContext(options);
        }

        private static HoleriteService CreateService(DataContext context, string uploadRoot)
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string?>("Uploads:RootPath", uploadRoot),
                    new KeyValuePair<string, string?>("Jwt:Key", "chave-de-teste-para-compartilhamento-de-holerites")
                })
                .Build();
            var storage = new LocalUsuarioArquivoStorage(new TestHostEnvironment(), configuration);

            return new HoleriteService(context, storage, configuration);
        }

        private static string CreateUploadRoot()
        {
            var uploadRoot = Path.Combine(Path.GetTempPath(), $"escola-api-holerite-tests-{Guid.NewGuid():N}");
            Directory.CreateDirectory(uploadRoot);
            return uploadRoot;
        }

        private static string ToPhysicalPath(string uploadRoot, string url)
        {
            var relative = url["/uploads/".Length..].Replace('/', Path.DirectorySeparatorChar);
            return Path.Combine(uploadRoot, relative);
        }

        private static void DeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }

        private sealed class TestHostEnvironment : IHostEnvironment
        {
            public string EnvironmentName { get; set; } = Environments.Development;
            public string ApplicationName { get; set; } = "ESCOLA_API.Tests";
            public string ContentRootPath { get; set; } = Directory.GetCurrentDirectory();
            public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
        }
    }
}
