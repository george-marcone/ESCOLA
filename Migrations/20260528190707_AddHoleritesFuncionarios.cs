using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESCOLA_API.Migrations
{
    /// <inheritdoc />
    public partial class AddHoleritesFuncionarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Holerite",
                columns: table => new
                {
                    IdHolerite = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    CompetenciaMes = table.Column<int>(type: "int", nullable: false),
                    CompetenciaAno = table.Column<int>(type: "int", nullable: false),
                    NomeOriginal = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NomeBlob = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    CriadoEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holerite", x => x.IdHolerite);
                    table.ForeignKey(
                        name: "FK_Holerite_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Holerite_IdUsuario_CompetenciaAno_CompetenciaMes",
                table: "Holerite",
                columns: new[] { "IdUsuario", "CompetenciaAno", "CompetenciaMes" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Holerite");
        }
    }
}
