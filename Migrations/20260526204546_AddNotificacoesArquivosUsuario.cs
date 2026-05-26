using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ESCOLA_API.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificacoesArquivosUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FotoPerfilUrl",
                table: "Usuario",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Notificacao",
                columns: table => new
                {
                    IdNotificacao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Mensagem = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Link = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IdCadernetaDigital = table.Column<int>(type: "int", nullable: true),
                    IdDisciplina = table.Column<int>(type: "int", nullable: true),
                    NomeDisciplina = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MediaAritmetica = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: true),
                    Situacao = table.Column<string>(type: "nvarchar(80)", maxLength: 80, nullable: true),
                    CorSituacao = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    OrigemMensagemId = table.Column<string>(type: "nvarchar(160)", maxLength: 160, nullable: true),
                    Lida = table.Column<bool>(type: "bit", nullable: false),
                    CriadaEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LidaEmUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificacao", x => x.IdNotificacao);
                    table.ForeignKey(
                        name: "FK_Notificacao_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioArquivo",
                columns: table => new
                {
                    IdUsuarioArquivo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    TipoArquivo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    NomeOriginal = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CaminhoRelativo = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    TamanhoBytes = table.Column<long>(type: "bigint", nullable: false),
                    CriadoEmUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioArquivo", x => x.IdUsuarioArquivo);
                    table.ForeignKey(
                        name: "FK_UsuarioArquivo_Usuario_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuario",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notificacao_IdUsuario",
                table: "Notificacao",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacao_OrigemMensagemId",
                table: "Notificacao",
                column: "OrigemMensagemId",
                unique: true,
                filter: "[OrigemMensagemId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioArquivo_IdUsuario",
                table: "UsuarioArquivo",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notificacao");

            migrationBuilder.DropTable(
                name: "UsuarioArquivo");

            migrationBuilder.DropColumn(
                name: "FotoPerfilUrl",
                table: "Usuario");
        }
    }
}
