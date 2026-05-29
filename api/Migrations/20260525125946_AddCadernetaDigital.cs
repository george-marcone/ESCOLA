using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESCOLA_API.Migrations
{
    /// <inheritdoc />
    public partial class AddCadernetaDigital : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Disciplina",
                columns: table => new
                {
                    IdDisciplina = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdProfessorUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Disciplina", x => x.IdDisciplina);
                    table.ForeignKey(
                        name: "FK_Disciplina_Usuario_IdProfessorUsuario",
                        column: x => x.IdProfessorUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CadernetaDigital",
                columns: table => new
                {
                    IdCadernetaDigital = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAlunoUsuario = table.Column<int>(type: "int", nullable: false),
                    IdDisciplina = table.Column<int>(type: "int", nullable: false),
                    Notas = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Presencas = table.Column<int>(type: "int", nullable: false),
                    Faltas = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CadernetaDigital", x => x.IdCadernetaDigital);
                    table.ForeignKey(
                        name: "FK_CadernetaDigital_Disciplina_IdDisciplina",
                        column: x => x.IdDisciplina,
                        principalTable: "Disciplina",
                        principalColumn: "IdDisciplina",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CadernetaDigital_Usuario_IdAlunoUsuario",
                        column: x => x.IdAlunoUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Perfil",
                keyColumn: "IdPerfil",
                keyValue: 2,
                column: "DescricaoPerfil",
                value: "Professor");

            migrationBuilder.UpdateData(
                table: "Perfil",
                keyColumn: "IdPerfil",
                keyValue: 3,
                column: "DescricaoPerfil",
                value: "Aluno");

            migrationBuilder.CreateIndex(
                name: "IX_CadernetaDigital_IdAlunoUsuario_IdDisciplina",
                table: "CadernetaDigital",
                columns: new[] { "IdAlunoUsuario", "IdDisciplina" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CadernetaDigital_IdDisciplina",
                table: "CadernetaDigital",
                column: "IdDisciplina");

            migrationBuilder.CreateIndex(
                name: "IX_Disciplina_IdProfessorUsuario_Nome",
                table: "Disciplina",
                columns: new[] { "IdProfessorUsuario", "Nome" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CadernetaDigital");

            migrationBuilder.DropTable(
                name: "Disciplina");

            migrationBuilder.UpdateData(
                table: "Perfil",
                keyColumn: "IdPerfil",
                keyValue: 2,
                column: "DescricaoPerfil",
                value: "Contribuinte");

            migrationBuilder.UpdateData(
                table: "Perfil",
                keyColumn: "IdPerfil",
                keyValue: 3,
                column: "DescricaoPerfil",
                value: "Leitor");
        }
    }
}
