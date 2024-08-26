using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.ViewModels
    {
    public class ProductSubLineTypeDto
    {
        public int id { get; set; }
        public int no_tecnicos { get; set; }
        public string horas_tecnicos { get; set; }
        public int? precio_visita { get; set; }
        public int? precio_hora_tecnico { get; set; }
        //public ProductSubLinesDto SubLinesDto { get; set; }
        public int id_categoria { get; set; }
        //public ServiceTypeDo Service_TypeDo { get; set; }
        public int id_tipo_servicio { get; set; }
        public bool estatus { get; set; }
    }
}
