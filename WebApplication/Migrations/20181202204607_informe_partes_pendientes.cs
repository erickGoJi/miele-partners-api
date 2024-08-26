using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class informe_partes_pendientes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "no_material",
                table: "Informe_parte_recibida",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<int>(
                name: "id_producto",
                table: "Informe_parte_recibida",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "no_material",
                table: "Informe_parte_recibida",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "id_producto",
                table: "Informe_parte_recibida",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
