using Newtonsoft.Json;

namespace WebApplication.ViewModels
{
    public class ProductLinesDto
    {
        [JsonIgnore]
        public int id { get; set; }
        public int id_categoria { get; set; }
        public string descripcion { get; set; }
        public bool estatus { get; set; }

    }
}
