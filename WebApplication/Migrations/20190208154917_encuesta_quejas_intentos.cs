using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class encuesta_quejas_intentos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "intentos",
                table: "encuesta_queja",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "intentos",
                table: "encuesta_general",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "intentos",
                table: "encuesta_queja");

            migrationBuilder.DropColumn(
                name: "intentos",
                table: "encuesta_general");
        }
    }
}
