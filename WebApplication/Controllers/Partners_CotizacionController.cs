using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using WebApplication.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Mail;
using System.Net;
using WebApplication.Repository;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Collections;
using WebApplication.ViewModels;
using WebApplication.Service;

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class Partners_CotizacionController : Controller
    {

        private IConfiguration _config;
        private IEmailRepository _emailRepository;
        private EmailSettings _emailSettings { get; set; }
        private ICotizacionPDF _documentoService;
        private readonly MieleContext _context;

        public Partners_CotizacionController(IConfiguration config, ICotizacionPDF documentoService, MieleContext context,
             IEmailRepository emailRepository,
            IOptions<EmailSettings> emailSettings
            )
        {
            _config = config;
            _emailRepository = emailRepository;
            _emailSettings = emailSettings.Value;
            _documentoService = documentoService;
            _context = context;
        }

        // GET: api/Partners_Cotizacion
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Partners_Cotizacion/5
        [HttpGet("{id}", Name = "Get_Cotizacion")]
        public string Get(int id)
        {
            return "value";
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult BuscarCotizaciones([FromBody]ModeloBusquedaCotRol busqueda)
        {
            var ids_ = (from a in _context.Cotizaciones
                        join z in _context.Cotizacion_Producto on a.Id equals z.Id_Cotizacion
                        join v in _context.Cat_Productos on z.Id_Producto equals v.id
                        where EF.Functions.Like(v.nombre, "%" + busqueda.TextoProd + "%") && EF.Functions.Like(v.modelo, "%" + busqueda.modelo + "%")
                        select new { id = a.Id }).ToList();

            var U_session = (from a in _context.Users
                             where a.id == busqueda.Id_user
                             select new { id_canal = a.id_canal, id_cuenta = a.id_cuenta, Id_sucursal = a.id_Sucursales, id_rol = a.id_rol, nivel = a.nivel, id = a.id }).ToList();

            if (busqueda.FechaFin == "" || busqueda.FechaFin == null) busqueda.FechaFin = "01/01/2050";
            if (busqueda.FechaIni == "" || busqueda.FechaIni == null) busqueda.FechaIni = "01/01/1900";
            DateTime fI = Convert.ToDateTime(busqueda.FechaFin);
            IActionResult response = Unauthorized();
            long id_nivel = 0;
            switch (U_session[0].nivel) { case "sucursal": id_nivel = 1; break; case "cuenta": id_nivel = 2; break; case "canal": id_nivel = 3; break; case "global": id_nivel = -1; break; }
            var item = (from a in _context.Cotizaciones
                        join b in _context.Clientes on a.Id_Cliente equals b.id
                        join c in _context.Users on a.Id_Vendedor equals c.id
                        join e in _context.Cat_Cuentas on a.Id_Cuenta equals e.Id
                        join r in _context.Cat_Sucursales on a.Id_sucursal equals r.Id
                        join m in _context.Cat_Estatus_Cotizacion on a.Estatus equals m.id
                        orderby a.Numero descending
                        where Convert.ToDateTime(busqueda.FechaFin).Date >= a.fecha_cotiza.Date
                        && Convert.ToDateTime(busqueda.FechaIni).Date <= a.fecha_cotiza.Date
                        && EF.Functions.Like(b.nombre + b.paterno + b.materno + a.Numero + a.ibs + c.name + c.paterno + c.materno, "%" + busqueda.TextoLibre + "%")
                        && a.Estatus == (busqueda.Estatus == 0 ? a.Estatus : busqueda.Estatus)
                        && ids_.Any(prods => prods.id == a.Id)
                        && a.Id_Canal == (busqueda.Canal == 0 ? a.Id_Canal : busqueda.Canal)
                        && a.Id_Cuenta == (busqueda.id_Cuenta == 0 ? a.Id_Cuenta : busqueda.id_Cuenta)
                        && a.Id_sucursal == (busqueda.Id_sucursal == 0 ? a.Id_sucursal : busqueda.Id_sucursal)
                        && a.Id_Canal == (id_nivel == 3 ? U_session[0].id_canal : a.Id_Canal)
                        && a.Id_Cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : a.Id_Cuenta)
                        && a.Id_sucursal == (id_nivel == 1 ? U_session[0].Id_sucursal : a.Id_sucursal)
                        // && a.Id_Vendedor == (U_session[0].id_rol == 10004 ? U_session[0].id : a.Id_Vendedor) no estan limitados solo a las suyas
                        && a.cancelada == (busqueda.canceladas ? a.cancelada : false)
                        && a.rechazada == (busqueda.rechazadas ? a.rechazada : false)
                        && a.entrega_sol == (busqueda.solicitudes ? a.entrega_sol : false)
                        && a.Estatus > 0
                        select new
                        {
                            id = a.Id,
                            folio = a.Numero,
                            cuenta = e.Cuenta_es,
                            //sucursales_aplicables = _context.Cat_Sucursales.Where(s => Suc_resultantes.Any(sr => sr.Id == s.Id))
                            // id_canal = e.Id_Canal, 
                            canal = _context.Cat_canales.FirstOrDefault(c => c.Id == e.Id_Canal).Canal_es,
                            id_canal = _context.Cat_canales.FirstOrDefault(c => c.Id == e.Id_Canal).Id,

                            sucursal = r.Sucursal,
                            cliente = b.nombre + " " + b.paterno + " " + b.materno,
                            estatus = a.Estatus,
                            fecha = a.fecha_cotiza.ToString("MM/dd/yyyy"),
                            vendedor = c.name + " " + c.paterno + " " + c.materno,
                            importe = (a.importe_precio_lista + a.iva_precio_lista).ToString("C"),
                            importe_c = (a.importe_condiciones_com + a.iva_condiciones_com).ToString("C"),
                            acciones = a.Acciones,
                            correo = b.email,
                            rechazada = a.rechazada,
                            cancelada = a.cancelada,
                            estatus_desc = m.Estatus_es,
                            cambio_ord_comp_generada = (a.cambio_ord_comp_generada.Year < 2000 ? "" : a.cambio_ord_comp_generada.ToString("MM/dd/yyyy")),
                            ibs = a.ibs,
                            nprod = (from x in _context.Cotizacion_Producto
                                     where x.Id_Cotizacion == a.Id
                                     select x.cantidad).Sum(),
                            id_cotizacion_padre = a.id_cotizacion_padre,
                            entrega_sol = a.entrega_sol,
                            puede_solicitar_env = (id_nivel > 0 ? a.puede_solicitar_env : 0)
                        }).ToList();

            var query = item.GroupBy(x => x.id_cotizacion_padre).Select(group => group.Where(x => x.estatus == group.Max(y => y.estatus)).First());
            if (!busqueda.duplicadas)
                item = query.ToList();


            if (item == null)
            {
                return response = Ok(new { token = "Error", item });
            }
            return response = Ok(new { token = "Success", item });
        }

        // POST: api/Partners_Cotizacion/Sepomex
        [Route("Sepomex")]
        [HttpPost("{Sepomex}")]
        public IActionResult Sepomex([FromBody] busquedaLibre item)
        {
            IActionResult response = Unauthorized();
            var cp_ = Convert.ToInt32(item.TextoLibre);
            var _item = (from a in _context.Cat_Localidad
                         join b in _context.Cat_Municipio on a.municipio.id equals b.id
                         join c in _context.Cat_Estado on b.estado.id equals c.id
                         where a.cp == cp_ && a.estatus == true
                         select new
                         {
                             cp = a.cp,
                             estado = c.desc_estado,
                             id_estado = c.id,
                             localidades = (from al in _context.Cat_Localidad
                                            join bl in _context.Cat_Municipio on al.municipio.id equals bl.id
                                            join cl in _context.Cat_Estado on bl.estado.id equals cl.id
                                            where al.cp == a.cp && al.estatus == true
                                            select new
                                            {
                                                id_localidad = al.id,
                                                localidad = al.desc_localidad
                                            }).Distinct().ToList(),
                             municipios = (from al in _context.Cat_Localidad
                                           join bl in _context.Cat_Municipio on al.municipio.id equals bl.id
                                           join cl in _context.Cat_Estado on bl.estado.id equals cl.id
                                           where al.cp == a.cp && al.estatus == true
                                           select new
                                           {
                                               municipio = bl.desc_municipio,
                                               id_municipio = bl.id,
                                           }).Distinct().ToList()
                         }).Distinct().Take(1).ToList();
            if (_item.Count == 0)
            {
                return response = Ok(new { resultado = "Error", _item });
            }
            return response = Ok(new { resultado = "Success", _item });
            // return new ObjectResult(_item);
        }

        // POST: api/Servicios
        [Route("GetDetalleCotizacion")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult GetDetalleCotizacion([FromBody]ModeloBusquedaCotID busqueda)
        {
            IActionResult response = Unauthorized();
            var item = (from a in _context.Cotizaciones
                        join b in _context.Clientes on a.Id_Cliente equals b.id
                        join c in _context.Vendedores on a.Id_Vendedor equals c.Id
                        orderby a.fecha_cotiza ascending
                        where a.Id == busqueda.Id
                        select new
                        {
                            id = a.Id,
                            folio = a.Id,
                            cuenta = a.Id_Cuenta,
                            clientecomp = b.nombre + " " + b.paterno + " " + b.materno,
                            clinombre = b.nombre,
                            clipat = b.paterno,
                            climat = b.materno,
                            estatus = a.Estatus,
                            fecha = a.fecha_cotiza.ToString("dd/MM/yyyy"),
                            vendedor = c.nombre + " " + c.paterno + " " + c.materno,
                            cendedorid = c.Id,
                            importe = (a.importe_precio_lista + a.iva_precio_lista).ToString("C"),
                            acciones = a.Acciones,
                            identidad = a.Id_Estado_Instalacion
                        }).ToList();

            if (item == null)
            {
                return response = Ok(new { token = "Error", item });
            }
            return response = Ok(new { token = "Success", item });
        }


        [Route("GetProductosCotizacion")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult GetProductosCotizacion([FromBody]ModeloBusquedaCotID busqueda)
        {
            IActionResult response = Unauthorized();

            var item = (from cp in _context.Cotizacion_Producto
                        join a in _context.Cat_Productos on cp.Id_Producto equals a.id
                        orderby a.nombre ascending
                        where cp.Id_Cotizacion == busqueda.Id
                        select new
                        {
                            id = a.id,
                            sku = a.sku,
                            modelo = a.modelo,
                            nombre = a.nombre,
                            descripcion_corta = a.descripcion_corta,
                            cantidad = cp.cantidad,
                            margen_cc = cp.margen_cc,
                            importe_precio_lista = cp.precio_lista + cp.iva_precio_lista,
                            importe_total_bruto = (cp.precio_lista + cp.iva_precio_lista) * cp.cantidad,
                            importe_condiciones_com = (cp.precio_condiciones_com + cp.iva_cond_comerciales) * cp.cantidad,
                            importe_con_descuento = (cp.precio_descuento + cp.iva_precio_descuento),
                            descuento = (cp.precio_lista + cp.iva_precio_lista) - (cp.precio_descuento + cp.iva_precio_descuento),
                            importetotal = (cp.precio_descuento + cp.iva_precio_descuento) * cp.cantidad,
                            es_regalo = cp.es_regalo,
                            agregado_automaticamente = cp.agregado_automaticamente,
                            cat_imagenes_producto = (from x in _context.Cat_Imagenes_Producto
                                                     where x.id_producto == a.id
                                                     select x)
                                                         .Select(d => new Cat_Imagenes_Producto
                                                         {
                                                             id = d.id,
                                                             id_producto = d.id_producto,
                                                             url = d.url
                                                         }).ToList()

                        }).ToList();
            if (item == null)
            {
                return response = Ok(new { token = "Error", item });
            }

            foreach (var prod in item)
            {
                if (prod.cat_imagenes_producto.Count == 0)
                {
                    Cat_Imagenes_Producto cip = new Cat_Imagenes_Producto();
                    cip.url = "../assets/img/img_prod_no_dips.png";
                    cip.id = 0;
                    cip.id_producto = prod.id;
                    prod.cat_imagenes_producto.Add(cip);
                }
            }

            return response = Ok(new { token = "Success", item });
        }

        [Route("GetProductosCarrito")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult GetProductosCarrito([FromBody]ModeloBusquedaCotID busqueda)
        {
            IActionResult response = Unauthorized();
            long id_C = 0;
            var id_C_l = _context.Cotizaciones.FirstOrDefault(C => C.Id_Cliente == busqueda.Id);

            if(id_C_l != null)
                id_C = _context.Cotizaciones.FirstOrDefault(C => C.Id_Cliente == busqueda.Id).Id;

            var Productos_Carrito_adds = _context.Productos_Carrito.FromSql("sp_add_prods_aut_carrito " + busqueda.Id,ToString()).ToList();
            Partners_ProductosController PC = new Partners_ProductosController(_config, _context);
            PC.crear_cotizacion_carrito(busqueda.Id);
            
            var item = get_productos_cotizacion (id_C);
            var lineas_hp = (from l in _context.Cat_Linea_Producto 
                             join sl in _context.Cat_SubLinea_Producto on l.id equals sl.id_linea_producto
                             join a in item[0].productos on sl.id equals a.id_sublinea
                             where a.id != 388 && sl.id_linea_producto != 36 && sl.id_linea_producto != 38
                             orderby sl.descripcion
                             select new
                             {
                                 id = sl.id,
                                 descripcion = l.descripcion + " | " + sl.descripcion,
                                 hrs = sl.hp_horas,
                             });
            
            if (item == null)
            {

                return response = Ok(new { token = "Error", item });
            }
            else
            {
                return response = Ok(new { token = "Success", item, lineas_hp});
            }


        }




        [Route("GetDocumentosCotizacion")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult GetDocumentosCotizacion([FromBody]ModeloBusquedaBD busqueda)
        {
            IActionResult response = Unauthorized();
            var Pagos_liquidacion = (from dc in _context.documentos_cotizacion
                                     join fp in _context.tipos_comprobantes on dc.id_tipo_tipo_pago equals fp.id
                                     where dc.Id_Cotizacion == busqueda.Id && fp.es_liquidacion == true
                                     select new
                                     {
                                         id = dc.Id
                                     }).ToList().Count;
            bool Sol_ent_valido = false;
            var cotizacion = _context.Cotizaciones.FirstOrDefault(c => c.Id == busqueda.Id);
            if (Pagos_liquidacion > 0 && !cotizacion.entrega_sol && !cotizacion.rechazada) //si hay pagos de liquidación y no ha solicitado la entrega antes 
                Sol_ent_valido = true;

            var id_cuenta = _context.Cat_Sucursales.FirstOrDefault(c => c.Id == cotizacion.Id_sucursal).Id_Cuenta;
            var id_canal = _context.Cat_Cuentas.FirstOrDefault(c => c.Id == id_cuenta).Id_Canal;
            if (id_canal == 2) // si es venta directa y no tiene ordenes es falso
            {
                var Ordenes = (from dc in _context.documentos_cotizacion
                               join fp in _context.tipos_comprobantes on dc.id_tipo_tipo_pago equals fp.id
                               where dc.Id_Cotizacion == busqueda.Id && fp.id == 5
                               select new
                               {
                                   id = dc.Id
                               }).ToList().Count;

                if (Ordenes < 1) //si hay pagos de liquidación y no ha solicitado la entrega antes 
                    Sol_ent_valido = false;
            }

            var item = (from dc in _context.documentos_cotizacion
                        join t in _context.tipos_comprobantes on dc.id_tipo_tipo_pago equals t.id
                        join u in _context.Users on dc.id_user equals u.id
                        orderby dc.tipo_docto ascending
                        where dc.Id_Cotizacion == busqueda.Id //&& dc.tipo_docto == (tpo_D == 0 ? dc.tipo_docto : tpo_D)
                        select new
                        {
                            Id_Cotizacion = dc.Id_Cotizacion,
                            Id_foto = dc.Id_foto,
                            tipo_docto = dc.tipo_docto,
                            Id = dc.Id,
                            tipo_comp = t.tipo_pago,
                            usuario = u.email,
                            fecha_subida = dc.fecha_subida.ToString("dd/MM/yyyy hh:mm")
                        }).ToList();
            if (item == null)
            {
                return response = Ok(new { token = "Error", item });
            }
            return response = Ok(new { token = "Success", Sol_ent_valido, item });
        }



        [Route("GetCotizacion")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult GetCotizacion([FromBody]ModeloBusquedaCotID busqueda)
        {
            IActionResult response = Unauthorized();

            var item = (from c in _context.Cotizaciones
                        join ce in _context.Cat_Estatus_Cotizacion on c.Estatus equals ce.id
                        where c.Id == busqueda.Id
                        select new
                        {
                            Numero = c.Numero,
                            Id_Cliente = c.Id_Cliente,
                            Id_Vendedor = c.Id_Vendedor,
                            fecha_cotiza = c.fecha_cotiza,
                            Inporte = (c.importe_precio_lista + c.iva_precio_lista),
                            Inporte_Final = (c.importe_precio_lista + c.iva_precio_lista),
                            Estatus = c.Estatus,
                            Acciones = c.Acciones,
                            Id_Canal = c.Id_Canal,
                            Id_Cuenta = c.Id_Cuenta,
                            Id_Estado_Instalacion = c.Id_Estado_Instalacion,
                            Observaciones = c.Observaciones,
                            creadopor = c.creadopor,
                            id_formapago = c.id_formapago,
                            estatus_desc = ce.Estatus_es

                            // ibs = c.ibs

                        }).ToList();
            if (item == null)
            {
                return response = Ok(new { resultado = "Error", item });
            }
            return response = Ok(new { resultado = "Success", item });
        }

        // POST: api/Servicios
        [Route("NumProdCarritoUsuario")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult NumProdCarritoUsuario([FromBody]ModeloNumProdC busqueda)
        {
            IActionResult response = Unauthorized();
            var item = (from x in _context.Productos_Carrito
                        where x.id_usuario == busqueda.Id_user
                        group x by x.id_usuario into g
                        select new
                        {
                            // ProductName = g.First().Id,
                            nprod = g.Sum(_ => _.cantidad).ToString(),
                            // Quantity = g.Count().ToString(),
                        }).ToList();

            return response = Ok(new { token = "Success", item });
        }

        // POST: api/Servicios
        [Route("SucursalesPorUsuario")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult SucursalesPorUsuario([FromBody]ModBusPorId busqueda)
        {
            IActionResult response = Unauthorized();
            var U_session = _context.Users.FirstOrDefault(u => u.id == busqueda.Id);
            var item = (from x in _context.Cat_Sucursales
                        where x.Id == U_session.id_Sucursales
                        select new
                        {
                            Id = x.Id,
                            Sucursal = x.Sucursal
                        }).ToList();

            if (U_session.nivel == "canal")
            {
                item = (from x in _context.Cat_Sucursales
                        join cta in _context.Cat_Cuentas on x.Id_Cuenta equals cta.Id
                        join cnl in _context.Cat_canales on cta.Id_Canal equals cnl.Id
                        where cnl.Id == U_session.id_canal
                        select new
                        {
                            Id = x.Id,
                            Sucursal = x.Sucursal
                        }).ToList();
            }
            if (U_session.nivel == "cuenta")
            {
                item = (from x in _context.Cat_Sucursales
                        where x.Id_Cuenta == U_session.id_cuenta
                        select new
                        {
                            Id = x.Id,
                            Sucursal = x.Sucursal
                        }).ToList();
            }
            return response = Ok(new { token = "Success", item });
        }

        // POST: api/Servicios
        [Route("Canales_por_usuario")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Canales_por_usuario_nivel([FromBody]ModBusPorId busqueda)
        {
            IActionResult response = Unauthorized();
            var U_session = _context.Users.FirstOrDefault(u => u.id == busqueda.Id);
            if (U_session.id_rol == 10004 || U_session.id_rol == 1) //si son administradores o vendedores se muestra solo su canal
            {
                var item = (from x in _context.Cat_canales
                            where x.Id == U_session.id_canal
                            select new
                            {
                                id = x.Id,
                                canal_es = x.Canal_es
                            }).ToList();
                return response = Ok(new { token = "Success", item });
            }
            else
            {
                var item = (from x in _context.Cat_canales
                            select new
                            {
                                id = x.Id,
                                canal_es = x.Canal_es
                            }).ToList();
                return response = Ok(new { token = "Success", item });
            }

        }

        // POST: api/Servicios
        [Route("Canales_por_usuario_por_nivel")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Canales_por_usuario_por_nivel([FromBody]ModBusPorId busqueda)
        {
            IActionResult response = Unauthorized();
            var U_session = _context.Users.FirstOrDefault(u => u.id == busqueda.Id);
            if (U_session.id_rol == 10004 || U_session.id_rol == 1) //si son administradores o vendedores se muestra solo su canal
            {
                var item = (from x in _context.Cat_canales
                            where x.Id == U_session.id_canal
                            select new
                            {
                                id = x.Id,
                                canal_es = x.Canal_es
                            }).ToList();
                return response = Ok(new { token = "Success", item });
            }
            else
            {
                var item = (from x in _context.Cat_canales
                            select new
                            {
                                id = x.Id,
                                canal_es = x.Canal_es
                            }).ToList();
                return response = Ok(new { token = "Success", item });
            }

        }


        // POST: api/Servicios
        [Route("Cuentas_por_Canal_usr")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Cuentas_por_Canal_usr([FromBody]ModBusPorDosId busqueda)
        {
            IActionResult response = Unauthorized();
            var U_session = _context.Users.FirstOrDefault(u => u.id == busqueda.Id2);
            if (U_session.id_rol == 10004 || U_session.id_rol == 1) //si son administradores o vendedores se muestra solo su canal
            {
                var id_cuenta = U_session.nivel == "canal" ? 0 : U_session.id_cuenta;
                var item = (from x in _context.Cat_Cuentas
                            orderby x.Cuenta_es ascending
                            where x.Id == (id_cuenta == 0 ? x.Id : U_session.id_cuenta) // si es admon de Canal muestra todas las cuentas si no solo la suya
                            && x.Id_Canal == U_session.id_canal
                            select new
                            {
                                id = x.Id,
                                cuenta_es = x.Cuenta_es
                            }).ToList();
                return response = Ok(new { token = "Success", item });
            }
            else
            {
                var item = (from x in _context.Cat_Cuentas
                            orderby x.Id_Canal, x.Cuenta_es ascending
                            where x.Id_Canal == (busqueda.Id1 == 0 ? x.Id_Canal : busqueda.Id1)
                            select new
                            {
                                id = x.Id,
                                cuenta_es = x.Cuenta_es
                            }).ToList();
                return response = Ok(new { token = "Success", item });
            }
        }

        // POST: api/Servicios
        [Route("SucursalesPorCanalCuentaUsuario")]
        [AllowAnonymous]
        [HttpOptions] //this was the key

        [HttpPost]
        public IActionResult SucursalesPorCanalCuentaUsuario([FromBody]ModBusPorTresId busqueda)
        {
            IActionResult response = Unauthorized();
            var U_session = _context.Users.FirstOrDefault(u => u.id == busqueda.Id3);
            if (U_session.id_rol == 10004 || U_session.id_rol == 1) //si son administradores o vendedores se muestra solo su canal
            {
                var item = (from x in _context.Cat_Sucursales
                            where x.Id == U_session.id_Sucursales
                            select new
                            {
                                Id = x.Id,
                                Sucursal = x.Sucursal
                            }).ToList();

                if (U_session.nivel == "cuenta")
                {
                    item = (from x in _context.Cat_Sucursales
                            join cta in _context.Cat_Cuentas on x.Id_Cuenta equals cta.Id
                            orderby x.Sucursal ascending
                            where cta.Id == U_session.id_cuenta
                            select new
                            {
                                Id = x.Id,
                                Sucursal = x.Sucursal
                            }).ToList();
                }
                if (U_session.nivel == "canal")
                {
                    item = (from x in _context.Cat_Sucursales
                            join cta in _context.Cat_Cuentas on x.Id_Cuenta equals cta.Id
                            orderby cta.Cuenta_es, x.Sucursal ascending
                            where cta.Id_Canal == U_session.id_canal
                            && x.Id_Cuenta == (busqueda.Id2 == 0 ? x.Id_Cuenta : busqueda.Id2)
                            select new
                            {
                                Id = x.Id,
                                Sucursal = x.Sucursal
                            }).ToList();
                }
                return response = Ok(new { token = "Success", item });
            }
            else
            {
                var item = (from x in _context.Cat_Sucursales
                            join cta in _context.Cat_Cuentas on x.Id_Cuenta equals cta.Id
                            orderby cta.Id_Canal, cta.Cuenta_es, x.Sucursal ascending
                            where cta.Id_Canal == (busqueda.Id1 == 0 ? cta.Id_Canal : busqueda.Id1)
                            && cta.Id == (busqueda.Id2 == 0 ? cta.Id : busqueda.Id2)
                            select new
                            {
                                id = x.Id,
                                Sucursal = x.Sucursal
                            }).ToList();
                return response = Ok(new { token = "Success", item });
            }
        }



        // POST: api/Servicios
        [Route("UsuarioCompleto")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult UsuarioCompleto([FromBody]ModBusPorId busqueda)
        {
            IActionResult response = Unauthorized();

            var item = (from a in _context.Users
                        where a.id == busqueda.Id
                        join can in _context.Cat_canales on a.id_canal equals can.Id
                        join cta in _context.Cat_Cuentas on a.id_cuenta equals cta.Id
                        join suc in _context.Cat_Sucursales on a.id_Sucursales equals suc.Id
                        join rol in _context.Cat_Roles on a.id_rol equals rol.id
                        select new
                        {
                            id = a.id,
                            username = a.username,
                            name = a.name,
                            paterno = a.paterno,
                            materno = a.materno,
                            password = a.password,
                            email = a.email,
                            telefono = a.telefono,
                            movil = a.telefono_movil,
                            rol = a.id,
                            id_canal = a.id_canal
                            ,
                            canal = can.Canal_es
                            ,
                            id_cuenta = a.id_cuenta
                            ,
                            cuenta = cta.Cuenta_es
                            ,
                            id_sucursal = a.id_Sucursales
                            ,
                            sucursal = suc.Sucursal
                            ,
                            nivel = a.nivel
                            ,
                            tipo_usuario = rol.rol

                        }).ToList();

            return response = Ok(new { token = "Success", item });
        }

        // POST: api/Servicios
        [Route("NuevoCliente")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Post([FromBody]Clientes item)
        {
            IActionResult response;
            try
            {
                if (item == null)
                {
                    response = Ok(new { id = "0", response = "Error" });
                }
                else
                {
                    var Empleado = _context.Users.FirstOrDefault(a => a.id == item.creadopor);
                    item.Id_sucursal = Empleado.id_Sucursales;
                    _context.Clientes.Add(item);
                    _context.SaveChanges();
                    response = Ok(new { id = item.id, response = "Success" });
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { id = "0", response = "Error" });
            }
            return new ObjectResult(response);
        }

        public float precio_cond_comerciales(float precio, int id_sublinea, int id_sucursal)
        {
            float precio_cond_com = 0;

            precio_cond_com = precio;
            var ccs = _context.condiones_comerciales_sucursal.FirstOrDefault(c => c.id_Cat_SubLinea_Producto == id_sublinea && c.id_Cat_Sucursales == id_sucursal);
            if (ccs == null)
                return precio_cond_com;
            else
                precio_cond_com = precio_cond_com * (1 + ccs.margen);
            return precio_cond_com;
        }

        // POST: api/Servicios
        [Route("NuevaCotizacion")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult NuevaCotizacion([FromBody]Cotizaciones item)
        {
            IActionResult response;
            int id_usuario = 0;
            id_usuario = item.creadopor;

            try
            {
                if (item == null)
                {
                    response = Ok(new { id = "0", response = "Error" });
                }
                else
                {
                    var _cotizacion = _context.Cotizaciones.FirstOrDefault(c => c.Id_Cliente == id_usuario);
                    var id_cuenta = (from usr in _context.Users
                                     where usr.id == item.Id_Vendedor
                                     select new
                                     {
                                         id_cuenta = usr.id_cuenta,
                                         id_canal = usr.id_canal,
                                         id_sucursal = usr.id_Sucursales
                                     }).ToList();
                    if (id_cuenta != null)
                    {
                        _cotizacion.Id_Cuenta = id_cuenta[0].id_cuenta;
                        _cotizacion.Id_Canal = id_cuenta[0].id_canal;
                        _cotizacion.Id_sucursal = id_cuenta[0].id_sucursal;
                    }

                
                    _cotizacion.Id_Cliente = item.Id_Cliente;
                    _cotizacion.numero_productos = _context.Cotizacion_Producto.Where(v=> v.Id_Cotizacion == _cotizacion.Id).AsEnumerable().Sum(o => o.cantidad);
                    _cotizacion.fecha_cotiza = DateTime.Now;
                    _cotizacion.Observaciones = item.Observaciones;
                    _cotizacion.Numero = _cotizacion.Id.ToString();
                    _cotizacion.id_cotizacion_padre = _cotizacion.Id;
                    _cotizacion.Id_Vendedor = item.Id_Vendedor;
                    _context.Cotizaciones.Update(_cotizacion);

                    var cert = _context.Cer_producto_cliente.Where(i => i.id_cliente == id_usuario).ToList();
                    var cert_p = _context.rel_certificado_producto.Where(i => cert.Any(k => k.id == i.id_certificado)).ToList();
                    var hp = _context.home_producto_cliente.Where(i => i.id_cotizacion == _cotizacion.Id).ToList();

                    foreach (var x in cert)
                    {
                        x.id_cliente = item.Id_Cliente;
                        _context.Cer_producto_cliente.Update(x);
                    }
                       
                    //foreach (var x in cert_p)
                    //{
                    //    x.id_cliente = item.Id_Cliente;
                    //    _context.rel_certificado_producto.Update(x);
                    //}
                       
                      
                    foreach (var x in hp)
                    {
                        x.id_cliente = item.Id_Cliente;
                        _context.home_producto_cliente.Update(x);
                    }

                    var pcarrito = _context.Productos_Carrito.Where(y => y.id_usuario == id_usuario).ToList();
                    foreach(var t in pcarrito)
                        _context.Productos_Carrito.Remove(t);

                    _context.SaveChanges();


                    //correo2 de asignacion de cotizacion
                    var vendedor = _context.Users.FirstOrDefault(us => us.id == _cotizacion.Id_Vendedor);
                    var ctaSuc = _context.Cat_Sucursales.Include(s => s.Cuenta).FirstOrDefault(s => s.Id == _cotizacion.Id_sucursal);
                    if (vendedor != null)
                    {
                        StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_Interno.html"));
                        string body = string.Empty;
                        body = reader.ReadToEnd();
                        body = body.Replace("{Titulo}", "Cotización asignada");
                        body = body.Replace("{Cuenta}", ", cuenta <strong>" + ctaSuc.Cuenta.Cuenta_es + "</strong>");
                        body = body.Replace("{Sucursal}", ", sucursal <strong>" + ctaSuc.Sucursal + "</strong>");
                        body = body.Replace("{Username}", vendedor.name);
                        body = body.Replace("{Texto}", "Has sido asignado al seguimiento de la orden número ");
                        body = body.Replace("{NumeroCotizacion}", _cotizacion.Id.ToString());
                        SendMail(vendedor.email, body, "Cotización asignada # " + _cotizacion.Id.ToString());
                    }
                    
                    response = Ok(new { id = _cotizacion.Id, response = "Success" });
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { id = "0", response = "Error" });
            }
            return new ObjectResult(response);
        }

        public float get_margen_cc_porSubid(int? id_sublinea)
        {
            float margen;
            //var C = _context.condiones_comerciales_sucursal.FirstOrDefault(c => c.id_Cat_Sucursales == id_sublinea);

            var C = (from c in _context.condiones_comerciales_sucursal
                     where c.id_Cat_Sucursales == id_sublinea
                     select new
                     {
                         c.margen
                     }).ToList();


            if (C.Count > 0)
                margen = C[0].margen;
            else
                margen = 0;

            return margen;
        }


        // POST: api/Servicios
        [Route("duplicar_cotizacion")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult duplicar_cotizacion([FromBody]ModBusPorId item)
        {
            IActionResult response;
            try
            {
                Cotizaciones duplicado = new Cotizaciones();
                if (item == null)
                {
                    response = Ok(new { id = "0", response = "Error" });
                }
                else
                {
                    var D = _context.Cotizaciones.Include(c=> c.direcciones_cotizacion)
                        .Include(c => c.Id_Cotizacion_Producto)
                        .Include(c => c.cotizacion_promocion)
                        .Include(c => c.cotizacion_monto_descuento)
                        .FirstOrDefault(c => c.Id == item.Id);
                    var ncotiza = (from x in _context.Cotizaciones
                                   where x.id_cotizacion_padre == D.id_cotizacion_padre
                                   select new { ncotiza = x.Id }).Count();
                    
                    //var ids_ = _context.Cotizacion_Producto.Where(cp => cp.Id_Cotizacion == D.Id).ToList();

                    duplicado.Estatus = 1; /// Siempre se crea como cotización 
                    duplicado.Numero = D.Id.ToString() + "-" + ncotiza.ToString();
                    duplicado.Acciones = D.Acciones;
                    duplicado.Id_Cliente = D.Id_Cliente;
                    duplicado.Id_sucursal = D.Id_sucursal;
                    duplicado.creadopor = D.creadopor;
                    duplicado.rechazada = false;
                    duplicado.entrega_sol = false;
                    duplicado.puede_solicitar_env = 0;
                    duplicado.cambio_ord_comp_generada = Convert.ToDateTime("0001-01-01 00:00:00.0000000");
                    duplicado.coment_cancel = "";

                    duplicado.fecha_cotiza = DateTime.Now;
                    duplicado.ibs = null;
                    duplicado.Id_Canal = D.Id_Canal;
                    duplicado.Id_Cliente = D.Id_Cliente;
                    duplicado.id_cotizacion_padre = D.id_cotizacion_padre;
                    duplicado.Id_Cuenta = D.Id_Cuenta;
                    duplicado.id_formapago = D.id_formapago;
                    duplicado.Id_Vendedor = D.Id_Vendedor;
                    duplicado.motivo_rechazo = "";
                    duplicado.acepto_terminos_condiciones = false;

                    // importes

                    duplicado.importe_condiciones_com = D.importe_precio_lista;
                    duplicado.iva_condiciones_com = D.iva_precio_lista;
                    duplicado.importe_precio_lista = D.importe_precio_lista;
                    duplicado.iva_precio_lista = D.iva_precio_lista;
                    duplicado.importe_promociones = D.importe_precio_lista;
                    duplicado.iva_promociones = D.iva_precio_lista;

                    duplicado.numero_productos = D.numero_productos;
                    duplicado.Observaciones = D.Observaciones;
                    duplicado.vigenica_ref = D.vigenica_ref;
                    
                    //Copia las direcciones a la nueva cotizacion
                    foreach (Direccion_Cotizacion direccion in D.direcciones_cotizacion)
                    {
                        duplicado.direcciones_cotizacion.Add(new Direccion_Cotizacion
                        {
                            calle_numero = direccion.calle_numero,
                            cp = direccion.cp,
                            id_estado = direccion.id_estado,
                            id_municipio = direccion.id_municipio,
                            colonia = direccion.id_localidad.ToString(),
                            telefono = direccion.telefono,
                            telefono_movil = direccion.telefono_movil,
                            creado = direccion.creado,
                            creadopor = direccion.creadopor,
                            tipo_direccion = direccion.tipo_direccion,
                            nombrecontacto = direccion.nombrecontacto,
                            numExt = direccion.numExt,
                            numInt = direccion.numInt,
                            Fecha_Estimada = direccion.Fecha_Estimada,
                            id_localidad = direccion.id_localidad,
                            id_prefijo_calle = direccion.id_prefijo_calle,
                        });
                        //Direccion_Cotizacion nuevo = new Direccion_Cotizacion();
                        //nuevo.cp = direccion.cp;
                        //nuevo.id_estado = direccion.id_estado;
                        //nuevo.id_municipio = direccion.id_municipio;
                        //nuevo.id_localidad = direccion.id_localidad;
                        //nuevo.colonia = direccion.id_localidad.ToString();
                        //nuevo.id_prefijo_calle = direccion.id_prefijo_calle;
                        //nuevo.calle_numero = direccion.calle_numero;
                        //nuevo.numExt = direccion.numExt;
                        //nuevo.numInt = direccion.numInt;
                        //nuevo.nombrecontacto = direccion.nombrecontacto;
                        //nuevo.telefono = direccion.telefono;
                        //nuevo.telefono_movil = direccion.telefono_movil;
                        //nuevo.tipo_direccion = direccion.tipo_direccion;
                        //nuevo.creado = direccion.creado;
                        //nuevo.Fecha_Estimada = direccion.Fecha_Estimada;
                        
                        //_context.SaveChanges();
                    }

                    foreach (Cotizacion_Promocion CPr in D.cotizacion_promocion)
                    {
                        duplicado.cotizacion_promocion.Add(new Cotizacion_Promocion { id_promocion = CPr.id_promocion });
                    }

                    foreach (cotizacion_monto_descuento cmd in D.cotizacion_monto_descuento)
                    {
                        duplicado.cotizacion_monto_descuento.Add(new cotizacion_monto_descuento
                        {
                            id_promocion = cmd.id_promocion,
                            monto_desc_con_iva = cmd.monto_desc_con_iva,
                            monto_desc_sin_iva = cmd.monto_desc_sin_iva
                        });
                    }

                    //Copia los prodctos a la nueva cotizacion
                    foreach (Cotizacion_Producto PC in D.Id_Cotizacion_Producto)
                    {
                        duplicado.Id_Cotizacion_Producto.Add(
                            new Cotizacion_Producto
                            {
                                Id_Producto = PC.Id_Producto,
                                cantidad = PC.cantidad,
                                iva_cond_comerciales = PC.iva_cond_comerciales,
                                iva_precio_descuento = PC.iva_precio_descuento,
                                iva_precio_lista = PC.iva_precio_lista,
                                precio_condiciones_com = PC.precio_condiciones_com,
                                precio_descuento = PC.precio_descuento,
                                precio_lista = PC.precio_lista,
                                margen_cc = PC.margen_cc,
                                agregado_automaticamente = PC.agregado_automaticamente,
                                es_regalo = PC.es_regalo,
                            });
                        //Cotizacion_Producto PD = new Cotizacion_Producto();
                        //PD.Id_Producto = PC.Id_Producto;
                        //PD.cantidad = PC.cantidad;
                        //PD.precio_lista = PC.precio_lista;
                        //PD.iva_precio_lista = PC.iva_precio_lista;
                        //PD.precio_descuento = PC.precio_descuento;
                        //PD.iva_precio_descuento = PC.iva_precio_descuento;
                        //PD.precio_condiciones_com = PC.precio_condiciones_com;
                        //PD.iva_cond_comerciales = PC.iva_cond_comerciales;
                        //PD.margen_cc = PC.margen_cc;
                        //PD.agregado_automaticamente = PC.agregado_automaticamente;
                        //PD.es_regalo = PC.es_regalo;
                        //_context.Cotizacion_Producto.Add(PD);
                        

                    }
                    _context.Cotizaciones.Add(duplicado);
                    _context.SaveChanges();
                    response = Ok(new { id = duplicado.Id, response = "Success" });
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { id = "0", response = "Error" });
            }
            return new ObjectResult(response);
        }

        // POST: api/Servicios
        [Route("actualizacantidadcotprod")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult actualizacantidadcotprod([FromBody]Cotizacion_Producto item)
        {
            IActionResult response = Unauthorized();

            try
            {
                var cotiza = _context.Cotizaciones.FirstOrDefault(c => c.Id == item.Id_Cotizacion);
                var cotprod = _context.Cotizacion_Producto.FirstOrDefault(s => s.Id_Producto == item.Id && s.Id_Cotizacion == item.Id_Cotizacion);

                if (cotprod == null)
                {
                    return BadRequest();
                }
                else
                {
                    int? sublinea = _context.Cat_Productos.FirstOrDefault(p => p.id == item.Id).id_sublinea;
                    int linea = _context.Cat_SubLinea_Producto.FirstOrDefault(sl => sl.id == sublinea).id_linea_producto;

                    if (item.cantidad == -1)
                    {
                        _context.Cotizacion_Producto.Remove(cotprod);
                        _context.SaveChanges();

                        if (linea == 36) //son certificados
                            recalcular_certificados(0, Convert.ToInt32(cotiza.Id_Cliente), Convert.ToInt32(cotiza.Id), Convert.ToInt32(item.Id));
                        if (linea == 38) //son home
                        {
                            var id_c_cu = _context.Cotizaciones.FirstOrDefault(c => c.Id == cotiza.Id).Id;
                            ajustar_sublineas_hp(Convert.ToInt32(id_c_cu), Convert.ToInt32(item.Id), Convert.ToInt32(cotiza.Id_Cliente), false, false);
                            // recalcular_hp(p_c.id_usuario, p_c.id_usuario, 0, Convert.ToInt32(p_c.Id));
                        }
                    }
                    else
                    {
                        cotprod.cantidad = item.cantidad;
                        _context.Cotizacion_Producto.Update(cotprod);
                        _context.SaveChanges();
                        if (linea == 38) //son home
                        {
                            ajustar_sublineas_hp(Convert.ToInt32(cotiza.Id), Convert.ToInt32(item.Id), Convert.ToInt32(cotiza.Id_Cliente), false, false);
                        }
                    }
                    //List<promocion> promociones = _context.promocion.FromSql("Get_recalcula_montos_cotizacionid '" + item.Id_Cotizacion.ToString() + "'").ToList();
                    var _C = get_productos_cotizacion(item.Id_Cotizacion);
                    var prods = _C[0].productos;
                    var servicios = (from a in _C select new { a.tiene_certificado, a.tiene_home, a.tiene_envio });
                    if (prods == null)
                    {
                        return response = Ok(new { response = "Error", prods });
                    }
                    else
                    {
                        var montos = (from c in _context.Cotizaciones
                                      where c.Id == cotiza.Id
                                      select new
                                      {
                                          //////////////////////////////////////MONTOS
                                          importe_precio_lista = _C[0].importe_precio_lista,
                                          iva_precio_lista = _C[0].iva_precio_lista,
                                          importe_condiciones_com = _C[0].importe_condiciones_com,
                                          iva_condiciones_com = _C[0].iva_condiciones_com,
                                          importe_promociones = _C[0].importe_promociones,
                                          iva_promociones = _C[0].iva_promociones,
                                          descuento_acumulado = _C[0].descuento_acumulado,
                                          descuento_acumulado_cond_com = _C[0].descuento_acumulado_cond_com,
                                          comision_vendedor = _C[0].comision_vendedor
                                      }).ToList();
                        
                        return response = Ok(new { response = "Success", prods, montos, detalle = "Cotización Cargada Correctamente", promos_aplicables = _C[0].promociones_respueta, servicios });
                    }
                }
            }
            catch (Exception ex)
            {
                return response = Ok(new { response = "Error", item });
            }

        }


        [AllowAnonymous]
        [Route("usuarios_por_cuenta")]
        [HttpPost("{usuarios_por_cuenta}")]
        public IActionResult usuarios_por_cuenta([FromBody]busquedaLibre busqueda)
        //  int id_ = busqueds
        {
            int id_cuenta = 0;
            if (busqueda != null)
            {
                var usr = (from b in _context.Users
                           join c in _context.Cat_Cuentas on b.id_cuenta equals c.Id
                           where Convert.ToInt32(busqueda.TextoLibre) == b.id
                           select new
                           {
                               id_cuenta = b.id_cuenta
                           }).ToList();
                id_cuenta = usr[0].id_cuenta;
            }

            var item = (from b in _context.Users
                        join c in _context.Cat_Cuentas on b.id_cuenta equals c.Id
                        where b.id_cuenta == id_cuenta
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

        [AllowAnonymous]
        [Route("sucursales_por_usuario")]
        [HttpPost("{sucursales_por_usuario}")]
        public IActionResult sucursales_por_usuario([FromBody]busquedaLibre busqueda)
        //  int id_ = busqueds
        {
            //int id_cuenta = 0;
            if (busqueda == null)
            {
                return BadRequest();
            }

            var usr = (from b in _context.Users
                       join s in _context.Cat_Sucursales on b.id_Sucursales equals s.Id
                       join cu in _context.Cat_Cuentas on s.Id_Cuenta equals cu.Id
                       join ca in _context.Cat_canales on cu.Id_Canal equals ca.Id
                       where Convert.ToInt32(busqueda.TextoLibre) == b.id
                       select new
                       {
                           b.nivel,
                           id_sucursal = (b.nivel == "sucursal" ?  b.id_Sucursales : 0),
                           id_cuenta = (b.nivel == "cuenta" ? cu.Id : 0),
                           id_canal = (b.nivel == "canal" ? ca.Id : 0),
                       }).FirstOrDefault();
            //id_cuenta = Convert.ToInt32(usr.id_cuenta);
            var sucursales = (from s in _context.Cat_Sucursales
                              join cu in _context.Cat_Cuentas on s.Id_Cuenta equals cu.Id
                              join ca in _context.Cat_canales on cu.Id_Canal equals ca.Id
                              //where s.Id == (usr.nivel == "sucursal" ? usr.id_sucursal : s.Id)
                              where s.Id == (usr.id_sucursal == 0 ? s.Id : usr.id_sucursal)
                              && cu.Id == (usr.id_cuenta == 0 ? cu.Id : usr.id_cuenta)
                              && ca.Id == (usr.id_canal == 0 ? ca.Id : usr.id_canal)
                              orderby cu.Cuenta_es, s.Sucursal ascending
                              select new
                              {
                                  id_sucursal = s.Id,
                                  nombre = cu.Cuenta_es + " | " + s.Sucursal
                              }).ToList();
           

            if (sucursales == null)
            {
                return BadRequest();
            }
            return new ObjectResult(sucursales);
        }

        [AllowAnonymous]
        [Route("get_usuarios_suc")]
        [HttpPost("{get_usuarios_suc}")]
        public IActionResult get_usuarios_suc([FromBody]ModBusPorId item)
        //  int id_ = busqueds
        {
            //int id_cuenta = 0;
            if (item == null)
            {
                return BadRequest();
            }

            //var vendedor = _context.Users.FirstOrDefault(us => us.id == item.Id2);

            var usrs = (from b in _context.Users
                        join c in _context.Cat_Cuentas on b.id_cuenta equals c.Id
                        where b.id_Sucursales == item.Id && b.estatus == true
                        && (b.id_rol == 10004 || b.id_rol == 1)
                        select new
                        {
                            id = b.id,
                            nombrecompleto = b.name + " " + b.paterno + " " + (b.materno == null ? "" : b.materno) + " - " + c.Cuenta_es
                        }).ToList();

            if (usrs == null)
            {
                return BadRequest();
            }
            return new ObjectResult(usrs);
        }


        //Solicita el id del usuario para 
        [Route("VaciarCarrito")]
        [HttpPost("VaciarCarrito/{usuario}")]
        public IActionResult VaciarCarrito(int usuario)
        {
            IActionResult response = Unauthorized();
            try
            {

                var del_carrito = _context.Productos_Carrito.Where(p => p.id_usuario == usuario);
                var usr_cotiza = _context.Cotizaciones.FirstOrDefault(c => c.Id_Cliente == usuario);
                if (usr_cotiza != null)
                {
                    var coti_prom = _context.Cotizacion_Promocion.Where(a => a.id_cotizacion == usr_cotiza.Id);
                    var coti_prod = _context.Cotizacion_Producto.Where(a => a.Id_Cotizacion == usr_cotiza.Id);
                    var prod_prom = _context.Producto_Promocion.Where(a => a.id_cotizacion == usr_cotiza.Id);
                    //var afecta_cc = _context.afectacion_cc.Where(a => a.id == usr_cotiza.Id);

                    var cer_prod_cte = _context.Cer_producto_cliente.Where(a => a.id_cotizacion == usr_cotiza.Id);
                    var rel_cer_prod = _context.rel_certificado_producto.Where(a => a.certificado.id_cotizacion == usr_cotiza.Id);
                    var home_prod_cte = _context.home_producto_cliente.Where(a => a.id_cotizacion == usr_cotiza.Id);

                    if (coti_prom.Count() > 0)   _context.Cotizacion_Promocion.RemoveRange(coti_prom);
                    if (coti_prod.Count() > 0) _context.Cotizacion_Producto.RemoveRange(coti_prod);
                    if (prod_prom.Count() > 0) _context.Producto_Promocion.RemoveRange(prod_prom);

                    if (cer_prod_cte.Count() > 0) _context.Cer_producto_cliente.RemoveRange(cer_prod_cte);
                    if (rel_cer_prod.Count() > 0) _context.rel_certificado_producto.RemoveRange(rel_cer_prod);
                    if (home_prod_cte.Count() > 0) _context.home_producto_cliente.RemoveRange(home_prod_cte);

                }

                
                if (del_carrito.Count() > 0) _context.Productos_Carrito.RemoveRange(del_carrito);
                if (usr_cotiza != null) _context.Cotizaciones.Remove(usr_cotiza);

                _context.SaveChanges();

            }
            catch (Exception ex)
            {
                response = Ok(new { del_all = true, result = "Success", detalle = ex.Message });
            }

            response = Ok(new { del_all = true, result = "Success" });

            return Ok(response);
        }

        // POST: api/Servicios
        [Route("actualizacantidadcarritoprod")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult actualizacantidadcarritoprod([FromBody]Productos_Carrito p_c)
        {
            IActionResult response = Unauthorized();
            try
            {
                var cotprod = _context.Productos_Carrito.FirstOrDefault(s => s.Id_Producto == p_c.Id && s.id_usuario == p_c.id_usuario);
                if (cotprod == null)
                {
                    return BadRequest();
                }
                else
                {
                    int? sublinea = _context.Cat_Productos.FirstOrDefault(p => p.id == p_c.Id).id_sublinea;
                    int linea = _context.Cat_SubLinea_Producto.FirstOrDefault(sl => sl.id == sublinea).id_linea_producto;

                    if (p_c.cantidad == -1)
                    {
                        _context.Productos_Carrito.Remove(cotprod);
                        _context.SaveChanges();
                        if (linea == 36) //son certificados
                            recalcular_certificados(p_c.id_usuario, p_c.id_usuario, 0, Convert.ToInt32(p_c.Id));
                        if (linea == 38) //son home
                        {
                            var id_c_cu = _context.Cotizaciones.FirstOrDefault(c => c.Id_Cliente == p_c.id_usuario).Id;
                            ajustar_sublineas_hp(Convert.ToInt32(id_c_cu), Convert.ToInt32(p_c.Id), p_c.id_usuario, false, true);
                           // recalcular_hp(p_c.id_usuario, p_c.id_usuario, 0, Convert.ToInt32(p_c.Id));
                        }
                            
                    }
                    else
                    {
                        cotprod.cantidad = p_c.cantidad;
                        _context.Productos_Carrito.Update(cotprod);
                        _context.SaveChanges();
                        if (linea == 38) //son home
                        {
                            var id_c_cu = _context.Cotizaciones.FirstOrDefault(c => c.Id_Cliente == p_c.id_usuario).Id;
                            ajustar_sublineas_hp(Convert.ToInt32(id_c_cu), Convert.ToInt32(p_c.Id), p_c.id_usuario, false, true);
                            // recalcular_hp(p_c.id_usuario, p_c.id_usuario, 0, Convert.ToInt32(p_c.Id));
                        }

                    }

                    Partners_ProductosController PC = new Partners_ProductosController(_config, _context); 
                    long id_cotizacion =  PC.crear_cotizacion_carrito(p_c.id_usuario);
                    var item = get_productos_cotizacion(id_cotizacion);
                    if (item == null)
                    {
                        return response = Ok(new { response = "Error", item });
                    }
                    else
                    {
                        return response = Ok(new { response = "Success", item });
                    }
                }
            }
            catch (Exception ex)
            {
                var deterror = ex.Message;
                return response = Ok(new { response = "Error", deterror });
            }
        }

        public int recalcular_certificados(int id_usuario, int id_cliente, int id_cotizacion, int id_producto)
        {
            Partners_ProductosController PC = new Partners_ProductosController(_config, _context);
            //PC.eliminar_registros_certificado(p_c.id_usuario, 0, 0);

            var eliminar_sublienas = _context.producto_certificado_sublinea.Where(u => u.id_producto == id_producto).ToList();
            var cpc = _context.Cer_producto_cliente.Where(u => u.id_cliente == (id_cotizacion == 0 ? id_usuario: id_cliente)).ToList(); // si la cotizacion es  0 es carrito y el cliente es el mismo usuario 
            var rcp_ = _context.rel_certificado_producto.Where(u => cpc.Any(k => k.id == u.id_certificado)).ToList();
            var rcpd = rcp_.Where(u => eliminar_sublienas.Any(k => k.Id_sublinea == u.id_sub_linea)).ToList(); // las que hay que borrar 
            foreach(var q in rcpd)
                 rcp_.Remove(q); // las restantes 

            List<Partners_ProductosController.ids_cantidad> lista_sublineas = new List<Partners_ProductosController.ids_cantidad>();

            //eliminar 
            foreach (var o in rcp_)
            {
                Partners_ProductosController.ids_cantidad j = new Partners_ProductosController.ids_cantidad();
                j.cantidad = 1;
                j.id = o.id_sub_linea;
                lista_sublineas.Add(j);
            }

            
            Partners_ProductosController.guardar_certificados lista = new Partners_ProductosController.guardar_certificados();
            lista.lista_sublineas = lista_sublineas;

            var id_viaticos = cpc[0].id_viaticos;
            var id_localidad = _context.Cer_viaticos.FirstOrDefault(l => l.id == id_viaticos).id_cat_localidad;
            var cp = _context.Cat_Localidad.FirstOrDefault(l => l.id == id_localidad).cp;

            lista.cp = cp.ToString();
            lista.id_usuario = id_usuario;
            lista.cotizacion_id = id_cotizacion;
            
            PC.save_certificados(lista);

            int res = 0;
            return res;
        }


        // POST: api/Servicios
        [Route("Nuevos_fiscales")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult NuevosFiscales([FromBody]DatosFiscales item)
        {
            IActionResult response;
            try
            {
                if (item == null)
                {
                    response = Ok(new { id = "0", response = "Error" });
                }
                else
                {
                    _context.DatosFiscales.Add(item);
                    _context.SaveChanges();
                    response = Ok(new { id = item.id, response = "Success" });
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { id = "0", response = "Error" });
            }
            return new ObjectResult(response);
        }

        //POST: api/Servicios/Direcciones_cliente
        [Route("get_Direcciones_Cliente")]
        [HttpPost("{Cliente_get_Direcciones_ClienteDirecciones}")]
        public IActionResult get_Direcciones_Cliente([FromBody] ModBusPorId busqueda)
        {
            IActionResult response = Unauthorized();
            var item = (from a in _context.Direcciones_Cliente
                        join b in _context.Clientes on a.id_cliente equals b.id
                        where b.id == busqueda.Id && a.tipo_direccion != null
                        orderby a.tipo_direccion descending
                        select new
                        {
                            id = a.id,
                            calle = a.calle_numero,
                            colonia = a.colonia,
                            cp = a.cp,
                            nombre = a.nombrecontacto,
                            id_estado = a.id_estado,
                            id_municipio = a.id_municipio,
                            telefono = a.telefono,
                            tipo_direccion = a.tipo_direccion,
                            telefono_movil = a.telefono_movil,
                            id_localidad = a.id_localidad,
                            numExt = a.numExt,
                            numInt = a.numInt,
                            Fecha_Estimada = a.Fecha_Estimada,
                            a.id_prefijo_calle
                        }).ToList().Take(2); ;
            if (item == null)
            {
                return response = Ok(new { resultado = "Error", item });
            }
            return response = Ok(new { resultado = "Success", item });
        }


        //POST: api/Servicios/Direcciones_cliente
        [Route("Cliente_Direcciones")]
        [HttpPost("{Cliente_Direcciones}")]
        public IActionResult Cliente_Direcciones([FromBody] ModBusPorId busqueda)
        {
            IActionResult response = Unauthorized();
            var item = (from a in _context.Cat_Direccion
                        join b in _context.Clientes on a.id_cliente equals b.id
                        where b.id == busqueda.Id && a.tipo_direccion != null
                        orderby a.tipo_direccion descending
                        select new
                        {
                            id = a.id,
                            calle = a.calle_numero,
                            colonia = a.colonia,
                            cp = a.cp,
                            nombre = a.nombrecontacto,
                            id_estado = a.id_estado,
                            id_municipio = a.id_municipio,
                            telefono = a.telefono,
                            tipo_direccion = a.tipo_direccion,
                            telefono_movil = a.telefono_movil,
                            id_localidad = a.id_localidad,
                            numExt = a.numExt,
                            numInt = a.numInt,
                            Fecha_Estimada = a.Fecha_Estimada,
                            a.id_prefijo_calle
                        }).ToList().Take(2); ;
            if (item == null)
            {
                return response = Ok(new { resultado = "Error", item });
            }
            return response = Ok(new { resultado = "Success", item });
        }


        // GET: API/Partners_Cotizacion/Direcciones_cliente
        // Controller para obtener la dirección de la cotización
        [Route("Direcciones_cliente")]
        [HttpPost("{Direcciones_cliente}")]
        public IActionResult Direcciones_cliente([FromBody] ModBusPorId busqueda)
        {
            IActionResult response = Unauthorized();
            var item = (from a in _context.direcciones_cotizacion
                        join b in _context.Cotizaciones on a.id_cotizacion equals b.Id
                        where b.Id_Cliente == busqueda.Id && a.tipo_direccion != null
                        orderby a.tipo_direccion ascending
                        select new
                        {
                            id = a.id,
                            calle = a.calle_numero,
                            colonia = a.colonia,
                            cp = a.cp,
                            nombre = a.nombrecontacto,
                            id_estado = a.id_estado,
                            id_municipio = a.id_municipio,
                            telefono = a.telefono,
                            tipo_direccion = a.tipo_direccion,
                            telefono_movil = a.telefono_movil,
                            id_localidad = a.id_localidad,
                            numExt = a.numExt,
                            numInt = a.numInt,
                            Fecha_Estimada = a.Fecha_Estimada,
                            a.id_prefijo_calle,
                        }).ToList();
            if (item == null)
            {
                return response = Ok(new { resultado = "Error", item });
            }
            return response = Ok(new { resultado = "Success", item });
        }

        // POST: api/Servicios/Direcciones_cliente
        [Route("formas_pago_cuenta")]
        [HttpPost("{formas_pago_cuenta}")]
        public IActionResult formas_pago([FromBody] ModBusPorId busqueda)
        {

            //return new ObjectResult(item);
            IActionResult response = Unauthorized();
            var item = (from a in _context.Cat_Formas_Pago
                        select new
                        {
                            id = a.id,
                            formaPago = a.FormaPago
                        }).ToList();
            if (item == null)
            {
                return response = Ok(new { resultado = "Error", item });
            }
            return response = Ok(new { resultado = "Success", item });
        }

        // POST: api/Servicios/tipo_comprobantes_fromap
        [Route("tipo_comprobantes_fromap")]
        [HttpPost("{tipo_comprobantes_fromap}")]
        public IActionResult tipo_comprobantes_fromap([FromBody] ModBusPorId busqueda)
        {
            //return new ObjectResult(item);
            IActionResult response = Unauthorized();
            var item = (from a in _context.formas_pago_tipos_comprobantes
                        join f in _context.tipos_comprobantes on a.id_tipo_comprobantes equals f.id
                        where a.id_Cat_Formas_Pago == busqueda.Id
                        select new
                        {
                            id = a.id_tipo_comprobantes,
                            tipo_pago = f.tipo_pago
                        }).ToList();
            if (item == null)
            {
                return response = Ok(new { resultado = "Error", item });
            }
            return response = Ok(new { resultado = "Success", item });
        }


        // POST: api/Servicios/Direcciones_cliente
        [Route("condiciones_pago_cuenta")]
        [HttpPost("{condiciones_pago_cuenta}")]
        public IActionResult condiciones_pago_cuenta([FromBody] ModBusPorId busqueda)
        {

            //return new ObjectResult(item);
            IActionResult response = Unauthorized();
            var item = (from a in _context.Cat_CondicionesPago
                        join b in _context.Users on a.id_cuenta equals b.id_cuenta
                        where b.id == busqueda.Id
                        select new
                        {
                            id = a.id,
                            id_Cat_Formas_Pago = a.id_Cat_Formas_Pago
                        }).ToList();
            if (item == null)
            {
                return response = Ok(new { resultado = "Error", item });
            }
            return response = Ok(new { resultado = "Success", item });
        }

        // POST: api/Servicios/Direcciones_cliente
        [Route("fiscales_cliente")]
        [HttpPost("{fiscales_cliente}")]
        public IActionResult fiscales_cliente([FromBody] ModBusPorId busqueda)
        {

            //return new ObjectResult(item);
            IActionResult response = Unauthorized();
            var item = (from a in _context.DatosFiscales
                        join b in _context.Clientes on a.id_cliente equals b.id
                        where b.id == busqueda.Id

                        select new
                        {
                            id = a.id,
                            nombre_fact = a.nombre_fact,
                            razon_social = a.razon_social,
                            rfc = a.rfc,
                            email = a.email,
                            calle_numero = a.calle_numero,
                            cp = a.cp,
                            id_estado = a.id_estado,
                            id_municipio = a.id_municipio,
                            colonia = a.colonia,
                            Ext_fact = a.Ext_fact,
                            Int_fact = a.Int_fact,
                            telefono_fact = a.telefono_fact,
                            id_cliente = a.id_cliente,
                        }).ToList();
            if (item == null)
            {
                return response = Ok(new { resultado = "Error", item });
            }
            return response = Ok(new { resultado = "Success", item });
        }

        // POST: api/Servicios
        [Route("AddProductosCarrito")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult AddProductoCarrito([FromBody]Productos_Carrito item)
        {
            IActionResult response;

            try
            {
                if (item == null)
                {
                    response = Ok(new { id = "0", response = "Error" });
                }
                else
                {
                    var cat_p = _context.Cat_Productos.FirstOrDefault(c => c.id == item.Id_Producto);
                    if (cat_p != null)
                    {
                        //La sublinea 113 producto base de home program (paquete de 3 horas)
                        if (cat_p.id_sublinea == 113)
                        {
                            var carrito_home = (from a in _context.Cat_Productos
                                                join b in _context.Productos_Carrito on a.id equals b.Id_Producto
                                                where a.id_linea == 38 && b.id_usuario == item.id_usuario
                                                select b).ToList();
                            _context.Productos_Carrito.RemoveRange(carrito_home);
                        }
                    }
                    var precio_prods = _context.Cat_Productos.FirstOrDefault(p => p.id == item.Id_Producto);
                    var pc = _context.Productos_Carrito.FirstOrDefault(s => s.Id_Producto == item.Id_Producto && s.id_usuario == item.id_usuario);
                    if (pc != null)
                    {
                        if (precio_prods != null)
                        {
                            pc.cantidad = pc.cantidad + (item.cantidad == 0 ? 1 : item.cantidad);
                            pc.precio_condiciones_com = precio_prods.precio_sin_iva;
                            pc.precio_descuento = precio_prods.precio_sin_iva;
                            pc.precio_lista = precio_prods.precio_sin_iva;
                            pc.iva_cond_comerciales = precio_prods.precio_con_iva - precio_prods.precio_sin_iva;
                            pc.iva_precio_descuento = precio_prods.precio_con_iva - precio_prods.precio_sin_iva;
                            pc.iva_precio_lista = precio_prods.precio_con_iva - precio_prods.precio_sin_iva;
                            _context.Productos_Carrito.Update(pc);
                            _context.SaveChanges();
                            //if (cat_p != null)
                            //{
                            //    if (cat_p.id_linea == 38)
                            //    {
                            //        recalcular_hp(0, pc.id_usuario, true, false);
                            //    }
                            //}

                            response = Ok(new { id = item.Id, response = "Success" });
                        }
                        else
                            response = Ok(new { id = "0", response = "Error" });

                    }
                    else
                    {
                        if (precio_prods != null)
                        {
                            item.cantidad = item.cantidad == 0 ? 1 : item.cantidad;

                            item.precio_condiciones_com = precio_prods.precio_sin_iva;
                            item.precio_descuento = precio_prods.precio_sin_iva;
                            item.precio_lista = precio_prods.precio_sin_iva;
                            item.iva_cond_comerciales = precio_prods.precio_con_iva - precio_prods.precio_sin_iva;
                            item.iva_precio_descuento = precio_prods.precio_con_iva - precio_prods.precio_sin_iva;
                            item.iva_precio_lista = precio_prods.precio_con_iva - precio_prods.precio_sin_iva;
                            _context.Productos_Carrito.Add(item);
                            _context.SaveChanges();
                            //if (cat_p != null)
                            //{
                            //    if (cat_p.id_linea == 38)
                            //    {
                            //        recalcular_hp(0, pc.id_usuario, true, false);
                            //    }
                            //}
                            response = Ok(new { id = item.Id, response = "Success" });
                        }
                        else
                            response = Ok(new { id = "0", response = "Error" });

                    }
                   

                    Partners_ProductosController PC = new Partners_ProductosController(_config, _context);
                    var current_cot = PC.crear_cotizacion_carrito(item.id_usuario);
                    /// funcion home program ticktes 
                    var id_c_cu = _context.Cotizaciones.FirstOrDefault(c => c.Id_Cliente == item.id_usuario).Id;
                    recalcular_hp(Convert.ToInt32(id_c_cu), item.id_usuario, true, false);
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { id = "0", response = "Error" });
            }
            return new ObjectResult(response);
        }

        
        public void crear_hp_tickets(int id_cliente, int horas, int id_cotizacion, bool estatus, int id_producto)
        {
            var hpc = _context.home_producto_cliente.FirstOrDefault(h => h.id_cliente == id_cliente && h.id_cotizacion == id_cotizacion);
            if (hpc != null)
            {
                hpc.horas = horas;
                _context.home_producto_cliente.Update(hpc);
            }
            else
            {
                Home_producto_cliente _h = new Home_producto_cliente();

                _h.folio = "0000" + id_cliente.ToString() + DateTime.Now.Second.ToString();
                _h.id_cliente = id_cliente;
                _h.no_visitas = 0;
                _h.costo = 0;
                _h.horas = horas;
                _h.id_cotizacion = id_cotizacion;
                _h.estatus_activo = estatus;
                _h.estatus_venta = estatus;
                _h.creado = DateTime.Now;
                _h.creadopor = 0;
                _h.actualizado = DateTime.Now;
                _h.actualizadopor = 0;
                _h.id_producto = id_producto;
                _context.home_producto_cliente.Add(_h);
            }
            _context.SaveChanges();

        }

        public int recalcular_hp(int id_cotizacion, int id_cliente, bool es_carrito, bool estatus)
        {
            var ph_basic = _context.Cat_Productos.Where(p => p.id_sublinea == 113).ToList();
            var ph_extra = _context.Cat_Productos.Where(p => p.id_sublinea == 143).ToList();
            int respuesta = 0;
            if(es_carrito)
            {
                var lista_car_hp = _context.Productos_Carrito.Where(p => p.id_usuario == id_cliente && ph_basic.Any(e => e.id == p.Id_Producto)).ToList();
                if (lista_car_hp.Count == 0)
                {
                    var hp_tickets = _context.home_producto_cliente.Where(p => p.id_cotizacion == id_cotizacion && p.id_cliente == id_cliente);
                    _context.home_producto_cliente.RemoveRange(hp_tickets);
                    _context.SaveChanges();
                }

                foreach (var h in lista_car_hp)
                {
                    int horas = 3;
                    var extra_hp = _context.productos_relacionados.FirstOrDefault(e => e.id_producto == h.Id_Producto);
                    var car_hp_ex = _context.Productos_Carrito.Where(p => p.id_usuario == id_cliente && p.Id_Producto == extra_hp.id_producto_2).ToList();
                    if(car_hp_ex.Count() > 0)
                    {
                        horas = horas + car_hp_ex[0].cantidad;
                    }
                    crear_hp_tickets(id_cliente, horas, id_cotizacion, estatus, h.Id_Producto);
                }
            }
            else
            {
                var id_c_c = _context.Cotizaciones.FirstOrDefault(p => p.Id == id_cotizacion).Id_Cliente;

                var hp_tickets = _context.home_producto_cliente.Where(p => p.id_cotizacion == id_cotizacion && p.id_cliente == id_c_c);
                foreach (var g in hp_tickets)
                    _context.home_producto_cliente.Remove(g);
                _context.SaveChanges();
                var lista_cot_hp = _context.Cotizacion_Producto.Where(p => p.Id_Cotizacion == id_cotizacion && ph_basic.Any(e => e.id == p.Id_Producto)).ToList();
                var lista_cot_hp_ex = _context.Cotizacion_Producto.Where(p => p.Id_Cotizacion == id_cotizacion && ph_extra.Any(e => e.id == p.Id_Producto)).ToList();

                foreach (var h in lista_cot_hp)
                {
                    int horas = 3;
                    var extra_hp = _context.productos_relacionados.FirstOrDefault(e => e.id_producto == h.Id_Producto);
                    var car_hp_ex = _context.Cotizacion_Producto.Where(p => p.Id_Cotizacion == id_cotizacion && p.Id_Producto == extra_hp.id_producto_2).ToList();
                    if (car_hp_ex.Count() > 0)
                    {
                        horas = horas + car_hp_ex[0].cantidad;
                    }
                    crear_hp_tickets(id_cliente, horas, id_cotizacion, estatus, h.Id_Producto);
                }
            }

            return respuesta;
        }

        public int ajustar_sublineas_hp(int id_cotizacion, int id_producto, int id_usuario_cliente, bool estatus, bool es_carrito)
        {
            int id_current = id_cotizacion;

           // var lista_hp = _context.Productos_Carrito.Where(p => p.id_usuario == id_usuario && p.Id_Producto == id_producto).ToList();
            var extra_hp = _context.productos_relacionados.FirstOrDefault(e => e.id_producto == id_producto);

            if(es_carrito && extra_hp != null)
            {
                var lista_extra_hp = _context.Productos_Carrito.Where(p => p.id_usuario == id_usuario_cliente && p.Id_Producto == extra_hp.id_producto_2).ToList();
                foreach (var hpx in lista_extra_hp)
                    _context.Productos_Carrito.Remove(hpx);
                _context.SaveChanges();

                recalcular_hp(id_cotizacion, id_usuario_cliente, true, estatus);


            }


            if(!es_carrito && extra_hp != null)
            {
                var lista_extra_hp = _context.Cotizacion_Producto.Where(p => p.Id_Cotizacion == id_cotizacion && p.Id_Producto == extra_hp.id_producto_2).ToList();
                foreach (var hpx in lista_extra_hp)
                    _context.Cotizacion_Producto.Remove(hpx);
                _context.SaveChanges();

                recalcular_hp(id_cotizacion, id_usuario_cliente, false, estatus);

            }
            return id_cotizacion;
        }

        [Route("save_homep_sublineas")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult save_homep_sublineas([FromBody]guardar_home_program item)
        {
            IActionResult response = Unauthorized();

            try
            {
                if (save_hp_cotizacion(item) == 1)
                    return response = Ok(new { result = "Success", detalle = "Servicios insertados en la cotizacion/carrito" });
                else
                    return response = Ok(new { result = "Error", detalle = "No hay Certificados asociados a la sublineas" });
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message });
            }
        }

        public int save_hp_cotizacion(guardar_home_program lista)
        {
            int respuesta = 0;


            if (lista != null)
            {

                if (lista.cotizacion_id == 0)
                {
                    var id_c_cu = _context.Cotizaciones.FirstOrDefault(c => c.Id_Cliente == lista.id_usuario).Id;
                    lista.cotizacion_id = Convert.ToInt32(id_c_cu);
                    recalcular_hp(lista.cotizacion_id, lista.id_usuario, true, false);
                    
                }
                else
                {
                    recalcular_hp(lista.cotizacion_id, lista.id_usuario, false, false);
                }


                //var hpc = (from h in _context.home_producto_cliente
                //           where h.id_cotizacion == lista.cotizacion_id
                //           select h).FirstOrDefault();
                var hpc = _context.home_producto_cliente.FirstOrDefault(h => h.id_cotizacion == lista.cotizacion_id);
                if (hpc != null)
                {
                    var del_reg = _context.rel_homep_productos.Where(a => a.id_homep == hpc.id);
                    foreach (sublineas_cantidad item in lista.lista_sublineas)
                    {
                        
                        _context.rel_homep_productos.RemoveRange(del_reg);
                        rel_homep_producto rel_ = new rel_homep_producto();
                        rel_.id_homep = hpc.id;
                        rel_.estatus_activo = false;
                        rel_.cantidad = item.cantidad;
                        rel_.id_sub_linea = item.id;
                        rel_.creado = DateTime.Now;
                        _context.rel_homep_productos.Add(rel_);
                    }

                    //_context.home_producto_cliente.Add(hpc);
                    _context.SaveChanges();
                }
                respuesta = 1;

            }
            else
            {
                respuesta = 0;
                //if (lista.cotizacion_id > 0)   ////////// hay que borrar todo lo que exista de Certificados en carrito y en cotizacion
                //{
                //    var id_clie_cot = _context.Cotizaciones.FirstOrDefault(h => h.Id == lista.cotizacion_id).Id_Cliente;
                //    eliminar_registros_certificado(lista.id_usuario, id_clie_cot, lista.cotizacion_id);
                //}
                //else
                //{
                //    eliminar_registros_certificado(lista.id_usuario, 0, 0);
                //}
            }
            return respuesta;
        }

        public respuesta_valida_cp validar_homep_cert_cp(int id_cotizacion, string cp)
        {
            var cp_ = Convert.ToInt32(cp);
            respuesta_valida_cp respuesta_valida_cp = new respuesta_valida_cp();
            var hps = _context.home_producto_cliente.Where(h => h.id_cotizacion == id_cotizacion).ToList();
            List<Cat_Localidad> localidades_hp = new List<Cat_Localidad>();

            if (hps.Count > 0)
            {
                localidades_hp = get_localidades_home_program(hps[0].id_producto);
                if(localidades_hp.Count > 0 )
                {
                    var cp_ok = localidades_hp.Where(l => l.cp == cp_).ToList();
                    if(cp_ok.Count > 0)
                        respuesta_valida_cp.valido_cp_home = true;
                    else
                        respuesta_valida_cp.valido_cp_home = false;
                }
                else // esto no deberia pasar, un certificado siempre podría darte un grupo de cp, a menos qu eno este aosciado a estados en la tabla home_Producto_Estados
                    respuesta_valida_cp.valido_cp_home = true; 
            }
            else // no tiene home program 
            {
                respuesta_valida_cp.valido_cp_home = true;
            }

            var cpc = _context.Cer_producto_cliente.Where(u => u.id_cotizacion == id_cotizacion).ToList();
            var tiene_cert = (from a in _context.Cotizacion_Producto
                              join b in _context.Cat_Productos on a.Id_Producto equals b.id
                              where a.Id_Cotizacion == id_cotizacion && b.id_linea == 36
                              select a).ToList();
           // var rcp = _context.rel_certificado_producto.Where(u => cpc.Any(k => k.id == u.id_certificado)).ToList();
           if (cpc.Count > 0 && tiene_cert.Count > 0)
            {
                int id_viaticos = cpc[0].id_viaticos;
                int id_localidad_cer = _context.Cer_viaticos.FirstOrDefault(y => y.id == id_viaticos).id_cat_localidad;
                long cp_cer = _context.Cat_Localidad.FirstOrDefault(w => w.id == id_localidad_cer).cp;
                if (cp_ == cp_cer) respuesta_valida_cp.valido_cp_cert = true;
                else respuesta_valida_cp.valido_cp_cert = false;

            }
           else // no tiene certificado 
            {
                respuesta_valida_cp.valido_cp_cert = true;
            }

            return respuesta_valida_cp;
        }


        public List<Cat_Localidad> get_localidades_home_program(int id_producto_home)
        {

            List<Cat_Localidad> localidades_hp = (from a in _context.Cat_Localidad
                             join b in _context.Cat_Municipio on a.municipio.id equals b.id
                             join c in _context.Cat_Estado on b.estado.id equals c.id
                             join hpe in _context.home_Producto_Estados on c.id equals hpe.id_estado
                             where hpe.id_producto_home == id_producto_home
                             select new Cat_Localidad
                             {
                                 id = a.id,
                                 cp = a.cp,
                                 municipio = a.municipio,
                                 desc_localidad = a.desc_localidad,
                                 zona = a.zona
                             }).ToList();

            return localidades_hp;
        }


        // POST: api/Servicios
        [Route("AddProductoCotiza")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult AddProductoCotiza([FromBody]Cotizacion_Producto item)
        {
            IActionResult response;

            try
            {

                if (item == null)
                {
                    response = Ok(new { id = "0", response = "Error" });
                }
                else
                {
                    var cat_p = _context.Cat_Productos.FirstOrDefault(c => c.id == item.Id_Producto);
                    if (cat_p != null)
                    {
                        //La sublinea 113 producto base de home program (paquete de 3 horas)
                        if (cat_p.id_sublinea == 113)
                        {
                            var cotiza_home = (from a in _context.Cat_Productos
                                                join b in _context.Cotizacion_Producto  on a.id equals b.Id_Producto
                                                where a.id_linea == 38 && b.Id_Cotizacion == item.Id_Cotizacion
                                                select b).ToList();
                            _context.Cotizacion_Producto.RemoveRange(cotiza_home);
                        }
                    }
                    var precio_prods = _context.Cat_Productos.FirstOrDefault(p => p.id == item.Id_Producto);
                    var cotiza = _context.Cotizaciones.FirstOrDefault(c => c.Id == item.Id_Cotizacion);
                    var pc = _context.Cotizacion_Producto.FirstOrDefault(s => s.Id_Producto == item.Id_Producto && s.Id_Cotizacion == item.Id_Cotizacion);
                    if (pc != null)
                    {
                        if (precio_prods != null)
                        {
                            pc.cantidad = pc.cantidad + (item.cantidad == 0 ? 1 : item.cantidad);
                            pc.precio_condiciones_com = precio_prods.precio_sin_iva;
                            pc.precio_descuento = precio_prods.precio_sin_iva;
                            pc.precio_lista = precio_prods.precio_sin_iva;
                            pc.iva_cond_comerciales = precio_prods.precio_con_iva - precio_prods.precio_sin_iva;
                            pc.iva_precio_descuento = precio_prods.precio_con_iva - precio_prods.precio_sin_iva;
                            pc.iva_precio_lista = precio_prods.precio_con_iva - precio_prods.precio_sin_iva;

                            _context.Cotizacion_Producto.Update(pc);
                            _context.SaveChanges();
                            response = Ok(new { id = item.Id, response = "Success" });
                        }
                        else
                            response = Ok(new { id = "0", response = "Error" });

                    }
                    else
                    {

                        if (precio_prods != null)
                        {
                            item.cantidad = item.cantidad == 0 ? 1 : item.cantidad;
                            item.precio_condiciones_com = precio_prods.precio_sin_iva;
                            item.precio_descuento = precio_prods.precio_sin_iva;
                            item.precio_lista = precio_prods.precio_sin_iva;
                            item.iva_cond_comerciales = precio_prods.precio_con_iva - precio_prods.precio_sin_iva;
                            item.iva_precio_descuento = precio_prods.precio_con_iva - precio_prods.precio_sin_iva;
                            item.iva_precio_lista = precio_prods.precio_con_iva - precio_prods.precio_sin_iva;
                            _context.Cotizacion_Producto.Add(item);
                            _context.SaveChanges();
                            response = Ok(new { id = item.Id, response = "Success" });
                        }
                        else
                            response = Ok(new { id = "0", response = "Error" });
                    }

                    /// funcion home program ticktes 
                    recalcular_hp(Convert.ToInt32(item.Id_Cotizacion), Convert.ToInt32(cotiza.Id_Cliente), false, false);

                    //var Cotizacion_productos_res = _context.Cotizacion_Producto.FromSql("sp_add_registros_home_tickets " + item.Id_Cotizacion.ToString()).ToList();
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { id = "0", response = "Error" });
            }
            return new ObjectResult(response);
        }

        // PUT: api/Partners_Cotizacion/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        // POST: api/Partners_Productos
        [Route("CargarClienteporId")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult CargarClienteporId([FromBody]ModBusPorId busqueda)
        {
            IActionResult response = Unauthorized();
            var item = (from x in _context.Cotizaciones
                        join a in _context.Clientes on x.Id_Cliente equals a.id
                        where x.Id == Convert.ToInt64(busqueda.Id)

                        select new
                        {
                            id = x.Id,
                            id_Cli = x.Id_Cliente,
                            nombre = a.nombre,
                            paterno = a.paterno,
                            materno = a.materno,
                            id_estado = x.Id_Estado_Instalacion,
                            email = a.email,
                            telefono = a.telefono,
                            telefono_movil = a.telefono_movil,
                            montocotiza = (x.importe_precio_lista + x.iva_precio_lista),
                            x.ibs
                        }).ToList();
            if (item == null)
            {
                return response = Ok(new { token = "Error", item });
            }
            return response = Ok(new { token = "Success", item });
        }

        // POST: api/Partners_Productos
        [Route("CargarVendedoresporId")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult CargarVendedoresporId([FromBody]ModBusPorId busqueda)
        {
            IActionResult response = Unauthorized();
            var item = (from x in _context.Cotizaciones
                        join a in _context.Users on x.Id_Vendedor equals a.id
                        where x.Id == Convert.ToInt64(busqueda.Id)

                        select new
                        {
                            nombre = a.name,
                            paterno = a.paterno,
                            materno = a.materno,
                            id_estado = x.Id_Estado_Instalacion,
                            email = a.email,
                            telefono = a.telefono,
                            telefono_movil = a.telefono_movil,
                            IdVendedor = x.Id_Vendedor,
                            observaciones = x.Observaciones
                        }).ToList();
            if (item == null)
            {
                return response = Ok(new { token = "Error", item });
            }
            return response = Ok(new { token = "Success", item });
        }

        // POST: api/Partners_Productos
        [Route("modelos_autocomplete")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult modelos_autocomplete([FromBody]busquedaLibre busqueda)
        {
            IActionResult response = Unauthorized();
            var item = _context.Cat_Productos.Select(m => new { m.modelo }).Distinct().ToList();

            if (item == null)
            {
                return response = Ok(new { result = "Error", item });
            }
            return response = Ok(new { result = "Success", item });
        }

        ////////////////////// POST AGREGAR Y EDITAR COTIZACION DIRECCIONES (Cat_Direccion)
        // POST: api/Servicios/Agregar_Direccion
        //[Route("Agregar_Direccion")]
        //[HttpPost("{Agregar_Direccion}")]
        //public IActionResult Agregar_Direccion([FromBody] Cat_Direccion item) 
        //{
        //    var result = new Models.Response();
        //    try
        //    {
        //        if (item == null)
        //        {
        //            return BadRequest();
        //        }
        //        else
        //        {
        //            _context.Cat_Direccion.Add(item);
        //            _context.SaveChanges();
        //            result = new Models.Response
        //            {
        //                response = "Success"
        //            };
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        result = new Models.Response
        //        {
        //            response = "Error"
        //        };
        //    }
        //    return new ObjectResult(result);
        //}

        //Servicio para insertar/editar direcciones de cotizacion y valida si el cliente indicado tiene un tipo de direccion indicado, si no lo tiene la inserta en las direcciones del cliente (cat_direccion)

        public int set_cat_direcciones(obj_dir_cliente obj_dir_cliente)
        {
            int respuesta = 0;
            Direccion_Cotizacion ins_cot = obj_dir_cliente.direcciones.FirstOrDefault(c => c.tipo_direccion == 1);
            Direccion_Cotizacion env_cot = obj_dir_cliente.direcciones.FirstOrDefault(c => c.tipo_direccion == 2);

            if (obj_dir_cliente.cat_dir) //indica si hay que actualizar Cat_direcciones
            {
                List<Cat_Direccion> dir_ins_l = _context.Cat_Direccion.Where(d => d.id_cliente == obj_dir_cliente.Id_cliente && d.tipo_direccion == 1).ToList();
                List<Cat_Direccion> dir_env_l = _context.Cat_Direccion.Where(d => d.id_cliente == obj_dir_cliente.Id_cliente && d.tipo_direccion == 2).ToList();
                //cat_direcciones Instalacion
                if (dir_ins_l.Count > 0)  //actualiza
                {
                    Cat_Direccion cat_ins = _context.Cat_Direccion.FirstOrDefault(z => z.id == dir_ins_l[0].id);
                    cat_ins.id_cliente = obj_dir_cliente.Id_cliente;
                    cat_ins.calle_numero = ins_cot.calle_numero;
                    cat_ins.cp = ins_cot.cp;
                    cat_ins.id_estado = ins_cot.id_estado;
                    cat_ins.id_municipio = ins_cot.id_municipio;
                    cat_ins.colonia = ins_cot.colonia;
                    cat_ins.telefono = ins_cot.telefono;
                    cat_ins.estatus = ins_cot.estatus;
                    cat_ins.creado = ins_cot.creado;
                    cat_ins.creadopor = ins_cot.creadopor;
                    cat_ins.actualizado = ins_cot.actualizado;
                    cat_ins.actualizadopor = ins_cot.actualizadopor;
                    cat_ins.tipo_direccion = ins_cot.tipo_direccion;
                    cat_ins.nombrecontacto = ins_cot.nombrecontacto;
                    cat_ins.numExt = ins_cot.numExt;
                    cat_ins.numInt = ins_cot.numInt;
                    cat_ins.telefono_movil = ins_cot.telefono_movil;
                    cat_ins.id_localidad = ins_cot.id_localidad;
                    cat_ins.Fecha_Estimada = ins_cot.Fecha_Estimada;
                    cat_ins.id_prefijo_calle = ins_cot.id_prefijo_calle;
                    _context.Cat_Direccion.Update(cat_ins);
                } 
                else //inserta
                {
                    Cat_Direccion cat_ins = new Cat_Direccion();
                    cat_ins.id_cliente = obj_dir_cliente.Id_cliente;
                    cat_ins.calle_numero = ins_cot.calle_numero;
                    cat_ins.cp = ins_cot.cp;
                    cat_ins.id_estado = ins_cot.id_estado;
                    cat_ins.id_municipio = ins_cot.id_municipio;
                    cat_ins.colonia = ins_cot.colonia;
                    cat_ins.telefono = ins_cot.telefono;
                    cat_ins.estatus = ins_cot.estatus;
                    cat_ins.creado = ins_cot.creado;
                    cat_ins.creadopor = ins_cot.creadopor;
                    cat_ins.actualizado = ins_cot.actualizado;
                    cat_ins.actualizadopor = ins_cot.actualizadopor;
                    cat_ins.tipo_direccion = ins_cot.tipo_direccion;
                    cat_ins.nombrecontacto = ins_cot.nombrecontacto;
                    cat_ins.numExt = ins_cot.numExt;
                    cat_ins.numInt = ins_cot.numInt;
                    cat_ins.telefono_movil = ins_cot.telefono_movil;
                    cat_ins.id_localidad = ins_cot.id_localidad;
                    cat_ins.Fecha_Estimada = ins_cot.Fecha_Estimada;
                    cat_ins.id_prefijo_calle = ins_cot.id_prefijo_calle;
                    _context.Cat_Direccion.Add(cat_ins);
                }

                //cat_direcciones envio
                if (dir_env_l.Count > 0)  //actualiza
                {
                    Cat_Direccion cat_env = _context.Cat_Direccion.FirstOrDefault(z => z.id == dir_env_l[0].id);
                    cat_env.id_cliente = obj_dir_cliente.Id_cliente;
                    cat_env.calle_numero = env_cot.calle_numero;
                    cat_env.cp = env_cot.cp;
                    cat_env.id_estado = env_cot.id_estado;
                    cat_env.id_municipio = env_cot.id_municipio;
                    cat_env.colonia = env_cot.colonia;
                    cat_env.telefono = env_cot.telefono;
                    cat_env.estatus = env_cot.estatus;
                    cat_env.creado = env_cot.creado;
                    cat_env.creadopor = env_cot.creadopor;
                    cat_env.actualizado = env_cot.actualizado;
                    cat_env.actualizadopor = env_cot.actualizadopor;
                    cat_env.tipo_direccion = env_cot.tipo_direccion;
                    cat_env.nombrecontacto = env_cot.nombrecontacto;
                    cat_env.numExt = env_cot.numExt;
                    cat_env.numInt = env_cot.numInt;
                    cat_env.telefono_movil = env_cot.telefono_movil;
                    cat_env.id_localidad = env_cot.id_localidad;
                    cat_env.Fecha_Estimada = env_cot.Fecha_Estimada;
                    cat_env.id_prefijo_calle = env_cot.id_prefijo_calle;
                    _context.Cat_Direccion.Update(cat_env);
                }
                else //inserta
                {
                    Cat_Direccion cat_env = new Cat_Direccion();
                    cat_env.id_cliente = obj_dir_cliente.Id_cliente;
                    cat_env.calle_numero = env_cot.calle_numero;
                    cat_env.cp = env_cot.cp;
                    cat_env.id_estado = env_cot.id_estado;
                    cat_env.id_municipio = env_cot.id_municipio;
                    cat_env.colonia = env_cot.colonia;
                    cat_env.telefono = env_cot.telefono;
                    cat_env.estatus = env_cot.estatus;
                    cat_env.creado = env_cot.creado;
                    cat_env.creadopor = env_cot.creadopor;
                    cat_env.actualizado = env_cot.actualizado;
                    cat_env.actualizadopor = env_cot.actualizadopor;
                    cat_env.tipo_direccion = env_cot.tipo_direccion;
                    cat_env.nombrecontacto = env_cot.nombrecontacto;
                    cat_env.numExt = env_cot.numExt;
                    cat_env.numInt = env_cot.numInt;
                    cat_env.telefono_movil = env_cot.telefono_movil;
                    cat_env.id_localidad = env_cot.id_localidad;
                    cat_env.Fecha_Estimada = env_cot.Fecha_Estimada;
                    cat_env.id_prefijo_calle = env_cot.id_prefijo_calle;
                    _context.Cat_Direccion.Add(cat_env);
                }
            }

            if (obj_dir_cliente.dir_clie) //indica si hay que actualizar Direcciones_Clientees
            {
                List<Direcciones_Cliente> dir_ins_l = _context.Direcciones_Cliente.Where(d => d.id_cliente == obj_dir_cliente.Id_cliente && d.tipo_direccion == 1).ToList();
                List<Direcciones_Cliente> dir_env_l = _context.Direcciones_Cliente.Where(d => d.id_cliente == obj_dir_cliente.Id_cliente && d.tipo_direccion == 2).ToList();
                //Direcciones_Clientees Instalacion
                if (dir_ins_l.Count > 0)  //actualiza
                {
                    Direcciones_Cliente clie_ins = _context.Direcciones_Cliente.FirstOrDefault(z => z.id == dir_ins_l[0].id);
                    clie_ins.id_cliente = obj_dir_cliente.Id_cliente;
                    clie_ins.cp = ins_cot.cp;
                    clie_ins.id_estado = ins_cot.id_estado;
                    clie_ins.id_municipio = ins_cot.id_municipio;
                    clie_ins.id_localidad = ins_cot.id_localidad;
                    clie_ins.colonia = ins_cot.id_localidad.ToString();
                    clie_ins.id_prefijo_calle = ins_cot.id_prefijo_calle;
                    clie_ins.calle_numero = ins_cot.calle_numero;
                    clie_ins.numExt = ins_cot.numExt;
                    clie_ins.numInt = ins_cot.numInt;
                    clie_ins.nombrecontacto = ins_cot.nombrecontacto;
                    clie_ins.telefono = ins_cot.telefono;
                    clie_ins.telefono_movil = ins_cot.telefono_movil;
                    clie_ins.tipo_direccion = ins_cot.tipo_direccion;
                    clie_ins.creado = ins_cot.creado;
                    _context.Direcciones_Cliente.Update(clie_ins);
                }
                else //inserta
                {
                    Direcciones_Cliente clie_ins = new Direcciones_Cliente();
                    clie_ins.id_cliente = obj_dir_cliente.Id_cliente;
                    clie_ins.cp = ins_cot.cp;
                    clie_ins.id_estado = ins_cot.id_estado;
                    clie_ins.id_municipio = ins_cot.id_municipio;
                    clie_ins.id_localidad = ins_cot.id_localidad;
                    clie_ins.colonia = ins_cot.id_localidad.ToString();
                    clie_ins.id_prefijo_calle = ins_cot.id_prefijo_calle;
                    clie_ins.calle_numero = ins_cot.calle_numero;
                    clie_ins.numExt = ins_cot.numExt;
                    clie_ins.numInt = ins_cot.numInt;
                    clie_ins.nombrecontacto = ins_cot.nombrecontacto;
                    clie_ins.telefono = ins_cot.telefono;
                    clie_ins.telefono_movil = ins_cot.telefono_movil;
                    clie_ins.tipo_direccion = ins_cot.tipo_direccion;
                    clie_ins.creado = ins_cot.creado;
                    _context.Direcciones_Cliente.Add(clie_ins);
                }

                //Direcciones_Clientees envio
                if (dir_env_l.Count > 0)  //actualiza
                {
                    Direcciones_Cliente clie_env = _context.Direcciones_Cliente.FirstOrDefault(z => z.id == dir_env_l[0].id);
                    clie_env.id_cliente = obj_dir_cliente.Id_cliente;
                    clie_env.cp = env_cot.cp;
                    clie_env.id_estado = env_cot.id_estado;
                    clie_env.id_municipio = env_cot.id_municipio;
                    clie_env.id_localidad = env_cot.id_localidad;
                    clie_env.colonia = env_cot.id_localidad.ToString();
                    clie_env.id_prefijo_calle = env_cot.id_prefijo_calle;
                    clie_env.calle_numero = env_cot.calle_numero;
                    clie_env.numExt = env_cot.numExt;
                    clie_env.numInt = env_cot.numInt;
                    clie_env.nombrecontacto = env_cot.nombrecontacto;
                    clie_env.telefono = env_cot.telefono;
                    clie_env.telefono_movil = env_cot.telefono_movil;
                    clie_env.tipo_direccion = env_cot.tipo_direccion;
                    clie_env.creado = env_cot.creado;
                    _context.Direcciones_Cliente.Update(clie_env);
                }
                else //inserta
                {
                    Direcciones_Cliente clie_env = new Direcciones_Cliente();
                    clie_env.id_cliente = obj_dir_cliente.Id_cliente;
                    clie_env.cp = env_cot.cp;
                    clie_env.id_estado = env_cot.id_estado;
                    clie_env.id_municipio = env_cot.id_municipio;
                    clie_env.id_localidad = env_cot.id_localidad;
                    clie_env.colonia = env_cot.id_localidad.ToString();
                    clie_env.id_prefijo_calle = env_cot.id_prefijo_calle;
                    clie_env.calle_numero = env_cot.calle_numero;
                    clie_env.numExt = env_cot.numExt;
                    clie_env.numInt = env_cot.numInt;
                    clie_env.nombrecontacto = env_cot.nombrecontacto;
                    clie_env.telefono = env_cot.telefono;
                    clie_env.telefono_movil = env_cot.telefono_movil;
                    clie_env.tipo_direccion = env_cot.tipo_direccion;
                    clie_env.creado = env_cot.creado;
                    _context.Direcciones_Cliente.Add(clie_env);
                }
            }

            _context.SaveChanges();
            return respuesta;

        }

        [Route("Validar_Servicios_Cot")]
        [HttpPost("Validar_Servicios_Cot")]
        public IActionResult ValidarServiciosCotizacion([FromBody]ModBusPorTresId ids)
        {
            //Id1 es codigo postal y id2 es cotizacion, el Id3 es el cliente, en caso de que cotizacion aun no este generada
            string cp1 = ids.Id1.ToString().Length < 5 ? "0" + ids.Id1.ToString() : ids.Id1.ToString();
            
            IActionResult response = Unauthorized();
            if (ids.Id2 == 0)
            {
                ids.Id2 = (int)_context.Cotizaciones.FirstOrDefault(c => c.Id_Cliente == ids.Id3).Id;
            }
            var res = validar_homep_cert_cp(ids.Id2, cp1);
            if (res == null)
            {
                return response = Ok(new { result = "Error", item = res, detalle = "No se puede" });
            }
            else
                return response = Ok(new { result = "Success", item = res });
        }

        [Route("crear_editar_direcciones_cotizacion")] // api/Partners_Cotizacion/Agregar_Direccion
        [HttpPost("crear_editar_direcciones_cotizacion")] // NOTA: Nombre del ENDPOINT: Agregar_Direccion_Cotizacion y descomentar el ENDPOINT de arriba (Agregar_Direccion)
        public IActionResult crear_editar_direcciones_cotizacion([FromBody] obj_dir_cliente objDirecciones)
        {
            var result = new Models.Response();
            IActionResult response = Unauthorized();
            string msg = "";
            try
            {
                if (objDirecciones.direcciones == null)
                {
                    return BadRequest();
                }
                else
                {
                    int i = 1;
                    foreach (var item in objDirecciones.direcciones)
                    {
                        msg = msg + (msg != "" ? ", " : "");
                        // Agregar registro
                        if (item.id == 0)
                        {
                            _context.direcciones_cotizacion.Add(item);
                            //Valida si existe una direccion de cliente por id_cliente y por tipo de direccion
                            var dir_cte = _context.Cat_Direccion.FirstOrDefault(d => d.id_cliente == objDirecciones.Id_cliente && d.tipo_direccion == item.tipo_direccion);
                            if (dir_cte == null)
                            {
                                Cat_Direccion dir = new Cat_Direccion();
                                dir.id_cliente = objDirecciones.Id_cliente;
                                dir.calle_numero = item.calle_numero;
                                dir.cp = item.cp;
                                dir.id_estado = item.id_estado;
                                dir.id_municipio = item.id_municipio;
                                dir.colonia = item.id_localidad.ToString();
                                dir.telefono = item.telefono;
                                dir.estatus = item.estatus;
                                dir.creado = item.creado;
                                dir.creadopor = item.creadopor;
                                dir.actualizado = item.actualizado;
                                dir.actualizadopor = item.actualizadopor;
                                dir.tipo_direccion = item.tipo_direccion;
                                dir.nombrecontacto = item.nombrecontacto;
                                dir.numExt = item.numExt;
                                dir.numInt = item.numInt;
                                dir.telefono_movil = item.telefono_movil;
                                dir.id_localidad = item.id_localidad;
                                dir.Fecha_Estimada = item.Fecha_Estimada;
                                dir.id_prefijo_calle = item.id_prefijo_calle;
                                _context.Cat_Direccion.Add(dir);

                            }
                            _context.SaveChanges();
                            msg = msg + "dir " + i + " guardado";
                        }
                        // Editar registro
                        else
                        {
                            var direccion = _context.direcciones_cotizacion.FirstOrDefault(d => d.id == item.id);
                            if (direccion == null)
                            {
                                return response = Ok(new { result = "Error", detalle = "No existe un registro con el id: " + item.id, id = item.id });
                            }
                            else
                            {
                                direccion.id_cotizacion = item.id_cotizacion;
                                direccion.calle_numero = item.calle_numero;
                                direccion.cp = item.cp;
                                direccion.id_estado = item.id_estado;
                                direccion.id_municipio = item.id_municipio;
                                direccion.colonia = item.id_localidad.ToString();
                                direccion.telefono = item.telefono;
                                direccion.estatus = item.estatus;
                                direccion.creado = item.creado;
                                direccion.creadopor = item.creadopor;
                                direccion.actualizado = item.actualizado;
                                direccion.actualizadopor = item.actualizadopor;
                                direccion.tipo_direccion = item.tipo_direccion;
                                direccion.nombrecontacto = item.nombrecontacto;
                                direccion.numExt = item.numExt;
                                direccion.numInt = item.numInt;
                                direccion.telefono_movil = item.telefono_movil;
                                direccion.id_localidad = item.id_localidad;
                                direccion.Fecha_Estimada = item.Fecha_Estimada;
                                direccion.id_prefijo_calle = item.id_prefijo_calle;
                                _context.direcciones_cotizacion.Update(direccion);
                                _context.SaveChanges();
                                msg = msg + "dir" + i + "actualizado";
                                
                            }
                        }
                        i += 1;
                    }

                    //int estatus_c = _context.Cotizaciones.FirstOrDefault(c => c.Id == objDirecciones.direcciones[0].id_cotizacion).Estatus;
                    //bool entrega_sol = _context.Cotizaciones.FirstOrDefault(c => c.Id == objDirecciones.direcciones[0].id_cotizacion).entrega_sol;


                    //obj_dir_cliente _obj_dir_cliente = new obj_dir_cliente();
                    //_obj_dir_cliente.direcciones = direcciones;
                    //_obj_dir_cliente.Id_cliente = cliente_id;
                    //if (estatus_c <= 2)
                    //{
                    //    _obj_dir_cliente.dir_clie = true; // se modifica direcciones_cliente
                    //    _obj_dir_cliente.cat_dir = false; // NO se modifica Cat_direcciones
                    //}
                    //if (estatus_c == 5 || entrega_sol)
                    //{
                    //    _obj_dir_cliente.dir_clie = false;
                    //    _obj_dir_cliente.cat_dir = true;
                    //}

                    set_cat_direcciones(objDirecciones);
                }
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message });
            }
            return response = Ok(new { result = "Success", detalle = msg });
        }

        ////////////////////// POST AGREGAR Y EDITAR COTIZACION DIRECCIONES (Direccion_Cotizacion)
        // POST: api/Servicios/Agregar_Direccion
        [Route("crear_editar_direccion_cotizacion")] // api/Partners_Cotizacion/Agregar_Direccion
        [HttpPost("{crear_editar_direccion_cotizacion}")] // NOTA: Nombre del ENDPOINT: Agregar_Direccion_Cotizacion y descomentar el ENDPOINT de arriba (Agregar_Direccion)
        public IActionResult crear_editar_direccion_cotizacion([FromBody] Direccion_Cotizacion item)
        {
            var result = new Models.Response();
            IActionResult response = Unauthorized();
            try
            {
                if (item == null)
                {
                    return BadRequest();
                }
                else
                {
                    // Agregar registro
                    if(item.id == 0)
                    {
                        _context.direcciones_cotizacion.Add(item);
                        _context.SaveChanges();
                        result = new Models.Response
                        {
                            response = "Success"
                        };
                    }
                    // Editar registro
                    else
                    {
                        var direccion = _context.direcciones_cotizacion.FirstOrDefault(d => d.id == item.id);
                        if(direccion == null )
                        {
                            return response = Ok(new { result = "Error", detalle = "No existe un registro con el id: " + item.id, id = item.id });
                        }
                        else
                        {
                            direccion.id_cotizacion = item.id_cotizacion;
                            direccion.calle_numero = item.calle_numero;
                            direccion.cp = item.cp;
                            direccion.id_estado = item.id_estado;
                            direccion.id_municipio = item.id_municipio;
                            direccion.colonia = item.colonia;
                            direccion.telefono = item.telefono;
                            direccion.estatus = item.estatus;
                            direccion.creado = item.creado;
                            direccion.creadopor = item.creadopor;
                            direccion.actualizado = item.actualizado;
                            direccion.actualizadopor = item.actualizadopor;
                            direccion.tipo_direccion = item.tipo_direccion;
                            direccion.nombrecontacto = item.nombrecontacto;
                            direccion.numExt = item.numExt;
                            direccion.numInt = item.numInt;
                            direccion.telefono_movil = item.telefono_movil;
                            direccion.id_localidad = item.id_localidad;
                            direccion.Fecha_Estimada = item.Fecha_Estimada;
                            direccion.id_prefijo_calle = item.id_prefijo_calle;
                            _context.direcciones_cotizacion.Update(direccion);
                            _context.SaveChanges();
                            return response = Ok(new { result = "Success", detalle = "Registro actualizado con éxito.", id = direccion.id });
                        }
                    }
                   
                }
            }
            catch (Exception ex)
            {
                result = new Models.Response
                {
                    response = "Error"
                };
            }
            return new ObjectResult(result);
        }


        // POST: api/Servicios/Actualizar_estatus
        [Route("actualizar_direccion")]
        [HttpPost("{actualizar_direccion}")]
        public IActionResult Actualizar_estatus_servicio_visita([FromBody] Cat_Direccion item)
        {
            var result = new Models.Response();
            try
            {
                var direccion = _context.Cat_Direccion.FirstOrDefault(s => s.id == item.id);
                if (direccion == null)
                {
                    return BadRequest();
                }
                else
                {

                    direccion.id_cliente = item.id_cliente;
                    direccion.calle_numero = item.calle_numero;
                    direccion.cp = item.cp;
                    direccion.id_estado = item.id_estado;
                    direccion.id_municipio = item.id_municipio;
                    direccion.colonia = item.colonia;
                    direccion.telefono = item.telefono;
                    direccion.estatus = item.estatus;
                    direccion.creado = item.creado;
                    direccion.creadopor = item.creadopor;
                    direccion.actualizado = item.actualizado;
                    direccion.actualizadopor = item.actualizadopor;
                    direccion.tipo_direccion = item.tipo_direccion;
                    direccion.nombrecontacto = item.nombrecontacto;
                    direccion.numExt = item.numExt;
                    direccion.numInt = item.numInt;
                    direccion.telefono_movil = item.telefono_movil;
                    direccion.Fecha_Estimada = item.Fecha_Estimada;
                    _context.Cat_Direccion.Update(direccion);

                    _context.SaveChanges();
                    result = new Models.Response
                    {
                        response = "Success"
                    };
                }
            }
            catch (Exception ex)
            {
                result = new Models.Response
                {
                    response = "Error"
                };
            }
            return new ObjectResult(result);
        }

        [Route("Upload")]
        [HttpPost("{Upload}")]
        public async Task<IActionResult> Upload(List<IFormFile> file)
        {
            
            var result = new Models.Response();

            try
            {
                long size = file.Sum(f => f.Length);

                var filePath = Environment.CurrentDirectory;
                var extencion = file[0].FileName.Split(".");
                var _guid = Guid.NewGuid();
                var selRuta = _context.parametro_Archivos.FirstOrDefault(p => p.id == 1);
                string ruta;
                if (selRuta == null) ruta = "Error";
                else
                {
                    var path = selRuta.ruta + _guid + "." + extencion[extencion.Length-1];
                    //string servePath = Request.IsHttps ? "https://" : "http://" + Request.Host.ToUriComponent();

                    foreach (var formFile in file)
                    {
                        if (formFile.Length > 0)
                        {
                            using (var stream = new FileStream(filePath + path, FileMode.Create))
                            {
                                await formFile.CopyToAsync(stream);
                            }
                        }
                    }
                    ruta = selRuta.funcion + _guid + "." + extencion[extencion.Length - 1];
                }
                
                result = new Models.Response
                {
                    response = ruta
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

        // POST: api/Servicios/agregar_documento
        [Route("agregar_documento")]
        [HttpPost("{agregar_documento}")]
        public IActionResult agregar_documento([FromBody] documentos_cotizacion item)
        {
            var result = new Models.Response();
            try
            {
                if (item == null)
                {
                    return BadRequest();
                }
                else
                {
                    item.fecha_subida = DateTime.Now;
                    _context.documentos_cotizacion.Add(item);
                   _context.SaveChanges();
                   var finanzas = _context.Users.Where(us => us.id_rol == 8 || us.id_rol == 9);
                   //correo3 nuevo comprobante de pago. Se incluye usr 9, ya estaba usr 8
                   foreach (var user in finanzas)
                   {
                       long id_suc = _context.Cotizaciones.FirstOrDefault(ct => ct.Id == item.Id_Cotizacion).Id_sucursal;
                       var ctaSuc = _context.Cat_Sucursales.Include(s => s.Cuenta).FirstOrDefault(s => s.Id == id_suc);
                       StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_Interno.html"));
                       string body = string.Empty;
                       body = reader.ReadToEnd();
                       body = body.Replace("{Titulo}", "Nuevo comprobante de pago");
                       body = body.Replace("{Cuenta}", ", cuenta <strong>" + ctaSuc.Cuenta.Cuenta_es + "</strong>");
                       body = body.Replace("{Sucursal}", ", sucursal <strong>" + ctaSuc.Sucursal + "</strong>");
                       body = body.Replace("{Username}", user.name + " " + user.paterno);
                       body = body.Replace("{Texto}", "Se agregó un nuevo comprobante de pago a la orden número ");
                       body = body.Replace("{NumeroCotizacion}", item.Id_Cotizacion.ToString());
                       SendMail(user.email, body, "Nuevo comprobante de pago orden # " + item.Id_Cotizacion.ToString());
                   }

                    var cotizacion = _context.Cotizaciones.FirstOrDefault(s => s.Id == item.Id_Cotizacion);
                    if (cotizacion != null)
                    {
                        cotizacion.id_formapago = item.id_forma_pago;
                        var comprobante = _context.tipos_comprobantes.FirstOrDefault(s => s.id == item.id_tipo_tipo_pago);
                        if (comprobante.es_liquidacion == true)
                            cotizacion.puede_solicitar_env = 1;
                        _context.Cotizaciones.Update(cotizacion);
                       
                    }
                    _context.SaveChanges();
                    result = new Models.Response
                    {
                        response = "Success"
                    };
                }
            }
            catch (Exception ex)
            {
                result = new Models.Response
                {
                    response = "Error"
                };
            }
            return new ObjectResult(result);
        }

        // POST: api/Servicios/edita_fiscales
        [Route("edita_fiscales")]
        [HttpPost("{edita_fiscales}")]
        public IActionResult Actualizar_estatus_servicio_visita([FromBody] DatosFiscales item)
        {
            var result = new Models.Response();

            try
            {
                var fiscales = _context.DatosFiscales.FirstOrDefault(s => s.id == item.id);
                if (fiscales == null)
                {
                    return BadRequest();
                }
                else
                {

                    fiscales.id_cliente = item.id_cliente == 0 ? fiscales.id_cliente : item.id_cliente;
                    fiscales.nombre_fact = item.nombre_fact;
                    fiscales.razon_social = item.razon_social;
                    fiscales.rfc = item.rfc;
                    fiscales.email = item.email;
                    fiscales.calle_numero = item.calle_numero;
                    fiscales.cp = item.cp;
                    fiscales.id_estado = item.id_estado;
                    fiscales.id_municipio = item.id_municipio;
                    fiscales.colonia = item.colonia;
                    fiscales.Ext_fact = item.Ext_fact;
                    fiscales.Int_fact = item.Int_fact;
                    fiscales.telefono_fact = item.telefono_fact;
                    //fiscales.id_cliente = item.id_cliente;

                    _context.DatosFiscales.Update(fiscales);

                    _context.SaveChanges();
                    result = new Models.Response
                    {
                        response = "Success"
                    };
                }
            }
            catch (Exception ex)
            {
                result = new Models.Response
                {
                    response = "Error"
                };
            }
            return new ObjectResult(result);
        }

        // POST: api/Servicios/edita_fiscales
        [Route("edita_cotizacion_condiciones")]
        [HttpPost("{edita_cotizacion_condiciones}")]
        public IActionResult edita_cotizacion_condiciones([FromBody] CotizacionesCond item)
        {
            var result = new Models.Response();
            try
            {
                var cotizacion = _context.Cotizaciones.FirstOrDefault(s => s.Id == item.Id);
                if (cotizacion == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (item.Estatus == 3)
                        cotizacion.Acciones = 1;
                    cotizacion.id_formapago = item.id_formapago;
                    cotizacion.Estatus = item.Estatus;
                    cotizacion.fecha_cotiza = DateTime.Now;
                    _context.Cotizaciones.Update(cotizacion);
                    _context.SaveChanges();
                    result = new Models.Response
                    {
                        response = "Success"
                    };
                }
            }
            catch (Exception ex)
            {
                result = new Models.Response
                {
                    response = "Error"
                };
            }
            return new ObjectResult(result);
        }

        // POST: api/Servicios/edita_fiscales
        [Route("edita_cotizacion_ibs")]
        [HttpPost("{edita_cotizacion_ibs}")]
        public IActionResult edita_cotizacion_ibs([FromBody] CotizacionesIbs item)
        {
            var result = new Models.Response();
            try
            {
                var cotizacion = _context.Cotizaciones.FirstOrDefault(s => s.Id == item.Id);
                if (cotizacion == null)
                {
                    return BadRequest();
                }
                else
                {

                    cotizacion.ibs = item.ibs;
                    cotizacion.Estatus = 4;
                    cotizacion.Acciones = 0;
                    _context.Cotizaciones.Update(cotizacion);
                    integracion_tickets(cotizacion.Id, cotizacion.Id_Cliente, cotizacion.Estatus);
                    //Agrega cotizacion a comisiones, si ya existe comision de cotizacion actualiza monto, si no, inserta nuevo registro
                    comisiones_vendedores _com_idcot = _context.comisiones_vendedores.FirstOrDefault(v => v.id_cotizacion == cotizacion.Id);
                    if (_com_idcot != null)
                    {
                        _com_idcot.pagada = false;
                        _com_idcot.monto_com_sin_iva = Convert.ToDecimal(Math.Round(cotizacion.comision_vendedor / 1.16, 2));
                        _com_idcot.monto_comision = Convert.ToDecimal(Math.Round(cotizacion.comision_vendedor / 7.25, 2));
                        _context.comisiones_vendedores.Update(_com_idcot);
                    }
                    else
                    {
                        comisiones_vendedores cv = new comisiones_vendedores();
                        cv.id_cat_tipo_comision = 1;
                        cv.id_cotizacion = cotizacion.Id;
                        cv.fecha_generacion = DateTime.Now;
                        cv.pago_programado = DateTime.Now;
                        cv.fecha_de_pago = DateTime.Now;
                        cv.pagada = false;
                        cv.id_quienpago = 10113;
                        cv.monto_com_sin_iva = Convert.ToDecimal(Math.Round(cotizacion.comision_vendedor / 1.16, 2));
                        cv.monto_comision = Convert.ToDecimal(Math.Round(cotizacion.comision_vendedor / 7.25, 2));
                        _context.comisiones_vendedores.Add(cv);

                    }
                    _context.SaveChanges();
                    
                    //correo6 envio de orden de venta pendiente
                    var vendedor = _context.Users.FirstOrDefault(c => c.id == cotizacion.Id_Vendedor);
                    if (vendedor != null)
                    {
                        var admin_canal = _context.Users.FirstOrDefault(us => us.id_canal == vendedor.id_canal && us.nivel == "canal");
                        //var finanzas = _context.Users.Where(us => us.id_rol == 8);
                        var venyFin = _context.Users.Where(us => us.id_rol == 8 || us.id_rol == 9);
                        var sucursal = _context.Cat_Sucursales.SingleOrDefault(s => s.Id == vendedor.id_Sucursales);
                        var ctaSuc = _context.Cat_Sucursales.Include(s => s.Cuenta).FirstOrDefault(s => s.Id == cotizacion.Id_sucursal);

                        foreach (var user in venyFin)
                        {
                            StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_Interno.html"));
                            string body = string.Empty;
                            body = reader.ReadToEnd();
                            body = body.Replace("{Titulo}", "Orden de venta pendiente");
                            body = body.Replace("{Cuenta}", ", cuenta <strong>" + ctaSuc.Cuenta.Cuenta_es + "</strong>");
                            body = body.Replace("{Sucursal}", ", sucursal <strong>" + ctaSuc.Sucursal + "</strong>");
                            body = body.Replace("{Texto}", "Se ha capturado el número de orden de Venta de acuerdo a su número de orden ");
                            body = body.Replace("{NumeroCotizacion}", item.Id.ToString());
                            body = body.Replace("{Username}", user.name + " " + user.paterno);
                            SendMail(user.email, body, "Orden de venta pendiente # " + item.Id.ToString());
                        }
                        if (admin_canal != null)
                        {
                            StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_Interno.html"));
                            string body = string.Empty;
                            body = reader.ReadToEnd();
                            body = body.Replace("{Titulo}", "Orden de venta pendiente");
                            body = body.Replace("{Cuenta}", ", cuenta <strong>" + ctaSuc.Cuenta.Cuenta_es + "</strong>");
                            body = body.Replace("{Sucursal}", ", sucursal <strong>" + ctaSuc.Sucursal + "</strong>");
                            body = body.Replace("{Texto}", "Se ha capturado el número de orden de Venta de acuerdo a su número de orden ");
                            body = body.Replace("{NumeroCotizacion}", item.Id.ToString());
                            body = body.Replace("{Username}", admin_canal.name + " " + admin_canal.paterno);
                            SendMail(admin_canal.email, body, "Orden de venta pendiente # " + item.Id.ToString());
                        }
                        var cte = _context.Clientes.FirstOrDefault(ct => ct.id == cotizacion.Id_Cliente);
                        if (cte != null)
                        {
                            var prods = _context.Cotizacion_Producto.Join(_context.Cat_Productos, cotP => cotP.Id_Producto, catP => catP.id,
                                (cotP, catP) => new { Cotizacion_Producto = cotP, Cat_Productos = catP })
                                .Where(cp => cp.Cotizacion_Producto.Id_Cotizacion == cotizacion.Id && cp.Cat_Productos.tipo < 5)
                                .OrderBy(cp => cp.Cat_Productos.nombre);

                            StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_Cliente.html"));
                            string body = string.Empty;
                            body = reader.ReadToEnd();
                            body = body.Replace("{Titulo}", "¡Gracias por su compra!");
                            body = body.Replace("{Saludo}", "Estimado cliente:");
                            body = body.Replace("{Username}", cte.nombre + " " + cte.paterno);
                            body = body.Replace("{Texto}", "Nos ponemos en contacto con usted desde la sucursal <strong>" + sucursal.Sucursal +
                                "</strong> para hacerle llegar las guías mecánicas y manuales de uso de los equipos MIELE incluidos en la orden ");
                            body = body.Replace("{NumeroCotizacion}", item.Id.ToString());
                            string tablproductos = "";
                            foreach (var producto in prods)
                            {

                                string manualurl = "";
                                string guiaurl = "";
                                string manualTexto = "";
                                string guiaTexto = "";
                                if (producto.Cat_Productos.url_manual != null && producto.Cat_Productos.url_manual != "")
                                {
                                    manualurl = producto.Cat_Productos.url_manual;
                                    manualTexto = "Descarga";
                                }
                                else manualTexto = "No Disponible";

                                if (producto.Cat_Productos.url_guia != null && producto.Cat_Productos.url_guia != "")
                                {
                                    guiaurl = producto.Cat_Productos.url_guia;
                                    guiaTexto = "Descarga";
                                }
                                else guiaTexto = "No Disponible";
                                tablproductos = tablproductos + "<tr style='font-size:18px; height:50px;' >" +
                                    "<td>" + producto.Cat_Productos.nombre + "</td>" +
                                    "<td style='text-align:center; vertical-align:central;'><a href='" + manualurl + "'>" + manualTexto + "</a></td>" +
                                    "<td style='text-align:center; vertical-align:central;'><a href='" + guiaurl + "'>" + guiaTexto + "</a></td>" +
                                    "</tr> ";
                            }
                            body = body.Replace("<tr>{ListaProductos}</tr>", tablproductos);
                            
                            foreach (string correo in item.correos)
                            {
                                SendMail(correo, body, "Manuales de productos de la orden " + item.Id.ToString());
                            }
                        }
                    }

                    result = new Models.Response
                    {
                        response = "Success"
                    };
                }
            }
            catch (Exception ex)
            {
                result = new Models.Response
                {
                    response = "Error"
                };
            }
            return new ObjectResult(result);
        }


        public permisos_cotizacion_detalle deshabilita_todo(permisos_cotizacion_detalle permisos)
        {
            permisos.deshabilitar_dir_ins = true;
            permisos.deshabilitar_dir_env = true;
            permisos.deshabilitar_fact = true;
            permisos.deshabilitar_lista_prods = true;
            permisos.deshabilitar_formas_pago = true;
            permisos.deshabilitar_vendedor = true;
            permisos.deshabilitar_cliente = true;
            permisos.deshabilitar_referencia = true;
            permisos.deshabilitar_rechazo = true;
            permisos.deshablitar_btn_guardar_gral = true;
            permisos.deshabilitar_acepto_terminos = true;
            permisos.deshabilitar_btn_cancelar = true;
            permisos.mostrar_boton_duplicar = false;
            permisos.deshabilitar_agregar_prod = true;
            permisos.deshabilitar_botones_upload = true;
            permisos.deshabilitar_sol_entrega = true;

            return permisos;
        }

        public permisos_cotizacion_detalle get_permisoscotizacion(Cotizaciones C, Users U)

        {
            var Rol = _context.Cat_Roles.FirstOrDefault(u => u.id == U.id_rol);
            var Cnl = _context.Cat_canales.FirstOrDefault(u => u.Id == U.id_canal);
            var Cl = _context.Clientes.FirstOrDefault(cl => cl.id == C.Id_Cliente);
            var EC = _context.Cat_Estatus_Cotizacion.FirstOrDefault(e => e.id == C.Estatus);
            var permisos = new permisos_cotizacion_detalle();

            permisos.mostrar_btn_guardar_gral = true; // por default 
            permisos.mostrar_acepto_terminos = true;
            permisos.deshabilitar_acepto_terminos = true;

            permisos = set_permisos_estatus(permisos, C.Estatus);
            permisos = set_permisos_rol_canal(permisos, C.Estatus, U.id_canal, Convert.ToInt32(U.id_rol), C);
            return permisos;
        }

        public permisos_cotizacion_detalle set_permisos_estatus(permisos_cotizacion_detalle permisos, int Estatus)

        {
            var EC = _context.Cat_Estatus_Cotizacion.FirstOrDefault(e => e.id == Estatus);
            //////// Permisos Por Estatus + Roles 

            switch (EC.Estatus_en)
            {
                case "COT": // cotizacion  1 -> 2 (De datos básicos a OC)
                    permisos.deshabilitar_dir_ins = false;
                    permisos.deshabilitar_dir_env = false;
                    permisos.deshabilitar_vendedor = false;
                    permisos.deshabilitar_cliente = false;
                    permisos.deshabilitar_referencia = false;
                    permisos.deshabilitar_lista_prods = false;
                    permisos.deshabilitar_formas_pago = false;
                    permisos.mostrar_subir_archivos = false;
                    permisos.mostrar_letrero_ibs = true;
                    permisos.mostrarBtnOrden = false;
                    permisos.mostrarBtnComp = false;
                    permisos.mostrar_boton_duplicar = true;
                    permisos.mostrar_boton_edit_basicos = false;
                    permisos.mostrar_boton_edit_detalles = false;
                    permisos.mostrar_btn_guardar_gral = true;
                    permisos.texto_btn_guardar_gral = "Guardar";
                    permisos.mostrar_rechazo = false;
                    permisos.mostrar_acepto_terminos = false;
                    permisos.deshabilitar_agregar_prod = false;
                    break;
                case "OCP": //Orden de Compra Pendiente 2 -> 3 (Validar Comprobantes)
                    permisos.deshabilitar_dir_ins = true;
                    permisos.deshabilitar_dir_env = true;
                    permisos.deshabilitar_fact = true;
                    permisos.deshabilitar_vendedor = true;
                    permisos.deshabilitar_cliente = true;
                    permisos.deshabilitar_referencia = true;
                    permisos.deshabilitar_formas_pago = true;
                    permisos.deshabilitar_rechazo = true;
                    permisos.mostrar_rechazo = false;
                    permisos.mostrar_subir_archivos = true;
                    permisos.mostrar_letrero_ibs = true;
                    permisos.mostrar_boton_duplicar = true;
                    permisos.texto_btn_guardar_gral = "Generar Orden de Compra";
                    permisos.deshabilitar_agregar_prod = true;
                    permisos.mostrar_fact = true;
                    permisos.mostrar_lista_comp = true;
                    break;
                case "OCG": //Orden Compra Generada 3 -> 4 (Captura de IBS)
                    permisos.deshabilitar_dir_ins = true;
                    permisos.deshabilitar_dir_env = true;
                    permisos.deshabilitar_fact = true;
                    permisos.deshabilitar_vendedor = true;
                    permisos.deshabilitar_lista_prods = true;
                    permisos.deshabilitar_cliente = true;
                    permisos.deshabilitar_referencia = true;
                    permisos.deshabilitar_formas_pago = true;
                    permisos.deshabilitar_rechazo = true;
                    permisos.deshablitar_btn_guardar_gral = true;
                    permisos.mostrar_rechazo = true;
                    permisos.mostrar_subir_archivos = true;
                    permisos.mostrar_letrero_ibs = true;
                    permisos.deshabilitar_agregar_prod = true;
                    permisos.texto_btn_guardar_gral = "Guardar";
                    permisos.mostrar_fact = true;
                    permisos.mostrar_boton_edit_basicos = false;
                    permisos.mostrar_boton_edit_detalles = false;
                    permisos.deshabilitar_editar_promociones = true;
                    permisos.mostrar_lista_comp = true;
                    break;
                case "OVP": //Orden de Venta 4 -> 5 ( Validar y Liberar O.C. )
                    permisos.deshabilitar_dir_ins = true;
                    permisos.deshabilitar_dir_env = true;
                    permisos.deshabilitar_fact = true;
                    permisos.deshabilitar_vendedor = true;
                    permisos.deshabilitar_lista_prods = true;
                    permisos.deshabilitar_formas_pago = true;
                    permisos.deshabilitar_cliente = true;
                    permisos.deshabilitar_referencia = true;
                    permisos.mostrar_rechazo = true;
                    permisos.mostrar_subir_archivos = true;
                    permisos.mostrar_letrero_ibs = true;
                    permisos.deshabilitar_agregar_prod = true;
                    permisos.texto_btn_guardar_gral = "Liberar O. Venta";
                    permisos.mostrar_fact = true;
                    permisos.deshabilitar_editar_promociones = true;
                    permisos.mostrar_lista_comp = true;
                    break;
                case "OVL": //Orden Venta Liberada

                    // ya no se puede editar nada
                    permisos.deshabilitar_dir_ins = true;
                    permisos.deshabilitar_dir_env = true;
                    permisos.deshabilitar_fact = true;
                    permisos.deshabilitar_lista_prods = true;
                    permisos.deshabilitar_formas_pago = true;
                    permisos.deshabilitar_vendedor = true;
                    permisos.deshabilitar_cliente = true;
                    permisos.deshabilitar_referencia = true;
                    permisos.mostrar_subir_archivos = true;
                    permisos.mostrar_letrero_ibs = true;
                    permisos.deshabilitar_agregar_prod = true;
                    permisos.texto_btn_guardar_gral = "Guardar";
                    permisos.mostrar_fact = true;
                    permisos.deshabilitar_editar_promociones = true;
                    permisos.mostrar_lista_comp = true;
                    break;
                default:
                    break;
            }

            return permisos;
        }

        public permisos_cotizacion_detalle permisos_cotizacion_rechazada(permisos_cotizacion_detalle permisos, int id_rol)
        {
            var Rol = _context.Cat_Roles.FirstOrDefault(u => u.id == id_rol);
            if (Rol.siglas == "VTAS" || Rol.siglas == "ADM")
            {
                permisos.deshablitar_btn_guardar_gral = false;
                permisos.texto_btn_guardar_gral = "Guardar";
                permisos.deshabilitar_cliente = true;
                permisos.deshabilitar_dir_env = false;
                permisos.deshabilitar_dir_ins = false;
                permisos.mostrar_boton_duplicar = true;
                permisos.deshabilitar_sol_entrega = true;
                permisos.deshabilitar_lista_prods = true;
                permisos.deshabilitar_agregar_prod = true;
                permisos.deshabilitar_editar_promociones = true;
                if (Rol.siglas == "ADM")
                {
                    permisos.deshabilitar_btn_cancelar = false;
                }
                else
                    permisos.deshabilitar_btn_cancelar = true;
            }
            else
            {
                permisos.deshablitar_btn_guardar_gral = true;
                permisos.mostrar_boton_IBS = false;
                permisos.deshabilitar_btn_cancelar = true;
            }
            return permisos;
        }

        public permisos_cotizacion_detalle set_permisos_rol_canal(permisos_cotizacion_detalle permisos, int Estatus, int id_canal, int id_rol, Cotizaciones C)
        {
            var Rol = _context.Cat_Roles.FirstOrDefault(u => u.id == id_rol);
            var Cnl = _context.Cat_canales.FirstOrDefault(u => u.Id == id_canal);
            var EC = _context.Cat_Estatus_Cotizacion.FirstOrDefault(e => e.id == Estatus);
            //var Pagos_liquidacion = _context.documentos_cotizacion.All(p => p.Id_Cotizacion == C.Id )
            var Pagos_liquidacion = (from dc in _context.documentos_cotizacion
                                     join fp in _context.tipos_comprobantes on dc.id_tipo_tipo_pago equals fp.id
                                     where dc.Id_Cotizacion == C.Id && fp.es_liquidacion == true
                                     select new
                                     {
                                         id = dc.Id
                                     }).ToList().Count;
            bool Sol_ent_valido = false;
            if (Pagos_liquidacion > 0 && !C.entrega_sol && !C.rechazada)  //si hay pagos de liquidación y no ha solicitado la entrega antes 
                Sol_ent_valido = true;

            if (id_canal == 2 ) // si es venta directa y no tiene ordenes es falso
            {
                var Ordenes = (from dc in _context.documentos_cotizacion
                               join fp in _context.tipos_comprobantes on dc.id_tipo_tipo_pago equals fp.id
                               where dc.Id_Cotizacion == C.Id && fp.id == 5
                               select new
                               {
                                   id = dc.Id
                               }).ToList().Count;

                if (Ordenes < 1) 
                    Sol_ent_valido = false;
            }

            switch (EC.Estatus_en)
            {
                case "COT": // cotizacion  1 -> 2 (De datos básicos a OC)
                    if (Cnl.Canal_en == "KD" || Cnl.Canal_en == "DS")
                    {
                        permisos.deshabilitar_fact = true;
                    }
                    else
                    {
                        permisos.deshabilitar_fact = false;
                        permisos.mostrar_fact = true; 
                    }
                    permisos.mostrar_condiciones_comerciales = false;
                    if (Rol.siglas == "VM" || Rol.siglas == "FM")
                    {
                      //  permisos.mostrar_btn_rechazar = true;
                        deshabilita_todo(permisos);
                    }
                    permisos.deshabilitar_referencia = true;
                    if (Rol.siglas == "ADM")
                        permisos.deshabilitar_btn_cancelar = false;
                    else
                        permisos.deshabilitar_btn_cancelar = true;

                    break;
                case "OCP": //Orden de Compra Pendiente 2 -> 3 (Validar Comprobantes)

                    if (Cnl.Canal_en == "OR")
                    {
                        permisos.mostrarBtnOrden = true;
                        if (Rol.siglas == "ADM")
                            permisos.deshabilitar_fact = false;
                        else
                            permisos.deshabilitar_fact = true;

                        permisos.mostrarBtnComp = true;
                        if (Rol.siglas == "VM" || Rol.siglas == "FM")
                        {
                            permisos.mostrarBtnComp = false;
                            permisos.mostrarBtnOrden = false;
                        }
                    }
                    else if (Cnl.Canal_en == "KD" || Cnl.Canal_en == "DS")
                    {
                        permisos.mostrarBtnComp = true;
                    }
                    if (Rol.siglas == "VTAS")
                    {
                        permisos.mostrarBtnComp = false;
                        permisos.mostrarBtnOrden = false;
                        permisos.mostrar_lista_comp = false;
                    }
                    if (Rol.siglas == "ADM")
                    {
                        permisos.deshabilitar_acepto_terminos = false;
                        permisos.deshablitar_btn_guardar_gral = false;
                        permisos.deshabilitar_formas_pago = false;
                        permisos.deshabilitar_botones_upload = false;
                        permisos.deshabilitar_agregar_prod = false;
                    }
                    else
                    {
                        permisos.deshablitar_btn_guardar_gral = true;
                        permisos.mostrar_condiciones_comerciales = false;
                        permisos.mostrar_acepto_terminos = false;
                    }
                    if (Rol.siglas == "VM" || Rol.siglas == "FM")
                    {
                        deshabilita_todo(permisos);
                        permisos.mostrar_condiciones_comerciales = true;
                    }
                    if (Rol.siglas == "ADM")
                    {
                        if (Cnl.Canal_en == "KD")
                            permisos.mostrar_condiciones_comerciales = true;
                        permisos.mostrar_boton_edit_detalles = true;
                        permisos.mostrar_boton_edit_basicos = true;
                        permisos.deshabilitar_btn_cancelar = false;
                    }
                    else
                        permisos.deshabilitar_btn_cancelar = true;

                    permisos.mostrar_sol_entrega = Sol_ent_valido;
                    permisos.deshabilitar_referencia = true;
                    break;
                case "OCG": //Orden Compra Generada 3 -> 4 (Captura de IBS)

                    if (Rol.siglas == "VM" || Rol.siglas == "FM")  // solo Ventas y Finanzas miele pueden editar en este paso 
                    {
                        deshabilita_todo(permisos);
                        permisos.deshabilitar_rechazo = false;
                        permisos.deshablitar_btn_guardar_gral = true;
                        permisos.mostrarBtnOrden = false;
                        permisos.mostrar_btn_rechazar = true;
                        if (Rol.siglas == "VM")
                            permisos.mostrar_boton_IBS = true;
                    }
                    else
                    {
                        permisos.deshablitar_btn_guardar_gral = true;
                        permisos.deshabilitar_rechazo = true;
                    }

                    if (Cnl.Canal_en == "OR") // Venta directa solo puede subir y ver Ordenes de Compra 
                    {
                        permisos.mostrarBtnOrden = true;
                        permisos.deshabilitar_botones_upload = false;
                        permisos.mostrarBtnComp = true;
                        if (Rol.siglas == "VM" || Rol.siglas == "FM")
                        {
                            permisos.mostrarBtnComp = false;
                            permisos.mostrarBtnOrden = false;
                        }
                    }
                    else if (Cnl.Canal_en == "KD" || Cnl.Canal_en == "DS") // Kitchens y Tiendas dep.  solo pueden subir comp de pago
                    {
                        permisos.mostrarBtnComp = true;
                        permisos.deshabilitar_botones_upload = false;
                    }
                    if (Rol.siglas == "VTAS")
                    {
                        permisos.mostrarBtnComp = false;
                        permisos.mostrarBtnOrden = false;
                        permisos.mostrar_lista_comp = false;
                        permisos.mostrar_condiciones_comerciales = false;
                        permisos.mostrar_acepto_terminos = false;
                    }
                    if (Rol.siglas == "VM" || Rol.siglas == "FM") // Los administrdores de todas la cuentas pueden verlo 
                    {
                        permisos.mostrar_condiciones_comerciales = true;
                    }
                    if ((Cnl.Canal_en == "DS" || Cnl.Canal_en == "KD") && Rol.siglas == "ADM")
                        permisos.mostrar_condiciones_comerciales = true;

                    if (Rol.siglas == "ADM")
                        permisos.deshabilitar_btn_cancelar = false;
                    else
                        permisos.deshabilitar_btn_cancelar = true;

                    permisos.mostrar_sol_entrega = Sol_ent_valido;
                    permisos.deshabilitar_referencia = true;
                    break;
                case "OVP": //Orden de Venta 4 -> 5 ( Validar y Liberar O.C. )
                    if (Cnl.Canal_en == "OR")
                    {
                        permisos.mostrarBtnOrden = true;
                        permisos.mostrarBtnComp = true;
                        if (Rol.siglas == "VM" || Rol.siglas == "FM")
                        {
                            permisos.mostrarBtnComp = false;
                            permisos.mostrarBtnOrden = false;
                        }
                    }
                    else if (Cnl.Canal_en == "KD" || Cnl.Canal_en == "DS")
                        permisos.mostrarBtnComp = true;

                    if (Rol.siglas == "VM" || Rol.siglas == "FM") // en este paso solo finazas puede Liberar la Orden 
                    {
                        deshabilita_todo(permisos);
                        permisos.mostrar_btn_rechazar = true;
                        permisos.deshabilitar_rechazo = false;
                        if (Rol.siglas == "FM")
                            permisos.deshablitar_btn_guardar_gral = false;
                        else
                            permisos.deshablitar_btn_guardar_gral = true;
                    }
                    else
                    {
                        permisos.deshabilitar_rechazo = true;
                        permisos.deshablitar_btn_guardar_gral = true;
                    }
                    if (Rol.siglas == "VTAS")
                    {
                        permisos.mostrar_condiciones_comerciales = false;
                        permisos.mostrar_acepto_terminos = false;
                        permisos.deshabilitar_botones_upload = true;
                        permisos.mostrarBtnComp = false;
                        permisos.mostrarBtnOrden = false;
                        permisos.mostrar_lista_comp = false;
                    }
                    if (Rol.siglas == "VM" || Rol.siglas == "FM") // Los administrdores de todas la cuentas pueden verlo 
                    {
                        permisos.mostrar_condiciones_comerciales = true;
                    }
                    if ((Cnl.Canal_en == "DS" || Cnl.Canal_en == "KD") && Rol.siglas == "ADM")
                        permisos.mostrar_condiciones_comerciales = true;

                    permisos.mostrar_sol_entrega = Sol_ent_valido;
                    permisos.deshabilitar_referencia = true;
                    permisos.deshabilitar_btn_cancelar = true;
                    break;
                case "OVL": //Orden Venta Liberada

                    if (Cnl.Canal_en == "OR")
                    {
                        permisos.mostrarBtnOrden = true;
                        permisos.mostrarBtnComp = true;
                        if (Rol.siglas == "VM" || Rol.siglas == "FM")
                        {
                            permisos.mostrarBtnComp = false;
                            permisos.mostrarBtnComp = false;
                            permisos.mostrarBtnOrden = false;
                        }
                    }
                    else if (Cnl.Canal_en == "KD" || Cnl.Canal_en == "DS")
                        permisos.mostrarBtnComp = true;
                    else if (Rol.siglas == "VM" || Rol.siglas == "FM")
                    {
                        permisos.mostrarBtnComp = false;
                        permisos.mostrarBtnOrden = false;
                    }
                    if (Rol.siglas == "FM")
                    {
                        permisos.deshabilitar_rechazo = false;
                    }
                    else
                    {
                        permisos.deshabilitar_btn_cancelar = true;
                        permisos.deshabilitar_rechazo = true;
                    }
                    if (Rol.siglas == "VTAS")
                    {
                        permisos.deshabilitar_botones_upload = true;
                        permisos.mostrar_acepto_terminos = false;
                        permisos.mostrarBtnComp = false;
                        permisos.mostrarBtnOrden = false;
                        permisos.mostrarBtnComp = false;
                        permisos.mostrarBtnOrden = false;
                        permisos.mostrar_lista_comp = false;
                    }
                    if (Rol.siglas == "VM" || Rol.siglas == "FM") // Los administrdores de todas la cuentas pueden verlo 
                    {
                        permisos.mostrar_condiciones_comerciales = true;
                    }
                    if (Cnl.Canal_en == "KD" && Rol.siglas == "ADM")
                        permisos.mostrar_condiciones_comerciales = true;

                    permisos.mostrar_sol_entrega = Sol_ent_valido;
                    permisos.deshablitar_btn_guardar_gral = true;
                    permisos.deshabilitar_btn_cancelar = true;
                    permisos.deshabilitar_referencia = true;

                    break;
                default:
                    break;
            }
            //}

            if (Rol.siglas == "VM" || Rol.siglas == "FM" || Cnl.Canal_en == "OR") // solo perosnal de Miele puede verlos 
            {
                permisos.mostrar_referencia = true;
                if (Cnl.Canal_en != "OR") //venta directa no ve cc
                    permisos.mostrar_condiciones_comerciales = true;
                permisos.mostrar_chkfact = true;
            }
            else
            {
                permisos.mostrar_chkfact = false; // solo perosnal de Miele puede facturar 
            }

            if (Rol.siglas == "DIR")
                permisos = deshabilita_todo(permisos);
            if (C.cancelada)
            { permisos = deshabilita_todo(permisos); }
            else if (C.rechazada)
            { permisos_cotizacion_rechazada(permisos, Convert.ToInt32(Rol.id)); }
            return permisos;
        }


        public List<CotizacionModel> get_productos_cotizacion (long Id_cotizacion)
        {

            try
            {
                //var Cnl = _context.Cat_canales.FirstOrDefault(u => u.Id == U.id_canal);

                List<promocion> promociones_ = _context.promocion.FromSql("Get_recalcula_montos_cotizacionid '" + Id_cotizacion.ToString() + "'").ToList();

                var promo_respueta = (from pr in promociones_
                                            select new PromocionesModel
                                            {
                                                id = pr.id,
                                                nombre = pr.nombre,
                                                beneficios = beneficios_promo_cadena(pr.id),
                                                inicio = pr.fecha_hora_inicio,
                                                fin = pr.fecha_hora_fin,
                                                vigencia_indefinida = pr.vigencia_indefinida,
                                                aplicada = valida_aplicacion(Convert.ToInt32(Id_cotizacion), pr.id),
                                                beneficio_obligatorio = pr.beneficio_obligatorio

                                            }).ToList();

                var cer_prods_rels = (from cp in _context.Cotizacion_Producto
                                      join p in _context.Cat_Productos on cp.Id_Producto equals p.id
                                      join slp in _context.producto_certificado_sublinea on p.id_sublinea equals slp.Id_sublinea
                                      where cp.Id_Cotizacion == Id_cotizacion
                                      group slp by slp.id_producto into sq1
                                      select new
                                      {
                                          id_producto = sq1.First().id_producto,
                                          id_sublinea = sq1.Max().Id_sublinea
                                      });

               var _roductos = (from cp in _context.Cotizacion_Producto
                             join a in _context.Cat_Productos on cp.Id_Producto equals a.id
                             where cp.Id_Cotizacion == Id_cotizacion
                             orderby a.tipo
                             select new {
                                 cp.Id,
                                 cp.precio_descuento,
                                 cp.precio_lista, 
                                 cp.precio_condiciones_com,
                                 cp.iva_precio_lista,
                                 cp.iva_precio_descuento,
                                 cp.iva_cond_comerciales,
                                 cp.es_regalo,
                                 cp.agregado_automaticamente,
                                 cp.cantidad
                             });



                var item = (from c in _context.Cotizaciones
                            join ce in _context.Cat_Estatus_Cotizacion on c.Estatus equals ce.id
                            join usr in _context.Users on c.Id_Vendedor equals usr.id
                            where c.Id == Id_cotizacion
                            select new CotizacionModel
                            {
                                Numero = c.Numero,
                                Id_Cliente = c.Id_Cliente,
                                Id_Vendedor = c.Id_Vendedor,
                                fecha_cotiza = c.fecha_cotiza,
                                Estatus = c.Estatus,
                                Acciones = c.Acciones,
                                Id_Canal = c.Id_Canal,
                                Id_Cuenta = c.Id_Cuenta,
                                Id_sucursal = c.Id_sucursal,
                                Id_Estado_Instalacion = c.Id_Estado_Instalacion,
                                acepto_terminos_condiciones = c.acepto_terminos_condiciones,
                                Observaciones = c.Observaciones,
                                creadopor = c.creadopor,
                                id_formapago = c.id_formapago,
                                entrega_sol = c.entrega_sol,
                                id_cuenta = usr.id_cuenta,
                                estatus_desc = ce.Estatus_es,
                                ibs = c.ibs,
                                Id = c.Id,
                                //canalsiglas = Cnl.Canal_en,
                                cambio_ord_comp_generada = c.cambio_ord_comp_generada,

                                //////////////////////////////////////MONTOS
                                importe_precio_lista = Convert.ToDouble(c.importe_precio_lista),
                                iva_precio_lista = Convert.ToDouble(c.iva_precio_lista),
                                importe_condiciones_com = Convert.ToDouble(c.importe_condiciones_com),
                                iva_condiciones_com = Convert.ToDouble(c.iva_condiciones_com),
                                importe_promociones = Convert.ToDouble(c.importe_promociones),
                                iva_promociones = Convert.ToDouble(c.iva_promociones),
                                descuento_acumulado = Convert.ToDouble(c.descuento_acumulado),
                                descuento_acumulado_cond_com = Convert.ToDouble(c.descuento_acumulado_cond_com),
                                comision_vendedor = Convert.ToDouble(c.comision_vendedor),
                                motivo_rechazo = c.motivo_rechazo,
                                rechazada = c.rechazada,
                                requiere_fact = c.requiere_fact,
                                id_cotizacion_padre = c.id_cotizacion_padre
                                ,
                                productos = (from cp in _context.Cotizacion_Producto
                                             join a in _context.Cat_Productos on cp.Id_Producto equals a.id
                                             //join sl in _context.Cat_SubLinea_Producto on a.id_sublinea equals sl.id
                                             join sq1 in cer_prods_rels on cp.Id_Producto equals sq1.id_producto into sq2
                                             join _p in _roductos on cp.Id equals _p.Id
                                             where cp.Id_Cotizacion == Id_cotizacion
                                             orderby a.tipo
                                             from s2 in sq2.DefaultIfEmpty()
                                             select new ProductosCotizacionModel
                                             {
                                                 //id_linea_orden = s2.id_sublinea > 0 ? s2.id_sublinea : a.id_sublinea,
                                                 id_superlinea_orden = _context.Cat_SubLinea_Producto.FirstOrDefault(ss => ss.id == (s2.id_sublinea > 0 ? s2.id_sublinea : a.id_sublinea)).cat_linea_producto.id_superlinea,
                                                 nombre_linea_orden = _context.Cat_SubLinea_Producto.FirstOrDefault(ss => ss.id == (s2.id_sublinea > 0 ? s2.id_sublinea : a.id_sublinea)).cat_linea_producto.descripcion,
                                                 id = a.id,
                                                 sku = a.sku,
                                                 modelo = a.modelo,
                                                 nombre = a.nombre,
                                                 descripcion_corta = a.descripcion_corta,
                                                 cantidad = cp.cantidad,
                                                 margen_cc = cp.margen_cc,
                                                 importe_precio_lista = _p.precio_lista + _p.iva_precio_lista,
                                                 importe_total_bruto = (_p.precio_lista + _p.iva_precio_lista) * _p.cantidad,
                                                 importe_condiciones_com = (_p.precio_condiciones_com + _p.iva_cond_comerciales) * _p.cantidad,
                                                 importe_con_descuento = (_p.precio_descuento + _p.iva_precio_descuento),
                                                 descuento = (_p.precio_lista + _p.iva_precio_lista) - (_p.precio_descuento + _p.iva_precio_descuento),
                                                 importetotal = (_p.precio_descuento + _p.iva_precio_descuento) * _p.cantidad,
                                                 es_regalo = _p.es_regalo,
                                                 agregado_automaticamente = _p.agregado_automaticamente,
                                                 id_sublinea = a.id_sublinea,
                                                 id_linea = _context.Cat_SubLinea_Producto.FirstOrDefault(L => L.id == a.id_sublinea).id_linea_producto,
                                                 cat_imagenes_producto = (from x in _context.Cat_Imagenes_Producto
                                                                          where x.id_producto == a.id
                                                                          select x)
                                                                              .Select(d => new Cat_Imagenes_Producto
                                                                              {
                                                                                  id = d.id,
                                                                                  id_producto = d.id_producto,
                                                                                  url = d.url
                                                                              }).ToList()

                                             }).OrderBy(a=> a.id_superlinea_orden).ThenBy(d=> d.nombre_linea_orden).ToList()

                                ,
                                tiene_envio = _context.Cotizacion_Producto.Where(C => C.Id_Cotizacion == Id_cotizacion && C.Id_Producto == 388).Count()
                                ,
                                tiene_home = _context.Cotizacion_Producto.Where(C => C.Id_Cotizacion == Id_cotizacion && _context.Cat_Productos.FirstOrDefault(P => P.id == C.Id_Producto).id_linea == 38).Count()
                                ,
                                tiene_certificado = _context.Cotizacion_Producto.Where(C => C.Id_Cotizacion == Id_cotizacion && _context.Cat_Productos.FirstOrDefault(P => P.id == C.Id_Producto).id_linea == 36).Count()
                                ,
                                faltan_certificados = Faltan_Certificados(Id_cotizacion),
                                promociones_respueta = promo_respueta
                                // , tiene_home = (from cp in _context.Cotizacion_Producto join a in _context.Cat_Productos on cp.Id_Producto equals a.id where cp.Id_Cotizacion == Id_cotizacion && a.id_linea == 38 select new { cp.Id }).ToList().Count()
                                // , tiene_certificado = (from cp in _context.Cotizacion_Producto join a in _context.Cat_Productos on cp.Id_Producto equals a.id where cp.Id_Cotizacion == Id_cotizacion && a.id_linea == 36 select new { cp.Id }).ToList().Count()
                            }).ToList();

                foreach (var prod in item[0].productos)
                {
                    if (prod.cat_imagenes_producto.Count == 0)
                    {
                        Cat_Imagenes_Producto cip = new Cat_Imagenes_Producto();
                        cip.url = "../assets/img/img_prod_no_dips.png";
                        cip.id = 0;
                        cip.id_producto = prod.id;
                        prod.cat_imagenes_producto.Add(cip);
                    }
                }

                return item;
            }
            catch (Exception ex)
            {
                return null;
            }
            
        }


        private bool Faltan_Certificados(long cotizacion)
        {
            var cert_todos = (from a in _context.Cotizacion_Producto
                              join b in _context.Cat_Productos on a.Id_Producto equals b.id
                              join c in _context.producto_certificado_sublinea on b.id_sublinea equals c.Id_sublinea
                              where a.Id_Cotizacion == cotizacion
                              group new { a, b, c } by c.id_producto into gb
                              select new
                              {
                                  id_prod = gb.First().c.id_producto,
                                  cantidad = gb.Sum(s => s.a.cantidad)
                              }).ToList();

            //Si la cotizacion no tiene productos que tengan un certificado relacionado devuelve falso
            if (cert_todos.Count == 0) return false;


            var cert_inc = (from a in _context.Cotizacion_Producto
                            join c in _context.Cat_Productos on a.Id_Producto equals c.id
                            where a.Id_Cotizacion == cotizacion && c.id_linea == 36
                            select a).ToList();
            //Si la cotizacion tiene menos certificados devuelve verdadero
            if (cert_inc.Count < cert_todos.Count()) return true;

            foreach (var item in cert_todos)
            {
                var coincide = cert_inc.Where(a => a.Id_Producto == item.id_prod).FirstOrDefault();
                if (coincide != null)
                {
                    if (item.cantidad != coincide.cantidad)
                    {
                        return true;
                    }
                }
                else return true;
            }

            return false;
        }

        /// Nuevo inicios fin 

        [Route("GetCotizacionCompleta")]
        [HttpPost("{GetCotizacionCompleta}")]
        [HttpPost]
        public IActionResult GetCotizacionCompleta([FromBody]ModeloBusquedaCotID_User busqueda)
       {
            IActionResult response = Unauthorized();
            try
            {
                var C = _context.Cotizaciones.FirstOrDefault(c => c.Id == busqueda.Id);
                var U = _context.Users.FirstOrDefault(u => u.id == C.Id_Vendedor);
                var U_session = _context.Users.FirstOrDefault(u => u.id == busqueda.Id_user);
                var Cl = _context.Clientes.FirstOrDefault(cl => cl.id == C.Id_Cliente);
                var Cnl = _context.Cat_canales.FirstOrDefault(u => u.Id == U.id_canal);

                permisos_cotizacion_detalle permisos = get_permisoscotizacion(C, U_session);

                var item_m = get_productos_cotizacion(busqueda.Id).ToList();

                var item = (from c in _context.Cotizaciones
                            join ce in _context.Cat_Estatus_Cotizacion on c.Estatus equals ce.id
                            join usr in _context.Users on c.Id_Vendedor equals usr.id
                            where c.Id == busqueda.Id
                            select new
                            {
                                Numero = c.Numero,
                                Id_Cliente = c.Id_Cliente,
                                Id_Vendedor = c.Id_Vendedor,
                                fecha_cotiza = c.fecha_cotiza,
                                Estatus = c.Estatus,
                                Acciones = c.Acciones,
                                Id_Canal = c.Id_Canal,
                                Id_Cuenta = c.Id_Cuenta,
                                Id_Estado_Instalacion = c.Id_Estado_Instalacion,
                                acepto_terminos_condiciones = c.acepto_terminos_condiciones,
                                Observaciones = c.Observaciones,
                                creadopor = c.creadopor,
                                id_formapago = c.id_formapago,
                                entrega_sol = c.entrega_sol,
                                id_cuenta = usr.id_cuenta,
                                estatus_desc = ce.Estatus_es,
                                ibs = c.ibs,
                                Id = c.Id,
                                canalsiglas = Cnl.Canal_en,
                                cambio_ord_comp_generada = c.cambio_ord_comp_generada,
                                id_tarjeta = c.id_tarjeta,

                                //////////////////////////////////////MONTOS
                                importe_precio_lista = item_m[0].importe_precio_lista,
                                iva_precio_lista = item_m[0].iva_precio_lista,
                                importe_condiciones_com = item_m[0].importe_condiciones_com,
                                iva_condiciones_com = item_m[0].iva_condiciones_com,
                                importe_promociones = item_m[0].importe_promociones,
                                iva_promociones = item_m[0].iva_promociones,
                                descuento_acumulado = item_m[0].descuento_acumulado,
                                descuento_acumulado_cond_com = item_m[0].descuento_acumulado_cond_com,
                                comision_vendedor = item_m[0].comision_vendedor,
                                motivo_rechazo = item_m[0].motivo_rechazo,
                                rechazada = item_m[0].rechazada,
                                requiere_fact = item_m[0].requiere_fact,
                                id_cotizacion_padre = item_m[0].id_cotizacion_padre,
                                productos = item_m[0].productos , 
                                tiene_envio = item_m[0].tiene_envio , 
                                tiene_home = item_m[0].tiene_home , 
                                tiene_certificado = item_m[0].tiene_certificado ,
                                faltan_certificados = item_m[0].faltan_certificados,
                                condiciones = (from ccc in _context.CondicionesComerciales_Cuenta
                                               join cc in _context.Cat_CondicionesComerciales on ccc.id_condicion equals cc.id
                                               orderby ccc.id_condicion ascending
                                               where ccc.id_cuenta == U.id_cuenta && ccc.Vigencia_inicial <= DateTime.Now && ccc.Vigencia_final >= DateTime.Now
                                               select new
                                               {
                                                   ccc.id, cc.condicion_comercial, ccc.Vigencia_final
                                               }).ToList()
                                ,vendedores = (from b in _context.Users
                                                join c in _context.Cat_Cuentas on b.id_cuenta equals c.Id
                                                where b.id_cuenta == U.id_cuenta
                                                select new
                                                {
                                                    id = b.id, nombrecompleto = b.name + " " + b.paterno + " " + (b.materno == null ? "" : b.materno) + " - " + c.Cuenta_es
                                                }).ToList()
                                , formas_pago = (from a in _context.Cat_CondicionesPago
                                                join f in _context.Cat_Formas_Pago on a.id_Cat_Formas_Pago equals f.id
                                                where a.id_cuenta == U.id_cuenta
                                                select new
                                                {
                                                    id = f.id, formaPago = f.FormaPago, comprobantes_obligatorios = f.comprobantes_obligatorios
                                                }).ToList()
                               ,direcciones_ins = (from a in _context.direcciones_cotizacion
                                                   where a.id_cotizacion == busqueda.Id && a.tipo_direccion == 1
                                                   orderby a.tipo_direccion ascending
                                                   select new
                                                   {
                                                       id = a.id,
                                                       calle_numero = a.calle_numero,
                                                       colonia = a.colonia,
                                                       cp = a.cp,
                                                       nombrecontacto = a.nombrecontacto,
                                                       id_estado = a.id_estado,
                                                       id_municipio = a.id_municipio,
                                                       telefono = a.telefono,
                                                       tipo_direccion = a.tipo_direccion,
                                                       telefono_movil = a.telefono_movil,
                                                       id_localidad = a.id_localidad,
                                                       numExt = a.numExt,
                                                       numInt = a.numInt,
                                                       Fecha_Estimada = a.Fecha_Estimada,
                                                       id_cotizacion = busqueda.Id,
                                                       prefijo_ins = a.id_prefijo_calle,
                                                       id_prefijo_calle = a.id_prefijo_calle
                                                   }).ToList(),
                                direcciones_env = (from a in _context.direcciones_cotizacion
                                                   where a.id_cotizacion == busqueda.Id && a.tipo_direccion == 2
                                                   orderby a.tipo_direccion ascending
                                                   select new
                                                   {
                                                       id = a.id,
                                                       calle_numero = a.calle_numero,
                                                       colonia = a.colonia,
                                                       cp = a.cp,
                                                       nombrecontacto = a.nombrecontacto,
                                                       id_estado = a.id_estado,
                                                       id_municipio = a.id_municipio,
                                                       telefono = a.telefono,
                                                       tipo_direccion = a.tipo_direccion,
                                                       telefono_movil = a.telefono_movil,
                                                       id_localidad = a.id_localidad,
                                                       numExt = a.numExt,
                                                       numInt = a.numInt,
                                                       Fecha_Estimada = a.Fecha_Estimada,
                                                       id_cotizacion = busqueda.Id,
                                                       prefijo_ins = a.id_prefijo_calle,
                                                       id_prefijo_calle = a.id_prefijo_calle
                                                   }).ToList(),
                                DatosFiscales = (from a in _context.DatosFiscales
                                                 join b in _context.Clientes on a.id_cliente equals b.id
                                                 where b.id == Cl.id
                                                 select new
                                                 {
                                                     id = a.id,
                                                     nombre_fact = a.nombre_fact,
                                                     razon_social = a.razon_social,
                                                     rfc = a.rfc,
                                                     email = a.email,
                                                     calle_numero = a.calle_numero,
                                                     cp = a.cp,
                                                     id_estado = a.id_estado,
                                                     id_municipio = a.id_municipio,
                                                     colonia = a.colonia,
                                                     Ext_fact = a.Ext_fact,
                                                     Int_fact = a.Int_fact,
                                                     telefono_fact = a.telefono_fact,
                                                     id_cliente = C.Id_Cliente,
                                                     prefijo_ins = a.id_prefijo_calle,
                                                     id_prefijo_calle = a.id_prefijo_calle
                                                 }).ToList(),
                                DatosFiscales_Cuenta = (from a in _context.DatosFiscales_Sucursales
                                                        where a.id_Sucursal == c.Id_sucursal
                                                        select new
                                                        {
                                                            id = a.id,
                                                            nombre_fact = a.nombre_fact,
                                                            razon_social = a.razon_social,
                                                            rfc = a.rfc,
                                                            email = a.email,
                                                            calle_numero = a.calle_numero,
                                                            cp = a.cp,
                                                            id_estado = a.id_estado,
                                                            id_municipio = a.id_municipio,
                                                            colonia = a.colonia,
                                                            Ext_fact = a.Ext_fact,
                                                            Int_fact = a.Int_fact,
                                                            telefono_fact = a.telefono_fact,
                                                            id_cliente = C.Id_Cliente,
                                                        }).ToList(),
                                documentos_cotizacion = (from dc in _context.documentos_cotizacion
                                                         join t in _context.tipos_comprobantes on dc.id_tipo_tipo_pago equals t.id
                                                         join u in _context.Users on dc.id_user equals u.id
                                                         orderby dc.tipo_docto ascending
                                                         where dc.Id_Cotizacion == busqueda.Id //&& dc.tipo_docto == (tpo_D == 0 ? dc.tipo_docto : tpo_D)
                                                         select new
                                                         {
                                                             Id_Cotizacion = dc.Id_Cotizacion,
                                                             Id_foto = dc.Id_foto,
                                                             tipo_docto = dc.tipo_docto,
                                                             Id = dc.Id,
                                                             tipo_comp = t.tipo_pago,
                                                             fecha_subida = dc.fecha_subida.ToString("dd/MM/yyyy hh:mm"),
                                                             usuario = u.email
                                                         }).ToList(),
                                permisos_cotizacion = permisos,
                                basicos_cliente = (from c_ in _context.Clientes
                                                   where c_.id == C.Id_Cliente
                                                   select new
                                                   {
                                                       id = c_.id,
                                                       folio = c_.folio,
                                                       nombre = c_.nombre,
                                                       paterno = c_.paterno,
                                                       materno = c_.materno,
                                                       nombre_comercial = c_.nombre_comercial,
                                                       nombre_contacto = c_.nombre_contacto,
                                                       email = c_.email,
                                                       telefono = c_.telefono,
                                                       telefono_movil = c_.telefono_movil,
                                                       referencias = c_.referencias,
                                                       tipo_persona = c_.tipo_persona,
                                                       estatus = c_.estatus,
                                                       referidopor = c_.referidopor,
                                                       vigencia_ref = c_.vigencia_ref
                                                   }).ToList()
                            }).ToList();

                if (item == null)
                {
                    return response = Ok(new { result = "Error", detalle = "No se encontro la Cotización", data = item });
                }

                //var direcciones_ins_clie = (from a in _context.Cat_Direccion
                //                            where a.id_cliente == Cl.id && a.tipo_direccion == 1
                //                            orderby a.tipo_direccion ascending
                //                            select new
                //                            {
                //                                id = 0, calle_numero = a.calle_numero, colonia = a.colonia,
                //                                cp = a.cp,  nombrecontacto = a.nombrecontacto,id_estado = a.id_estado,
                //                                id_municipio = a.id_municipio,  telefono = a.telefono, tipo_direccion = a.tipo_direccion,
                //                                telefono_movil = a.telefono_movil, id_localidad = a.id_localidad,  numExt = a.numExt,
                //                                numInt = a.numInt,  Fecha_Estimada = a.Fecha_Estimada, id_cotizacion = busqueda.Id,
                //                                prefijo_ins = a.id_prefijo_calle,  id_prefijo_calle = a.id_prefijo_calle
                //                            }).ToList();

                //if (direcciones_ins_clie.Count > 0 && item[0].direcciones_ins.Count < 1)
                //    item[0].direcciones_ins.Add(direcciones_ins_clie[0]);
                
                //var direcciones_env_clie = (from a in _context.Cat_Direccion
                //                            where a.id_cliente == Cl.id && a.tipo_direccion == 2
                //                            orderby a.tipo_direccion ascending
                //                            select new
                //                            {
                //                                id = 0, calle_numero = a.calle_numero,  colonia = a.colonia,
                //                                cp = a.cp, nombrecontacto = a.nombrecontacto, id_estado = a.id_estado,
                //                                id_municipio = a.id_municipio, telefono = a.telefono, tipo_direccion = a.tipo_direccion,
                //                                telefono_movil = a.telefono_movil, id_localidad = a.id_localidad, numExt = a.numExt,
                //                                numInt = a.numInt, Fecha_Estimada = a.Fecha_Estimada, id_cotizacion = busqueda.Id,
                //                                prefijo_ins = a.id_prefijo_calle, id_prefijo_calle = a.id_prefijo_calle
                //                            }).ToList();

                //if (direcciones_env_clie.Count > 0 && item[0].direcciones_env.Count < 1)
                //    item[0].direcciones_env.Add(direcciones_env_clie[0]);

                return response = Ok(new { result = "Success", detalle = "Cotización Cargada Correctamente", item , promos_aplicables = item_m[0].promociones_respueta });
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message, id = 0 });
            }
            // return response = Ok(new { resultado = "Success" });
        }


        [Route("get_cat_tarjetas")]
        [HttpPost("get_cat_tarjetas")]
        public IActionResult get_cat_tarjetas()
        {
            IActionResult response = Unauthorized();
            var res = _context.cat_Tarjetas.Where(t => t.estatus == true).OrderBy(t => t.nombre).ToList();

            if (res.Count == 0)
            {
                return response = Ok(new { resultado = "Error", res });
            }
            return response = Ok(new { resultado = "Success", res });
            // return new ObjectResult(_item);
        }

        [Route("ActualizarServiciosCP")]
        [HttpPost("ActualizarServiciosCP")]
        public IActionResult ActualizarServiciosCP([FromBody] ModBusPorDosId ids)
        {
            //Id1 es el codigo postal, Id2 es el id de la cotizacion.
            bool actualizaCot = false;
            var cot = _context.Cotizaciones.Include(c => c.direcciones_cotizacion).FirstOrDefault(c => c.Id == ids.Id2);

            //1. Validar HP
            //var tiene_home = _context.Cotizacion_Producto.Where(C => C.Id_Cotizacion == ids.Id2 && _context.Cat_Productos.FirstOrDefault(P => P.id == C.Id_Producto).id_linea == 38).Count();
            var producto_home = (from pc in _context.Cotizacion_Producto
                              join pr in _context.Cat_Productos on pc.Id_Producto equals pr.id
                              where pr.id_linea == 38
                              orderby pr.id
                              select pc).ToList();
            var producto_cert = (from pc in _context.Cotizacion_Producto
                                 join pr in _context.Cat_Productos on pc.Id_Producto equals pr.id
                                 where pr.id_linea == 36
                                 select pc).ToList();

            //Busca el estado del nuevo CP y muestra los ids de los productos home program asociados
            var nuevo_edo = (from a in _context.Cat_Localidad
                       join b in _context.Cat_Municipio on a.municipio.id equals b.id
                       join c in _context.Cat_Estado on b.estado.id equals c.id
                       join hpe in _context.home_Producto_Estados on c.id equals hpe.id_estado
                       where a.cp == ids.Id1
                       orderby hpe.id_producto_home
                       select new
                       {
                           id_producto = hpe.id_producto_home,
                           id_estado = c.id,
                           estado = c.desc_estado
                       }).ToList();

            if (producto_home.Count > 0)
            {
                //Busca el estado del producto home existente en la cotizacion
                var edo_act = (from hpe in _context.home_Producto_Estados
                               where hpe.id_producto_home == producto_home[0].Id_Producto
                               select hpe).SingleOrDefault();
                //El home program no cambia
                int i = 0;
                foreach (Cotizacion_Producto item in producto_home)
                {

                    if (nuevo_edo[i].id_estado != edo_act.id_estado)
                    {
                        item.Id_Producto = nuevo_edo[i].id_producto;
                        _context.Cotizacion_Producto.Update(item);
                        actualizaCot = true;
                    }
                }
                if (actualizaCot) _context.SaveChanges();
            }

            if (producto_cert.Count >0 )
            {
                //
            }


            var tiene_certificado = _context.Cotizacion_Producto.Where(C => C.Id_Cotizacion == ids.Id2 && _context.Cat_Productos.FirstOrDefault(P => P.id == C.Id_Producto).id_linea == 36).Count();

            return Ok(new { result="Sucess" });
        }

        [Route("Roles_list")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Roles_list([FromBody]ModBusPorDosId busqueda)
        {
            IActionResult response = Unauthorized();
            var roles = (from c in _context.Cat_Roles

                            select new
                            {
                                c.id,
                               
                                c.rol,
                            }).ToList();

            if (roles == null)
            {
                return response = Ok(new { result = "Error", item = roles });
            }
            return response = Ok(new { result = "Success", item = roles });
        }

        [Route("Sucursales_List")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Sucursales_List([FromBody]ModBusPorDosId busqueda)
        {
            IActionResult response = Unauthorized();
            var roles = (from c in _context.Cat_Sucursales

                         select new
                         {
                             c.Id,
                             c.Sucursal,
                         }).ToList();

            if (roles == null)
            {
                return response = Ok(new { result = "Error", item = roles });
            }
            return response = Ok(new { result = "Success", item = roles });
        }


        // POST: api/Servicios/Actualizar_estatus
        [Route("crear_editar_direccion_cliente")]
        [HttpPost("{crear_editar_direccion_cliente}")]
        public IActionResult crear_editar_direccion_cliente([FromBody] Direcciones_Cliente item)
        {
            //var result = new Models.Response();

            IActionResult response = Unauthorized();

            try
            {
                if (item.id == 0) // Crea Registro 
                {
                    var direccion = _context.Direcciones_Cliente.Where(s => s.id_cliente == item.id_cliente && s.tipo_direccion == item.tipo_direccion).ToList();
                    if (direccion.Count == 0)
                    {
                        _context.Direcciones_Cliente.Add(item);
                        _context.SaveChanges();
                        return response = Ok(new { result = "Success", detalle = "Registro Creado con éxito.", id = item.id });
                    }

                    return response = Ok(new { result = "Success", detalle = "direccion de instalación existente" });
                }
                else // Edita Registo 
                {
                    var direccion = _context.Direcciones_Cliente.FirstOrDefault(s => s.id == item.id);
                    if (direccion == null)
                        return response = Ok(new { result = "Error", detalle = "No existe ningún registro con el Id: " + item.id, id = item.id });
                    else
                    {
                        direccion.id_cliente = item.id_cliente;
                        direccion.calle_numero = item.calle_numero;
                        direccion.cp = item.cp;
                        direccion.id_estado = item.id_estado;
                        direccion.id_municipio = item.id_municipio;
                        direccion.colonia = item.colonia;
                        direccion.telefono = item.telefono;
                        direccion.estatus = item.estatus;
                        direccion.creado = item.creado;
                        direccion.creadopor = item.creadopor;
                        direccion.actualizado = item.actualizado;
                        direccion.actualizadopor = item.actualizadopor;
                        direccion.tipo_direccion = item.tipo_direccion;
                        direccion.nombrecontacto = item.nombrecontacto;
                        direccion.numExt = item.numExt;
                        direccion.numInt = item.numInt;
                        direccion.telefono_movil = item.telefono_movil;
                        direccion.id_localidad = item.id_localidad;
                        direccion.Fecha_Estimada = item.Fecha_Estimada;
                        direccion.id_prefijo_calle = item.id_prefijo_calle;
                        _context.Direcciones_Cliente.Update(direccion);
                        _context.SaveChanges();
                        return response = Ok(new { result = "Success", detalle = "Registro actualizado con éxito.", id = direccion.id });
                    }
                }
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message, id = 0 });
            }
        }

        // POST: api/Servicios/Actualizar_estatus
        [Route("crear_editar_direccion")]
        [HttpPost("{crear_editar_direccion}")]
        public IActionResult crear_editar_direccion([FromBody] Cat_Direccion item)
        {
            //var result = new Models.Response();

            IActionResult response = Unauthorized();

            try
            {
                if (item.id == 0) // Crea Registro 
                {
                    var direccion = _context.Cat_Direccion.Where(s => s.id_cliente == item.id_cliente && s.tipo_direccion == item.tipo_direccion).ToList();
                    if(direccion.Count == 0)
                    {
                        _context.Cat_Direccion.Add(item);
                        _context.SaveChanges();
                        return response = Ok(new { result = "Success", detalle = "Registro Creado con éxito.", id = item.id });
                    }
                    
                    return response = Ok(new { result = "Success", detalle = "direccion de instalación existente" });
                }
                else // Edita Registo 
                {
                    var direccion = _context.Cat_Direccion.FirstOrDefault(s => s.id == item.id);
                    if (direccion == null)
                        return response = Ok(new { result = "Error", detalle = "No existe ningún registro con el Id: " + item.id, id = item.id });
                    else
                    {
                        direccion.id_cliente = item.id_cliente;
                        direccion.calle_numero = item.calle_numero;
                        direccion.cp = item.cp;
                        direccion.id_estado = item.id_estado;
                        direccion.id_municipio = item.id_municipio;
                        direccion.colonia = item.colonia;
                        direccion.telefono = item.telefono;
                        direccion.estatus = item.estatus;
                        direccion.creado = item.creado;
                        direccion.creadopor = item.creadopor;
                        direccion.actualizado = item.actualizado;
                        direccion.actualizadopor = item.actualizadopor;
                        direccion.tipo_direccion = item.tipo_direccion;
                        direccion.nombrecontacto = item.nombrecontacto;
                        direccion.numExt = item.numExt;
                        direccion.numInt = item.numInt;
                        direccion.telefono_movil = item.telefono_movil;
                        direccion.id_localidad = item.id_localidad;
                        direccion.Fecha_Estimada = item.Fecha_Estimada;
                        direccion.id_prefijo_calle = item.id_prefijo_calle;
                        _context.Cat_Direccion.Update(direccion);
                        _context.SaveChanges();
                        return response = Ok(new { result = "Success", detalle = "Registro actualizado con éxito.", id = direccion.id });
                    }
                }
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message, id = 0 });
            }
        }


        /*


                INICIO   SECCIÓN SUCURSALES 


                  */
        [Route("crear_editar_Sucursal")]
        [HttpPost("{crear_editar_Sucursal}")]
        public IActionResult crear_editar_sucursal([FromBody] Cat_Sucursales item, List<condiones_comerciales_sucursal> cc_suc)
        {
            //var result = new Models.Response();

            IActionResult response = Unauthorized();

            try
            {
                if (item.Id == 0) // Crea Registro 
                {
                    //item.condiones_comerciales_sucursal = cc_suc;
                    _context.Cat_Sucursales.Add(item);
                    _context.SaveChanges();
                    
                    List<Cat_SubLinea_Producto> all_sublineas = _context.Cat_SubLinea_Producto.Where(s => s.id > 0).ToList();
                    //foreach (var sub in all_sublineas)
                    //{
                    //    condiones_comerciales_sucursal cc = new condiones_comerciales_sucursal();
                    //    cc.id_Cat_SubLinea_Producto = sub.id;
                    //    cc.id_Cat_Sucursales = item.Id;
                    //    cc.margen = 10;
                    //    _context.condiones_comerciales_sucursal.Add(cc);
                    //    _context.SaveChanges();
                    //}
                    return response = Ok(new { result = "Success", detalle = "Registro Creado con éxito.", id = item.Id });

                }


                else // Edita Registo 
                {
                    var suc = _context.Cat_Sucursales.FirstOrDefault(s => s.Id == item.Id);

                    if (suc == null)
                        return response = Ok(new { result = "Error", detalle = "No existe ningún registro con el Id: " + item.Id, id = item.Id });
                    else
                    {
                        suc.Id_Cuenta = item.Id_Cuenta;
                        suc.Sucursal = item.Sucursal;
                        suc.url_logo = item.url_logo;
                        suc.margen_vendedores = item.margen_vendedores;
                        suc.tipo = item.tipo;
                        foreach (condiones_comerciales_sucursal cc in item.condiones_comerciales_sucursal)
                        {

                            if (cc.id > 0) _context.condiones_comerciales_sucursal.Update(cc);
                            else _context.condiones_comerciales_sucursal.Add(cc);
                        }
                        var fisc = _context.DatosFiscales_Sucursales.FirstOrDefault(s => s.id == item.DatosFiscales_Sucursales[0].id);

                        if (fisc != null)
                        {
                            fisc.rfc = item.DatosFiscales_Sucursales[0].rfc;
                            fisc.colonia = item.DatosFiscales_Sucursales[0].colonia;
                            fisc.calle_numero = item.DatosFiscales_Sucursales[0].calle_numero;
                            fisc.cp = item.DatosFiscales_Sucursales[0].cp;
                            fisc.email = item.DatosFiscales_Sucursales[0].email;
                            fisc.id_Sucursal = item.DatosFiscales_Sucursales[0].id_Sucursal;
                            fisc.id_estado = item.DatosFiscales_Sucursales[0].id_estado;
                            fisc.id_municipio = item.DatosFiscales_Sucursales[0].id_municipio;
                            fisc.nombre_fact = item.DatosFiscales_Sucursales[0].nombre_fact;
                            fisc.razon_social = item.DatosFiscales_Sucursales[0].razon_social;
                            fisc.telefono_fact = item.DatosFiscales_Sucursales[0].telefono_fact;

                            _context.DatosFiscales_Sucursales.Update(fisc);
                            //  _context.SaveChanges();

                        }
                        var dsuc = _context.cat_direccion_sucursales.FirstOrDefault(d => d.id == item.cat_direccion_sucursales[0].id);
                        if (dsuc != null)
                        {
                            dsuc.id_estado = item.cat_direccion_sucursales[0].id_estado;
                            dsuc.id_localidad = item.cat_direccion_sucursales[0].id_localidad;
                            dsuc.id_municipio = item.cat_direccion_sucursales[0].id_municipio;
                            dsuc.id_prefijo_calle = item.cat_direccion_sucursales[0].id_prefijo_calle;
                            //dsuc.id_sucursales = item.cat_direccion_sucursales[0].id_sucursales;
                            dsuc.calle_numero = item.cat_direccion_sucursales[0].calle_numero;
                            dsuc.numExt = item.cat_direccion_sucursales[0].numExt;
                            dsuc.numInt = item.cat_direccion_sucursales[0].numInt;
                            dsuc.cp = item.cat_direccion_sucursales[0].cp;
                            dsuc.actualizado = item.cat_direccion_sucursales[0].actualizado;
                            dsuc.telefono = item.cat_direccion_sucursales[0].telefono;

                            //dsuc.colonia = item.cat_direccion_sucursales[0].colonia;
                            _context.cat_direccion_sucursales.Update(dsuc);
                        }

                        _context.Cat_Sucursales.Update(suc);
                        _context.SaveChanges();
                        return response = Ok(new { result = "Success", detalle = "Registro actualizado con éxito.", id = suc.Id });
                    }
                }




            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message, id = 0 });
            }


        }

        [Route("crear_editar_usuario")]
        [HttpPost("{crear_editar_usuario}")]
        public IActionResult crear_editar_usuario([FromBody] Users item)
        {
            //var result = new Models.Response();

            IActionResult response = Unauthorized();

            try
            {
                if (item.id == 0) // Crea Registro 
                {
                    item.id_app = 2;
                    item.estatus = true;

                    var val_email = _context.Users.FirstOrDefault(u => u.email == item.email);
                    if(val_email != null)
                    {
                        return response = Ok(new { result = "Error", detalle = "El e-mail " + item.email + " ya existe, favor de verificarlo", id = item.id });
                    }
                    else
                    {
                        _context.Users.Add(item);
                        _context.SaveChanges();
                        return response = Ok(new { result = "Success", detalle = "Registro Creado con éxito.", id = item.id });
                    }
                }

                else // Edita Registo 
                {
                    var suc = _context.Users.FirstOrDefault(s => s.id == item.id);

                    if (suc == null)
                        return response = Ok(new { result = "Error", detalle = "No existe ningún registro con el Id: " + item.id, id = item.id });
                    else
                    {
                        var val_email = _context.Users.FirstOrDefault(u => u.email == item.email && u.id != item.id);
                        if (val_email != null)
                        {
                            return response = Ok(new { result = "Error", detalle = "El e-mail " + item.email + " ya existe, favor de verificarlo", id= item.id });
                        }
                        else
                        {
                            
                            if (!item.estatus && suc.id_rol != item.id_rol)
                            {
                                //cerrar sesiones de usuario desactivado
                                var query = _context.TokenItems.Where(t => t.Id_user == item.id);
                                _context.TokenItems.RemoveRange(query);
                            }

                            suc.id_app = 2;
                            suc.id_canal = item.id_canal;
                            suc.id_cuenta = item.id_cuenta;
                            suc.id_Sucursales = item.id_Sucursales;
                            suc.id_rol = item.id_rol;
                            suc.nivel = item.nivel;
                            suc.name = item.name;
                            suc.paterno = item.paterno;
                            suc.materno = item.materno;
                            suc.email = item.email;
                            suc.estatus = item.estatus;
                            suc.password = item.password;
                            suc.id_rol = item.id_rol;
                            suc.username = item.username;
                            suc.telefono = item.telefono;
                            suc.telefono_movil = item.telefono_movil;
                            suc.actualizadopor = item.creadopor;
                            suc.actualizado = DateTime.Now;

                            _context.Users.Update(suc);
                            _context.SaveChanges();
                            return response = Ok(new { result = "Success", detalle = "Registro actualizado con éxito.", id = suc.id });
                        }
                        
                    }
                }




            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message, id = 0 });
            }


        }


        [Route("crear_editar_cuenta")]
        [HttpPost("{crear_editar_cuenta}")]
        public IActionResult crear_editar_cuenta([FromBody] Cat_Cuentas item, Cat_CondicionesPago dat)
        {
            //var result = new Models.Response();

            IActionResult response = Unauthorized();

            try
            {
                if (item.Id == 0) // Crea Registro 
                {
                    _context.Cat_Cuentas.Add(item);
                    _context.SaveChanges();
                    return response = Ok(new { result = "Success", detalle = "Registro Creado con éxito.", id = item.Id });
                }
                if (item.cat_Formas_Pago.Count == 0)
                {

                    var cond = _context.Cat_CondicionesPago.Where(s => s.id_cuenta == item.Id);
                    foreach (Cat_CondicionesPago condiciones in cond)
                        _context.Cat_CondicionesPago.Remove(condiciones);
                    _context.SaveChanges();
                    return response = Ok(new { result = "Error", detalle = "No existe ningún registro con el Id: " + item.Id, id = item.Id });

                }
                else // Edita Registo 
                {
                    var cond = _context.Cat_CondicionesPago.Where(s => s.id_cuenta == item.Id);
                    foreach (Cat_CondicionesPago condiciones in cond)
                        _context.Cat_CondicionesPago.Remove(condiciones);

                    _context.Cat_Cuentas.Update(item);
                    _context.SaveChanges();
                    return response = Ok(new { result = "Success", detalle = "Registro Creado con éxito.", id = item.Id });


                    /*  var suc = _context.Cat_CondicionesPago.FirstOrDefault(s => s.id_cuenta == item.Id);
                      if (suc == null)
                          return response = Ok(new { result = "Error", detalle = "No existe ningún registro con el Id: " + item.Id, id = item.Id });
                      else
                      {




                          _context.Cat_Cuentas.Update(item);
                          _context.Cat_CondicionesPago.Update(suc);
                          _context.SaveChanges();
                          return response = Ok(new { result = "Success", detalle = "Registro actualizado con éxito.", id = suc.id });
                      }*/
                }




            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message, id = 0 });
            }


        }


        [Route("cargar_cat_sucur_id")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult cargar_cat_sucur_id([FromBody]ModBusPorId busqueda)
        {
            
            IActionResult response = Unauthorized();

            var cc_ = _context.condiones_comerciales_sucursal.Where(c => c.id_Cat_Sucursales == busqueda.Id);
            var sucur_id = (from c in _context.Cat_Sucursales
                            join u in _context.Cat_Cuentas on c.Id_Cuenta equals u.Id
                            where c.Id == busqueda.Id
                            select new
                            {
                                c.Id,
                                c.Id_Cuenta,
                                c.Sucursal,
                                c.url_logo,
                                c.tipo,
                                c.margen_vendedores,
                                u.Id_Canal,
                                datosFiscales_Sucursales = (from c1 in _context.DatosFiscales_Sucursales
                                                            where c1.id_Sucursal == busqueda.Id
                                                            select new
                                                            {
                                                                c1.id,
                                                                c1.id_estado,
                                                                c1.id_municipio,
                                                                c1.Int_fact,
                                                                c1.nombre_fact,
                                                                c1.razon_social,
                                                                c1.rfc,
                                                                c1.telefono_fact,
                                                                c1.calle_numero,
                                                                c1.colonia,
                                                                c1.email,
                                                                c1.Ext_fact,
                                                                c1.cp,
                                                                c1.id_Sucursal}).ToList(),
                                cat_direccion_sucursales = (from d1 in _context.cat_direccion_sucursales
                                                            where d1.id_sucursales == busqueda.Id
                                                            select new {
                                                                d1.id,
                                                                d1.id_estado,
                                                                d1.id_municipio,
                                                                d1.id_localidad,
                                                                d1.calle_numero,
                                                                d1.numExt,
                                                                d1.numInt,
                                                                d1.cp,
                                                                d1.colonia,
                                                                d1.telefono,
                                                                d1.id_prefijo_calle,
                                                                d1.tipo_direccion,
                                                                d1.creadopor,
                                                                d1.actualizadopor
                                                            }).ToList(),
                                condiones_comerciales_sucursal = (from t in _context.Cat_SubLinea_Producto
                                       //join x in _context.Cat_Sucursales on cc.id_Cat_Sucursales equals x.Id
                                       join  cc in cc_ on t.id equals cc.id_Cat_SubLinea_Producto into subq1
                                       from leftJ1 in subq1.DefaultIfEmpty()
                                       orderby t.descripcion, t.id_linea_producto ascending
                                       select new
                                       {
                                           id = leftJ1.id == null ? 0: leftJ1.id,
                                           id_Cat_Sucursales = c.Id,
                                           sucursal = c.Sucursal,
                                           margen = leftJ1.margen == null ? 0 : leftJ1.margen,
                                           margen_adicional = 0,
                                           id_Cat_SubLinea_Producto = leftJ1.id_Cat_SubLinea_Producto == null ? t.id : leftJ1.id_Cat_SubLinea_Producto,
                                           sublinea = t.descripcion,
                                           id_linea = t.id_linea_producto,
                                           linea = _context.Cat_Linea_Producto.FirstOrDefault(l => l.id == t.id_linea_producto).descripcion
                                       }).ToList()
                            }).ToList();


           
            if (sucur_id == null)
            {
                //var sublineas = _context.Cat_SubLinea_Producto.Include(sl=> sl.cat_linea_producto);
                //if (sucur_id[0].condiones_comerciales_sucursal.Count() < sublineas.Count())
                //{
                //    foreach (var item in sublineas)
                //    {
                //        if (!sucur_id[0].condiones_comerciales_sucursal.Exists(c=> c.id_Cat_SubLinea_Producto == item.id))
                //        {
                //            var a = new { 0, sucur_id[0].Id, sucur_id[0].Sucursal, 10, 0, item.id, item.descripcion, item.id_linea_producto, item.cat_linea_producto.descripcion };
                //            sucur_id[0].condiones_comerciales_sucursal.Add(a);
                //        }
                //    }
                //}

                return response = Ok(new { result = "Error", item = sucur_id });
            }
            return response = Ok(new { result = "Success", item = sucur_id });
            // return new ObjectResult(promocion); 
        }


        /*

            *
            *

            FIN SECCIÓN SUCURSALES 


            INICIO SECCIÓN CUENTAS 

            */



        [Route("get_entidades_busqueda_suc")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult get_entidades_busqueda_suc([FromBody]busqueda_entidades busqueda)


        {
            var U_session = (from a in _context.Users
                             where a.id == busqueda.IDUSR
                             select new { id_canal = a.id_canal, id_cuenta = a.id_cuenta, Id_sucursal = a.id_Sucursales, id_rol = a.id_rol, nivel = a.nivel, id = a.id }).ToList();

            IActionResult response = Unauthorized();
            long id_nivel = 0;
            long _cuenta = 0;
            long _canal = 0;
            if (busqueda.TextoLibre == null) { busqueda.TextoLibre = ""; }

            if (busqueda.Cuenta == "") { _cuenta = 0; } else { _cuenta = Convert.ToInt64(busqueda.Cuenta); }
            if (busqueda.Canal == "") { _canal = 0; } else { _canal = Convert.ToInt64(busqueda.Canal); }
            switch (U_session[0].nivel) { case "sucursal": id_nivel = 1; break; case "cuenta": id_nivel = 2; break; case "canal": id_nivel = 3; break; case "global": id_nivel = -1; break; }


            if (_canal != 0 && _cuenta != 0)
            {
                var sucursales = from c in _context.Cat_Sucursales
                                 join u in _context.Cat_Cuentas on c.Id_Cuenta equals u.Id
                                 join a in _context.Cat_canales on u.Id_Canal equals a.Id
                                 where EF.Functions.Like(c.Sucursal, "%" + busqueda.TextoLibre + "%")

                                 && u.Id_Canal == (id_nivel == 3 ? U_session[0].id_canal : u.Id_Canal)
                                 && c.Id_Cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.Id_Cuenta)
                                 && c.Id == (id_nivel == 1 ? U_session[0].Id_sucursal : c.Id)
                                 && c.Id_Cuenta == (_cuenta)
                                 && u.Id_Canal == (_canal)
                                 select new
                                 {
                                     id = (Int32)c.Id,
                                     entidad = c.Sucursal,
                                     tipo = "sucursal",
                                     cuenta = u.Cuenta_es,
                                     canal = a.Canal_es ,
                                     id_cuenta = u.Id,
                                     id_canal = a.Id

                                 };
                var entidades = sucursales;


                if (entidades == null)
                {
                    return response = Ok(new { result = "Error", item = entidades });
                }
                return response = Ok(new { result = "Success", item = entidades });

            }
            if (_canal == 0 && _cuenta != 0)
            {
                var sucursales = from c in _context.Cat_Sucursales
                                 join u in _context.Cat_Cuentas on c.Id_Cuenta equals u.Id
                                 join a in _context.Cat_canales on u.Id_Canal equals a.Id
                                 where EF.Functions.Like(c.Sucursal, "%" + busqueda.TextoLibre + "%")

                                 && u.Id_Canal == (id_nivel == 3 ? U_session[0].id_canal : u.Id_Canal)
                                 && c.Id_Cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.Id_Cuenta)
                                 && c.Id == (id_nivel == 1 ? U_session[0].Id_sucursal : c.Id)
                                 && c.Id_Cuenta == (_cuenta)
                                 select new
                                 {
                                     id = (Int32)c.Id,
                                     entidad = c.Sucursal,
                                     tipo = "sucursal",
                                     cuenta = u.Cuenta_es,
                                     canal = a.Canal_es,
                                     id_cuenta = u.Id,
                                     id_canal = a.Id

                                 };
                var entidades = sucursales;


                if (entidades == null)
                {
                    return response = Ok(new { result = "Error", item = entidades });
                }
                return response = Ok(new { result = "Success", item = entidades });

            }
            if (_canal != 0 && _cuenta == 0)
            {
                var sucursales = from c in _context.Cat_Sucursales
                                 join u in _context.Cat_Cuentas on c.Id_Cuenta equals u.Id
                                 join a in _context.Cat_canales on u.Id_Canal equals a.Id
                                 where EF.Functions.Like(c.Sucursal, "%" + busqueda.TextoLibre + "%")

                                 && u.Id_Canal == (id_nivel == 3 ? U_session[0].id_canal : u.Id_Canal)
                                 && c.Id_Cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.Id_Cuenta)
                                 && c.Id == (id_nivel == 1 ? U_session[0].Id_sucursal : c.Id)
                                 && u.Id_Canal == (_canal)
                                 select new
                                 {
                                     id = (Int32)c.Id,
                                     entidad = c.Sucursal,
                                     tipo = "sucursal",
                                     cuenta = u.Cuenta_es,
                                     canal = a.Canal_es,
                                     id_cuenta = u.Id,
                                     id_canal = a.Id

                                 };
                var entidades = sucursales;


                if (entidades == null)
                {
                    return response = Ok(new { result = "Error", item = entidades });
                }
                return response = Ok(new { result = "Success", item = entidades });

            }
            else
            {
                var sucursales = from c in _context.Cat_Sucursales
                                 join u in _context.Cat_Cuentas on c.Id_Cuenta equals u.Id
                                 join a in _context.Cat_canales on u.Id_Canal equals a.Id
                                 where EF.Functions.Like(c.Sucursal, "%" + busqueda.TextoLibre + "%")

                                 && u.Id_Canal == (id_nivel == 3 ? U_session[0].id_canal : u.Id_Canal)
                                 && c.Id_Cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.Id_Cuenta)
                                 && c.Id == (id_nivel == 1 ? U_session[0].Id_sucursal : c.Id)

                                 select new
                                 {
                                     id = (Int32)c.Id,
                                     entidad = c.Sucursal,
                                     tipo = "sucursal",
                                     cuenta = u.Cuenta_es,
                                     canal = a.Canal_es,
                                     id_cuenta = u.Id,
                                     id_canal = a.Id

                                 };
                var entidades = sucursales;


                if (entidades == null)
                {

                    return response = Ok(new { result = "Error", item = entidades });
                }
                return response = Ok(new { result = "Success", item = entidades });
            }



        }

        [Route("cargar_cat_cuenta_id")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult cargar_cat_cuenta_id([FromBody]ModBusPorId busqueda)
        {

            IActionResult response = Unauthorized();
            var cuenta_id = (from c in _context.Cat_Cuentas
                             join a in _context.Cat_CondicionesPago on c.Id equals a.id_cuenta

                             where c.Id == busqueda.Id
                             select new
                             {
                                 c.Id,
                                 c.Id_Canal,
                                 c.Cuenta_es,
                                 a.Cat_Formas_Pago,


                             }).ToList();

            if (cuenta_id.Count == 0)
            {
                var cuenta = (from c in _context.Cat_Cuentas


                              where c.Id == busqueda.Id
                              select new
                              {
                                  c.Id,
                                  c.Id_Canal,
                                  c.Cuenta_es,


                              }).ToList();
                if (cuenta == null)
                {
                    return response = Ok(new { result = "Error", item = cuenta });
                }
                return response = Ok(new { result = "Success", item = cuenta });
            }
            if (cuenta_id == null)
            {
                return response = Ok(new { result = "Error", item = cuenta_id });
            }
            return response = Ok(new { result = "Success", item = cuenta_id });


            // return new ObjectResult(promocion); 
            /*
              join u in _context.Cat_Formas_Pago on c.Id equals u.id
                             join a in _context.Cat_CondicionesPago on c.Id equals a.id
                             where c.Id == busqueda.Id &&
                             a.id_cuenta== busqueda.Id
             */
        }



        [Route("get_entidades_busqueda_com")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult get_entidades_busqueda_com([FromBody]busqueda_entidades busqueda)
        {

            var U_session = (from a in _context.Users
                             where a.id == busqueda.IDUSR
                             select new { id_canal = a.id_canal, id_cuenta = a.id_cuenta, Id_sucursal = a.id_Sucursales, id_rol = a.id_rol, nivel = a.nivel, id = a.id }).ToList();

            if (busqueda.FecFin == "" || busqueda.FecFin == null) busqueda.FecFin = "01/01/2050";
            if (busqueda.FecIni == "" || busqueda.FecIni == null) busqueda.FecIni = "01/01/1900";
            DateTime fI = Convert.ToDateTime(busqueda.FecFin);

            IActionResult response = Unauthorized();
            long _cuenta = 0;
            long _canal = 0;
            long id_nivel = 0;
            if (busqueda.TextoLibre == null) { busqueda.TextoLibre = ""; }
            // && a.Id_Vendedor == (U_session[0].id_rol == 10004 ? U_session[0].id : a.Id_Vendedor) no estan limitados solo a las suyas

            if (busqueda.Cuenta == "") { _cuenta = 0; } else { _cuenta = Convert.ToInt64(busqueda.Cuenta); }
            if (busqueda.Canal == "") { _canal = 0; } else { _canal = Convert.ToInt64(busqueda.Canal); }
            switch (U_session[0].nivel) { case "sucursal": id_nivel = 1; break; case "cuenta": id_nivel = 2; break; case "canal": id_nivel = 3; break; case "global": id_nivel = -1; break; }

            if (busqueda.id == 0 && _cuenta == 0 && _canal == 0 && busqueda.FecIni == "01/01/1900")
            {
                var comisiones_id = (from c in _context.Users
                                     // join a in _context.Cotizaciones on c.id equals a.Id_Vendedor
                                     where
                                      EF.Functions.Like(c.name, "%" + busqueda.TextoLibre + "%")
                                      && c.id_canal == (id_nivel == 3 ? U_session[0].id_canal : c.id_canal)
                                      && c.id_cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.id_cuenta)
                                      && c.id_Sucursales == (id_nivel == 1 ? U_session[0].Id_sucursal : c.id_Sucursales)
                                      && c.id == (U_session[0].id_rol == 10004 ? busqueda.IDUSR: c.id)
                                      && c.id_app == 2
                                     
                                     select new
                                     {
                                         id = c.id,
                                         name = c.name,
                                         paterno = c.paterno,
                                         materno = c.materno,
                                         username = c.username,
                                         email = c.email,
                                         nivel = c.nivel,
                                         id_comision = c.id,
                                         id_sucursal = c.id_Sucursales,
                                         id_cuenta = c.id_cuenta,
                                         id_canal = c.id_canal,
                                         canal = _context.Cat_canales.FirstOrDefault(x => x.Id == c.id_canal).Canal_es,
                                         cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == c.id_cuenta).Cuenta_es ,
                                         sucursal = _context.Cat_Sucursales.FirstOrDefault(x => x.Id == c.id_Sucursales).Sucursal,
                                         comision = _context.Cotizaciones.Where(x => x.Id_Vendedor == c.id && x.Estatus > 3).Sum(m => m.comision_vendedor),
                                         imp_precio = _context.Cotizaciones.Where(x => x.Id_Vendedor == c.id && x.Estatus > 3).Sum(m => m.importe_condiciones_com),
                                         iva_precio = _context.Cotizaciones.Where(x => x.Id_Vendedor == c.id && x.Estatus > 3).Sum(m => m.iva_condiciones_com ),
                                         
                                         // fechas = a.cambio_ord_comp_generada,


                                         /* fechas = (from x in _context.Cotizaciones
                                                    where x.Id_Vendedor == c.id && x.Estatus>3 
                                                    && Convert.ToDateTime(busqueda.FecFin).Date >= x.cambio_ord_comp_generada.Date
                                                    && Convert.ToDateTime(busqueda.FecIni).Date <= x.cambio_ord_comp_generada.Date
                                                    select x.cambio_ord_comp_generada),*/

                                     }).Distinct().ToList();
                if (comisiones_id == null)
                {
                    return response = Ok(new { result = "Error", item = comisiones_id });
                }

                return response = Ok(new { result = "Success", item = comisiones_id });
            }
            if (busqueda.id == 0 && _cuenta == 0 && _canal == 0 && busqueda.FecIni != "01/01/1900")
            {
                var comisiones_id = (from c in _context.Users
                                     join a in _context.Cotizaciones on c.id equals a.Id_Vendedor
                                     where
                                      EF.Functions.Like(c.name, "%" + busqueda.TextoLibre + "%")
                                      && c.id_canal == (id_nivel == 3 ? U_session[0].id_canal : c.id_canal)
                                      && c.id_cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.id_cuenta)
                                      && c.id_Sucursales == (id_nivel == 1 ? U_session[0].Id_sucursal : c.id_Sucursales)
                                      && c.id_app == 2
                                      && Convert.ToDateTime(busqueda.FecFin).Date >= a.cambio_ord_comp_generada.Date
                                      && Convert.ToDateTime(busqueda.FecIni).Date <= a.cambio_ord_comp_generada.Date

                                     select new
                                     {
                                         id = c.id,
                                         name = c.name,
                                         paterno = c.paterno,
                                         materno = c.materno,
                                         username = c.username,
                                         email = c.email,
                                         nivel = c.nivel,
                                         id_comision = c.id,
                                         id_sucursal = c.id_Sucursales,
                                         id_cuenta = c.id_cuenta,
                                         id_canal = c.id_canal,
                                         canal = _context.Cat_canales.FirstOrDefault(x => x.Id == c.id_canal).Canal_es,
                                         cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == c.id_cuenta).Cuenta_es,
                                         sucursal = _context.Cat_Sucursales.FirstOrDefault(x => x.Id == c.id_Sucursales).Sucursal,
                                         comision = _context.Cotizaciones.Where(x => x.Id_Vendedor == c.id && x.Estatus > 3).Sum(m => m.comision_vendedor).ToString("C"),
                                        // fechas = a.cambio_ord_comp_generada,
                                          /* fechas = (from x in _context.Cotizaciones
                                                    where x.Id_Vendedor == c.id && x.Estatus>3 
                                                    && Convert.ToDateTime(busqueda.FecFin).Date >= x.cambio_ord_comp_generada.Date
                                                    && Convert.ToDateTime(busqueda.FecIni).Date <= x.cambio_ord_comp_generada.Date
                                                    select x.cambio_ord_comp_generada),*/

                                     }).Distinct().ToList();
                if (comisiones_id == null)
                {
                    return response = Ok(new { result = "Error", item = comisiones_id });
                }
                return response = Ok(new { result = "Success", item = comisiones_id });
            }
            if (busqueda.id == 0 && _cuenta == 0 && _canal != 0 && busqueda.FecIni == "01/01/1900")
            {
                {
                    var users_id = (from c in _context.Users
                                        // join a in _context.Cotizaciones on c.id equals a.Id_Vendedor
                                    where EF.Functions.Like(c.name, "%" + busqueda.TextoLibre + "%")
                                     && c.id_canal == (id_nivel == 3 ? U_session[0].id_canal : c.id_canal)
                                     && c.id_cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.id_cuenta)
                                     && c.id_Sucursales == (id_nivel == 1 ? U_session[0].Id_sucursal : c.id_Sucursales)
                                     && c.id_canal == (_canal)
                                     && c.id_app == 2
                                    select new
                                    {
                                        id_usuario = c.id,
                                        name = c.name,
                                        paterno = c.paterno,
                                        materno = c.materno,
                                        username = c.username,
                                        email = c.email,
                                        nivel = c.nivel,
                                        id = c.id,
                                        id_sucursal = c.id_Sucursales,
                                        id_cuenta = c.id_cuenta,
                                        id_canal = c.id_canal,
                                        canal = _context.Cat_canales.FirstOrDefault(x => x.Id == c.id_canal).Canal_es,
                                        cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == c.id_cuenta).Cuenta_es,
                                        sucursal = _context.Cat_Sucursales.FirstOrDefault(x => x.Id == c.id_Sucursales).Sucursal,
                                         comision = _context.Cotizaciones.Where(x => x.Id_Vendedor == c.id && x.Estatus > 3).Sum(m => m.comision_vendedor).ToString("C")
                                    }).ToList();

                    if (users_id == null)
                    {
                        return response = Ok(new { result = "Error", item = users_id });
                    }
                    return response = Ok(new { result = "Success", item = users_id });
                }
            }


            if (busqueda.id == 0 && _cuenta != 0 && _canal == 0 && busqueda.FecIni == "01/01/1900")
            {
                {
                    var users_id = (from c in _context.Users
                                        //join a in _context.Cotizaciones on c.id equals a.Id_Vendedor
                                    where EF.Functions.Like(c.name, "%" + busqueda.TextoLibre + "%")
                                     && c.id_canal == (id_nivel == 3 ? U_session[0].id_canal : c.id_canal)
                                     && c.id_cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.id_cuenta)
                                     && c.id_Sucursales == (id_nivel == 1 ? U_session[0].Id_sucursal : c.id_Sucursales)
                                     && c.id_cuenta == (_cuenta)
                                     && c.id_app == 2
                                    select new
                                    {
                                        id_usuario = c.id,
                                        name = c.name,
                                        paterno = c.paterno,
                                        materno = c.materno,
                                        username = c.username,
                                        email = c.email,
                                        nivel = c.nivel,
                                        id = c.id,
                                        id_sucursal = c.id_Sucursales,
                                        id_cuenta = c.id_cuenta,
                                        id_canal = c.id_canal,
                                        canal = _context.Cat_canales.FirstOrDefault(x => x.Id == c.id_canal).Canal_es,
                                        cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == c.id_cuenta).Cuenta_es,
                                        sucursal = _context.Cat_Sucursales.FirstOrDefault(x => x.Id == c.id_Sucursales).Sucursal,
                                         comision = _context.Cotizaciones.Where(x => x.Id_Vendedor == c.id && x.Estatus > 3).Sum(m => m.comision_vendedor).ToString("C")
                                    }).ToList();

                    if (users_id == null)
                    {
                        return response = Ok(new { result = "Error", item = users_id });
                    }
                    return response = Ok(new { result = "Success", item = users_id });
                }
            }
            if (busqueda.id == 0 && _cuenta != 0 && _canal != 0 && busqueda.FecIni == "01/01/1900")
            {
                {
                    var users_id = (from c in _context.Users
                                    //join a in _context.Cotizaciones on c.id equals a.Id_Vendedor
                                    where EF.Functions.Like(c.name, "%" + busqueda.TextoLibre + "%")
                                     && c.id_canal == (id_nivel == 3 ? U_session[0].id_canal : c.id_canal)
                                     && c.id_cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.id_cuenta)
                                     && c.id_Sucursales == (id_nivel == 1 ? U_session[0].Id_sucursal : c.id_Sucursales)
                                     && c.id_cuenta == (_cuenta)
                                     && c.id_canal ==(_canal)
                                     && c.id_app == 2
                                    select new
                                    {
                                        id_usuario = c.id,
                                        name = c.name,
                                        paterno = c.paterno,
                                        materno = c.materno,
                                        username = c.username,
                                        email = c.email,
                                        nivel = c.nivel,
                                        id = c.id,
                                        id_sucursal = c.id_Sucursales,
                                        id_cuenta = c.id_cuenta,
                                        id_canal = c.id_canal,
                                        canal = _context.Cat_canales.FirstOrDefault(x => x.Id == c.id_canal).Canal_es, cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == c.id_cuenta).Cuenta_es , sucursal = _context.Cat_Sucursales.FirstOrDefault(x => x.Id == c.id_Sucursales).Sucursal,
                                         comision = _context.Cotizaciones.Where(x => x.Id_Vendedor == c.id && x.Estatus > 3).Sum(m => m.comision_vendedor).ToString("C")
                                    }).ToList();

                    if (users_id == null)
                    {
                        return response = Ok(new { result = "Error", item = users_id });
                    }
                    return response = Ok(new { result = "Success", item = users_id });
                }
            }
            if (busqueda.id == 0 && _cuenta != 0 && _canal == 0 && busqueda.FecIni != "01/01/1900")
            {
                {
                    var users_id = (from c in _context.Users
                                        // join a in _context.Cotizaciones on c.id equals a.Id_Vendedor
                                    where
                                     EF.Functions.Like(c.name, "%" + busqueda.TextoLibre + "%")
                                     && c.id_canal == (id_nivel == 3 ? U_session[0].id_canal : c.id_canal)
                                     && c.id_cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.id_cuenta)
                                     && c.id_Sucursales == (id_nivel == 1 ? U_session[0].Id_sucursal : c.id_Sucursales)

                                     && c.id_app == 2

                                    select new
                                    {
                                        id = c.id,
                                        name = c.name,
                                        paterno = c.paterno,
                                        materno = c.materno,
                                        username = c.username,
                                        email = c.email,
                                        nivel = c.nivel,
                                        id_comision = c.id,
                                        id_sucursal = c.id_Sucursales,
                                        id_cuenta = c.id_cuenta,
                                        id_canal = c.id_canal,
                                        canal = _context.Cat_canales.FirstOrDefault(x => x.Id == c.id_canal).Canal_es,
                                        cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == c.id_cuenta).Cuenta_es,
                                        sucursal = _context.Cat_Sucursales.FirstOrDefault(x => x.Id == c.id_Sucursales).Sucursal,
                                        comision = _context.Cotizaciones.Where(x => x.Id_Vendedor == c.id && x.Estatus > 3).Sum(m => m.comision_vendedor).ToString("C"),


                                    }).Distinct().ToList();

                    if (users_id == null)
                    {
                        return response = Ok(new { result = "Error", item = users_id });
                    }
                    return response = Ok(new { result = "Success", item = users_id });
                }
            }



            if (busqueda.id == 0 && _cuenta == 0 && _canal != 0 && busqueda.FecIni != "01/01/1900")
            {
                {
                    var users_id = (from c in _context.Users
                                        // join a in _context.Cotizaciones on c.id equals a.Id_Vendedor
                                    where
                                     EF.Functions.Like(c.name, "%" + busqueda.TextoLibre + "%")
                                     && c.id_canal == (id_nivel == 3 ? U_session[0].id_canal : c.id_canal)
                                     && c.id_cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.id_cuenta)
                                     && c.id_Sucursales == (id_nivel == 1 ? U_session[0].Id_sucursal : c.id_Sucursales)

                                     && c.id_app == 2

                                    select new
                                    {
                                        id = c.id,
                                        name = c.name,
                                        paterno = c.paterno,
                                        materno = c.materno,
                                        username = c.username,
                                        email = c.email,
                                        nivel = c.nivel,
                                        id_comision = c.id,
                                        id_sucursal = c.id_Sucursales,
                                        id_cuenta = c.id_cuenta,
                                        id_canal = c.id_canal,
                                        canal = _context.Cat_canales.FirstOrDefault(x => x.Id == c.id_canal).Canal_es,
                                        cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == c.id_cuenta).Cuenta_es,
                                        sucursal = _context.Cat_Sucursales.FirstOrDefault(x => x.Id == c.id_Sucursales).Sucursal,
                                        comision = _context.Cotizaciones.Where(x => x.Id_Vendedor == c.id && x.Estatus > 3).Sum(m => m.comision_vendedor).ToString("C"),


                                    }).Distinct().ToList();

                    if (users_id == null)
                    {
                        return response = Ok(new { result = "Error", item = users_id });
                    }
                    return response = Ok(new { result = "Success", item = users_id });
                }
            }
            if (busqueda.id == 0 && _cuenta == 0 && _canal != 0 && busqueda.FecIni == "01/01/1900")
            {
                {
                    var users_id = (from c in _context.Users
                                   // join a in _context.Cotizaciones on c.id equals a.Id_Vendedor
                                    where EF.Functions.Like(c.name, "%" + busqueda.TextoLibre + "%")
                                     && c.id_canal == (id_nivel == 3 ? U_session[0].id_canal : c.id_canal)
                                     && c.id_cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.id_cuenta)
                                     && c.id_Sucursales == (id_nivel == 1 ? U_session[0].Id_sucursal : c.id_Sucursales)
                                     && c.id_canal == (_canal)
                                     && c.id_app == 2
                                      
                                    select new
                                    {
                                        id_usuario = c.id,
                                        name = c.name,
                                        paterno = c.paterno,
                                        materno = c.materno,
                                        username = c.username,
                                        email = c.email,
                                        nivel = c.nivel,
                                        id = c.id,
                                        id_sucursal = c.id_Sucursales,
                                        id_cuenta = c.id_cuenta,
                                        id_canal = c.id_canal,
                                        canal = _context.Cat_canales.FirstOrDefault(x => x.Id == c.id_canal).Canal_es,
                                        cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == c.id_cuenta).Cuenta_es,
                                        sucursal = _context.Cat_Sucursales.FirstOrDefault(x => x.Id == c.id_Sucursales).Sucursal,
                                        comision = _context.Cotizaciones.Where(x => x.Id_Vendedor == c.id && x.Estatus > 3).Sum(m => m.comision_vendedor).ToString("C"),
                                      //  fechas = a.cambio_ord_comp_generada,

                                    }).ToList();

                    if (users_id == null)
                    {
                        return response = Ok(new { result = "Error", item = users_id });
                    }
                    return response = Ok(new { result = "Success", item = users_id });
                }
            }


            if (busqueda.id == 0 && _canal != 0 && _cuenta != 0 && busqueda.FecIni != "01/01/1900")
            {
                {
                    var users_id = (from c in _context.Users
                                    join a in _context.Cotizaciones on c.id equals a.Id_Vendedor
                                    where EF.Functions.Like(c.name, "%" + busqueda.TextoLibre + "%")
                                     && c.id_canal == (id_nivel == 3 ? U_session[0].id_canal : c.id_canal)
                                     && c.id_cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.id_cuenta)
                                     && c.id_Sucursales == (id_nivel == 1 ? U_session[0].Id_sucursal : c.id_Sucursales)
                                     && c.id_canal == (_canal)
                                     && c.id_cuenta == (_cuenta)
                                     && c.id_app == 2
                                         && Convert.ToDateTime(busqueda.FecFin).Date >= a.cambio_ord_comp_generada.Date
                                      && Convert.ToDateTime(busqueda.FecIni).Date <= a.cambio_ord_comp_generada.Date

                                    select new
                                    {
                                        id_usuario = c.id,
                                        name = c.name,
                                        paterno = c.paterno,
                                        materno = c.materno,
                                        username = c.username,
                                        email = c.email,
                                        nivel = c.nivel,
                                        id = c.id,
                                        id_sucursal = c.id_Sucursales,
                                        id_cuenta = c.id_cuenta,
                                        id_canal = c.id_canal,
                                        canal = _context.Cat_canales.FirstOrDefault(x => x.Id == c.id_canal).Canal_es,
                                        cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == c.id_cuenta).Cuenta_es,
                                        sucursal = _context.Cat_Sucursales.FirstOrDefault(x => x.Id == c.id_Sucursales).Sucursal,
                                        comision = _context.Cotizaciones.Where(x => x.Id_Vendedor == c.id && x.Estatus > 3).Sum(m => m.comision_vendedor).ToString("C"),
                                        fechas = a.cambio_ord_comp_generada,

                                    }).ToList();

                    if (users_id == null)
                    {
                        return response = Ok(new { result = "Error", item = users_id });
                    }
                    return response = Ok(new { result = "Success", item = users_id });
                }
            }
           
            else
            {
                var users_id = (from c in _context.Users
                                    // join a in _context.Cotizaciones on c.id equals a.Id_Vendedor
                                where
                                 EF.Functions.Like(c.name, "%" + busqueda.TextoLibre + "%")
                                 && c.id_canal == (id_nivel == 3 ? U_session[0].id_canal : c.id_canal)
                                 && c.id_cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.id_cuenta)
                                 && c.id_Sucursales == (id_nivel == 1 ? U_session[0].Id_sucursal : c.id_Sucursales)

                                 && c.id_app == 2

                                select new
                                {
                                    id = c.id,
                                    name = c.name,
                                    paterno = c.paterno,
                                    materno = c.materno,
                                    username = c.username,
                                    email = c.email,
                                    nivel = c.nivel,
                                    id_comision = c.id,
                                    id_sucursal = c.id_Sucursales,
                                    id_cuenta = c.id_cuenta,
                                    id_canal = c.id_canal,
                                    canal = _context.Cat_canales.FirstOrDefault(x => x.Id == c.id_canal).Canal_es,
                                    cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == c.id_cuenta).Cuenta_es,
                                    sucursal = _context.Cat_Sucursales.FirstOrDefault(x => x.Id == c.id_Sucursales).Sucursal,
                                    comision = _context.Cotizaciones.Where(x => x.Id_Vendedor == c.id && x.Estatus > 3).Sum(m => m.comision_vendedor).ToString("C"),
                                }).Distinct().ToList();

                if (users_id == null)
                {
                    return response = Ok(new { result = "Error", item = users_id });
                }
                return response = Ok(new { result = "Success", item = users_id });
            }

        }



        [Route("get_entidades_busqueda_com_id")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult get_entidades_busqueda_com_id([FromBody]busqueda_entidades busqueda)
        { /* && c.id_cuenta == (_cuenta)
                                &&c.id_canal ==(_canal)*/

            if (busqueda.FecFin == "" || busqueda.FecFin == null) busqueda.FecFin = "01/01/2050";
            if (busqueda.FecIni == "" || busqueda.FecIni == null) busqueda.FecIni = "01/01/1900";

            /*         DateTime fI = Convert.ToDateTime(busqueda.FecFin);
                                      */

            IActionResult response = Unauthorized();

            var users_id = (from c in _context.Users
                            join a in _context.Cotizaciones on c.id equals a.Id_Vendedor 
                            join b in _context.comisiones_vendedores on a.Id equals b.id_cotizacion 
                            join d in _context.cat_tipos_comisiones on b.id_cat_tipo_comision equals d.id

                            where c.id == busqueda.id
                            && a.Estatus > 3
                             && Convert.ToDateTime(busqueda.FecFin).Date >= b.fecha_generacion.Date
                             && Convert.ToDateTime(busqueda.FecIni).Date <= b.fecha_generacion.Date
                            select new
                            {
                                id = a.Id,
                                id_usuario = c.id,
                                name = c.name,
                                paterno = c.paterno,
                                materno = c.materno,
                                username = c.username,
                                email = c.email,
                                nivel = c.nivel,
                                id_comision = a.Id,
                                estatus = a.Estatus,
                                fec_generacion = b.fecha_generacion.ToShortDateString(),
                                comision = a.comision_vendedor,
                                tipo_comision = d.tipo_comision,
                                pagada = b.pagada,
                                fecha_pago = (from x in _context.comisiones_vendedores
                                         where x.id_cotizacion == a.Id && x.pagada == true
                                         select x.fecha_de_pago.ToShortDateString()),
                                canal = (from x in _context.Cat_canales
                                         where x.Id == c.id_canal
                                         select x.Canal_es),
                                estatus_ = (from x in _context.cat_tipos_comisiones
                                            where x.id == a.Estatus
                                            select x.tipo_comision),
                                cuenta = (from x in _context.Cat_Cuentas
                                          where x.Id == c.id_cuenta
                                          select x.Cuenta_es),
                                sucursal = (from x in _context.Cat_Sucursales
                                            where x.Id == c.id_Sucursales
                                            select x.Sucursal),
                                importe_promociones = a.importe_precio_lista,
                                iva_promociones = a.iva_precio_lista,
                                comisiontotal = _context.Cotizaciones.Where(x => x.Id_Vendedor == busqueda.id && x.Estatus > 3).Sum(m => m.comision_vendedor),
                                //comisiontotalpagada = _context.comisiones_vendedores.FirstOrDefault(cv => a.Id == cv.id_cotizacion && cv.pagada == true). ,

                                comisiontotalpagada = (from b in _context.comisiones_vendedores
                                                       where a.Id == b.id_cotizacion
                                                       && b.pagada == true
                                                       select a.comision_vendedor).FirstOrDefault(),


                            }).ToList();

            var _vendedor = (from c in _context.Users
                             join a in _context.Cotizaciones on c.id equals a.Id_Vendedor
                             join b in _context.comisiones_vendedores on a.Id equals b.id_cotizacion
                             join d in _context.cat_tipos_comisiones on b.id_cat_tipo_comision equals d.id

                             where c.id == busqueda.id
                             && a.Estatus > 3
                             
                             select new
                             {
                                 id = a.Id,
                                 id_usuario = c.id,
                                 name = c.name,
                                 paterno = c.paterno,
                                 materno = c.materno,
                                 username = c.username,
                                 email = c.email,
                                 nivel = c.nivel,
                                 id_comision = a.Id,
                                 estatus = a.Estatus,
                                 fec_generacion = b.fecha_generacion.ToShortDateString(),
                                 comision = a.comision_vendedor,
                                 tipo_comision = d.tipo_comision,
                                 pagada = b.pagada,
                                 fecha_pago = (from x in _context.comisiones_vendedores
                                               where x.id_cotizacion == a.Id && x.pagada == true
                                               select x.fecha_de_pago.ToShortDateString()),
                                 canal = (from x in _context.Cat_canales
                                          where x.Id == c.id_canal
                                          select x.Canal_es),
                                 estatus_ = (from x in _context.cat_tipos_comisiones
                                             where x.id == a.Estatus
                                             select x.tipo_comision),
                                 cuenta = (from x in _context.Cat_Cuentas
                                           where x.Id == c.id_cuenta
                                           select x.Cuenta_es),
                                 sucursal = (from x in _context.Cat_Sucursales
                                             where x.Id == c.id_Sucursales
                                             select x.Sucursal),
                                 importe_promociones = a.importe_precio_lista,
                                 iva_promociones = a.iva_precio_lista,
                                 comisiontotal = _context.Cotizaciones.Where(x => x.Id_Vendedor == busqueda.id && x.Estatus > 3).Sum(m => m.comision_vendedor),

                                 comisiontotalpagada = (from b in _context.comisiones_vendedores
                                                        where a.Id == b.id_cotizacion
                                                        && b.pagada == true
                                                        select a.comision_vendedor),


                             }).ToList();

            var _vendedordat = (from c in _context.Users
                            
                             where c.id == busqueda.id
                           
                             select new
                             {
                                 id_usuario = c.id,
                                 name = c.name,
                                 paterno = c.paterno,
                                 materno = c.materno,
                                 username = c.username,
                                 email = c.email,
                                 nivel = c.nivel,
                            
                                 canal = (from x in _context.Cat_canales
                                          where x.Id == c.id_canal
                                          select x.Canal_es),
                               
                                 cuenta = (from x in _context.Cat_Cuentas
                                           where x.Id == c.id_cuenta
                                           select x.Cuenta_es),
                                 sucursal = (from x in _context.Cat_Sucursales
                                             where x.Id == c.id_Sucursales
                                             select x.Sucursal),
                                

                             }).ToList();



            if (users_id == null)
            {
                return response = Ok(new { result = "Error", item = users_id, vendedor = _vendedordat });
            }
            if (users_id.Count == 0)
            {
                
                return response = Ok(new { result = "Success", item = _vendedordat });
            }
            return response = Ok(new { result = "Success", item = users_id, vendedor = _vendedor });
        }


        [Route("pagar_comision")]
        [HttpPost("{pagar_comision}")]
        public IActionResult pagar_comision([FromBody] comisiones_vendedores item)
        {
            //var result = new Models.Response();

            IActionResult response = Unauthorized();

            try
            {
                //  item.cat_tipos_comisiones.id = 1; 

                var comisiones_vendedores = _context.comisiones_vendedores.FirstOrDefault(s => s.id_cotizacion == item.id_cotizacion);


                comisiones_vendedores.pagada = true;
                comisiones_vendedores.fecha_de_pago = DateTime.Now;
                comisiones_vendedores.id_quienpago = item.id_quienpago; 


                //_context.comisiones_vendedores.Add(item);
                _context.SaveChanges();
                return response = Ok(new { result = "Success", detalle = "Registro Creado con éxito.", id = item.id });



            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message, id = 0 });
            }


        }

        [Route("pagar_comision_suc")]
        [HttpPost("{pagar_comision_suc}")]
        public IActionResult pagar_comision_suc([FromBody] comisiones_sucursales item)
        {
            //var result = new Models.Response();
            IActionResult response = Unauthorized();

            try
            {

                var comisiones_sucursales = _context.comisiones_sucursales.FirstOrDefault(s => s.id_cotizacion == item.id_cotizacion);


                comisiones_sucursales.pagada = true;
                comisiones_sucursales.fecha_de_pago = DateTime.Now;
                comisiones_sucursales.id_quienpago = item.id_quienpago;


                //_context.comisiones_vendedores.Add(item);
                _context.SaveChanges();
                return response = Ok(new { result = "Success", detalle = "Registro Creado con éxito.", id = item.id });




            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message, id = 0 });
            }
        }




        [Route("get_entidades_busqueda_usu")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult get_entidades_busqueda_usu([FromBody]busqueda_entidades busqueda)
        {

            var U_session = (from a in _context.Users
                             where a.id == busqueda.IDUSR
                             select new { id_canal = a.id_canal, id_cuenta = a.id_cuenta, Id_sucursal = a.id_Sucursales, id_rol = a.id_rol, nivel = a.nivel, id = a.id }).ToList();


            IActionResult response = Unauthorized();
            long _cuenta = 0;
            long _canal = 0;
            long id_nivel = 0;
            if (busqueda.TextoLibre == null) { busqueda.TextoLibre = ""; }

            if (busqueda.Cuenta == "") { _cuenta = 0; } else { _cuenta = Convert.ToInt64(busqueda.Cuenta); }
            if (busqueda.Canal == "") { _canal = 0; } else { _canal = Convert.ToInt64(busqueda.Canal); }
            switch (U_session[0].nivel) { case "sucursal": id_nivel = 1; break; case "cuenta": id_nivel = 2; break; case "canal": id_nivel = 3; break; case "global": id_nivel = -1; break; }

            if (busqueda.id == 0 && _cuenta == 0 && _canal == 0)
            {
                var users_id = (from c in _context.Users
                                where EF.Functions.Like(c.name, "%" + busqueda.TextoLibre + "%")
                                 && c.id_canal == (id_nivel == 3 ? U_session[0].id_canal : c.id_canal)
                                 && c.id_cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.id_cuenta)
                                 && c.id_Sucursales == (id_nivel == 1 ? U_session[0].Id_sucursal : c.id_Sucursales)
                                 &&c.id_app ==2
                                select new
                                {
                                    c.id,
                                    c.id_Sucursales,
                                    c.id_canal,
                                    c.id_cuenta,
                                    c.id_rol,
                                    c.name,
                                    c.paterno,
                                    c.materno,
                                    c.username,
                                    c.telefono,
                                    c.telefono_movil,
                                    c.email,
                                    c.nivel,
                                    c.password,
                                    canal = _context.Cat_canales.FirstOrDefault(x => x.Id == c.id_canal).Canal_es,
                                    cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == c.id_cuenta).Cuenta_es,
                                    sucursal = _context.Cat_Sucursales.FirstOrDefault(x => x.Id == c.id_Sucursales).Sucursal,
                                    rol = _context.Cat_Roles.FirstOrDefault(x => x.id == c.id_rol).rol,


                                }).ToList();

                if (users_id == null)
                {
                    return response = Ok(new { result = "Error", item = users_id });
                }
                return response = Ok(new { result = "Success", item = users_id });
            }
            if (busqueda.id == 0 && _cuenta != 0 && _canal == 0)
            {
                {
                    var users_id = (from c in _context.Users


                                    where EF.Functions.Like(c.name, "%" + busqueda.TextoLibre + "%")
                                      && c.id_canal == (id_nivel == 3 ? U_session[0].id_canal : c.id_canal)
                                 && c.id_cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.id_cuenta)
                                 && c.id_Sucursales == (id_nivel == 1 ? U_session[0].Id_sucursal : c.id_Sucursales)
                                  && c.id_app == 2
                                   && c.id_cuenta == (_cuenta)
                                    select new
                                    {
                                        c.id,
                                        c.id_Sucursales,
                                        c.id_canal,
                                        c.id_cuenta,
                                        c.id_rol,
                                        c.name,
                                        c.paterno,
                                        c.materno,
                                        c.username,
                                        c.telefono,
                                        c.telefono_movil,
                                        c.email,
                                        c.nivel,
                                        c.password,
                                        canal = _context.Cat_canales.FirstOrDefault(x => x.Id == c.id_canal).Canal_es,
                                        cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == c.id_cuenta).Cuenta_es,
                                        sucursal = _context.Cat_Sucursales.FirstOrDefault(x => x.Id == c.id_Sucursales).Sucursal,
                                        rol = _context.Cat_Roles.FirstOrDefault(x => x.id == c.id_rol).rol,
                                    }).ToList();

                    if (users_id == null)
                    {
                        return response = Ok(new { result = "Error", item = users_id });
                    }
                    return response = Ok(new { result = "Success", item = users_id });
                }
            }
            if (busqueda.id == 0 && _canal != 0 && _cuenta == 0)
            {
                {
                    var users_id = (from c in _context.Users


                                    where EF.Functions.Like(c.name, "%" + busqueda.TextoLibre + "%")
                                      && c.id_canal == (id_nivel == 3 ? U_session[0].id_canal : c.id_canal)
                                 && c.id_cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.id_cuenta)
                                 && c.id_Sucursales == (id_nivel == 1 ? U_session[0].Id_sucursal : c.id_Sucursales)
                                  && c.id_app == 2
                                    && c.id_canal == (_canal)
                                    select new
                                    {
                                        c.id,
                                        c.id_Sucursales,
                                        c.id_canal,
                                        c.id_cuenta,
                                        c.id_rol,
                                        c.name,
                                        c.paterno,
                                        c.materno,
                                        c.username,
                                        c.telefono,
                                        c.telefono_movil,
                                        c.email,
                                        c.nivel,
                                        c.password,
                                        canal = _context.Cat_canales.FirstOrDefault(x => x.Id == c.id_canal).Canal_es,
                                        cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == c.id_cuenta).Cuenta_es,
                                        sucursal = _context.Cat_Sucursales.FirstOrDefault(x => x.Id == c.id_Sucursales).Sucursal,
                                        rol = _context.Cat_Roles.FirstOrDefault(x => x.id == c.id_rol).rol,
                                    }).ToList();

                    if (users_id == null)
                    {
                        return response = Ok(new { result = "Error", item = users_id });
                    }
                    return response = Ok(new { result = "Success", item = users_id });
                }
            }
            if (busqueda.id == 0 && _canal != 0 && _cuenta != 0)
            {
                {
                    var users_id = (from c in _context.Users


                                    where EF.Functions.Like(c.name, "%" + busqueda.TextoLibre + "%")
                                      && c.id_canal == (id_nivel == 3 ? U_session[0].id_canal : c.id_canal)
                                 && c.id_cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.id_cuenta)
                                 && c.id_Sucursales == (id_nivel == 1 ? U_session[0].Id_sucursal : c.id_Sucursales)
                                  && c.id_app == 2
                                    && c.id_canal == (_canal)
                                    && c.id_cuenta == (_cuenta)
                                    select new
                                    {
                                        c.id,
                                        c.id_Sucursales,
                                        c.id_canal,
                                        c.id_cuenta,
                                        c.id_rol,
                                        c.name,
                                        c.paterno,
                                        c.materno,
                                        c.username,
                                        c.telefono,
                                        c.telefono_movil,
                                        c.email,
                                        c.nivel,
                                        c.password,
                                        canal = _context.Cat_canales.FirstOrDefault(x => x.Id == c.id_canal).Canal_es,
                                        cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == c.id_cuenta).Cuenta_es,
                                        sucursal = _context.Cat_Sucursales.FirstOrDefault(x => x.Id == c.id_Sucursales).Sucursal,
                                        rol = _context.Cat_Roles.FirstOrDefault(x => x.id == c.id_rol).rol,
                                    }).ToList();

                    if (users_id == null)
                    {
                        return response = Ok(new { result = "Error", item = users_id });
                    }
                    return response = Ok(new { result = "Success", item = users_id });
                }
            }
            else
            {
                var users_id = (from c in _context.Users
                                where c.id == busqueda.id
                                  && c.id_canal == (id_nivel == 3 ? U_session[0].id_canal : c.id_canal)
                             && c.id_cuenta == (id_nivel == 2 ? U_session[0].id_cuenta : c.id_cuenta)
                             && c.id_Sucursales == (id_nivel == 1 ? U_session[0].Id_sucursal : c.id_Sucursales)
                              && c.id_app == 2
                                select new
                                {
                                    c.id,
                                    c.id_Sucursales,
                                    c.id_canal,
                                    c.id_cuenta,
                                    c.id_rol,
                                    c.name,
                                    c.paterno,
                                    c.telefono,
                                    c.telefono_movil,
                                    c.materno,
                                    c.username,
                                    c.email,
                                    c.nivel,
                                    canal = _context.Cat_canales.FirstOrDefault(x => x.Id == c.id_canal).Canal_es,
                                    cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == c.id_cuenta).Cuenta_es,
                                    sucursal = _context.Cat_Sucursales.FirstOrDefault(x => x.Id == c.id_Sucursales).Sucursal,
                                    rol = _context.Cat_Roles.FirstOrDefault(x => x.id == c.id_rol).rol,
                                }).ToList();

                if (users_id == null)
                {
                    return response = Ok(new { result = "Error", item = users_id });
                }
                return response = Ok(new { result = "Success", item = users_id });
            }
            // return new ObjectResult(promocion); 
            /*
              join u in _context.Cat_Formas_Pago on c.Id equals u.id
                             join a in _context.Cat_CondicionesPago on c.Id equals a.id
                             where c.Id == busqueda.Id &&
                             a.id_cuenta== busqueda.Id
             */
        }


        [Route("get_entidades_busqueda_usu_id")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult get_entidades_busqueda_usu_id([FromBody]busqueda_entidades busqueda)
        { /* && c.id_cuenta == (_cuenta)
                                &&c.id_canal ==(_canal)*/

            IActionResult response = Unauthorized();

            var users_id = (from c in _context.Users
                            where c.id == busqueda.id
                            select new
                            {
                                c.id,
                                c.id_Sucursales,
                                c.id_canal,
                                c.id_cuenta,
                                c.id_rol,
                                c.name,
                                c.paterno,
                                c.telefono,
                                c.telefono_movil,
                                c.materno,
                                c.username,
                                c.email,
                                c.nivel,
                                c.password,
                                c.estatus,
                            }).ToList();

            if (users_id == null)
            {
                return response = Ok(new { result = "Error", item = users_id });
            }
            return response = Ok(new { result = "Success", item = users_id });
        }


        [Route("get_entidades_busqueda_cuentas")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult get_entidades_busqueda_cuentas([FromBody]busqueda_entidades busqueda)
        {
            var U_session = (from a in _context.Users
                             where a.id == busqueda.IDUSR
                             select new { id_canal = a.id_canal, id_cuenta = a.id_cuenta, Id_sucursal = a.id_Sucursales, id_rol = a.id_rol, nivel = a.nivel, id = a.id }).ToList();

            IActionResult response = Unauthorized();

            long id_nivel = 0;
            long _cuenta = 0;
            long _canal = 0;
            if (busqueda.TextoLibre == null) { busqueda.TextoLibre = ""; }
            if (busqueda.Cuenta == "") { _cuenta = 0; } else { _cuenta = Convert.ToInt64(busqueda.Cuenta); }
            if (busqueda.Canal == "") { _canal = 0; } else { _canal = Convert.ToInt64(busqueda.Canal); }
            switch (U_session[0].nivel) { case "sucursal": id_nivel = 1; break; case "cuenta": id_nivel = 2; break; case "canal": id_nivel = 3; break; case "global": id_nivel = -1; break; }


            if (_canal != 0 && _cuenta != 0)
            {

                var cuentas = from u in _context.Cat_Cuentas
                              join a in _context.Cat_canales on u.Id_Canal equals a.Id
                              where EF.Functions.Like(u.Cuenta_es, "%" + busqueda.TextoLibre + "%")
                                 && u.Id_Canal == (id_nivel == 3 ? U_session[0].id_canal : u.Id_Canal)
                                 && u.Id == (id_nivel == 2 ? U_session[0].id_cuenta : u.Id)
                                 && u.Id == (id_nivel == 1 ? U_session[0].Id_sucursal : u.Id)
                                 && u.Id == (_cuenta)
                                 && a.Id == (_canal)
                              select new
                              {
                                  id = (Int32)u.Id,
                                  entidad = u.Cuenta_es,
                                  tipo = "Cuenta",
                                  cuenta = "",
                                  canal = a.Canal_es, 
                                  id_cuenta = u.Id,
                                  id_canal = a.Id
                              };

                var entidades = cuentas;

                if (entidades == null)
                {
                    return response = Ok(new { result = "Error", item = entidades });
                }
                return response = Ok(new { result = "Success", item = entidades });

            }
            if (_canal == 0 && _cuenta != 0)
            {

                var cuentas = from u in _context.Cat_Cuentas
                              join a in _context.Cat_canales on u.Id_Canal equals a.Id
                              where EF.Functions.Like(u.Cuenta_es, "%" + busqueda.TextoLibre + "%")
                                 && u.Id_Canal == (id_nivel == 3 ? U_session[0].id_canal : u.Id_Canal)
                                 && u.Id == (id_nivel == 2 ? U_session[0].id_cuenta : u.Id)
                                 && u.Id == (id_nivel == 1 ? U_session[0].Id_sucursal : u.Id)
                                 && u.Id == (_cuenta)
                              select new
                              {
                                  id = (Int32)u.Id,
                                  entidad = u.Cuenta_es,
                                  tipo = "Cuenta",
                                  cuenta = "",
                                  canal = a.Canal_es,
                                  id_cuenta = u.Id,
                                  id_canal = a.Id
                              };

                var entidades = cuentas;

                if (entidades == null)
                {
                    return response = Ok(new { result = "Error", item = entidades });
                }
                return response = Ok(new { result = "Success", item = entidades });

            }
            if (_canal != 0 && _cuenta == 0)
            {

                var cuentas = from u in _context.Cat_Cuentas
                              join a in _context.Cat_canales on u.Id_Canal equals a.Id
                              where EF.Functions.Like(u.Cuenta_es, "%" + busqueda.TextoLibre + "%")
                                 && u.Id_Canal == (id_nivel == 3 ? U_session[0].id_canal : u.Id_Canal)
                                 && u.Id == (id_nivel == 2 ? U_session[0].id_cuenta : u.Id)
                                 && u.Id == (id_nivel == 1 ? U_session[0].Id_sucursal : u.Id)
                                 && a.Id == (_canal)
                              select new
                              {
                                  id = (Int32)u.Id,
                                  entidad = u.Cuenta_es,
                                  tipo = "Cuenta",
                                  cuenta = "",
                                  canal = a.Canal_es,
                                  id_cuenta = u.Id,
                                  id_canal = a.Id
                              };

                var entidades = cuentas;


                if (entidades == null)
                {
                    return response = Ok(new { result = "Error", item = entidades });
                }
                return response = Ok(new { result = "Success", item = entidades });

            }
            else
            {

                var cuentas = from u in _context.Cat_Cuentas
                              join a in _context.Cat_canales on u.Id_Canal equals a.Id
                              where EF.Functions.Like(u.Cuenta_es, "%" + busqueda.TextoLibre + "%")
                                 && u.Id_Canal == (id_nivel == 3 ? U_session[0].id_canal : u.Id_Canal)
                                 && u.Id == (id_nivel == 2 ? U_session[0].id_cuenta : u.Id)
                                 && u.Id == (id_nivel == 1 ? U_session[0].Id_sucursal : u.Id)
                              select new
                              {
                                  id = (Int32)u.Id,
                                  entidad = u.Cuenta_es,
                                  tipo = "Cuenta",
                                  cuenta = "",
                                  canal = a.Canal_es,
                                  id_cuenta = u.Id,
                                  id_canal = a.Id
                              };

                var entidades = cuentas;


                if (entidades == null)
                {
                    return response = Ok(new { result = "Error", item = entidades });
                }
                return response = Ok(new { result = "Success", item = entidades });
            }
        }

        /*
        FIN SECCIÓN CUENTAS 


        */

        // POST: api/Partners_Productos
        [Route("cancelar_cotizacion")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult cancelar_cotizacion([FromBody]ModIdTexto busqueda)
        {
            //var result = new Models.Response();

            IActionResult response = Unauthorized();

            try
            {
                var cotizacion = _context.Cotizaciones.FirstOrDefault(s => s.Id == busqueda.Id);
                if (cotizacion == null)
                    return response = Ok(new { result = "Error", detalle = "No existe ningún registro con el Id: " + busqueda.Id, id = busqueda.Id });
                else
                {
                    cotizacion.cancelada = true;
                    cotizacion.coment_cancel = busqueda.texto;
                    _context.Cotizaciones.Update(cotizacion);
                    _context.SaveChanges();
                    //correo1 confirmacion de cancelacion
                    var vendedor = _context.Users.FirstOrDefault(c => c.id == cotizacion.Id_Vendedor);
                    if (vendedor != null)
                    {
                        //var canal = _context.Cat_canales.FirstOrDefault(c => c.Id == cotizacion.Id_Canal);
                        var ctaSuc = _context.Cat_Sucursales.Include(s => s.Cuenta).FirstOrDefault(s => s.Id == cotizacion.Id_sucursal);
                        var admin_canal = _context.Users.FirstOrDefault(us => us.id_canal == vendedor.id_canal && us.nivel == "canal");
                        StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_Interno.html"));
                        string body = string.Empty;
                        body = reader.ReadToEnd();
                        body = body.Replace("{Titulo}", "Solicitud Cancelada");
                        body = body.Replace("{Cuenta}", ", cuenta <strong>" + ctaSuc.Cuenta.Cuenta_es + "</strong>");
                        body = body.Replace("{Sucursal}", ", sucursal <strong>" + ctaSuc.Sucursal + "</strong>");
                        body = body.Replace("{Username}", vendedor.name + " " + vendedor.paterno);
                        body = body.Replace("{Texto}", "Se canceló correctamente la cotización número ");
                        body = body.Replace("{NumeroCotizacion}", cotizacion.Id.ToString());
                        SendMail(vendedor.email, body, "Cotización Cancelada # " + cotizacion.Id.ToString());

                        if (admin_canal != null)
                        {
                            body.Replace(vendedor.name + " " + vendedor.paterno, admin_canal.name + " " + admin_canal.paterno);
                            SendMail(admin_canal.email, body, "Cotización Cancelada # " + cotizacion.Id.ToString());
                        }
                    }
                    return response = Ok(new { result = "Success", detalle = "Registro actualizado con éxito.", id = busqueda.Id });

                }
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message, id = 0 });
            }
        }

        private void SendMail(string emailTo, string body, string subject)
        {
            var ruta = _context.parametro_Archivos.FirstOrDefault(p => p.id == 2);
            body = body.Replace("{ruta_rec}", ruta.funcion);

            EmailModel email = new EmailModel();
            //email.To = "luishs@minimalist.mx";
            email.To = emailTo;
            email.Subject = subject;
            email.Body = body;
            email.IsBodyHtml = true;
            email.id_app = 2;
            _emailRepository.SendEmail(email);
            
            //new SmtpClient
            //{
            //    Host = _emailSettings.PrimaryDomain,
            //    Port = _emailSettings.PrimaryPort,
            //    EnableSsl = true,
            //    Timeout = 10000,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    UseDefaultCredentials = false,
            //    Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword)
            //}.Send(new MailMessage
            //{
            //    From = new MailAddress("no-reply@techo.org", "Miele"),
            //    To = { "luishs@minimalist.mx" },
            //    Subject = subject,
            //    IsBodyHtml = true,
            //    Body = body
            //});
        }

        [HttpPost("notificacion_servicios")]
        public IActionResult notificacion_servicios([FromBody]ModBusPorDosId busqueda)
        {
            //busqueda por Id1 es el id_de la cotizacion, Id2 es el id del usuario que realizó la modificacion de la direccion y montos
            IActionResult response = Unauthorized();
            try
            {
                var cotizacion = _context.Cotizaciones.FirstOrDefault(s => s.Id == busqueda.Id1);
                if (cotizacion == null)
                    return response = Ok(new { result = "Error", detalle = "No existe ningún registro con el Id: " + busqueda.Id1.ToString() });
                else
                {
                    var usuario = _context.Users.FirstOrDefault(c => c.id == busqueda.Id2);
                    if (usuario != null)
                    {
                        //var canal = _context.Cat_canales.FirstOrDefault(c => c.Id == cotizacion.Id_Canal);
                        var usrsFin = _context.Users.Where(us => us.id_rol == 8);
                        var ctaSuc = _context.Cat_Sucursales.Include(s => s.Cuenta).FirstOrDefault(s => s.Id == cotizacion.Id_sucursal);
                        //var admin_canal = _context.Users.FirstOrDefault(us => us.id_canal == usuario.id_canal && us.nivel == "canal");

                        foreach (var user in usrsFin)
                        {
                            StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_Interno.html"));
                            string body = string.Empty;
                            body = reader.ReadToEnd();
                            body = body.Replace("{Titulo}", "Modificación de servicios");
                            body = body.Replace("{Cuenta}", ", cuenta <strong>" + ctaSuc.Cuenta.Cuenta_es + "</strong>");
                            body = body.Replace("{Sucursal}", ", sucursal <strong>" + ctaSuc.Sucursal + "</strong>");
                            body = body.Replace("{Username}", user.name + " " + user.paterno);
                            body = body.Replace("{Texto}", "El usuario <i>" + usuario.name + " " + usuario.paterno + "</i> modificó los servicios de la cotización ");
                            body = body.Replace("{NumeroCotizacion}", cotizacion.Id.ToString());
                            //SendMail(user.email, body, "Cotización modificada # " + cotizacion.Id.ToString());
                        }

                    }

                    return response = Ok(new { result = "Success", detalle = "Correo enviado correctamente" });
                }
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = "detalle del error" + ex.Message });
            }
        }


        [Route("liberar_cotizacion")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult liberar_cotizacion([FromBody]ModIdTexto busqueda)
        {
            //var result = new Models.Response();

            IActionResult response = Unauthorized();

            try
            {
                var cotizacion = _context.Cotizaciones.FirstOrDefault(s => s.Id == busqueda.Id);
                if (cotizacion == null)
                    return response = Ok(new { result = "Error", detalle = "No existe ningún registro con el Id: " + busqueda.Id, id = busqueda.Id });
                else
                {
                    if(cotizacion.Estatus == 4)
                        cotizacion.Estatus = 5;
                    _context.Cotizaciones.Update(cotizacion);
                    integracion_tickets(cotizacion.Id, cotizacion.Id_Cliente, cotizacion.Estatus);
                    _context.SaveChanges();
                    var ctaSuc = _context.Cat_Sucursales.Include(s => s.Cuenta).FirstOrDefault(s => s.Id == cotizacion.Id_sucursal);
                    var vendedor = _context.Users.FirstOrDefault(c => c.id == cotizacion.Id_Vendedor);
                    var admin_canal = _context.Users.FirstOrDefault(us => us.id_canal == vendedor.id_canal && us.nivel == "canal");
                    //correo7 orden de venta liberada
                    if (admin_canal != null)
                    {
                        StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_Interno.html"));
                        string body = string.Empty;
                        body = reader.ReadToEnd();
                        body = body.Replace("{Titulo}", "Orden de venta libearada");
                        body = body.Replace("{Cuenta}", ", cuenta <strong>" + ctaSuc.Cuenta.Cuenta_es + "</strong>");
                        body = body.Replace("{Sucursal}", ", sucursal <strong>" + ctaSuc.Sucursal + "</strong>");
                        body = body.Replace("{Texto}", "Se ha liberado la orden número ");
                        body = body.Replace("{NumeroCotizacion}", cotizacion.Id.ToString());
                        body = body.Replace("{Username}", admin_canal.name + " " + admin_canal.paterno);
                        SendMail(admin_canal.email, body, "Orden de Venta liberada # " + cotizacion.Id.ToString());
                    }

                    var venyFin = _context.Users.Where(us => us.id_rol == 8 || us.id_rol == 9);
                    foreach (var user in venyFin)
                    {
                        StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_Interno.html"));
                        string body = string.Empty;
                        body = reader.ReadToEnd();
                        body = body.Replace("{Titulo}", "Orden de venta libearada");
                        body = body.Replace("{Cuenta}", ", cuenta <strong>" + ctaSuc.Cuenta.Cuenta_es + "</strong>");
                        body = body.Replace("{Sucursal}", ", sucursal <strong>" + ctaSuc.Sucursal + "</strong>");
                        body = body.Replace("{Texto}", "Se ha liberado la orden número ");
                        body = body.Replace("{NumeroCotizacion}", cotizacion.Id.ToString());
                        body = body.Replace("{Username}", user.name + " " + user.paterno);
                        SendMail(user.email, body, "Orden de venta liberada # " + cotizacion.Id.ToString());
                    }

                    var tiene_home = _context.Cotizacion_Producto.Where(C => C.Id_Cotizacion == busqueda.Id && _context.Cat_Productos.FirstOrDefault(P => P.id == C.Id_Producto).id_linea == 38).Count();
                    var tiene_certificado = _context.Cotizacion_Producto.Where(C => C.Id_Cotizacion == busqueda.Id && _context.Cat_Productos.FirstOrDefault(P => P.id == C.Id_Producto).id_linea == 36).Count();

                    string serv_cot = "";

                    serv_cot = serv_cot +  (tiene_home > 0 ? "Home Program," : "");
                    serv_cot = serv_cot + (tiene_certificado > 0 ? " Certificados de mantenimiento," : "");
                    //sBeneficios = sBeneficios.Substring(0, sBeneficios.Length - 3);
                    serv_cot = serv_cot.Substring(0, serv_cot.Length - 1);
                    var usrService = _context.Users.Where(us => us.id_rol == 10007);
                    foreach (var user in usrService)
                    {
                        StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_Interno.html"));
                        string body = string.Empty;
                        body = reader.ReadToEnd();
                        body = body.Replace("{Titulo}", "Orden de venta liberada");
                        body = body.Replace("{Cuenta}", ", cuenta <strong>" + ctaSuc.Cuenta.Cuenta_es + "</strong>");
                        body = body.Replace("{Sucursal}", ", sucursal <strong>" + ctaSuc.Sucursal + "</strong>");
                        body = body.Replace("{Texto}", "Se ha liberado la orden número " + cotizacion.Id.ToString() + " con los servicios ");
                        body = body.Replace("{NumeroCotizacion}", serv_cot);
                        body = body.Replace("{Username}", user.name + " " + user.paterno);
                        SendMail(user.email, body, "Orden de venta liberada # " + cotizacion.Id.ToString());
                    }
                    if (tiene_home >0)
                    {
                        set_notificaciones_partners("Orden de venta liberada con home program", "serviciodetalle/" + cotizacion.Id_Cliente.ToString(), Convert.ToInt32(cotizacion.Id));
                    }
                    return response = Ok(new { result = "Success", detalle = "Registro actualizado con éxito.", id = busqueda.Id });

                }
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message, id = 0 });
            }
        }


        // POST: api/rechazar_cotizacion
        [Route("rechazar_cotizacion")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult rechazar_cotizacion([FromBody]ModIdTexto busqueda)
        {
            //var result = new Models.Response();

            IActionResult response = Unauthorized();

            try
            {
                var cotizacion = _context.Cotizaciones.FirstOrDefault(s => s.Id == busqueda.Id);
                if (cotizacion == null)
                    return response = Ok(new { result = "Error", detalle = "No existe ningún registro con el Id: " + busqueda.Id, id = busqueda.Id });
                else
                {
                    cotizacion.rechazada = cotizacion.rechazada == true ? false : true; ;
                    cotizacion.motivo_rechazo = cotizacion.motivo_rechazo.Length > 0 ? "" : busqueda.texto;
                    if(cotizacion.entrega_sol)
                    {
                        cotizacion.entrega_sol = false;
                        cotizacion.puede_solicitar_env = 1;
                    }
                    _context.Cotizaciones.Update(cotizacion);
                    _context.SaveChanges();
                    // correo8 rechazo o liberacion de cotización
                    var vendedor = _context.Users.FirstOrDefault(c => c.id == cotizacion.Id_Vendedor);
                    if (vendedor != null)
                    {
                        var ctaSuc = _context.Cat_Sucursales.Include(s => s.Cuenta).FirstOrDefault(s => s.Id == cotizacion.Id_sucursal);

                        var admin_canal = _context.Users.FirstOrDefault(us => us.id_canal == vendedor.id_canal && us.nivel == "canal");
                        StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_Interno.html"));
                        string body = string.Empty;
                        string estado = string.Empty;
                        if (cotizacion.rechazada)
                        {
                            body = reader.ReadToEnd();
                            body = body.Replace("{Titulo}", "Orden rechazada");
                            body = body.Replace("{Cuenta}", ", cuenta <strong>" + ctaSuc.Cuenta.Cuenta_es + "</strong>");
                            body = body.Replace("{Sucursal}", ", sucursal <strong>" + ctaSuc.Sucursal + "</strong>");
                            body = body.Replace("{Username}", vendedor.name + " " + vendedor.paterno);
                            body = body.Replace("{Texto}", "Ha sido rechazada la orden con número ");
                            body = body.Replace("Para ver la orden", "Motivo:  " + cotizacion.motivo_rechazo + "<br>Para ver la orden");
                            body = body.Replace("HAZ CLICK AQUI", " ");
                            body = body.Replace("{NumeroCotizacion}", cotizacion.Id.ToString());
                            SendMail(vendedor.email, body, "Se rechazó la orden # " + cotizacion.Id.ToString());
                            if (admin_canal != null)
                            {
                                body = body.Replace(vendedor.name + " " + vendedor.paterno, admin_canal.name + " " + admin_canal.paterno);
                                SendMail(admin_canal.email, body, "Se rechazó la orden # " + cotizacion.Id.ToString());
                            }

                        }
                    }
                    return response = Ok(new { result = "Success", detalle = "Registro actualizado con éxito.", id = busqueda.Id });

                }
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message, id = 0 });
            }
        }

        // POST: api/solicitar_entrega
        [Route("solicitar_entrega")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult solicitar_entrega([FromBody]ModBusPorId busqueda)
        {
            //var result = new Models.Response();

            IActionResult response = Unauthorized();

            try
            {
                var cotizacion = _context.Cotizaciones.FirstOrDefault(s => s.Id == busqueda.Id);
                if (cotizacion == null)
                    return response = Ok(new { result = "Error", detalle = "No existe ningún registro con el Id: " + busqueda.Id, id = busqueda.Id });
                else
                {
                    cotizacion.entrega_sol = true;
                    cotizacion.acepto_terminos_condiciones = true;
                    cotizacion.cambio_ord_comp_generada = DateTime.Now;
                    cotizacion.Estatus = cotizacion.Estatus < 3 ? 3 : cotizacion.Estatus;
                    cotizacion.puede_solicitar_env = 0;
                    _context.Cotizaciones.Update(cotizacion);
                    integracion_tickets(cotizacion.Id, cotizacion.Id_Cliente, cotizacion.Estatus);
                    _context.SaveChanges();


                    //correo5 solicitud de envío/instalacion de productos
                    //long id_cte = _context.Cotizaciones.FirstOrDefault(co => co.Id == busqueda.Id).Id_Cliente;
                    var dir_envio = _context.direcciones_cotizacion.FirstOrDefault(cd => cd.tipo_direccion == 2 && cd.id_cotizacion == busqueda.Id);
                    var dir_instalacion = _context.direcciones_cotizacion.FirstOrDefault(cd => cd.tipo_direccion == 1 && cd.id_cotizacion == busqueda.Id);

                    string fec_envio = "";
                    string fec_instalacion = "";
                    if (dir_envio == null) fec_envio = "No Asignada";
                    else fec_envio = dir_envio.Fecha_Estimada;
                    if (dir_instalacion == null) fec_instalacion = "No Asignada";
                    else fec_instalacion = dir_instalacion.Fecha_Estimada;
                    //Se cambia a solo finanzas, rol 8
                    //var venyFin = _context.Users.Where(us => us.id_rol == 8 || us.id_rol == 9);
                    var venyFin = _context.Users.Where(us => us.id_rol == 8);
                    foreach (var user in venyFin)
                    {
                        var ctaSuc = _context.Cat_Sucursales.Include(s => s.Cuenta).FirstOrDefault(s => s.Id == cotizacion.Id_sucursal);

                        StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_Interno.html"));
                        string body = string.Empty;
                        body = reader.ReadToEnd();
                        body = body.Replace("{Titulo}", "Solicitud de entrega");
                        body = body.Replace("{Cuenta}", "");
                        body = body.Replace("{Sucursal}", "");
                        body = body.Replace("{Username}", user.name + " " + user.paterno);
                        body = body.Replace("{Texto}", "Se ha generado una solicitud de entrega de equipos con número de orden " +
                            "<strong style='color: #8c0014;'>" + cotizacion.Id.ToString() + "</strong>" +
                            ", cuenta <strong>" + ctaSuc.Cuenta.Cuenta_es + "</strong>" + ", sucursal <strong>" + ctaSuc.Sucursal + "</strong></p>" +
                            "<p>Fecha de envío: <strong style='color: #8c0014;'>" + fec_envio + "</strong></p>" +
                            "<p>Fecha de instalación: ");
                        body = body.Replace("{NumeroCotizacion}", fec_instalacion);
                        SendMail(user.email, body, "Solicitud de entrega # " + cotizacion.Id.ToString());
                    }

                    List<Direccion_Cotizacion> direcciones = new List<Direccion_Cotizacion>();
                    //direcciones.Add(dir_envio);
                    //direcciones.Add(dir_instalacion);
                    //obj_dir_cliente _obj_dir_cliente = new obj_dir_cliente();
                    //_obj_dir_cliente.direcciones = direcciones;
                    //_obj_dir_cliente.Id_cliente = Convert.ToInt32(cotizacion.Id_Cliente);
                    //_obj_dir_cliente.dir_clie = false;
                    //_obj_dir_cliente.cat_dir = true;
                    //set_cat_direcciones(_obj_dir_cliente);

                    return response = Ok(new { result = "Success", detalle = "entrega solicitada con éxito.", id = busqueda.Id });
                }
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message, id = 0 });
            }
        }

        // POST: api/Servicios/Actualizar_estatus
        [Route("crear_editar_fiscales")]
        [HttpPost("{crear_editar_fiscales}")]
        public IActionResult crear_editar_fiscales([FromBody] DatosFiscales item)
        {
            //var result = new Models.Response();

            IActionResult response = Unauthorized();

            try
            {
                if (item.id == 0) // Crea Registro 
                {
                    _context.DatosFiscales.Add(item);
                    _context.SaveChanges();
                    return response = Ok(new { result = "Success", detalle = "Registro Creado con éxito.", id = item.id });
                }
                else // Edita Registo 
                {
                    var fiscales = _context.DatosFiscales.FirstOrDefault(s => s.id == item.id);
                    if (fiscales == null)
                    {
                        return BadRequest();
                    }
                    else
                    {

                        fiscales.id_cliente = item.id_cliente;
                        fiscales.nombre_fact = item.nombre_fact;
                        fiscales.razon_social = item.razon_social;
                        fiscales.rfc = item.rfc;
                        fiscales.email = item.email;
                        fiscales.calle_numero = item.calle_numero;
                        fiscales.cp = item.cp;
                        fiscales.id_estado = item.id_estado;
                        fiscales.id_municipio = item.id_municipio;
                        fiscales.colonia = item.colonia;
                        fiscales.Ext_fact = item.Ext_fact;
                        fiscales.Int_fact = item.Int_fact;
                        fiscales.telefono_fact = item.telefono_fact;
                        fiscales.id_cliente = item.id_cliente;
                        fiscales.id_prefijo_calle = item.id_prefijo_calle;
                        _context.DatosFiscales.Update(fiscales);
                        _context.SaveChanges();
                        return response = Ok(new { result = "Success", detalle = "Registro actualizado con éxito.", id = fiscales.id });
                    }
                }
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message, id = 0 });
            }
        }

        // POST: api/Servicios/Actualizar_estatus
        [Route("crear_editar_cotizacion")]
        [HttpPost("{crear_editar_cotizacion}")]
        public IActionResult crear_editar_cotizacion([FromBody] Cotizaciones item) //se guarda al editar el detalle 
        {
            //var result = new Models.Response();
            IActionResult response = Unauthorized();
            try
            {
                if (item.Id == 0) // Crea Registro 
                {
                    _context.Cotizaciones.Add(item);
                    _context.SaveChanges();
                    return response = Ok(new { result = "Success", detalle = "Registro Creado con éxito.", id = item.Id });
                }
                else // Edita Registo 
                {
                    var cotizacion = _context.Cotizaciones.FirstOrDefault(s => s.Id == item.Id);
                    if (cotizacion == null)
                    {
                        return BadRequest();
                    }
                    else
                    {
                        cotizacion.ibs = item.ibs;
                        cotizacion.Id_Canal = item.Id_Canal;
                        cotizacion.Id_Cliente = item.Id_Cliente;
                        cotizacion.Id_Cotizacion_Producto = item.Id_Cotizacion_Producto;
                        cotizacion.Id_Cuenta = item.Id_Cuenta;
                        cotizacion.Id_Estado_Instalacion = item.Id_Estado_Instalacion;
                        cotizacion.id_formapago = item.id_formapago;
                        cotizacion.id_tarjeta = item.id_tarjeta;
                        cotizacion.Id_Vendedor = item.Id_Vendedor;
                        cotizacion.Estatus = item.Estatus;
                        cotizacion.Numero = item.Numero;
                        cotizacion.Observaciones = item.Observaciones;
                        cotizacion.rechazada = item.rechazada;
                        cotizacion.motivo_rechazo = item.rechazada == true ? item.motivo_rechazo : "";
                        cotizacion.requiere_fact = item.requiere_fact;
                        cotizacion.acepto_terminos_condiciones = item.acepto_terminos_condiciones;
                        if (cotizacion.Estatus == 3)
                            cotizacion.cambio_ord_comp_generada = DateTime.Now;
                        cotizacion.cambio_ord_comp_generada = DateTime.Now;
                        cotizacion.Acciones = item.Acciones;
                        _context.Cotizaciones.Update(cotizacion);
                       // _context.SaveChanges();

                        if (cotizacion.Estatus > 2)
                        {
                            integracion_tickets(item.Id, item.Id_Cliente, item.Estatus);
                        }

                        _context.SaveChanges();
                        //correo4 cambio a orden de compra generada
                        if (cotizacion.Estatus == 3)
                        {
                            var venyFin = _context.Users.Where(us => us.id_rol == 8 || us.id_rol == 9);
                            foreach (var user in venyFin)
                            {
                                var ctaSuc = _context.Cat_Sucursales.Include(s => s.Cuenta).FirstOrDefault(s => s.Id == cotizacion.Id_sucursal);
                                StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_Interno.html"));
                                string body = string.Empty;
                                body = reader.ReadToEnd();
                                body = body.Replace("{Titulo}", "Orden de compra generada");
                                body = body.Replace("{Cuenta}", ", cuenta <strong>" + ctaSuc.Cuenta.Cuenta_es + "</strong>");
                                body = body.Replace("{Sucursal}", ", sucursal <strong>" + ctaSuc.Sucursal + "</strong>");
                                body = body.Replace("{Texto}", "Se cambió el estado correctamente a la orden de compra generada número ");
                                body = body.Replace("{NumeroCotizacion}", item.Id.ToString());
                                body = body.Replace("{Username}", user.name + " " + user.paterno);
                                SendMail(user.email, body, "Orden de compra generada # " + item.Id.ToString());
                            }                            
                       }

                       

                        return response = Ok(new { result = "Success", detalle = "Registro actualizado con éxito.", id = cotizacion.Id });
                    }
                }

            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message, id = 0 });
            }
        }

        public void integracion_tickets(long id_cotizacion, long id_cliente, int cot_estatus)
        {

           
            if (cot_estatus == 3)
            {
                var productos = _context.Cotizacion_Producto.Where(p => p.Id_Cotizacion == id_cotizacion);
                foreach (var cot_p in productos) 
                {

                    var tipo_ = _context.Cat_Productos.FirstOrDefault(i => i.id == cot_p.Id_Producto).tipo;

                    for (int i = 0; i < cot_p.cantidad ; i++)
                    {
                        Cliente_Productos clie_p = new Cliente_Productos();
                        if (tipo_ == 1 || tipo_ == 4 || tipo_ == 2) //solo productos y no isntalables
                        {
                            clie_p.Id_EsatusCompra = 1009; // orden de compra liberada
                            clie_p.FechaCompra = DateTime.Now;
                            clie_p.Id_Cliente = id_cliente;
                            clie_p.Id_Producto = cot_p.Id_Producto;
                            clie_p.creado = DateTime.Now;
                            clie_p.actualizado = DateTime.Now;
                            clie_p.creadopor = 0;
                            clie_p.actualizadopor = 0;
                            clie_p.id_cotizacion = Convert.ToInt32(id_cotizacion);
                            _context.Cliente_Productos.Add(clie_p);
                        }
                        var cliente = _context.Clientes.FirstOrDefault(c => c.id == id_cliente);
                        cliente.tipo_cliente = 1;
                        _context.Clientes.Update(cliente);
                    }
                }
            }

            if (cot_estatus == 4)
            {   
                var p = _context.Cliente_Productos.Where(pp => pp.id_cotizacion == id_cotizacion).ToList();
                foreach (var cli_p in p)
                {
                    cli_p.Id_EsatusCompra = 1010; // orden de compra pendiente
                    _context.Cliente_Productos.Update(cli_p);
                }   
            }
            
            if (cot_estatus == 5)
            {
                var p = _context.Cliente_Productos.Where(pp => pp.id_cotizacion == id_cotizacion).ToList();
                foreach (var cli_p in p)
                {
                    cli_p.Id_EsatusCompra = 1012;// orden de compra pendiente
                    _context.Cliente_Productos.Update(cli_p);
                }
                //Aqui va la nitificacion
                var _cot = _context.Cotizaciones.FirstOrDefault(f => f.Id == id_cotizacion);
                var cliente_ = _context.Clientes.FirstOrDefault(f => f.id == _cot.Id_Cliente);


                var cert = _context.Cer_producto_cliente.Where(i => i.id_cliente == cliente_.id).ToList();
                var cert_p = _context.rel_certificado_producto.Where(i => cert.Any(k => k.id == i.id_certificado)).ToList();
                var hp = _context.home_producto_cliente.Where(i => i.id_cotizacion == id_cotizacion).ToList();

                foreach (var x in cert)
                { x.estatus_venta = true;  _context.Cer_producto_cliente.Update(x); }

                foreach (var x in cert_p)
                { x.estatus_activo = true;  _context.rel_certificado_producto.Update(x); }

                foreach (var x in hp)
                { x.estatus_activo = true;  x.estatus_venta = true; _context.home_producto_cliente.Update(x); }

                set_notificaciones_partners("Orden de venta liberada", "serviciodetalle/" + cliente_.id.ToString(), Convert.ToInt32(id_cotizacion));

                // _context.SaveChanges();
            }
        }

        public void set_notificaciones_partners(string titulo, string url, int id_cotizacion)
        {
            Notificaciones Notificaciones = new Notificaciones();
            var fecha = _context.direcciones_cotizacion.FirstOrDefault(f => f.id_cotizacion == id_cotizacion && f.tipo_direccion == 1);
            string fecha_ins = DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year;
            if (fecha != null)
                fecha_ins = fecha.Fecha_Estimada;

            var _cot = _context.Cotizaciones.FirstOrDefault(f => f.Id == id_cotizacion);
            var cliente_ = _context.Clientes.FirstOrDefault(f => f.id == _cot.Id_Cliente);
            string nombre = cliente_.nombre + " " + cliente_.paterno;
            if(titulo == "Orden de venta liberada")
                 Notificaciones.descripcion = "Ahora puedes iniciar el servicio con Orden de venta # " + id_cotizacion.ToString() + " | Fecha de Entrega: " + fecha_ins + " | Cliente: " + nombre;
            if(titulo == "Entrega solicitada por el cliente")
                Notificaciones.descripcion = "El cliente a completado su pago y ha solicitado la entrega de los  productos de la orden #" + id_cotizacion.ToString() + " | Fecha de Entrega: " + fecha_ins + " | Cliente: " + nombre;
            if (titulo == "Servicio de Home Program liberado")
                Notificaciones.descripcion = "El servicio de home program esta listo para agendarse, orden #" + id_cotizacion.ToString() + " | Fecha de Entrega: " + fecha_ins + " | Cliente: " + nombre;
            //if para home program
            //   Notificaciones.evento = "Orden de venta liberada";
            Notificaciones.evento = titulo;
            Notificaciones.rol_notificado = 10008;
            Notificaciones.estatus_leido = false;
            Notificaciones.creado = DateTime.Now;
            Notificaciones.creadopor = 0;
            Notificaciones.url = url;
            // Notificaciones.url = "serviciodetalle/" + cliente_.id.ToString();
            _context.Notificaciones.Add(Notificaciones);
            _context.SaveChanges();
        }

        // POST: api/Servicios/Actualizar_estatus
        [Route("crear_editar_ckiente")]
        [HttpPost("{crear_editar_ckiente}")]
        public IActionResult crear_editar_ckiente([FromBody] Clientes item)
        {
            //var result = new Models.Response();
            IActionResult response = Unauthorized();
            try
            {
                if (item.id == 0) // Crea Registro 
                {
                    _context.Clientes.Add(item);
                    _context.SaveChanges();
                    return response = Ok(new { result = "Success", detalle = "Registro Creado con éxito.", id = item.id });
                }
                else // Edita Registo 
                {
                    var cliente = _context.Clientes.FirstOrDefault(s => s.id == item.id);
                    if (cliente == null)
                    {
                        return BadRequest();
                    }
                    else
                    {
                        cliente.materno = item.materno;
                        cliente.nombre = item.nombre;
                        cliente.nombre_comercial = item.nombre_comercial;
                        cliente.paterno = item.paterno;
                        cliente.referidopor = item.referidopor;
                        cliente.telefono_movil = item.telefono_movil;
                        cliente.telefono = item.telefono_movil;
                        cliente.vigencia_ref = item.vigencia_ref;
                        cliente.actualizadopor = item.actualizadopor;
                        cliente.actualizado = DateTime.Now;
                        cliente.email = item.email;
                        _context.Clientes.Update(cliente);
                        _context.SaveChanges();
                        return response = Ok(new { result = "Success", detalle = "Registro actualizado con éxito.", id = cliente.id });
                    }
                }

            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message, id = 0 });
            }
        }


        // POST: api/Partners_Cotizacion
        [Route("get_entidades_busqueda")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult get_entidades_busqueda([FromBody]busqueda_entidades busqueda)
        {

            IActionResult response = Unauthorized();

            var sucursales = from c in _context.Cat_Sucursales
                            join u in _context.Cat_Cuentas on c.Id_Cuenta equals u.Id
                            join a in _context.Cat_canales on u.Id_Canal equals a.Id
                             where EF.Functions.Like(c.Sucursal, "%" + busqueda.TextoLibre + "%")
                             select new
                            {
                                id = (Int32)c.Id,
                                entidad = c.Sucursal,
                                tipo = "sucursal",
                                cuenta = u.Cuenta_es,
                                canal = a.Canal_es
                            };

              var   cuentas = from c in _context.Cat_Cuentas
                             join a in _context.Cat_canales on c.Id_Canal equals a.Id
                              where EF.Functions.Like(c.Cuenta_es, "%" + busqueda.TextoLibre + "%")
                              select new
                             {
                                 id = (Int32)c.Id,
                                 entidad = c.Cuenta_es,
                                 tipo = "Cuenta",
                                 cuenta = "",
                                 canal = a.Canal_es
                             };
            
                  
                var canales  = from c in _context.Cat_canales
                               where EF.Functions.Like(c.Canal_es, "%" + busqueda.TextoLibre + "%")
                               select new
                            {
                                id = (Int32)c.Id,
                                entidad = c.Canal_es,
                                tipo = "Canal",
                                cuenta = "",
                                canal = ""
                            };

            var entidades = sucursales;
            if (busqueda.tipo_entidad == 2)
                entidades = cuentas;
            if (busqueda.tipo_entidad == 1)
                entidades = canales;
            if (busqueda.tipo_entidad == 0)
            {
                entidades = entidades.Union(cuentas);
                entidades = entidades.Union(canales);
            }

                if (entidades == null)
            {
                return response = Ok(new { result = "Error", item = entidades });
            }
            return response = Ok(new { result = "Success", item = entidades });
        }

        // POST: api/Partners_Cotizacion
        /// //////////////////////  PROMOCIONES //////////////////////////

        // POST: api/Partners_Productos
        [Route("get_entidades_promo")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult get_entidades_promo([FromBody]busquedaLibre texto)
        {

            IActionResult response = Unauthorized();
            var grp_canal = from c in _context.Cat_canales
                            select new
                            {
                                id = c.Id,
                                descripcion = c.Canal_es + " | Canal",
                                tipo = "Canal",
                                id_tipo = 1
                            };

            var grp_cuenta = from c in _context.Cat_Cuentas
                             select new
                             {
                                 id = c.Id,
                                 descripcion = c.Cuenta_es + " | Cuenta",
                                 tipo = "Cuenta",
                                 id_tipo = 2
                             };
            var grp_sucrusales = from c in _context.Cat_Sucursales
                                 select new
                                 {
                                     id = (long)c.Id,
                                     descripcion = c.Sucursal + " | Sucursal",
                                     tipo = "Sucursal",
                                     id_tipo = 3
                                 };

            grp_cuenta = grp_cuenta.Union(grp_sucrusales);
            grp_canal = grp_canal.Union(grp_cuenta);
            if (grp_canal == null)
            {
                return response = Ok(new { result = "Error", item = grp_canal });
            }
            return response = Ok(new { result = "Success", item = grp_canal });
        }

        [Route("get_menu")]
        [HttpGet]
        public IActionResult get_menu_productos()
        {
            IActionResult response = Unauthorized();
            try
            {
                //var menu = _context.cat_SuperLineas.Include(ss => ss.cat_Linea_Productos).ThenInclude(ln => ln.cat_sublinea_producto).ThenInclude(sl => sl.productos);
                var menu = _context.cat_SuperLineas.Include(ss => ss.cat_Linea_Productos).ThenInclude(ln => ln.cat_sublinea_producto);

                if (menu == null)
                {
                    return response = Ok(new { result = "Error", item = menu });
                }

                var s = (from a in menu
                         select new
                         {
                             idNivel = a.id,
                             displayName = a.descripcion,
                             children = (from b in a.cat_Linea_Productos
                                             //  where _context.Cat_SubLinea_Producto.First(x => x.id_linea_producto == b.id).id > 0
                                         where b.id != 34 && b.estatus == true
                                         orderby b.descripcion ascending
                                         select new
                                         {
                                             idNivel = b.id,
                                             displayName = b.descripcion,
                                             children = (from c in b.cat_sublinea_producto
                                                         where c.id == (a.id == 5 ? 0 : c.id) && c.estatus == true
                                                       orderby c.descripcion ascending
                                                       select new
                                                     {
                                                         idNivel = c.id,
                                                         displayName = c.descripcion,
                                                         //children = (from d in c.productos
                                                         //            where d.visible_partners == true
                                                         //          select new
                                                         //          {
                                                         //              idNivel = d.id,
                                                         //              displayName = d.modelo + " | " + d.nombre,
                                                         //          }).ToList()
                                                     }).ToList()
                                       }).ToList()
                         }).ToList();

                foreach (var super in s)
                {
                    foreach (var linea in super.children)
                    {
                        var id_l_ = _context.Cat_Linea_Producto.FirstOrDefault(l_ => l_.descripcion == linea.displayName).id;
                        List<String> subs_out = new List<String> ();
                        foreach (var sub in linea.children)
                        {
                            var id_s_ = _context.Cat_SubLinea_Producto.FirstOrDefault(s_ => s_.descripcion == sub.displayName && s_.id_linea_producto == id_l_).id;
                            int n = _context.Cat_Productos.Where(p => p.id_sublinea == id_s_ && p.visible_partners == true).Count();
                            if (n == 0)
                                subs_out.Add(sub.displayName);
                                //linea.children.RemoveAll(a => a.displayName == sub.displayName);
                        }
                        linea.children.RemoveAll(p => subs_out.Any(e => e == p.displayName));
                    }
                }

                
                return response = Ok(new { result = "Success", item = s });

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        // POST: api/Partners_Productos
        [Route("get_cat_productos_promo")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult get_cat_productos_promo([FromBody]busquedaLibre texto)
        {

            IActionResult response = Unauthorized();
            var grplinea = from c in _context.Cat_Linea_Producto
                           select new
                           {
                               id = c.id,
                               descripcion = c.descripcion + " | Línea",
                               tipo = "Linea",
                               id_tipo = 1
                           };

            var grp_sublinea = from c in _context.Cat_SubLinea_Producto
                               select new
                               {
                                   id = c.id,
                                   descripcion = c.descripcion + " | Sublínea",
                                   tipo = "Sublínea",
                                   id_tipo = 2
                               };
            var grp_producto = from c in _context.Cat_Productos
                               select new
                               {
                                   id = c.id,
                                   descripcion = c.nombre + " | Producto",
                                   tipo = "Producto",
                                   id_tipo = 3
                               };

            grp_sublinea = grp_sublinea.Union(grp_producto);
            grplinea = grplinea.Union(grp_sublinea);
            if (grplinea == null)
            {
                return response = Ok(new { result = "Error", item = grplinea });
            }
            return response = Ok(new { result = "Success", item = grplinea });
        }


        // POST: api/Partners_Productos
        [Route("get_catalogos_conf_com")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult get_catalogos_conf_com([FromBody]busquedaLibre texto)
        {

            IActionResult response = Unauthorized();

            var cat_tipos_herencia = from c in _context.cat_tipos_herencia
                                     select new
                                     {
                                         id = c.id,
                                         tipo = c.tipo
                                     };

            var cat_tipo_condicion = from c in _context.cat_tipo_condicion
                                     select new
                                     {
                                         id = c.id,
                                         descripcion = c.tipo_condicion
                                     };

            var cat_msi = from c in _context.cat_msi
                          select new
                          {
                              id = c.id,
                              descripcion = c.desc_msi
                          };

            ///////////////////////////// cat productos ////////////////////////
            var allProductos = from c in _context.Cat_Linea_Producto
                               select new
                               {
                                   id = c.id,
                                   descripcion = c.descripcion + " | Línea",
                                   tipo = "Linea",
                                   id_tipo = 1
                               };

            var grp_sublinea = from c in _context.Cat_SubLinea_Producto
                               select new
                               {
                                   id = c.id,
                                   descripcion = c.descripcion + " - Sublínea | " + _context.Cat_Linea_Producto.FirstOrDefault(s => s.id == c.id_linea_producto).descripcion + " - Linea",
                                   tipo = "Sublínea",
                                   id_tipo = 2
                               };
            var grp_producto = from c in _context.Cat_Productos
                               where c.descripcion_corta.Length > 1
                               select new
                               {
                                   id = c.id,
                                   descripcion = c.nombre + " |  " + c.modelo,
                                   tipo = "Producto",
                                   id_tipo = 3
                               };

            grp_sublinea = grp_sublinea.Union(grp_producto);
            allProductos = allProductos.Union(grp_sublinea);
            ////////////////////////////  cat productos ////////////////////////
            /////////////////////////// entidades promo ////////////////////////
            var grp_canal = from c in _context.Cat_canales
                            select new
                            {
                                id = c.Id,
                                descripcion = c.Canal_es + " | Canal",
                                tipo = "Canal",
                                id_tipo = 1
                            };

            var grp_cuenta = from c in _context.Cat_Cuentas
                             select new
                             {
                                 id = c.Id,
                                 descripcion = c.Cuenta_es + " | Cuenta",
                                 tipo = "Cuenta",
                                 id_tipo = 2
                             };
            var grp_sucrusales = from c in _context.Cat_Sucursales
                                 select new
                                 {
                                     id = (long)c.Id,
                                     descripcion = c.Sucursal + " | Sucursal",
                                     tipo = "Sucursal",
                                     id_tipo = 3
                                 };

            grp_cuenta = grp_cuenta.Union(grp_sucrusales);
            grp_canal = grp_canal.Union(grp_cuenta);
            /////////////////////////// solo productos promo ////////////////////////

            var justProductos = from c in _context.Cat_Productos
                                select new
                                {
                                    id = c.id,
                                    descripcion = c.nombre + " |  " + c.modelo,
                                    tipo = "Producto",
                                    id_tipo = 3
                                };
            /////////////////////////// solo las sublineas ////////////////////////

            var allsublineas = from c in _context.Cat_SubLinea_Producto
                               select new
                               {
                                   id = c.id,
                                   id_linea = c.id_linea_producto,
                                   descripcion = c.descripcion
                               };
            /////////////////////////// solo las sucursales ////////////////////////

            var allsucursales = from c in _context.Cat_Sucursales
                                join d in _context.Cat_Cuentas on c.Id_Cuenta equals d.Id
                                select new
                                {
                                    id = c.Id,
                                    id_cuenta = c.Id_Cuenta,
                                    sucursal = c.Sucursal,
                                    id_canal = d.Id_Canal
                                };
            /////////////////////////// sucursales y su CC ////////////////////////

            var allsucursales_cc = from cc in _context.condiones_comerciales_sucursal
                                   join c in _context.Cat_Sucursales on cc.id_Cat_Sucursales equals c.Id
                                   select new
                                   {
                                       id_sucursal = c.Id,
                                       id_cuenta = c.Id_Cuenta,
                                       sucursal = c.Sucursal,
                                       id_canal = c.Id_Cuenta,
                                       margen = cc.margen,
                                       id_cc = cc.id
                                   };

            /////////////////////////// all promociones ////////////////////////

            var all_promociones = from p in _context.promocion
                                      // where Convert.ToDateTime(p.fecha_hora_fin) >= DateTime.Now
                                  select new
                                  {
                                      id = p.id,
                                      nombre = p.nombre,
                                      fecha_hora_inicio = p.fecha_hora_inicio,
                                      fecha_hora_fin = p.fecha_hora_fin,
                                      compatible = false
                                  };

            var catalogos = new List<IQueryable>()
            {
                cat_tipos_herencia,//0
                cat_tipo_condicion,//1
                cat_msi,//2
                allProductos,//3
                justProductos,//4
                grp_canal,//5
                grp_canal,//6
                allsublineas,//7
                allsucursales,//8
                all_promociones//9
            };

            if (catalogos == null)
            {
                return response = Ok(new { result = "Error", item = catalogos });
            }
            return response = Ok(new { result = "Success", item = catalogos });
        }


        // POST: api/Partners_Productos
        [Route("get_catalogos_promocion")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult get_catalogos_promocion([FromBody]busquedaLibre texto)
        {

            IActionResult response = Unauthorized();

            var cat_tipos_herencia = from c in _context.cat_tipos_herencia
                                     select new
                                     {
                                         id = c.id,
                                         tipo = c.tipo
                                     };

            var cat_tipo_condicion = from c in _context.cat_tipo_condicion
                                     select new
                                     {
                                         id = c.id,
                                         descripcion = c.tipo_condicion
                                     };

            var cat_msi = from c in _context.cat_msi
                          select new
                          {
                              id = c.id,
                              descripcion = c.desc_msi
                          };

            ///////////////////////////// cat productos ////////////////////////
            var allProductos = from c in _context.Cat_Linea_Producto
                               select new
                               {
                                   id = c.id,
                                   descripcion = c.descripcion + " | Línea",
                                   tipo = "Linea",
                                   id_tipo = 1
                               };

            var grp_sublinea = from c in _context.Cat_SubLinea_Producto
                               select new
                               {
                                   id = c.id,
                                   descripcion = c.descripcion + " - Sublínea | " + _context.Cat_Linea_Producto.FirstOrDefault(s => s.id == c.id_linea_producto).descripcion + " - Linea",
                                   tipo = "Sublínea",
                                   id_tipo = 2
                               };
            var grp_producto = from c in _context.Cat_Productos
                               where c.descripcion_corta.Length > 1 && c.visible_partners == true
                               select new
                               {
                                   id = c.id,
                                   descripcion = c.nombre + " |  " + (c.modelo == null ? "Sin Modelo" : c.modelo) ,
                                   tipo = "Producto",
                                   id_tipo = 3
                               };

            grp_sublinea = grp_sublinea.Union(grp_producto);
            allProductos = allProductos.Union(grp_sublinea);
            ////////////////////////////  cat productos ////////////////////////
            /////////////////////////// entidades promo ////////////////////////
            var grp_canal = from c in _context.Cat_canales
                            select new
                            {
                                id = c.Id,
                                descripcion = c.Canal_es + " | Canal",
                                tipo = "Canal",
                                id_tipo = 1
                            };

            var grp_cuenta = from c in _context.Cat_Cuentas
                             select new
                             {
                                 id = c.Id,
                                 descripcion = c.Cuenta_es + " | Cuenta",
                                 tipo = "Cuenta",
                                 id_tipo = 2
                             };
            var grp_sucrusales = from c in _context.Cat_Sucursales
                                 select new
                                 {
                                     id = (long)c.Id,
                                     descripcion = c.Sucursal + " | Sucursal",
                                     tipo = "Sucursal",
                                     id_tipo = 3
                                 };

            grp_cuenta = grp_cuenta.Union(grp_sucrusales);
            grp_canal = grp_canal.Union(grp_cuenta);
            /////////////////////////// solo productos promo ////////////////////////

            var justProductos = from c in _context.Cat_Productos
                                where c.visible_partners == true
                                select new
                                {
                                    id = c.id,
                                    descripcion = c.nombre + " |  " + (c.modelo ==null? "Sin Modelo" : c.modelo) + " | " + (c.sku == null ? "Sin SKU" : c.sku),
                                    tipo = "Producto",
                                    id_tipo = 3
                                };
            /////////////////////////// solo las sublineas ////////////////////////

            var allsublineas = from c in _context.Cat_SubLinea_Producto
                               select new
                               {
                                   id = c.id,
                                   id_linea = c.id_linea_producto,
                                   descripcion = c.descripcion
                               };
            /////////////////////////// solo las sucursales ////////////////////////

            var allsucursales = from c in _context.Cat_Sucursales
                                join d in _context.Cat_Cuentas on c.Id_Cuenta equals d.Id
                                select new
                                {
                                    id = c.Id,
                                    id_cuenta = c.Id_Cuenta,
                                    sucursal = c.Sucursal,
                                    id_canal = d.Id_Canal
                                };
            /////////////////////////// sucursales y su CC ////////////////////////

            var allsucursales_cc = from cc in _context.condiones_comerciales_sucursal
                                   join c in _context.Cat_Sucursales on cc.id_Cat_Sucursales equals c.Id
                                   select new
                                   {
                                       id_sucursal = c.Id,
                                       id_cuenta = c.Id_Cuenta,
                                       sucursal = c.Sucursal,
                                       id_canal = c.Id_Cuenta,
                                       margen = cc.margen,
                                       id_cc = cc.id
                                   };

            /////////////////////////// all promociones ////////////////////////

            var all_promociones = from p in _context.promocion
                                      // where Convert.ToDateTime(p.fecha_hora_fin) >= DateTime.Now
                                  select new
                                  {
                                      id = p.id,
                                      nombre = p.nombre,
                                      fecha_hora_inicio = p.fecha_hora_inicio,
                                      fecha_hora_fin = p.fecha_hora_fin,
                                      compatible = false
                                  };

            var catalogos = new List<IQueryable>()
            {
                cat_tipos_herencia,//0
                cat_tipo_condicion,//1
                cat_msi,//2
                allProductos,//3
                justProductos,//4
                grp_canal,//5
                grp_canal,//6
                allsublineas,//7
                allsucursales,//8
                all_promociones//9
            };

            if (catalogos == null)
            {
                return response = Ok(new { result = "Error", item = catalogos });
            }
            return response = Ok(new { result = "Success", item = catalogos });
        }

        // POST: api/Servicios/Actualizar_estatus
        [Route("crear_editar_promocion")]
        [HttpPost("{crear_editar_promocion}")]
        public IActionResult crear_editar_promocion([FromBody] promocion item)
        {
            //var result = new Models.Response();
            if (item.fecha_hora_fin.IndexOf('N') >= 0)
                item.fecha_hora_fin = item.fecha_hora_inicio;
            IActionResult response = Unauthorized();

            try
            {
                if (item.id == 0) // Crea Registro 
                {
                    _context.promocion.Add(item);
                    _context.SaveChanges();
                    return response = Ok(new { result = "Success", detalle = "Promocion creada con éxito.", id = item.id });
                }
                else // Edita Registo 
                {
                    var promocion = _context.promocion.FirstOrDefault(s => s.id == item.id);
                    if (promocion == null)
                        return response = Ok(new { result = "Error", detalle = "No existe ningún registro con el Id: " + item.id, id = item.id });
                    else
                    {
                        promocion.id = item.id;
                        promocion.nombre = item.nombre;
                        promocion.fecha_hora_inicio = item.fecha_hora_inicio;
                        promocion.fecha_hora_fin = item.fecha_hora_fin;
                        promocion.vigencia_indefinida = item.vigencia_indefinida;
                        promocion.id_tipos_herencia_promo = item.id_tipos_herencia_promo;
                        promocion.id_cat_tipo_condicion = item.id_cat_tipo_condicion;
                        promocion.monto_inferior_condicion = item.monto_inferior_condicion;
                        promocion.aplica_cc = item.aplica_cc;
                        promocion.monto_condicion = item.monto_condicion;
                        promocion.incluir_desc_adic = item.incluir_desc_adic;
                        promocion.beneficio_obligatorio = item.beneficio_obligatorio;
                        promocion.id_tipo_beneficio = item.id_tipo_beneficio;

                        var _entidades_participantes = _context.entidades_participantes.Where(s => s.id_promocion == item.id);
                        foreach (entidades_participantes entidad in _entidades_participantes)
                            _context.entidades_participantes.Remove(entidad);
                        foreach (entidades_participantes entidad in item.entidades_participantes)
                            entidad.id = 0;
                        promocion.entidades_participantes = item.entidades_participantes;

                        var _entidades_excluidas = _context.entidades_excluidas.Where(s => s.id_promocion == item.id);
                        foreach (entidades_excluidas entidad in _entidades_excluidas)
                            _context.entidades_excluidas.Remove(entidad);
                        foreach (entidades_excluidas entidad in item.entidades_excluidas)
                            entidad.id = 0;
                        promocion.entidades_excluidas = item.entidades_excluidas;

                        var _productos_condicion = _context.productos_condicion.Where(s => s.id_promocion == item.id);
                        foreach (productos_condicion entidad in _productos_condicion)
                            _context.productos_condicion.Remove(entidad);
                        foreach (productos_condicion entidad in item.productos_condicion)
                            entidad.id = 0;
                        promocion.productos_condicion = item.productos_condicion;

                        var _productos_excluidos = _context.productos_excluidos.Where(s => s.id_promocion == item.id);
                        foreach (productos_excluidos entidad in _productos_excluidos)
                            _context.productos_excluidos.Remove(entidad);
                        foreach (productos_excluidos entidad in item.productos_excluidos)
                            entidad.id = 0;
                        promocion.productos_excluidos = item.productos_excluidos;

                        var _beneficios_promocion = _context.beneficios_promocion.Where(s => s.id_promocion == item.id);
                        foreach (beneficios_promocion entidad in _beneficios_promocion)
                            _context.beneficios_promocion.Remove(entidad);
                        foreach (beneficios_promocion entidad in item.beneficios_promocion)
                            entidad.id = 0;
                        promocion.beneficios_promocion = item.beneficios_promocion;

                        var _beneficio_desc = _context.beneficio_desc.Where(s => s.id_promocion == item.id);
                        foreach (beneficio_desc entidad in _beneficio_desc)
                            _context.beneficio_desc.Remove(entidad);
                        foreach (beneficio_desc entidad in item.beneficio_desc)
                            entidad.id = 0;
                        promocion.beneficio_desc = item.beneficio_desc;

                        var _beneficio_productos = _context.beneficio_productos.Where(s => s.id_promocion == item.id);
                        foreach (beneficio_productos entidad in _beneficio_productos)
                            _context.beneficio_productos.Remove(entidad);
                        foreach (beneficio_productos entidad in item.beneficio_productos)
                            entidad.id = 0;
                        promocion.beneficio_productos = item.beneficio_productos;

                        var _beneficio_msi = _context.beneficio_msi.Where(s => s.id_promocion == item.id);
                        foreach (beneficio_msi entidad in _beneficio_msi)
                            _context.beneficio_msi.Remove(entidad);
                        foreach (beneficio_msi entidad in item.beneficio_msi)
                            entidad.id = 0;
                        promocion.beneficio_msi = item.beneficio_msi;

                        var _entidades_obligatorias = _context.entidades_obligatorias.Where(s => s.id_promocion == item.id);
                        foreach (entidades_obligatorias entidad in _entidades_obligatorias)
                            _context.entidades_obligatorias.Remove(entidad);
                        foreach (entidades_obligatorias entidad in item.entidades_obligatorias)
                            entidad.id = 0;
                        promocion.entidades_obligatorias = item.entidades_obligatorias;


                        promocion.afectacion_cc = null;

                        var _afectacion_cc = _context.afectacion_cc.Where(s => s.id_promocion == item.id);
                        //1. Actualizar registros modificados
                        var no_editados = (from n in item.afectacion_cc
                                           join p in _afectacion_cc on
                                           new
                                           {
                                               n.id_condiones_comerciales_sucursal,
                                               n.id_promocion
                                           }
                                           equals
                                           new
                                           {
                                               p.id_condiones_comerciales_sucursal,
                                               p.id_promocion
                                           }
                                           where n.margen == p.margen
                                           select p);

                        var reg_upd = (from n in item.afectacion_cc
                                       join p in _afectacion_cc on 
                                       new
                                       {
                                           n.id_condiones_comerciales_sucursal,
                                           n.id_promocion
                                       }
                                       equals
                                       new
                                       {
                                           p.id_condiones_comerciales_sucursal,
                                           p.id_promocion
                                       }
                                       where n.margen != p.margen
                                       select p);
                        //var actuales = item.afectacion_cc;
                        //var reg = _afectacion_cc.Where(r => item.afectacion_cc.Any(x => r.id_condiones_comerciales_sucursal == x.id_condiones_comerciales_sucursal && r.id_promocion == x.id_promocion));
                        foreach (afectacion_cc actualiza in reg_upd)
                        {
                            actualiza.margen = item.afectacion_cc.FirstOrDefault(a => a.id_condiones_comerciales_sucursal == actualiza.id_condiones_comerciales_sucursal 
                            && a.id_promocion == actualiza.id_promocion).margen;
                            _context.afectacion_cc.Update(actualiza);
                        }

                        
                        //2. Eliminar de la base los que ya no existan
                        var reg_del = (from p in _afectacion_cc
                                       join n in item.afectacion_cc on
                                       new
                                       {
                                           p.id_condiones_comerciales_sucursal,
                                           p.id_promocion
                                       }
                                       equals
                                       new
                                       {
                                           n.id_condiones_comerciales_sucursal,
                                           n.id_promocion
                                       } into del
                                       from sq1 in del.DefaultIfEmpty()
                                       where sq1.id == null
                                       select p);

                        _context.afectacion_cc.RemoveRange(reg_del);

                        //3. Inserta nuevos registros
                       
                        var temp = reg_upd.Union(no_editados);
                        //var reg = _afectacion_cc.Where(r => item.afectacion_cc.Any(x => r.id_condiones_comerciales_sucursal == x.id_condiones_comerciales_sucursal && r.id_promocion == x.id_promocion));
                        item.afectacion_cc.RemoveAll(r => temp.Any(x => r.id_condiones_comerciales_sucursal == x.id_condiones_comerciales_sucursal && r.id_promocion == x.id_promocion));

                        //var nuevos = item.afectacion_cc.Where(r=> temp.Any(x => r.id_condiones_comerciales_sucursal == x.id_condiones_comerciales_sucursal && r.id_promocion == x.id_promocion)).ToList();

                        if (item.afectacion_cc.Count > 0)
                        {
                            _context.afectacion_cc.AddRange(item.afectacion_cc);

                        }
                        /*
                        foreach (afectacion_cc entidad in _afectacion_cc)
                            _context.afectacion_cc.Remove(entidad);
                        foreach (afectacion_cc entidad in item.afectacion_cc)
                            entidad.id = 0;

                        promocion.afectacion_cc = item.afectacion_cc;*/


                        /*
                        var _promociones_compatibles = _context.promociones_compatibles.Where(s => s.id_promocion == item.id);
                        var prom_comid = item.promociones_compatibles[0].id_promocion;
                        var prom_comid2 = item.promociones_compatibles[0].id_promocion_2;

                        promocion item2 = new promocion();

                        item2 = item;

                        item2.promociones_compatibles[0].id_promocion = prom_comid2; 
                        item2.promociones_compatibles[0].id_promocion_2 = prom_comid  ;

                
                        foreach (promociones_compatibles entidad in _promociones_compatibles)
                            _context.promociones_compatibles.Remove(entidad);
                        foreach (promociones_compatibles entidad in item.promociones_compatibles)
                        {
                            entidad.id = 0;
                            promocion.promociones_compatibles.Add(entidad);
                        }
                        foreach (promociones_compatibles entidad in item2.promociones_compatibles)
                        {
                            entidad.id = 0;
                            promocion.promociones_compatibles.Add(entidad);
                        }

                        //   promocion2.promociones_compatibles = item2.promociones_compatibles;
                       // promociones_compatibles pcn = new promociones_compatibles();*/

                        //var _promociones_compatibles = _context.promociones_compatibles.Where(s => s.id_promocion == item.id || s.id_promocion_2==item.id);
                        //foreach (promociones_compatibles entidad in _promociones_compatibles)
                        //    _context.promociones_compatibles.Remove(entidad);
                        //foreach (promociones_compatibles entidad in item.promociones_compatibles)
                        //    entidad.id = 0;
                        //promocion.promociones_compatibles = item.promociones_compatibles;

                        var _promociones_compatibles = _context.promociones_compatibles.Where(s => s.id_promocion == item.id || s.id_promocion_2 == item.id);
                        var promos_comp_eliminadas = (from a in _promociones_compatibles
                                                      join b in item.promociones_compatibles on a.id equals b.id into del
                                                      from s1 in del.DefaultIfEmpty()
                                                      where s1.id == null
                                                      select a
                                                      );
                        var promos_comp_nuevas = item.promociones_compatibles.Where(a => a.id == 0);
                        _context.promociones_compatibles.RemoveRange(promos_comp_eliminadas);
                        _context.promociones_compatibles.AddRange(promos_comp_nuevas);

                        _context.SaveChanges();


                        //var borradas = _entidades_participantes.Except(item.entidades_participantes);
                        //var nuevas = item.entidades_participantes.Except(_entidades_participantes);
                        return response = Ok(new { result = "Success", detalle = "Promocion actualizada con éxito.", id = promocion.id });
                    }
                }
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error al crear-editar promocion: ", detalle = ex.Message, id = 0 });
            }
        }




        // POST: api/Servicios/Actualizar_estatus
        [Route("crear_editar_config_comision")]
        [HttpPost("{crear_editar_config_comision}")]
        public IActionResult crear_editar_config_comision([FromBody] config_comision item)
        {
            //var result = new Models.Response();
            if (item.fecha_hora_fin.IndexOf('N') >= 0)
                item.fecha_hora_fin = item.fecha_hora_inicio;
            IActionResult response = Unauthorized();

            try
            { 
               if (item.id == 0) // Crea Registro 
                {
                    _context.config_comisiones.Add(item);
                    _context.SaveChanges();
                    return response = Ok(new { result = "Success", detalle = "Promocion creada con éxito.", id = item.id });
                }
                else // Edita Registo 
                {
                    var promocion = _context.config_comisiones.FirstOrDefault(s => s.id == item.id);
                    if (promocion == null)
                        return response = Ok(new { result = "Error", detalle = "No existe ningún registro con el Id: " + item.id, id = item.id });
                    else
                    {
                        promocion.id = item.id;
                        promocion.nombre = item.nombre;
                        promocion.fecha_hora_inicio = item.fecha_hora_inicio;
                        promocion.fecha_hora_fin = item.fecha_hora_fin;
                        promocion.vigencia_indefinida = item.vigencia_indefinida;
                        promocion.id_tipos_herencia_promo = item.id_tipos_herencia_promo;
                        promocion.id_cat_tipo_condicion = item.id_cat_tipo_condicion;
                        promocion.monto_condicion = item.monto_condicion;
                        promocion.monto_superior = item.monto_superior;
                        /*Verifica las entidades participantes */

                        var _entidades_participantes = _context.com_entidades_participantes.Where(s => s.id_comisionv == item.id);
                        foreach (com_entidades_participantes entidad in _entidades_participantes)
                            _context.com_entidades_participantes.Remove(entidad);
                        foreach (com_entidades_participantes entidad in item.entidades_participantes)
                            entidad.id = 0;
                        promocion.entidades_participantes = item.entidades_participantes;
                        /*Verifica los productos_condicion */

                        //var _pc_modificados = _context.com_productos_condicions.Where(cc => item.productos_condicion.Any(n => n.id == cc.id));
                        //var _pc_eliminados = _context.com_productos_condicions.Where(cc1 => item.productos_condicion.Any(n1 => n1.id != cc1.id));
                        //_context.com_productos_condicions.UpdateRange(_pc_modificados);
                        //_context.com_productos_condicions.AddRange(item.productos_condicion.Where(a => a.id == 0));


                        var _productos_condicion = _context.com_productos_condicions.Where(pc => pc.id_comisionv == item.id);
                        //_context.com_productos_condicions.RemoveRange(_productos_condicion);
                        foreach (com_productos_condicion entidad in _productos_condicion)
                            _context.com_productos_condicions.Remove(entidad);
                        foreach (com_productos_condicion entidad in item.productos_condicion)
                            entidad.id = 0;
                        promocion.productos_condicion = item.productos_condicion;




                        /*Verifica los productos  excluidos 

                        var _productos_excluidos = _context.productos_excluidos.Where(s => s.id_promocion == item.id);
                        foreach (productos_excluidos entidad in _productos_excluidos)
                            _context.productos_excluidos.Remove(entidad);
                        foreach (productos_excluidos entidad in item.productos_excluidos)
                            entidad.id = 0;
                        promocion.productos_excluidos = item.productos_excluidos;*/


                        //var cc_modificados = _context.com_afectacion_ccs.Where(cc => item.afectacion_cc.Any(n => n.id == cc.id));
                        //_context.com_afectacion_ccs.UpdateRange(cc_modificados);
                        //_context.com_afectacion_ccs.AddRange(item.afectacion_cc.Where(a => a.id == 0));


                        var com_afectacion_ccs = _context.com_afectacion_ccs.Where(cc => cc.id_comisionv == item.id);

                        _context.com_afectacion_ccs.RemoveRange(com_afectacion_ccs);
                        foreach (com_afectacion_cc entidad in com_afectacion_ccs)
                            _context.com_afectacion_ccs.Remove(entidad);
                        foreach (com_afectacion_cc entidad in item.afectacion_cc)
                            entidad.id = 0;
                        promocion.afectacion_cc = item.afectacion_cc;


                        //Mejora para editar, agregar o borrar afectaciones de comisiones


                         /*
                          * 
                          * Verificar si va a tener comisiones compatibles 
                        var _promociones_compatibles = _context.promociones_compatibles.Where(s => s.id_promocion == item.id);
                        foreach (promociones_compatibles entidad in _promociones_compatibles)
                            _context.promociones_compatibles.Remove(entidad);
                        foreach (promociones_compatibles entidad in item.promociones_compatibles)
                            entidad.id = 0;
                        promocion.promociones_compatibles = item.promociones_compatibles;*/ 

                        _context.SaveChanges();
                        //var borradas = _entidades_participantes.Except(item.entidades_participantes);
                        //var nuevas = item.entidades_participantes.Except(_entidades_participantes);
                        return response = Ok(new { result = "Success", detalle = "Promocion actualizada con éxito.", id = promocion.id });
                    } 
                }
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error al crear-editar promocion: ", detalle = ex.Message, id = 0 });
            }
        }

        //POST: api/cargar_promocion_por_id
        [Route("cargar_promocion_por_id")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult cargar_promocion_por_id([FromBody]ModBusPorId busqueda)
        {

            IActionResult response = Unauthorized();
            var promocion = (from c in _context.promocion
                             where c.id == busqueda.Id
                             select new
                             {
                                 c.id,
                                 c.nombre,
                                 c.fecha_hora_inicio,
                                 c.fecha_hora_fin,
                                 c.vigencia_indefinida,
                                 c.id_tipos_herencia_promo,
                                 c.id_cat_tipo_condicion,
                                 c.monto_inferior_condicion,
                                 c.aplica_cc,
                                 c.monto_condicion,
                                 c.incluir_desc_adic,
                                 c.beneficio_obligatorio,
                                 entidades_participantes = (from c1 in _context.entidades_participantes
                                                            where c1.id_promocion == busqueda.Id
                                                            select new
                                                            {
                                                                c1.id,
                                                                c1.id_promocion,
                                                                c1.id_entidad,
                                                                c1.id_tipo_entidad
                                                            }).ToList(),
                                 productos_excluidos = (from c1 in _context.productos_excluidos
                                                        where c1.id_promocion == busqueda.Id
                                                        select new
                                                        {
                                                            c1.id,
                                                            c1.id_promocion, //promocion
                                                            c1.id_producto, // productos, sublinea, linea
                                                            c1.id_tipo_categoria, // 1 Línea , 2 Sublínea , 3 producto 
                                                        }).ToList(),
                                 entidades_obligatorias = (from c1 in _context.entidades_obligatorias
                                                           where c1.id_promocion == busqueda.Id
                                                           select new
                                                           {
                                                               c1.id,
                                                               c1.id_promocion,
                                                               c1.id_entidad,
                                                               c1.id_tipo_entidad,
                                                           }).ToList(),
                                 promociones_compatibles= (from c1 in _context.promociones_compatibles
                                                             where c1.id_promocion == busqueda.Id
                                                             select new
                                                             {
                                                                 c1.id,
                                                                 c1.id_promocion, //promocion
                                                                 c1.id_promocion_2, //promocion
                                                             }).Union((from c1 in _context.promociones_compatibles
                                                                                where c1.id_promocion_2 == busqueda.Id
                                                                                select new
                                                                                {
                                                                                    c1.id,
                                                                                    c1.id_promocion, //promocion
                                                                                    c1.id_promocion_2, //promocion
                                                                                })).ToList(),
                                    entidades_excluidas = (from c1 in _context.entidades_excluidas
                                                        where c1.id_promocion == busqueda.Id
                                                        select new
                                                        {
                                                            c1.id,
                                                            c1.id_promocion,
                                                            c1.id_entidad,
                                                            c1.id_tipo_entidad
                                                        }).ToList(),
                                 productos_condicion = (from c1 in _context.productos_condicion
                                                        where c1.id_promocion == busqueda.Id
                                                        select new
                                                        {
                                                            c1.id,
                                                            c1.id_promocion, //promocion
                                                            c1.id_producto, // productos, sublinea, linea
                                                            c1.id_tipo_categoria, // 1 Línea , 2 Sublínea , 3 producto 
                                                            c1.cantidad
                                                        }).ToList(),
                                 beneficios_promocion = (from c1 in _context.beneficios_promocion
                                                         where c1.id_promocion == busqueda.Id
                                                         select new
                                                         {
                                                             c1.id,
                                                             c1.id_promocion, //promocion
                                                             c1.id_cat_beneficios //cat_beneficios
                                                         }).ToList(),
                                 beneficio_desc = (from c1 in _context.beneficio_desc
                                                   where c1.id_promocion == busqueda.Id
                                                   select new
                                                   {
                                                       c1.id,
                                                       c1.id_promocion, //promocion
                                                       c1.cantidad,
                                                       c1.es_porcentaje
                                                   }).ToList(),
                                 beneficio_productos = (from c1 in _context.beneficio_productos
                                                        where c1.id_promocion == busqueda.Id
                                                        select new
                                                        {
                                                            c1.id,
                                                            c1.id_promocion, //promocion
                                                            c1.id_producto, //productos
                                                            c1.cantidad
                                                        }).ToList(),
                                 beneficio_msi = (from c1 in _context.beneficio_msi
                                                  where c1.id_promocion == busqueda.Id
                                                  select new
                                                  {
                                                      c1.id,
                                                      c1.id_promocion, //promocion
                                                      c1.id_cat_msi
                                                  }).ToList(),
                                 afectacion_cc = (from c1 in _context.afectacion_cc
                                                  where c1.id_promocion == busqueda.Id
                                                  select new
                                                  {
                                                      c1.id,
                                                      c1.id_promocion,
                                                      c1.id_condiones_comerciales_sucursal,
                                                      c1.margen
                                                  }).ToList()
                             }).ToList();
            if (promocion == null)
            {
                return response = Ok(new { result = "Error", item = promocion });
            }
            return response = Ok(new { result = "Success", item = promocion });
            // return new ObjectResult(promocion); 
        }



        //POST: api/cargar_promocion_por_id
        [Route("cargar_promocion_por_id_comision")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult cargar_promocion_por_id_comision([FromBody]ModBusPorId busqueda)
        {

            IActionResult response = Unauthorized();
            var promocion = (from c in _context.config_comisiones
                             where c.id == busqueda.Id
                             select new
                             {
                                 c.id,
                                 c.nombre,
                                 c.fecha_hora_inicio,
                                 c.fecha_hora_fin,
                                 c.vigencia_indefinida,
                                 c.id_tipos_herencia_promo,
                                 c.id_cat_tipo_condicion,
                                 c.monto_condicion,
                                 c.monto_superior,
                                 entidades_participantes = (from c1 in _context.com_entidades_participantes
                                                            where c1.id_comisionv == busqueda.Id
                                                            select new
                                                            {
                                                                c1.id,
                                                                c1.id_comisionv,
                                                                c1.id_entidad,
                                                                c1.id_tipo_entidad
                                                            }).ToList(),
                              
                                
                                
                                
                                 productos_condicion = (from c1 in _context.com_productos_condicions
                                                        where c1.id_comisionv == busqueda.Id
                                                        select new
                                                        {
                                                            c1.id,
                                                            c1.id_comisionv, //promocion
                                                            c1.id_producto, // productos, sublinea, linea
                                                            c1.id_tipo_categoria, // 1 Línea , 2 Sublínea , 3 producto 
                                                            c1.cantidad
                                                        }).ToList(),
                             
                              
                                 afectacion_cc = (from c1 in _context.com_afectacion_ccs
                                                  where c1.id_comisionv == busqueda.Id
                                                  select new
                                                  {
                                                      c1.id,
                                                      c1.id_comisionv,
                                                      c1.id_condiones_comerciales_sucursal,
                                                      c1.margen
                                                  }).ToList()
                                //                  ,
                                //productos_herencia = (from c2 in _context.com_)
                             }).ToList();
            if (promocion == null)
            {
                return response = Ok(new { result = "Error", item = promocion });
            }
            return response = Ok(new { result = "Success", item = promocion });
            // return new ObjectResult(promocion); 
        }




        // POST: api/Servicios/Actualizar_estatus
        [Route("consultar_condiconesc_comerciales_comisiones")]
        [HttpPost("{consultar_condiconesc_comerciales_comisiones}")]
        public IActionResult consultar_condiconesc_comerciales_comisiones([FromBody] config_comision item)
        {
            //var result = new Models.Response();

            IActionResult response = Unauthorized();
            try
            {
                
                /// obtiene los productos Aplicables
                List<Cat_Productos> Productos_condicion = prod_part_productos_com(item.productos_condicion); //No va
                List<Cat_Productos> Productos_excluidos = new List<Cat_Productos>(); // no va 
                List<Cat_Productos> Productos_herencia = prod_herencia_comision(item.id_tipos_herencia_promo, item.id_cat_tipo_condicion, Productos_condicion, Productos_excluidos);
                List<Cat_Productos> Productos_aplicables = Productos_condicion; // va editada 
                  Productos_aplicables.AddRange(Productos_herencia);
                  Productos_aplicables.RemoveAll(p => Productos_excluidos.Any(e => e.id == p.id));


                //// obtiene la Sublinea de los productos aplicables
                List<Cat_SubLinea_Producto> SubLineas_aplicables = _context.Cat_SubLinea_Producto.Where(sb => Productos_aplicables.Any(pa => pa.id_sublinea == sb.id)).ToList();

                /////  obtiene las sucursales obligatorias no se ocupan acá 
                //  List<Cat_Sucursales> Sucursales_EO = entobliga_a_sucursales(item.entidades_obligatorias);

                //// obtiene sucursales aplicables 
                List<Cat_Sucursales> Sucursales_EP = entpart_a_sucursales_com(item.entidades_participantes); // si
                //List<Cat_Sucursales> Sucursales_EX = new List<Cat_Sucursales>(); // no va 
                List<Cat_Sucursales> Suc_resultantes = Sucursales_EP; // si 
                //Suc_resultantes.RemoveAll(r => Sucursales_EX.Any(a => a.Id == r.Id));

                var _canales_aplicables = (from a in Suc_resultantes
                                           join e in _context.Cat_Sucursales on a.Id equals e.Id
                                           join b in _context.Cat_Cuentas on e.Id_Cuenta equals b.Id
                                           join c in _context.Cat_canales on b.Id_Canal equals c.Id
                                           select new
                                           {
                                               c.Id,
                                               canal = c.Canal_en + " | " + c.Canal_es
                                           }).Distinct().ToList();
                //var xas = (from a in _context.Cat_Sucursales
                //               join b in _context.condiones_comerciales_sucursal on a.Id equals b.id_Cat_Sucursales
                //               select a).ToList();

                /*
                 * 
                 * 
                 *                  CAMBIAR POR LAS TABLAS COM_* 
                 * 
                 *
                 **/
                //// obtiene las condiciones comerciales de las sucursales aplicables 
                //var scc_acc = (from x in _context.Cat_Sucursales
                //               join s in Suc_resultantes on x.Id equals s.Id
                //               join cc in _context.condiones_comerciales_sucursal on x.Id equals cc.id_Cat_Sucursales
                //               join sb in Productos_condicion on cc.id_Cat_SubLinea_Producto equals sb.id_sublinea
                //               join t in _context.Cat_SubLinea_Producto on sb.id_sublinea equals t.id
                //               select new scc
                //               {
                //                   id = cc.id
                //                ,
                //                   id_sucursal = s.Id
                //                ,
                //                   sucursal = x.Sucursal
                //                ,
                //                   margen_original = cc.margen
                //                ,
                //                   margen_adicional = 0
                //                ,
                //                   id_sublinea = cc.id_Cat_SubLinea_Producto
                //                ,
                //                   sublinea = t.descripcion
                //               }).ToList();


                var scc_acc = (from x in _context.Cat_Sucursales
                               join s in Suc_resultantes on x.Id equals s.Id
                               //join can in _context.Cat_Cuentas on 
                               join cc in _context.condiones_comerciales_sucursal on x.Id equals cc.id_Cat_Sucursales
                               join sb in Productos_condicion on cc.id_Cat_SubLinea_Producto equals sb.id_sublinea
                               join t in _context.Cat_SubLinea_Producto on sb.id_sublinea equals t.id
                               join l in _context.Cat_Linea_Producto on t.id_linea_producto equals l.id
                               select new scc
                               {
                                   id = cc.id
                                ,
                                   id_sucursal = s.Id
                                ,
                                   sucursal = x.Sucursal
                                ,
                                   id_canal = x.Cuenta.Id_Canal
                                ,
                                   margen_original = cc.margen
                                ,
                                   margen_adicional = 0
                                ,
                                   id_linea = t.id_linea_producto
                                ,
                                   id_sublinea = cc.id_Cat_SubLinea_Producto
                                ,
                                   sublinea = "Sublinea: " + t.descripcion + " | Linea: " + l.descripcion

                               }).ToList();

                var _cc_res = scc_acc;
                //// obtiene las condiciones comerciales modificadas de las sucursales aplicables 
                if (item.id > 0)
                {
                    var scc = (from x in _context.Cat_Sucursales
                               join s in Suc_resultantes on x.Id equals s.Id
                               join cc in _context.condiones_comerciales_sucursal on x.Id equals cc.id_Cat_Sucursales
                               join acc in _context.com_afectacion_ccs on cc.id equals acc.id_condiones_comerciales_sucursal
                               join sb in Productos_condicion on cc.id_Cat_SubLinea_Producto equals sb.id_sublinea
                               join t in _context.Cat_SubLinea_Producto on sb.id_sublinea equals t.id
                               join l in _context.Cat_Linea_Producto on t.id_linea_producto equals l.id
                               where acc.id_comisionv == item.id // cuando es nuevo es 0 y no trae nada 
                               select new scc
                               {
                                   id = cc.id // condicion comercial sucursal
                                   ,
                                   id_sucursal = x.Id
                                   ,
                                   sucursal = x.Sucursal
                                   ,
                                   id_canal = x.Cuenta.Id_Canal
                                   ,
                                   margen_original = cc.margen // margen regular
                                   ,
                                   margen_adicional = acc.margen //modificacion al margen
                                   ,
                                   id_linea = t.id_linea_producto
                                   ,
                                   id_sublinea = cc.id_Cat_SubLinea_Producto
                                   ,
                                   sublinea = "Sublinea: " + t.descripcion + " | Linea: " + l.descripcion
                               }).ToList();

                    //// obtiene las condiciones comerciales de las sucursales aplicables y los margenes adicionales 

                    _cc_res.RemoveAll(r => scc.Any(a => a.id == r.id));
                    _cc_res.AddRange(scc);
                }


                var cc_res = (from x in _cc_res
                              select new
                              {
                                  id = x.id // condicion comercial sucursal
                                 ,
                                 id_canal = x.id_canal
                                 ,
                                  id_sucursal = x.id_sucursal
                                 ,
                                  sucursal = x.sucursal
                                 ,
                                  margen_original = x.margen_original // margen regular
                                 ,
                                  margen_adicional = x.margen_adicional //modificacion al margen
                                 ,
                                 id_linea = x.id_linea
                                 ,
                                  id_sublinea = x.id_sublinea
                                 ,
                                  sublinea = x.sublinea
                              }).Distinct().ToList();

                /////  afectaciones en condiciones comerciales promocion 
                var afectacion_cc = (from x in cc_res
                                     select new
                                     {
                                         id_condiones_comerciales_sucursal = x.id
                                        ,
                                         id_promocion = item.id
                                        ,
                                         id = 0
                                        ,
                                         margen = x.margen_adicional
                                     }).ToList();

                respuesta_cc respuesta_cc_ = new respuesta_cc();
                var respuestaQ = (from t in _context.Cat_canales
                                  where t.Id == 1
                                  select new
                                  {
                                      id = t.Id
                                      ,
                                      lineas_aplicables = (from sla in SubLineas_aplicables join l in _context.Cat_Linea_Producto on sla.id_linea_producto equals l.id orderby l.descripcion select new { l.id, l.descripcion }).Distinct().ToList()
                                      ,
                                      subLineas_aplicables = (from s in SubLineas_aplicables orderby s.descripcion select new { id = s.id, descripcion = s.descripcion }).ToList()
                                      ,
                                      sucursales_aplicables = _context.Cat_Sucursales.Where(s => Suc_resultantes.Any(sr => sr.Id == s.Id)).OrderBy(sr=> sr.Sucursal)
                                      ,
                                      canales_aplicables = _canales_aplicables
                                      ,
                                      cc_res = cc_res
                                      ,
                                      afectacion_cc = afectacion_cc
                                  }).ToList();

                return response = Ok(new { result = "Success", item = respuestaQ });
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message });
            }
        }


        // POST: api/Servicios/Actualizar_estatus 
        // front nuevo-promocion
        [Route("consultar_condiconesc_comerciales")]
        [HttpPost("{consultar_condiconesc_comerciales}")]
        public IActionResult consultar_condiconesc_comerciales([FromBody] promocion item)
        {
            //var result = new Models.Response();

            IActionResult response = Unauthorized();
            try
            {
                /// obtiene los productos Aplicables
                List<Cat_Productos> Productos_condicion = prod_part_productos(item.productos_condicion);
                List<Cat_Productos> Productos_excluidos = prod_exclu_productos(item.productos_excluidos);
                List<Cat_Productos> Productos_herencia = prod_herencia(item.id_tipos_herencia_promo, item.id_cat_tipo_condicion, Productos_condicion, Productos_excluidos);

                List<Cat_Productos> Productos_aplicables = Productos_condicion;
                Productos_aplicables.AddRange(Productos_herencia);
                Productos_aplicables.RemoveAll(p => Productos_excluidos.Any(e => e.id == p.id));


                //// obtiene la Sublinea de los productos aplicables
                List<Cat_SubLinea_Producto> SubLineas_aplicables = _context.Cat_SubLinea_Producto.Where(sb => Productos_aplicables.Any(pa => pa.id_sublinea == sb.id)).ToList();

                /////  obtiene las sucursales obligatorias no se ocupan acá 
                //  List<Cat_Sucursales> Sucursales_EO = entobliga_a_sucursales(item.entidades_obligatorias);

                //// obtiene sucursales aplicables 
                List<Cat_Sucursales> Sucursales_EP = entpart_a_sucursales(item.entidades_participantes);
                List<Cat_Sucursales> Sucursales_EX = entexcl_a_sucursales(item.entidades_excluidas);
                List<Cat_Sucursales> Suc_resultantes = Sucursales_EP;
                Suc_resultantes.RemoveAll(r => Sucursales_EX.Any(a => a.Id == r.Id));

                var _canales_aplicables = (from a in Suc_resultantes
                                           join e in _context.Cat_Sucursales on a.Id equals e.Id
                                           join b in _context.Cat_Cuentas on e.Id_Cuenta equals b.Id
                                           join c in _context.Cat_canales on b.Id_Canal equals c.Id
                                           select new
                                           {
                                               c.Id,
                                               canal = c.Canal_en + " | " + c.Canal_es
                                           }).Distinct().ToList();

                var scc_acc = (from x in _context.Cat_Sucursales
                               join s in Suc_resultantes on x.Id equals s.Id
                               join cc in _context.condiones_comerciales_sucursal on x.Id equals cc.id_Cat_Sucursales
                               join sb in Productos_condicion on cc.id_Cat_SubLinea_Producto equals sb.id_sublinea
                               join t in _context.Cat_SubLinea_Producto on sb.id_sublinea equals t.id
                               join l in _context.Cat_Linea_Producto on t.id_linea_producto equals l.id
                               select new scc
                               {
                                   id = cc.id
                                ,
                                   id_canal = x.Cuenta.Canal.Id
                                ,
                                   canal = x.Cuenta.Canal.Canal_es
                                ,
                                   id_sucursal = s.Id
                                ,
                                   sucursal = x.Sucursal
                                ,
                                   margen_original = cc.margen
                                ,
                                   margen_adicional = 0
                                ,
                                   id_linea = t.id_linea_producto
                                ,
                                   id_sublinea = cc.id_Cat_SubLinea_Producto
                                ,
                                   //sublinea = x.sublinea + " " + _context.Cat_Linea_Producto.FirstOrDefault(l => l.id == (_context.Cat_SubLinea_Producto.FirstOrDefault(s => s.id == x.id_sublinea).id_linea_producto)).descripcion + " - Linea"
                                   sublinea = t.descripcion + " - Sublinea | " + l.descripcion + " - Linea"

                               }).ToList();

                //// obtiene las condiciones comerciales modificadas de las sucursales aplicables 

                var scc = (from x in _context.Cat_Sucursales
                           join s in Suc_resultantes on x.Id equals s.Id
                           join cc in _context.condiones_comerciales_sucursal on x.Id equals cc.id_Cat_Sucursales
                           join acc in _context.afectacion_cc on cc.id equals acc.id_condiones_comerciales_sucursal
                           join sb in Productos_condicion on cc.id_Cat_SubLinea_Producto equals sb.id_sublinea
                           join t in _context.Cat_SubLinea_Producto on sb.id_sublinea equals t.id
                           join l in _context.Cat_Linea_Producto on t.id_linea_producto equals l.id
                           where acc.id_promocion == item.id // cuando es nuevo es 0 y no trae nada 
                           select new scc
                           {
                               id = cc.id // condicion comercial sucursal
                               ,
                               id_canal = x.Cuenta.Canal.Id
                               ,
                               canal = x.Cuenta.Canal.Canal_es
                               ,
                               id_sucursal = x.Id
                               ,
                               sucursal = x.Sucursal
                               ,
                               margen_original = cc.margen // margen regular
                               ,
                               margen_adicional = acc.margen //modificacion al margen
                               ,
                               id_linea = t.id_linea_producto
                               ,
                               id_sublinea = cc.id_Cat_SubLinea_Producto
                               ,
                               sublinea = t.descripcion + " - Sublinea | " + l.descripcion + " - Linea"
                           }).ToList();


                //Obtiene los registros existentes de comisiones 

                //var cc_ex = (from a in _context.comisiones_promo_sublinea_config
                //             where a.id_promocion == item.id
                //             select a);
                


                //// obtiene las condiciones comerciales de vendedores en base a los regis existente y agrega los faltantes
                ///
                //var cc_ven = (from ab in _context.condiones_comerciales_sucursal
                //              join cat_suc in _context.Cat_Sucursales on ab.id_Cat_Sucursales equals cat_suc.Id
                //              join su in Suc_resultantes on ab.id_Cat_Sucursales equals su.Id
                //              join sb in Productos_condicion on ab.id_Cat_SubLinea_Producto equals sb.id_sublinea
                //              join t in _context.Cat_SubLinea_Producto on sb.id_sublinea equals t.id
                //              join l in _context.Cat_Linea_Producto on t.id_linea_producto equals l.id
                //              join ccc in cc_ex on ab.id equals ccc.id_cc_sucursal into sub1
                //              from s1 in sub1.DefaultIfEmpty()
                //              select new
                //              {
                //                  id = s1.id > 0 ? s1.id : 0,
                //                  id_canal = cat_suc.Cuenta.Canal.Id,
                //                  canal = cat_suc.Cuenta.Canal.Canal_es,
                //                  id_cc_sucursal = ab.id,
                //                  id_sucursal = ab.id_Cat_Sucursales,
                //                  sucursal = cat_suc.Sucursal,
                //                  id_promocion = s1.id_promocion == null ? 0 : s1.id_promocion,
                //                  margen = s1.margen == null ? 0 : s1.margen,
                //                  sublinea = t.descripcion + " - Sublinea | " + l.descripcion + " - Linea"
                //              }).Distinct().OrderByDescending(a => a.id).ToList();


                //// obtiene las condiciones comerciales de las sucursales aplicables y los margenes adicionales 
                var _cc_res = scc_acc;
                _cc_res.RemoveAll(r => scc.Any(a => a.id == r.id));
                _cc_res.AddRange(scc);

                var cc_res = (from x in _cc_res
                              select new
                              {
                                  id = x.id // condicion comercial sucursal
                                 , id_canal = x.id_canal
                                 , canal = x.canal
                                 , id_sucursal = x.id_sucursal
                                 , sucursal = x.sucursal
                                 , margen_original = x.margen_original // margen regular
                                 , margen_adicional = x.margen_adicional //modificacion al margen
                                 , id_linea = x.id_linea
                                 , id_sublinea = x.id_sublinea
                                 , sublinea = x.sublinea
                              }).Distinct().ToList();

                /////  afectaciones en condiciones comerciales promocion 
                var afectacion_cc = (from x in cc_res
                                     select new
                                     {
                                         id_condiones_comerciales_sucursal = x.id
                                        , id_promocion = item.id
                                        , id = 0
                                        , margen = x.margen_adicional
                                     }).ToList();

                respuesta_cc respuesta_cc_ = new respuesta_cc();
                var respuestaQ = (from t in _context.Cat_canales
                                  where t.Id == 1
                                  select new
                                  {
                                      id = t.Id
                                      ,lineas_aplicables = (from sla in SubLineas_aplicables join l in _context.Cat_Linea_Producto on sla.id_linea_producto equals l.id orderby l.descripcion  select new { l.id, l.descripcion }).Distinct().ToList()
                                      ,subLineas_aplicables = (from s in SubLineas_aplicables orderby s.descripcion select new { id = s.id, descripcion = s.descripcion }).ToList()
                                      ,sucursales_aplicables = _context.Cat_Sucursales.Where(s => Suc_resultantes.Any(sr => sr.Id == s.Id)).OrderByDescending(a=>a.Sucursal)
                                     // ,_sucursales_aplicables = (from s in Suc_resultantes join select new { id = s.Id, descripcion = s.Sucursal }).ToList()
                                      , cc_res = cc_res
                                      ,canales_aplicables = _canales_aplicables
                                      ,
                                      afectacion_cc = afectacion_cc
                                      //,
                                      //cc_vendedores = cc_ven
                                  }).ToList();

                return response = Ok(new { result = "Success", item = respuestaQ });
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message });
            }
        }


        [Route("consultar_condicones_comisiones")]
        [HttpPost("{consultar_condicones_comisiones}")]
        public IActionResult consultar_condicones_comisiones([FromBody] promocion item)
        {
            //var result = new Models.Response();

            IActionResult response = Unauthorized();
            try
            {
                /// obtiene los productos Aplicables
                List<Cat_Productos> Productos_condicion = prod_part_productos(item.productos_condicion);
                List<Cat_Productos> Productos_excluidos = prod_exclu_productos(item.productos_excluidos);
                List<Cat_Productos> Productos_herencia = prod_herencia(item.id_tipos_herencia_promo, item.id_cat_tipo_condicion, Productos_condicion, Productos_excluidos);

                List<Cat_Productos> Productos_aplicables = Productos_condicion;
                Productos_aplicables.AddRange(Productos_herencia);
                Productos_aplicables.RemoveAll(p => Productos_excluidos.Any(e => e.id == p.id));


                //// obtiene la Sublinea de los productos aplicables
                List<Cat_SubLinea_Producto> SubLineas_aplicables = _context.Cat_SubLinea_Producto.Where(sb => Productos_aplicables.Any(pa => pa.id_sublinea == sb.id)).ToList();

                /////  obtiene las sucursales obligatorias no se ocupan acá 
                //  List<Cat_Sucursales> Sucursales_EO = entobliga_a_sucursales(item.entidades_obligatorias);

                //// obtiene sucursales aplicables 
                List<Cat_Sucursales> Sucursales_EP = entpart_a_sucursales(item.entidades_participantes);
                List<Cat_Sucursales> Sucursales_EX = entexcl_a_sucursales(item.entidades_excluidas);
                List<Cat_Sucursales> Suc_resultantes = Sucursales_EP;
                Suc_resultantes.RemoveAll(r => Sucursales_EX.Any(a => a.Id == r.Id));

                var _canales_aplicables = (from a in Suc_resultantes
                                           join e in _context.Cat_Sucursales on a.Id equals e.Id
                                           join b in _context.Cat_Cuentas on e.Id_Cuenta equals b.Id
                                           join c in _context.Cat_canales on b.Id_Canal equals c.Id
                                           select new
                                           {
                                               c.Id,
                                               canal = c.Canal_en + " | " + c.Canal_es
                                           }).Distinct().ToList();



                //Obtiene los registros existentes de comisiones

                var cc_ex = (from a in _context.comisiones_promo_sublinea_config
                             where a.id_promocion == item.id
                             select a);



                // obtiene las condiciones comerciales de vendedores en base a los regis existente y agrega los faltantes
                var cc_ven = (from ab in _context.condiones_comerciales_sucursal
                              join cat_suc in _context.Cat_Sucursales on ab.id_Cat_Sucursales equals cat_suc.Id
                              join su in Suc_resultantes on ab.id_Cat_Sucursales equals su.Id
                              join sb in Productos_condicion on ab.id_Cat_SubLinea_Producto equals sb.id_sublinea
                              join t in _context.Cat_SubLinea_Producto on sb.id_sublinea equals t.id
                              join l in _context.Cat_Linea_Producto on t.id_linea_producto equals l.id
                              join ccc in cc_ex on ab.id equals ccc.id_cc_sucursal into sub1
                              from s1 in sub1.DefaultIfEmpty()
                              select new
                              {
                                  id = s1.id > 0 ? s1.id : 0,
                                  id_canal = cat_suc.Cuenta.Canal.Id,
                                  canal = cat_suc.Cuenta.Canal.Canal_es,
                                  id_cc_sucursal = ab.id,
                                  id_sucursal = ab.id_Cat_Sucursales,
                                  sucursal = cat_suc.Sucursal,
                                  id_promocion = s1.id_promocion == null ? 0 : s1.id_promocion,
                                  margen = s1.margen == null ? 0 : s1.margen,
                                  sublinea = t.descripcion + " - Sublinea | " + l.descripcion + " - Linea"
                              }).Distinct().OrderByDescending(a => a.id).ToList();

                

                respuesta_cc respuesta_cc_ = new respuesta_cc();
                var respuestaQ = (from t in _context.Cat_canales
                                  where t.Id == 1
                                  select new
                                  {
                                      id = t.Id
                                      ,
                                      lineas_aplicables = (from sla in SubLineas_aplicables join l in _context.Cat_Linea_Producto on sla.id_linea_producto equals l.id orderby l.descripcion select new { l.id, l.descripcion }).Distinct().ToList()
                                      ,
                                      subLineas_aplicables = (from s in SubLineas_aplicables orderby s.descripcion select new { id = s.id, descripcion = s.descripcion }).ToList()
                                      ,
                                      sucursales_aplicables = _context.Cat_Sucursales.Where(s => Suc_resultantes.Any(sr => sr.Id == s.Id))
                                      ,
                                      canales_aplicables = _canales_aplicables
                                      ,
                                      cc_vendedores = cc_ven
                                  }).ToList();

                return response = Ok(new { result = "Success", item = respuestaQ });
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message });
            }
        }


        [Route("consultar_cc_sucursal")]
        [HttpPost("{consultar_cc_sucursal}")]
        public IActionResult consultar_cc_sucursal([FromBody] ModBusPorId item)
        {
            //var result = new Models.Response();

            IActionResult response = Unauthorized();
            try
            {

                //// obtiene la Sublinea de los productos aplicables
                List<Cat_SubLinea_Producto> SubLineas_aplicables = _context.Cat_SubLinea_Producto.Include(sl=> sl.cat_linea_producto).ToList();
                foreach (Cat_SubLinea_Producto sublinea in SubLineas_aplicables)
                {
                    sublinea.descripcion = "Sublinea: " + sublinea.descripcion + " | Linea: " + sublinea.cat_linea_producto.descripcion;
                    sublinea.cat_linea_producto = null;
                }
                //// obtiene las condiciones comerciales de las sucursales aplicables 
                //List<CC_Suc> cc_suc = new List<CC_Suc>();
                ////var cc_suc = new List<>;
                var cc_suc = (from a in _context.Cat_SubLinea_Producto.Where(sl => sl.id >0)
                           select new
                           {
                               id = 0,
                               id_Cat_Sucursales = 0,
                               sucursal = "",
                               margen = 0,
                               margen_adicional = 0,
                               id_Cat_SubLinea_Producto = a.id,
                               sublinea = a.descripcion,
                               id_linea = a.id_linea_producto,
                               linea = a.cat_linea_producto.descripcion
                           }).ToList();
                //var cc_suc = (from cc in _context.condiones_comerciales_sucursal
                //               join x in _context.Cat_Sucursales on cc.id_Cat_Sucursales equals x.Id
                //               join t in _context.Cat_SubLinea_Producto on cc.id_Cat_SubLinea_Producto equals t.id
                //               where cc.id_Cat_Sucursales == item.Id
                //               orderby t.descripcion, t.id_linea_producto ascending
                //               select new
                //               {
                //                   id = cc.id,
                //                   id_sucursal = x.Id,
                //                   sucursal = x.Sucursal,
                //                   margen_original = cc.margen,
                //                   margen_adicional = 0,
                //                   id_sublinea = cc.id_Cat_SubLinea_Producto,
                //                   sublinea = t.descripcion,
                //                   id_linea = t.id_linea_producto,
                //                   linea = _context.Cat_Linea_Producto.FirstOrDefault(l => l.id == t.id_linea_producto).descripcion
                //               }).ToList();
                    return response = Ok(new { result = "Success", item = cc_suc, subLineas_aplicables = SubLineas_aplicables });
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message });
            }
        }

        public List<Cat_Productos> prod_herencia(int id_tipos_herencia_promo, int id_cat_tipo_condicion, List<Cat_Productos> ListaProds_cond, List<Cat_Productos> ListaProds_exc)
        {
            List <Cat_Productos> Productos_herencia =  new List<Cat_Productos>();

            if (id_tipos_herencia_promo > 1 || id_cat_tipo_condicion == 1) //1 No hay herencia // 1 en tipo condicion es todos por que es por monto 
            {
                if (id_tipos_herencia_promo == 5 || id_cat_tipo_condicion == 1)//5   Todos los productos // 1 en tipo condicion es todos por que es por monto 
                {
                    Productos_herencia = _context.Cat_Productos.Where(p => p.id == p.id).ToList();
                    Productos_herencia.RemoveAll(p => ListaProds_exc.Any(e => e.id == p.id));
                }
                else
                {
                    Productos_herencia = ListaProds_cond;
                    Productos_herencia.RemoveAll(p => ListaProds_exc.Any(e => e.id == p.id));

                    var ids_relacionados = (from x in Productos_herencia
                                            join r in _context.productos_relacionados on x.id equals r.id_producto
                                            select new { id = r.id_producto_2 }).ToList(); //x).Select(d => new Cat_Productos { id = d.id, id_sublinea = d.id_sublinea, id_linea = d.id_linea, tipo = d.tipo }).ToList();

                    var relacionados = (from x in _context.Cat_Productos
                                        join r in ids_relacionados on x.id equals r.id
                                        select x).Select(d => new Cat_Productos { id = d.id, id_sublinea = d.id_sublinea, id_linea = d.id_linea, tipo = d.tipo }).ToList();


                    if (id_tipos_herencia_promo == 2) //2   Consumibles Rel.
                    {
                        Productos_herencia = relacionados.Where(p => p.tipo == 2).ToList();
                    }
                    if(id_tipos_herencia_promo == 3)//3   Accesorios Rel.
                    {
                        Productos_herencia = relacionados.Where(p => p.tipo == 3).ToList();
                    }
                    if (id_tipos_herencia_promo == 4) //4   Consumibles / Accesorios Rel.
                    {
                        Productos_herencia = relacionados;
                    }
                }
                
            }
            return Productos_herencia;
        }


        public List<Cat_Productos> prod_herencia_comision(int id_tipos_herencia_promo, int id_cat_tipo_condicion, List<Cat_Productos> ListaProds_cond, List<Cat_Productos> ListaProds_exc)
        {
            List<Cat_Productos> Productos_herencia = new List<Cat_Productos>();

            if (id_tipos_herencia_promo > 1 || id_cat_tipo_condicion == 1) //1 No hay herencia // 1 en tipo condicion es todos por que es por monto 
            {
                if (id_tipos_herencia_promo == 5 || id_cat_tipo_condicion == 1)//5   Todos los productos // 1 en tipo condicion es todos por que es por monto 
                {
                    Productos_herencia = _context.Cat_Productos.Where(p => p.id == p.id).ToList();
                    Productos_herencia.RemoveAll(p => ListaProds_exc.Any(e => e.id == p.id));
                }
                else
                {
                    Productos_herencia = ListaProds_cond;
                    Productos_herencia.RemoveAll(p => ListaProds_exc.Any(e => e.id == p.id));

                    var ids_relacionados = (from x in Productos_herencia
                                            join r in _context.productos_relacionados on x.id equals r.id_producto
                                            select new { id = r.id_producto_2 }).ToList(); //x).Select(d => new Cat_Productos { id = d.id, id_sublinea = d.id_sublinea, id_linea = d.id_linea, tipo = d.tipo }).ToList();

                    var relacionados = (from x in _context.Cat_Productos
                                        join r in ids_relacionados on x.id equals r.id
                                        select x).Select(d => new Cat_Productos { id = d.id, id_sublinea = d.id_sublinea, id_linea = d.id_linea, tipo = d.tipo }).ToList();


                    if (id_tipos_herencia_promo == 2) //2   Consumibles Rel.
                    {
                        Productos_herencia = relacionados.Where(p => p.tipo == 2).ToList();
                    }
                    if (id_tipos_herencia_promo == 3)//3   Accesorios Rel.
                    {
                        Productos_herencia = relacionados.Where(p => p.tipo == 3).ToList();
                    }
                    if (id_tipos_herencia_promo == 4) //4   Consumibles / Accesorios Rel.
                    {
                        Productos_herencia = relacionados;
                    }
                }

            }
            return Productos_herencia;
        }

        //Productos para comisiones
        public List<Cat_Productos> prod_part_productos_com(List<com_productos_condicion> ListaProds)
        {
            List<com_productos_condicion> PC_Linea = ListaProds.Where(r => r.id_tipo_categoria == 1).ToList();
            List<Cat_Linea_Producto> Lineas = _context.Cat_Linea_Producto.Where(r => PC_Linea.Any(a => a.id_producto == r.id)).ToList();
            List<Cat_SubLinea_Producto> SubLineas = _context.Cat_SubLinea_Producto.Where(r => Lineas.Any(a => a.id == r.id_linea_producto)).ToList();
            List<Cat_Productos> Productos = _context.Cat_Productos.Where(r => SubLineas.Any(a => a.id == r.id_sublinea)).ToList();

            List<com_productos_condicion> PC_SubsLinea_ = ListaProds.Where(r => r.id_tipo_categoria == 2).ToList();
            List<Cat_SubLinea_Producto> SubLineas_ = _context.Cat_SubLinea_Producto.Where(r => PC_SubsLinea_.Any(a => a.id_producto == r.id)).ToList();
            List<Cat_Productos> Productos_ = _context.Cat_Productos.Where(r => SubLineas_.Any(a => a.id == r.id_sublinea)).ToList();

            List<com_productos_condicion> _PC_Prods_ = ListaProds.Where(r => r.id_tipo_categoria == 3).ToList();
            List<Cat_Productos> _Productos_ = _context.Cat_Productos.Where(r => _PC_Prods_.Any(a => a.id_producto == r.id)).ToList();

            Productos_.AddRange(_Productos_);
            Productos.AddRange(Productos_);
            return Productos;
        }

        public List<Cat_Productos> prod_part_productos(List<productos_condicion> ListaProds)
        {
            List<productos_condicion> PC_Linea = ListaProds.Where(r => r.id_tipo_categoria == 1).ToList();
            List<Cat_Linea_Producto> Lineas = _context.Cat_Linea_Producto.Where(r => PC_Linea.Any(a => a.id_producto == r.id)).ToList();
            List<Cat_SubLinea_Producto> SubLineas = _context.Cat_SubLinea_Producto.Where(r => Lineas.Any(a => a.id == r.id_linea_producto)).ToList();
            List<Cat_Productos> Productos = _context.Cat_Productos.Where(r => SubLineas.Any(a => a.id == r.id_sublinea)).ToList();

            List<productos_condicion> PC_SubsLinea_ = ListaProds.Where(r => r.id_tipo_categoria == 2).ToList();
            List<Cat_SubLinea_Producto> SubLineas_ = _context.Cat_SubLinea_Producto.Where(r => PC_SubsLinea_.Any(a => a.id_producto == r.id)).ToList();
            List<Cat_Productos> Productos_ = _context.Cat_Productos.Where(r => SubLineas_.Any(a => a.id == r.id_sublinea)).ToList();

            List<productos_condicion> _PC_Prods_ = ListaProds.Where(r => r.id_tipo_categoria == 3).ToList();
            List<Cat_Productos> _Productos_ = _context.Cat_Productos.Where(r => _PC_Prods_.Any(a => a.id_producto == r.id)).ToList();

            Productos_.AddRange(_Productos_);
            Productos.AddRange(Productos_);
            return Productos;
        }

        public List<Cat_Productos> prod_exclu_productos(List<productos_excluidos> ListaProds)
        {
            List<productos_excluidos> PC_Linea = ListaProds.Where(r => r.id_tipo_categoria == 1).ToList();
            List<Cat_Linea_Producto> Lineas = _context.Cat_Linea_Producto.Where(r => PC_Linea.Any(a => a.id_producto == r.id)).ToList();
            List<Cat_SubLinea_Producto> SubLineas = _context.Cat_SubLinea_Producto.Where(r => Lineas.Any(a => a.id == r.id_linea_producto)).ToList();
            List<Cat_Productos> Productos = _context.Cat_Productos.Where(r => SubLineas.Any(a => a.id == r.id_sublinea)).ToList();

            List<productos_excluidos> PC_SubsLinea_ = ListaProds.Where(r => r.id_tipo_categoria == 2).ToList();
            List<Cat_SubLinea_Producto> SubLineas_ = _context.Cat_SubLinea_Producto.Where(r => PC_SubsLinea_.Any(a => a.id_producto == r.id)).ToList();
            List<Cat_Productos> Productos_ = _context.Cat_Productos.Where(r => SubLineas_.Any(a => a.id == r.id_sublinea)).ToList();

            List<productos_excluidos> _PC_Prods_ = ListaProds.Where(r => r.id_tipo_categoria == 3).ToList();
            List<Cat_Productos> _Productos_ = _context.Cat_Productos.Where(r => _PC_Prods_.Any(a => a.id_producto == r.id)).ToList();

            Productos_.AddRange(_Productos_);
            Productos.AddRange(Productos_);
            return Productos;
        }


        //obtener entidades participantes por comisiones
        public List<Cat_Sucursales> entpart_a_sucursales_com(List<com_entidades_participantes> EP)
        {
            List<com_entidades_participantes> entidades_canales = EP.Where(e => e.id_tipo_entidad == 1).ToList();
            var ids_canales = (from x in entidades_canales select x).Select(d => new Cat_canales { Id = d.id_entidad }).ToList();
            var ids_ctas = (from cta in _context.Cat_Cuentas join can in ids_canales on cta.Id_Canal equals can.Id select cta).Select(d => new Cat_Cuentas { Id = d.Id }).ToList();
            var ids_sucs = (from suc in _context.Cat_Sucursales join cta in ids_ctas on suc.Id_Cuenta equals cta.Id select suc).Select(d => new Cat_Sucursales { Id = Convert.ToInt32(d.Id) }).ToList();

            List<com_entidades_participantes> entidades_cuentas = EP.Where(e => e.id_tipo_entidad == 2).ToList();
            var ids_ctas_ = (from x in entidades_cuentas select x).Select(d => new Cat_Cuentas { Id = d.id_entidad }).ToList();
            var ids_sucs_ = (from suc in _context.Cat_Sucursales join cta in ids_ctas_ on suc.Id_Cuenta equals cta.Id select suc).Select(d => new Cat_Sucursales { Id = Convert.ToInt32(d.Id) }).ToList();
            ids_sucs_.AddRange(ids_sucs);

            List<com_entidades_participantes> entidades_sucursales = EP.Where(e => e.id_tipo_entidad == 3).ToList();
            var _ids_sucs_ = (from x in entidades_sucursales select x).Select(d => new Cat_Sucursales { Id = d.id_entidad }).ToList();
            _ids_sucs_.AddRange(ids_sucs_);

            return _ids_sucs_;
        }

        public List<Cat_Sucursales> entpart_a_sucursales(List<entidades_participantes> EP)
        {
            List<entidades_participantes> entidades_canales = EP.Where(e => e.id_tipo_entidad == 1).ToList();
            var ids_canales = (from x in entidades_canales select x).Select(d => new Cat_canales { Id = d.id_entidad }).ToList();
            var ids_ctas = (from cta in _context.Cat_Cuentas join can in ids_canales on cta.Id_Canal equals can.Id select cta).Select(d => new Cat_Cuentas { Id = d.Id }).ToList();
            var ids_sucs = (from suc in _context.Cat_Sucursales join cta in ids_ctas on suc.Id_Cuenta equals cta.Id select suc).Select(d => new Cat_Sucursales { Id = Convert.ToInt32(d.Id) }).ToList();

            List<entidades_participantes> entidades_cuentas = EP.Where(e => e.id_tipo_entidad == 2).ToList();
            var ids_ctas_ = (from x in entidades_cuentas select x).Select(d => new Cat_Cuentas { Id = d.id_entidad }).ToList();
            var ids_sucs_ = (from suc in _context.Cat_Sucursales join cta in ids_ctas_ on suc.Id_Cuenta equals cta.Id select suc).Select(d => new Cat_Sucursales { Id = Convert.ToInt32(d.Id) }).ToList();
            ids_sucs_.AddRange(ids_sucs);

            List<entidades_participantes> entidades_sucursales = EP.Where(e => e.id_tipo_entidad == 3).ToList();
            var _ids_sucs_ = (from x in entidades_sucursales select x).Select(d => new Cat_Sucursales { Id = d.id_entidad }).ToList();
            _ids_sucs_.AddRange(ids_sucs_);

            return _ids_sucs_;
        }

        public List<Cat_Sucursales> entexcl_a_sucursales(List<entidades_excluidas> EP)
        {
            List<entidades_excluidas> entidades_canales = EP.Where(e => e.id_tipo_entidad == 1).ToList();
            var ids_canales = (from x in entidades_canales select x).Select(d => new Cat_canales { Id = d.id_entidad }).ToList();
            var ids_ctas = (from cta in _context.Cat_Cuentas join can in ids_canales on cta.Id_Canal equals can.Id select cta).Select(d => new Cat_Cuentas { Id = d.Id }).ToList();
            var ids_sucs = (from suc in _context.Cat_Sucursales join cta in ids_ctas on suc.Id_Cuenta equals cta.Id select suc).Select(d => new Cat_Sucursales { Id = Convert.ToInt32(d.Id) }).ToList();

            List<entidades_excluidas> entidades_cuentas = EP.Where(e => e.id_tipo_entidad == 2).ToList();
            var ids_ctas_ = (from x in entidades_cuentas select x).Select(d => new Cat_Cuentas { Id = d.id_entidad }).ToList();
            var ids_sucs_ = (from suc in _context.Cat_Sucursales join cta in ids_ctas_ on suc.Id_Cuenta equals cta.Id select suc).Select(d => new Cat_Sucursales { Id = Convert.ToInt32(d.Id) }).ToList();
            ids_sucs_.AddRange(ids_sucs);

            List<entidades_excluidas> entidades_sucursales = EP.Where(e => e.id_tipo_entidad == 3).ToList();
            var _ids_sucs_ = (from x in entidades_sucursales select x).Select(d => new Cat_Sucursales { Id = d.id_entidad }).ToList();
            _ids_sucs_.AddRange(ids_sucs_);

            return _ids_sucs_;
        }

        public List<Cat_Sucursales> entobliga_a_sucursales(List<entidades_obligatorias> EP)
        {
            List<entidades_obligatorias> entidades_canales = EP.Where(e => e.id_tipo_entidad == 1).ToList();
            var ids_canales = (from x in entidades_canales select x).Select(d => new Cat_canales { Id = d.id_entidad }).ToList();
            var ids_ctas = (from cta in _context.Cat_Cuentas join can in ids_canales on cta.Id_Canal equals can.Id select cta).Select(d => new Cat_Cuentas { Id = d.Id }).ToList();
            var ids_sucs = (from suc in _context.Cat_Sucursales join cta in ids_ctas on suc.Id_Cuenta equals cta.Id select suc).Select(d => new Cat_Sucursales { Id = Convert.ToInt32(d.Id) }).ToList();

            List<entidades_obligatorias> entidades_cuentas = EP.Where(e => e.id_tipo_entidad == 2).ToList();
            var ids_ctas_ = (from x in entidades_cuentas select x).Select(d => new Cat_Cuentas { Id = d.id_entidad }).ToList();
            var ids_sucs_ = (from suc in _context.Cat_Sucursales join cta in ids_ctas_ on suc.Id_Cuenta equals cta.Id select suc).Select(d => new Cat_Sucursales { Id = Convert.ToInt32(d.Id) }).ToList();
            ids_sucs_.AddRange(ids_sucs);

            List<entidades_obligatorias> entidades_sucursales = EP.Where(e => e.id_tipo_entidad == 3).ToList();
            var _ids_sucs_ = (from x in entidades_sucursales select x).Select(d => new Cat_Sucursales { Id = d.id_entidad }).ToList();
            _ids_sucs_.AddRange(ids_sucs_);

            return _ids_sucs_;
        }

        //funcion para calculo de comisiones de por cotizacion
        //public void calcular_comisiones_vendedores(long cot_id)
        //{
        //    //var prods = (from a in _context.Cotizacion_Producto
        //    //             join b in _context.Cat_Productos on a.Id_Producto equals b.id
        //    //             join c in _context.comisiones_promo_sublinea_config on b.id_sublinea equals c.id_sublinea
        //    //             where a.Id_Cotizacion == cot_id
        //    //             select a);

        //}


        [Route("busqueda_promociones")]
        [HttpPost("{busqueda_promociones}")]
        public IActionResult busqueda_promociones([FromBody] BusquedaPromociones texto)
        {
            //var result = new Models.Response();
         
            if (texto.FechaFin == "" || texto.FechaFin == null) texto.FechaFin = "01/01/2050";
            if (texto.FechaIni == "" || texto.FechaIni == null) texto.FechaIni = "01/01/1900";
        
            IActionResult response = Unauthorized();
            try
            {
                var promociones = (from p in _context.promocion
                                   join c in _context.cat_tipo_condicion on p.id_cat_tipo_condicion equals c.id
                                   join b in _context.cat_tipos_herencia on p.id_tipos_herencia_promo equals b.id
                                   join h in _context.cat_beneficios on p.id_tipo_beneficio equals h.id
                                   //where  Convert.ToDateTime(p.fecha_hora_inicio, CultureInfo.InvariantCulture).Date >= Convert.ToDateTime(texto.FechaIni, CultureInfo.InvariantCulture).Date
                                   //   && Convert.ToDateTime(p.fecha_hora_fin, CultureInfo.InvariantCulture).Date <= Convert.ToDateTime(texto.FechaFin , CultureInfo.InvariantCulture).Date

                                   select new
                                   {
                                       id = p.id,
                                       nombre = p.nombre,
                                       fecha_hora_fin = p.vigencia_indefinida ? "Indefinido" : p.fecha_hora_fin,
                                       fecha_hora_inicio = p.fecha_hora_inicio,
                                       tipos_herencia_promo = b.tipo,
                                       tipo_condicion = c.tipo_condicion,
                                       obligatoria = p.beneficio_obligatorio ? "Si" : "No",
                                       id_cat_tipo_condicion = p.id_cat_tipo_condicion,
                                       tipo_beneficios = h.beneficio
                                   }).ToList();

                return response = Ok(new { result = "Success", item = promociones });



            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = "detalle: " + ex.Message });
            }
            // var cat_imagenes_producto = (from x in _context.Cat_Imagenes_Producto where x.id_producto == a.id select x).Select(d => new Cat_Imagenes_Producto { id = d.id, id_producto = d.id_producto, url = d.url }).ToList()
        }

        [Route("busqueda_comisiones_vend")]
        [HttpPost("{busqueda_comisiones_vend}")]
        public IActionResult busqueda_comisiones_vend([FromBody] BusquedaPromociones texto)
         {
            //var result = new Models.Response();

            if (texto.FechaFin == "" || texto.FechaFin == null) texto.FechaFin = "01/01/2050";
            if (texto.FechaIni == "" || texto.FechaIni == null) texto.FechaIni = "01/01/1900";
            DateTime fin = Convert.ToDateTime(texto.FechaFin, CultureInfo.InvariantCulture).Date; 
            DateTime ini = Convert.ToDateTime(texto.FechaIni, CultureInfo.InvariantCulture).Date;
            IActionResult response = Unauthorized();
            try
            {
                var promociones = (from p in _context.config_comisiones
                                   join c in _context.cat_tipo_condicion on p.id_cat_tipo_condicion equals c.id
                                   join b in _context.cat_tipos_herencia on p.id_tipos_herencia_promo equals b.id
                                   where
                                   //DateTime dt = DateTime.ParseExact(yourObject.ToString(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

                                     Convert.ToDateTime(p.fecha_hora_inicio, CultureInfo.InvariantCulture) >= ini
                                    && Convert.ToDateTime(p.fecha_hora_fin, CultureInfo.InvariantCulture ) <= fin


                                   select new
                                   {
                                       id = p.id,
                                       nombre = p.nombre,
                                       fecha_hora_fin = p.vigencia_indefinida ? "Indefinido" : p.fecha_hora_fin,
                                       fecha_hora_inicio = p.fecha_hora_inicio,
                                       tipos_herencia_promo = b.tipo,
                                       tipo_condicion = c.tipo_condicion,
                                       id_cat_tipo_condicion = p.id_cat_tipo_condicion
                                   }).ToList();

                return response = Ok(new { result = "Success", item = promociones });



            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = "detalle: " + ex.Message });
            }
            // var cat_imagenes_producto = (from x in _context.Cat_Imagenes_Producto where x.id_producto == a.id select x).Select(d => new Cat_Imagenes_Producto { id = d.id, id_producto = d.id_producto, url = d.url }).ToList()
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// ///////////////////////////////////////////// APLICACION DE LA PROMOCION /////////////////////////////////////////////////
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [Route("cargar_promociones_disponibles")]
        [HttpPost("{cargar_promociones_disponibles}")]
        public IActionResult cargar_promociones_disponibles([FromBody] ModBusPorId busqueda)
        {
            IActionResult response = Unauthorized();
            try
            {
                
                List<promociones_aplicables> pa = new List<promociones_aplicables>();
                Cotizaciones cotizacion = _context.Cotizaciones.FirstOrDefault(c => c.Id == busqueda.Id);
                List<promocion> promos = _context.promocion.Where(p => (Convert.ToDateTime(p.fecha_hora_inicio) <= DateTime.Now) && ((Convert.ToDateTime(p.fecha_hora_fin) > DateTime.Now) || p.vigencia_indefinida )).ToList();
                //List<promociones_aplicables> promociones_aplicables = new List<promociones_aplicables>();

                foreach (var item in promos)
                {
                    bool aplica = false;
                    /// verificar sucursales participantes 
                    item.entidades_participantes = _context.entidades_participantes.Where(pe => pe.id_promocion == item.id).ToList();
                    item.entidades_excluidas = _context.entidades_excluidas.Where(pe => pe.id_promocion == item.id).ToList();

                    List<Cat_Sucursales> Sucursales_EP = entpart_a_sucursales(item.entidades_participantes);
                    List<Cat_Sucursales> Sucursales_EX = entexcl_a_sucursales(item.entidades_excluidas);
                    List<Cat_Sucursales> Suc_resultantes = Sucursales_EP;
                    Suc_resultantes.RemoveAll(r => Sucursales_EX.Any(a => a.Id == r.Id));

                    List<Cat_Sucursales> Sucursal_cotiza = Suc_resultantes.Where(a => a.Id == cotizacion.Id_sucursal).ToList();

                    if (Sucursal_cotiza.Count > 0) { ////  es una sucursal valida para la promocion 

                        List<criterio_promocion_cumplido> cp = new List<criterio_promocion_cumplido>();
                        var importe = _context.Cotizacion_Producto.Where(x => x.Id_Cotizacion == busqueda.Id).Sum(m => m.precio_lista);
                        //val decimal
                        if (Convert.ToDecimal(importe) >= item.monto_condicion)
                        {
                            criterio_promocion_cumplido c = new criterio_promocion_cumplido();
                            c.id_promocion = item.id; c.criterio = "monto"; cp.Add(c);
                        }

                        if (item.id_cat_tipo_condicion == 1)
                        {
                            if (cp.Where(c => c.criterio == "monto").ToList().Count > 0)
                                aplica = true;
                        }
                        else {
                            item.productos_excluidos = _context.productos_excluidos.Where(pe => pe.id_promocion == item.id).ToList();
                            item.productos_condicion = _context.productos_condicion.Where(pe => pe.id_promocion == item.id).ToList();
                            List<Cat_Productos> Productos_excluidos = new List<Cat_Productos>();
                            if (item.productos_excluidos.Count > 0)
                                Productos_excluidos = prod_exclu_productos(item.productos_excluidos);

                            var aplica_pords = prod_part_promo_cotiza(item.productos_condicion, Productos_excluidos, busqueda.Id);

                            if(aplica_pords)
                            {
                                criterio_promocion_cumplido c = new criterio_promocion_cumplido();
                                c.id_promocion = item.id; c.criterio = "productos"; cp.Add(c);
                            }

                            if (item.id_cat_tipo_condicion == 2)
                            {
                                if (cp.Where(c => c.criterio == "productos").ToList().Count > 0)
                                    aplica = true;
                            }
                            if (item.id_cat_tipo_condicion == 3)
                            {
                                if (cp.Count == 2)
                                    aplica = true;
                            }
                        }
                   
                        if(aplica)
                        {
                            promociones_aplicables p_ = new promociones_aplicables();
                            p_.id_cotizacion = busqueda.Id; p_.id_promocion = item.id;
                            pa.Add(p_);
                        }

                    } // fin sucursales validas promocion 
                } // fin for promociones

               // var beneficio = beneficios_promo_cadena(13);
                List<promocion> promociones_ = _context.promocion.Where(p => pa.Any(a => a.id_promocion == p.id)).ToList();
                var promociones_respueta = (from pr in promociones_
                                            select new
                                            {
                                                 id = pr.id,
                                                 nombre = pr.nombre,
                                                 beneficios = beneficios_promo_cadena(pr.id),
                                                 inicio = pr.fecha_hora_inicio,
                                                 fin = pr.fecha_hora_fin
                                            } 
                                            ).ToList();

                return response = Ok(new { result = "Success", item = promociones_respueta });

            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = "detalle: " + ex.Message });
            }
         }

        public List<promocion> get_promociones_aplicables(ModeloBusquedaCotID_User busqueda)
        {
             try
            {
                
                List<promociones_aplicables> pa = new List<promociones_aplicables>();
                Cotizaciones cotizacion = _context.Cotizaciones.FirstOrDefault(c => c.Id == busqueda.Id);
                List<promocion> promos = _context.promocion.Where(p => (Convert.ToDateTime(p.fecha_hora_inicio) <= DateTime.Now) && ((Convert.ToDateTime(p.fecha_hora_fin) > DateTime.Now) || p.vigencia_indefinida )).ToList();
                //List<promociones_aplicables> promociones_aplicables = new List<promociones_aplicables>();

                foreach (var item in promos)
                {
                    bool aplica = false;
                    /// verificar sucursales participantes 
                    item.entidades_participantes = _context.entidades_participantes.Where(pe => pe.id_promocion == item.id).ToList();
                    item.entidades_excluidas = _context.entidades_excluidas.Where(pe => pe.id_promocion == item.id).ToList();

                    List<Cat_Sucursales> Sucursales_EP = entpart_a_sucursales(item.entidades_participantes);
                    List<Cat_Sucursales> Sucursales_EX = entexcl_a_sucursales(item.entidades_excluidas);
                    List<Cat_Sucursales> Suc_resultantes = Sucursales_EP;
                    Suc_resultantes.RemoveAll(r => Sucursales_EX.Any(a => a.Id == r.Id));

                    List<Cat_Sucursales> Sucursal_cotiza = Suc_resultantes.Where(a => a.Id == cotizacion.Id_sucursal).ToList();

                    if (Sucursal_cotiza.Count > 0) { ////  es una sucursal valida para la promocion 

                        List<criterio_promocion_cumplido> cp = new List<criterio_promocion_cumplido>();
                        var importe = _context.Cotizacion_Producto.Where(x => x.Id_Cotizacion == busqueda.Id).Sum(m => m.precio_lista);
                        //val decimal
                        if (Convert.ToDecimal(importe) >= item.monto_condicion)
                        {
                            criterio_promocion_cumplido c = new criterio_promocion_cumplido();
                            c.id_promocion = item.id; c.criterio = "monto"; cp.Add(c);
                        }

                        if (item.id_cat_tipo_condicion == 1)
                        {
                            if (cp.Where(c => c.criterio == "monto").ToList().Count > 0)
                                aplica = true;
                        }
                        else {
                            item.productos_excluidos = _context.productos_excluidos.Where(pe => pe.id_promocion == item.id).ToList();
                            item.productos_condicion = _context.productos_condicion.Where(pe => pe.id_promocion == item.id).ToList();
                            List<Cat_Productos> Productos_excluidos = new List<Cat_Productos>();
                            if (item.productos_excluidos.Count > 0)
                                Productos_excluidos = prod_exclu_productos(item.productos_excluidos);

                            var aplica_pords = prod_part_promo_cotiza(item.productos_condicion, Productos_excluidos, busqueda.Id);

                            if(aplica_pords)
                            {
                                criterio_promocion_cumplido c = new criterio_promocion_cumplido();
                                c.id_promocion = item.id; c.criterio = "productos"; cp.Add(c);
                            }

                            if (item.id_cat_tipo_condicion == 2)
                            {
                                if (cp.Where(c => c.criterio == "productos").ToList().Count > 0)
                                    aplica = true;
                            }
                            if (item.id_cat_tipo_condicion == 3)
                            {
                                if (cp.Count == 2)
                                    aplica = true;
                            }
                        }
                   
                        if(aplica)
                        {
                            promociones_aplicables p_ = new promociones_aplicables();
                            p_.id_cotizacion = busqueda.Id; p_.id_promocion = item.id;
                            pa.Add(p_);
                        }

                    } // fin sucursales validas promocion 
                } // fin for promociones

               // var beneficio = beneficios_promo_cadena(13);
                List<promocion> promociones_ = _context.promocion.Where(p => pa.Any(a => a.id_promocion == p.id)).ToList();
                var promociones_respueta = (from pr in promociones_
                                            select new
                                            {
                                                 id = pr.id,
                                                 nombre = pr.nombre,
                                                 beneficios = beneficios_promo_cadena(pr.id),
                                                 inicio = pr.fecha_hora_inicio,
                                                 fin = pr.fecha_hora_fin
                                            } 
                                            ).ToList();

                return  promociones_;
            }
            catch (Exception ex)
            {
                List<promocion> promociones_ = new List<promocion>();
                return promociones_;
            }

        }

        public bool valida_aplicacion(int id_cotizacion, int id_promocion)
        {
            bool aplicada;

            var aplicadas  = (from b in _context.Cotizacion_Promocion
                               where b.id_promocion == id_promocion && b.id_cotizacion == id_cotizacion
                               select new
                               {
                                   b.id
                               }
                             ).ToList();
            if (aplicadas.Count() > 0)
                aplicada = true;
            else
                aplicada = false;

            return aplicada;
        }


        public string beneficios_promo_cadena(int id_promo)
        {
            var descuento = (from b in _context.beneficio_desc
                              where b.id_promocion == id_promo
                              select new
                              { 
                                  descripcion= (b.es_porcentaje ? " % " : " $ "), 
                                  b.cantidad
                              }
                             ).ToList();

            var msi = (from d in _context.beneficio_msi
                       join m in _context.cat_msi on d.id_cat_msi equals m.id
                       where d.id_promocion == id_promo
                       select new
                           {
                               descripcion = m.desc_msi
                           }
                      ).ToList();

            var regalos = (from d in _context.beneficio_productos
                       join m in _context.Cat_Productos on d.id_producto equals m.id
                       where d.id_promocion == id_promo
                       select new
                       {
                           descripcion = m.nombre
                       }
                     ).ToList();

            
            string beneficios = "";
            if (descuento.Count > 0)
            {
                string desc_ben = (descuento[0].descripcion == " % " ? descuento[0].cantidad + descuento[0].descripcion : descuento[0].descripcion + " " + descuento[0].cantidad);
                beneficios = beneficios + "Descuento " + desc_ben;
            }
                
            if (msi.Count > 0)
                beneficios = beneficios + " | Meses Sin Int. " + msi[0].descripcion ;
            if (regalos.Count > 0)
                beneficios = beneficios + " | Regalo " + regalos[0].descripcion ;
            return beneficios;
        }

        public bool prod_part_promo_cotiza(List<productos_condicion> ListaProdsLista, List<Cat_Productos> Productos_excluidos, int Id_cotiza)
        {
            var ProdsCotiza = (from cp in _context.Cotizacion_Producto where cp.Id_Cotizacion == Id_cotiza select new { id = cp.Id_Producto }).ToList();
            bool existe = false;
            foreach (var ListaProds in ListaProdsLista)
            {
                if (ListaProds.id_tipo_categoria == 1) {
                    List<Cat_SubLinea_Producto> SubLineas = _context.Cat_SubLinea_Producto.Where(a => a.id_linea_producto == ListaProds.id_producto).ToList();
                    List<Cat_Productos> ProductosL = _context.Cat_Productos.Where(r => SubLineas.Any(a => a.id == r.id_sublinea)).ToList();
                    List<Cat_Productos> ProductosExistentes = ProductosL.Where(r => ProdsCotiza.Any(a => a.id == r.id)).ToList();
                    ProductosExistentes.RemoveAll(p => Productos_excluidos.Any(e => e.id == p.id));
                    if (ProductosExistentes.Count > 0)
                        existe = true;
                 }
                if (ListaProds.id_tipo_categoria == 2)
                {
                    List<Cat_Productos> ProductosL = _context.Cat_Productos.Where(a => a.id_sublinea == ListaProds.id_producto).ToList();
                    List<Cat_Productos> ProductosExistentes = ProductosL.Where(r => ProdsCotiza.Any(a => a.id == r.id)).ToList();
                    ProductosExistentes.RemoveAll(p => Productos_excluidos.Any(e => e.id == p.id));
                    if (ProductosExistentes.Count > 0)
                        existe = true;
                }
                if (ListaProds.id_tipo_categoria == 3)
                {
                    List<Cat_Productos> ProductosL = _context.Cat_Productos.Where(p => p.id == ListaProds.id_producto).ToList();
                    List<Cat_Productos> ProductosExistentes = ProductosL.Where(r => ProdsCotiza.Any(a => a.id == r.id)).ToList();
                    ProductosExistentes.RemoveAll(p => Productos_excluidos.Any(e => e.id == p.id));
                    if (ProductosExistentes.Count > 0)
                        existe = true;
                }
                if (existe == false)
                    break;
            }
            return existe;
        }

        // POST: api/Servicios/Actualizar_estatus
        [Route("agregar_quitar_promocion")]
        [HttpPost("{agregar_quitar_promocion}")]
        public IActionResult agregar_quitar_promocion([FromBody]AddQuitPromocion item)
        {
            IActionResult response = Unauthorized();

            try
            {
                var cotiza = _context.Cotizaciones.FirstOrDefault(c => c.Id == item.Id_Cotizacion);
                if (item.agregar_quitar == 0)
                {
                    var cotprod = _context.Cotizacion_Promocion.Where(s => s.id_promocion == item.Id && s.id_cotizacion == item.Id_Cotizacion);
                    foreach(var cp in cotprod)
                    {
                        _context.Cotizacion_Promocion.Remove(cp);
                    }
                    
                    _context.SaveChanges();
                }
                else
                {
                    //se requiere que se solo se aplique una promo por cotizacion
                    //List<Cotizacion_Promocion> ants = _context.Cotizacion_Promocion.Where(cp => cp.id_cotizacion == item.Id_Cotizacion).ToList();
                    //_context.Cotizacion_Promocion.RemoveRange(ants);
                    Cotizacion_Promocion cpr = new Cotizacion_Promocion();
                    cpr.id_cotizacion = item.Id_Cotizacion;
                    cpr.id_promocion = item.Id;
                    _context.Cotizacion_Promocion.Add(cpr);
                    _context.SaveChanges();
                }

                List<promocion> promociones = _context.promocion.FromSql("Get_recalcula_montos_cotizacionid '" + item.Id_Cotizacion.ToString() + "'").ToList();

                var prods = (from cp in _context.Cotizacion_Producto
                                join a in _context.Cat_Productos on cp.Id_Producto equals a.id
                                orderby a.nombre ascending
                                where cp.Id_Cotizacion == item.Id_Cotizacion
                                orderby a.id_linea ascending
                                select new
                                {
                                    id = a.id,
                                    sku = a.sku,
                                    modelo = a.modelo,
                                    nombre = a.nombre,
                                    descripcion_corta = a.descripcion_corta,
                                    cantidad = cp.cantidad,
                                    margen_cc = cp.margen_cc,
                                    importe_precio_lista = cp.precio_lista + cp.iva_precio_lista,
                                    importe_total_bruto = (cp.precio_lista + cp.iva_precio_lista) * cp.cantidad,
                                    importe_condiciones_com = (cp.precio_condiciones_com + cp.iva_cond_comerciales) * cp.cantidad,
                                    importe_con_descuento = (cp.precio_descuento + cp.iva_precio_descuento),
                                    descuento = (cp.precio_lista + cp.iva_precio_lista) - (cp.precio_descuento + cp.iva_precio_descuento),
                                    importetotal = (cp.precio_descuento + cp.iva_precio_descuento) * cp.cantidad,
                                    es_regalo = cp.es_regalo,
                                    agregado_automaticamente = cp.agregado_automaticamente,
                                    cat_imagenes_producto = (from x in _context.Cat_Imagenes_Producto
                                                            where x.id_producto == a.id
                                                            select x)
                                                                .Select(d => new Cat_Imagenes_Producto
                                                                {
                                                                    id = d.id,
                                                                    id_producto = d.id_producto,
                                                                    url = d.url
                                                                }).ToList()

                                }).ToList();

                if (prods == null)
                {
                    return response = Ok(new { response = "Error", prods });
                }
                else
                {

                    foreach (var prod in prods)
                    {
                        if (prod.cat_imagenes_producto.Count == 0)
                        {
                            Cat_Imagenes_Producto cip = new Cat_Imagenes_Producto();
                            cip.url = "../assets/img/img_prod_no_dips.png";
                            cip.id = 0;
                            cip.id_producto = prod.id;
                            prod.cat_imagenes_producto.Add(cip);
                        }
                    }

                    var montos = (from c in _context.Cotizaciones
                                    where c.Id == cotiza.Id
                                    select new
                                    {

                                        //////////////////////////////////////MONTOS
                                        importe_precio_lista = c.importe_precio_lista,
                                        iva_precio_lista = c.iva_precio_lista,
                                        importe_condiciones_com = c.importe_condiciones_com,
                                        iva_condiciones_com = c.iva_condiciones_com,
                                        importe_promociones = c.importe_promociones,
                                        iva_promociones = c.iva_promociones,
                                        descuento_acumulado = c.descuento_acumulado,
                                        descuento_acumulado_cond_com = c.descuento_acumulado_cond_com,
                                        comision_vendedor = c.comision_vendedor
                                    }).ToList();

                    var promociones_respueta = (from pr in promociones
                                                select new
                                                {
                                                    id = pr.id,
                                                    nombre = pr.nombre,
                                                    beneficios = beneficios_promo_cadena(pr.id),
                                                    inicio = pr.fecha_hora_inicio,
                                                    fin = pr.fecha_hora_fin,
                                                    vigencia_indefinida = pr.vigencia_indefinida,
                                                    aplicada = valida_aplicacion(Convert.ToInt32(item.Id_Cotizacion), pr.id),
                                                    beneficio_obligatorio = pr.beneficio_obligatorio
                                                }
                                                                ).ToList();

                    return response = Ok(new { response = "Success", prods, montos, detalle = "Cotización Cargada Correctamente", promos_aplicables = promociones_respueta });
                }
                
            }
            catch (Exception ex)
            {
                return response = Ok(new { response = "Error", item });
            }

        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////  CONDICIONES C. ////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [Route("get_cc_nueva_suc_sublinea")]
        [HttpPost("{get_cc_nueva_suc_sublinea}")]
        public IActionResult get_cc_nueva_suc_sublinea([FromBody] BusquedaPromociones texto)
        {
            //var result = new Models.Response();
            IActionResult response = Unauthorized();
            try
            {
                var cc_suc_sl = (from s in _context.Cat_SubLinea_Producto
                                   select new
                                   {
                                       id = 0,
                                       id_Cat_SubLinea_Producto = s.id,
                                       id_Cat_Sucursales = 0,
                                       margen = 0
                                   }).ToList();

                return response = Ok(new { result = "Success", item = cc_suc_sl });
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = "detalle: " + ex.Message });
            }
            // var cat_imagenes_producto = (from x in _context.Cat_Imagenes_Producto where x.id_producto == a.id select x).Select(d => new Cat_Imagenes_Producto { id = d.id, id_producto = d.id_producto, url = d.url }).ToList()
        }


        // Margenes Comerciales
        [Route("get_margenes_comerciales")]
        [HttpPost("get_margenes_comerciales")]
        public IActionResult get_margenes_comerciales([FromBody]busqueda_entidades busqueda)
        {
            var U_session = (from a in _context.Users
                             where a.id == busqueda.IDUSR
                             select new { id_canal = a.id_canal, id_cuenta = a.id_cuenta, Id_sucursal = a.id_Sucursales, id_rol = a.id_rol, nivel = a.nivel, id = a.id }).ToList();

            if (busqueda.FecFin == "" || busqueda.FecFin == null) busqueda.FecFin = "01/01/2050";
            if (busqueda.FecIni == "" || busqueda.FecIni == null) busqueda.FecIni = "01/01/1900";
            DateTime fI = Convert.ToDateTime(busqueda.FecFin);

            IActionResult response = Unauthorized();
            long _cuenta = 0;
            long _canal = 0;
            long id_nivel = 0;
            if (busqueda.TextoLibre == null) { busqueda.TextoLibre = ""; }
            // && a.Id_Vendedor == (U_session[0].id_rol == 10004 ? U_session[0].id : a.Id_Vendedor) no estan limitados solo a las suyas

            if (busqueda.Cuenta == "") { _cuenta = 0; } else { _cuenta = Convert.ToInt64(busqueda.Cuenta); }
            if (busqueda.Canal == "") { _canal = 0; } else { _canal = Convert.ToInt64(busqueda.Canal); }
            switch (U_session[0].nivel) { case "sucursal": id_nivel = 1; break; case "cuenta": id_nivel = 2; break; case "canal": id_nivel = 3; break; case "global": id_nivel = -1; break; }

            try
            {
                //Left outer join
                //var subq1 = (from a in _context.Cotizaciones
                //             join b in _context.comisiones_sucursales on a equals b.cotizaciones into s1
                //             from sb in s1.DefaultIfEmpty()
                //             where a.Estatus > 3
                //             select new
                //             {
                //                 a.Id_sucursal,
                //                 vtot = a.importe_precio_lista + a.iva_precio_lista,
                //                 a.comision_sucrusal,
                //                 pendiente = sb.pagada == true ? 0 : a.comision_sucrusal
                //             });

                var subq1 = (from a in _context.Cotizaciones
                             join b in _context.comisiones_sucursales on a.Id equals b.id_cotizacion
                             where a.Estatus > 3
                     && Convert.ToDateTime(busqueda.FecFin).Date >= a.cambio_ord_comp_generada.Date
                                      && Convert.ToDateTime(busqueda.FecIni).Date <= a.cambio_ord_comp_generada.Date
                             select new
                             {
                                 a.Id_sucursal,
                                 vtot = a.importe_precio_lista + a.iva_precio_lista,
                                 a.comision_sucrusal,
                                 pendiente = b.pagada == true ? 0 : a.comision_sucrusal
                             });

                
                var subq2 = (from s in subq1
                             group s by s.Id_sucursal into s2
                             select new
                             {
                                 Id_suc = s2.First().Id_sucursal,
                                 vtotales = s2.Sum(s => s.vtot),
                                 ctotales = s2.Sum(s => s.comision_sucrusal),
                                 cpendientes = s2.Sum(s => s.comision_sucrusal)
                             });

                var res = (from s in _context.Cat_Sucursales
                           join s2 in subq2 on s.Id equals s2.Id_suc into s2
                           from sb2 in s2.DefaultIfEmpty()
                           select new
                           {
                               s.Id,
                               s.Sucursal,
                               ventas_totales = sb2.vtotales > 0 ? sb2.vtotales : 0,
                               comisiones_totales = sb2.ctotales > 0 ? sb2.ctotales : 0,
                               comisiones_pendientes = sb2.cpendientes > 0 ? sb2.cpendientes : 0,
                               canal = _context.Cat_canales.FirstOrDefault(x => x.Id == _context.Cat_Cuentas.FirstOrDefault(c => c.Id == s.Id_Cuenta).Id_Canal).Canal_es,
                               id_canal = _context.Cat_canales.FirstOrDefault(x => x.Id == _context.Cat_Cuentas.FirstOrDefault(c => c.Id == s.Id_Cuenta).Id_Canal).Id,
                               cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == s.Id_Cuenta).Cuenta_es,
                               id_cuenta = _context.Cat_Cuentas.FirstOrDefault(x => x.Id == s.Id_Cuenta).Id,
                           });

                return response = Ok(new { result = "Success", item = res });
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = "detalle: " + ex.Message });
            }    
        }

        // Detalle Margenes Comerciales
        [Route("get_detalle_marg_comerciales")]
        [HttpPost("get_detalle_marg_comerciales")]
        public IActionResult get_detalle_marg_comerciales([FromBody] busqueda_entidades id)
        {
            var U_session = (from a in _context.Users
                             where a.id == id.IDUSR
                             select new { id_canal = a.id_canal, id_cuenta = a.id_cuenta, Id_sucursal = a.id_Sucursales, id_rol = a.id_rol, nivel = a.nivel, id = a.id }).ToList();

            if (id.FecFin == "" || id.FecFin == null) id.FecFin = "01/01/2050";
            if (id.FecIni == "" || id.FecIni == null) id.FecIni = "01/01/1900";
            DateTime fI = Convert.ToDateTime(id.FecFin);

            IActionResult response = Unauthorized();
          

            try
            {
                // item1: contenido tabla
                var tabla = (from c in _context.Cotizaciones
                            join cl in _context.Clientes on c.Id_Cliente equals cl.id
                            join cs in _context.comisiones_sucursales on c equals cs.cotizaciones into s1
                            from sb in s1.DefaultIfEmpty()
                            where c.Estatus > 3 // Verificar esta condicion
                            && c.Id_sucursal == id.id // cachar en el endpoint
                            && Convert.ToDateTime(id.FecFin).Date >= sb.fecha_generacion.Date
                                      && Convert.ToDateTime(id.FecIni).Date <= sb.fecha_generacion.Date

                             select new
                            {
                                c.Id,
                                referido = cl.referidopor == 0 ? "NO" : "SI",
                                c.ibs,
                                f_generacion = sb.fecha_generacion > DateTime.MinValue ? sb.fecha_generacion.ToShortDateString() : "No generada",
                                venta_cotizacion = c.importe_precio_lista + c.iva_precio_lista,
                                comision_cotizacion = c.comision_sucrusal,
                                pagado = sb.pagada == true ? sb.fecha_de_pago.ToString() : "boton:Pagar",
                               
                            });

                // item2: Resumen
                var subq1 = (from a in _context.Cotizaciones
                                           join b in _context.comisiones_sucursales on a equals b.cotizaciones into s1
                                           from sb in s1.DefaultIfEmpty()
                                           where a.Estatus > 3
                                           && a.Id_sucursal == id.id
                                           select new
                                           {
                                               a.Id_sucursal,
                                               vtot = a.importe_precio_lista + a.iva_precio_lista,
                                               a.comision_sucrusal,
                                               pendiente = sb.pagada == true ? 0 : a.comision_sucrusal
                                           });

                var subq2 = (from s in subq1
                             group s by s.Id_sucursal into s2
                             select new
                             {
                                 Id_suc = s2.First().Id_sucursal,
                                 vtotales = s2.Sum(s => s.vtot),
                                 ctotales = s2.Sum(s => s.comision_sucrusal),
                                 cpendientes = s2.Sum(s => s.comision_sucrusal)
                             });

                // item3: sucursal
                var sucursal = (from s in _context.Cat_Sucursales
                                where s.Id == id.id
                                select new
                                {
                                    s.Sucursal
                                }).FirstOrDefault();

                return response = Ok(new { result = "Success", item = tabla, item2 = subq2, item3 = sucursal });
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = "detalle: " + ex.Message });
            }

        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ///////////////////////////////////////////////////  DEFINICIONES //////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        public class criterio_promocion_cumplido
        {
            public int id_promocion { get; set; }
            public string criterio { get; set; }
        }

        public class respuesta_cc
        {
            //public List<Cat_Productos> Productos_aplicables { get; set; }
            public List<Cat_SubLinea_Producto> SubLineas_aplicables { get; set; }
            //public List<Cat_Sucursales> Suc_resultantes { get; set; }
            public List<scc> scc_acc { get; set; }
            //public List<scc> scc { get; set; }
        }

        public class scc
        {
            public int id { get; set; }
            public long id_canal { get; set; }
            public string canal { get; set; }
            public int id_sucursal { get; set; }
            public string sucursal { get; set; }
            public float margen_original { get; set; }
            public float margen_adicional { get; set; }
            public int id_linea { get; set; }
            public int id_sublinea { get; set; }
            public string sublinea { get; set; }
        }


        public class busquedaLibre
        {
            public string TextoLibre { get; set; }
        }

        public class busqueda_entidades
        {
            public int IDUSR { get; set; }
            public int id { get; set;  }
            public string TextoLibre { get; set; }
            public string Canal { get; set;  }
            public string Cuenta { get; set; }
            public int tipo_entidad { get; set; }
            public string FecIni { get; set;  }
            public string FecFin { get; set;  }
        }

        public class ModeloBusquedaCot
        {
            public string TextoLibre { get; set; }
            public string FechaIni { get; set; }
            public string FechaFin { get; set; }
            public int Estatus { get; set; }
            public string Cuenta { get; set; }
            public long Canal { get; set; }
            public string TextoProd { get; set; }
        }

        public class BusquedaPromociones
        {
            public string TextoLibre { get; set; }
            public string FechaIni { get; set; }
            public string FechaFin { get; set; }
            public int id_cat_tipo_condicion { get; set; }
        }

        public class ModeloBusquedaCotRol
        {
            public string TextoLibre { get; set; }
            public string FechaIni { get; set; }
            public string FechaFin { get; set; }
            public int Estatus { get; set; }
            public long id_Cuenta { get; set; }
            public long Canal { get; set; }
            public string TextoProd { get; set; }
            public int Id_user { get; set; }
            public string modelo { get; set; }
            public bool duplicadas { get; set; }
            public int Id_sucursal { get; set; }
            public bool solicitudes { get; set; }
            public bool canceladas { get; set; }
            public bool rechazadas { get; set; }
        }

        public class ModeloBusquedaCotID
        {
            public int Id { get; set; }
        }

        public class ModeloBusquedaCotID_User
        {
            public int Id { get; set; }
            public int Id_user { get; set; }
        }

        public class permisos_cotizacion_detalle
        {
            public bool mostrar_referencia { get; set; }
            public bool mostrar_subir_archivos { get; set; }
            public bool mostrar_condiciones_comerciales { get; set; }
            public bool mostrar_letrero_ibs { get; set; }
            public bool deshabilitar_dir_ins { get; set; }
            public bool deshabilitar_dir_env { get; set; }
            public bool deshabilitar_fact { get; set; }
            public bool deshabilitar_lista_prods { get; set; }
            public bool deshabilitar_formas_pago { get; set; }
            public bool deshabilitar_vendedor { get; set; }
            public bool deshabilitar_cliente { get; set; }
            public bool deshabilitar_referencia { get; set; }
            public bool deshabilitar_rechazo { get; set; }
            public bool deshablitar_btn_guardar_gral { get; set; }
            public bool deshabilitar_acepto_terminos { get; set; }
            public bool deshabilitar_btn_cancelar { get; set; }
            public bool deshabilitar_agregar_prod { get; set; }
            public bool deshabilitar_botones_upload { get; set; }
            public bool deshabilitar_editar_promociones { get; set; }
            public bool deshabilitar_sol_entrega { get; set; }

            public bool mostrarBtnOrden { get; set; }
            public bool mostrarBtnComp { get; set; }
            public bool mostrar_boton_duplicar { get; set; }
            public bool mostrar_boton_edit_basicos { get; set; }
            public bool mostrar_boton_edit_detalles { get; set; }
            public bool mostrar_btn_guardar_gral { get; set; }
            public bool mostrar_rechazo { get; set; }
            public string texto_btn_guardar_gral { get; set; }
            public bool mostrar_chkfact { get; set; }
            public bool mostrar_acepto_terminos { get; set; }
            public bool mostrar_fact { get; set; }
            public bool mostrar_sol_entrega { get; set; }
            public bool mostrar_boton_IBS { get; set; }
            public bool mostrar_lista_comp { get; set; }
            public bool mostrar_btn_rechazar { get; set; }
        }

        public class cotizacion_detalle_post
        {
            public Cotizaciones cotizacion;
            public Cat_Direccion direccion_ins;
            public Cat_Direccion direccion_env;
            public DatosFiscales fiscales;
            public Clientes cliente;

        }

        public class ModeloBusquedaBD
        {
            public int Id { get; set; }
            public int Id_Rol { get; set; }
        }

        public class ModeloResultadoCot
        {
            public long Id { get; set; }
            public string Numero { get; set; }
            public int Cuenta { get; set; }
            public string NombreCliente { get; set; }
            public string Fecha { get; set; }
            public string NombreVendedor { get; set; }
            public double importe { get; set; }
            public int NumeroArticulos { get; set; }
            public int Estatus { get; set; }
            public bool Acciones { get; set; }
        }

        public class ModeloNumProdC
        {
            public int Id_user { get; set; }

        }

        public class CC_Suc
        {
            public int id { get; set; }
            public int id_sucursal { get; set; }
            public string sucursal { get; set; }
            public float margen_original { get; set; }
            public float margen_adicional { get; set; }
            public int id_sublinea { get; set; }
            public string sublinea { get; set; }
            public int id_linea { get; set; }
            public string linea { get; set; }


        }

        public class ModBusPorId
        {
            public int Id { get; set; }
        }

        public class AddQuitPromocion
        {
            public int Id_Cotizacion { get; set; }
            public int Id { get; set; }
            public int agregar_quitar { get; set; }
        }

        public class ModBusPorDosId
        {
            public int Id1 { get; set; }
            public int Id2 { get; set; }
        }

        public class ModBusPorTresId
        {
            public int Id1 { get; set; }
            public int Id2 { get; set; }
            public int Id3 { get; set; }
        }

        public class AddProdcotiza
        {
            public int Id { get; set; }
            public int cantidad { get; set; }
            public int Id_Cotizacion { get; set; }
        }

        public class ModIdTexto
        {
            public int Id { get; set; }
            public string texto { get; set; }
        }

        public class obj_dir_cliente
        {
            public List<Direccion_Cotizacion> direcciones { get; set; }
            public int Id_cliente { get; set; }
            public bool cat_dir { get; set; } // viene en true cuando hay que editar Cat_direcciones
            public bool dir_clie { get; set; } // viene en true cuando hay que editar direcciones_cliente
        }

        public class CotizacionesCond
        {
            public int Id { get; set; }
            public int id_formapago { get; set; }
            public int id_condpago { get; set; }
            public int Estatus { get; set; }


        }

        public class CotizacionesIbs
        {
            public int Id { get; set; }
            public string ibs { get; set; }
            public List<string> correos { get; set; }
        }

        public class respuesta_valida_cp
        {
            public bool valido_cp_home { get; set; }
            public bool valido_cp_cert { get; set; }
        }


        public class sublineas_cantidad
        {
            public int id { get; set; }
            public int cantidad { get; set; }
        }

        //public class lista_certificado
        //{
        //    public string cp { get; set; }
        //    public List<ids_cantidad> lista_sublineas { get; set; }
        //}

        //public class guardar_certificados
        //{
        //    public string cp { get; set; }
        //    public int cotizacion_id { get; set; }
        //    public int id_usuario { get; set; }
        //    public List<ids_cantidad> lista_sublineas { get; set; }
        //}
        public class guardar_home_program
        {
            public int cotizacion_id { get; set; }
            public int id_usuario { get; set; }
            public List<sublineas_cantidad> lista_sublineas { get; set; }
            
        }
    }

    
}

