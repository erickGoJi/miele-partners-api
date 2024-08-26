using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class add_rireccion_cliente : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Direcciones_Cliente",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_cliente = table.Column<long>(nullable: false),
                    calle_numero = table.Column<string>(nullable: true),
                    cp = table.Column<string>(nullable: true),
                    id_estado = table.Column<int>(nullable: false),
                    id_municipio = table.Column<int>(nullable: false),
                    colonia = table.Column<string>(nullable: true),
                    telefono = table.Column<string>(nullable: true),
                    telefono_movil = table.Column<string>(nullable: true),
                    estatus = table.Column<bool>(nullable: false),
                    creado = table.Column<DateTime>(nullable: false),
                    creadopor = table.Column<long>(nullable: false),
                    actualizado = table.Column<DateTime>(nullable: false),
                    actualizadopor = table.Column<long>(nullable: false),
                    tipo_direccion = table.Column<int>(nullable: true),
                    nombrecontacto = table.Column<string>(nullable: true),
                    numExt = table.Column<string>(nullable: true),
                    numInt = table.Column<string>(nullable: true),
                    Fecha_Estimada = table.Column<string>(nullable: true),
                    id_localidad = table.Column<int>(nullable: false),
                    id_prefijo_calle = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Direcciones_Cliente", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Direcciones_Clientes",
                        column: x => x.id_cliente,
                        principalTable: "Clientes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Direcciones_Cliente_id_cliente",
                table: "Direcciones_Cliente",
                column: "id_cliente");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Direcciones_Cliente");
        }
    }
}
