using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplication.Models;

namespace WebApplication.Controllers
{

    [Produces("application/json")]
    [Route("api/Notificaciones")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class NotificacionesController : Controller
    {
        private IConfiguration _config;
        private readonly MieleContext _context;

        public NotificacionesController(IConfiguration config, MieleContext context)
        {
            _config = config;
            _context = context;
        }

        // GET: api/Notificaciones
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Notificaciones/5
        [HttpGet("{id}", Name = "Get_notificaciones")]
        public IActionResult Get(int id)
        {
            return new ObjectResult(_context.Notificaciones.Where(x => x.id == id).ToList());
        }

        // POST: api/Notificaciones
        [HttpPost]
        public IActionResult Post([FromBody] Notificaciones value)
        {
            if (value == null)
            {
                return BadRequest();
            }
            else
            {
                _context.Notificaciones.Add(value);
                _context.SaveChanges();
            }

            return new ObjectResult(Ok(new { response = "OK" }));
        }

        // POST: api/Notificaciones/notificacion_usuario
        [Route("notificacion_usuario")]
        [HttpPost("{notificacion_usuario}")]
        public IActionResult notificacion_usuario([FromBody] tecnicobyid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var diff = EF.Functions.DateDiffDay(request[i].creado, request[i].fecha_inicio_visita.Value.Date);
            var item = (from a in _context.Notificaciones
                        where a.rol_notificado == id.id && a.estatus_leido == false
                        orderby a.creado descending
                        select new
                        {
                            a.id,
                            a.descripcion,
                            a.estatus_leido,
                            a.evento,
                            a.rol_notificado,
                            fecha = a.creado.ToString("yyyyMMdd"),
                            no_notificaciones = (_context.Notificaciones.Where(x => x.rol_notificado == id.id).Count()),
                            a.url,
                            hace_fecha =  EF.Functions.DateDiffHour(a.creado, DateTime.Now) <= 1 ? "Hace un momento" :
                                          EF.Functions.DateDiffHour(a.creado, DateTime.Now) > 1 && EF.Functions.DateDiffHour(a.creado, DateTime.Now)  <= 24 ? "Hace " + EF.Functions.DateDiffHour(a.creado, DateTime.Now) + "horas":
                                          EF.Functions.DateDiffDay(a.creado, DateTime.Now) > 1 ? "Hace " + EF.Functions.DateDiffDay(a.creado, DateTime.Now) + " días" : ""
                        }).ToList();

            return new ObjectResult(item);
        }

        // POST: api/Notificaciones/notificacion_usuario
        [Route("notificacion_usuario_total")]
        [HttpPost("{notificacion_usuario_total}")]
        public IActionResult notificacion_usuario_total([FromBody] tecnicobyid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = (from a in _context.Notificaciones
                        where a.rol_notificado == id.id && a.estatus_leido == false
                        select a.id).Count();



            return new ObjectResult(item);
        }

        // POST: api/Notificaciones/actualizar_estatus_notificacion
        [Route("actualizar_estatus_notificacion")]
        [HttpPost("{actualizar_estatus_notificacion}")]
        public IActionResult actualizar_estatus_notificacion([FromBody] tecnicobyid id)
        {
            var result = new Models.Response();
            var notificacion = _context.Notificaciones.FirstOrDefault(t => t.id == id.id);

            if (notificacion == null)
            {
                return NotFound();
            }
            else
            {
                notificacion.estatus_leido = true;
                _context.Notificaciones.Update(notificacion);
                _context.SaveChanges();

                result = new Models.Response
                {
                    response = "OK"
                };
            }


            return new ObjectResult(result);
        }

        // PUT: api/Notificaciones/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        public class tecnicobyid
        {
            public int id { get; set; }
        }
    }
}
