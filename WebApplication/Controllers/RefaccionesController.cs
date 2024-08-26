using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/Refacciones")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RefaccionesController : Controller
    {
        private IConfiguration _config;
        private readonly MieleContext _context;

        public RefaccionesController(IConfiguration config, MieleContext context)
        {
            _config = config;
            _context = context;
        }

        // GET: api/Refacciones
        [HttpGet]
        public IActionResult GetAll()
        {
            var item = (from a in _context.Cat_Materiales
                        join b in _context.Cat_Lista_Precios on a.id_grupo_precio equals b.id
                        orderby a.id ascending
                        where a.id <50000
                        select new
                        {
                            id = a.id,
                            name = a.descripcion,
                            sku = a.no_material,
                            price_not_iva = b.precio_sin_iva,
                            price_group = b.grupo_precio
                        }).ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return new ObjectResult(item);
        }

        // GET: api/Refacciones/5
        [HttpGet("{id}", Name = "Get_Refacciones")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/Refacciones
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/Refacciones/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
