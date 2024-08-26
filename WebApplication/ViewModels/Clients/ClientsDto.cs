using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WebApplication.ViewModels
{
    public class ClientsDto
    {
        [JsonIgnore]
        public long id { get; set; }
        [Required]
        public string nombre { get; set; }
        [Required]
        public string paterno { get; set; }
        public string materno { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string email { get; set; }
        public string telefono { get; set; }
        public string telefono_movil { get; set; }
        [Required]
        public long? referidopor { get; set; }
        [Required]
        public long? creadopor { get; set; }
    }
}
