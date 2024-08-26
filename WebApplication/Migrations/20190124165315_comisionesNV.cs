using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication.Migrations
{
    public partial class comisionesNV : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_com_afectacion_ccs_condiones_comerciales_sucursal_condiones_comerciales_sucursalid",
                table: "com_afectacion_ccs");

            migrationBuilder.DropForeignKey(
                name: "FK_com_entidades_participantes_cat_tipo_entidades_cat_tipo_entidadesid",
                table: "com_entidades_participantes");

            migrationBuilder.DropIndex(
                name: "IX_com_entidades_participantes_cat_tipo_entidadesid",
                table: "com_entidades_participantes");

            migrationBuilder.DropIndex(
                name: "IX_com_afectacion_ccs_condiones_comerciales_sucursalid",
                table: "com_afectacion_ccs");

            migrationBuilder.DropColumn(
                name: "cat_tipo_entidadesid",
                table: "com_entidades_participantes");

            migrationBuilder.DropColumn(
                name: "condiones_comerciales_sucursalid",
                table: "com_afectacion_ccs");

            migrationBuilder.CreateIndex(
                name: "IX_com_entidades_participantes_id_tipo_entidad",
                table: "com_entidades_participantes",
                column: "id_tipo_entidad");

            migrationBuilder.CreateIndex(
                name: "IX_com_afectacion_ccs_id_condiones_comerciales_sucursal",
                table: "com_afectacion_ccs",
                column: "id_condiones_comerciales_sucursal");

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_comafectacioncc_condiones_comerciales_sucursal2",
                table: "com_afectacion_ccs",
                column: "id_condiones_comerciales_sucursal",
                principalTable: "condiones_comerciales_sucursal",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_comentidadesparticipantes_comcattipoentidades2",
                table: "com_entidades_participantes",
                column: "id_tipo_entidad",
                principalTable: "cat_tipo_entidades",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_comafectacioncc_condiones_comerciales_sucursal2",
                table: "com_afectacion_ccs");

            migrationBuilder.DropForeignKey(
                name: "ForeignKey_comentidadesparticipantes_comcattipoentidades2",
                table: "com_entidades_participantes");

            migrationBuilder.DropIndex(
                name: "IX_com_entidades_participantes_id_tipo_entidad",
                table: "com_entidades_participantes");

            migrationBuilder.DropIndex(
                name: "IX_com_afectacion_ccs_id_condiones_comerciales_sucursal",
                table: "com_afectacion_ccs");

            migrationBuilder.AddColumn<int>(
                name: "cat_tipo_entidadesid",
                table: "com_entidades_participantes",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "condiones_comerciales_sucursalid",
                table: "com_afectacion_ccs",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_com_entidades_participantes_cat_tipo_entidadesid",
                table: "com_entidades_participantes",
                column: "cat_tipo_entidadesid");

            migrationBuilder.CreateIndex(
                name: "IX_com_afectacion_ccs_condiones_comerciales_sucursalid",
                table: "com_afectacion_ccs",
                column: "condiones_comerciales_sucursalid");

            migrationBuilder.AddForeignKey(
                name: "FK_com_afectacion_ccs_condiones_comerciales_sucursal_condiones_comerciales_sucursalid",
                table: "com_afectacion_ccs",
                column: "condiones_comerciales_sucursalid",
                principalTable: "condiones_comerciales_sucursal",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_com_entidades_participantes_cat_tipo_entidades_cat_tipo_entidadesid",
                table: "com_entidades_participantes",
                column: "cat_tipo_entidadesid",
                principalTable: "cat_tipo_entidades",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
