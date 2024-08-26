using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class direcciones_Suc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cat_Direccion_sucursales",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_sucursales = table.Column<int>(nullable: false),
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
                    table.PrimaryKey("PK_Cat_Direccion_sucursales", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_cat_direcciones_sucu_cat_suc",
                        column: x => x.id_sucursales,
                        principalTable: "Cat_Sucursales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cat_Direccion_sucursales_id_sucursales",
                table: "Cat_Direccion_sucursales",
                column: "id_sucursales");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cat_Direccion_sucursales");
        }
    }
}
