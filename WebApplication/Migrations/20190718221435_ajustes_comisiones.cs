using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class ajustes_comisiones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "aplica_comision",
                table: "config_comisiones",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "monto_superior",
                table: "config_comisiones",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<float>(
                name: "margen",
                table: "com_afectacion_ccs",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "aplica_comision",
                table: "config_comisiones");

            migrationBuilder.DropColumn(
                name: "monto_superior",
                table: "config_comisiones");

            migrationBuilder.AlterColumn<int>(
                name: "margen",
                table: "com_afectacion_ccs",
                nullable: false,
                oldClrType: typeof(float));
        }
    }
}
