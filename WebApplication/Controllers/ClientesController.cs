using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplication.Models;
using WebApplication.Repository;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/Clientes")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ClientesController : Controller
    {
        private IConfiguration _config;
        private IEmailRepository _emailRepository;
        //private EmailSettings _emailSettings { get; set; }
        private readonly MieleContext _context;
        private IConverter _converter;

        public ClientesController(IConverter converter, IConfiguration config, IEmailRepository emailRepository, MieleContext context)
        {
            _converter = converter;
            _emailRepository = emailRepository;
            //_emailSettings = emailSettings;
            _config = config;
            _context = context;
        }

        // GET: api/Clientes
        [HttpGet]
        public IEnumerable<Users> GetAll()
        {
            return _context.Users.ToList();
        }

        // GET: api/Clientes/5
        [HttpGet("{id}", Name = "GetCliente")]
        public IActionResult GetById(long id)
        {
            var item = (from a in _context.Clientes
                        join _servicio in _context.Servicio on a.id equals _servicio.id_cliente into bb
                        from b in bb.DefaultIfEmpty()
                        join _cat_solicitud_via in _context.Cat_solicitud_via on b.id_solicitud_via equals _cat_solicitud_via.id into cc
                        from c in cc.DefaultIfEmpty()
                        where a.id == id
                        select new
                        {
                            id = a.id,
                            folio = a.folio,
                            nombre = a.nombre == null ? "" : a.nombre,
                            paterno = a.paterno == null ? "" : a.paterno,
                            materno = a.materno == null ? "" : a.materno,
                            email = a.email,
                            nombre_comercial = a.nombre_comercial == null ? "" : a.nombre_comercial,
                            nombre_contacto = a.nombre_contacto == null ? "" : a.nombre_contacto,
                            referencias = a.referencias,
                            telefono = a.telefono,
                            telefono_movil = a.telefono_movil,
                            refrerencias = a.referencias,
                            contacto = b.contacto,
                            tipo_contacto = c.desc_solicitud_via,
                            visitas = (from x in _context.Visita
                                       join ser in _context.Servicio on x.id_servicio equals ser.id
                                       join d in _context.Cat_Direccion on x.id_direccion equals d.id
                                       join e in _context.Cat_Estado on d.id_estado equals e.id
                                       join f in _context.Cat_Municipio on d.id_municipio equals f.id
                                       join tv in _context.rel_tecnico_visita on x.id equals tv.id_vista
                                       join i in _context.Tecnicos on tv.id_tecnico equals i.id
                                       join j in _context.Users on i.id equals j.id
                                       join kk in _context.CatEstatus_Visita on x.estatus equals kk.id into t
                                       from fee in t.DefaultIfEmpty()
                                       where ser.id_cliente == a.id
                                       select new
                                       {
                                           id = x.id,
                                           direccion = d.calle_numero + ", " + d.colonia + ", " + e.desc_estado + ", " + f.desc_municipio,
                                           tecnico = j.name + " " + j.paterno + " " + j.materno,
                                           actividades_realizar = x.actividades_realizar,
                                           cantidad = x.cantidad,
                                           comprobante = x.comprobante,
                                           concepto = x.concepto,
                                           factura = x.factura,
                                           fecha_deposito = x.fecha_deposito,
                                           hora = x.hora,
                                           fecha_visita = x.fecha_visita.ToShortDateString(),
                                           garantia = x.garantia,
                                           pagado = x.pagado,
                                           pago_pendiente = x.pago_pendiente,
                                           terminos_condiciones = x.terminos_condiciones,
                                           id_estatus = x.estatus,
                                           estatus_visita = (fee == null) ? "" : fee.desc_estatus_visita_en
                                       }).Take(3).ToList(),
                            direcciones = (from z in _context.Cat_Direccion
                                           join b in _context.Cat_Estado on z.id_estado equals b.id
                                           join c in _context.Cat_Municipio on z.id_municipio equals c.id
                                           join d in _context.Cat_Localidad on Convert.ToInt32(z.colonia) equals d.id
                                           where z.id_cliente == a.id
                                           orderby z.creado descending
                                           select new
                                           {
                                               id = z.id,
                                               calle_numero = z.calle_numero,
                                               numExt = z.numExt == null ? "" : z.numExt,
                                               numInt = z.numInt == null ? "" : z.numInt,
                                               cp = z.cp,
                                               estado = b.desc_estado,
                                               id_estado = b.id,
                                               municipio = c.desc_municipio,
                                               id_municipio = c.id,
                                               colonia = d.desc_localidad,
                                               telefono = z.telefono,
                                               d.desc_localidad,
                                               id_localidad = z.id_localidad
                                           }).ToList(),
                            productos = (from x in _context.Cliente_Productos
                                         join prod in _context.Cat_Productos on x.Id_Producto equals prod.id
                                         where x.Id_Cliente == a.id
                                         select new
                                         {
                                             id = x.Id_Producto,
                                             sku = prod.sku,
                                             modelo = prod.modelo,
                                             nombre_prodcuto = prod.nombre,
                                             prod.descripcion_corta,
                                             prod.descripcion_larga,
                                             prod.atributos,
                                             //precio_con_iva = prod.precio_con_iva == null ? "" : prod.precio_con_iva,
                                             //precio_sin_iva = prod.precio_sin_iva.ToString(),
                                             //prod.linea,
                                             //prod.id_linea,
                                             prod.id_sublinea,
                                             //prod.sublinea,
                                             prod.ficha_tecnica,
                                             prod.horas_tecnico,
                                             prod.precio_hora

                                         }).ToList(),
                            datos_fiscales = (from df in _context.DatosFiscales
                                              join b in _context.Cat_Estado on df.id_estado equals b.id
                                              join c in _context.Cat_Municipio on df.id_municipio equals c.id
                                              where df.id_cliente == a.id
                                              select new
                                              {
                                                  df.id,
                                                  df.nombre_fact,
                                                  df.razon_social,
                                                  df.rfc,
                                                  df.email,
                                                  calle_numero = df.calle_numero,
                                                  df.cp,
                                                  df.id_estado,
                                                  estado = b.desc_estado,
                                                  df.id_municipio,
                                                  municipio = c.desc_municipio,
                                                  df.colonia,
                                                  Ext_fact = df.Ext_fact == null ? "" : df.Ext_fact,
                                                  Int_fact = df.Int_fact == null ? "" : df.Int_fact,
                                                  df.telefono_fact
                                              }).ToList()
                        }).ToList();

            if (item.Count == 0)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        [HttpGet("Cliente/{cliente}")]
        public IActionResult GetByCliente(int cliente)
        {
            try
            {
                var item = _context.Clientes.FirstOrDefault(t => t.id == cliente);

                if (item == null)
                {
                    return NotFound();
                }

                return Ok(item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }

        // POST: api/Clientes/ProductosBycliente
        [Route("ProductosBycliente")]
        [HttpPost("{ProductosBycliente}")]
        public IActionResult ProductosBycliente([FromBody] clienteid id)
        {
            var item = (from a in _context.Servicio
                        join b in _context.Cat_Categoria_Servicio on a.id_categoria_servicio equals b.id
                        join c in _context.Clientes on a.id_cliente equals c.id
                        join g in _context.Cat_distribuidor_autorizado on a.id_distribuidor_autorizado equals g.id into es
                        from fd in es.DefaultIfEmpty()
                        join h in _context.Cat_solicitado_por on a.id_solicitado_por equals h.id
                        join hh in _context.Cat_solicitud_via on a.id_solicitud_via equals hh.id
                        join j in _context.Cat_tipo_servicio on a.id_tipo_servicio equals j.id
                        join k in _context.Cat_estatus_servicio on a.id_estatus_servicio equals k.id into tu
                        from fe in tu.DefaultIfEmpty()
                        where c.id == id.id
                        select new
                        {
                            id = a.id,
                            id_cliente = c.id,
                            cliente = c.nombre + " " + c.paterno + " " + c.materno,
                            cliente_telefono = c.telefono,
                            cliente_movil = c.telefono_movil,
                            categoria_servicio = b.desc_categoria_servicio,
                            distribuidor_autorizado = (fd == null) ? "" : fd.desc_distribuidor,
                            solicitado_por = h.desc_solicitado_por,
                            solicitud_via = hh.desc_solicitud_via,
                            id_tipo_servicio = j.id,
                            tipo_servicio = j.desc_tipo_servicio,
                            id_estatus = fe == null ? 0 : fe.id,
                            estatus_servicio = (fe == null) ? "" : fe.desc_estatus_servicio,
                            contacto = a.contacto,
                            IBS = a.IBS,
                            actividades_realizar = a.descripcion_actividades,
                            productos = (from x in _context.Rel_servicio_producto
                                         join xx in _context.Visita on x.id_vista equals xx.id
                                         join ser in _context.Servicio on xx.id_servicio equals ser.id
                                         join pro in _context.Clientes on ser.id_cliente equals pro.id
                                         join e in _context.Cat_Productos on x.id_producto equals e.id
                                         join f in _context.Cat_SubLinea_Producto on e.id_sublinea equals f.id
                                         join g in _context.Cat_Linea_Producto on e.id_linea equals g.id
                                         join h in _context.Cat_SubLinea_Producto on e.id_sublinea equals h.id
                                         join estatus in _context.CatEstatus_Producto on x.estatus equals estatus.id into t
                                         from e_estatus in t.DefaultIfEmpty()
                                         where pro.id == id.id
                                         select new
                                         {
                                             id = e.id,
                                             sku = e.sku,
                                             modelo = e.modelo,
                                             nombre = e.nombre,
                                             id_estatus = e_estatus.id,
                                             estatus = e_estatus.desc_estatus_producto,
                                             descripcion_corta = e.descripcion_corta,
                                             descripcion_larga = e.descripcion_larga,
                                             atributos = e.atributos,
                                             precio_sin_iva = e.precio_sin_iva,
                                             precio_con_iva = e.precio_con_iva,
                                             categoria = f.descripcion,
                                             linea = g.descripcion,
                                             sublinea = h.descripcion,
                                             ficha_tecnica = e.ficha_tecnica,
                                             poliza = "N/A",
                                             garantia = "12/03/2020"
                                         }).ToList()
                        }).ToList();

            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST: api/Clientes/Nuevo_cliente
        [Route("Nuevo_cliente")]
        [HttpPost("{Nuevo_cliente}")]
        public IActionResult Nuevo_cliente([FromBody]Clientes item)
        {
            IActionResult response;
            ServiciosController ser = new ServiciosController(_converter, _config, _emailRepository, _context);
            try
            {
                if (item == null)
                {
                    response = Ok(new { id = "0", response = "Error, intenta mas tarde" });
                }
                else
                {
                    _context.Clientes.Add(item);
                    _context.SaveChanges();

                    var cliente = _context.Clientes.FirstOrDefault(t => t.id == item.id);
                    if (cliente != null)
                    {
                        if (cliente != null)
                        {
                            //ser.CreatePDFOordenServicio(Convert.ToInt64(cliente.servicio[0].id), Convert.ToInt64(cliente.servicio[0].visita[0].id));
                            //CreatePDFOordenServicio(Convert.ToInt64(cliente.servicio[0].id), Convert.ToInt64(cliente.servicio[0].visita[0].id));
                        }
                    }
                    var ordenes = (from a in _context.Servicio
                                   join b in _context.Visita on a.id equals b.id_servicio
                                   where a.id_cliente == item.id
                                   select new
                                   {
                                       id = a.id,
                                       id_visita = b.id,
                                       id_direccion = (from c in _context.Cat_Direccion
                                                       where c.id_cliente == item.id
                                                       select new
                                                       {
                                                           c.id
                                                       }).Take(1).ToList()
                                   }).ToList();

                    response = Ok(new { ordenes, response = "Cliente creado correctamente", id = cliente.id });
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { id = "0", response = ex.ToString() });
            }
            return new ObjectResult(response);
        }

        // POST: api/Clientes/Nuevo_cliente
        [Route("Editar_numero_cliente")]
        [HttpPost("{Editar_numero_cliente}")]
        public IActionResult Editar_numero_cliente([FromBody]Clientes item)
        {
            IActionResult response;

            try
            {
                var cliente = _context.Clientes.FirstOrDefault(t => t.id == item.id);
                if (cliente == null)
                {
                    return NotFound();
                }
                else
                {
                    cliente.folio = item.folio;
                    _context.Clientes.Update(cliente);
                    _context.SaveChanges();

                    response = Ok(new { response = "OK" });
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { response = ex.ToString()});
            }
            return new ObjectResult(response);
        }

        [Route("Editar_cliente")]
        [HttpPost("{Editar_cliente}")]
        public IActionResult Editar_cliente([FromBody]Clientes item)
        {
            IActionResult response;

            try
            {
                var cliente = _context.Clientes.FirstOrDefault(t => t.id == item.id);
                if (cliente == null)
                {
                    return NotFound();
                }
                else
                {
                    cliente.nombre = item.nombre ;
                    cliente.paterno = item.paterno;
                    cliente.materno = item.materno;
                    cliente.telefono = item.telefono;
                    cliente.telefono_movil = item.telefono_movil;
                    cliente.email = item.email;
                    cliente.nombre_comercial = item.nombre_comercial;
                    cliente.folio = item.folio;
                    _context.Clientes.Update(cliente);
                    _context.SaveChanges();

                    response = Ok(new { response = "OK" });
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { response = ex.ToString() });
            }
            return new ObjectResult(response);
        }

        // POST: api/Clientes
        [Route("Busqueda")]
        [HttpPost]
        public IActionResult Post([FromBody] busqueda texto)
        {
            IActionResult response;
            var ordenes = (from a in _context.Clientes
                           join z in _context.Cat_Direccion on a.id equals z.id_cliente into _direccion
                           from direccion in _direccion.DefaultIfEmpty()
                           join b in _context.Cat_Estado on direccion.id_estado equals b.id into _estado
                           from estado in _estado.DefaultIfEmpty()
                           join c in _context.Cat_Municipio on direccion.id_municipio equals c.id into _municipio
                           from municipio in _municipio.DefaultIfEmpty()
                           join d in _context.Cliente_Productos on a.id equals d.Id_Cliente 
                           where EF.Functions.Like(a.folio, "%" + texto.texto + "%") && a.tipo_cliente == 1 && direccion.tipo_direccion == 1
                           select new
                           {
                               id = a.id,
                               folio = a.folio,
                               direccion.tipo_direccion,
                               persona = a.tipo_persona == "Persona fisica" ? (a.nombre + " " + a.paterno + " " + (a.materno == null ? "" : a.materno)) : a.nombre_comercial,
                               direccion = direccion.calle_numero + ", " + direccion.colonia + ", " + direccion.cp + ", " + estado.desc_estado + ", " + municipio.desc_municipio
                           }).Distinct().ToList();

            var telefonos = (from a in _context.Clientes
                             join d in _context.Cliente_Productos on a.id equals d.Id_Cliente
                             join z in _context.Cat_Direccion on a.id equals z.id_cliente into _direccion
                             from direccion in _direccion.DefaultIfEmpty()
                             join b in _context.Cat_Estado on direccion.id_estado equals b.id into _estado
                             from estado in _estado.DefaultIfEmpty()
                             join c in _context.Cat_Municipio on direccion.id_municipio equals c.id into _municipio
                             from municipio in _municipio.DefaultIfEmpty()
                             join e in _context.Cliente_Productos on a.id equals e.Id_Cliente
                             where EF.Functions.Like(a.telefono + a.telefono_movil, "%" + texto.texto + "%") && a.tipo_cliente == 1 && direccion.tipo_direccion == 1
                             select new
                             {
                                 id = a.id,
                                 folio = a.folio,
                                 direccion = direccion.calle_numero + ", " + direccion.colonia + ", " + direccion.cp + ", " + estado.desc_estado + ", " + municipio.desc_municipio,
                                 direccion.tipo_direccion,
                                 telefono = a.telefono,
                                 movil = a.telefono_movil,
                                 persona = a.tipo_persona == "Persona fisica" ? (a.nombre + " " + a.paterno + " " + (a.materno == null ? "" : a.materno)) : a.nombre_comercial
                             }).Distinct().ToList();

            var direcciones = (from a in _context.Clientes
                               join z in _context.Cat_Direccion on a.id equals z.id_cliente into _direccion
                               from direccion in _direccion.DefaultIfEmpty()
                               join b in _context.Cat_Estado on direccion.id_estado equals b.id into _estado
                               from estado in _estado.DefaultIfEmpty()
                               join c in _context.Cat_Municipio on direccion.id_municipio equals c.id into _municipio
                               from municipio in _municipio.DefaultIfEmpty()
                               join d in _context.Cliente_Productos on a.id equals d.Id_Cliente
                               where EF.Functions.Like(direccion.calle_numero + direccion.colonia + direccion.cp + estado.desc_estado + municipio.desc_municipio, "%" + texto.texto + "%") && a.tipo_cliente == 1
                               && direccion.tipo_direccion == 1
                               select new
                               {
                                   id = a.id,
                                   folio = a.folio,
                                   direccion.tipo_direccion,
                                   persona = a.tipo_persona == "Persona fisica" ? (a.nombre + " " + a.paterno + " " + (a.materno == null ? "" : a.materno)) : a.nombre_comercial,
                                   direccion = direccion.calle_numero + ", " + direccion.colonia + ", " + direccion.cp + ", " + estado.desc_estado + ", " + municipio.desc_municipio
                               }).Distinct().ToList();

            var personas = (from a in _context.Clientes
                            join z in _context.Cat_Direccion on a.id equals z.id_cliente into _direccion
                            from direccion in _direccion.DefaultIfEmpty()
                            join b in _context.Cat_Estado on direccion.id_estado equals b.id into _estado
                            from estado in _estado.DefaultIfEmpty()
                            join c in _context.Cat_Municipio on direccion.id_municipio equals c.id into _municipio
                            from municipio in _municipio.DefaultIfEmpty()
                            join d in _context.Cat_Localidad on direccion.id_localidad equals d.id into _localidad
                            from localidad in _localidad.DefaultIfEmpty()
                            join e in _context.Cliente_Productos on a.id equals e.Id_Cliente into _cliente_producto
                            from cliente_producto in _cliente_producto.DefaultIfEmpty()
                                //where EF.Functions.Like(a.nombre + "" + a.paterno + "" + a.materno + "" + a.nombre_comercial + "" + a.nombre_contacto, "%"+texto.texto+"%")
                            where (a.nombre + a.paterno).Contains(texto.texto) && a.tipo_cliente == 1 && direccion.tipo_direccion == 1
                            select new
                            {
                                id = a.id,
                                folio = a.folio,
                                direccion.tipo_direccion,
                                persona = a.tipo_persona == "Persona fisica" ? (a.nombre + " " + a.paterno + " " + (a.materno == null ? "" : a.materno)) : a.nombre_comercial,
                                direccion = direccion.calle_numero + " " + direccion.numExt + ", " + localidad.desc_localidad + ", " + direccion.cp + ", " + estado.desc_estado + ", " + municipio.desc_municipio
                            }).Distinct().ToList();


            //if (ordenes.Count() != 0) {
            //    if (ordenes[0].direccion != null)
            //    {
            //        ordenes = ordenes.Where(x => x.tipo_direccion == 1).ToList();
            //    }
            //}
       
            //if(telefonos.Count() != 0){
            //    if (telefonos[0].direccion != null)
            //    {
            //        telefonos = telefonos.Where(x => x.tipo_direccion == 1).ToList();
            //    }
            //}

            //if (direcciones.Count() != 0) {
            //    if (direcciones[0].direccion != null)
            //    {
            //        direcciones = direcciones.Where(x => x.tipo_direccion == 1).ToList();
            //    }
            //}

            //if (personas.Count() != 0) {
            //    if (personas[0].direccion != null)
            //    {
            //        personas = personas.Where(x => x.tipo_direccion == 1).ToList();
            //    }
            //}

            response = Ok(new { ordenes, telefonos, personas, direcciones });

            return new ObjectResult(response);
        }

        // POST: api/Servicios/Busqueda_Cliente
        [Route("Busqueda_Cliente")]
        [HttpPost("{Busqueda_Cliente}")]
        public IActionResult Busqueda_Cliente([FromBody] busqueda busqueda)
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
                    var dire = _context.Cat_Direccion.FirstOrDefault(a => a.id_cliente == 10180);
                    var dire1 = _context.Cat_Direccion.FirstOrDefault(a => a.id_cliente == 10180 && a.tipo_direccion ==1);
                    var item = (from a in _context.Clientes
                                join b in _context.Cat_Direccion on a.id equals b.id_cliente
                                join c in _context.Cat_Estado on b.id_estado equals c.id
                                join d in _context.Cat_Municipio on b.id_municipio equals d.id
                                where EF.Functions.Like(a.nombre + ' ' + a.paterno + ' ' + a.materno + a.nombre_comercial + a.email + b.calle_numero + b.colonia + b.cp + c.desc_estado + d.desc_municipio, "%" + busqueda.texto + "%")
                                && b.tipo_direccion == 1
                                orderby a.creado descending
                                select new
                                {
                                    a.id,
                                    nombre = a.nombre + ' ' + a.paterno + ' ' + (a.materno == null ? "" : a.materno),
                                    name = a.nombre,
                                    paterno = a.paterno,
                                    materno = a.materno,
                                    a.tipo_persona,
                                    a.email,
                                    a.telefono,
                                    a.telefono_movil,
                                    dire = b.id,
                                    direccion = b.calle_numero + ", " + c.desc_estado + ", " + d.desc_municipio + ", " + b.cp,
                                    fecha_alta = a.creado.ToShortDateString(),
                                    referidopor = (/*a.referidopor == 0 ? "" :*/  _context.Cat_Cuentas.FirstOrDefault(z => z.Id == _context.Cat_Sucursales.FirstOrDefault(e => e.Id == a.Id_sucursal).Id_Cuenta).Cuenta_es  + " - " + _context.Cat_Sucursales.FirstOrDefault(e => e.Id == a.Id_sucursal).Sucursal),
                                    vigencia = (a.vigencia_ref.Year == 0001 ? "No Referido" : a.vigencia_ref.ToString("dd/MM/yyyy"))
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

        // POST: api/Servicios/Busqueda_Cliente
        [Route("Busqueda_Cliente_tickets")]
        [HttpPost("{Busqueda_Cliente_tickets}")]
        public IActionResult Busqueda_Cliente_tickets([FromBody] busqueda busqueda)
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
                    var item = (from a in _context.Clientes
                                join z in _context.Cat_Direccion on a.id equals z.id_cliente into _direccion
                                from direccion in _direccion.DefaultIfEmpty()
                                join b in _context.Cat_Estado on direccion.id_estado equals b.id into _estado
                                from estado in _estado.DefaultIfEmpty()
                                join c in _context.Cat_Municipio on direccion.id_municipio equals c.id into _municipio
                                from municipio in _municipio.DefaultIfEmpty()
                                join d in _context.Cat_Localidad on direccion.id_localidad equals d.id into _localidad
                                from localidad in _localidad.DefaultIfEmpty()
                                join e in _context.Cliente_Productos on a.id equals e.Id_Cliente into _Cliente_Productos
                                from Cliente_Productos in _Cliente_Productos.DefaultIfEmpty()
                                where a.tipo_cliente == 1
                                select new
                                {
                                    id = a.id,
                                    folio = a.folio,
                                    nombre = (a.nombre + " " + a.paterno + " " + (a.materno == null ? "" : a.materno)),
                                    a.nombre_comercial,
                                    a.email,
                                    a.telefono,
                                    direccion.tipo_direccion,
                                    direccion = direccion.calle_numero + " " + direccion.numExt + ", " + localidad.desc_localidad + ", " + direccion.cp + ", " + estado.desc_estado + ", " + municipio.desc_municipio
                                }).Distinct().ToList();

                    if (busqueda.texto != "") {
                        item = item.Where(x => EF.Functions.Like(x.nombre + x.nombre_comercial + x.email + x.direccion, "%" + busqueda.texto + "%")).ToList();
                    }

                    if (item.Count != 0) {
                        if (item[0].direccion != null)
                        {
                            item = item.Where(x => x.tipo_direccion == 1).ToList();
                        }

                    }

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

        // POST: api/Servicios/Busqueda_Cliente
        [Route("Busqueda_Cliente_nombre")]
        [HttpPost("Busqueda_Cliente_nombre/{id_usuario}")]
        public IActionResult Busqueda_Cliente_nombre([FromBody] busqueda busqueda, int id_usuario)
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
                    var suc_usr = _context.Users.FirstOrDefault(u => u.id == id_usuario);
                    int busq1;
                    if (suc_usr != null)
                        busq1 = suc_usr.id_Sucursales;
                    else busq1 = 0;
                    var item = (from a in _context.Clientes
                                where EF.Functions.Like(a.nombre + ' ' + a.paterno + ' ' + a.materno + ' ' + a.nombre_comercial , "%" + busqueda.texto + "%")
                                orderby a.nombre + ' ' + a.paterno + ' ' + a.materno descending
                                select new
                                {
                                    a.id,
                                    nombre = a.nombre + ' ' + a.paterno + ' ' + a.materno,
                                    name = a.nombre,
                                    paterno = a.paterno,
                                    materno = a.materno,
                                    a.tipo_persona,
                                    email = (a.Id_sucursal == busq1 ? a.email: "******"),
                                    telefono = (a.Id_sucursal == busq1 ? a.telefono : "******"),
                                    telefono_movil = (a.Id_sucursal == busq1 ? a.telefono_movil : "******"),
                                    fecha_alta = a.creado.ToString("dd/MM/yyyy"),
                                    referidopor = (_context.Cat_Cuentas.FirstOrDefault(z => z.Id == _context.Cat_Sucursales.FirstOrDefault(e => e.Id == a.Id_sucursal).Id_Cuenta).Cuenta_es + " - " + _context.Cat_Sucursales.FirstOrDefault(e => e.Id == a.Id_sucursal).Sucursal),
                                    vigencia = (a.referidopor == 0 ? "No Referido" : a.vigencia_ref.ToString("dd/MM/yyyy"))
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

        // POST: api/Servicios/Busqueda_Cliente
        [Route("Busqueda_Tecnico")]
        [HttpPost("{Busqueda_Tecnico}")]
        public IActionResult Busqueda_Tecnico([FromBody] busqueda busqueda)
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
                    var item = (from a in _context.Tecnicos
                                join b in _context.Users on a.id equals b.id
                                where EF.Functions.Like(b.name + ' ' + b.paterno + ' ' + b.materno + b.email + ' ' + a.noalmacen, "%" + busqueda.texto + "%")
                                orderby a.creado descending
                                select new
                                {
                                    a.id,
                                    nombre = b.name + ' ' + b.paterno + ' ' + b.materno,
                                    b.email,
                                    a.noalmacen,
                                    b.telefono,
                                    b.telefono_movil
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

        [Route("Busqueda_producto")]
        [HttpPost("{Busqueda_producto}")]
        public IActionResult Busqueda_producto([FromBody] busqueda_productos texto)
        {
            var item = (from a in _context.Cat_Productos
                        //join b in _context.Cat_SubLinea_Producto on a.id_sublinea equals b.id
                        //join c in _context.Rel_Categoria_Producto_Tipo_Producto on b.id equals c.id_categoria 
                        where EF.Functions.Like(a.modelo + ' ' + a.nombre, "%" + texto.texto + "%")
                        select new
                        {
                            id = a.id,
                            modelo = a.modelo,
                            sku = a.sku,
                            tipo = a.nombre,
                        }).Take(5).Distinct().ToList();

            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/Historial_cliente
        [Route("Historial_cliente")]
        [HttpPost("{Historial_cliente}")]
        public IActionResult Historial_cliente([FromBody] busqueda busqueda)
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
                    var item = (from a in _context.Clientes
                                join b in _context.Servicio on a.id equals b.id_cliente
                                join bb in _context.Cat_estatus_servicio on b.id_estatus_servicio equals bb.id 
                                join c in _context.Visita on b.id equals c.id_servicio into _visita
                                from Visita in _visita.DefaultIfEmpty()
                                //join d in _context.Rel_servicio_producto on c.id equals d.id_vista
                                where a.id == Convert.ToInt32(busqueda.texto)
                                orderby a.creado descending
                                select new
                                {
                                    a.id,
                                    id_servicio = b.id,
                                    fecha_servicio = b.fecha_servicio.ToShortDateString(),
                                    b.id_estatus_servicio,
                                    bb.desc_estatus_servicio
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

        // PUT: api/Clientes/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        public string TemplateOrdenServicioPDF(long id, long id_visita)
        {

            var selRuta = _context.parametro_Archivos.FirstOrDefault(p => p.id == 6);
            string path;
            if (selRuta == null) path = "/Imagenes/";
            else
            {
                path = selRuta.ruta;
            }

            var item = (from a in _context.Servicio
                        join b in _context.Cat_Categoria_Servicio on a.id_categoria_servicio equals b.id into _cat_servicio
                        from cat_servicio in _cat_servicio.DefaultIfEmpty()
                        join c in _context.Clientes on a.id_cliente equals c.id
                        join g in _context.Cat_distribuidor_autorizado on a.id_distribuidor_autorizado equals g.id into es
                        from fd in es.DefaultIfEmpty()
                        join h in _context.Cat_solicitado_por on a.id_solicitado_por equals h.id
                        join hh in _context.Cat_solicitud_via on a.id_solicitud_via equals hh.id
                        join j in _context.Cat_tipo_servicio on a.id_tipo_servicio equals j.id
                        join sub in _context.Sub_cat_tipo_servicio on a.id_sub_tipo_servicio equals sub.id
                        join k in _context.Cat_estatus_servicio on a.id_estatus_servicio equals k.id into tu
                        from fe in tu.DefaultIfEmpty()
                        where a.id == id
                        select new
                        {
                            id = a.id,
                            id_cliente = c.id,
                            cliente = c.nombre + " " + c.paterno + " " + c.materno,
                            cliente_telefono = c.telefono,
                            cliente_movil = c.telefono_movil,
                            c.email,
                            categoria_servicio = (cat_servicio == null) ? "" : cat_servicio.desc_categoria_servicio,
                            distribuidor_autorizado = (fd == null) ? "" : fd.desc_distribuidor,
                            solicitado_por = h.desc_solicitado_por,
                            solicitud_via = hh.desc_solicitud_via,
                            id_tipo_servicio = j.id,
                            tipo_servicio = j.desc_tipo_servicio,
                            sub_tipo_servicio = sub.sub_desc_tipo_servicio,
                            id_estatus = fe == null ? 0 : fe.id,
                            estatus_servicio = (fe == null) ? "" : fe.desc_estatus_servicio,
                            contacto = a.contacto,
                            IBS = a.IBS,
                            actividades_realizar = a.descripcion_actividades,
                            a.descripcion_actividades,
                            piezas_cotizacion = (from y in _context.Piezas_Repuesto
                                                 join pc in _context.Rel_servicio_Refaccion on y.id_rel_servicio_refaccion equals pc.id
                                                 join u in _context.Cat_Materiales on y.id_material equals u.id
                                                 join pre in _context.Cat_Lista_Precios on u.id_grupo_precio equals Convert.ToInt32(pre.grupo_precio) //into g
                                                 where pc.id_vista == id_visita
                                                 select new
                                                 {
                                                     id = y.id,
                                                     refaccion = u.descripcion,
                                                     cantidad = y.cantidad,
                                                     precio_sin_iva = pre.precio_sin_iva * y.cantidad,
                                                     total_cantidad = (from a in _context.Piezas_Repuesto
                                                                       join b in _context.Rel_servicio_Refaccion on a.id_rel_servicio_refaccion equals b.id
                                                                       where b.id_vista == id_visita
                                                                       select a.cantidad).Sum(),
                                                     total_precio = (from a in _context.Piezas_Repuesto
                                                                     join aa in _context.Rel_servicio_Refaccion on a.id_rel_servicio_refaccion equals aa.id
                                                                     join b in _context.Cat_Materiales on a.id_material equals b.id
                                                                     join c in _context.Cat_Lista_Precios on b.id_grupo_precio equals Convert.ToInt32(c.grupo_precio)
                                                                     where aa.id_vista == id_visita
                                                                     select c.precio_sin_iva).Sum()
                                                 }).ToList(),
                            visitas = (from x in _context.Visita
                                       join d in _context.Cat_Direccion on x.id_direccion equals d.id
                                       join e in _context.Cat_Estado on d.id_estado equals e.id
                                       join f in _context.Cat_Municipio on d.id_municipio equals f.id
                                       join g in _context.Cat_Localidad on d.id_localidad equals g.id
                                       join kk in _context.CatEstatus_Visita on x.estatus equals kk.id into t
                                       from fee in t.DefaultIfEmpty()
                                       where x.id_servicio == id && x.id == id_visita
                                       select new
                                       {
                                           id = x.id,
                                           direccion = d.calle_numero + ", " + g.desc_localidad + ", " + e.desc_estado + ", " + f.desc_municipio,
                                           calle = d.calle_numero,
                                           colonia = g.desc_localidad,
                                           cp = d.cp,
                                           e.desc_estado,
                                           d.telefono,
                                           d.telefono_movil,
                                           id_direccion = d.id,
                                           id_servicio = x.id_servicio,
                                           fecha_inicio_visita = Convert.ToDateTime(x.fecha_inicio_visita).ToShortDateString(),
                                           x.imagen_firma,
                                           x.hora,
                                           tecnico = (from visita_tecnico in _context.rel_tecnico_visita
                                                          //join tv in _context.rel_tecnico_visita on visita_tecnico.id equals tv.id_vista
                                                      join i in _context.Tecnicos on visita_tecnico.id_tecnico equals i.id
                                                      join j in _context.Users on i.id equals j.id
                                                      where visita_tecnico.id_vista == x.id
                                                      select new
                                                      {
                                                          nombre = j.name + " " + j.paterno + " " + j.materno,
                                                          visita_tecnico.tecnico_responsable
                                                      }),
                                           checklist = (from w in _context.Producto_Check_List_Respuestas
                                                        join prod in _context.Cat_Productos on w.id_producto equals prod.id
                                                        join visita in _context.Visita on w.id_vista equals visita.id
                                                        join ec in _context.CatEstatus_Producto on w.estatus equals ec.id
                                                        where visita.id == id_visita && visita.id_servicio == id
                                                        select new
                                                        {
                                                            id_visita = visita.id,
                                                            id_estatus_prodcuto_checklist = w.estatus,
                                                            estatus_prodcuto_checklist = ec.desc_estatus_producto,
                                                            prod.nombre,
                                                            prod.modelo,
                                                            imagenes_checklist = (from xz in _context.Rel_Imagen_Producto_Visita
                                                                                  where xz.id_visita == id_visita && xz.id_producto == w.id_producto
                                                                                  select new
                                                                                  {
                                                                                      id = xz.id,
                                                                                      url = xz.path,
                                                                                      xz.actividad
                                                                                  }),
                                                            preguntas = (from check in _context.Producto_Check_List_Respuestas
                                                                             //join prod in _context.Cat_Productos on check.id_producto equals prod.id
                                                                         join visita_c in _context.Visita on check.id_vista equals visita_c.id
                                                                         join respuesta in _context.Check_List_Respuestas on check.id equals respuesta.id_producto_check_list_respuestas
                                                                         join pregunta in _context.Check_List_Preguntas on respuesta.id_pregunta equals pregunta.id
                                                                         where visita_c.id == id_visita && visita_c.id_servicio == id
                                                                         select new
                                                                         {
                                                                             pregunta = pregunta.pregunta,
                                                                             pregunta_en = pregunta.pregunta_en,
                                                                             respuesta = respuesta.respuesta,
                                                                             comentario = respuesta.comentarios,
                                                                         })
                                                        }).ToList(),
                                           reporte = (from a in _context.Visita
                                                      join c in _context.Servicio on a.id_servicio equals c.id
                                                      join e in _context.Cat_estatus_servicio on c.id_estatus_servicio equals e.id into es
                                                      from fd in es.DefaultIfEmpty()
                                                      where a.id == x.id
                                                      orderby a.fecha_visita ascending
                                                      select new
                                                      {
                                                          id_servicio = a.id_servicio,
                                                          estatus_servicio = (fd == null) ? "" : fd.desc_estatus_servicio,
                                                          id_visita = a.id,
                                                          productos = (from xe in _context.Rel_servicio_Refaccion
                                                                       join sev in _context.CatEstatus_Producto on xe.estatus equals sev.id
                                                                       join prod in _context.Cat_Productos on xe.id_producto equals prod.id
                                                                       join serie in _context.Rel_servicio_producto on prod.id equals serie.id_producto //&& serie.id_vista equals 
                                                                       where xe.id_vista == id_visita && serie.id_vista == id_visita
                                                                       select new
                                                                       {
                                                                           id = xe.id_producto,
                                                                           nombre_prodcuto = prod.nombre,
                                                                           estatus_servicio = sev.desc_estatus_producto,
                                                                           id_estatus = xe.estatus,
                                                                           //estatus = sev.desc_estatus_producto_en,
                                                                           actividades = xe.actividades,
                                                                           fallas = xe.fallas,
                                                                           serie.no_serie,
                                                                           serie.garantia,
                                                                           prod.modelo,
                                                                           imagenes_reparacion = (from xz in _context.Rel_Imagen_Producto_Visita
                                                                                                  where xz.id_visita == id_visita && xz.id_producto == xe.id_producto
                                                                                                  select new
                                                                                                  {
                                                                                                      id = xz.id,
                                                                                                      url = xz.path,
                                                                                                      xz.actividad
                                                                                                  }).ToList(),
                                                                           piezas_repuesto = (from y in _context.Piezas_Repuesto
                                                                                              join u in _context.Cat_Materiales on y.id_material equals u.id
                                                                                              join o in _context.Cat_Lista_Precios on u.id_grupo_precio equals o.id
                                                                                              where y.id_rel_servicio_refaccion == xe.id
                                                                                              select new
                                                                                              {
                                                                                                  id = y.id,
                                                                                                  refaccion = u.descripcion,
                                                                                                  cantidad = y.cantidad,
                                                                                                  precio = o.precio_sin_iva

                                                                                              }).ToList(),
                                                                           piezas_tecnico = (from y in _context.Piezas_Repuesto_Tecnico
                                                                                             join u in _context.Cat_Materiales on y.id_material equals u.id
                                                                                             where y.id_rel_servicio_refaccion == xe.id
                                                                                             select new
                                                                                             {
                                                                                                 id = y.id,
                                                                                                 refaccion = u.descripcion,
                                                                                                 cantidad = y.cantidad
                                                                                             }).ToList(),
                                                                           mano_de_obra = (from mo in _context.Rel_Categoria_Producto_Tipo_Producto
                                                                                           where mo.id_categoria == prod.id_sublinea && mo.id_tipo_servicio == xe.visita.servicio.id_tipo_servicio
                                                                                           select new
                                                                                           {
                                                                                               mo.precio_hora_tecnico
                                                                                           }).Take(1).ToList()


                                                                       }).ToList()
                                                      }).ToList()
                                       }).Distinct().ToList()
                        }).Distinct().ToList();
            //var todo = DataStorage.
            var sb = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                                <link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css' integrity='sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u' crossorigin='anonymous'>
                            </head>
                            <body>
                                </br>
                                <div class='row'>
                                        <div class='content'>
                                            <div class='col-md-12'>
                                                <table style='width: 100%; text-align: center'>
                                                  <tr>
                                                    <td style='width: 200px'>
                                                        <img heigth='30%' width='30%' src='" + selRuta.funcion + @"/mieletickets/Cierre_Servicio/b8839cc1-5927-4817-ad2f-359fb8f6f86b.jpg'>
                                                    </td>
                                                 </tr>
                                              </table>
                                              <table style='width: 100%; text-align: left'>
                                                  <tr>
                                                    <td>
                                                        <h1>Confirmación de Orden de Servicio
                                                            <span style = 'color:#A5000D'>" + item[0].id); sb.Append(@" </span>
                                                        </h1>
                                                    </td>
                                                 </tr>
                                              </table>
                                            </div>
                                            <div class='col-md-12'>
                                                 <h4>Estimado (a) cliente, por este medio le enviamos la confirmación del servicio solicitado, en caso de alguna duda, error en sus datos o cancelación por favor contactenos al teléfono 01-800 MIELE 00 (01 800 64353 00)</h4>
                                                 <h4><span style = 'color:#A5000D'>Tipo de servicio </span>" + item[0].tipo_servicio);
            sb.Append(@" 
                                                 <h4><span style = 'color:#A5000D'>Fecha de atención: </span>" + item[0].visitas[0].fecha_inicio_visita); sb.Append(@" </h4>
                                                 <h4><span style = 'color:#A5000D'>Hora de visita: </span> Entre " + (Convert.ToInt32(item[0].visitas[0].hora) - 1) + " hrs " + " y " + (Convert.ToInt32(item[0].visitas[0].hora) + 1) + " hrs "); sb.Append(@" </h4>
                                                 <h4><span style = 'color:#A5000D'>Cliente: </span>" + item[0].cliente); sb.Append(@" </h4>                                                
                                            </div>
                                            <div class='col-md-12'>
                                                <table style='width: 100%; text-align: left'>
                                                  <tr>
                                                    <td>
                                                        <span style = 'color:#A5000D'> Calle y número: "); sb.Append(@" </span>
                                                    </td>
                                                    <td>
                                                        <span>" + item[0].visitas[0].calle); sb.Append(@" </span>
                                                    </td>
                                                    <td>
                                                        <span style = 'color:#A5000D'> Teléfono: "); sb.Append(@" </span>
                                                    </td>
                                                    <td>
                                                        <span>" + item[0].cliente_telefono); sb.Append(@" </span>
                                                    </td>
                                                 </tr>
                                                 <tr>
                                                    <td>
                                                        <span style = 'color:#A5000D'> Colonia: "); sb.Append(@" </span>
                                                    </td>
                                                    <td>
                                                        <span>" + item[0].visitas[0].colonia); sb.Append(@" </span>
                                                    </td>
                                                    <td>
                                                        <span style = 'color:#A5000D'> Cel: "); sb.Append(@" </span>
                                                    </td>
                                                    <td>
                                                        <span>" + item[0].cliente_movil); sb.Append(@" </span>
                                                    </td>
                                                 </tr>
                                                <tr>
                                                    <td>
                                                        <span style = 'color:#A5000D'> C.P: "); sb.Append(@" </span>
                                                    </td>
                                                    <td>
                                                        <span>" + item[0].visitas[0].cp); sb.Append(@" </span>
                                                    </td>
                                                    <td>
                                                        <span style = 'color:#A5000D'> Email: "); sb.Append(@" </span>
                                                    </td>
                                                    <td>
                                                        <span>" + item[0].email); sb.Append(@" </span>
                                                    </td>
                                                 </tr>
                                                 <tr>
                                                    <td>
                                                        <span style = 'color:#A5000D'> Estado: "); sb.Append(@" </span>
                                                    </td>
                                                    <td>
                                                        <span>" + item[0].visitas[0].desc_estado); sb.Append(@" </span>
                                                    </td>
                                                    <td>
                                                        <span style = 'color:#A5000D'> Contacto: "); sb.Append(@" </span>
                                                    </td>
                                                    <td>
                                                        <span>" + item[0].contacto); sb.Append(@" </span>
                                                    </td>
                                                 </tr>
                                              </table>
                                             </div>");
            if (item[0].visitas[0].reporte[0].productos.Count() != 0)
            {
                sb.Append(@"
                                                                                                                                    <div class='col-md-12' style='margin-bottom: 20px'>
                                                                                                                                    <h3>Equipos a revisar</h2>
                                                                                                                                    <table style='width: 100%; text-align: left'>");
                foreach (var emp in item[0].visitas[0].reporte[0].productos)
                {
                    sb.Append(@"
                                                                                                                                       <tr>
                                                                                                                                            <td>
                                                                                                                                                  <h4><span style = 'color:#A5000D'>Producto: </span>" + emp.nombre_prodcuto); sb.Append(@" </h4>
                                                                                                                                                  <h4><span style = 'color:#A5000D'>Modelo: </span>" + emp.modelo); sb.Append(@" </h4>
                                                                                                                                                  <h4><span style = 'color:#A5000D'>No. Serie: </span>" + emp.no_serie); sb.Append(@" </h4>
                                                                                                                                                  <h4><span style = 'color:#A5000D'>Garantía: </span>"); if (emp.garantia)
                    {
                        sb.Append(@"Si");
                    }
                    else
                    {
                        sb.Append(@"No");
                    }
                    sb.Append(@" </h4>
                                                                                                                                                  <h4><span style = 'color:#A5000D'>Diagnóstico técnico: </span>" + emp.actividades); sb.Append(@" </h4>
                                                                                                                                           </td></tr></table>");
                    sb.Append(@"<table><tr>");
                    for (var i = 0; i < emp.imagenes_reparacion.Count(); i++)
                    {
                        sb.Append(@"<td>
                                        <div style='height:450px; width:300px;'><img src='" + selRuta.funcion + emp.imagenes_reparacion[i].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[i].actividad + "</span> </div>");
                        sb.Append(@"</td>");

                    }
                    sb.Append(@"</tr></table>");
                }
                sb.Append(@"
                                                                                                                                 </div>");

            }

            sb.Append(@"<hr style='border-color:black 4px;'>
                        <div class='col-md-12'>
                                                                                                    <h4>Consideraciones:</h4>
                                                                                                    <span>- En caso de aplicar garantía, el servicio no tendra costo</span>
                                                                                                    <span>- En costo por visita de servicio fuera de garantía es de $890, no incluye refacciones</span>
                                                                                                    <span>- Las refacciones necesarias serán cotizadas por separado asi como mano de obra</span>
                                                                                                    <span>- Metodo de pago: Efectivo, Tarjeta Bancaria / American Express</span>
                                                                                                     
                                                                                                </div>
                                      </div>
                                 </div>
                            </body>
                        </html>");

            return sb.ToString();
        }

        public string CreatePDFOordenServicio(long id, long id_visita)
        {
            var filePath = Environment.CurrentDirectory;
            var _guid = Guid.NewGuid();
            var _guid_cotizacion = Guid.NewGuid();
            var path = "/Imagenes/pdf_reportes/" + _guid + ".pdf";
            var path_cotizacion = "/Imagenes/pdf_reportes/" + _guid_cotizacion + ".pdf";
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report",
                Out = filePath + path  //USE THIS PROPERTY TO SAVE PDF TO A PROVIDED LOCATION
            };

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,

                HtmlContent = TemplateOrdenServicioPDF(id, id_visita),
                //Page = "https://www.amigosdetechodigital.com.mx//#/", //USE THIS PROPERTY TO GENERATE PDF CONTENT FROM AN HTML PAGE
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Arquimides #43, Col. Polanco Chapultepec, 11560, México, D.F. 01 800 MIELE 00", Right = "Página [page] of [toPage]" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            //_converter.Convert(pdf); IF WE USE Out PROPERTY IN THE GlobalSettings CLASS, THIS IS ENOUGH FOR CONVERSION

            var file = _converter.Convert(pdf);

            var item = (from a in _context.Servicio
                        join b in _context.Clientes on a.id_cliente equals b.id
                        where a.id == id
                        select new
                        {
                            a.id,
                            nombre = b.nombre + " " + b.paterno + " " + b.materno
                        }).ToList();

            var path_template = Path.GetFullPath("TemplateMail/Email_orden_servicio.html");
            StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_orden_servicio.html"));
            string body = string.Empty;
            body = reader.ReadToEnd();
            body = body.Replace("{username}", "Estimado (a) cliente: " + item[0].nombre);
            body = body.Replace("{no_servicio}", item[0].id.ToString());

            //var smtp = new SmtpClient
            //{
            //    Host = "smtp.gmail.com",
            //    Port = 587,
            //    EnableSsl = true,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    UseDefaultCredentials = false,
            //    Credentials = new NetworkCredential("conf modificada", "bostones01")
            //};
            //using (var message = new MailMessage
            //{
            //    From = new MailAddress("no-reply@miele.com.mx", "Miele"),
            //    To = { "linelsk@hotmail.com" },
            //    Subject = "Miele - Confirmación de servicio " + item[0].id,
            //    IsBodyHtml = true,
            //    Body = body

            //})
            //{
            //    var fs = new FileStream(filePath + path, FileMode.Open);
            //    message.Attachments.Add(new Attachment(fs, "Confirmación de orden de Servicio.pdf", "text/pdf"));
            //    smtp.Send(message);
            //}
            var fs = new FileStream(filePath + path, FileMode.Open);
            List<Attachment> adjuntos = new List<Attachment>();
            adjuntos.Add(new Attachment(fs, "Cotización" + id.ToString() + ".pdf", "text/pdf"));
            _emailRepository.SendAttachment("linelsk@hotmail.com", "Miele - Confirmación de servicio " + item[0].id.ToString(), true, body, 1, adjuntos);

            return "Successfully created PDF document.";

            
        }
    }

    public class busqueda
    {
        public string texto { get; set; }
        public productos id { get; set; }
    }

    public class busqueda_productos
    {
        public string texto { get; set; }
        public int id { get; set; }
    }

    public class clienteid
    {
        public int id { get; set; }
    }

    public class cliente_Id
    {
        public int id { get; set; }
    }

    public class productos
    {
        public int id { get; set; }
    }
}
