using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class comisiones_config : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "fecha_limite_pago",
                table: "Visita",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "aplica_comision_config",
                table: "promocion",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "monto_com_sin_iva",
                table: "comisiones_vendedores",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "monto_comision",
                table: "comisiones_vendedores",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "comisiones_promo_sublinea_config",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_promocion = table.Column<int>(nullable: false),
                    id_sublinea = table.Column<int>(nullable: false),
                    margen = table.Column<float>(nullable: false),
                    subLinea_Productoid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comisiones_promo_sublinea_config", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_promocion_comisiones_configuracion",
                        column: x => x.id_promocion,
                        principalTable: "promocion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_comisiones_promo_sublinea_config_Cat_SubLinea_Producto_subLinea_Productoid",
                        column: x => x.subLinea_Productoid,
                        principalTable: "Cat_SubLinea_Producto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_promo_sublinea_config_id_promocion",
                table: "comisiones_promo_sublinea_config",
                column: "id_promocion");

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_promo_sublinea_config_subLinea_Productoid",
                table: "comisiones_promo_sublinea_config",
                column: "subLinea_Productoid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "comisiones_promo_sublinea_config");

            migrationBuilder.DropColumn(
                name: "fecha_limite_pago",
                table: "Visita");

            migrationBuilder.DropColumn(
                name: "aplica_comision_config",
                table: "promocion");

            migrationBuilder.DropColumn(
                name: "monto_com_sin_iva",
                table: "comisiones_vendedores");

            migrationBuilder.DropColumn(
                name: "monto_comision",
                table: "comisiones_vendedores");
        }
    }
}
