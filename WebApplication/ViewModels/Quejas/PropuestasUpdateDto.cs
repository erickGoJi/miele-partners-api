using Newtonsoft.Json;

namespace WebApplication.ViewModels
{
    public class PropuestasUpdateDto
    {
        public int Id { get; set; }
        [JsonIgnore]
        public int QuejaId { get; set; }
        public string Solucion { get; set; }
        public string DetalleCierre { get; set; }
    }
}
