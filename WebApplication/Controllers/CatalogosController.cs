using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/Catalogos")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CatalogosController : Controller
    {
        private IConfiguration _config;
        private readonly MieleContext _context;

        public CatalogosController(IConfiguration config, MieleContext context)
        {
            _config = config;
            _context = context;
        }

        // GET: api/Catalogos
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Catalogos/5
        [HttpGet("{id}", Name = "Get_")]
        public string Get(int id)
        {
            return "test";
        }

        // GET: api/Catalogos/Productos
        //[AllowAnonymous]
        //[Route("Productos")]
        //[HttpPost("{Productos}")]
        //public IEnumerable<Cat_Producto> GetProductos()
        //{
        //    return _context.Cat_Producto.ToList();
        //}

        // GET: api/Catalogos/Fallas
        [AllowAnonymous]
        [Route("Fallas")]
        [HttpPost("{Fallas}")]
        public IActionResult Fallas()
        {
            var item = (from a in _context.cat_falla
                        select new
                        {
                            id = a.id,
                            desc_falla_en = a.desc_falla_en.ToUpper(),
                            desc_falla_es = a.desc_falla_es.ToUpper()
                        }).OrderBy(f => f.desc_falla_es).ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return new ObjectResult(item);
        }

        [AllowAnonymous]
        [Route("Reparacion")]
        [HttpPost("{Reparacion}")]
        public IActionResult Reparacion()
        {
            var item = (from a in _context.cat_reparacion
                        select new
                        {
                            id = a.id,
                            desc_reparacion_en = a.desc_reparacion_en.ToUpper(),
                            desc_reparacion_es = a.desc_reparacion_es.ToUpper()
                        }).OrderBy(r => r.desc_reparacion_es).ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return new ObjectResult(item);
        }

        // GET: api/Catalogos/Productos
        [AllowAnonymous]
        [Route("Productos")]
        [HttpPost("{Productos}")]
        public IEnumerable<Cat_Producto> GetProductos()
        {
            return _context.Cat_Producto.ToList();
        }

        // GET: api/Catalogos/Tipo_Refaccion
        [AllowAnonymous]
        [Route("Tipo_Refaccion")]
        [HttpPost("{Tipo_Refaccion}")]
        public IEnumerable<cat_tipo_refaccion> GetTipo_Refaccion()
        {
            return _context.Cat_Tipo_Refaccion.ToList();
        }

        // GET: api/Catalogos/Cat_Checklist_Producto
        [AllowAnonymous]
        [Route("Cat_Checklist_Producto")]
        [HttpPost("{Cat_Checklist_Producto}")]
        public IActionResult Cat_Checklist_Producto()
        {
            var item = (from a in _context.Cat_Checklist_Producto
                        select new
                        {
                            id = a.id,
                            desc_estatus_producto = a.desc_checklist_producto.ToUpper()
                        }).ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return new ObjectResult(item);
        }

        // GET: api/Catalogos/Tecnicos
        [AllowAnonymous]
        [Route("Tecnicos")]
        [HttpPost("{Tecnicos}")]
        public IActionResult Tecnicos()
        {
            var item = (from a in _context.Tecnicos
                        join b in _context.Users on a.id equals b.id
                        select new
                        {
                            id = a.id,
                            tecnico = b.name + " " + b.paterno + " " + (b.materno == null ? "" : b.materno)

                        }).ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return new ObjectResult(item);
        }

        // GET: api/Catalogos/usuarios
        [AllowAnonymous]
        [Route("usuarios")]
        [HttpPost("{usuarios}")]
        public IActionResult usuarios()
        {
            var item = (from b in _context.Users
                        join c in _context.Cat_Cuentas on b.id_cuenta equals c.Id
                        where b.id_app == 2
                        select new
                        {
                            id = b.id,
                            nombrecompleto = b.name + " " + b.paterno + " " + (b.materno == null ? "" : b.materno) + " - " + c.Cuenta_es

                        }).ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return new ObjectResult(item);
        }

        // GET: api/Catalogos/Tecnicos
        [AllowAnonymous]
        [Route("TecnicosId")]
        [HttpPost("{TecnicosId}")]
        public IActionResult TecnicosId([FromBody] estado_id id)
        {
            var item = (from a in _context.Tecnicos
                        join b in _context.Users on a.id equals b.id
                        where a.id == id.id
                        select new
                        {
                            id = a.id,
                            tecnico = b.name + " " + b.paterno + " " + (b.materno == null ? "" : b.materno)

                        }).ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return new ObjectResult(item);
        }

        // GET: api/Catalogos/Tipo_Tecnico
        [AllowAnonymous]
        [Route("Tipo")]
        [HttpPost("{Tecnicos_Tipo}")]
        public IEnumerable<Cat_Tecnicos_Tipo> GetTipo_Tecnico()
        {
            return _context.Cat_Tecnicos_Tipo.ToList();
        }

        // GET: api/Catalogos/Sub_Tipo_Tecnico
        [AllowAnonymous]
        [Route("Sub_Tipo_Tecnico")]
        [HttpPost("{Sub_Tipo_Tecnico}")]
        public IEnumerable<Cat_Tecnicos_Sub_Tipo> GetSub_Tipo_Tecnico([FromBody] estado_id id)
        {
            return _context.Cat_Tecnicos_Sub_Tipo.Where(x => x.id_tipo == id.id).ToList();
        }

        [AllowAnonymous]
        [Route("TipoServicio")]
        [HttpPost("{TipoServicio}")]
        public IEnumerable<Cat_tipo_servicio> GetTipoServicio()
        {
            return _context.Cat_tipo_servicio.Where(p => p.estatus == true).ToList();
        }

        [AllowAnonymous]
        [Route("SubTipoServicio")]
        [HttpPost("{SubTipoServicio}")]
        public IActionResult SubTipoServicio([FromBody] estado_id id)
        {
            var item = (from a in _context.Sub_cat_tipo_servicio
                        where a.id_tipo_servicio == id.id
                        orderby a.sub_desc_tipo_servicio descending
                        select new
                        {
                            a.id,
                            a.sub_desc_tipo_servicio

                        }).ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return new ObjectResult(item);
        }

        [AllowAnonymous]
        [Route("garantia")]
        [HttpPost("{garantia}")]
        public IEnumerable<cat_garantia> GetGarantia()
        {
            return _context.cat_garantia.ToList();
        }

        [AllowAnonymous]
        [Route("periodo")]
        [HttpPost("{periodo}")]
        public IEnumerable<cat_periodo> Getperiodo()
        {
            return _context.cat_periodo.ToList();
        }

        [AllowAnonymous]
        [Route("Distribuidor")]
        [HttpPost("{Distribuidor}")]
        public IEnumerable<Cat_distribuidor_autorizado> GetDistribuidor()
        {
            return _context.Cat_distribuidor_autorizado.ToList().OrderBy(x => x.desc_distribuidor);
        }

        [AllowAnonymous]
        [Route("CategoriaServicio")]
        [HttpPost("{CategoriaServicio}")]
        public IActionResult GetCataegoriaServicio([FromBody] estado_id id)
        {
            var item = (from a in _context.Cat_Categoria_Servicio
                        where a.id_tipo_servicio == id.id
                        select new
                        {
                            id = a.id,
                            desc_categoria_servicio = a.desc_categoria_servicio

                        }).ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return new ObjectResult(item);
        }

        [AllowAnonymous]
        [Route("CategoriaProducto")]
        [HttpPost("{CategoriaProducto}")]
        public IActionResult CategoriaProducto()
        {
            var item = (from a in _context.Cat_SubLinea_Producto
                        select new
                        {
                            id = a.id,
                            descripcion = a.descripcion.ToUpper(),
                            a.id_linea_producto

                        }).ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return new ObjectResult(item);
        }

        [AllowAnonymous]
        [Route("ListaPrecios")]
        [HttpPost("{ListaPrecios}")]
        public IEnumerable<Cat_Lista_Precios> ListaPrecios()
        {
            return _context.Cat_Lista_Precios.ToList();
        }

        //[AllowAnonymous]
        //[Route("Estatus_visita")]
        //[HttpPost("{ListaPrecios}")]
        //public IEnumerable<Cat_Lista_Precios> ListaPrecios()
        //{
        //    return _context.Cat_Lista_Precios.ToList();
        //}

        [AllowAnonymous]
        [Route("Estados")]
        [HttpPost("{Estados}")]
        public IEnumerable<Cat_Estado> Estados()
        {
            return _context.Cat_Estado.ToList();
        }

        [AllowAnonymous]
        [Route("Municipio")]
        [HttpPost("{Municipio}")]
        public IActionResult Municipios([FromBody] estado_id id)
        {
            var item = (from a in _context.Cat_Municipio
                        where a.estado.id == id.id
                        select new
                        {
                            id = a.id,
                            municipio = a.desc_municipio

                        }).ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return new ObjectResult(item);
        }

        [AllowAnonymous]
        [Route("Estatus_Troubleshooting")]
        [HttpPost("{Estatus_Troubleshooting}")]
        public IEnumerable<Cat_Productos_Estatus_Troubleshooting> Estatus_Troubleshooting()
        {
            return _context.Cat_Productos_Estatus_Troubleshooting.ToList();
        }

        [AllowAnonymous]
        [Route("Estatus_Producto")]
        [HttpPost("{Estatus_Producto}")]
        public IActionResult Estatus_Producto()
        {
            var item = (from a in _context.CatEstatus_Producto
                        select new
                        {
                            id = a.id,
                            desc_estatus_producto = a.desc_estatus_producto.ToUpper(),
                            desc_estatus_producto_en = a.desc_estatus_producto_en.ToUpper()
                        }).ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return new ObjectResult(item);
        }

        [AllowAnonymous]
        [Route("Labor")]
        [HttpPost("{Labor}")]
        public IEnumerable<Cer_labor> GetLabor([FromBody] Cer_labor labor)
        {
            return _context.Cer_labor.Where(x => x.cantidad_equipos == labor.cantidad_equipos).ToList();
        }

        [AllowAnonymous]
        [Route("Viaticos")]
        [HttpPost("{Viaticos}")]
        public IActionResult GetViaticos([FromBody] Cat_Localidad cer)
        {
            var item = (from a in _context.Cat_Localidad
                         join b in _context.Cat_Municipio on a.municipio.id equals b.id
                         join c in _context.Cat_Estado on b.estado.id equals c.id
                         join d in _context.Cer_viaticos on a.id equals d.id_cat_localidad
                         where a.cp == cer.cp
                         select new
                         {
                             id_viatico = d.id,
                             cp = a.cp,
                             estado = c.desc_estado,
                             id_estado = c.id,
                             municipio = b.desc_municipio,
                             id_municipio = b.id,
                             d.costo_unitario,
                             d.anual,
                             localidades = (from a in _context.Cat_Localidad
                                            join b in _context.Cat_Municipio on a.municipio.id equals b.id
                                            join c in _context.Cat_Estado on b.estado.id equals c.id
                                            where a.cp == cer.cp
                                            select new
                                            {
                                                id_localidad = a.id,
                                                localidad = a.desc_localidad,
                                            }).Distinct().ToList()

                         }).Distinct().Take(1).ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return new ObjectResult(item);
            //return _context.Cer_viaticos.ToList();
        }

        // POST: api/Catalogos
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Catalogos/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        public class estado_id
        {
            public int id { get; set; }
        }

        ////////////////// PARTNERS /////////////////
        ////////////////////////////////////////////

        // GET: api/Catalogos/Tipo_Tecnico
        [AllowAnonymous]
        [Route("Cat_Vendedores")]
        [HttpPost("{Cat_Vendedores}")]
        public IEnumerable<Vendedores> Get_CatVendedores()
        {
            return _context.Vendedores.ToList();
        }

        [AllowAnonymous]
        [Route("EstatusCotizacion")]
        [HttpPost("{EstatusCotizacion}")]
        public IEnumerable<Cat_Estatus_Cotizacion> Get_CatEstatusCotizacion()
        {
            return _context.Cat_Estatus_Cotizacion.ToList();
        }

        // GET: api/Catalogos/Tipo_Tecnico
        [AllowAnonymous]
        [Route("Cat_canales")]
        [HttpPost("{Cat_canales}")]
        public IEnumerable<Cat_canales> Get_Cat_canales()
        {
            return _context.Cat_canales.ToList();
        }

        // GET: api/Catalogos/Cat_Productos
        /*  [AllowAnonymous]
          [Route("Cat_Productos")]
          [HttpPost("{Cat_Productos}")]
          public IEnumerable<Cat_Productos> GetCat_Productos()
          {
              return _context.Cat_Productos.ToList();
          }*/

        [AllowAnonymous]
        [Route("Cat_Productos")]
        [HttpPost("{Cat_Productos}")]
        public IActionResult  GetCat_Productos()
        {
            IActionResult response = Unauthorized();
            try
            {

        var grp_producto =( from c in _context.Cat_Productos
                           join a in _context.Cat_SubLinea_Producto on c.id_sublinea equals a.id
                           select new
                           {
                               id = c.id,
                               sku = c.sku, 
                               nombre= c.nombre,
                               modelo=c.modelo, 
                               id_categoria = c.id_categoria, 
                               id_sublinea = c.id_sublinea, 
                               sublinea= a.descripcion,
                                      
                                   }).ToList();

                
                if (grp_producto == null)
                {
                    return response = Ok(new { result = "Error", item = grp_producto});
                }
                return response = Ok(new { result = "Success", item = grp_producto });
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error al crear-editar promocion: ", detalle = ex.Message, id = 0 });
            }
        }

        // POST: api/Catalogos/Nuevo_Producto
        [Route("Nuevo_Producto")]
        [HttpPost("{Nuevo_Producto}")]
        public IActionResult Nuevo_Producto([FromBody]Cat_Productos item)
        {
            IActionResult response;

            try
            {
                if (item == null)
                {
                    response = Ok(new { id = "0", response = "Error, intenta mas tarde" });
                }
                else
                {
                    _context.Cat_Productos.Add(item);
                    item.id_caracteristica_base = 36;
                    _context.SaveChanges();
                    var query = (from a in _context.Cat_Productos
                                 join b in _context.Cat_SubLinea_Producto on a.id_sublinea equals b.id
                                 where a.id == item.id
                                 select new
                                 {
                                     id = a.id,
                                     sku = a.sku,
                                     categoria = b.descripcion,
                                     a.id_sublinea,
                                     nombre = a.nombre,
                                     modelo = a.modelo,
                                     //codigo = b.codigo,
                                     hora_tecnico = a.horas_tecnico,
                                     hora_precio = a.precio_hora,
                                     
                                 }).ToList();

                    response = Ok(new { item = query, response = "Producto agregado correctamente" });
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { id = "0", response = ex.ToString() });
            }
            return new ObjectResult(response);
        }

        // POST: api/Catalogos/Horas_tecnico
        [Route("Horas_tecnico")]
        [HttpPost("{Horas_tecnico}")]
        public IActionResult Horas_tecnico([FromBody]Rel_Categoria_Producto_Tipo_Producto tipo)
        {
            IActionResult response;

            try
            {
                var item = _context.Rel_Categoria_Producto_Tipo_Producto.SingleOrDefault(t => t.id_categoria == tipo.id_categoria && t.id_tipo_servicio == tipo.id_tipo_servicio);
                if (item == null)
                {
                    response = Ok(new { id = "0", response = "Error, intenta mas tarde" });
                }
                else
                {
                    response = Ok(new { item });
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { id = "0", response = ex.ToString() });
            }
            return new ObjectResult(response);
        }

        // GET: api/Catalogos/Cat_Cuentas
        [AllowAnonymous]
        [Route("Cat_Cuentas")]
        [HttpPost("{Cat_Cuentas}")]
        public IEnumerable<Cat_Cuentas> GetCat_Cuentas()
        {
            return _context.Cat_Cuentas.ToList();
        }

        // GET: api/Catalogos/Cat_Sucursales
        [AllowAnonymous]
        [Route("Cat_Sucursales")]
        [HttpPost("{Cat_Sucursales}")]
        public IEnumerable<Cat_Sucursales> Cat_Sucursales()
        {
            return _context.Cat_Sucursales.ToList();
        }

        [AllowAnonymous]
        [Route("Cat_Formas_Pago")]
        [HttpPost("{Cat_Formas_Pago}")]
        public IEnumerable<Cat_Formas_Pago> GetCat_Formas_Pago()
        {
            return _context.Cat_Formas_Pago.ToList();
        }

        [AllowAnonymous]
        [Route("Cat_CondicionesPago")]
        [HttpPost("{Cat_CondicionesPago}")]
        public IEnumerable<Cat_CondicionesPago> GetCat_CondicionesPago()
        {
            return _context.Cat_CondicionesPago.ToList();
        }
    }
}
