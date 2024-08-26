using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class certificados_update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rel_certificado_producto_Cer_producto_cliente_certificadoid",
                table: "rel_certificado_producto");

            migrationBuilder.DropIndex(
                name: "IX_rel_certificado_producto_certificadoid",
                table: "rel_certificado_producto");

            migrationBuilder.DropColumn(
                name: "certificadoid",
                table: "rel_certificado_producto");

            migrationBuilder.AlterColumn<int>(
                name: "id_certificado",
                table: "rel_certificado_producto",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.CreateIndex(
                name: "IX_rel_certificado_producto_id_certificado",
                table: "rel_certificado_producto",
                column: "id_certificado");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Producto_Cer_Mantenimiento",
                table: "rel_certificado_producto",
                column: "id_certificado",
                principalTable: "Cer_producto_cliente",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Producto_Cer_Mantenimiento",
                table: "rel_certificado_producto");

            migrationBuilder.DropIndex(
                name: "IX_rel_certificado_producto_id_certificado",
                table: "rel_certificado_producto");

            migrationBuilder.AlterColumn<long>(
                name: "id_certificado",
                table: "rel_certificado_producto",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "certificadoid",
                table: "rel_certificado_producto",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_rel_certificado_producto_certificadoid",
                table: "rel_certificado_producto",
                column: "certificadoid");

            migrationBuilder.AddForeignKey(
                name: "FK_rel_certificado_producto_Cer_producto_cliente_certificadoid",
                table: "rel_certificado_producto",
                column: "certificadoid",
                principalTable: "Cer_producto_cliente",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
