using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class campo_horas_hp_sublineas1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "horas_hp",
                table: "Cat_SubLinea_Producto");

            migrationBuilder.AddColumn<float>(
                name: "hp_horas",
                table: "Cat_SubLinea_Producto",
                nullable: false,
                defaultValue: 0f);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hp_horas",
                table: "Cat_SubLinea_Producto");

            migrationBuilder.AddColumn<int>(
                name: "horas_hp",
                table: "Cat_SubLinea_Producto",
                nullable: false,
                defaultValue: 0);
        }
    }
}
