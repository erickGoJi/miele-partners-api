using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class pdf_historial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "fecha_agendado",
                table: "Visita",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "url_ppdf_reporte",
                table: "Visita",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "fecha_agendado",
                table: "Visita");

            migrationBuilder.DropColumn(
                name: "url_ppdf_reporte",
                table: "Visita");
        }
    }
}
