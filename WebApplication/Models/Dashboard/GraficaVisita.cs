namespace WebApplication.Models.Dashboard
{
    public class GraficaVisita
    {
        public int? ReparacionPrimeraVisita { get; set; }
        public int? ReparacionTotal { get; set; }

        public GraficaVisita()
        {
            ReparacionPrimeraVisita = null;
            ReparacionTotal = null;
        }
    }
}
