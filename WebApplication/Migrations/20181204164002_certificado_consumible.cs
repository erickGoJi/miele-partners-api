using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class certificado_consumible : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Accesorio_Mantenimiento",
                table: "Cer_producto_cliente");

            migrationBuilder.DropIndex(
                name: "IX_Cer_producto_cliente_id_accesorio",
                table: "Cer_producto_cliente");

            migrationBuilder.DropColumn(
                name: "id_accesorio",
                table: "Cer_producto_cliente");

            migrationBuilder.AddColumn<int>(
                name: "Cer_accesoriosid",
                table: "Cer_producto_cliente",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_consumible",
                table: "Cer_producto_cliente",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Cer_consumibles",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    consumible = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    costo_unitario = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    id_sublinea = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cer_consumibles", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cer_producto_cliente_Cer_accesoriosid",
                table: "Cer_producto_cliente",
                column: "Cer_accesoriosid");

            migrationBuilder.CreateIndex(
                name: "IX_Cer_producto_cliente_id_consumible",
                table: "Cer_producto_cliente",
                column: "id_consumible");

            migrationBuilder.AddForeignKey(
                name: "FK_Cer_producto_cliente_Cer_accesorios_Cer_accesoriosid",
                table: "Cer_producto_cliente",
                column: "Cer_accesoriosid",
                principalTable: "Cer_accesorios",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Consumible_Mantenimiento",
                table: "Cer_producto_cliente",
                column: "id_consumible",
                principalTable: "Cer_consumibles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cer_producto_cliente_Cer_accesorios_Cer_accesoriosid",
                table: "Cer_producto_cliente");

            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Consumible_Mantenimiento",
                table: "Cer_producto_cliente");

            migrationBuilder.DropTable(
                name: "Cer_consumibles");

            migrationBuilder.DropIndex(
                name: "IX_Cer_producto_cliente_Cer_accesoriosid",
                table: "Cer_producto_cliente");

            migrationBuilder.DropIndex(
                name: "IX_Cer_producto_cliente_id_consumible",
                table: "Cer_producto_cliente");

            migrationBuilder.DropColumn(
                name: "Cer_accesoriosid",
                table: "Cer_producto_cliente");

            migrationBuilder.DropColumn(
                name: "id_consumible",
                table: "Cer_producto_cliente");

            migrationBuilder.AddColumn<int>(
                name: "id_accesorio",
                table: "Cer_producto_cliente",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Cer_producto_cliente_id_accesorio",
                table: "Cer_producto_cliente",
                column: "id_accesorio");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Accesorio_Mantenimiento",
                table: "Cer_producto_cliente",
                column: "id_accesorio",
                principalTable: "Cer_accesorios",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
