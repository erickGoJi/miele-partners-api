using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using WebApplication.Models;
using WebApplication.Models.Dashboard;

namespace WebApplication.Repository.Dashboard
{
    public class DashboardRepository : IDashboardRepository
    {
        protected MieleContext _context;

        public DashboardRepository(MieleContext context)
        {
            _context = context;
        }

        #region DiasRespuesta
        public Promedio PromedioDiasRespuesta(bool? Garantia, string PeriodoTiempo, int? TipoServicio, int? SubTipoServicio, int? TecnicoId, int? TipoTecnico, int? SubTipoTecnico)
        {
            return _context.Query<Promedio>().FromSql("EXEC PromedioDiasRespuesta @Garantia, @PeriodoTiempo, @TipoServicio, @SubTipoServicio, @TecnicoId, @TipoTecnico, @SubTipoTecnico",
                new[] {
                new SqlParameter("@Garantia", (object)Garantia ?? DBNull.Value),
                new SqlParameter("@PeriodoTiempo", (object)PeriodoTiempo ?? DBNull.Value),
                new SqlParameter("@TipoServicio", (object)TipoServicio ?? DBNull.Value),
                new SqlParameter("@SubTipoServicio", (object)SubTipoServicio ?? DBNull.Value),
                new SqlParameter("@TecnicoId", (object)TecnicoId ?? DBNull.Value),
                new SqlParameter("@TipoTecnico", (object)TipoTecnico ?? DBNull.Value),
                new SqlParameter("@SubTipoTecnico", (object)SubTipoTecnico ?? DBNull.Value)
                }).FirstOrDefault();
        }

        public Porcentaje PorcentajeDiasRespuestaMenorIgual(bool? Garantia, int? TipoServicio, int? SubTipoServicio, int? TecnicoId, int? TipoTecnico, int? SubTipoTecnico, int? dia)
        {
            return _context.Query<Porcentaje>().FromSql("EXEC PorcentajeDiasRespuestaMenorIgual @Garantia, @TipoServicio, @SubTipoServicio, @TecnicoId, @TipoTecnico, @SubTipoTecnico, @dia",
                new[] {
                new SqlParameter("@Garantia", (object)Garantia ?? DBNull.Value),
                new SqlParameter("@TipoServicio", (object)TipoServicio ?? DBNull.Value),
                new SqlParameter("@SubTipoServicio", (object)SubTipoServicio ?? DBNull.Value),
                new SqlParameter("@TecnicoId", (object)TecnicoId ?? DBNull.Value),
                new SqlParameter("@TipoTecnico", (object)TipoTecnico ?? DBNull.Value),
                new SqlParameter("@SubTipoTecnico", (object)SubTipoTecnico ?? DBNull.Value),
                new SqlParameter("@Dia", (object)dia ?? DBNull.Value)
                }).FirstOrDefault();
        }

        public Porcentaje PorcentajeDiasRespuestaMayor(bool? Garantia, int? TipoServicio, int? SubTipoServicio, int? TecnicoId, int? TipoTecnico, int? SubTipoTecnico, int? dia)
        {
            return _context.Query<Porcentaje>().FromSql("EXEC PorcentajeDiasRespuestaMayor @Garantia, @TipoServicio, @SubTipoServicio, @TecnicoId, @TipoTecnico, @SubTipoTecnico, @dia",
                new[] {
                new SqlParameter("@Garantia", (object)Garantia ?? DBNull.Value),
                new SqlParameter("@TipoServicio", (object)TipoServicio ?? DBNull.Value),
                new SqlParameter("@SubTipoServicio", (object)SubTipoServicio ?? DBNull.Value),
                new SqlParameter("@TecnicoId", (object)TecnicoId ?? DBNull.Value),
                new SqlParameter("@TipoTecnico", (object)TipoTecnico ?? DBNull.Value),
                new SqlParameter("@SubTipoTecnico", (object)SubTipoTecnico ?? DBNull.Value),
                new SqlParameter("@Dia", (object)dia ?? DBNull.Value)
                }).FirstOrDefault();


        } 
        #endregion

        public Promedio PromedioDiasSolucion(bool? Garantia, string PeriodoTiempo, int? TipoServicio, int? SubTipoServicio, int? TecnicoId, int? TipoTecnico, int? SubTipoTecnico)
        {
            return _context.Query<Promedio>().FromSql("EXEC PromedioDiasSolucion @Garantia, @PeriodoTiempo, @TipoServicio, @SubTipoServicio, @TecnicoId, @TipoTecnico, @SubTipoTecnico",
                new[] {
                new SqlParameter("@Garantia", (object)Garantia ?? DBNull.Value),
                new SqlParameter("@PeriodoTiempo", (object)PeriodoTiempo ?? DBNull.Value),
                new SqlParameter("@TipoServicio", (object)TipoServicio ?? DBNull.Value),
                new SqlParameter("@SubTipoServicio", (object)SubTipoServicio ?? DBNull.Value),
                new SqlParameter("@TecnicoId", (object)TecnicoId ?? DBNull.Value),
                new SqlParameter("@TipoTecnico", (object)TipoTecnico ?? DBNull.Value),
                new SqlParameter("@SubTipoTecnico", (object)SubTipoTecnico ?? DBNull.Value)
                }).FirstOrDefault();
        }

