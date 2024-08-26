using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class modelo_respuesta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Cat_Productos_Respuestas_Troubleshooting",
                table: "Cat_Productos_Respuestas_Troubleshooting");

            migrationBuilder.DropIndex(
                name: "IX_Cat_Productos_Respuestas_Troubleshooting_id_troubleshooting",
                table: "Cat_Productos_Respuestas_Troubleshooting");

            migrationBuilder.DropColumn(
                name: "id_troubleshooting",
                table: "Cat_Productos_Respuestas_Troubleshooting");

            migrationBuilder.AddColumn<string>(
                name: "modelo",
                table: "Cat_Productos_Respuestas_Troubleshooting",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "modelo",
                table: "Cat_Productos_Respuestas_Troubleshooting");

            migrationBuilder.AddColumn<int>(
                name: "id_troubleshooting",
                table: "Cat_Productos_Respuestas_Troubleshooting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Productos_Respuestas_Troubleshooting_id_troubleshooting",
                table: "Cat_Productos_Respuestas_Troubleshooting",
                column: "id_troubleshooting");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Cat_Productos_Respuestas_Troubleshooting",
                table: "Cat_Productos_Respuestas_Troubleshooting",
                column: "id_troubleshooting",
                principalTable: "Cat_Productos_Preguntas_Troubleshooting",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
