using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class url_pdf_visitas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "url_pdf_checklist",
                table: "Visita",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "url_pdf_cotizacion",
                table: "Visita",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "url_pdf_checklist",
                table: "Visita");

            migrationBuilder.DropColumn(
                name: "url_pdf_cotizacion",
                table: "Visita");
        }
    }
}
