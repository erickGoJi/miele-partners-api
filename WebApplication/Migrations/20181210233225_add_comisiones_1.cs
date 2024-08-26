using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class add_comisiones_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "comision_vendedor",
                table: "Cotizaciones",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateTable(
                name: "cat_tipos_comisiones",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Estatus_es = table.Column<string>(nullable: true),
                    Estatus_en = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cat_tipos_comisiones", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "comisiones_sucursales",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_cotizacion = table.Column<long>(nullable: false),
                    id_cat_tipo_comision = table.Column<int>(nullable: false),
                    fecha_generacion = table.Column<DateTime>(nullable: false),
                    pago_programado = table.Column<DateTime>(nullable: false),
                    fecha_de_pago = table.Column<DateTime>(nullable: false),
                    pagada = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comisiones_sucursales", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_comisiones_sucursales_tipo_comisiones",
                        column: x => x.id_cat_tipo_comision,
                        principalTable: "cat_tipos_comisiones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_comisiones_sucursales_cotizaciones",
                        column: x => x.id_cotizacion,
                        principalTable: "Cotizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "comisiones_vendedores",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_cotizacion = table.Column<long>(nullable: false),
                    id_cat_tipo_comision = table.Column<int>(nullable: false),
                    fecha_generacion = table.Column<DateTime>(nullable: false),
                    pago_programado = table.Column<DateTime>(nullable: false),
                    fecha_de_pago = table.Column<DateTime>(nullable: false),
                    pagada = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comisiones_vendedores", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_comisiones_vendedores_tipo_comisiones",
                        column: x => x.id_cat_tipo_comision,
                        principalTable: "cat_tipos_comisiones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_comisiones_vendedores_cotizaciones",
                        column: x => x.id_cotizacion,
                        principalTable: "Cotizaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_sucursales_id_cat_tipo_comision",
                table: "comisiones_sucursales",
                column: "id_cat_tipo_comision");

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_sucursales_id_cotizacion",
                table: "comisiones_sucursales",
                column: "id_cotizacion");

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_vendedores_id_cat_tipo_comision",
                table: "comisiones_vendedores",
                column: "id_cat_tipo_comision");

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_vendedores_id_cotizacion",
                table: "comisiones_vendedores",
                column: "id_cotizacion");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comisiones_sucursales");

            migrationBuilder.DropTable(
                name: "comisiones_vendedores");

            migrationBuilder.DropTable(
                name: "cat_tipos_comisiones");

            migrationBuilder.DropColumn(
                name: "comision_vendedor",
                table: "Cotizaciones");
        }
    }
}
