using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class cer_cp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "region",
                table: "Cer_viaticos",
                newName: "municipio");

            migrationBuilder.RenameColumn(
                name: "area",
                table: "Cer_viaticos",
                newName: "estaso");

            migrationBuilder.AddColumn<int>(
                name: "cp",
                table: "Cer_viaticos",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cp",
                table: "Cer_viaticos");

            migrationBuilder.RenameColumn(
                name: "municipio",
                table: "Cer_viaticos",
                newName: "region");

            migrationBuilder.RenameColumn(
                name: "estaso",
                table: "Cer_viaticos",
                newName: "area");
        }
    }
}
