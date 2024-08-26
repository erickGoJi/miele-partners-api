using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class viaticos_cp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estaso",
                table: "Cer_viaticos");

            migrationBuilder.DropColumn(
                name: "municipio",
                table: "Cer_viaticos");

            migrationBuilder.RenameColumn(
                name: "cp",
                table: "Cer_viaticos",
                newName: "id_cat_localidad");

            migrationBuilder.CreateIndex(
                name: "IX_Cer_viaticos_id_cat_localidad",
                table: "Cer_viaticos",
                column: "id_cat_localidad");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_cat_localidad_viaticos",
                table: "Cer_viaticos",
                column: "id_cat_localidad",
                principalTable: "Cat_Localidad",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_cat_localidad_viaticos",
                table: "Cer_viaticos");

            migrationBuilder.DropIndex(
                name: "IX_Cer_viaticos_id_cat_localidad",
                table: "Cer_viaticos");

            migrationBuilder.RenameColumn(
                name: "id_cat_localidad",
                table: "Cer_viaticos",
                newName: "cp");

            migrationBuilder.AddColumn<string>(
                name: "estaso",
                table: "Cer_viaticos",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "municipio",
                table: "Cer_viaticos",
                nullable: true);
        }
    }
}
