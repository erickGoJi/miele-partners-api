using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class direccion_cotizacion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "direcciones_cotizacion",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_cotizacion = table.Column<long>(nullable: false),
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
                    table.PrimaryKey("PK_direcciones_cotizacion", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "direcciones_cotizacion");
        }
    }
}
