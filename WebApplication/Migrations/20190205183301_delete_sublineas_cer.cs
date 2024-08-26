using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class delete_sublineas_cer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cat_Productosid",
                table: "rel_certificado_producto",
                nullable: true);

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
    }
}
