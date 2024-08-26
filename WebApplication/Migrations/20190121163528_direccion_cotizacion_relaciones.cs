using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class direccion_cotizacion_relaciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_direcciones_cotizacion_id_cotizacion",
                table: "direcciones_cotizacion",
                column: "id_cotizacion");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Direcciones_Cotizaciones",
                table: "direcciones_cotizacion",
                column: "id_cotizacion",
                principalTable: "Cotizaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Direcciones_Cotizaciones",
                table: "direcciones_cotizacion");

            migrationBuilder.DropIndex(
                name: "IX_direcciones_cotizacion_id_cotizacion",
                table: "direcciones_cotizacion");
        }
    }
}
