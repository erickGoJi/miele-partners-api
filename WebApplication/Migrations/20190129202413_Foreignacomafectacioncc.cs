using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class Foreignacomafectacioncc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "com_producto_promocions",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_comisionv = table.Column<int>(nullable: false),
                    id_producto = table.Column<int>(nullable: false),
                    Cat_Productosid = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_com_producto_promocions", x => x.id);
                    table.ForeignKey(
                        name: "FK_com_producto_promocions_Cat_Productos_Cat_Productosid",
                        column: x => x.Cat_Productosid,
                        principalTable: "Cat_Productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "ForeignKey_config_comision_productos_misiones_prom",
                        column: x => x.id_comisionv,
                        principalTable: "config_comisiones",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_com_producto_promocions_Cat_Productosid",
                table: "com_producto_promocions",
                column: "Cat_Productosid");

            migrationBuilder.CreateIndex(
                name: "IX_com_producto_promocions_id_comisionv",
                table: "com_producto_promocions",
                column: "id_comisionv");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "com_producto_promocions");
        }
    }
}
