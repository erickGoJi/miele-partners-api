using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class encuestas_12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "encuesta_general",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    pregunta_1 = table.Column<string>(nullable: true),
                    pregunta_2 = table.Column<string>(nullable: true),
                    pregunta_3 = table.Column<string>(nullable: true),
                    pregunta_4 = table.Column<string>(nullable: true),
                    pregunta_5 = table.Column<string>(nullable: true),
                    pregunta_6 = table.Column<string>(nullable: true),
                    pregunta_7 = table.Column<string>(nullable: true),
                    pregunta_8 = table.Column<string>(nullable: true),
                    pregunta_9 = table.Column<string>(nullable: true),
                    pregunta_10 = table.Column<string>(nullable: true),
                    pregunta_11 = table.Column<string>(nullable: true),
                    pregunta_12 = table.Column<string>(nullable: true),
                    pregunta_13 = table.Column<string>(nullable: true),
                    pregunta_14 = table.Column<string>(nullable: true),
                    pregunta_15 = table.Column<string>(nullable: true),
                    id_servicio = table.Column<long>(nullable: false),
                    id_cliente = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_encuesta_general", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_e_general_servicio",
                        column: x => x.id_servicio,
                        principalTable: "Servicio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "encuesta_queja",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    pregunta_1 = table.Column<string>(nullable: true),
                    pregunta_2 = table.Column<string>(nullable: true),
                    pregunta_3 = table.Column<string>(nullable: true),
                    pregunta_4 = table.Column<string>(nullable: true),
                    pregunta_5 = table.Column<string>(nullable: true),
                    pregunta_6 = table.Column<string>(nullable: true),
                    pregunta_7 = table.Column<string>(nullable: true),
                    pregunta_8 = table.Column<string>(nullable: true),
                    pregunta_9 = table.Column<string>(nullable: true),
                    pregunta_10 = table.Column<string>(nullable: true),
                    pregunta_11 = table.Column<string>(nullable: true),
                    id_servicio = table.Column<long>(nullable: false),
                    id_cliente = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_encuesta_queja", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_e_queja_servicio",
                        column: x => x.id_servicio,
                        principalTable: "Servicio",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_encuesta_general_id_servicio",
                table: "encuesta_general",
                column: "id_servicio");

            migrationBuilder.CreateIndex(
                name: "IX_encuesta_queja_id_servicio",
                table: "encuesta_queja",
                column: "id_servicio");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "encuesta_general");

            migrationBuilder.DropTable(
                name: "encuesta_queja");
        }
    }
}
