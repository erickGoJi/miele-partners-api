using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class certificados : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cer_producto_cliente_Cer_accesorios_Cer_accesoriosid",
                table: "Cer_producto_cliente");

            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Consumible_Mantenimiento",
                table: "Cer_producto_cliente");

            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Producto_Mantenimiento",
                table: "Cer_producto_cliente");

            migrationBuilder.DropForeignKey(
                name: "ForeignKey_Consumible_sublinea",
                table: "rel_consumible_sublinea");

            migrationBuilder.DropTable(
                name: "Cer_accesorios");

            migrationBuilder.DropIndex(
                name: "IX_Cer_producto_cliente_Cer_accesoriosid",
                table: "Cer_producto_cliente");

            migrationBuilder.DropIndex(
                name: "IX_Cer_producto_cliente_id_consumible",
                table: "Cer_producto_cliente");

            migrationBuilder.DropIndex(
                name: "IX_Cer_producto_cliente_id_producto",
                table: "Cer_producto_cliente");

            migrationBuilder.DropColumn(
                name: "Cer_accesoriosid",
                table: "Cer_producto_cliente");

            migrationBuilder.DropColumn(
                name: "id_consumible",
                table: "Cer_producto_cliente");

            migrationBuilder.DropColumn(
                name: "id_producto",
                table: "Cer_producto_cliente");

            migrationBuilder.CreateTable(
                name: "rel_certificado_producto",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_certificado = table.Column<long>(nullable: false),
                    certificadoid = table.Column<int>(nullable: true),
                    id_producto = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rel_certificado_producto", x => x.id);
                    table.ForeignKey(
                        name: "FK_rel_certificado_producto_Cer_producto_cliente_certificadoid",
                        column: x => x.certificadoid,
                        principalTable: "Cer_producto_cliente",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "ForeignKey_Producto_Mantenimiento",
                        column: x => x.id_producto,
                        principalTable: "Cat_Productos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rel_certificado_producto_consumibles",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    id_rel_certificado_producto = table.Column<int>(nullable: false),
                    id_consumible = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_rel_certificado_producto_consumibles", x => x.id);
                    table.ForeignKey(
                        name: "ForeignKey_Consumible_Mantenimiento",
                        column: x => x.id_consumible,
                        principalTable: "Cer_consumibles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "ForeignKey_Prodcuto_cer_Mantenimiento",
                        column: x => x.id_rel_certificado_producto,
                        principalTable: "rel_certificado_producto",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_rel_certificado_producto_certificadoid",
                table: "rel_certificado_producto",
                column: "certificadoid");

            migrationBuilder.CreateIndex(
                name: "IX_rel_certificado_producto_id_producto",
                table: "rel_certificado_producto",
                column: "id_producto");

            migrationBuilder.CreateIndex(
                name: "IX_rel_certificado_producto_consumibles_id_consumible",
                table: "rel_certificado_producto_consumibles",
                column: "id_consumible");

            migrationBuilder.CreateIndex(
                name: "IX_rel_certificado_producto_consumibles_id_rel_certificado_producto",
                table: "rel_certificado_producto_consumibles",
                column: "id_rel_certificado_producto");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_rel_consumible_sublinea",
                table: "rel_consumible_sublinea",
                column: "id_consumible",
                principalTable: "Cer_consumibles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_rel_consumible_sublinea",
                table: "rel_consumible_sublinea");

            migrationBuilder.DropTable(
                name: "rel_certificado_producto_consumibles");

            migrationBuilder.DropTable(
                name: "rel_certificado_producto");

            migrationBuilder.AddColumn<int>(
                name: "Cer_accesoriosid",
                table: "Cer_producto_cliente",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "id_consumible",
                table: "Cer_producto_cliente",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "id_producto",
                table: "Cer_producto_cliente",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Cer_accesorios",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    accesorio = table.Column<string>(nullable: true),
                    costo_unitario = table.Column<decimal>(nullable: false),
                    sublinea = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cer_accesorios", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cer_producto_cliente_Cer_accesoriosid",
                table: "Cer_producto_cliente",
                column: "Cer_accesoriosid");

            migrationBuilder.CreateIndex(
                name: "IX_Cer_producto_cliente_id_consumible",
                table: "Cer_producto_cliente",
                column: "id_consumible");

            migrationBuilder.CreateIndex(
                name: "IX_Cer_producto_cliente_id_producto",
                table: "Cer_producto_cliente",
                column: "id_producto");

            migrationBuilder.AddForeignKey(
                name: "FK_Cer_producto_cliente_Cer_accesorios_Cer_accesoriosid",
                table: "Cer_producto_cliente",
                column: "Cer_accesoriosid",
                principalTable: "Cer_accesorios",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Consumible_Mantenimiento",
                table: "Cer_producto_cliente",
                column: "id_consumible",
                principalTable: "Cer_consumibles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Producto_Mantenimiento",
                table: "Cer_producto_cliente",
                column: "id_producto",
                principalTable: "Cat_Productos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_Consumible_sublinea",
                table: "rel_consumible_sublinea",
                column: "id_consumible",
                principalTable: "Cer_consumibles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
