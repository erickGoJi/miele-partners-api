using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class config_comisiones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "config_comisiones",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(nullable: true),
                    fecha_hora_inicio = table.Column<string>(nullable: true),
                    fecha_hora_fin = table.Column<string>(nullable: true),
                    vigencia_indefinida = table.Column<bool>(nullable: false),
                    id_tipos_herencia_promo = table.Column<int>(nullable: false),
                    id_cat_tipo_condicion = table.Column<int>(nullable: false),
                    monto_condicion = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_config_comisiones", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_config_comision_condicion",
                        column: x => x.id_cat_tipo_condicion,
                        principalTable: "cat_tipo_condicion",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_config_comision_herencia",
                        column: x => x.id_tipos_herencia_promo,
                        principalTable: "cat_tipos_herencia",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_config_comisiones_id_cat_tipo_condicion",
                table: "config_comisiones",
                column: "id_cat_tipo_condicion");

            migrationBuilder.CreateIndex(
                name: "IX_config_comisiones_id_tipos_herencia_promo",
                table: "config_comisiones",
                column: "id_tipos_herencia_promo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "config_comisiones");
        }
    }
}