        public Porcentaje PorcentajeServiciosResultosPrimeraVisita(bool? Garantia, string PeriodoTiempo, int? TipoServicio, int? SubTipoServicio, int? TecnicoId, int? TipoTecnico, int? SubTipoTecnico)
        {
            return _context.Query<Porcentaje>().FromSql("EXEC PorcentajeServiciosResultosPrimeraVisita @Garantia, @PeriodoTiempo, @TipoServicio, @SubTipoServicio, @TecnicoId, @TipoTecnico, @SubTipoTecnico",
                new[] {
                new SqlParameter("@Garantia", (object)Garantia ?? DBNull.Value),
                new SqlParameter("@PeriodoTiempo", (object)PeriodoTiempo ?? DBNull.Value),
                new SqlParameter("@TipoServicio", (object)TipoServicio ?? DBNull.Value),
                new SqlParameter("@SubTipoServicio", (object)SubTipoServicio ?? DBNull.Value),
                new SqlParameter("@TecnicoId", (object)TecnicoId ?? DBNull.Value),
                new SqlParameter("@TipoTecnico", (object)TipoTecnico ?? DBNull.Value),
                new SqlParameter("@SubTipoTecnico", (object)SubTipoTecnico ?? DBNull.Value)
                }).FirstOrDefault();


        }

        public Promedio CantidadCasos(bool? Garantia, string PeriodoTiempo, int? TipoServicio, int? SubTipoServicio, int? TecnicoId, int? TipoTecnico, int? SubTipoTecnico, int? productoId)
        {
            return _context.Query<Promedio>().FromSql("EXEC CantidadCasos @Garantia, @PeriodoTiempo, @TipoServicio, @SubTipoServicio, @TecnicoId, @TipoTecnico, @SubTipoTecnico, @ProductoId",
                new[] {
                new SqlParameter("@Garantia", (object)Garantia ?? DBNull.Value),
                new SqlParameter("@PeriodoTiempo", (object)PeriodoTiempo ?? DBNull.Value),
                new SqlParameter("@TipoServicio", (object)TipoServicio ?? DBNull.Value),
                new SqlParameter("@SubTipoServicio", (object)SubTipoServicio ?? DBNull.Value),
                new SqlParameter("@TecnicoId", (object)TecnicoId ?? DBNull.Value),
                new SqlParameter("@TipoTecnico", (object)TipoTecnico ?? DBNull.Value),
                new SqlParameter("@SubTipoTecnico", (object)SubTipoTecnico ?? DBNull.Value),
                new SqlParameter("@ProductoId", (object)productoId ?? DBNull.Value)
                }).FirstOrDefault();
        }

        public Promedio CantidadCasosIncumplimiento(bool? Garantia, int? TipoServicio, int? SubTipoServicio, int? TecnicoId, int? TipoTecnico, int? SubTipoTecnico)
        {
            return _context.Query<Promedio>().FromSql("EXEC CantidadCasosIncumplimiento @Garantia, @TipoServicio, @SubTipoServicio, @TecnicoId, @TipoTecnico, @SubTipoTecnico",
                new[] {
                new SqlParameter("@Garantia", (object)Garantia ?? DBNull.Value),
                new SqlParameter("@TipoServicio", (object)TipoServicio ?? DBNull.Value),
                new SqlParameter("@SubTipoServicio", (object)SubTipoServicio ?? DBNull.Value),
                new SqlParameter("@TecnicoId", (object)TecnicoId ?? DBNull.Value),
                new SqlParameter("@TipoTecnico", (object)TipoTecnico ?? DBNull.Value),
                new SqlParameter("@SubTipoTecnico", (object)SubTipoTecnico ?? DBNull.Value)
                }).FirstOrDefault();
        }

