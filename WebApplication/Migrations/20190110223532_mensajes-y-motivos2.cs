using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class mensajesymotivos2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fecha_envio",
                table: "mensajes");

            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_msj",
                table: "mensajes",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fecha_msj",
                table: "mensajes");

            migrationBuilder.AddColumn<int>(
                name: "fecha_envio",
                table: "mensajes",
                nullable: false,
                defaultValue: 0);
        }
    }
}
