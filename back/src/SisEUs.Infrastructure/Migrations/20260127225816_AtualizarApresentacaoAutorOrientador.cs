using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SisEUs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AtualizarApresentacaoAutorOrientador : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Sessao_EventoId",
                table: "Eventos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Eventos",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "NomeAutor",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "NomeOrientador",
                table: "Eventos");

            migrationBuilder.RenameTable(
                name: "Eventos",
                newName: "Apresentacoes");

            migrationBuilder.AddColumn<int>(
                name: "AutorId",
                table: "Apresentacoes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "OrientadorId",
                table: "Apresentacoes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Apresentacoes",
                table: "Apresentacoes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Apresentacoes_Sessao_EventoId",
                table: "Apresentacoes",
                column: "EventoId",
                principalTable: "Sessao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Avaliacoes_Apresentacoes_ApresentacaoId",
                table: "Avaliacoes",
                column: "ApresentacaoId",
                principalTable: "Apresentacoes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Apresentacoes_Sessao_EventoId",
                table: "Apresentacoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Avaliacoes_Apresentacoes_ApresentacaoId",
                table: "Avaliacoes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Apresentacoes",
                table: "Apresentacoes");

            migrationBuilder.DropColumn(
                name: "AutorId",
                table: "Apresentacoes");

            migrationBuilder.DropColumn(
                name: "OrientadorId",
                table: "Apresentacoes");

            migrationBuilder.RenameTable(
                name: "Apresentacoes",
                newName: "Eventos");

            migrationBuilder.AddColumn<string>(
                name: "NomeAutor",
                table: "Eventos",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "NomeOrientador",
                table: "Eventos",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Eventos",
                table: "Eventos",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Sessao_EventoId",
                table: "Eventos",
                column: "EventoId",
                principalTable: "Sessao",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
