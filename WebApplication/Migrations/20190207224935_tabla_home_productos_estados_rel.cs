using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class tabla_home_productos_estados_rel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "home_Producto_Estados",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_producto_home = table.Column<int>(nullable: false),
                    id_estado = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_home_Producto_Estados", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Estados_Home_Program",
                        column: x => x.id_estado,
                        principalTable: "Cat_Estado",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Productos_Home_Estados",
                        column: x => x.id_producto_home,
                        principalTable: "Cat_Productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_home_Producto_Estados_id_estado",
                table: "home_Producto_Estados",
                column: "id_estado");

            migrationBuilder.CreateIndex(
                name: "IX_home_Producto_Estados_id_producto_home",
                table: "home_Producto_Estados",
                column: "id_producto_home");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "home_Producto_Estados");
        }
    }
}
