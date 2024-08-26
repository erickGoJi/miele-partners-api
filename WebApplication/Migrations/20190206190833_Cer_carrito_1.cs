using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class Cer_carrito_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estatus_encuenta",
                table: "Servicio");

            migrationBuilder.CreateTable(
                name: "Cer_producto_carrito",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    folio = table.Column<string>(nullable: true),
                    id_carrito = table.Column<long>(nullable: false),
                    id_viaticos = table.Column<int>(nullable: false),
                    id_labor = table.Column<int>(nullable: false),
                    costo = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cer_producto_carrito", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rel_certificado_carrito",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_carrito = table.Column<int>(nullable: false),
                    id_producto = table.Column<int>(nullable: false),
                    estatus_activo = table.Column<bool>(nullable: false),
                    no_visitas = table.Column<int>(nullable: false),
                    id_sub_linea = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rel_certificado_carrito", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cer_producto_carrito");

            migrationBuilder.DropTable(
                name: "rel_certificado_carrito");

            migrationBuilder.AddColumn<bool>(
                name: "estatus_encuenta",
                table: "Servicio",
                nullable: true);
        }
    }
}
