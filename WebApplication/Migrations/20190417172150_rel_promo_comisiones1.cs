using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class rel_promo_comisiones1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_comisiones_promo_sublinea_config_condiones_comerciales_sucursal_condiones_comerciales_sucursalid",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.DropIndex(
                name: "IX_comisiones_promo_sublinea_config_condiones_comerciales_sucursalid",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.DropColumn(
                name: "condiones_comerciales_sucursalid",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_promo_sublinea_config_id_promocion",
                table: "comisiones_promo_sublinea_config",
                column: "id_promocion");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Promocion_comisiones_sublinea_config",
                table: "comisiones_promo_sublinea_config",
                column: "id_promocion",
                principalTable: "promocion",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Promocion_comisiones_sublinea_config",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.DropIndex(
                name: "IX_comisiones_promo_sublinea_config_id_promocion",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.AddColumn<int>(
                name: "condiones_comerciales_sucursalid",
                table: "comisiones_promo_sublinea_config",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_promo_sublinea_config_condiones_comerciales_sucursalid",
                table: "comisiones_promo_sublinea_config",
                column: "condiones_comerciales_sucursalid");

            migrationBuilder.AddForeignKey(
                name: "FK_comisiones_promo_sublinea_config_condiones_comerciales_sucursal_condiones_comerciales_sucursalid",
                table: "comisiones_promo_sublinea_config",
                column: "condiones_comerciales_sucursalid",
                principalTable: "condiones_comerciales_sucursal",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
