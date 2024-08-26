using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace WebApplication.Migrations
{
    public partial class add_SP_addprodsautomaticos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sp = @"CREATE PROCEDURE sp_add_prods_aut_cotizacion -- Get_prom_avalibles_by_quoteid  13
                         @cotizacion_id int
                        AS
                        BEGIN
                            select * from Cotizacion_Producto
                        END";

            migrationBuilder.Sql(sp);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
