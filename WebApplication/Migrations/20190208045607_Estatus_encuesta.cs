using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class Estatus_encuesta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "estatus_encuesta",
                table: "encuesta_queja",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "estatus_encuesta",
                table: "encuesta_general",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estatus_encuesta",
                table: "encuesta_queja");

            migrationBuilder.DropColumn(
                name: "estatus_encuesta",
                table: "encuesta_general");
        }
    }
}
