using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class sublineas_cer_rel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Producto_Mantenimiento",
                table: "rel_certificado_producto");

            migrationBuilder.DropIndex(
                name: "IX_rel_certificado_producto_id_producto",
                table: "rel_certificado_producto");

            migrationBuilder.DropColumn(
                name: "id_sub_linea",
                table: "Cer_producto_cliente");

            migrationBuilder.AddColumn<int>(
                name: "Cat_Productosid",
                table: "rel_certificado_producto",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_sub_linea",
                table: "rel_certificado_producto",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_rel_certificado_producto_Cat_Productosid",
                table: "rel_certificado_producto",
                column: "Cat_Productosid");

            migrationBuilder.AddForeignKey(
                name: "FK_rel_certificado_producto_Cat_Productos_Cat_Productosid",
                table: "rel_certificado_producto",
                column: "Cat_Productosid",
                principalTable: "Cat_Productos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rel_certificado_producto_Cat_Productos_Cat_Productosid",
                table: "rel_certificado_producto");

            migrationBuilder.DropIndex(
                name: "IX_rel_certificado_producto_Cat_Productosid",
                table: "rel_certificado_producto");

            migrationBuilder.DropColumn(
                name: "Cat_Productosid",
                table: "rel_certificado_producto");

            migrationBuilder.DropColumn(
                name: "id_sub_linea",
                table: "rel_certificado_producto");

            migrationBuilder.AddColumn<int>(
                name: "id_sub_linea",
                table: "Cer_producto_cliente",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_rel_certificado_producto_id_producto",
                table: "rel_certificado_producto",
                column: "id_producto");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Producto_Mantenimiento",
                table: "rel_certificado_producto",
                column: "id_producto",
                principalTable: "Cat_Productos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
