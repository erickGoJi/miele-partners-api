using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class AddcomisionesSucrusal : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Cat_Direccion_sucursales",
                table: "Cat_Direccion_sucursales");

            migrationBuilder.DropColumn(
                name: "Estatus_en",
                table: "cat_tipos_comisiones");

            migrationBuilder.RenameTable(
                name: "Cat_Direccion_sucursales",
                newName: "cat_direccion_sucursales");

            migrationBuilder.RenameColumn(
                name: "Estatus_es",
                table: "cat_tipos_comisiones",
                newName: "tipo_comision");

            migrationBuilder.RenameIndex(
                name: "IX_Cat_Direccion_sucursales_id_sucursales",
                table: "cat_direccion_sucursales",
                newName: "IX_cat_direccion_sucursales_id_sucursales");

            migrationBuilder.AddColumn<long>(
                name: "id_quienpago",
                table: "comisiones_vendedores",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "id_quienpago",
                table: "comisiones_sucursales",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_cat_direccion_sucursales",
                table: "cat_direccion_sucursales",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_vendedores_id_quienpago",
                table: "comisiones_vendedores",
                column: "id_quienpago");

            migrationBuilder.CreateIndex(
                name: "IX_comisiones_sucursales_id_quienpago",
                table: "comisiones_sucursales",
                column: "id_quienpago");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Users_comisiones_sucursales",
                table: "comisiones_sucursales",
                column: "id_quienpago",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Users_comisiones_vendedoress",
                table: "comisiones_vendedores",
                column: "id_quienpago",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Users_comisiones_sucursales",
                table: "comisiones_sucursales");

            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Users_comisiones_vendedoress",
                table: "comisiones_vendedores");

            migrationBuilder.DropIndex(
                name: "IX_comisiones_vendedores_id_quienpago",
                table: "comisiones_vendedores");

            migrationBuilder.DropIndex(
                name: "IX_comisiones_sucursales_id_quienpago",
                table: "comisiones_sucursales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_cat_direccion_sucursales",
                table: "cat_direccion_sucursales");

            migrationBuilder.DropColumn(
                name: "id_quienpago",
                table: "comisiones_vendedores");

            migrationBuilder.DropColumn(
                name: "id_quienpago",
                table: "comisiones_sucursales");

            migrationBuilder.RenameTable(
                name: "cat_direccion_sucursales",
                newName: "Cat_Direccion_sucursales");

            migrationBuilder.RenameColumn(
                name: "tipo_comision",
                table: "cat_tipos_comisiones",
                newName: "Estatus_es");

            migrationBuilder.RenameIndex(
                name: "IX_cat_direccion_sucursales_id_sucursales",
                table: "Cat_Direccion_sucursales",
                newName: "IX_Cat_Direccion_sucursales_id_sucursales");

            migrationBuilder.AddColumn<string>(
                name: "Estatus_en",
                table: "cat_tipos_comisiones",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Cat_Direccion_sucursales",
                table: "Cat_Direccion_sucursales",
                column: "id");
        }
    }
}