        public Promedio CantidadCasosPendientes(bool? Garantia, string PeriodoTiempo, int? TipoServicio, int? SubTipoServicio, int? TecnicoId, int? TipoTecnico, int? SubTipoTecnico)
        {
            return _context.Query<Promedio>().FromSql("EXEC CantidadCasosPendientes @Garantia, @PeriodoTiempo, @TipoServicio, @SubTipoServicio, @TecnicoId, @TipoTecnico, @SubTipoTecnico",
                new[] {
                new SqlParameter("@Garantia", (object)Garantia ?? DBNull.Value),
                new SqlParameter("@PeriodoTiempo", (object)PeriodoTiempo ?? DBNull.Value),
                new SqlParameter("@TipoServicio", (object)TipoServicio ?? DBNull.Value),
                new SqlParameter("@SubTipoServicio", (object)SubTipoServicio ?? DBNull.Value),
                new SqlParameter("@TecnicoId", (object)TecnicoId ?? DBNull.Value),
                new SqlParameter("@TipoTecnico", (object)TipoTecnico ?? DBNull.Value),
                new SqlParameter("@SubTipoTecnico", (object)SubTipoTecnico ?? DBNull.Value)
                }).FirstOrDefault();
        }

        public Porcentaje PorcentajeServicioQuejas(int? productoId)
        {
            return _context.Query<Porcentaje>().FromSql("EXEC PorcentajeServicioQuejas @productoId",
                new[] {
                new SqlParameter("@ProductoId", (object)productoId ?? DBNull.Value)
                }).FirstOrDefault();
        }

        public Promedio PromedioTiempoReaccionQuejas(int? productoId)
        {
            return _context.Query<Promedio>().FromSql("EXEC PromedioTiempoReaccionQuejas @productoId",
                new[] {
                new SqlParameter("@ProductoId", (object)productoId ?? DBNull.Value)
                }).FirstOrDefault();
        }

        public Promedio PromedioTiempoSolucionQuejas(int? productoId)
        {
            return _context.Query<Promedio>().FromSql("EXEC PromedioTiempoSolucionQuejas @productoId",
                new[] {
                new SqlParameter("@ProductoId", (object)productoId ?? DBNull.Value)
                }).FirstOrDefault();
        }

        public Porcentaje PorcentajePrimeraInstalacion(bool? Garantia, string PeriodoTiempo, int? TipoServicio, int? SubTipoServicio, int? TecnicoId, int? TipoTecnico, int? SubTipoTecnico)
        {
            return _context.Query<Porcentaje>().FromSql("EXEC PorcentajePrimeraInstalacion @Garantia, @PeriodoTiempo, @TipoServicio, @SubTipoServicio, @TecnicoId, @TipoTecnico, @SubTipoTecnico",
                new[] {
                new SqlParameter("@Garantia", (object)Garantia ?? DBNull.Value),
                new SqlParameter("@PeriodoTiempo", (object)PeriodoTiempo ?? DBNull.Value),
                new SqlParameter("@TipoServicio", (object)TipoServicio ?? DBNull.Value),
                new SqlParameter("@SubTipoServicio", (object)SubTipoServicio ?? DBNull.Value),
                new SqlParameter("@TecnicoId", (object)TecnicoId ?? DBNull.Value),
                new SqlParameter("@TipoTecnico", (object)TipoTecnico ?? DBNull.Value),
                new SqlParameter("@SubTipoTecnico", (object)SubTipoTecnico ?? DBNull.Value)
                }).FirstOrDefault();
        }

        #region Graficas
        public Resumen ResumenDashboard(bool? Garantia, string PeriodoTiempo, int? TipoServicio, int? SubTipoServicio, int? TecnicoId, int? TipoTecnico, int? SubTipoTecnico)
        {
            return _context.Query<Resumen>().FromSql("EXEC ResumenDashboard @Garantia, @PeriodoTiempo, @TipoServicio, @SubTipoServicio, @TecnicoId, @TipoTecnico, @SubTipoTecnico",
                new[] {
                new SqlParameter("@Garantia", (object)Garantia ?? DBNull.Value),
                new SqlParameter("@PeriodoTiempo", (object)PeriodoTiempo ?? DBNull.Value),
                new SqlParameter("@TipoServicio", (object)TipoServicio ?? DBNull.Value),
                new SqlParameter("@SubTipoServicio", (object)SubTipoServicio ?? DBNull.Value),
                new SqlParameter("@TecnicoId", (object)TecnicoId ?? DBNull.Value),
                new SqlParameter("@TipoTecnico", (object)TipoTecnico ?? DBNull.Value),
                new SqlParameter("@SubTipoTecnico", (object)SubTipoTecnico ?? DBNull.Value)
                }).FirstOrDefault() ?? new Resumen();
        }

