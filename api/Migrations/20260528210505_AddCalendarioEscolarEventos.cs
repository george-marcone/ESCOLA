using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESCOLA_API.Migrations
{
    /// <inheritdoc />
    public partial class AddCalendarioEscolarEventos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CalendarioEscolarEvento",
                columns: table => new
                {
                    IdEventoCalendarioEscolar = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Data = table.Column<DateOnly>(type: "date", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PublicoAlvo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IdUsuarioCriador = table.Column<int>(type: "int", nullable: false),
                    NomeUsuarioCriador = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CriadoEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalendarioEscolarEvento", x => x.IdEventoCalendarioEscolar);
                    table.ForeignKey(
                        name: "FK_CalendarioEscolarEvento_Usuario_IdUsuarioCriador",
                        column: x => x.IdUsuarioCriador,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioEscolarEvento_Data",
                table: "CalendarioEscolarEvento",
                column: "Data");

            migrationBuilder.CreateIndex(
                name: "IX_CalendarioEscolarEvento_IdUsuarioCriador",
                table: "CalendarioEscolarEvento",
                column: "IdUsuarioCriador");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CalendarioEscolarEvento");
        }
    }
}
