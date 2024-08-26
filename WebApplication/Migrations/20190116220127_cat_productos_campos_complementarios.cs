using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class cat_productos_campos_complementarios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "id_caracteristica_base",
                table: "Cat_Productos",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "url_impresion",
                table: "Cat_Productos",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "id_caracteristica_base",
                table: "Cat_Productos");

            migrationBuilder.DropColumn(
                name: "url_impresion",
                table: "Cat_Productos");
        }
    }
}
