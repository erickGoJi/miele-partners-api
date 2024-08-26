using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class AgregarProducto_promocion_edit2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "id_cotizacion",
                table: "Producto_Promocion",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Producto_Promocion_id_cotizacion",
                table: "Producto_Promocion",
                column: "id_cotizacion");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_producto_promocion_cotizaciones",
                table: "Producto_Promocion",
                column: "id_cotizacion",
                principalTable: "Cotizaciones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_producto_promocion_cotizaciones",
                table: "Producto_Promocion");

            migrationBuilder.DropIndex(
                name: "IX_Producto_Promocion_id_cotizacion",
                table: "Producto_Promocion");

            migrationBuilder.DropColumn(
                name: "id_cotizacion",
                table: "Producto_Promocion");
        }
    }
}
