using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class sublineas_categorias : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Categoria_Producto",
                table: "Cat_Productos");

            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Categoria_Producto_Rel",
                table: "Rel_Categoria_Producto_Tipo_Producto");

            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Users_Tecnicos_Producto_Cat",
                table: "Tecnicos_Producto");

            migrationBuilder.DropIndex(
                name: "IX_Cat_Productos_id_categoria",
                table: "Cat_Productos");

            migrationBuilder.AddColumn<int>(
                name: "Cat_Categoria_Productoid",
                table: "Tecnicos_Producto",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cat_Categoria_Productoid",
                table: "Rel_Categoria_Producto_Tipo_Producto",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Cat_Categoria_Productoid",
                table: "Cat_Productos",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tecnicos_Producto_Cat_Categoria_Productoid",
                table: "Tecnicos_Producto",
                column: "Cat_Categoria_Productoid");

            migrationBuilder.CreateIndex(
                name: "IX_Rel_Categoria_Producto_Tipo_Producto_Cat_Categoria_Productoid",
                table: "Rel_Categoria_Producto_Tipo_Producto",
                column: "Cat_Categoria_Productoid");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Productos_Cat_Categoria_Productoid",
                table: "Cat_Productos",
                column: "Cat_Categoria_Productoid");

            migrationBuilder.AddForeignKey(
                name: "FK_Cat_Productos_Cat_Categoria_Producto_Cat_Categoria_Productoid",
                table: "Cat_Productos",
                column: "Cat_Categoria_Productoid",
                principalTable: "Cat_Categoria_Producto",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Rel_Categoria_Producto_Tipo_Producto_Cat_Categoria_Producto_Cat_Categoria_Productoid",
                table: "Rel_Categoria_Producto_Tipo_Producto",
                column: "Cat_Categoria_Productoid",
                principalTable: "Cat_Categoria_Producto",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Categoria_Producto_Rel",
                table: "Rel_Categoria_Producto_Tipo_Producto",
                column: "id_categoria",
                principalTable: "Cat_SubLinea_Producto",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tecnicos_Producto_Cat_Categoria_Producto_Cat_Categoria_Productoid",
                table: "Tecnicos_Producto",
                column: "Cat_Categoria_Productoid",
                principalTable: "Cat_Categoria_Producto",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Users_Tecnicos_Producto_Cat",
                table: "Tecnicos_Producto",
                column: "id_categoria_producto",
                principalTable: "Cat_SubLinea_Producto",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cat_Productos_Cat_Categoria_Producto_Cat_Categoria_Productoid",
                table: "Cat_Productos");

            migrationBuilder.DropForeignKey(
                name: "FK_Rel_Categoria_Producto_Tipo_Producto_Cat_Categoria_Producto_Cat_Categoria_Productoid",
                table: "Rel_Categoria_Producto_Tipo_Producto");

            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Categoria_Producto_Rel",
                table: "Rel_Categoria_Producto_Tipo_Producto");

            migrationBuilder.DropForeignKey(
                name: "FK_Tecnicos_Producto_Cat_Categoria_Producto_Cat_Categoria_Productoid",
                table: "Tecnicos_Producto");

            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Users_Tecnicos_Producto_Cat",
                table: "Tecnicos_Producto");

            migrationBuilder.DropIndex(
                name: "IX_Tecnicos_Producto_Cat_Categoria_Productoid",
                table: "Tecnicos_Producto");

            migrationBuilder.DropIndex(
                name: "IX_Rel_Categoria_Producto_Tipo_Producto_Cat_Categoria_Productoid",
                table: "Rel_Categoria_Producto_Tipo_Producto");

            migrationBuilder.DropIndex(
                name: "IX_Cat_Productos_Cat_Categoria_Productoid",
                table: "Cat_Productos");

            migrationBuilder.DropColumn(
                name: "Cat_Categoria_Productoid",
                table: "Tecnicos_Producto");

            migrationBuilder.DropColumn(
                name: "Cat_Categoria_Productoid",
                table: "Rel_Categoria_Producto_Tipo_Producto");

            migrationBuilder.DropColumn(
                name: "Cat_Categoria_Productoid",
                table: "Cat_Productos");

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Productos_id_categoria",
                table: "Cat_Productos",
                column: "id_categoria");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Categoria_Producto",
                table: "Cat_Productos",
                column: "id_categoria",
                principalTable: "Cat_Categoria_Producto",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Categoria_Producto_Rel",
                table: "Rel_Categoria_Producto_Tipo_Producto",
                column: "id_categoria",
                principalTable: "Cat_Categoria_Producto",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Users_Tecnicos_Producto_Cat",
                table: "Tecnicos_Producto",
                column: "id_categoria_producto",
                principalTable: "Cat_Categoria_Producto",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
