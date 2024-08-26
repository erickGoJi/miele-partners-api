using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class AgregarFacturacion_Scurusales : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "url_logo",
                table: "Cat_Sucursales",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DatosFiscales_Sucursales",
                columns: table => new
                {
                    id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    nombre_fact = table.Column<string>(nullable: true),
                    razon_social = table.Column<string>(nullable: true),
                    rfc = table.Column<string>(nullable: true),
                    email = table.Column<string>(nullable: true),
                    calle_numero = table.Column<string>(nullable: true),
                    cp = table.Column<string>(nullable: true),
                    id_estado = table.Column<int>(nullable: false),
                    id_municipio = table.Column<int>(nullable: false),
                    colonia = table.Column<string>(nullable: true),
                    Ext_fact = table.Column<string>(nullable: true),
                    Int_fact = table.Column<string>(nullable: true),
                    telefono_fact = table.Column<string>(nullable: true),
                    id_Sucursal = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DatosFiscales_Sucursales", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_DatosFiscales_Sucursales_datos_fiscales",
                        column: x => x.id_Sucursal,
                        principalTable: "Cat_Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DatosFiscales_Sucursales_id_Sucursal",
                table: "DatosFiscales_Sucursales",
                column: "id_Sucursal");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DatosFiscales_Sucursales");

            migrationBuilder.DropColumn(
                name: "url_logo",
                table: "Cat_Sucursales");
        }
    }
}
