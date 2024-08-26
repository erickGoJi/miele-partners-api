using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class cat_servicios_sin_pago : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "servicio_sin_pago",
                table: "Servicio",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "monto_inferior_condicion",
                table: "promocion",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.AlterColumn<decimal>(
                name: "monto_condicion",
                table: "promocion",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.CreateTable(
                name: "Servicio_Sin_Pagos",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    nombre = table.Column<string>(nullable: true),
                    Estatus = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicio_Sin_Pagos", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Servicio_Sin_Pagos");

            migrationBuilder.DropColumn(
                name: "servicio_sin_pago",
                table: "Servicio");

            migrationBuilder.AlterColumn<int>(
                name: "monto_inferior_condicion",
                table: "promocion",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<int>(
                name: "monto_condicion",
                table: "promocion",
                nullable: false,
                oldClrType: typeof(decimal));
        }
    }
}
