using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class homep_sublineas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "rel_homep_productos",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_homep = table.Column<int>(nullable: false),
                    id_producto = table.Column<int>(nullable: false),
                    estatus_activo = table.Column<bool>(nullable: false),
                    cantidad = table.Column<int>(nullable: false),
                    id_sub_linea = table.Column<int>(nullable: false),
                    creado = table.Column<DateTime>(nullable: false),
                    fecha_visita_1 = table.Column<DateTime>(nullable: false),
                    fecha_visita_2 = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rel_homep_productos", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_rel_hpp_productos_cliente",
                        column: x => x.id_homep,
                        principalTable: "home_producto_cliente",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rel_homep_productos_id_homep",
                table: "rel_homep_productos",
                column: "id_homep");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rel_homep_productos");
        }
    }
}
