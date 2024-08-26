using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class rel_comisiones1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comisiones_promo_sublinea_config_condiones_comerciales_sucursal_Condiones_Comerciales_Sucursalid",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.DropForeignKey(
                name: "FK_comisiones_promo_sublinea_config_Cat_SubLinea_Producto_subLinea_Productoid",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.DropIndex(
                name: "IX_comisiones_promo_sublinea_config_Condiones_Comerciales_Sucursalid",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.DropIndex(
                name: "IX_comisiones_promo_sublinea_config_subLinea_Productoid",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.DropColumn(
                name: "Condiones_Comerciales_Sucursalid",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.DropColumn(
                name: "subLinea_Productoid",
                table: "comisiones_promo_sublinea_config");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Condiones_Comerciales_Sucursalid",
                table: "comisiones_promo_sublinea_config",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "subLinea_Productoid",
                table: "comisiones_promo_sublinea_config",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_promo_sublinea_config_Condiones_Comerciales_Sucursalid",
                table: "comisiones_promo_sublinea_config",
                column: "Condiones_Comerciales_Sucursalid");

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_promo_sublinea_config_subLinea_Productoid",
                table: "comisiones_promo_sublinea_config",
                column: "subLinea_Productoid");

            migrationBuilder.AddForeignKey(
                name: "FK_comisiones_promo_sublinea_config_condiones_comerciales_sucursal_Condiones_Comerciales_Sucursalid",
                table: "comisiones_promo_sublinea_config",
                column: "Condiones_Comerciales_Sucursalid",
                principalTable: "condiones_comerciales_sucursal",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_comisiones_promo_sublinea_config_Cat_SubLinea_Producto_subLinea_Productoid",
                table: "comisiones_promo_sublinea_config",
                column: "subLinea_Productoid",
                principalTable: "Cat_SubLinea_Producto",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
