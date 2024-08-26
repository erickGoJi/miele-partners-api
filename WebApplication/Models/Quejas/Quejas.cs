using System;
using System.Collections.Generic;

namespace WebApplication.Models
{
    public class Quejas
    {
        public Quejas()
        {
            ProductosQuejas = new List<ProductosQuejas>();
            Propuestas = new List<Propuestas>();
        }

        public int Id { get; set; }
        public string Folio { get; set; }
        public long ClienteId { get; set; }
        public string Telefono { get; set; }
        public DateTime Fecha { get; set; }
        public int TipoQuejaId { get; set; }
        public string Atendio { get; set; }
        public string DetalleReclamo { get; set; }
        public int CanalId { get; set; }
        public bool Estatus { get; set; }

        public Canales Canal { get; set; }
        public Clientes Cliente { get; set; }
        public TipoQueja TipoQueja { get; set; }
        public IList<ProductosQuejas> ProductosQuejas { get; set; }
        public IList<Propuestas> Propuestas { get; set; }
        //public List<quejas_servicios> quejas_servicios { get; set; }
    }
}
