using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class certificado_mantenimiento : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cer_accesorios",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    accesorio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    costo_unitario = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    sublinea = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cer_accesorios", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cer_labor",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    anual = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    cantidad_equipos = table.Column<int>(type: "int", nullable: false),
                    costo_base = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    costo_unitario = table.Column<decimal>(type: "decimal(18, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cer_labor", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cer_viaticos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    anual = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    area = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    costo_unitario = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    region = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cer_viaticos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Cer_producto_cliente",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    actualizado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    actualizadopor = table.Column<long>(type: "bigint", nullable: false),
                    creado = table.Column<DateTime>(type: "datetime2", nullable: false),
                    creadopor = table.Column<long>(type: "bigint", nullable: false),
                    estatus_activo = table.Column<bool>(type: "bit", nullable: false),
                    id_accesorio = table.Column<int>(type: "int", nullable: false),
                    id_cliente = table.Column<long>(type: "bigint", nullable: false),
                    id_labor = table.Column<int>(type: "int", nullable: false),
                    id_producto = table.Column<int>(type: "int", nullable: false),
                    id_viaticos = table.Column<int>(type: "int", nullable: false),
                    no_visitas = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cer_producto_cliente", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Accesorio_Mantenimiento",
                        column: x => x.id_accesorio,
                        principalTable: "Cer_accesorios",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Cliente_Mantenimiento",
                        column: x => x.id_cliente,
                        principalTable: "Clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Labor_Mantenimiento",
                        column: x => x.id_labor,
                        principalTable: "Cer_labor",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Producto_Mantenimiento",
                        column: x => x.id_producto,
                        principalTable: "Cat_Productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Viaticos_Mantenimiento",
                        column: x => x.id_viaticos,
                        principalTable: "Cer_viaticos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cer_producto_cliente_id_accesorio",
                table: "Cer_producto_cliente",
                column: "id_accesorio");

            migrationBuilder.CreateIndex(
                name: "IX_Cer_producto_cliente_id_cliente",
                table: "Cer_producto_cliente",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Cer_producto_cliente_id_labor",
                table: "Cer_producto_cliente",
                column: "id_labor");

            migrationBuilder.CreateIndex(
                name: "IX_Cer_producto_cliente_id_producto",
                table: "Cer_producto_cliente",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_Cer_producto_cliente_id_viaticos",
                table: "Cer_producto_cliente",
                column: "id_viaticos");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cer_producto_cliente");

            migrationBuilder.DropTable(
                name: "Cer_accesorios");

            migrationBuilder.DropTable(
                name: "Cer_labor");

            migrationBuilder.DropTable(
                name: "Cer_viaticos");
        }
    }
}
