using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "detalle",
                table: "mensajes");

            migrationBuilder.AddColumn<string>(
                name: "detalle_msj",
                table: "mensajes",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "detalle_msj",
                table: "mensajes");

            migrationBuilder.AddColumn<int>(
                name: "detalle",
                table: "mensajes",
                nullable: false,
                defaultValue: 0);
        }
    }
}
