using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class info_pago : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "fec_pago",
                table: "Visita",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "id_pago",
                table: "Visita",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "succes_pago",
                table: "Visita",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fec_pago",
                table: "Visita");

            migrationBuilder.DropColumn(
                name: "id_pago",
                table: "Visita");

            migrationBuilder.DropColumn(
                name: "succes_pago",
                table: "Visita");
        }
    }
}
