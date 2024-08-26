using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class AgregarCotProd_regalos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "agregado_automaticamente",
                table: "Cotizacion_Producto",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "es_regalo",
                table: "Cotizacion_Producto",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "agregado_automaticamente",
                table: "Cotizacion_Producto");

            migrationBuilder.DropColumn(
                name: "es_regalo",
                table: "Cotizacion_Producto");
        }
    }
}
