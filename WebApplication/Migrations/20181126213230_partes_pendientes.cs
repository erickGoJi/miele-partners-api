using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class partes_pendientes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Visita_llegada",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    id_material = table.Column<long>(type: "bigint", nullable: false),
                    id_servicio = table.Column<int>(type: "int", nullable: false),
                    llegada = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visita_llegada", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Visita_solicitada",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    cantidad = table.Column<int>(type: "int", nullable: false),
                    id_material = table.Column<long>(type: "bigint", nullable: false),
                    id_servicio = table.Column<int>(type: "int", nullable: false),
                    solicitada = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visita_solicitada", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Visita_llegada");

            migrationBuilder.DropTable(
                name: "Visita_solicitada");
        }
    }
}
