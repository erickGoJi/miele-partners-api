using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class quejas_servicio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "quejas_servicios",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_queja = table.Column<int>(nullable: false),
                    id_servicio = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quejas_servicios", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Servicio_Queja",
                        column: x => x.id_servicio,
                        principalTable: "Servicio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_quejas_servicios_id_servicio",
                table: "quejas_servicios",
                column: "id_servicio");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "quejas_servicios");
        }
    }
}
