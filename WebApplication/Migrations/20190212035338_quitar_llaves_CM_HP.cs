using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class quitar_llaves_CM_HP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Cliente_Mantenimiento",
                table: "Cer_producto_cliente");

            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Cliente_Home_Program",
                table: "home_producto_cliente");

            migrationBuilder.DropIndex(
                name: "IX_home_producto_cliente_id_cliente",
                table: "home_producto_cliente");

            migrationBuilder.DropIndex(
                name: "IX_Cer_producto_cliente_id_cliente",
                table: "Cer_producto_cliente");

            migrationBuilder.AddColumn<long>(
                name: "Clientesid",
                table: "home_producto_cliente",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Clientesid",
                table: "Cer_producto_cliente",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_home_producto_cliente_Clientesid",
                table: "home_producto_cliente",
                column: "Clientesid");

            migrationBuilder.CreateIndex(
                name: "IX_Cer_producto_cliente_Clientesid",
                table: "Cer_producto_cliente",
                column: "Clientesid");

            migrationBuilder.AddForeignKey(
                name: "FK_Cer_producto_cliente_Clientes_Clientesid",
                table: "Cer_producto_cliente",
                column: "Clientesid",
                principalTable: "Clientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_home_producto_cliente_Clientes_Clientesid",
                table: "home_producto_cliente",
                column: "Clientesid",
                principalTable: "Clientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cer_producto_cliente_Clientes_Clientesid",
                table: "Cer_producto_cliente");

            migrationBuilder.DropForeignKey(
                name: "FK_home_producto_cliente_Clientes_Clientesid",
                table: "home_producto_cliente");

            migrationBuilder.DropIndex(
                name: "IX_home_producto_cliente_Clientesid",
                table: "home_producto_cliente");

            migrationBuilder.DropIndex(
                name: "IX_Cer_producto_cliente_Clientesid",
                table: "Cer_producto_cliente");

            migrationBuilder.DropColumn(
                name: "Clientesid",
                table: "home_producto_cliente");

            migrationBuilder.DropColumn(
                name: "Clientesid",
                table: "Cer_producto_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_home_producto_cliente_id_cliente",
                table: "home_producto_cliente",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_Cer_producto_cliente_id_cliente",
                table: "Cer_producto_cliente",
                column: "id_cliente");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Cliente_Mantenimiento",
                table: "Cer_producto_cliente",
                column: "id_cliente",
                principalTable: "Clientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Cliente_Home_Program",
                table: "home_producto_cliente",
                column: "id_cliente",
                principalTable: "Clientes",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
