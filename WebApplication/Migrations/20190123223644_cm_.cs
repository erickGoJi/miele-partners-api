using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class cm_ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estatus_activo",
                table: "Cer_producto_cliente");

            migrationBuilder.RenameColumn(
                name: "no_visitas",
                table: "Cer_producto_cliente",
                newName: "costo");

            migrationBuilder.AddColumn<bool>(
                name: "estatus_activo",
                table: "rel_certificado_producto",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "no_visitas",
                table: "rel_certificado_producto",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Home_producto_cliente",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    folio = table.Column<string>(nullable: true),
                    id_cliente = table.Column<long>(nullable: false),
                    clientesid = table.Column<long>(nullable: true),
                    no_visitas = table.Column<int>(nullable: false),
                    costo = table.Column<int>(nullable: false),
                    horas = table.Column<int>(nullable: false),
                    estatus_activo = table.Column<bool>(nullable: false),
                    estatus_venta = table.Column<bool>(nullable: false),
                    creado = table.Column<DateTime>(nullable: false),
                    creadopor = table.Column<long>(nullable: false),
                    actualizado = table.Column<DateTime>(nullable: false),
                    actualizadopor = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Home_producto_cliente", x => x.id);
                    table.ForeignKey(
                        name: "FK_Home_producto_cliente_Clientes_clientesid",
                        column: x => x.clientesid,
                        principalTable: "Clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Home_producto_cliente_clientesid",
                table: "Home_producto_cliente",
                column: "clientesid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Home_producto_cliente");

            migrationBuilder.DropColumn(
                name: "estatus_activo",
                table: "rel_certificado_producto");

            migrationBuilder.DropColumn(
                name: "no_visitas",
                table: "rel_certificado_producto");

            migrationBuilder.RenameColumn(
                name: "costo",
                table: "Cer_producto_cliente",
                newName: "no_visitas");

            migrationBuilder.AddColumn<bool>(
                name: "estatus_activo",
                table: "Cer_producto_cliente",
                nullable: false,
                defaultValue: false);
        }
    }
}
