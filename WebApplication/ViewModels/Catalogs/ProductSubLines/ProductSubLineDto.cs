using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.ViewModels
{
    public class ProductSubLineDto
    {
        public ProductSubLineDto()
        {
            rel_categoria = new List<ProductSubLineTypeDto>();
        }

        public int id { get; set; }
        public int id_linea_producto { get; set; }
        public string descripcion { get; set; }
        public bool estatus { get; set; }

        public IList<ProductSubLineTypeDto> rel_categoria { get; set; }
    }
}
