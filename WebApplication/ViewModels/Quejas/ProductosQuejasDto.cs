using Newtonsoft.Json;

namespace WebApplication.ViewModels
{
    public class ProductosQuejasDto
    {
        public int ProductoId { get; set; }
        [JsonIgnore]
        public int QuejaId { get; set; }
    }
}
