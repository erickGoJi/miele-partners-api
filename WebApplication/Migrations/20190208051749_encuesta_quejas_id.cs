using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class encuesta_quejas_id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_e_queja_servicio",
                table: "encuesta_queja");

            migrationBuilder.DropIndex(
                name: "IX_encuesta_queja_id_servicio",
                table: "encuesta_queja");

            migrationBuilder.DropColumn(
                name: "id_servicio",
                table: "encuesta_queja");

            migrationBuilder.AddColumn<int>(
                name: "id_queja",
                table: "encuesta_queja",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "id_queja",
                table: "encuesta_queja");

            migrationBuilder.AddColumn<long>(
                name: "id_servicio",
                table: "encuesta_queja",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_encuesta_queja_id_servicio",
                table: "encuesta_queja",
                column: "id_servicio");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_e_queja_servicio",
                table: "encuesta_queja",
                column: "id_servicio",
                principalTable: "Servicio",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