        public Grafica GraficaCasosPendientes(bool? Garantia, string PeriodoTiempo, int? TipoServicio, int? SubTipoServicio, int? TecnicoId, int? TipoTecnico, int? SubTipoTecnico)
        {
            return _context.Query<Grafica>().FromSql("EXEC GraficaCasosPendientes @Garantia, @PeriodoTiempo, @TipoServicio, @SubTipoServicio, @TecnicoId, @TipoTecnico, @SubTipoTecnico",
                new[] {
                new SqlParameter("@Garantia", (object)Garantia ?? DBNull.Value),
                new SqlParameter("@PeriodoTiempo", (object)PeriodoTiempo ?? DBNull.Value),
                new SqlParameter("@TipoServicio", (object)TipoServicio ?? DBNull.Value),
                new SqlParameter("@SubTipoServicio", (object)SubTipoServicio ?? DBNull.Value),
                new SqlParameter("@TecnicoId", (object)TecnicoId ?? DBNull.Value),
                new SqlParameter("@TipoTecnico", (object)TipoTecnico ?? DBNull.Value),
                new SqlParameter("@SubTipoTecnico", (object)SubTipoTecnico ?? DBNull.Value)
                }).FirstOrDefault() ?? new Grafica();
        }

        public GraficaVisita GraficaReparacionVisita(bool? Garantia, string PeriodoTiempo, int? TipoServicio, int? SubTipoServicio, int? TecnicoId, int? TipoTecnico, int? SubTipoTecnico)
        {
            return _context.Query<GraficaVisita>().FromSql("EXEC GraficaReparacionVisita @Garantia, @PeriodoTiempo, @TipoServicio, @SubTipoServicio, @TecnicoId, @TipoTecnico, @SubTipoTecnico",
                new[] {
                new SqlParameter("@Garantia", (object)Garantia ?? DBNull.Value),
                new SqlParameter("@PeriodoTiempo", (object)PeriodoTiempo ?? DBNull.Value),
                new SqlParameter("@TipoServicio", (object)TipoServicio ?? DBNull.Value),
                new SqlParameter("@SubTipoServicio", (object)SubTipoServicio ?? DBNull.Value),
                new SqlParameter("@TecnicoId", (object)TecnicoId ?? DBNull.Value),
                new SqlParameter("@TipoTecnico", (object)TipoTecnico ?? DBNull.Value),
                new SqlParameter("@SubTipoTecnico", (object)SubTipoTecnico ?? DBNull.Value)
                }).FirstOrDefault() ?? new GraficaVisita();
        }

        public Grafica GraficaTiempoRespuesta(bool? Garantia, string PeriodoTiempo, int? TipoServicio, int? SubTipoServicio, int? TecnicoId, int? TipoTecnico, int? SubTipoTecnico)
        {
            return _context.Query<Grafica>().FromSql("EXEC GraficaTiempoRespuesta @Garantia, @PeriodoTiempo, @TipoServicio, @SubTipoServicio, @TecnicoId, @TipoTecnico, @SubTipoTecnico",
                new[] {
                new SqlParameter("@Garantia", (object)Garantia ?? DBNull.Value),
                new SqlParameter("@PeriodoTiempo", (object)PeriodoTiempo ?? DBNull.Value),
                new SqlParameter("@TipoServicio", (object)TipoServicio ?? DBNull.Value),
                new SqlParameter("@SubTipoServicio", (object)SubTipoServicio ?? DBNull.Value),
                new SqlParameter("@TecnicoId", (object)TecnicoId ?? DBNull.Value),
                new SqlParameter("@TipoTecnico", (object)TipoTecnico ?? DBNull.Value),
                new SqlParameter("@SubTipoTecnico", (object)SubTipoTecnico ?? DBNull.Value)
                }).FirstOrDefault() ?? new Grafica();
        }

        public Grafica GraficaTiempoSolucion(bool? Garantia, string PeriodoTiempo, int? TipoServicio, int? SubTipoServicio, int? TecnicoId, int? TipoTecnico, int? SubTipoTecnico)
        {
            return _context.Query<Grafica>().FromSql("EXEC GraficaTiempoSolucion @Garantia, @PeriodoTiempo, @TipoServicio, @SubTipoServicio, @TecnicoId, @TipoTecnico, @SubTipoTecnico",
                new[] {
                new SqlParameter("@Garantia", (object)Garantia ?? DBNull.Value),
                new SqlParameter("@PeriodoTiempo", (object)PeriodoTiempo ?? DBNull.Value),
                new SqlParameter("@TipoServicio", (object)TipoServicio ?? DBNull.Value),
                new SqlParameter("@SubTipoServicio", (object)SubTipoServicio ?? DBNull.Value),
                new SqlParameter("@TecnicoId", (object)TecnicoId ?? DBNull.Value),
                new SqlParameter("@TipoTecnico", (object)TipoTecnico ?? DBNull.Value),
                new SqlParameter("@SubTipoTecnico", (object)SubTipoTecnico ?? DBNull.Value)
                }).FirstOrDefault() ?? new Grafica();
        } 
        #endregion
    }
}
