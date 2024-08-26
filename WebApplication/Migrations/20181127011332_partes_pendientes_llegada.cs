using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class partes_pendientes_llegada : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Visita_llegada");

            migrationBuilder.DropTable(
                name: "Visita_solicitada");

            migrationBuilder.AddColumn<bool>(
                name: "llegada",
                table: "Piezas_Repuesto",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "solicitada",
                table: "Piezas_Repuesto",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "llegada",
                table: "Piezas_Repuesto");

            migrationBuilder.DropColumn(
                name: "solicitada",
                table: "Piezas_Repuesto");

            migrationBuilder.CreateTable(
                name: "Visita_llegada",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    cantidad = table.Column<int>(nullable: false),
                    id_material = table.Column<long>(nullable: false),
                    id_servicio = table.Column<int>(nullable: false),
                    llegada = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visita_llegada", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Visita_solicitada",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    cantidad = table.Column<int>(nullable: false),
                    id_material = table.Column<long>(nullable: false),
                    id_servicio = table.Column<int>(nullable: false),
                    solicitada = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visita_solicitada", x => x.id);
                });
        }
    }
}
