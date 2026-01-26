using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SisEUs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CorrecaoPresencaCheckInCheckOut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "CheckOutValido",
                table: "Presencas",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AlterColumn<bool>(
                name: "CheckInValido",
                table: "Presencas",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "CheckOutValido",
                table: "Presencas",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "CheckInValido",
                table: "Presencas",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldDefaultValue: false);
        }
    }
}
