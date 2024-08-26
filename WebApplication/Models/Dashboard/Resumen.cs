namespace WebApplication.Models.Dashboard
{
    public class Resumen
    {
        public int? TotalPendientes { get; set; }
        public decimal? PorcentajeServiciosResultosPrimeraVisita { get; set; }
        public int? PromedioTiempoRespuesta { get; set; }
        public int? PromedioTiempoSolucion { get; set; }

        public Resumen()
        {
            TotalPendientes = null;
            PorcentajeServiciosResultosPrimeraVisita = null;
            PromedioTiempoRespuesta = null;
            PromedioTiempoSolucion = null;
        }
    }
}
