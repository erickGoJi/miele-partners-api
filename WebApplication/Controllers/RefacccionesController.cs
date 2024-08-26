using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/Refacciones")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class RefacccionesController : Controller
    {
        private IConfiguration _config;
        private readonly MieleContext _context;

        public RefacccionesController(IConfiguration config, MieleContext context)
        {
            _config = config;
            _context = context;
        }

        // GET: Refaccciones
        public ActionResult Index()
        {
            return View();
        }

        // GET: Refaccciones/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Refaccciones/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Refaccciones/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: api/Refacciones/Busqueda_refaccion
        [Route("Busqueda_refaccion")]
        [HttpPost("{Busqueda_refaccion}")]
        public IActionResult Busqueda_refaccion([FromBody] busqueda busqueda)
        {
            IActionResult response;

            try
            {
                if (busqueda == null)
                {
                    return NotFound();
                }
                else
                {
                    var item = (from a in _context.Cat_Materiales
                                join b in _context.Cat_Lista_Precios on a.id_grupo_precio equals b.id
                                where EF.Functions.Like(a.no_material + a.descripcion, "%" + busqueda.texto + "%")
                                orderby a.no_material descending
                                select new
                                {
                                    a.id,
                                    a.no_material,
                                    descripcion = a.no_material + " - " + a.descripcion,
                                    cantidad = a.cantidad == null ? 0 : a.cantidad,
                                    b.grupo_precio,
                                    b.precio_sin_iva
                                }).Distinct().ToList();

                    if (item.Count == 0)
                    {
                        return response = Ok(new { item = "No hay resultado para la busqueda" });
                    }
                    return new ObjectResult(item);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }

        [Route("filtro_refacciones")]
        [HttpGet("filtro_refacciones/{texto}")]
        public IActionResult filtro_refacciones(string texto)
        {
            IActionResult response;

            try
            {
                if (texto == null)
                {
                    texto = "110";
                }
                var item = (from a in _context.Cat_Materiales
                            join b in _context.Cat_Lista_Precios on a.id_grupo_precio equals b.id
                            where EF.Functions.Like(a.no_material + a.descripcion,  texto + "%")
                            orderby a.no_material descending
                            select new
                            {
                                a.id,
                                a.no_material,
                                descripcion = a.no_material + " - " + a.descripcion,
                                cantidad = a.cantidad == null ? 0 : a.cantidad,
                                b.grupo_precio,
                                b.precio_sin_iva
                            }).Distinct().ToList();

                if (item.Count == 0)
                {
                    return response = Ok(new { item = "No hay resultado para la busqueda" });
                }
                return new ObjectResult(item);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }

        // POST: api/Refacciones/Busqueda_refaccion_Precios
        [Route("Busqueda_refaccion_Precios")]
        [HttpPost("{Busqueda_refaccion_Precios}")]
        public IActionResult Busqueda_refaccion_Precios([FromBody] busqueda busqueda)
        {
            IActionResult response;

            try
            {
                if (busqueda == null)
                {
                    return NotFound();
                }
                else
                {
                    var item = (from a in _context.Cat_Lista_Precios
                                where EF.Functions.Like(a.grupo_precio + a.precio_sin_iva, "%" + busqueda.texto + "%")
                                orderby a.id ascending
                                select new
                                {
                                    a.id,
                                    //a.no_material,
                                    //descripcion = a.no_material + " - " + a.descripcion,
                                    //cantidad = a.cantidad == null ? 0 : a.cantidad,
                                    a.grupo_precio,
                                    a.precio_sin_iva
                                }).Distinct().ToList();

                    if (item.Count == 0)
                    {
                        return response = Ok(new { item = "No hay resultado para la busqueda" });
                    }
                    return new ObjectResult(item);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }

        // POST: api/Refacciones/Guardar_Lista_Precios
        [Route("Guardar_Lista_Precios")]
        [HttpPost("{Guardar_Lista_Precios}")]
        public IActionResult Guardar_Lista_Precios([FromBody] Cat_Lista_Precios lista)
        {
            var result = new Models.Response();

            var item = _context.Cat_Lista_Precios.FirstOrDefault(t => t.grupo_precio == lista.grupo_precio);
            if (item == null)
            {
                _context.Cat_Lista_Precios.Add(lista);
                _context.SaveChanges();
                result = new Models.Response
                {
                    response = "OK"
                };
            }
            else
            {
                item.precio_sin_iva = lista.precio_sin_iva;
                _context.Cat_Lista_Precios.Update(item);
                _context.SaveChanges();
                result = new Models.Response
                {
                    response = "OK"
                };
            }

            return new ObjectResult(result);
        }

        // POST: api/Refacciones/Guardar_Lista_Precios_Refaccion
        [Route("Guardar_Lista_Precios_Refaccion")]
        [HttpPost("{Guardar_Lista_Precios_Refaccion}")]
        public IActionResult Guardar_Lista_Precios_Refaccion([FromBody] Cat_Materiales lista)
        {
            var result = new Models.Response();

            var item = _context.Cat_Materiales.FirstOrDefault(t => t.id == lista.id);
            if (item == null)
            {
                _context.Cat_Materiales.Add(lista);
                _context.SaveChanges();
                result = new Models.Response
                {
                    response = "OK"
                };
            }
            else
            {
                item.id_grupo_precio = lista.id_grupo_precio;
                _context.Cat_Materiales.Update(item);
                _context.SaveChanges();
                result = new Models.Response
                {
                    response = "OK"
                };
            }

            return new ObjectResult(result);
        }


        // POST: api/Refacciones/Ver_Precio_Refaccion
        [Route("Ver_Precio_Refaccion")]
        [HttpPost("{Ver_Precio_Refaccion}")]
        public IActionResult Ver_Precio_Refaccion([FromBody] Cat_Materiales material)
        {
            IActionResult response;

            try
            {
                if (material == null)
                {
                    return NotFound();
                }
                else
                {
                    var item = (from a in _context.Cat_Materiales
                                join b in _context.Cat_Lista_Precios on a.id_grupo_precio equals b.id
                                where a.id == material.id
                                select new
                                {
                                    a.id,
                                    a.no_material,
                                    descripcion = a.no_material + " - " + a.descripcion,
                                    cantidad = a.cantidad == null ? 0 : a.cantidad,
                                    b.grupo_precio,
                                    b.precio_sin_iva
                                }).Distinct().ToList();

                    if (item.Count == 0)
                    {
                        return response = Ok(new { item = "No hay resultado para la busqueda" });
                    }
                    return new ObjectResult(item);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }

        // POST: api/Refacciones/Validar_Refaccion
        [Route("Validar_Refaccion")]
        [HttpPost("{Validar_Refaccion}")]
        public IActionResult Validar_Refaccion([FromBody] Cat_Materiales material)
        {
            IActionResult response;

            try
            {
                if (material == null)
                {
                    return NotFound();
                }
                else
                {
                    var item = (from a in _context.Cat_Materiales
                                where a.no_material == material.no_material
                                select new
                                {
                                    a.id,
                                    a.no_material,
                                    descripcion = a.no_material + " - " + a.descripcion,
                                    cantidad = a.cantidad == null ? 0 : a.cantidad
                                }).Distinct().ToList();

                    if (item.Count == 0)
                    {
                        return response = Ok(new { cantidad = item });
                    }
                    return new ObjectResult(item);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }

        // POST: api/Refacciones/Validar_Refaccion_Entrega
        [Route("Validar_Refaccion_Entrega")]
        [HttpPost("{Validar_Refaccion_Entrega}")]
        public IActionResult Validar_Refaccion_Entrega([FromBody] Cat_Materiales material)
        {
            IActionResult response;

            try
            {
                if (material == null)
                {
                    return NotFound();
                }
                else
                {
                    var item = (from a in _context.Cat_Materiales
                                where a.no_material == material.no_material
                                select new
                                {
                                    a.id,
                                    a.no_material,
                                    descripcion = a.no_material + " - " + a.descripcion,
                                    cantidad = a.cantidad == null ? 0 : a.cantidad
                                }).Distinct().ToList();

                    if (item[0].cantidad < material.cantidad)
                    {
                        return response = Ok(new { cantidad = item[0].descripcion });
                    }
                    else
                    {
                        return response = Ok(new { cantidad = "OK" });
                    }

                    // return new ObjectResult(item);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }

        // POST: api/Refacciones/Busqueda_refaccion
        [Route("Inventario_tecnico")]
        [HttpPost("{Inventario_tecnico}")]
        public IActionResult Inventario_tecnico([FromBody] Tecnicos tecnico)
        {
            IActionResult response;

            try
            {
                if (tecnico == null)
                {
                    return NotFound();
                }
                else
                {
                    var item = (from a in _context.Cat_Materiales_Tecnico
                                join b in _context.Cat_Materiales on a.id_material equals b.id
                                where EF.Functions.Like(b.no_material + ' ' + b.descripcion, "%" + tecnico.noalmacen + "%") && a.id_tecnico == tecnico.id && a.cantidad > 0
                                //where a.id_tecnico == tecnico.id
                                //orderby a.no_material descending
                                select new
                                {
                                    a.id,
                                    id_material = b.id,
                                    id_tecnico = a.id_tecnico,
                                    b.no_material,
                                    b.descripcion,
                                    a.cantidad
                                }).Distinct().ToList();

                    if (item.Count == 0)
                    {
                        return response = Ok(new { item });
                    }
                    return response = Ok(new { item });
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }

        // POST: api/Refacciones/No_material
        [Route("No_material")]
        [HttpPost("{No_material}")]
        public IActionResult No_material()
        {
            IActionResult response;

            try
            {
                var item = (from a in _context.Cat_Materiales
                            orderby a.no_material descending
                            select new
                            {
                                a.no_material
                            }).Take(1).ToList();

                if (item.Count == 0)
                {
                    return response = Ok(new { item = "No hay resultado para la busqueda" });
                }
                return new ObjectResult(item);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }

        // POST: api/Refacciones/Actualizar_refaccion
        [Route("Actualizar_refaccion")]
        [HttpPost("{Actualizar_refaccion}")]
        public IActionResult Actualizar_refaccion([FromBody] Cat_Materiales refaccion)
        {
            var result = new Models.Response();

            try
            {
                var item = _context.Cat_Materiales.FirstOrDefault(t => t.id == refaccion.id);
                if (item == null)
                {
                    return BadRequest();
                }
                else
                {
                    item.descripcion = refaccion.descripcion;
                    item.cantidad = refaccion.cantidad;
                    _context.Cat_Materiales.Update(item);
                    _context.SaveChanges();
                    result = new Models.Response
                    {
                        response = "OK"
                    };
                }
            }
            catch (Exception ex)
            {
                result = new Models.Response
                {
                    response = ex.ToString()
                };
            }
            return new ObjectResult(result);
        }

        // POST: api/Refacciones/Guardar_refaccion
        [Route("Guardar_refaccion")]
        [HttpPost("{Guardar_refaccion}")]
        public IActionResult Guardar_refaccion([FromBody] Cat_Materiales refaccion)
        {
            var result = new Models.Response();

            var item = _context.Cat_Materiales.FirstOrDefault(t => t.no_material == refaccion.no_material);
            if (item == null)
            {
                _context.Cat_Materiales.Add(refaccion);
                _context.SaveChanges();
                result = new Models.Response
                {
                    response = "OK"
                };
            }
            else
            {
                item.cantidad = item.cantidad + refaccion.cantidad;
                _context.Cat_Materiales.Update(item);
                _context.SaveChanges();
                result = new Models.Response
                {
                    response = "OK"
                };
            }

            return new ObjectResult(result);
        }

        // POST: api/Refacciones/Guardar_refaccion
        [Route("Guardar_refaccion_Excel")]
        [HttpPost("{Guardar_refaccion_Excel}")]
        public IActionResult Guardar_refaccion_Excel([FromBody] Cat_Materiales refaccion)
        {
            var result = new Models.Response();
            try
            {
                var item = _context.Cat_Materiales.FirstOrDefault(t => t.no_material == refaccion.no_material);
                
                if (item == null)
                {
                    _context.Cat_Materiales.Add(refaccion);
                    _context.SaveChanges();
                    result = new Models.Response
                    {
                        response = "OK-ADD"
                    };
                }
                else
                {
                    item.cantidad = item.cantidad + refaccion.cantidad;
                    _context.Cat_Materiales.Update(item);
                    _context.SaveChanges();
                    result = new Models.Response
                    {
                        response = "OK-UPDATE"
                    };
                }
            }
            catch (Exception ex)
            {
                result = new Models.Response
                {
                    response = ex.ToString()
                };
            }
            return new ObjectResult(result);
        }

        // POST: api/Refacciones/refaccion_visita
        [Route("refaccion_visita")]
        [HttpPost("{refaccion_visita}")]
        public IActionResult refaccion_visita([FromBody] Informe_parte_recibida refaccion)
        {
            try
            {
                var item = (from aa in _context.Piezas_Repuesto
                                   join bb in _context.Rel_servicio_Refaccion on aa.id_rel_servicio_refaccion equals bb.id
                                   join cc in _context.Cat_Materiales on aa.id_material equals cc.id
                                   where cc.no_material == refaccion.no_material/* && bb.id_producto == refaccion.id_producto*/
                                   select new
                                   {
                                       id_rel = bb.id,
                                       bb.id,
                                       id_material = cc.id,
                                       bb.id_vista,
                                       bb.id_producto,
                                       cc.no_material,
                                       id_servicio = bb.visita.servicio.id
                                   }).ToList();
                if (item == null)
                {
                    return NotFound();
                }
                return new ObjectResult(item);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }

        // POST: api/Refacciones/lista_pendientes_partes
        [Route("lista_pendientes_partes")]
        [HttpPost("{lista_pendientes_partes}")]
        public IActionResult lista_pendientes_partes()
        {
            try
            {
                var item = (from aa in _context.Informe_parte_recibida
                            join bb in _context.Visita on aa.id_visita equals bb.id
                            join cc in _context.Servicio on bb.id_servicio equals cc.id
                            join ee in _context.Cliente_Productos on aa.id_producto equals ee.Id
                            join dd in _context.Cat_Productos on ee.Id_Producto equals dd.id
                            //where aa.id_visita == refaccion.id_visita
                            orderby cc.creado ascending
                            select new
                            {
                                folio = cc.id,
                                ibs = cc.IBS,
                                id_proucto = dd.id,
                                dd.nombre,
                                fecha_inicio = cc.creado.ToString("dd/MM/yyyy"),
                                no_material = aa.no_material
                            }).ToList();
                if (item == null)
                {
                    return NotFound();
                }
                return new ObjectResult(item);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }

        // POST: api/Refacciones/Guardar_informe
        [Route("Guardar_informe")]
        [HttpPost("{Guardar_informe}")]
        public IActionResult Guardar_informe([FromBody] Informe_parte_recibida refaccion)
        {

            var result = new Models.Response();
            var item = _context.Informe_parte_recibida.FirstOrDefault(t => t.id_producto == refaccion.id_producto && t.id_visita == refaccion.id_visita && t.no_material == refaccion.no_material);

            if (item == null)
            {
                //var ipr = _context.Informe_parte_recibida.Where(a => a.id > 0);
                //_context.Informe_parte_recibida.RemoveRange(ipr);
                _context.Informe_parte_recibida.Add(refaccion);
                _context.SaveChanges();
                result = new Models.Response
                {
                    response = "OK-ADD"
                };
            }
            else
            {
                item.cantidad = item.cantidad + refaccion.cantidad;
                _context.Informe_parte_recibida.Update(item);
                _context.SaveChanges();
                result = new Models.Response
                {
                    response = "OK-UPDATE"
                };
            }

            return new ObjectResult(result);
        }

        // POST: api/Refacciones/Eliminar_refaccion
        [Route("Eliminar_refaccion")]
        [HttpPost("{Eliminar_refaccion}")]
        public IActionResult Eliminar_refaccion([FromBody] Cat_Materiales refaccion)
        {
            var result = new Models.Response();

            try
            {
                var item = _context.Cat_Materiales.FirstOrDefault(t => t.id == refaccion.id);
                if (item == null)
                {
                    return BadRequest();
                }
                else
                {
                    _context.Cat_Materiales.Remove(item);
                    _context.SaveChanges();
                    result = new Models.Response
                    {
                        response = "OK"
                    };
                }
            }
            catch (Exception ex)
            {
                result = new Models.Response
                {
                    response = ex.ToString()
                };
            }
            return new ObjectResult(result);
        }

        // POST: api/Refacciones/Actualizar_refaccion
        [Route("Actualizar_refaccion_tecnico")]
        [HttpPost("{Actualizar_refaccion_tecnico}")]
        public IActionResult Actualizar_refaccion_tecnico([FromBody] Cat_Materiales_Tecnico_Refaccion refaccion)
        {
            var result = new Models.Response();

            try
            {
                var item = _context.Cat_Materiales_Tecnico.FirstOrDefault(t => t.id_material == refaccion.id_material && t.id_tecnico == refaccion.id_tecnico);
                if (item == null)
                {
                    return BadRequest();
                }
                else
                {
                    var item_material = _context.Cat_Materiales.FirstOrDefault(t => t.id == refaccion.id_material);

                    if (item_material != null)
                    {
                        if (item_material.cantidad > refaccion.cantidad)
                        {
                            item_material.cantidad = item_material.cantidad - refaccion.id_grupo_precio;

                            item.cantidad = refaccion.cantidad;
                            _context.Cat_Materiales_Tecnico.Update(item);
                            _context.Cat_Materiales.Update(item_material);
                            _context.SaveChanges();
                            result = new Models.Response
                            {
                                response = "OK"
                            };
                        }
                        else
                        {
                            result = new Models.Response
                            {
                                response = "La cantidad asignada no puede ser mayor al stock en inventario"
                            };
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                result = new Models.Response
                {
                    response = ex.ToString()
                };
            }
            return new ObjectResult(result);
        }

        // POST: api/Refacciones/Eliminar_refaccion
        [Route("Eliminar_refaccion_tecnico")]
        [HttpPost("{Eliminar_refaccion_tecnico}")]
        public IActionResult Eliminar_refaccion_tecnico([FromBody] Cat_Materiales_Tecnico refaccion)
        {
            var result = new Models.Response();

            try
            {
                var item = _context.Cat_Materiales_Tecnico.FirstOrDefault(t => t.id_material == refaccion.id_material && t.id_tecnico == refaccion.id_tecnico);
                if (item == null)
                {
                    return BadRequest();
                }
                else
                {
                    _context.Cat_Materiales_Tecnico.Remove(item);
                    _context.SaveChanges();
                    result = new Models.Response
                    {
                        response = "OK"
                    };
                }
            }
            catch (Exception ex)
            {
                result = new Models.Response
                {
                    response = ex.ToString()
                };
            }
            return new ObjectResult(result);
        }

        // POST: api/Refacciones/Asignar_refaccion_tecnico
        [Route("Asignar_refaccion_tecnico")]
        [HttpPost("{Asignar_refaccion_tecnico}")]
        public IActionResult Asignar_refaccion_tecnico([FromBody] Cat_Materiales_Tecnico refaccion)
        {
            var result = new Models.Response();
            try
            {
                var item = _context.Cat_Materiales_Tecnico.FirstOrDefault(t => t.id_material == refaccion.id_material && t.id_tecnico == refaccion.id_tecnico);
                if (item == null)
                {
                    _context.Cat_Materiales_Tecnico.Add(refaccion);
                }
                else
                {
                    item.cantidad = item.cantidad + refaccion.cantidad;
                    _context.Cat_Materiales_Tecnico.Update(item);
                    result = new Models.Response
                    {
                        response = "OK"
                    };
                }

                var item_material = _context.Cat_Materiales.FirstOrDefault(t => t.id == refaccion.id_material);

                if (item_material != null)
                {
                    if (item_material.cantidad > refaccion.cantidad)
                    {
                        item_material.cantidad = item_material.cantidad - refaccion.cantidad;

                        _context.Cat_Materiales.Update(item_material);

                        result = new Models.Response
                        {
                            response = "OK"
                        };
                    }
                    else
                    {
                        result = new Models.Response
                        {
                            response = "La cantidad asignada no puede ser mayor al stock en inventario"
                        };
                    }

                }
                _context.SaveChanges();
                result = new Models.Response
                {
                    response = "OK"
                };
            }
            catch (Exception ex)
            {
                result = new Models.Response
                {
                    response = ex.ToString()
                };
            }
            return new ObjectResult(result);
        }

        // POST: api/Refacciones/Prediagnostico
        [Route("Prediagnostico")]
        [HttpPost("{Prediagnostico}")]
        public IActionResult Prediagnostico([FromBody] Prediagnostico item)
        {

            IActionResult response;

            try
            {
                var visita = _context.Visita.FirstOrDefault(t => t.id == item.id_visita);

                visita.pre_diagnostico = true;
                _context.Prediagnostico.Add(item);
                _context.Visita.Update(visita);
                _context.SaveChanges();

                return response = Ok(new { item = "OK" });
            }
            catch (Exception ex)
            {
                return response = Ok(new
                {
                    item = ex.ToString()
                });
            }
            //return new ObjectResult(result);
        }

        // POST: api/Refacciones/Prediagnostico_Tecnico_Refacciones
        [Route("Prediagnostico_Tecnico_Refacciones")]
        [HttpPost("{Prediagnostico_Tecnico_Refacciones}")]
        public IActionResult Prediagnostico_Tecnico_Refacciones([FromBody] Tecnicos tecnico)
        {
            IActionResult response;

            try
            {
                if (tecnico == null)
                {
                    return NotFound();
                }
                else
                {
                    var item = (from a in _context.Visita
                                join b in _context.Prediagnostico on a.id equals b.id_visita
                                join c in _context.rel_tecnico_visita on a.id equals c.id_vista
                                where c.id_tecnico == tecnico.id && a.fecha_visita == DateTime.Today && a.asignacion_refacciones == true
                                //orderby a.no_material descending
                                select new
                                {
                                    a.id,
                                    id_servicio = a.id_servicio,
                                    //id_tecnico = a.id_tecnico,
                                    fecha_servcio = a.fecha_visita.ToShortDateString(),
                                    refacciones = (from pre in _context.Prediagnostico
                                                   join p_r in _context.Prediagnostico_Refacciones on pre.id equals p_r.id_prediagnostico
                                                   join ma in _context.Cat_Materiales on p_r.id_material equals ma.id
                                                   where pre.id_visita == a.id
                                                   select new
                                                   {
                                                       id_material = ma.id,
                                                       ma.no_material,
                                                       material = ma.descripcion
                                                   })
                                }).Distinct().ToList();

                    if (item.Count == 0)
                    {
                        return response = Ok(new { item = "No hay resultado para la busqueda" });
                    }
                    return new ObjectResult(item);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }

        //POST: api/Refacciones/Prediagnostico_Tecnico_Refacciones
        [Route("Prediagnostico_Tecnico_Refacciones_Entrega")]
        [HttpPost("{Prediagnostico_Tecnico_Refacciones_Entrega}")]
        public IActionResult Prediagnostico_Tecnico_Refacciones_Entrega()
        {
            IActionResult response;

            try
            {
                var visitas = _context.Visita.FirstOrDefault(v => v.id == 11239);

                string fecha = visitas.fecha_entrega_refaccion.ToShortDateString();
                string fecha2 = visitas.fecha_visita.ToShortDateString();

                var item = (from a in _context.Visita
                            join b in _context.Prediagnostico on a.id equals b.id_visita
                            //join re in _context.Prediagnostico_Refacciones on b.id equals re.id_prediagnostico
                            where a.fecha_entrega_refaccion.ToShortDateString() == DateTime.Today.ToShortDateString() || a.fecha_visita.ToShortDateString() == DateTime.Today.ToShortDateString()
                            //orderby a.no_material descending
                            select new
                            {
                                a.id,
                                id_servicio = a.id_servicio,
                                //id_tecnico = a.id_tecnico,
                                tecnicos = (from _a in _context.rel_tecnico_visita
                                            join _c in _context.Users on _a.id_tecnico equals _c.id
                                            where _a.id_vista == a.id && _a.tecnico_responsable == true
                                            select new
                                            {
                                                _c.id,
                                                tecnico = _c.name + " " + _c.paterno + " " + (_c.materno == null ? "" : _c.materno),
                                                _a.tecnico_responsable
                                            }).Distinct().ToList(),
                                a.pre_diagnostico,
                                a.asignacion_refacciones,
                                a.entrega_refacciones,
                                fecha_servcio = a.fecha_visita.ToShortDateString(),
                                b.observaciones,
                                //tecnico = user.name + " " + user.paterno + " " + user.materno,
                                refacciones = (from pre in _context.Prediagnostico
                                               join visita in _context.Visita on pre.id_visita equals visita.id
                                               join p_r in _context.Prediagnostico_Refacciones on pre.id equals p_r.id_prediagnostico
                                               join ma in _context.Cat_Materiales on p_r.id_material equals ma.id
                                               where visita.id == a.id && ma.cantidad > 0 //&& visita.fecha_visita == DateTime.Today 
                                               select new
                                               {
                                                   p_r.id,
                                                   id_material = ma.id,
                                                   ma.no_material,
                                                   p_r.numero_ir,
                                                   material = ma.descripcion,
                                                   p_r.cantidad,
                                                   p_r.estatus
                                               }).ToList()
                            //}).ToList();
                }).Where(t => t.entrega_refacciones == false && t.refacciones.Count() > 0).ToList();


                if (item.Count() == 0)
                {
                    return response = Ok(new { item = "No hay resultado para la busqueda" });
                }
                return new ObjectResult(item);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }

        //POST: api/Refacciones/Prediagnostico_Tecnico_Refacciones
        [Route("Prediagnostico_Tecnico_Entrega_Refacciones")]
        [HttpPost("{Prediagnostico_Tecnico_Entrega_Refacciones}")]
        public IActionResult Prediagnostico_Tecnico_Entrega_Refacciones([FromBody] rel_tecnico_visita tecnico)
        {
           
            try
            {
                var item = (from a in _context.Visita
                            join b in _context.Prediagnostico on a.id equals b.id_visita
                            join c in _context.rel_tecnico_visita on a.id equals c.id_vista
                            where c.id_tecnico == tecnico.id_tecnico && a.si_acepto_tecnico_refaccion == false                            
                            select new
                            {
                                a.id,
                                id_servicio = a.id_servicio,
                                //id_tecnico = a.id_tecnico,
                                tecnicos = (from _a in _context.Visita
                                            join _b in _context.rel_tecnico_visita on _a.id equals _b.id_vista
                                            join _c in _context.Users on _b.id_tecnico equals _c.id
                                            where _a.id == a.id && _c.estatus == true
                                            select new
                                            {
                                                _c.id,
                                                tecnico = _c.name + " " + _c.paterno + " " + (_c.materno == null ? "" : _c.materno),
                                                _b.tecnico_responsable
                                            }).Distinct().ToList(),
                                a.pre_diagnostico,
                                a.asignacion_refacciones,
                                a.entrega_refacciones,
                                fecha_servcio = a.fecha_visita.ToShortDateString(),
                                b.observaciones,
                                //tecnico = user.name + " " + user.paterno + " " + user.materno,
                                refacciones = (from pre in _context.Prediagnostico
                                               join visita in _context.Visita on pre.id_visita equals visita.id
                                               join p_r in _context.Prediagnostico_Refacciones on pre.id equals p_r.id_prediagnostico
                                               join ma in _context.Cat_Materiales on p_r.id_material equals ma.id
                                               where visita.id == a.id && p_r.cantidad > 0
                                               select new
                                               {
                                                   p_r.id,
                                                   id_material = ma.id,
                                                   ma.no_material,
                                                   material = ma.descripcion,
                                                   p_r.cantidad,
                                                   p_r.estatus
                                               }).Distinct().ToList()
                            }).ToList().Where(x => x.refacciones.Count() != 0);

                return new ObjectResult(item);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }

        // POST: api/Refacciones/Aceptar_Refacciones
        [Route("Aceptar_Refacciones")]
        [HttpPost("{Aceptar_Refacciones}")]
        public IActionResult Aceptar_Refacciones([FromBody] rel_tecnico_visita visita)
        {
            var result = new Models.Response();

            try
            {
                var vis = _context.Visita.SingleOrDefault(t => t.id == visita.id_vista);
                vis.si_acepto_tecnico_refaccion = true;
                _context.Visita.Update(vis);

                var item = (from pre in _context.Prediagnostico
                            join rel in _context.Prediagnostico_Refacciones on pre.id equals rel.id_prediagnostico
                            where pre.id_visita == visita.id_vista && rel.estatus
                            select rel).ToList();

                if (item == null)
                {
                    return BadRequest();
                }
                else
                {
                    for (int j = 0; j < item.Count; j++)
                    {
                        var _item = _context.Cat_Materiales.FirstOrDefault(t => t.id == item[j].id_material);
                        var _item1 = _context.Cat_Materiales_Tecnico.FirstOrDefault(t => t.id_material == item[j].id_material && t.id_tecnico == visita.id_tecnico);

                        if (_item.cantidad == null)
                        {
                            _item.cantidad = 0;
                        }
                        _item.cantidad = _item.cantidad - item[j].cantidad;
                        _context.Cat_Materiales.Update(_item);

                        if (_item1 != null)
                        {
                            _item1.cantidad = _item1.cantidad + item[j].cantidad;
                            _context.Cat_Materiales_Tecnico.Update(_item1);
                        }
                        else
                        {
                            Cat_Materiales_Tecnico _Cat_Materiales_Tecnico = new Cat_Materiales_Tecnico();
                            _Cat_Materiales_Tecnico.cantidad = item[j].cantidad;
                            _Cat_Materiales_Tecnico.estatus = true;
                            _Cat_Materiales_Tecnico.id_tecnico = Convert.ToInt32(visita.id_tecnico);
                            _Cat_Materiales_Tecnico.id_material = Convert.ToInt32(item[j].id_material);

                            _context.Cat_Materiales_Tecnico.Add(_Cat_Materiales_Tecnico);
                        }
                    }

                    _context.SaveChanges();
                    result = new Models.Response
                    {
                        response = "OK"
                    };
                }
            }
            catch (Exception ex)
            {
                result = new Models.Response
                {
                    response = ex.ToString()
                };
            }
            return new ObjectResult(result);
        }

        // POST: api/Refacciones/Aceptar_Refacciones
        [Route("Entregar_Refacciones")]
        [HttpPost("{Entregar_Refacciones}")]
        public IActionResult Entregar_Refacciones([FromBody] Visita visita)
        {
            var result = new Models.Response();

            try
            {
                var _item = _context.Visita.FirstOrDefault(t => t.id == visita.id);
                _item.entrega_refacciones = true;
                _context.Visita.Update(_item);

                _context.SaveChanges();
                result = new Models.Response
                {
                    response = "OK"
                };
            }
            catch (Exception ex)
            {
                result = new Models.Response
                {
                    response = ex.ToString()
                };
            }
            return new ObjectResult(result);
        }

        [Route("Entregar_Refacciones_Cantidad")]
        [HttpPost("{Entregar_Refacciones_Cantidad}")]
        public IActionResult Entregar_Refacciones_Cantidad([FromBody] Prediagnostico_Refacciones pre)
        {
            var result = new Models.Response();

            try
            {
                var _item = _context.Prediagnostico_Refacciones.FirstOrDefault(t => t.id == pre.id);
                _item.cantidad = pre.cantidad;
                _item.numero_ir = pre.numero_ir;
                _context.Prediagnostico_Refacciones.Update(_item);

                _context.SaveChanges();
                result = new Models.Response
                {
                    response = "OK"
                };
            }
            catch (Exception ex)
            {
                result = new Models.Response
                {
                    response = ex.ToString()
                };
            }
            return new ObjectResult(result);
        }

        // POST: api/Refacciones/Aceptar_Refacciones
        [Route("Asignar_Refacciones_Tecnico")]
        [HttpPost("{Asignar_Refacciones_Tecnico}")]
        public IActionResult Asignar_Refacciones_Tecnico([FromBody] Visita visita)
        {
            var result = new Models.Response();

            try
            {
                var item = _context.Visita.FirstOrDefault(t => t.id == visita.id);
                var tecnico = _context.rel_tecnico_visita.FirstOrDefault(t => t.id_tecnico == Convert.ToInt64(visita.comprobante) && t.id_vista == visita.id);
                var refaccion = (from a in _context.Prediagnostico
                                 where a.id_visita == visita.id
                                 select new
                                 {
                                     a.id
                                 }).Distinct().ToList();

                List<Prediagnostico_Refacciones> parts = new List<Prediagnostico_Refacciones>();
                for (int i = 0; i < visita.no_operacion.Split(',').Count(); i++)
                {
                    if (visita.no_operacion.Split(',')[i] != "")
                    {
                        parts.Add(new Prediagnostico_Refacciones() { id_material = Convert.ToInt64(visita.no_operacion.Split(',')[i]) });
                    }
                }

                for (int j = 0; j < parts.Count(); j++)
                {
                    var item_prediagnostico = _context.Prediagnostico_Refacciones.FirstOrDefault(t => t.id_prediagnostico == refaccion[0].id && t.id_material == parts[j].id_material);
                    item_prediagnostico.estatus = true;
                }

                List<Prediagnostico_Refacciones> garantia = new List<Prediagnostico_Refacciones>();
                for (int i = 0; i < visita.concepto.Split(',').Count(); i++)
                {
                    if (visita.concepto.Split(',')[i] != "")
                    {
                        garantia.Add(new Prediagnostico_Refacciones() { id_material = Convert.ToInt64(visita.concepto.Split(',')[i]) });
                    }
                }

                for (int j = 0; j < garantia.Count(); j++)
                {
                    var item_garantia = _context.Prediagnostico_Refacciones.FirstOrDefault(t => t.id_prediagnostico == refaccion[0].id && t.id_material == garantia[j].id_material);
                    item_garantia.garantia = true;
                }

                if (item == null)
                {
                    return BadRequest();
                }
                else
                {
                    tecnico.tecnico_responsable = true;
                    item.asignacion_refacciones = visita.asignacion_refacciones;
                    item.fecha_entrega_refaccion = visita.fecha_entrega_refaccion;
                    _context.rel_tecnico_visita.Update(tecnico);
                    _context.Visita.Update(item);
                    _context.SaveChanges();
                    result = new Models.Response
                    {
                        response = "OK"
                    };
                }
            }
            catch (Exception ex)
            {
                result = new Models.Response
                {
                    response = ex.ToString()
                };
            }
            return new ObjectResult(result);
        }

        // POST: api/Refacciones/Asignar_Refacciones
        [Route("Asignar_Refacciones")]
        [HttpPost("{Asignar_Refacciones}")]
        public IActionResult Asignar_Refacciones([FromBody] Visita visita)
        {
            var _item = (from pre in _context.Prediagnostico
                         join vis in _context.Visita on pre.id_visita equals vis.id
                         where pre.id_visita == visita.id
                         select new
                         {
                             pre.observaciones,
                             fecha_entrega_refaccion = vis.fecha_entrega_refaccion.ToShortDateString(),
                             refacciones = (from a in _context.Prediagnostico
                                            join b in _context.Prediagnostico_Refacciones on a.id equals b.id_prediagnostico
                                            join c in _context.Cat_Materiales on b.id_material equals c.id
                                            where a.id_visita == visita.id
                                            select new
                                            {
                                                c.id,
                                                c.no_material,
                                                c.descripcion,
                                                b.cantidad,
                                                b.estatus,
                                                b.garantia

                                            }).Distinct().ToList(),
                             tecnicos = (from a in _context.Visita
                                         join b in _context.rel_tecnico_visita on a.id equals b.id_vista
                                         join c in _context.Users on b.id_tecnico equals c.id
                                         where a.id == visita.id
                                         select new
                                         {
                                             c.id,
                                             tecnico = c.name + " " + c.paterno + " " + (c.materno == null ? "" : c.materno),
                                             b.tecnico_responsable
                                         }).Distinct().ToList()

                         }).Distinct().ToList();

            //if (_item.Count == 0)
            //{
            //    return NotFound();
            //}
            return new ObjectResult(_item);
        }

        // POST: api/Refacciones/no_material_validar
        [Route("no_material_validar")]
        [HttpPost("{no_material_validar}")]
        public IActionResult no_material_validar([FromBody] Cat_Materiales material)
        {
            var _item = (from pre in _context.Cat_Materiales
                         where pre.no_material == material.no_material
                         select new
                         {
                             pre.no_material,
                             pre.descripcion,
                             pre.id_grupo_precio

                         }).Distinct().ToList();

            if (_item.Count == 0)
            {
                return NotFound();
            }
            return new ObjectResult(_item);
        }

        // POST: api/Refacciones/Almacenes
        [Route("Almacenes")]
        [HttpPost("{Almacenes}")]
        public IActionResult almacenes()
        {
            var _item = (from u in _context.Users
                         join t in _context.Tecnicos on u.id equals t.id
                         select new
                         {
                             u.id,
                             t.noalmacen,
                             tecnico = u.name + " " + u.paterno + " " + (u.materno == null ? "" : u.materno)

                         }).Distinct().ToList();

            if (_item.Count == 0)
            {
                return NotFound();
            }
            return new ObjectResult(_item);
        }

        // POST: api/Refacciones/Almacenes_Tecnico
        [Route("Almacenes_Tecnico")]
        [HttpPost("{Almacenes_Tecnico}")]
        public IActionResult Almacenes_Tecnico([FromBody] Tecnicos tecnico)
        {
            IQueryable _item;
            if (tecnico.id != 0)
            {
                _item = (from u in _context.Users
                         join t in _context.Tecnicos on u.id equals t.id
                         join r in _context.Cat_Materiales_Tecnico on Convert.ToInt32(t.id) equals r.id_tecnico
                         join rf in _context.Cat_Materiales on r.id_material equals rf.id
                         where u.id == tecnico.id && EF.Functions.Like(rf.no_material + "" + rf.descripcion, "%" + tecnico.noalmacen + "%")
                         orderby rf.descripcion
                         select new
                         {
                             u.id,
                             id_refaccion = r.id_material,
                             rf.no_material,
                             refaccion = rf.descripcion,
                             r.cantidad

                         }).Distinct().AsQueryable();
            }
            else
            {
                _item = (from u in _context.Cat_Materiales
                         where EF.Functions.Like(u.no_material + "" + u.descripcion, "%" + tecnico.noalmacen + "%")
                         orderby u.descripcion
                         select new
                         {
                             u.id,
                             id_refaccion = u.id,
                             u.no_material,
                             refaccion = u.descripcion,
                             u.cantidad

                         }).Distinct().AsQueryable();
            }

            if (_item == null)
            {
                return NotFound();
            }
            return new ObjectResult(_item);
        }

        // POST: api/Refacciones/Almacenes_Devolucion
        [Route("Almacenes_Devolucion")]
        [HttpPost("{Almacenes_Devolucion}")]
        public IActionResult Almacenes_Devolucion([FromBody] List<Cat_Materiales_Tecnico_Refaccion_Inventario> tecnico)
        {
            for (int i = 0; i < tecnico.Count(); i++)
            {

                var _item_tecnico = _context.Cat_Materiales_Tecnico.FirstOrDefault(t => t.id_tecnico == tecnico[i].id_tecnico && t.id_material == tecnico[i].id_material);
                var _item_general = _context.Cat_Materiales.FirstOrDefault(t => t.id == tecnico[i].id_material);

                if (_item_tecnico == null)
                {
                    return NotFound();
                }

                _item_tecnico.cantidad = Convert.ToInt32(_item_tecnico.cantidad) - Convert.ToInt32(tecnico[i].descripcion);
                _item_general.cantidad = Convert.ToInt32(_item_general.cantidad) + Convert.ToInt32(tecnico[i].descripcion);

                Log_refacciones _Log_refacciones = new Log_refacciones();

                _Log_refacciones.id_usuario = tecnico[i].id_usuario.Value;
                _Log_refacciones.id_refaccion = tecnico[i].id_material;
                _Log_refacciones.almacen_entrada = "531";
                _Log_refacciones.almacen_salida = tecnico[i].id_tecnico.ToString();
                _Log_refacciones.cantidad = Convert.ToInt32(tecnico[i].descripcion);

                _context.Log_refacciones.Add(_Log_refacciones);
                _context.Cat_Materiales_Tecnico.Update(_item_tecnico);
                _context.Cat_Materiales.Update(_item_general);
            }

            _context.SaveChanges();

            return new ObjectResult("OK");
        }

        // GET: Refaccciones/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Refaccciones/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Refaccciones/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Refaccciones/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public class Cat_Materiales_Tecnico_Refaccion
        {
            public int id { get; set; }
            public string descripcion { get; set; }
            public int id_grupo_precio { get; set; }
            public int id_material { get; set; }
            public int? id_tecnico { get; set; }
            public int cantidad { get; set; }
            public bool estatus { get; set; }

        }

        public class Cat_Materiales_Tecnico_Refaccion_Inventario
        {
            public int id { get; set; }
            public string descripcion { get; set; }
            public int id_grupo_precio { get; set; }
            public int id_material { get; set; }
            public int? id_tecnico { get; set; }
            public int? id_usuario { get; set; }
            public int cantidad { get; set; }
            public bool estatus { get; set; }

        }
    }
}