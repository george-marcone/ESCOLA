using System.Security.Claims;
using ESCOLA_API.Data;
using ESCOLA_API.Models;
using ESCOLA_API.Services;
using ESCOLA_API.ViewModels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ESCOLA_API.Tests.Services
{
    public class CadernetaDigitalServiceTests
    {
        [Fact]
        public async Task AddAsync_WhenProfessorAssociatesAlunoToDisciplina_CreatesCaderneta()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            await using var context = CreateContext(connection);
            await context.Database.EnsureCreatedAsync();

            var service = new CadernetaDigitalService(context);
            var professor = CreatePrincipal(2, PerfilSistema.Professor);
            var disciplina = await service.AddDisciplinaAsync(new DisciplinaCreateUpdateViewModel
            {
                Nome = "Matematica"
            }, professor);

            var created = await service.AddAsync(new CadernetaDigitalCreateUpdateViewModel
            {
                IdAlunoUsuario = 12,
                IdDisciplina = disciplina.IdDisciplina,
                Notas = new[] { 8.5m, 9m },
                Presencas = 18,
                Faltas = 2
            }, professor);

            Assert.Equal("Aluno Maria", created.NomeAluno);
            Assert.Equal("Matematica", created.NomeDisciplina);
            Assert.Equal(new[] { 8.5m, 9m }, created.Notas);
            Assert.Equal(18, created.Presencas);
            Assert.Equal(2, created.Faltas);
        }

        [Fact]
        public async Task GetAllAsync_WhenAlunoIsAssociated_ReturnsOnlyOwnCadernetas()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            await using var context = CreateContext(connection);
            await context.Database.EnsureCreatedAsync();

            var service = new CadernetaDigitalService(context);
            var professor = CreatePrincipal(2, PerfilSistema.Professor);
            var disciplina = await service.AddDisciplinaAsync(new DisciplinaCreateUpdateViewModel
            {
                Nome = "Portugues"
            }, professor);

            await service.AddAsync(new CadernetaDigitalCreateUpdateViewModel
            {
                IdAlunoUsuario = 12,
                IdDisciplina = disciplina.IdDisciplina,
                Notas = new[] { 7m },
                Presencas = 10,
                Faltas = 1
            }, professor);

            var cadernetasDoAluno = await service.GetAllAsync(CreatePrincipal(12, PerfilSistema.Aluno));
            var cadernetasDeOutroAluno = await service.GetAllAsync(CreatePrincipal(13, PerfilSistema.Aluno));

            Assert.Single(cadernetasDoAluno);
            Assert.Empty(cadernetasDeOutroAluno);
        }

        [Fact]
        public async Task AddAsync_WhenAdminTriesToCreate_ThrowsUnauthorizedAccessException()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            await using var context = CreateContext(connection);
            await context.Database.EnsureCreatedAsync();

            var service = new CadernetaDigitalService(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.AddDisciplinaAsync(new DisciplinaCreateUpdateViewModel
                {
                    Nome = "Historia"
                }, CreatePrincipal(1, PerfilSistema.Administrador)));
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
