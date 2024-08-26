using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class AddLLaveSublineas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cat_SubLinea_Producto_Cat_Linea_Producto_id_lineaid",
                table: "Cat_SubLinea_Producto");

            migrationBuilder.DropIndex(
                name: "IX_Cat_SubLinea_Producto_id_lineaid",
                table: "Cat_SubLinea_Producto");

            migrationBuilder.DropColumn(
                name: "id_lineaid",
                table: "Cat_SubLinea_Producto");

            migrationBuilder.AlterColumn<long>(
                name: "id_cuenta",
                table: "Cat_CondicionesPago",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_Cat_SubLinea_Producto_id_linea_producto",
                table: "Cat_SubLinea_Producto",
                column: "id_linea_producto");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_CondicionesPago_id_Cat_Formas_Pago",
                table: "Cat_CondicionesPago",
                column: "id_Cat_Formas_Pago");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_CondicionesPago_id_cuenta",
                table: "Cat_CondicionesPago",
                column: "id_cuenta");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Cat_Formas_Pago_Cat_CondicionesPago",
                table: "Cat_CondicionesPago",
                column: "id_Cat_Formas_Pago",
                principalTable: "Cat_Formas_Pago",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_cuentas_Cat_CondicionesPago",
                table: "Cat_CondicionesPago",
                column: "id_cuenta",
                principalTable: "Cat_Cuentas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Cat_sublinea_cat_linea",
                table: "Cat_SubLinea_Producto",
                column: "id_linea_producto",
                principalTable: "Cat_Linea_Producto",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Cat_Formas_Pago_Cat_CondicionesPago",
                table: "Cat_CondicionesPago");

            migrationBuilder.DropForeignKey(
                name: "ForeignKey_cuentas_Cat_CondicionesPago",
                table: "Cat_CondicionesPago");

            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Cat_sublinea_cat_linea",
                table: "Cat_SubLinea_Producto");

            migrationBuilder.DropIndex(
                name: "IX_Cat_SubLinea_Producto_id_linea_producto",
                table: "Cat_SubLinea_Producto");

            migrationBuilder.DropIndex(
                name: "IX_Cat_CondicionesPago_id_Cat_Formas_Pago",
                table: "Cat_CondicionesPago");

            migrationBuilder.DropIndex(
                name: "IX_Cat_CondicionesPago_id_cuenta",
                table: "Cat_CondicionesPago");

            migrationBuilder.AddColumn<int>(
                name: "id_lineaid",
                table: "Cat_SubLinea_Producto",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id_cuenta",
                table: "Cat_CondicionesPago",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.CreateIndex(
                name: "IX_Cat_SubLinea_Producto_id_lineaid",
                table: "Cat_SubLinea_Producto",
                column: "id_lineaid");

            migrationBuilder.AddForeignKey(
                name: "FK_Cat_SubLinea_Producto_Cat_Linea_Producto_id_lineaid",
                table: "Cat_SubLinea_Producto",
                column: "id_lineaid",
                principalTable: "Cat_Linea_Producto",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
