using System.Security.Claims;
using ESCOLA_API.Data;
using ESCOLA_API.Models;
using ESCOLA_API.Services;
using ESCOLA_API.ViewModels;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace ESCOLA_API.Tests.Services
{
    public class CalendarioEscolarServiceTests
    {
        [Fact]
        public async Task GetCalendarioAnualAsync_WhenAnoIs2026_ReturnsNationalHolidaysAndSelectedMonth()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            await using var context = CreateContext(connection);
            await context.Database.EnsureCreatedAsync();

            var service = new CalendarioEscolarService(context);
            var calendario = await service.GetCalendarioAnualAsync(2026, 5);

            Assert.Equal(2026, calendario.Ano);
            Assert.Equal(5, calendario.MesSelecionado);
            Assert.Equal(12, calendario.Meses.Length);
            Assert.Equal(10, calendario.FeriadosNacionais.Length);
            Assert.Contains(calendario.FeriadosNacionais, feriado =>
                feriado.Data == new DateOnly(2026, 4, 3)
                && feriado.Nome == "Paixao de Cristo");
            Assert.Contains(calendario.FeriadosNacionais, feriado =>
                feriado.Data == new DateOnly(2026, 11, 20)
                && feriado.Nome.Contains("Consciencia Negra"));

            var novembro20 = calendario.Meses[10].Dias.Single(dia => dia.Data == new DateOnly(2026, 11, 20));
            Assert.True(novembro20.FeriadoNacional);
            Assert.Equal("Dia Nacional de Zumbi e da Consciencia Negra", novembro20.NomeFeriado);
        }

        [Fact]
        public async Task GetCalendarioAnualAsync_WhenMesIsInvalid_ThrowsInvalidOperationException()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            await using var context = CreateContext(connection);
            await context.Database.EnsureCreatedAsync();

            var service = new CalendarioEscolarService(context);
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.GetCalendarioAnualAsync(2026, 13));

            Assert.Equal("Informe um mes entre 1 e 12.", exception.Message);
        }

        [Fact]
        public async Task AddEventoAsync_WhenReuniaoProfessores_NotifiesOnlyProfessors()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            await using var context = CreateContext(connection);
            await context.Database.EnsureCreatedAsync();

            var professoresEsperados = await context.Usuarios
                .CountAsync(usuario => usuario.IdPerfil == PerfilSistema.ProfessorId);
            var service = new CalendarioEscolarService(context);

            var evento = await service.AddEventoAsync(new CalendarioEscolarEventoCreateViewModel
            {
                Data = new DateOnly(2026, 6, 10),
                Tipo = "ReuniaoProfessores",
                Titulo = "Reuniao pedagogica",
                Descricao = "Alinhamento do bimestre"
            }, CreatePrincipal(1, PerfilSistema.Administrador));

            Assert.True(evento.IdEventoCalendarioEscolar > 0);
            Assert.Equal("Professores", evento.PublicoAlvo);
            Assert.Equal(professoresEsperados, evento.TotalNotificados);

            var notificacoes = await context.Notificacoes
                .Include(notificacao => notificacao.Usuario)
                .Where(notificacao => notificacao.Tipo == "CalendarioEscolarEvento")
                .ToArrayAsync();

            Assert.Equal(professoresEsperados, notificacoes.Length);
            Assert.All(notificacoes, notificacao =>
                Assert.Equal(PerfilSistema.ProfessorId, notificacao.Usuario!.IdPerfil));
        }

        [Fact]
        public async Task AddEventoAsync_WhenReuniaoPaisMestres_NotifiesStudentsAndProfessors()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            await using var context = CreateContext(connection);
            await context.Database.EnsureCreatedAsync();

            var destinatariosEsperados = await context.Usuarios
                .CountAsync(usuario =>
                    usuario.IdPerfil == PerfilSistema.AlunoId
                    || usuario.IdPerfil == PerfilSistema.ProfessorId);
            var service = new CalendarioEscolarService(context);

            var evento = await service.AddEventoAsync(new CalendarioEscolarEventoCreateViewModel
            {
                Data = new DateOnly(2026, 8, 12),
                Tipo = "ReuniaoPaisMestres",
                Titulo = "Reuniao de pais e mestres"
            }, CreatePrincipal(1, PerfilSistema.Administrador));

            Assert.Equal("AlunosEProfessores", evento.PublicoAlvo);
            Assert.Equal(destinatariosEsperados, evento.TotalNotificados);
            Assert.Contains(PerfilSistema.Aluno, evento.PerfisDestinatarios);
            Assert.Contains(PerfilSistema.Professor, evento.PerfisDestinatarios);
        }

        [Fact]
        public async Task AddEventoAsync_WhenProfessorTries_ThrowsUnauthorizedAccessException()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();
            await using var context = CreateContext(connection);
            await context.Database.EnsureCreatedAsync();

            var service = new CalendarioEscolarService(context);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.AddEventoAsync(new CalendarioEscolarEventoCreateViewModel
                {
                    Data = new DateOnly(2026, 6, 10),
                    Titulo = "Evento"
                }, CreatePrincipal(2, PerfilSistema.Professor)));
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
