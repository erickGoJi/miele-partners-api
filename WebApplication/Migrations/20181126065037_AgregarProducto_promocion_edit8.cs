using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class AgregarProducto_promocion_edit8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Producto_Promocion_id_producto",
                table: "Producto_Promocion",
                column: "id_producto");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_producto_promocion_cat_productos",
                table: "Producto_Promocion",
                column: "id_producto",
                principalTable: "Cat_Productos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_producto_promocion_cat_productos",
                table: "Producto_Promocion");

            migrationBuilder.DropIndex(
                name: "IX_Producto_Promocion_id_producto",
                table: "Producto_Promocion");
        }
    }
}
