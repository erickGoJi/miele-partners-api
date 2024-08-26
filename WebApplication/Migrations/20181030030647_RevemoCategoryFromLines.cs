using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class RevemoCategoryFromLines : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cat_Linea_Producto_Cat_Categoria_Producto_id_categoriaid",
                table: "Cat_Linea_Producto");

            migrationBuilder.DropIndex(
                name: "IX_Cat_Linea_Producto_id_categoriaid",
                table: "Cat_Linea_Producto");

            migrationBuilder.DropColumn(
                name: "id_categoriaid",
                table: "Cat_Linea_Producto");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "id_categoriaid",
                table: "Cat_Linea_Producto",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Linea_Producto_id_categoriaid",
                table: "Cat_Linea_Producto",
                column: "id_categoriaid");

            migrationBuilder.AddForeignKey(
                name: "FK_Cat_Linea_Producto_Cat_Categoria_Producto_id_categoriaid",
                table: "Cat_Linea_Producto",
                column: "id_categoriaid",
                principalTable: "Cat_Categoria_Producto",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
