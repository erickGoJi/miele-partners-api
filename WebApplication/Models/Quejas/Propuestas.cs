using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication.Models
{
    public class Propuestas
    {
        public int Id { get; set; }
        public int QuejaId { get; set; }
        public string Solucion { get; set; }
        public DateTime Fecha { get; set; }
        public string DetalleCierre { get; set; }
        public DateTime? FechaCierre { get; set; }
        public bool Email { get; set; }

        public Quejas Queja { get; set; }
    }
}
