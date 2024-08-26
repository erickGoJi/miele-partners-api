using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class sublinea_certificado_partners : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "agregado_automaticamente",
                table: "Productos_Carrito",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "es_regalo",
                table: "Productos_Carrito",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "iva_cond_comerciales",
                table: "Productos_Carrito",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "iva_precio_descuento",
                table: "Productos_Carrito",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "iva_precio_lista",
                table: "Productos_Carrito",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "margen_cc",
                table: "Productos_Carrito",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "precio_condiciones_com",
                table: "Productos_Carrito",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "precio_descuento",
                table: "Productos_Carrito",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "precio_lista",
                table: "Productos_Carrito",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.CreateTable(
                name: "sublinea_certificado_partners",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Id_producto_carrito = table.Column<int>(nullable: false),
                    Id_cotizacion_producto = table.Column<int>(nullable: false),
                    Id_sublinea = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sublinea_certificado_partners", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sublinea_certificado_partners");

            migrationBuilder.DropColumn(
                name: "agregado_automaticamente",
                table: "Productos_Carrito");

            migrationBuilder.DropColumn(
                name: "es_regalo",
                table: "Productos_Carrito");

            migrationBuilder.DropColumn(
                name: "iva_cond_comerciales",
                table: "Productos_Carrito");

            migrationBuilder.DropColumn(
                name: "iva_precio_descuento",
                table: "Productos_Carrito");

            migrationBuilder.DropColumn(
                name: "iva_precio_lista",
                table: "Productos_Carrito");

            migrationBuilder.DropColumn(
                name: "margen_cc",
                table: "Productos_Carrito");

            migrationBuilder.DropColumn(
                name: "precio_condiciones_com",
                table: "Productos_Carrito");

            migrationBuilder.DropColumn(
                name: "precio_descuento",
                table: "Productos_Carrito");

            migrationBuilder.DropColumn(
                name: "precio_lista",
                table: "Productos_Carrito");
        }
    }
}
