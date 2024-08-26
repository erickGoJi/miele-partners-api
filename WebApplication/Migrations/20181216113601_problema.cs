using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class problema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "modelo",
                table: "Cat_Productos_Respuestas_Troubleshooting");

            migrationBuilder.DropColumn(
                name: "modelo",
                table: "Cat_Productos_Preguntas_Troubleshooting");

            migrationBuilder.AddColumn<int>(
                name: "id_pregunta",
                table: "Cat_Productos_Respuestas_Troubleshooting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "id_problema",
                table: "Cat_Productos_Preguntas_Troubleshooting",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Cat_Productos_Problema_Troubleshooting",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    problema = table.Column<string>(nullable: true),
                    modelo = table.Column<string>(nullable: true),
                    estatus = table.Column<bool>(nullable: false),
                    creado = table.Column<DateTime>(nullable: false),
                    creadopor = table.Column<long>(nullable: false),
                    actualizado = table.Column<DateTime>(nullable: false),
                    actualizadopor = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Productos_Problema_Troubleshooting", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cat_Productos_Problema_Troubleshooting");

            migrationBuilder.DropColumn(
                name: "id_pregunta",
                table: "Cat_Productos_Respuestas_Troubleshooting");

            migrationBuilder.DropColumn(
                name: "id_problema",
                table: "Cat_Productos_Preguntas_Troubleshooting");

            migrationBuilder.AddColumn<string>(
                name: "modelo",
                table: "Cat_Productos_Respuestas_Troubleshooting",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "modelo",
                table: "Cat_Productos_Preguntas_Troubleshooting",
                nullable: true);
        }
    }
}
