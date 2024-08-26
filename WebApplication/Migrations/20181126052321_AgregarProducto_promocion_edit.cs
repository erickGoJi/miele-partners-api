using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class AgregarProducto_promocion_edit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Producto_Producto",
                table: "Producto_Producto");

            migrationBuilder.RenameTable(
                name: "Producto_Producto",
                newName: "Producto_Promocion");

            migrationBuilder.RenameIndex(
                name: "IX_Producto_Producto_id_promocion",
                table: "Producto_Promocion",
                newName: "IX_Producto_Promocion_id_promocion");

            migrationBuilder.RenameIndex(
                name: "IX_Producto_Producto_id_producto",
                table: "Producto_Promocion",
                newName: "IX_Producto_Promocion_id_producto");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Producto_Promocion",
                table: "Producto_Promocion",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Producto_Promocion",
                table: "Producto_Promocion");

            migrationBuilder.RenameTable(
                name: "Producto_Promocion",
                newName: "Producto_Producto");

            migrationBuilder.RenameIndex(
                name: "IX_Producto_Promocion_id_promocion",
                table: "Producto_Producto",
                newName: "IX_Producto_Producto_id_promocion");

            migrationBuilder.RenameIndex(
                name: "IX_Producto_Promocion_id_producto",
                table: "Producto_Producto",
                newName: "IX_Producto_Producto_id_producto");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Producto_Producto",
                table: "Producto_Producto",
                column: "id");
        }
    }
}
