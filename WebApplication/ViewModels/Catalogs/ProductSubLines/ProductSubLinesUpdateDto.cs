using System.Collections.Generic;

namespace WebApplication.ViewModels
{
    public class ProductSubLinesUpdateDto
    {
       
        public int id { get; set; }
        public int id_linea_producto { get; set; }
        public string descripcion { get; set; }
        public bool estatus { get; set; }

        
    }
}
