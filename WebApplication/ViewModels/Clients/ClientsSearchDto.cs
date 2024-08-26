using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.ViewModels
{
    public class ClientsSearchDto
    {
        public string text { get; set; }
        public int Id_sucursal { get; set; }
        public long id_account { get; set; }
        public int id_channel { get; set; }
    }
}
