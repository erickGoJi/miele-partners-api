using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class superlinea_y_caracteristica_base_relaciones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Cat_Productos_id_caracteristica_base",
                table: "Cat_Productos",
                column: "id_caracteristica_base");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Linea_Producto_id_superlinea",
                table: "Cat_Linea_Producto",
                column: "id_superlinea");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Cat_linea_Cat_superlinea",
                table: "Cat_Linea_Producto",
                column: "id_superlinea",
                principalTable: "cat_SuperLineas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_cat_productos_caracteristica_base",
                table: "Cat_Productos",
                column: "id_caracteristica_base",
                principalTable: "caracteristicas_Bases",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Cat_linea_Cat_superlinea",
                table: "Cat_Linea_Producto");

            migrationBuilder.DropForeignKey(
                name: "ForeignKey_cat_productos_caracteristica_base",
                table: "Cat_Productos");

            migrationBuilder.DropIndex(
                name: "IX_Cat_Productos_id_caracteristica_base",
                table: "Cat_Productos");

            migrationBuilder.DropIndex(
                name: "IX_Cat_Linea_Producto_id_superlinea",
                table: "Cat_Linea_Producto");
        }
    }
}
