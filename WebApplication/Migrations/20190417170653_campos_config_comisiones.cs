using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class campos_config_comisiones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "condiones_comerciales_sucursalid",
                table: "comisiones_promo_sublinea_config",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_promocion",
                table: "comisiones_promo_sublinea_config",
                nullable: false,
                defaultValue: 0);

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

        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "id_promocion",
                table: "comisiones_promo_sublinea_config");
        }
    }
}
