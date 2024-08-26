using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class consumible_sublinea_id : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_rel_consumible_sublinea_Cer_consumibles_consumibleid",
                table: "rel_consumible_sublinea");

            migrationBuilder.DropIndex(
                name: "IX_rel_consumible_sublinea_consumibleid",
                table: "rel_consumible_sublinea");

            migrationBuilder.DropColumn(
                name: "consumibleid",
                table: "rel_consumible_sublinea");

            migrationBuilder.AlterColumn<int>(
                name: "id_consumible",
                table: "rel_consumible_sublinea",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_rel_consumible_sublinea_id_consumible",
                table: "rel_consumible_sublinea",
                column: "id_consumible");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Consumible_sublinea",
                table: "rel_consumible_sublinea",
                column: "id_consumible",
                principalTable: "Cer_consumibles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Consumible_sublinea",
                table: "rel_consumible_sublinea");

            migrationBuilder.DropIndex(
                name: "IX_rel_consumible_sublinea_id_consumible",
                table: "rel_consumible_sublinea");

            migrationBuilder.AlterColumn<string>(
                name: "id_consumible",
                table: "rel_consumible_sublinea",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "consumibleid",
                table: "rel_consumible_sublinea",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_rel_consumible_sublinea_consumibleid",
                table: "rel_consumible_sublinea",
                column: "consumibleid");

            migrationBuilder.AddForeignKey(
                name: "FK_rel_consumible_sublinea_Cer_consumibles_consumibleid",
                table: "rel_consumible_sublinea",
                column: "consumibleid",
                principalTable: "Cer_consumibles",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
