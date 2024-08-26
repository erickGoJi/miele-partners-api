using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class cotizacion_monto_descuento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cotizacion_monto_descuento",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_cotizacion = table.Column<long>(type: "bigint", nullable: false),
                    id_promocion = table.Column<int>(type: "int", nullable: false),
                    monto_desc_con_iva = table.Column<float>(type: "real", nullable: false),
                    monto_desc_sin_iva = table.Column<float>(type: "real", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cotizacion_monto_descuento", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_cotizacion_monto_descuento_cotizacion",
                        column: x => x.id_cotizacion,
                        principalTable: "Cotizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_cotizacion_monto_descuento_promocion",
                        column: x => x.id_promocion,
                        principalTable: "promocion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cotizacion_monto_descuento_id_cotizacion",
                table: "cotizacion_monto_descuento",
                column: "id_cotizacion");

            migrationBuilder.CreateIndex(
                name: "IX_cotizacion_monto_descuento_id_promocion",
                table: "cotizacion_monto_descuento",
                column: "id_promocion");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cotizacion_monto_descuento");
        }
    }
}
