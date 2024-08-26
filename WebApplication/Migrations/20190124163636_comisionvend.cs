using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class comisionvend : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "com_afectacion_ccs",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_comisionv = table.Column<int>(nullable: false),
                    id_condiones_comerciales_sucursal = table.Column<int>(nullable: false),
                    condiones_comerciales_sucursalid = table.Column<int>(nullable: true),
                    margen = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_com_afectacion_ccs", x => x.id);
                    table.ForeignKey(
                        name: "FK_com_afectacion_ccs_condiones_comerciales_sucursal_condiones_comerciales_sucursalid",
                        column: x => x.condiones_comerciales_sucursalid,
                        principalTable: "condiones_comerciales_sucursal",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "ForeignKey_config_comision_afectacion",
                        column: x => x.id_comisionv,
                        principalTable: "config_comisiones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "com_entidades_participantes",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_comisionv = table.Column<int>(nullable: false),
                    id_entidad = table.Column<int>(nullable: false),
                    id_tipo_entidad = table.Column<int>(nullable: false),
                    cat_tipo_entidadesid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_com_entidades_participantes", x => x.id);
                    table.ForeignKey(
                        name: "FK_com_entidades_participantes_cat_tipo_entidades_cat_tipo_entidadesid",
                        column: x => x.cat_tipo_entidadesid,
                        principalTable: "cat_tipo_entidades",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "ForeignKey_config_comision_participantes",
                        column: x => x.id_comisionv,
                        principalTable: "config_comisiones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "com_productos_condicions",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_comisionv = table.Column<int>(nullable: false),
                    id_producto = table.Column<int>(nullable: false),
                    id_tipo_categoria = table.Column<int>(nullable: false),
                    cantidad = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_com_productos_condicions", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_config_comision_productos_condicion",
                        column: x => x.id_comisionv,
                        principalTable: "config_comisiones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_com_afectacion_ccs_condiones_comerciales_sucursalid",
                table: "com_afectacion_ccs",
                column: "condiones_comerciales_sucursalid");

            migrationBuilder.CreateIndex(
                name: "IX_com_afectacion_ccs_id_comisionv",
                table: "com_afectacion_ccs",
                column: "id_comisionv");

            migrationBuilder.CreateIndex(
                name: "IX_com_entidades_participantes_cat_tipo_entidadesid",
                table: "com_entidades_participantes",
                column: "cat_tipo_entidadesid");

            migrationBuilder.CreateIndex(
                name: "IX_com_entidades_participantes_id_comisionv",
                table: "com_entidades_participantes",
                column: "id_comisionv");

            migrationBuilder.CreateIndex(
                name: "IX_com_productos_condicions_id_comisionv",
                table: "com_productos_condicions",
                column: "id_comisionv");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "com_afectacion_ccs");

            migrationBuilder.DropTable(
                name: "com_entidades_participantes");

            migrationBuilder.DropTable(
                name: "com_productos_condicions");
        }
    }
}
