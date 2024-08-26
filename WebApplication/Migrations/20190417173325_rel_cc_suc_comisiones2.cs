using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class rel_cc_suc_comisiones2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_comisiones_promo_sublinea_config_id_cc_sucursal",
                table: "comisiones_promo_sublinea_config",
                column: "id_cc_sucursal");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_comisiones_sublinea_config_cc_suc",
                table: "comisiones_promo_sublinea_config",
                column: "id_cc_sucursal",
                principalTable: "condiones_comerciales_sucursal",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_comisiones_sublinea_config_cc_suc",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.DropIndex(
                name: "IX_comisiones_promo_sublinea_config_id_cc_sucursal",
                table: "comisiones_promo_sublinea_config");
        }
    }
}
