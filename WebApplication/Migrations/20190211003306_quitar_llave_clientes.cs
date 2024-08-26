using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class quitar_llave_clientes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Cotizacion_Cliente",
                table: "Cotizaciones");

            migrationBuilder.DropIndex(
                name: "IX_Cotizaciones_Id_Cliente",
                table: "Cotizaciones");

            migrationBuilder.AddColumn<long>(
                name: "Clientesid",
                table: "Cotizaciones",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_Clientesid",
                table: "Cotizaciones",
                column: "Clientesid");

            migrationBuilder.AddForeignKey(
                name: "FK_Cotizaciones_Clientes_Clientesid",
                table: "Cotizaciones",
                column: "Clientesid",
                principalTable: "Clientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cotizaciones_Clientes_Clientesid",
                table: "Cotizaciones");

            migrationBuilder.DropIndex(
                name: "IX_Cotizaciones_Clientesid",
                table: "Cotizaciones");

            migrationBuilder.DropColumn(
                name: "Clientesid",
                table: "Cotizaciones");

            migrationBuilder.CreateIndex(
                name: "IX_Cotizaciones_Id_Cliente",
                table: "Cotizaciones",
                column: "Id_Cliente");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Cotizacion_Cliente",
                table: "Cotizaciones",
                column: "Id_Cliente",
                principalTable: "Clientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
