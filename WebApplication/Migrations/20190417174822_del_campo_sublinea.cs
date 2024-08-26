using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class del_campo_sublinea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "activo",
                table: "comisiones_promo_sublinea_config");

            migrationBuilder.DropColumn(
                name: "id_sublinea",
                table: "comisiones_promo_sublinea_config");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "activo",
                table: "comisiones_promo_sublinea_config",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "id_sublinea",
                table: "comisiones_promo_sublinea_config",
                nullable: false,
                defaultValue: 0);
        }
    }
}
