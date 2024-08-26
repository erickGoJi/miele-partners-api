using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class idProd_home_prod_idCotizacion_cer_prod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "id_producto",
                table: "home_producto_cliente",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "id_cotizacion",
                table: "Cer_producto_cliente",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "id_producto",
                table: "home_producto_cliente");

            migrationBuilder.DropColumn(
                name: "id_cotizacion",
                table: "Cer_producto_cliente");
        }
    }
}
