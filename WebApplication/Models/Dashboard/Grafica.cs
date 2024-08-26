namespace WebApplication.Models.Dashboard
{
    public class Grafica
    {
        public string Rango { get; set; }
        public int? Total { get; set; }

        public Grafica()
        {
            Rango = null;
            Total = null;
        }
    }
}
