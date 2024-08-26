using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class home_program : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Home_producto_cliente_Clientes_clientesid",
                table: "Home_producto_cliente");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Home_producto_cliente",
                table: "Home_producto_cliente");

            migrationBuilder.DropIndex(
                name: "IX_Home_producto_cliente_clientesid",
                table: "Home_producto_cliente");

            migrationBuilder.DropColumn(
                name: "clientesid",
                table: "Home_producto_cliente");

            migrationBuilder.RenameTable(
                name: "Home_producto_cliente",
                newName: "home_producto_cliente");

            migrationBuilder.AddPrimaryKey(
                name: "PK_home_producto_cliente",
                table: "home_producto_cliente",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_home_producto_cliente_id_cliente",
                table: "home_producto_cliente",
                column: "id_cliente");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Cliente_Home_Program",
                table: "home_producto_cliente",
                column: "id_cliente",
                principalTable: "Clientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Cliente_Home_Program",
                table: "home_producto_cliente");

            migrationBuilder.DropPrimaryKey(
                name: "PK_home_producto_cliente",
                table: "home_producto_cliente");

            migrationBuilder.DropIndex(
                name: "IX_home_producto_cliente_id_cliente",
                table: "home_producto_cliente");

            migrationBuilder.RenameTable(
                name: "home_producto_cliente",
                newName: "Home_producto_cliente");

            migrationBuilder.AddColumn<long>(
                name: "clientesid",
                table: "Home_producto_cliente",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Home_producto_cliente",
                table: "Home_producto_cliente",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_Home_producto_cliente_clientesid",
                table: "Home_producto_cliente",
                column: "clientesid");

            migrationBuilder.AddForeignKey(
                name: "FK_Home_producto_cliente_Clientes_clientesid",
                table: "Home_producto_cliente",
                column: "clientesid",
                principalTable: "Clientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
