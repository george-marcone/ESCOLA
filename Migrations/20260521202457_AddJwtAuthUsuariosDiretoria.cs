using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace form_API.Migrations
{
    /// <inheritdoc />
    public partial class AddJwtAuthUsuariosDiretoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdUsuario",
                table: "Professores",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdUsuario",
                table: "Alunos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Perfil",
                columns: table => new
                {
                    IdPerfil = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DescricaoPerfil = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfil", x => x.IdPerfil);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Senha = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IdPerfil = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_Usuario_Perfil_IdPerfil",
                        column: x => x.IdPerfil,
                        principalTable: "Perfil",
                        principalColumn: "IdPerfil",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Diretoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diretoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Diretoria_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 1,
                column: "IdUsuario",
                value: 12);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 2,
                column: "IdUsuario",
                value: 13);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 3,
                column: "IdUsuario",
                value: 14);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 4,
                column: "IdUsuario",
                value: 15);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 5,
                column: "IdUsuario",
                value: 16);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 6,
                column: "IdUsuario",
                value: 17);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 7,
                column: "IdUsuario",
                value: 18);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 8,
                column: "IdUsuario",
                value: 19);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 9,
                column: "IdUsuario",
                value: 20);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 10,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 11,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 12,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 13,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 14,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 15,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 16,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 17,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 18,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 19,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 20,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 21,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 22,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 23,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 24,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 25,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 26,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 27,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 28,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 29,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 30,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 31,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 32,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 33,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 34,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 35,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 36,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 37,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 38,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 39,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 40,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 41,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 42,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 43,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 44,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 45,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 46,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 47,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 48,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 49,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Alunos",
                keyColumn: "Id",
                keyValue: 50,
                column: "IdUsuario",
                value: null);

            migrationBuilder.InsertData(
                table: "Perfil",
                columns: new[] { "IdPerfil", "DescricaoPerfil" },
                values: new object[,]
                {
                    { 1, "Administrador" },
                    { 2, "Contribuinte" },
                    { 3, "Leitor" }
                });

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 1,
                column: "IdUsuario",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 2,
                column: "IdUsuario",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 3,
                column: "IdUsuario",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 4,
                column: "IdUsuario",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 5,
                column: "IdUsuario",
                value: 6);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 6,
                column: "IdUsuario",
                value: 7);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 7,
                column: "IdUsuario",
                value: 8);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 8,
                column: "IdUsuario",
                value: 9);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 9,
                column: "IdUsuario",
                value: 10);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 10,
                column: "IdUsuario",
                value: 11);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 11,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 12,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 13,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 14,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 15,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 16,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 17,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 18,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 19,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 20,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 21,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 22,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 23,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 24,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 25,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 26,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 27,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 28,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 29,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 30,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 31,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 32,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 33,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 34,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 35,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 36,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 37,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 38,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 39,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 40,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 41,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 42,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 43,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 44,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 45,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 46,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 47,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 48,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 49,
                column: "IdUsuario",
                value: null);

            migrationBuilder.UpdateData(
                table: "Professores",
                keyColumn: "Id",
                keyValue: 50,
                column: "IdUsuario",
                value: null);

            migrationBuilder.InsertData(
                table: "Usuario",
                columns: new[] { "IdUsuario", "Email", "IdPerfil", "Nome", "Senha", "Telefone" },
                values: new object[,]
                {
                    { 1, "admin@escola.com", 1, "Administrador Sistema", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTAx$FfQ5HKxd+P+bqyIRbLEJT21eK0xwqw0fRymSJ50btiM=", "11999990001" },
                    { 2, "professor01@escola.com", 2, "Professor Vinicius", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTAy$LzErqoJ36wdajqImPtWmjfIDcSHdw/Qsd0yPTJDXANA=", "11988880001" },
                    { 3, "professor02@escola.com", 2, "Professor Paula", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTAz$2ZjPXI5OMTXvvZcLsCWjvzrPiZjbJM4UUispcmKV2pE=", "11988880002" },
                    { 4, "professor03@escola.com", 2, "Professor Suzana", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTA0$me64hINFuBY2VgrYnV00Rt9o4O3DcKOF6ERXELwK4oo=", "11988880003" },
                    { 5, "professor04@escola.com", 2, "Professor Carlos", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTA1$BeQmoNfbZekUVMDeCs0DshesPlX/lWejxwjbOomV+SQ=", "11988880004" },
                    { 6, "professor05@escola.com", 2, "Professor Mariana", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTA2$VLz/eGnLJgRIP2hrbQiYK6iAL4AJHl+PUEe15qKAyu0=", "11988880005" },
                    { 7, "professor06@escola.com", 2, "Professor Roberto", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTA3$r52DPiT2e9YLVzIVcB6GkIH8LLA2WYqjl+JDK3VV96w=", "11988880006" },
                    { 8, "professor07@escola.com", 2, "Professor Fernanda", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTA4$xNGwJEi2R8Zc+ERPBwp4xsrb3UgrcTWrXu5Ro68EzRw=", "11988880007" },
                    { 9, "professor08@escola.com", 2, "Professor Ricardo", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTA5$WqNwjDfux46SoJFVp8URb25WqhvfnnqSSezf4mTHmO0=", "11988880008" },
                    { 10, "professor09@escola.com", 2, "Professor Patricia", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTEw$8pck4mOGxTBDGJ7TuNhF4fIvWVP1BukeZeSMOI9EovA=", "11988880009" },
                    { 11, "professor10@escola.com", 2, "Professor Marcelo", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTEx$pGtYaxggmZZrorWXNTVbhtpFw9qWgDXDj8LZlaKTRyw=", "11988880010" },
                    { 12, "aluno01@escola.com", 3, "Aluno Maria", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTEy$PSoiqNNWzRiAvRVzute150mhDSL4rTOTisKBi9TTrJM=", "11977770001" },
                    { 13, "aluno02@escola.com", 3, "Aluno Joao", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTEz$sxGpNfM7WC8nMjkLmcQ9WN2J6Lhe7O07oyFXwLO4/iU=", "11977770002" },
                    { 14, "aluno03@escola.com", 3, "Aluno Alex", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTE0$0mh8FyroAgQNjTYZXcypWMYARFKyzXJ48MNVyB78E+U=", "11977770003" },
                    { 15, "aluno04@escola.com", 3, "Aluno Ana", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTE1$SpupVdnneUNyvnPwlmVBvm0OUOr/Yjov1o5xA20Q1+w=", "11977770004" },
                    { 16, "aluno05@escola.com", 3, "Aluno Bruno", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTE2$oqDVRdKBe7SS3kMj3hAd6kgoC9dH9f8fyOHlTOZrl9k=", "11977770005" },
                    { 17, "aluno06@escola.com", 3, "Aluno Carla", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTE3$ss/qC9hAEFzI4JwGoPvKuAuI4DN8bXtKp1Ulbt/WF1o=", "11977770006" },
                    { 18, "aluno07@escola.com", 3, "Aluno Daniel", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTE4$Ks5ex2/VDVyP7B3EbbqTJqr/34e1PGXqDCTx30kSYdk=", "11977770007" },
                    { 19, "aluno08@escola.com", 3, "Aluno Elisa", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTE5$p4DpMKEz6wYRtFN4Qimq7IhXnIl+So6O8D7Zl82D59M=", "11977770008" },
                    { 20, "aluno09@escola.com", 3, "Aluno Fabio", "PBKDF2-SHA256$100000$dXN1YXJpby1zZWVkLTIw$XIIyE2s98F4W58nJ2sbRe5rriyLwskk2/VxBAgNNSZc=", "11977770009" }
                });

            migrationBuilder.InsertData(
                table: "Diretoria",
                columns: new[] { "Id", "IdUsuario", "Nome" },
                values: new object[] { 1, 1, "Administrador Sistema" });

            migrationBuilder.CreateIndex(
                name: "IX_Professores_IdUsuario",
                table: "Professores",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Alunos_IdUsuario",
                table: "Alunos",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Diretoria_IdUsuario",
                table: "Diretoria",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Email",
                table: "Usuario",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_IdPerfil",
                table: "Usuario",
                column: "IdPerfil");

            migrationBuilder.AddForeignKey(
                name: "FK_Alunos_Usuario_IdUsuario",
                table: "Alunos",
                column: "IdUsuario",
                principalTable: "Usuario",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Professores_Usuario_IdUsuario",
                table: "Professores",
                column: "IdUsuario",
                principalTable: "Usuario",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Alunos_Usuario_IdUsuario",
                table: "Alunos");

            migrationBuilder.DropForeignKey(
                name: "FK_Professores_Usuario_IdUsuario",
                table: "Professores");

            migrationBuilder.DropTable(
                name: "Diretoria");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Perfil");

            migrationBuilder.DropIndex(
                name: "IX_Professores_IdUsuario",
                table: "Professores");

            migrationBuilder.DropIndex(
                name: "IX_Alunos_IdUsuario",
                table: "Alunos");

            migrationBuilder.DropColumn(
                name: "IdUsuario",
                table: "Professores");

            migrationBuilder.DropColumn(
                name: "IdUsuario",
                table: "Alunos");
        }
    }
}
