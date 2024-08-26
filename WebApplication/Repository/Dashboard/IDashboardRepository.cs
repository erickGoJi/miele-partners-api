using WebApplication.Models.Dashboard;
using WebApplication.Repositoriy.Generic;

namespace WebApplication.Repository.Dashboard
{
    public interface IDashboardRepository
    {
        Promedio PromedioDiasRespuesta(
            bool? Garantia,
            string PeriodoTiempo,
            int? TipoServicio,
            int? SubTipoServicio,
            int? TecnicoId,
            int? TipoTecnico,
            int? SubTipoTecnico
            );

        Porcentaje PorcentajeDiasRespuestaMenorIgual(
            bool? Garantia,
            int? TipoServicio,
            int? SubTipoServicio,
            int? TecnicoId,
            int? TipoTecnico,
            int? SubTipoTecnico,
            int? dia
            );

        Porcentaje PorcentajeDiasRespuestaMayor(
            bool? Garantia,
            int? TipoServicio,
            int? SubTipoServicio,
            int? TecnicoId,
            int? TipoTecnico,
            int? SubTipoTecnico,
            int? dia
            );

        Promedio PromedioDiasSolucion(
            bool? Garantia,
            string PeriodoTiempo,
            int? TipoServicio,
            int? SubTipoServicio,
            int? TecnicoId,
            int? TipoTecnico,
            int? SubTipoTecnico
            );

        Porcentaje PorcentajeServiciosResultosPrimeraVisita(
            bool? Garantia,
            string PeriodoTiempo,
            int? TipoServicio,
            int? SubTipoServicio,
            int? TecnicoId,
            int? TipoTecnico,
            int? SubTipoTecnico
            );

        Promedio CantidadCasos(
            bool? Garantia,
            string PeriodoTiempo,
            int? TipoServicio,
            int? SubTipoServicio,
            int? TecnicoId,
            int? TipoTecnico,
            int? SubTipoTecnico,
            int? ProductoId
            );

        Promedio CantidadCasosIncumplimiento(
            bool? Garantia,
            int? TipoServicio,
            int? SubTipoServicio,
            int? TecnicoId,
            int? TipoTecnico,
            int? SubTipoTecnico
            );

        Promedio CantidadCasosPendientes(
            bool? Garantia,
            string PeriodoTiempo,
            int? TipoServicio,
            int? SubTipoServicio,
            int? TecnicoId,
            int? TipoTecnico,
            int? SubTipoTecnico
            );

        Porcentaje PorcentajeServicioQuejas(int? productoId);

        Promedio PromedioTiempoReaccionQuejas(int? productoId);

        Promedio PromedioTiempoSolucionQuejas(int? productoId);

        Porcentaje PorcentajePrimeraInstalacion(
            bool? Garantia,
            string PeriodoTiempo,
            int? TipoServicio,
            int? SubTipoServicio,
            int? TecnicoId,
            int? TipoTecnico,
            int? SubTipoTecnico
            );


        #region Graficas
        Resumen ResumenDashboard(
        bool? Garantia,
        string PeriodoTiempo,
        int? TipoServicio,
        int? SubTipoServicio,
        int? TecnicoId,
        int? TipoTecnico,
        int? SubTipoTecnico
    );

        Grafica GraficaCasosPendientes(
            bool? Garantia,
            string PeriodoTiempo,
            int? TipoServicio,
            int? SubTipoServicio,
            int? TecnicoId,
            int? TipoTecnico,
            int? SubTipoTecnico
         );

        GraficaVisita GraficaReparacionVisita(
            bool? Garantia,
            string PeriodoTiempo,
            int? TipoServicio,
            int? SubTipoServicio,
            int? TecnicoId,
            int? TipoTecnico,
            int? SubTipoTecnico
         );

        Grafica GraficaTiempoRespuesta(
            bool? Garantia,
            string PeriodoTiempo,
            int? TipoServicio,
            int? SubTipoServicio,
            int? TecnicoId,
            int? TipoTecnico,
            int? SubTipoTecnico
         );

        Grafica GraficaTiempoSolucion(
            bool? Garantia,
            string PeriodoTiempo,
            int? TipoServicio,
            int? SubTipoServicio,
            int? TecnicoId,
            int? TipoTecnico,
            int? SubTipoTecnico
         );


        #endregion

    }
}
