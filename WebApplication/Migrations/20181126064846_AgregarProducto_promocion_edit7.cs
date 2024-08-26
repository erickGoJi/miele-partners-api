using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class AgregarProducto_promocion_edit7 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_producto_promocion_cat_productos",
                table: "Producto_Promocion");

            migrationBuilder.DropIndex(
                name: "IX_Producto_Promocion_id_producto",
                table: "Producto_Promocion");

            migrationBuilder.AlterColumn<int>(
                name: "id_producto",
                table: "Producto_Promocion",
                type: "int",
                nullable: false,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "id_producto",
                table: "Producto_Promocion",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Producto_Promocion_id_producto",
                table: "Producto_Promocion",
                column: "id_producto");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_producto_promocion_cat_productos",
                table: "Producto_Promocion",
                column: "id_producto",
                principalTable: "Cat_Producto",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
