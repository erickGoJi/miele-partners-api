using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class spget_promociones : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE PROCEDURE Get_promos_disponibles_cotizacionid -- Get_prom_avalibles_by_quoteid  13
                         @promo_id int
                        AS
                        BEGIN
                         select 
		                        id,
		                        beneficio_obligatorio,
		                        fecha_hora_fin	,
		                        fecha_hora_inicio	,
		                        id_cat_tipo_condicion,
		                        id_tipo_beneficio,
		                        id_tipos_herencia_promo,
		                        incluir_desc_adic,
		                        monto_condicion,
		                        nombre,
		                        vigencia_indefinida
                        from 
                              promocion 
                        where id = @promo_id

                        END";

            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
