using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class ref_solicitadas_garantia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "garantia",
                table: "Piezas_Repuesto",
                nullable: false,
                defaultValue: false);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropColumn(
                name: "garantia",
                table: "Piezas_Repuesto");

        }
    }
}
