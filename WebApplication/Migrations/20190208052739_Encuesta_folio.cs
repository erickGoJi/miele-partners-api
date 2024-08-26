using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class Encuesta_folio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "folio",
                table: "encuesta_queja",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "folio",
                table: "encuesta_general",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "folio",
                table: "encuesta_queja");

            migrationBuilder.DropColumn(
                name: "folio",
                table: "encuesta_general");
        }
    }
}
