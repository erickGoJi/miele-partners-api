using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class promos_tarjetas_meses : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "aplica_cc",
                table: "promocion",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "id_tarjeta",
                table: "Cotizaciones",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "cat_Tarjetas",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(nullable: true),
                    estatus = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cat_Tarjetas", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cat_Tarjetas");

            migrationBuilder.DropColumn(
                name: "aplica_cc",
                table: "promocion");

            migrationBuilder.DropColumn(
                name: "id_tarjeta",
                table: "Cotizaciones");
        }
    }
}
