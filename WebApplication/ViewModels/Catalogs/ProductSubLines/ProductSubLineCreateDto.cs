using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class ProductSubLineCreateDto
    {

        public ProductSubLineCreateDto()
        {
            rel_categoria = new List<ProductSubLineTypeDto>();
        }
        [JsonIgnore]
        public int id { get; set; }
        public int id_linea_producto { get; set; }
        public string descripcion { get; set; }
        public bool estatus { get; set; }

        public IList<ProductSubLineTypeDto> rel_categoria { get; set; }
    }
}
