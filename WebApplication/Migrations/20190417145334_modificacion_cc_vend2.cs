using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class modificacion_cc_vend2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_promocion_comisiones_configuracion",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.DropIndex(
                name: "IX_comisiones_promo_sublinea_config_id_promocion",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.RenameColumn(
                name: "id_promocion",
                table: "comisiones_promo_sublinea_config",
                newName: "id_cc_sucursal");

            migrationBuilder.AddColumn<int>(
                name: "Condiones_Comerciales_Sucursalid",
                table: "comisiones_promo_sublinea_config",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_promo_sublinea_config_Condiones_Comerciales_Sucursalid",
                table: "comisiones_promo_sublinea_config",
                column: "Condiones_Comerciales_Sucursalid");

            migrationBuilder.AddForeignKey(
                name: "FK_comisiones_promo_sublinea_config_condiones_comerciales_sucursal_Condiones_Comerciales_Sucursalid",
                table: "comisiones_promo_sublinea_config",
                column: "Condiones_Comerciales_Sucursalid",
                principalTable: "condiones_comerciales_sucursal",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comisiones_promo_sublinea_config_condiones_comerciales_sucursal_Condiones_Comerciales_Sucursalid",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.DropIndex(
                name: "IX_comisiones_promo_sublinea_config_Condiones_Comerciales_Sucursalid",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.DropColumn(
                name: "Condiones_Comerciales_Sucursalid",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.RenameColumn(
                name: "id_cc_sucursal",
                table: "comisiones_promo_sublinea_config",
                newName: "id_promocion");

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_promo_sublinea_config_id_promocion",
                table: "comisiones_promo_sublinea_config",
                column: "id_promocion");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_promocion_comisiones_configuracion",
                table: "comisiones_promo_sublinea_config",
                column: "id_promocion",
                principalTable: "promocion",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
