using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class margenes_comerciales_detalle_margenes_comerciales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Users_comisiones_sucursales",
                table: "comisiones_sucursales");

            migrationBuilder.DropIndex(
                name: "IX_comisiones_sucursales_id_quienpago",
                table: "comisiones_sucursales");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_comisiones_sucursales_id_quienpago",
                table: "comisiones_sucursales",
                column: "id_quienpago");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Users_comisiones_sucursales",
                table: "comisiones_sucursales",
                column: "id_quienpago",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
