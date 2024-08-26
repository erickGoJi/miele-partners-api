using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class comision_sublineas_FK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comisiones_promo_sublinea_config_Cat_SubLinea_Producto_subLinea_Productoid",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.DropIndex(
                name: "IX_comisiones_promo_sublinea_config_subLinea_Productoid",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.DropColumn(
                name: "subLinea_Productoid",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_promo_sublinea_config_id_sublinea",
                table: "comisiones_promo_sublinea_config",
                column: "id_sublinea");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_sublineas_comisiones_configuracion",
                table: "comisiones_promo_sublinea_config",
                column: "id_sublinea",
                principalTable: "Cat_SubLinea_Producto",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_sublineas_comisiones_configuracion",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.DropIndex(
                name: "IX_comisiones_promo_sublinea_config_id_sublinea",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.AddColumn<int>(
                name: "subLinea_Productoid",
                table: "comisiones_promo_sublinea_config",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_promo_sublinea_config_subLinea_Productoid",
                table: "comisiones_promo_sublinea_config",
                column: "subLinea_Productoid");

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
