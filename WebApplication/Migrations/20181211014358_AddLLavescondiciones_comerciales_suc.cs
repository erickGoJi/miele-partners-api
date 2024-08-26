using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class AddLLavescondiciones_comerciales_suc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_condiones_comerciales_sucursal_Cat_SubLinea_Producto_Cat_SubLinea_Productoid",
                table: "condiones_comerciales_sucursal");

            migrationBuilder.DropForeignKey(
                name: "FK_condiones_comerciales_sucursal_Cat_Sucursales_Cat_SucursalesId",
                table: "condiones_comerciales_sucursal");

            migrationBuilder.DropIndex(
                name: "IX_condiones_comerciales_sucursal_Cat_SubLinea_Productoid",
                table: "condiones_comerciales_sucursal");

            migrationBuilder.DropIndex(
                name: "IX_condiones_comerciales_sucursal_Cat_SucursalesId",
                table: "condiones_comerciales_sucursal");

            migrationBuilder.DropColumn(
                name: "Cat_SubLinea_Productoid",
                table: "condiones_comerciales_sucursal");

            migrationBuilder.DropColumn(
                name: "Cat_SucursalesId",
                table: "condiones_comerciales_sucursal");

            migrationBuilder.CreateIndex(
                name: "IX_condiones_comerciales_sucursal_id_Cat_SubLinea_Producto",
                table: "condiones_comerciales_sucursal",
                column: "id_Cat_SubLinea_Producto");

            migrationBuilder.CreateIndex(
                name: "IX_condiones_comerciales_sucursal_id_Cat_Sucursales",
                table: "condiones_comerciales_sucursal",
                column: "id_Cat_Sucursales");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_condiones_comerciales_sucursal_sublinea",
                table: "condiones_comerciales_sucursal",
                column: "id_Cat_SubLinea_Producto",
                principalTable: "Cat_SubLinea_Producto",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_condiones_comerciales_sucursal_sucursales",
                table: "condiones_comerciales_sucursal",
                column: "id_Cat_Sucursales",
                principalTable: "Cat_Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_condiones_comerciales_sucursal_sublinea",
                table: "condiones_comerciales_sucursal");

            migrationBuilder.DropForeignKey(
                name: "ForeignKey_condiones_comerciales_sucursal_sucursales",
                table: "condiones_comerciales_sucursal");

            migrationBuilder.DropIndex(
                name: "IX_condiones_comerciales_sucursal_id_Cat_SubLinea_Producto",
                table: "condiones_comerciales_sucursal");

            migrationBuilder.DropIndex(
                name: "IX_condiones_comerciales_sucursal_id_Cat_Sucursales",
                table: "condiones_comerciales_sucursal");

            migrationBuilder.AddColumn<int>(
                name: "Cat_SubLinea_Productoid",
                table: "condiones_comerciales_sucursal",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cat_SucursalesId",
                table: "condiones_comerciales_sucursal",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_condiones_comerciales_sucursal_Cat_SubLinea_Productoid",
                table: "condiones_comerciales_sucursal",
                column: "Cat_SubLinea_Productoid");

            migrationBuilder.CreateIndex(
                name: "IX_condiones_comerciales_sucursal_Cat_SucursalesId",
                table: "condiones_comerciales_sucursal",
                column: "Cat_SucursalesId");

            migrationBuilder.AddForeignKey(
                name: "FK_condiones_comerciales_sucursal_Cat_SubLinea_Producto_Cat_SubLinea_Productoid",
                table: "condiones_comerciales_sucursal",
                column: "Cat_SubLinea_Productoid",
                principalTable: "Cat_SubLinea_Producto",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_condiones_comerciales_sucursal_Cat_Sucursales_Cat_SucursalesId",
                table: "condiones_comerciales_sucursal",
                column: "Cat_SucursalesId",
                principalTable: "Cat_Sucursales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
