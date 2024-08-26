using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class Encuesta_estatus_activo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "estatus_activo",
                table: "encuesta_queja",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "estatus_activo",
                table: "encuesta_general",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estatus_activo",
                table: "encuesta_queja");

            migrationBuilder.DropColumn(
                name: "estatus_activo",
                table: "encuesta_general");
        }
    }
}
