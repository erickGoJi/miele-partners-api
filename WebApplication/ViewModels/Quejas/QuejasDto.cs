using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace WebApplication.ViewModels
{
    public class QuejasDto
    {
        public QuejasDto()
        {
            ProductosQuejas = new List<ProductosQuejasDto>();
            Propuestas = new List<PropuestasDto>();
        }

        [JsonIgnore]
        public int Id { get; set; }
        [JsonIgnore]
        public string Folio { get; set; }
        public long ClienteId { get; set; }
        public string Telefono { get; set; }
        [JsonIgnore]
        public DateTime Fecha { get; set; }
        public int TipoQuejaId { get; set; }
        public string Atendio { get; set; }
        public string DetalleReclamo { get; set; }
        public int CanalId { get; set; }
        [JsonIgnore]
        public bool Estatus { get; set; }

        public List<ProductosQuejasDto> ProductosQuejas { get; set; }
        public List<PropuestasDto> Propuestas { get; set; }
    }
}
