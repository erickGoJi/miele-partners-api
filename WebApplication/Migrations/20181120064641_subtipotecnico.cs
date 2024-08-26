using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class subtipotecnico : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "id_cat_tecnicos_sub_Tipo",
                table: "Tecnicos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Cat_Tecnicos_Sub_Tipo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Sub_desc_tipo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    id_tipo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cat_Tecnicos_Sub_Tipo", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cat_Tecnicos_Sub_Tipo");

            migrationBuilder.DropColumn(
                name: "id_cat_tecnicos_sub_Tipo",
                table: "Tecnicos");
        }
    }
}
