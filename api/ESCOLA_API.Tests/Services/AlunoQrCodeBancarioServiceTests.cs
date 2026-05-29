using System.Security.Claims;
using ESCOLA_API.Data;
using ESCOLA_API.Models;
using ESCOLA_API.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ESCOLA_API.Tests.Services
{
    public class AlunoQrCodeBancarioServiceTests
    {
        [Fact]
        public async Task GerarParaAlunoLogadoAsync_WhenAlunoIsAuthenticated_ReturnsQrCodeAndShareLinks()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            await using var context = CreateContext(connection);
            await context.Database.EnsureCreatedAsync();

            var service = new AlunoQrCodeBancarioService(context);

            var result = await service.GerarParaAlunoLogadoAsync(CreatePrincipal(12, PerfilSistema.Aluno));

            Assert.NotNull(result);
            Assert.Equal(12, result!.IdUsuario);
            Assert.Equal("Aluno Maria", result.NomeAluno);
            Assert.Equal("Banco Escola Ficticio S.A.", result.DadosBancarios.Banco);
            Assert.Contains("Dados ficticios", result.DadosBancarios.Aviso);
            Assert.Contains("DADOS BANCARIOS FICTICIOS", result.ConteudoQrCode);
            Assert.Contains("Aluno Maria", result.ConteudoQrCode);
            Assert.StartsWith("data:image/png;base64,", result.QrCodeDataUrl);
            Assert.False(string.IsNullOrWhiteSpace(result.QrCodeBase64));
            Assert.Equal(new byte[] { 137, 80, 78, 71 }, Convert.FromBase64String(result.QrCodeBase64).Take(4).ToArray());
            Assert.StartsWith("mailto:?subject=", result.EmailCompartilhamentoUrl);
            Assert.StartsWith("https://wa.me/?text=", result.WhatsAppCompartilhamentoUrl);
            Assert.Contains("Aluno Maria", Uri.UnescapeDataString(result.TextoCompartilhamento));
        }

        [Fact]
        public async Task GerarParaAlunoLogadoAsync_WhenProfessorTriesToGenerate_ThrowsUnauthorizedAccessException()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            await using var context = CreateContext(connection);
            await context.Database.EnsureCreatedAsync();

            var service = new AlunoQrCodeBancarioService(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.GerarParaAlunoLogadoAsync(CreatePrincipal(2, PerfilSistema.Professor)));
        }

        [Fact]
        public async Task GerarParaAlunoLogadoAsync_WhenAlunoDoesNotExist_ReturnsNull()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            await using var context = CreateContext(connection);
            await context.Database.EnsureCreatedAsync();

            var service = new AlunoQrCodeBancarioService(context);

            var result = await service.GerarParaAlunoLogadoAsync(CreatePrincipal(999, PerfilSistema.Aluno));

            Assert.Null(result);
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
    }
}
