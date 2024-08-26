using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class mensajesymotivos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cat_Motivos",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    descripcion = table.Column<string>(nullable: true),
                    correo = table.Column<string>(nullable: true),
                    estatus = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cat_Motivos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mensajes",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    motivo_id = table.Column<int>(nullable: false),
                    orden_id = table.Column<int>(nullable: false),
                    detalle = table.Column<int>(nullable: false),
                    usuario_id = table.Column<int>(nullable: false),
                    fecha_envio = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mensajes", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Motivo_Mensajes",
                        column: x => x.motivo_id,
                        principalTable: "cat_Motivos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mensajes_motivo_id",
                table: "mensajes",
                column: "motivo_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mensajes");

            migrationBuilder.DropTable(
                name: "cat_Motivos");
        }
    }
}
