using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class consumible_sublinea : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "id_sublinea",
                table: "Cer_consumibles");

            migrationBuilder.CreateTable(
                name: "rel_consumible_sublinea",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    consumibleid = table.Column<int>(type: "int", nullable: true),
                    id_consumible = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_sublinea = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rel_consumible_sublinea", x => x.id);
                    table.ForeignKey(
                        name: "FK_rel_consumible_sublinea_Cer_consumibles_consumibleid",
                        column: x => x.consumibleid,
                        principalTable: "Cer_consumibles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rel_consumible_sublinea_consumibleid",
                table: "rel_consumible_sublinea",
                column: "consumibleid");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "rel_consumible_sublinea");

            migrationBuilder.AddColumn<int>(
                name: "id_sublinea",
                table: "Cer_consumibles",
                nullable: false,
                defaultValue: 0);
        }
    }
}
