using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class modelo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_cat_productos_preguntas_troubleshooting",
                table: "Cat_Productos_Preguntas_Troubleshooting");

            migrationBuilder.DropIndex(
                name: "IX_Cat_Productos_Preguntas_Troubleshooting_id_producto",
                table: "Cat_Productos_Preguntas_Troubleshooting");

            migrationBuilder.DropColumn(
                name: "id_producto",
                table: "Cat_Productos_Preguntas_Troubleshooting");

            migrationBuilder.AddColumn<int>(
                name: "Cat_Productosid",
                table: "Cat_Productos_Preguntas_Troubleshooting",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modelo",
                table: "Cat_Productos_Preguntas_Troubleshooting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Productos_Preguntas_Troubleshooting_Cat_Productosid",
                table: "Cat_Productos_Preguntas_Troubleshooting",
                column: "Cat_Productosid");

            migrationBuilder.AddForeignKey(
                name: "FK_Cat_Productos_Preguntas_Troubleshooting_Cat_Productos_Cat_Productosid",
                table: "Cat_Productos_Preguntas_Troubleshooting",
                column: "Cat_Productosid",
                principalTable: "Cat_Productos",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cat_Productos_Preguntas_Troubleshooting_Cat_Productos_Cat_Productosid",
                table: "Cat_Productos_Preguntas_Troubleshooting");

            migrationBuilder.DropIndex(
                name: "IX_Cat_Productos_Preguntas_Troubleshooting_Cat_Productosid",
                table: "Cat_Productos_Preguntas_Troubleshooting");

            migrationBuilder.DropColumn(
                name: "Cat_Productosid",
                table: "Cat_Productos_Preguntas_Troubleshooting");

            migrationBuilder.DropColumn(
                name: "modelo",
                table: "Cat_Productos_Preguntas_Troubleshooting");

            migrationBuilder.AddColumn<int>(
                name: "id_producto",
                table: "Cat_Productos_Preguntas_Troubleshooting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Productos_Preguntas_Troubleshooting_id_producto",
                table: "Cat_Productos_Preguntas_Troubleshooting",
                column: "id_producto");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_cat_productos_preguntas_troubleshooting",
                table: "Cat_Productos_Preguntas_Troubleshooting",
                column: "id_producto",
                principalTable: "Cat_Productos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
