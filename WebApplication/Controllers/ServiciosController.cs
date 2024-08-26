using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplication.Models;
using System.Text;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.Net.Mail;
using System.Net;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.Globalization;
using WebApplication.Repository;
using System.Text.RegularExpressions;
using System.Drawing;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/Servicios")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class ServiciosController : Controller
    {
        private IConfiguration _config;
        private IEmailRepository _emailRepository;
        //private EmailSettings _emailSettings;
        private readonly MieleContext _context;
        private IConverter _converter;

        //public ServiciosController(IConverter converter, IConfiguration config, MieleContext context, IEmailRepository emailRepository, EmailSettings emailSettings,)
        public ServiciosController(IConverter converter, IConfiguration config, IEmailRepository emailRepository, MieleContext context)
        {
            _converter = converter;
            _config = config;
            _emailRepository = emailRepository;
            //_emailSettings = emailSettings;
            _context = context;
        }

        // GET: api/Servicios
        [HttpGet("values")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Servicios/5
        [HttpGet("{id}", Name = "Get_Servicios")]
        public IActionResult GetById(long id)
        {
            var item = (from a in _context.Servicio
                        join b in _context.Cat_Categoria_Servicio on a.id_categoria_servicio equals b.id into _cat_servicio
                        from cat_servicio in _cat_servicio.DefaultIfEmpty()
                        join c in _context.Clientes on a.id_cliente equals c.id
                        join g in _context.Cat_distribuidor_autorizado on a.id_distribuidor_autorizado equals g.id into es
                        from fd in es.DefaultIfEmpty()
                        join h in _context.Cat_solicitado_por on a.id_solicitado_por equals h.id
                        join hh in _context.Cat_solicitud_via on a.id_solicitud_via equals hh.id
                        join j in _context.Cat_tipo_servicio on a.id_tipo_servicio equals j.id into _cts
                        from cts in _cts.DefaultIfEmpty()
                        join sub in _context.Sub_cat_tipo_servicio on a.id_sub_tipo_servicio equals sub.id into _scts
                        from scts in _scts.DefaultIfEmpty()
                        join k in _context.Cat_estatus_servicio on a.id_estatus_servicio equals k.id into tu
                        from fe in tu.DefaultIfEmpty()
                        where a.id == id
                        select new
                        {
                            id = a.id,
                            id_cliente = c.id,
                            cliente = c.nombre.ToUpper() + " " + c.paterno.ToUpper() + " " + c.materno.ToUpper(),
                            cliente_telefono = c.telefono == null ? "" : c.telefono,
                            cliente_movil = c.telefono_movil == null ? "" : c.telefono_movil,
                            categoria_servicio = (cat_servicio == null) ? "" : cat_servicio.desc_categoria_servicio.ToUpper(),
                            distribuidor_autorizado = (fd == null) ? "" : fd.desc_distribuidor,
                            solicitado_por = h.desc_solicitado_por.ToUpper(),
                            solicitud_via = hh.desc_solicitud_via.ToUpper(),
                            id_tipo_servicio = cts.id,
                            tipo_servicio = cts.desc_tipo_servicio.ToUpper(),
                            sub_tipo_servicio = scts.sub_desc_tipo_servicio.ToUpper(),
                            id_estatus = fe == null ? 0 : fe.id,
                            estatus_servicio = (fe == null) ? "" : fe.desc_estatus_servicio.ToUpper(),
                            contacto = a.contacto.ToUpper(),
                            IBS = a.IBS,
                            ref_direccion = c.referencias == null ? " " : c.referencias,
                            actividades_realizar = a.descripcion_actividades.ToUpper(),
                            descripcion_actividades = a.descripcion_actividades.ToUpper(),
                            motiva_cancelacion = a.visita[0].motiva_cancelación == null ? "" : a.visita[0].motiva_cancelación,
                            motiva_completado = a.visita[0].motiva_completado == null ? "" : a.visita[0].motiva_completado,
                            visitas = (from x in _context.Visita
                                       join d in _context.Cat_Direccion on x.id_direccion equals d.id
                                       join e in _context.Cat_Estado on d.id_estado equals e.id
                                       join f in _context.Cat_Municipio on d.id_municipio equals f.id
                                       join g in _context.Cat_Localidad on d.id_localidad equals g.id
                                       join kk in _context.CatEstatus_Visita on x.estatus equals kk.id into t
                                       from fee in t.DefaultIfEmpty()
                                       join pd in _context.Prediagnostico on x.id equals pd.id_visita into _pre
                                       from pre in _pre.DefaultIfEmpty()
                                       where x.id_servicio == a.id
                                       select new
                                       {
                                           id = x.id,
                                           direccion = d.calle_numero.ToUpper() + ", " + d.numExt + ", " + (d.numInt == null ? "" : d.numInt) + ", " + g.desc_localidad.ToUpper() + ", " + e.desc_estado.ToUpper() + ", " + f.desc_municipio.ToUpper(),
                                           id_direccion = d.id,
                                           id_servicio = x.id_servicio,
                                           pre.observaciones,
                                           motiva_cancelación = x.motiva_cancelación,
                                           productos = (from rel in _context.Rel_servicio_producto
                                                        join zz in _context.Cliente_Productos on rel.id_producto equals zz.Id
                                                        join z in _context.Visita on rel.id_vista equals z.id
                                                        join di in _context.Cat_Direccion on z.id_direccion equals di.id into _direccion
                                                        from direccion in _direccion.DefaultIfEmpty()
                                                        join col in _context.Cat_Localidad on direccion.id_localidad equals col.id into _localidad
                                                        from localidad in _localidad.DefaultIfEmpty()
                                                        join Cat_Productos in _context.Cat_Productos on zz.Id_Producto equals Cat_Productos.id
                                                        join Cat_Categoria_Producto in _context.Cat_SubLinea_Producto on Cat_Productos.id_sublinea equals Cat_Categoria_Producto.id into _categoria
                                                        from categoria in _categoria.DefaultIfEmpty()
                                                        join estatus in _context.CatEstatus_Producto on zz.Id_EsatusCompra equals estatus.id
                                                        where zz.Id_Cliente == c.id && rel.id_vista == x.id
                                                        select new
                                                        {
                                                            id = zz.Id,
                                                            rel_cliente = zz.Id,
                                                            id_producto = Cat_Productos.id,
                                                            id_servicio = z.id_servicio,
                                                            sku = Cat_Productos.sku,
                                                            modelo = (Cat_Productos.modelo == null ? "" : Cat_Productos.modelo.ToUpper()),
                                                            nombre = Cat_Productos.nombre.ToUpper(),
                                                            no_serie = zz.no_serie == null ? "" : zz.no_serie,
                                                            estatus_producto = estatus.desc_estatus_producto == null ? "N/A" : estatus.desc_estatus_producto.ToUpper(),
                                                            //no_visitas = (from count in _context.Visita
                                                            //              where count.id == z.id
                                                            //              select new
                                                            //              {
                                                            //                  count.id
                                                            //              }).Count(),
                                                            descripcion_corta = Cat_Productos.descripcion_corta.ToUpper(),
                                                            descripcion_larga = Cat_Productos.descripcion_larga.ToUpper(),
                                                            atributos = Cat_Productos.atributos.ToUpper(),
                                                            //precio_sin_iva = Cat_Productos.precio_sin_iva,
                                                            //precio_con_iva = Cat_Productos.precio_con_iva,
                                                            categoria = (categoria == null) ? "" : categoria.descripcion.ToUpper(),
                                                            id_categoria = (categoria == null) ? 0 : categoria.id,
                                                            ficha_tecnica = Cat_Productos.ficha_tecnica.ToUpper(),
                                                            poliza = (from c_producto in _context.Cliente_Productos
                                                                      join g in _context.rel_certificado_producto on c_producto.Id_Producto equals g.id_producto
                                                                      join h in _context.Cer_producto_cliente on g.id_certificado equals h.id
                                                                      where c_producto.Id_Cliente == Cat_Productos.id && h.id_cliente == zz.Id_Cliente
                                                                      select new
                                                                      {
                                                                          h.folio
                                                                      }),//zz.NoPoliza == "" ? "N/A" : zz.NoPoliza,
                                                            garantia = zz.FinGarantia.ToShortDateString(),
                                                            direccion = direccion.calle_numero + " " + localidad.desc_localidad.ToUpper(),
                                                            //cat_imagenes_producto = (from xz in _context.Cat_Imagenes_Producto
                                                            //                         where xz.id_producto == rel.id_producto
                                                            //                         select new
                                                            //                         {
                                                            //                             id = xz.id,
                                                            //                             url = xz.url
                                                            //                         }).Distinct().ToList()
                                                        }).Distinct().ToList(),
                                           tecnico = (from visita_tecnico in _context.rel_tecnico_visita
                                                          //join tv in _context.rel_tecnico_visita on visita_tecnico.id equals tv.id_vista
                                                      join i in _context.Tecnicos on visita_tecnico.id_tecnico equals i.id
                                                      join j in _context.Users on i.id equals j.id
                                                      where visita_tecnico.id_vista == x.id
                                                      select new
                                                      {
                                                          nombre = j.name.ToUpper() + " " + j.paterno.ToUpper() + " " + (j.materno == null ? "" : j.materno.ToUpper()),
                                                          visita_tecnico.tecnico_responsable
                                                      }),
                                           actividades_realizar = x.actividades_realizar.ToUpper(),
                                           cantidad = x.cantidad,
                                           comprobante = x.comprobante,
                                           concepto = x.concepto.ToUpper(),
                                           tipo_servicio = a.tipo_servicio.desc_tipo_servicio.ToUpper(),
                                           factura = x.factura,
                                           fecha_deposito = x.fecha_deposito,
                                           hora = x.hora,
                                           hora_inicio = Convert.ToDateTime(x.fecha_inicio_visita).ToString("hh:mm:ss"),
                                           fecha_visita = x.fecha_visita.ToShortDateString(),
                                           garantia = x.garantia,
                                           pagado = x.pagado,
                                           pago_pendiente = x.pago_pendiente,
                                           terminos_condiciones = x.terminos_condiciones,
                                           id_estatus = x.estatus,
                                           estatus_visita = (fee == null) ? "" : fee.desc_estatus_visita.ToUpper(),
                                           hora_fin = Convert.ToDateTime(x.fecha_fin_visita).ToString("hh:mm:ss"),
                                           //total_horas = Convert.ToDateTime(x.fecha_fin_visita).AddHours(-Convert.ToDateTime(x.fecha_inicio_visita).Hour),
                                           x.pre_diagnostico,
                                           x.asignacion_refacciones,
                                           x.latitud_inicio,
                                           x.longitud_inicio,
                                           x.latitud_fin,
                                           x.longitud_fin,
                                           x.fec_pago,
                                           pdf_reporte = x.url_ppdf_reporte.Replace("Imagenes", "mieletickets"),
                                           pdf_checklist = x.url_pdf_checklist.Replace("Imagenes", "mieletickets"),
                                           pdf_cotizacion = x.url_pdf_cotizacion.Replace("Imagenes", "mieletickets"),
                                           pdf_confirmacion_visita = x.url_pdf_confirmacion_visita == null ? "" : x.url_pdf_confirmacion_visita.Replace("Imagenes", "mieletickets"),
                                           //id_pago =String.IsNullOrEmpty(x.id_pago) ? "" : x.id_pago,
                                           x.id_pago,
                                           monto_pago = x.cantidad,
                                           x.succes_pago,
                                           checklist = (from w in _context.Producto_Check_List_Respuestas
                                                        join serie in _context.Rel_servicio_producto on w.id_vista equals serie.id_vista
                                                        join c_p in _context.Cliente_Productos on serie.id_producto equals c_p.Id
                                                        join prod in _context.Cat_Productos on c_p.Id_Producto equals prod.id
                                                        join ec in _context.Cat_Checklist_Producto on w.estatus equals ec.id
                                                        where w.id_vista == x.id //&& visita.id_servicio == id && serie.id_vista == id_visita
                                                        select new
                                                        {
                                                            estatus_prodcuto_checklist = ec.desc_checklist_producto,
                                                            prod.nombre,
                                                            prod.modelo,
                                                            c_p.no_serie,
                                                            serie.garantia,
                                                            imagenes_checklist = (from xz in _context.Rel_Imagen_Producto_Visita
                                                                                  where xz.id_visita == x.id && xz.id_producto == w.id_producto && xz.checklist == true
                                                                                  select new
                                                                                  {
                                                                                      id = xz.id,
                                                                                      url = xz.path,
                                                                                      xz.actividad
                                                                                  }).ToList(),
                                                            preguntas = (from check in _context.Producto_Check_List_Respuestas
                                                                         join respuesta in _context.Check_List_Respuestas on check.id equals respuesta.id_producto_check_list_respuestas
                                                                         join pregunta in _context.Check_List_Preguntas on respuesta.id_pregunta equals pregunta.id
                                                                         where check.id_vista == x.id && check.id_producto == c_p.Id_Producto //&& visita_c.id_servicio == id && check.id_producto == prod.id && check.id_vista == id_visita
                                                                         select new
                                                                         {
                                                                             pregunta = pregunta.pregunta,
                                                                             pregunta_en = pregunta.pregunta_en,
                                                                             respuesta = respuesta.respuesta,
                                                                             comentario = respuesta.comentarios,
                                                                         }).Distinct().ToList()
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
                                                          a.factura,
                                                          fecha_inicio = Convert.ToDateTime(a.fecha_inicio_visita).ToShortDateString(),
                                                          hora_inicio = Convert.ToDateTime(a.fecha_inicio_visita).ToShortTimeString(),
                                                          productos = (from xe in _context.Rel_servicio_Refaccion
                                                                       join c_p in _context.Cliente_Productos on xe.id_producto equals c_p.Id
                                                                       join sev in _context.CatEstatus_Producto on xe.estatus equals sev.id
                                                                       join prod in _context.Cat_Productos on c_p.Id_Producto equals prod.id
                                                                       join serie in _context.Rel_servicio_producto on c_p.Id equals serie.id_producto //&& serie.id_vista equals 
                                                                       where xe.id_vista == x.id && serie.id_vista == x.id
                                                                       select new
                                                                       {
                                                                           id = c_p.Id,
                                                                           id_rel = xe.id,
                                                                           id_producto = xe.id_producto,
                                                                           nombre_prodcuto = prod.nombre.ToUpper(),
                                                                           estatus_servicio = sev.desc_estatus_producto.ToUpper(),
                                                                           id_estatus = xe.estatus,
                                                                           //estatus = sev.desc_estatus_producto_en,
                                                                           actividades = xe.actividades.ToUpper(),
                                                                           fallas = xe.fallas.ToUpper(),
                                                                           no_serie = c_p.no_serie == null ? "" : c_p.no_serie,
                                                                           imagenes_reparacion = (from xz in _context.Rel_Imagen_Producto_Visita
                                                                                                  where xz.id_visita == x.id && xz.id_producto == xe.id_producto && xz.checklist == false
                                                                                                  select new
                                                                                                  {
                                                                                                      id = xz.id,
                                                                                                      url = xz.path,
                                                                                                      xz.actividad
                                                                                                  }).ToList(),
                                                                           piezas_repuesto = (from y in _context.Piezas_Repuesto
                                                                                              join u in _context.Cat_Materiales on y.id_material equals u.id
                                                                                              //join pre in _context.Cat_Lista_Precios on u. equals pre.id
                                                                                              //join pre in _context.Prediagnostico on a.id equals pre.id_visita
                                                                                              //join r in _context.Prediagnostico_Refacciones on pre.id equals r.id_prediagnostico
                                                                                              where y.id_rel_servicio_refaccion == xe.id //&& y.refacciones.id_vista == x.id
                                                                                              select new
                                                                                              {
                                                                                                  id = u.id,
                                                                                                  u.no_material,
                                                                                                  //r.estatus,
                                                                                                  refaccion = u.descripcion.ToUpper(),
                                                                                                  cantidad = y.cantidad,
                                                                                                  precio = u.grupo_precio.precio_sin_iva,
                                                                                              }).Distinct().ToList(),
                                                                           piezas_tecnico = (from y in _context.Piezas_Repuesto_Tecnico
                                                                                             join u in _context.Cat_Materiales on y.id_material equals u.id
                                                                                             where y.id_rel_servicio_refaccion == xe.id
                                                                                             select new
                                                                                             {
                                                                                                 id = u.id,
                                                                                                 refaccion = u.descripcion.ToUpper(),
                                                                                                 cantidad = y.cantidad
                                                                                             }).ToList()

                                                                       }).ToList()
                                                      }).Distinct().ToList()

                                       }).ToList(),
                            productos = (from rel in _context.Rel_servicio_producto
                                         join c_p in _context.Cliente_Productos on rel.id_producto equals c_p.Id
                                         join z in _context.Visita on rel.id_vista equals z.id
                                         join di in _context.Cat_Direccion on z.id_direccion equals di.id into _direccion
                                         from direccion in _direccion.DefaultIfEmpty()
                                         join col in _context.Cat_Localidad on direccion.id_localidad equals col.id into _localidad
                                         from localidad in _localidad.DefaultIfEmpty()
                                         join e in _context.Cat_Productos on c_p.Id_Producto equals e.id
                                         join f in _context.Cat_SubLinea_Producto on e.id_sublinea equals f.id
                                         join estatus in _context.CatEstatus_Producto on rel.estatus equals estatus.id into t
                                         from e_estatus in t.DefaultIfEmpty()
                                         where c_p.Id_Cliente == a.id_cliente && z.id_servicio == a.id && rel.primera_visita == true
                                         select new
                                         {
                                             id = c_p.Id,
                                             id_producto = e.id,
                                             id_servicio = z.id_servicio,
                                             sku = e.sku,
                                             modelo = (e.modelo == null ? "" : e.modelo),
                                             nombre = e.nombre.ToUpper(),
                                             no_serie = c_p.no_serie == null ? "" : c_p.no_serie,
                                             //no_visitas = (from count in _context.Visita
                                             //              where count.id == z.id
                                             //              select new
                                             //              {
                                             //                  count.id
                                             //              }).Distinct().Count(),
                                             descripcion_corta = e.descripcion_corta.ToUpper(),
                                             descripcion_larga = e.descripcion_larga.ToUpper(),
                                             atributos = e.atributos.ToUpper(),
                                             //precio_sin_iva = e.precio_sin_iva,
                                             //precio_con_iva = e.precio_con_iva,
                                             categoria = (f.descripcion == null) ? "" : f.descripcion.ToUpper(),
                                             id_categoria = (f.id == null) ? 0 : f.id,
                                             ficha_tecnica = e.ficha_tecnica.ToUpper(),
                                             poliza = (from c_producto in _context.Cliente_Productos
                                                       join g in _context.rel_certificado_producto on c_producto.Id_Producto equals g.id_producto
                                                       join h in _context.Cer_producto_cliente on g.id_certificado equals h.id
                                                       where c_producto.Id_Producto == e.id && h.id_cliente == c_p.Id_Cliente
                                                       select new
                                                       {
                                                           h.folio
                                                       }).Distinct(),//zz.NoPoliza == "" ? "N/A" : zz.NoPoliza,
                                             garantia = c_p.FinGarantia.ToShortDateString(),
                                             direccion = direccion.calle_numero + " " + direccion.numExt + ", " + (direccion.numInt == null ? "" : direccion.numInt) + ", " + localidad.desc_localidad.ToUpper() == null ? "N/A" : direccion.calle_numero + " " + localidad.desc_localidad.ToUpper(),
                                             //cat_imagenes_producto = (from xz in _context.Cat_Imagenes_Producto
                                             //                         where xz.id_producto == rel.id_producto
                                             //                         select new
                                             //                         {
                                             //                             id = xz.id,
                                             //                             url = xz.url
                                             //                         }).Distinct().ToList()
                                         }).Distinct().ToList()
                        }).Distinct().ToList();

            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/ServicoById
        [Route("ServicoById")]
        [HttpPost("{ServicoById}")]
        public IActionResult ServicoById([FromBody] tecnicobyid id)
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
                        where a.id == id.id
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
                            tipo_servicio = j.desc_tipo_servicio,
                            id_estatus = fe.id,
                            estatus_servicio = (fe == null) ? "" : fe.desc_estatus_servicio,
                            contacto = a.contacto,
                            IBS = a.IBS,
                            descripcion_actividades = a.descripcion_actividades,
                            visitas = (from x in _context.Visita
                                       join d in _context.Cat_Direccion on x.id_direccion equals d.id
                                       join e in _context.Cat_Estado on d.id_estado equals e.id
                                       join f in _context.Cat_Municipio on d.id_municipio equals f.id
                                       join lo in _context.Cat_Localidad on d.id_localidad equals lo.id
                                       join tv in _context.rel_tecnico_visita on x.id equals tv.id_vista
                                       join i in _context.Tecnicos on tv.id_tecnico equals i.id
                                       join j in _context.Users on i.id equals j.id
                                       join kk in _context.CatEstatus_Visita on x.estatus equals kk.id into t
                                       from fee in t.DefaultIfEmpty()
                                       where x.id_servicio == a.id
                                       select new
                                       {
                                           id = x.id,
                                           direccion = d.calle_numero + ", " + d.numExt + ", " + (d.numInt == null ? "" : d.numInt) + ", " + lo.desc_localidad + ", " + f.desc_municipio + ", " + d.cp + ", " + e.desc_estado,
                                           tecnico = j.name + " " + j.paterno + " " + (j.materno == null ? "" : j.materno),
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
                                           estatus = x.estatus,
                                           estatus_servicio = (fee == null) ? "" : fee.desc_estatus_visita,
                                           productos = (from x in _context.Rel_servicio_producto
                                                        join zz in _context.Cliente_Productos on x.id_producto equals zz.Id
                                                        join z in _context.Visita on x.id_vista equals z.id
                                                        join di in _context.Cat_Direccion on z.id_direccion equals di.id
                                                        join lo in _context.Cat_Localidad on di.id_localidad equals lo.id
                                                        join e in _context.Cat_Productos on zz.Id_Producto equals e.id
                                                        join f in _context.Cat_SubLinea_Producto on e.id_sublinea equals f.id
                                                        join g in _context.Cat_Linea_Producto on e.id_linea equals g.id
                                                        join h in _context.Cat_SubLinea_Producto on e.id_sublinea equals h.id
                                                        join kk in _context.CatEstatus_Producto on x.estatus equals kk.id into t
                                                        from fee in t.DefaultIfEmpty()
                                                        where z.id_servicio == a.id
                                                        select new
                                                        {
                                                            id = e.id,
                                                            id_servicio = z.id_servicio,
                                                            sku = e.sku,
                                                            modelo = e.modelo,
                                                            nombre = e.nombre,
                                                            estatus = e.estatus,
                                                            descripcion_corta = e.descripcion_corta,
                                                            descripcion_larga = e.descripcion_larga,
                                                            atributos = e.atributos,
                                                            precio_sin_iva = e.precio_sin_iva,
                                                            precio_con_iva = e.precio_con_iva,
                                                            categoria = f.descripcion,
                                                            linea = g.descripcion,
                                                            sublinea = h.descripcion,
                                                            ficha_tecnica = e.ficha_tecnica,
                                                            poliza = zz.NoPoliza == "" ? "N/A" : zz.NoPoliza,
                                                            garantia = zz.FinGarantia.ToShortDateString(),
                                                            direccion = di.calle_numero + " " + lo.desc_localidad,
                                                            cat_imagenes_producto = (from xz in _context.Cat_Imagenes_Producto
                                                                                     where xz.id_producto == x.id_producto
                                                                                     select new
                                                                                     {
                                                                                         id = xz.id,
                                                                                         url = xz.url
                                                                                     }).ToList()
                                                        }).ToList()
                                       }).ToList()
                        }).ToList();

            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/GetTecnicoById
        [Route("Servicios_TecnicoId")]
        [HttpPost("{Servicios_TecnicoId}")]
        public IActionResult Servicios_TecnicoId([FromBody] tecnicobyid id)
        {
            var item = (from a in _context.Servicio
                        join c in _context.Clientes on a.id_cliente equals c.id
                        join j in _context.Cat_tipo_servicio on a.id_tipo_servicio equals j.id
                        select new
                        {
                            id = a.id,
                            tipo_servicio = j.desc_tipo_servicio.ToUpper(),
                            cliente = c.nombre.ToUpper() + " " + c.paterno.ToUpper() + " " + (c.materno == null ? "" : c.materno.ToUpper()),
                            visitas = (from x in _context.Visita
                                       join d in _context.Cat_Direccion on x.id_direccion equals d.id
                                       join lo in _context.Cat_Localidad on d.id_localidad equals lo.id
                                       join e in _context.Cat_Estado on d.id_estado equals e.id
                                       join f in _context.Cat_Municipio on d.id_municipio equals f.id
                                       join tv in _context.rel_tecnico_visita on x.id equals tv.id_vista
                                       join et in _context.CatEstatus_Visita on x.estatus equals et.id into _ev
                                       from ev in _ev.DefaultIfEmpty()
                                       where x.id_servicio == a.id && tv.id_tecnico == id.id && tv.tecnico_responsable == true
                                       select new
                                       {
                                           id = x.id,
                                           id_estatus = x.estatus,
                                           estatus = ev.desc_estatus_visita.ToUpper(),
                                           //id_tecnico = tv.tecnico.users.name,
                                           direccion = d.calle_numero.ToUpper() + ", " + (d.numExt == null ? "" : d.numExt) + ", " + (d.numInt == null ? "" : d.numInt) + ", " + lo.desc_localidad.ToUpper() + ", " + f.desc_municipio.ToUpper() + ", " + d.cp + ", " + e.desc_estado.ToUpper(),
                                           fecha_visita = x.fecha_visita,
                                           hora = x.hora,
                                           tipo_servicio = a.tipo_servicio.desc_tipo_servicio.ToUpper(),
                                           tecnico = (from t_x in _context.rel_tecnico_visita
                                                      join t_j in _context.Users on t_x.id_tecnico equals t_j.id
                                                      where t_x.id_vista == x.id && t_x.tecnico_responsable == true
                                                      select new
                                                      {
                                                          id_tecnico = t_j.id,
                                                          nombre = t_j.name.ToUpper() + " " + t_j.paterno.ToUpper() + " " + (t_j.materno == null ? "" : t_j.materno.ToUpper()),
                                                          t_x.tecnico_responsable
                                                      }).ToList(),
                                           checklist = (from w in _context.Producto_Check_List_Respuestas
                                                        join ec in _context.Cat_Checklist_Producto on w.estatus equals ec.id into ec
                                                        from fee in ec.DefaultIfEmpty()
                                                        where w.visita.id_servicio == a.id
                                                        select new
                                                        {
                                                            id_visita = w.visita.id,
                                                            id_estatus_prodcuto_checklist = w.estatus,
                                                            estatus_prodcuto_checklist = fee.desc_checklist_producto.ToUpper(),
                                                            imagenes_checklist = (from xz in _context.Rel_Imagen_Producto_Visita
                                                                                  where xz.id_visita == w.id_vista && xz.id_producto == w.id_producto
                                                                                  select new
                                                                                  {
                                                                                      id = xz.id,
                                                                                      url = xz.path
                                                                                  }).Distinct().ToList(),
                                                            preguntas = (from check in _context.Producto_Check_List_Respuestas
                                                                         join visita_c in _context.Visita on check.id_vista equals visita_c.id
                                                                         join tv in _context.rel_tecnico_visita on visita_c.id equals tv.id_vista
                                                                         join respuesta in _context.Check_List_Respuestas on check.id equals respuesta.id_producto_check_list_respuestas
                                                                         join pregunta in _context.Check_List_Preguntas on respuesta.id_pregunta equals pregunta.id
                                                                         where tv.id_tecnico == id.id && visita_c.id_servicio == a.id
                                                                         select new
                                                                         {
                                                                             pregunta = pregunta.pregunta.ToUpper(),
                                                                             pregunta_en = pregunta.pregunta_en.ToUpper(),
                                                                             respuesta = respuesta.respuesta,
                                                                             comentario = respuesta.comentarios.ToUpper(),
                                                                         }).Distinct().ToList()
                                                        })

                                       }).ToList()
                        }).ToList(); //.Where(x => x.visitas[0].tecnico[0].id_tecnico == id.id && x.visitas[0].tecnico[0].tecnico_responsable == true).ToList();


            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/inicio_servicio
        [Route("Inicio_Servicio")]
        [HttpPost("{Inicio_Servicio}")]
        public IActionResult Inicio_Servicio([FromBody] inicio_servicio id)
        {

            var result = new Models.Response();

            try
            {
                Notificaciones Notificaciones = new Notificaciones();
                Notificaciones Notificaciones1 = new Notificaciones();
                var todo = _context.Visita.FirstOrDefault(t => t.id == id.id);
                var tecnico = (from a in _context.rel_tecnico_visita
                               join b in _context.Tecnicos on a.id_tecnico equals b.id
                               join c in _context.Users on b.id equals c.id
                               where a.id == id.id && a.tecnico_responsable == true
                               select new
                               {
                                   nombre = c.name + " " + c.paterno + " " + (c.materno == null ? "" : c.materno)
                               }).ToList();

                if (todo == null)
                {
                    return NotFound();
                }
                else
                {
                    TimeSpan hora = TimeSpan.FromHours(Convert.ToInt32(todo.hora));
                    TimeSpan fromTimeActual = DateTime.Now.TimeOfDay;

                    TimeSpan resta = fromTimeActual.Subtract(hora);

                    if (resta.TotalMinutes > 30.0)
                    {
                        Notificaciones1.evento = "Técnico fuera de horario";
                        Notificaciones1.rol_notificado = 10010;
                        Notificaciones1.descripcion = "El servicio con folio" + todo.id_servicio + "fue iniciado fuera del horario asignado";
                        Notificaciones1.estatus_leido = false;
                        Notificaciones1.creado = DateTime.Now;
                        Notificaciones1.creadopor = 0;
                        Notificaciones1.url = "editarservicio/" + todo.id_servicio;
                        _context.Notificaciones.Add(Notificaciones1);
                    }

                    todo.fecha_inicio_visita = DateTime.Now;
                    todo.latitud_inicio = id.latitud;
                    todo.longitud_inicio = id.longitud;
                    todo.estatus = 3;
                    Notificaciones.evento = "Visita, Agendado a En Visita";
                    Notificaciones.rol_notificado = 10010;
                    Notificaciones.descripcion = "El servicio con folio " + todo.id_servicio + "fue iniciado";
                    Notificaciones.estatus_leido = false;
                    Notificaciones.creado = DateTime.Now;
                    Notificaciones.creadopor = 0;
                    Notificaciones1.url = "editarservicio/" + todo.id_servicio;
                    _context.Notificaciones.Add(Notificaciones);
                    _context.Visita.Update(todo);
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

        [Route("Cerrar_Servicio")]
        [HttpPost("{Cerrar_Servicio}")]
        public IActionResult Cerrar_Servicio([FromBody] cerrar_servicio item)
        {

            IActionResult response;
            Notificaciones Notificaciones = new Notificaciones();
            try
            {
                var _servicio = _context.Servicio.FirstOrDefault(t => t.id == item.id_servicio);
                var visita = _context.Visita.FirstOrDefault(t => t.id == item.id_visita);
                //var cliente = _context.Cliente_Productos.FirstOrDefault(t => t.Id_Producto == item.servicio_refaccion[0].id_producto && t.id_vista == item.id_visita);

                if (visita == null)
                {
                    return NotFound();
                }
                else
                {
                    var refacciones_tecnico = (from a in _context.Visita
                                               join tv in _context.rel_tecnico_visita on a.id equals tv.id_vista
                                               where a.id == item.id_visita && tv.tecnico_responsable
                                               select new
                                               {
                                                   tv.id_tecnico
                                               }).Distinct().ToList();

                    for (int i = 0; i < item.servicio_refaccion.Count(); i++)
                    {
                        for (int j = 0; j < item.servicio_refaccion[i].piezas_repuesto_tecnico.Count(); j++)
                        {
                            var material_tecnico = _context.Cat_Materiales_Tecnico.FirstOrDefault(t => t.id_material == item.servicio_refaccion[i].piezas_repuesto_tecnico[j].id_material && t.id_tecnico == refacciones_tecnico[0].id_tecnico);

                            if (material_tecnico == null)
                            {
                                var nombre_material = _context.Cat_Materiales.FirstOrDefault(t => t.id == item.servicio_refaccion[i].piezas_repuesto_tecnico[j].id_material);
                                return response = Ok(new { item = "La refación " + nombre_material.descripcion + " no esta en el inventario del técnico" });
                            }
                            else
                            {
                                if (material_tecnico.cantidad < item.servicio_refaccion[i].piezas_repuesto_tecnico[j].cantidad)
                                {
                                    var nombre_material = _context.Cat_Materiales.FirstOrDefault(t => t.id == item.servicio_refaccion[i].piezas_repuesto_tecnico[j].id_material);
                                    return response = Ok(new { item = "La refación " + nombre_material.descripcion + " no tiene stock suficiente, el técnico solo tiene " + material_tecnico.cantidad + " piezas" });
                                }
                            }
                        }
                    }

                    //if (no_serie != null) {
                    //    no_serie.no_serie = item.no_serie;
                    //    _context.Rel_servicio_producto.Update(no_serie);
                    //}

                    //visita.imagen_firma = item.url;
                    visita.estatus = Convert.ToInt32(item.estatus);
                    visita.fecha_fin_visita = DateTime.Now;
                    //visita.latitud_fin = item.latitud;
                    //visita.longitud_fin = item.longitud;
                    visita.servicio_refaccion = item.servicio_refaccion;
                    for (int i = 0; i < item.servicio_productos.Count(); i++)
                    {
                        var s_p = _context.Rel_servicio_producto.FirstOrDefault(t => t.id_producto == item.servicio_productos[i].id_producto && t.id_vista == item.id_visita);
                        //var cliente_producto = _context.Cliente_Productos.FirstOrDefault(t => t.Id == item.servicio_productos[i].id_producto && t.Id_Cliente == _servicio.id_cliente);
                        var cliente_producto = _context.Cliente_Productos.FirstOrDefault(t => t.Id == item.servicio_productos[i].id_producto);
                        if (item.servicio_refaccion[i].estatus == 3)
                        {
                            Notificaciones.evento = "Producto pendiente por autorización";
                            Notificaciones.rol_notificado = 10008;
                            Notificaciones.descripcion = "El servicio con folio " + visita.id_servicio + " contiene el siguiente producto con estatus pendiente por autorizació";
                            Notificaciones.estatus_leido = false;
                            Notificaciones.creado = DateTime.Now;
                            Notificaciones.creadopor = 0;
                            Notificaciones.url = "editarservicio/" + visita.id_servicio;
                        }
                        cliente_producto.Id_EsatusCompra = item.servicio_refaccion[i].estatus;
                        cliente_producto.no_serie = item.servicio_productos[i].no_serie;
                        cliente_producto.Id_Producto = item.servicio_productos[i].id_producto_id_cat_producto == 0 ? cliente_producto.Id_Producto : item.servicio_productos[i].id_producto_id_cat_producto;
                        s_p.garantia = item.servicio_productos[i].garantia;
                        s_p.reparacion = item.servicio_productos[i].reparacion;
                        _context.Cliente_Productos.Update(cliente_producto);
                        _context.Rel_servicio_producto.Update(s_p);
                    }
                    //visita.servicio_producto = item.servicio_productos;
                    _context.Visita.Update(visita);
                    _context.SaveChanges();

                    return response = Ok(new { item = "OK" });
                }
            }
            catch (Exception ex)
            {
                return response = Ok(new { item = ex.ToString() });
            }
            //return new ObjectResult(result);
        }

        [Route("Cerrar_Servicio_Estatus")]
        [HttpPost("{Cerrar_Servicio_Estatus}")]
        public IActionResult Cerrar_Servicio_Estatus([FromBody] cerrar_servicio item)
        {
            Notificaciones Notificaciones = new Notificaciones();
            Notificaciones Notificaciones_1 = new Notificaciones();
            Notificaciones Notificaciones_2 = new Notificaciones();
            var result = new Models.Response();

            try
            {
                var _servicio = _context.Servicio.FirstOrDefault(t => t.id == item.id_servicio);
                var visita = _context.Visita.FirstOrDefault(t => t.id == item.id_visita);

                if (_servicio == null)
                {
                    return NotFound();
                }
                else
                {
                    var servicio = (from a in _context.Rel_servicio_Refaccion
                                    join c in _context.CatEstatus_Producto on a.estatus equals c.id
                                    where a.id_vista == item.id_visita
                                    select new
                                    {
                                        id = a.id,
                                        id_estatus = a.estatus,
                                        estatus = c.desc_estatus_producto
                                    }).Distinct().ToList();


                    bool _estatus_return = true;

                    if (servicio.Count() != 0)
                    {
                        var _estatus = servicio[0].id_estatus;
                        for (int i = 0; i < servicio.Count; i++)
                        {
                            if (_estatus == servicio[i].id_estatus)
                            {
                                _estatus_return = _estatus_return && true;
                            }
                            else
                            {
                                _estatus_return = _estatus_return && false;
                            }
                        }

                        if (_estatus_return)
                        {
                            if (servicio[0].id_estatus == 5)
                            {
                                _servicio.id_estatus_servicio = Convert.ToInt32(15);
                            }
                            else
                            {
                                if (servicio[0].id_estatus == 7)
                                {
                                    _servicio.id_estatus_servicio = Convert.ToInt32(15);
                                }
                                else
                                {
                                    if (servicio[0].id_estatus == 1006)
                                    {
                                        _servicio.id_estatus_servicio = Convert.ToInt32(15);
                                    }
                                    else
                                    {
                                        _servicio.id_estatus_servicio = 14;
                                    }
                                }
                            }
                        }
                        else
                        {
                            _servicio.id_estatus_servicio = 14;
                        }

                        var refacciones_tecnico = (from a in _context.Visita
                                                   join tv in _context.rel_tecnico_visita on a.id equals tv.id_vista
                                                   join c in _context.Rel_servicio_Refaccion on a.id equals c.id_vista
                                                   join d in _context.Piezas_Repuesto_Tecnico on c.id equals d.id_rel_servicio_refaccion
                                                   where a.id == item.id_visita && tv.tecnico_responsable == true
                                                   select new
                                                   {
                                                       d.id_material,
                                                       tv.id_tecnico,
                                                       d.cantidad
                                                   }).Distinct().ToList();

                        for (int i = 0; i < refacciones_tecnico.Count(); i++)
                        {
                            var material_tecnico = _context.Cat_Materiales_Tecnico.FirstOrDefault(t => t.id_material == refacciones_tecnico[i].id_material && t.id_tecnico == refacciones_tecnico[i].id_tecnico);
                            material_tecnico.cantidad = material_tecnico.cantidad - refacciones_tecnico[i].cantidad;

                            _context.Cat_Materiales_Tecnico.Update(material_tecnico);
                        }
                    }
                    else
                    {
                        _servicio.id_estatus_servicio = 14;
                    }

                    //if (_servicio.id_tipo_servicio == 1)
                    //{
                    //    visita.garantia = DateTime.Now.ToString("dd/MM/yyyy");
                    //}
                    if (_servicio.id_tipo_servicio == 5)
                    {
                        StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_inicio_servicio.html"));
                        string body = string.Empty;
                        body = reader.ReadToEnd();
                        body = body.Replace("{username}", "Hola," + " " + _servicio.cliente.nombre + " " + _servicio.cliente.paterno + " " + _servicio.cliente.materno);
                        body = body.Replace("{comentarios}", "El servicio con el siguiente número de folio: " + _servicio.id + ", esta terminado y su equipo listo para ser recogido en las instalciones de Miele");

                        //new SmtpClient
                        //{
                        //    Host = "Smtp.Gmail.com",
                        //    Port = 587,
                        //    EnableSsl = true,
                        //    Timeout = 10000,
                        //    DeliveryMethod = SmtpDeliveryMethod.Network,
                        //    UseDefaultCredentials = false,
                        //    Credentials = new NetworkCredential("conf modificada", "bostones01")
                        //}.Send(new MailMessage
                        //{
                        //    From = new MailAddress("no-reply@techo.org", "Miele"),
                        //    To = { _servicio.cliente.email },
                        //    Subject = "Servicio " + _servicio.id,
                        //    IsBodyHtml = true,
                        //    Body = body
                        //    //Body = "Nueva contraseña: " + "$miele" + guid, BodyEncoding = Encoding.UTF8 
                        //});

                        EmailModel email = new EmailModel();
                        email.To = _servicio.cliente.email;
                        email.Subject = "Servicio " + _servicio.id;
                        email.Body = body;
                        email.IsBodyHtml = true;
                        email.id_app = 1;
                        _emailRepository.SendMailAsync(email);
                    }

                    visita.imagen_firma = item.url;
                    visita.latitud_fin = item.latitud;
                    visita.longitud_fin = item.longitud;
                    visita.imagen_pago_referenciado = item.imagen_pago_referenciado;
                    visita.persona_recibe = item.persona_recibe;
                    Notificaciones.evento = "Servicio completado";
                    Notificaciones.rol_notificado = 10008;
                    Notificaciones.descripcion = "El servicio con folio " + visita.id_servicio + " termino, ahora puedes realizar una llamada para la encuesta de satisfacción";
                    Notificaciones.estatus_leido = false;
                    Notificaciones.creado = DateTime.Now;
                    Notificaciones.creadopor = 0;
                    Notificaciones.url = "serviciodetalle/" + _servicio.id_cliente;
                    Notificaciones_1.evento = "Servicio completado";
                    Notificaciones_1.rol_notificado = 10010;
                    Notificaciones_1.descripcion = "El servicio con folio " + visita.id_servicio + " termino, ahora puedes auditar el resultado de la visita";
                    Notificaciones_1.estatus_leido = false;
                    Notificaciones_1.creado = DateTime.Now;
                    Notificaciones_1.creadopor = 0;
                    Notificaciones_1.url = "editarservicio/" + visita.id_servicio;
                    if (_servicio.id_estatus_servicio == 15)
                    {
                        Notificaciones_2.evento = "Servicio completado";
                        Notificaciones_2.rol_notificado = 10005;
                        Notificaciones_2.descripcion = "El servicio con folio " + visita.id_servicio + " termino, ahora puedes hacer la aprovación final";
                        Notificaciones_2.estatus_leido = false;
                        Notificaciones_2.creado = DateTime.Now;
                        Notificaciones_2.creadopor = 0;
                        Notificaciones_2.url = "editarservicio/" + visita.id_servicio;
                        _context.Notificaciones.Add(Notificaciones_2);
                    };
                    _context.Notificaciones.Add(Notificaciones);
                    _context.Notificaciones.Add(Notificaciones_1);
                    _context.Visita.Update(visita);
                    _context.Servicio.Update(_servicio);
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

        [Route("Editar_Producto")]
        [HttpPost("{Editar_Producto}")]
        public IActionResult Editar_Producto([FromBody] Cat_Productos item)
        {
            var result = new Models.Response();

            try
            {
                var producto = _context.Cat_Productos.FirstOrDefault(t => t.id == item.id);

                if (producto == null)
                {
                    return NotFound();
                }
                else
                {
                    producto.id_sublinea = item.id_sublinea;
                    producto.modelo = item.modelo;
                    producto.no_serie = item.no_serie;

                    _context.Cat_Productos.Update(producto);
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

        [Route("Cerrar_Checklist")]
        [HttpPost("{Cerrar_Checklist}")]
        public IActionResult Cerrar_Checklist([FromBody] cerrar_checklist item)
        {
            var result = new Models.Response();

            try
            {
                var _servicio = _context.Servicio.FirstOrDefault(t => t.id == item.id_servicio);
                var visita = _context.Visita.FirstOrDefault(t => t.id == item.id_visita);

                if (visita == null)
                {
                    return NotFound();
                }
                else
                {
                    visita.estatus = Convert.ToInt32(item.estatus);
                    visita.producto_check_List_respuestas = item.producto;
                    for (int i = 0; i < item.producto.Count(); i++)
                    {
                        var cliente_producto = _context.Cliente_Productos.FirstOrDefault(t => t.Id == item.producto[i].id_producto && t.Id_Cliente == _servicio.id_cliente);
                        cliente_producto.Id_EsatusCompra = item.producto[i].estatus;
                        _context.Cliente_Productos.Update(cliente_producto);
                    }
                    _context.Visita.Update(visita);
                    //_context.SaveChanges();

                    var servicio = (from a in _context.Rel_servicio_Refaccion
                                    join c in _context.CatEstatus_Producto on a.estatus equals c.id
                                    where a.id_vista == item.id_visita
                                    select new
                                    {
                                        id = a.id,
                                        id_estatus = a.estatus,
                                        estatus = c.desc_estatus_producto
                                    }).Distinct().ToList();

                    if (servicio.Count != 0)
                    {
                        var _estatus = servicio[0].id_estatus;
                        bool _estatus_return = true;
                        for (int i = 0; i < servicio.Count; i++)
                        {
                            if (_estatus == servicio[i].id_estatus)
                            {
                                _estatus_return = _estatus_return && true;
                            }
                            else
                            {
                                _estatus_return = _estatus_return && false;
                            }
                        }

                        if (_estatus_return)
                        {
                            if (servicio[0].id_estatus == 5)
                            {
                                _servicio.id_estatus_servicio = Convert.ToInt32(19);
                            }
                            else
                            {
                                _servicio.id_estatus_servicio = 14;
                            }

                        }
                        else
                        {
                            _servicio.id_estatus_servicio = 14;
                        }

                    }

                    _context.Servicio.Update(_servicio);
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


        // POST: api/Servicios/Historial_servicio
        [Route("Historial_servicio")]
        [HttpPost("{Historial_servicio}")]
        public IActionResult Historial_servicio([FromBody] tecnicobyid id)
        {
            var item = (from a in _context.Visita
                        join c in _context.Servicio on a.id_servicio equals c.id
                        join e in _context.Cat_estatus_servicio on c.id_estatus_servicio equals e.id into es
                        from fd in es.DefaultIfEmpty()
                        where a.id == id.id
                        orderby a.fecha_visita ascending
                        select new
                        {
                            id_servicio = a.id_servicio,
                            estatus_servicio = (fd == null) ? "" : fd.desc_estatus_servicio.ToUpper(),
                            id_visita = a.id,
                            hora_inicio = a.hora,
                            hora_fin = a.hora_fin,
                            tipo_servicio = c.tipo_servicio.desc_tipo_servicio.ToUpper(),
                            ur_pdf_reporte = a.url_ppdf_reporte,
                            checklist = (from w in _context.Producto_Check_List_Respuestas
                                         join c_p in _context.Cliente_Productos on w.id_producto equals c_p.Id
                                         join visita in _context.Visita on w.id_vista equals visita.id
                                         join ec in _context.Cat_Checklist_Producto on w.estatus equals ec.id
                                         join prod in _context.Cat_Productos on c_p.Id_Producto equals prod.id
                                         join cate in _context.Cat_SubLinea_Producto on prod.id_sublinea equals cate.id
                                         where visita.id == id.id && visita.id_servicio == c.id
                                         select new
                                         {
                                             id = prod.id,
                                             nombre_prodcuto = prod.nombre.ToUpper(),
                                             categoria = cate.descripcion.ToUpper(),
                                             id_visita = c_p.Id,
                                             id_estatus_prodcuto_checklist = w.estatus,
                                             estatus_prodcuto_checklist = ec.desc_checklist_producto.ToUpper(),
                                             imagenes_checklist = (from xz in _context.Rel_Imagen_Producto_Visita
                                                                   where xz.id_visita == w.id_vista && xz.id_producto == w.id_producto
                                                                   select new
                                                                   {
                                                                       id = xz.id,
                                                                       url = xz.path
                                                                   }),
                                             preguntas = (from check in _context.Producto_Check_List_Respuestas
                                                          join visita_c in _context.Visita on check.id_vista equals visita_c.id
                                                          join respuesta in _context.Check_List_Respuestas on check.id equals respuesta.id_producto_check_list_respuestas
                                                          join pregunta in _context.Check_List_Preguntas on respuesta.id_pregunta equals pregunta.id
                                                          where visita_c.id == id.id && visita_c.id_servicio == c.id
                                                          select new
                                                          {
                                                              pregunta = pregunta.pregunta.ToUpper(),
                                                              pregunta_en = pregunta.pregunta_en.ToUpper(),
                                                              respuesta = respuesta.respuesta,
                                                              comentario = respuesta.comentarios.ToUpper(),
                                                          })
                                         }),
                            productos = (from x in _context.Rel_servicio_Refaccion
                                         join c_p in _context.Cliente_Productos on x.id_producto equals c_p.Id
                                         join sev in _context.CatEstatus_Producto on x.estatus equals sev.id
                                         join prod in _context.Cat_Productos on c_p.Id_Producto equals prod.id
                                         join z in _context.Rel_servicio_producto on x.id_producto equals z.id_producto
                                         where x.id_vista == id.id
                                         select new
                                         {
                                             id = prod.id,
                                             nombre_prodcuto = prod.nombre.ToUpper(),
                                             id_estatus = x.estatus,
                                             estatus = sev.desc_estatus_producto.ToUpper(),
                                             actividades = x.actividades.ToUpper(),
                                             fallas = x.fallas.ToUpper(),
                                             no_serie = c_p.no_serie == null ? "" : c_p.no_serie,
                                             categoria = prod.linea.descripcion.TrimEnd().ToUpper(),
                                             //z.no_serie,
                                             modelo = prod.modelo.ToUpper(),
                                             imagenes_reparacion = (from xz in _context.Rel_Imagen_Producto_Visita
                                                                    where xz.id_visita == id.id && xz.id_producto == x.id_producto
                                                                    select new
                                                                    {
                                                                        id = xz.id,
                                                                        url = xz.path
                                                                    }).ToList(),
                                             piezas_repuesto = (from y in _context.Piezas_Repuesto
                                                                join u in _context.Cat_Materiales on y.id_material equals u.id
                                                                where y.id_rel_servicio_refaccion == x.id
                                                                select new
                                                                {
                                                                    id = y.id,
                                                                    refaccion = u.descripcion.ToUpper(),
                                                                    cantidad = y.cantidad
                                                                }).ToList(),
                                             piezas_tecnico = (from y in _context.Piezas_Repuesto_Tecnico
                                                               join u in _context.Cat_Materiales on y.id_material equals u.id
                                                               where y.id_rel_servicio_refaccion == x.id
                                                               select new
                                                               {
                                                                   id = y.id,
                                                                   refaccion = u.descripcion.ToUpper(),
                                                                   cantidad = y.cantidad
                                                               }).ToList()

                                         }).ToList()
                        }).Distinct().ToList();

            if (item.Count == 0)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/CheckList
        [Route("CheckList")]
        [HttpPost("{CheckList}")]
        public IActionResult CheckList([FromBody] Cat_Productos sku)
        {
            var _sku = _context.Cat_Productos.FirstOrDefault(t => t.sku == sku.sku);
            var item = (from a in _context.Cat_Productos
                        join b in _context.Check_List_Preguntas on a.id_sublinea equals b.id_categoria
                        where b.id_categoria == Convert.ToInt32(_sku.id_sublinea)
                        select new
                        {
                            id_pregunta = b.id,
                            pregunta = b.pregunta,
                            pregunta_en = b.pregunta

                        }).Distinct().ToList();

            if (item.Count == 0)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/Actividades_Tecnicos
        [Route("Actividades_Tecnicos")]
        [HttpPost("{Actividades_Tecnicos}")]
        public IActionResult Actividades_Tecnicos([FromBody] Tecnicos tec)
        {
            //var myInClause = new int[] { 2, 5 };
            List<long> myInClause = new List<long>() { };
            for (int i = 0; i < tec.tecnicos_actividad.Count(); i++)
            {
                myInClause.Add(Convert.ToInt64(tec.tecnicos_actividad[i].id_actividad));
            }
            var item = ((from a in _context.Cat_tipo_servicio
                         join b in _context.Tecnicos_Actividad on a.id equals b.id_actividad
                         where
                           b.id_user == tec.id
                         select new
                         {
                             a.id,
                             a.desc_tipo_servicio,
                             a.estatus,
                             check = 1
                         }).Union
                        (from a in _context.Cat_tipo_servicio
                         where
                           !(myInClause).Contains(a.id) &&
                           a.estatus == true
                         orderby
                           a.id
                         select new
                         {
                             id = a.id,
                             desc_tipo_servicio = a.desc_tipo_servicio,
                             estatus = a.estatus,
                             check = 0
                         }

                )).OrderBy(p => p.id).ToList();

            if (item.Count == 0)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/CategoriaProducto_Tecnicos
        [Route("CategoriaProducto_Tecnicos")]
        [HttpPost("{CategoriaProducto_Tecnicos}")]
        public IActionResult CategoriaProducto_Tecnicos([FromBody] Tecnicos tec)
        {
            //var myInClause = new int[] { 2, 5 };
            List<long> myInClause = new List<long>() { };
            for (int i = 0; i < tec.tecnicos_producto.Count(); i++)
            {
                myInClause.Add(Convert.ToInt64(tec.tecnicos_producto[i].id_categoria_producto));
            }
            var item = ((from a in _context.Cat_SubLinea_Producto
                         join b in _context.Tecnicos_Producto on a.id equals b.id_categoria_producto
                         where
                           b.id_user == tec.id
                         select new
                         {
                             a.id,
                             a.descripcion,
                             //a.codigo,
                             a.estatus,
                             check = true
                         }).Union
                        (from a in _context.Cat_SubLinea_Producto
                         where
                           !(myInClause).Contains(a.id) &&
                           a.estatus == true
                         orderby
                           a.id
                         select new
                         {
                             a.id,
                             a.descripcion,
                             //a.codigo,
                             a.estatus,
                             check = false
                         }

                )).OrderBy(p => p.id).ToList();

            if (item.Count == 0)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/TecnicoInfo
        [Route("TecnicoInfo")]
        [HttpPost("{TecnicoInfo}")]
        public IActionResult TecnicoInfo([FromBody] tecnicobyid id)
        {
            var item = (from a in _context.Users
                        join b in _context.Tecnicos on a.id equals b.id
                        join c in _context.Cat_Tecnicos_Tipo on b.id_tipo_tecnico equals c.id
                        where b.id == id.id
                        select new
                        {
                            id = a.id,
                            username = a.username,
                            name = a.name,
                            paterno = a.paterno,
                            materno = a.materno,
                            password = a.password,
                            email = a.email,
                            telefono = a.telefono == null ? "" : a.telefono,
                            movil = a.telefono_movil == null ? "" : a.telefono,
                            tecnicos_actividad = (from x in _context.Tecnicos_Actividad
                                                  join j in _context.Cat_tipo_servicio on x.id_actividad equals j.id
                                                  where x.id_user == a.id
                                                  select new
                                                  {
                                                      id = x.id,
                                                      id_actividad = j.id,
                                                      id_user = x.id_user,
                                                      desc_actividad = j.desc_tipo_servicio
                                                  }).ToList(),
                            area_cobertura = (from x in _context.Tecnicos_Cobertura
                                              join j in _context.Cat_Area_Cobertura on x.id_cobertura equals j.id
                                              where x.id_user == a.id
                                              select new
                                              {
                                                  id = x.id,
                                                  cobertura = j.desc_cobertura
                                              }).ToList(),
                            tecnicos_producto = (from x in _context.Tecnicos_Producto
                                                 join j in _context.Cat_SubLinea_Producto on x.id_categoria_producto equals j.id
                                                 where x.id_user == a.id
                                                 select new
                                                 {
                                                     x.id,
                                                     id_categoria_producto = j.id,
                                                     id_user = x.id_user
                                                 }).ToList(),
                            noalmacen = b.noalmacen,
                            tipo = c.desc_tipo
                        }).ToList();

            if (item.Count == 0)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/Sepomex
        [Route("Sepomex")]
        [HttpPost("{Sepomex}")]
        public IActionResult Sepomex([FromBody] tecnicobyid item)
        {
            var _item = (from a in _context.Cat_Localidad
                         join b in _context.Cat_Municipio on a.municipio.id equals b.id
                         join c in _context.Cat_Estado on b.estado.id equals c.id
                         where a.cp == item.id
                         select new
                         {
                             cp = a.cp,
                             estado = c.desc_estado,
                             id_estado = c.id,
                             municipio = b.desc_municipio,
                             id_municipio = b.id,
                             localidades = (from a in _context.Cat_Localidad
                                            join b in _context.Cat_Municipio on a.municipio.id equals b.id
                                            join c in _context.Cat_Estado on b.estado.id equals c.id
                                            where a.cp == item.id
                                            select new
                                            {
                                                id_localidad = a.id,
                                                localidad = a.desc_localidad,
                                            }).Distinct().ToList()

                         }).Distinct().Take(1).ToList();

            if (_item.Count == 0)
            {
                return NotFound();
            }
            return new ObjectResult(_item);
        }

        //// POST: api/Servicios/Respuestas
        //[Route("Respuestas")]
        //[HttpPost("{Respuestas}")]
        //public IActionResult Respuestas([FromBody] Check_List_Preguntas pregunta)
        //{
        //    var result = new Models.Response();
        //    var chk = _context.Check_List_Preguntas.FirstOrDefault(t => t.id == pregunta.id);

        //    try
        //    {
        //        chk.Respuestas = pregunta.Respuestas;
        //        _context.Check_List_Preguntas.Update(chk);
        //        _context.SaveChanges();

        //        result = new Models.Response
        //        {
        //            response = "OK"
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        result = new Models.Response
        //        {
        //            response = ex.ToString()
        //        };
        //    }
        //    return new ObjectResult(result);
        //}

        // POST: api/Servicios/GetTecnicoById
        [Route("Busqueda_Servicio")]
        [HttpPost("{Busqueda_Servicio}")]
        public IActionResult Busqueda_Servicio([FromBody] busqueda busqueda)
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

                    var item = (from a in _context.Servicio
                                join c in _context.Clientes on a.id_cliente equals c.id
                                join z in _context.Visita on a.id equals z.id_servicio into _visita
                                from visita in _visita.DefaultIfEmpty()
                                join tv in _context.rel_tecnico_visita on visita.id equals tv.id_vista into _rel_tecnico_visita
                                from rel_tecnico_visita in _rel_tecnico_visita.DefaultIfEmpty()
                                join d in _context.Cat_Direccion on visita.id_direccion equals d.id into _Cat_Direccion
                                from direccion in _Cat_Direccion.DefaultIfEmpty()
                                join e in _context.Cat_Estado on direccion.id_estado equals e.id into _Cat_Estado
                                from estado in _Cat_Estado.DefaultIfEmpty()
                                join f in _context.Cat_Municipio on direccion.id_municipio equals f.id into _Cat_Municipio
                                from municipio in _Cat_Municipio.DefaultIfEmpty()
                                join k in _context.Cat_estatus_servicio on a.id_estatus_servicio equals k.id
                                where c.tipo_cliente == 1
                                //where EF.Functions.Like(a.no_servicio + ' ' + c.telefono + ' ' + (direccion.calle_numero + estado.desc_estado + municipio.desc_municipio == null ? "" : direccion.calle_numero + estado.desc_estado + municipio.desc_municipio) + ' ' + c.nombre + c.paterno + c.materno, "%" + busqueda.desc_busqueda + "%")
                                ////where a.no_servicio == busqueda.desc_busqueda
                                //&& (a.fecha_servicio.Date >= _fecha_servicio_inicio.Date && a.fecha_servicio.Date <= _fecha_servicio_fin.Date)
                                //&& (a.creado.Date >= _fecha_creacion_inicio.Date && a.creado.Date <= _fecha_creacion_fin.Date)
                                //&& rel_tecnico_visita.id_tecnico == (busqueda.tecnico == 0 ? rel_tecnico_visita.id_tecnico : busqueda.tecnico)
                                orderby a.creado descending
                                select new
                                {
                                    id_servicio = a.id,
                                    //id_vista = z.id,
                                    numero_servicio = a.no_servicio,
                                    ibs = a.IBS,
                                    cliente = c.nombre + " " + c.paterno + " " + (c.materno == null ? "" : c.materno),
                                    estatus_servicio = k.desc_estatus_servicio,
                                    fecha_servicio = visita.fecha_visita.ToShortDateString(),
                                    fecha_ser = visita.fecha_visita,
                                    fecha_creacion = a.creado,
                                    a.no_servicio,
                                    telefono = c.telefono == null ? "0" : c.telefono,
                                    direccion.calle_numero,
                                    estado.desc_estado,
                                    municipio.desc_municipio,
                                    c.nombre,
                                    c.paterno,
                                    c.materno
                                }).Distinct();

                    if (busqueda.desc_busqueda != "")
                    {
                        item = item.Where(x => EF.Functions.Like(x.no_servicio + ' ' + x.telefono + ' ' + (x.calle_numero + x.desc_estado + x.desc_municipio == null ? "" : x.calle_numero + x.desc_estado + x.desc_municipio) + ' ' + x.nombre + x.paterno + x.materno, "%" + busqueda.desc_busqueda + "%"));
                    }

                    if (busqueda.fecha_creacion_fin.Date != Convert.ToDateTime("01/01/1900").Date)
                    {
                        item = item.Where(x => x.fecha_creacion.Date >= busqueda.fecha_creacion_inicio.Date && x.fecha_creacion.Date <= busqueda.fecha_creacion_fin.Date);
                    }

                    if (busqueda.fecha_servicio_inicio.Date != Convert.ToDateTime("01/01/1900").Date)
                    {
                        item = item.Where(x => x.fecha_ser.Date >= busqueda.fecha_servicio_inicio.Date && x.fecha_ser.Date <= busqueda.fecha_servicio_fin.Date);
                    }

                    if (item.Count() == 0)
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

        // POST: api/Servicios/Busqueda_Servicio_ultimos
        [Route("Busqueda_Servicio_ultimos")]
        [HttpPost("{Busqueda_Servicio_ultimos}")]
        public IActionResult Busqueda_Servicio_ultimos([FromBody] busqueda busqueda)
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
                    DateTime _fecha_servicio_inicio = Convert.ToDateTime("01/01/1900");
                    DateTime _fecha_servicio_fin = Convert.ToDateTime("01/01/3000");

                    DateTime _fecha_creacion_inicio = Convert.ToDateTime("01/01/1900");
                    DateTime _fecha_creacion_fin = Convert.ToDateTime("01/01/3000");

                    if (busqueda.fecha_servicio_inicio != Convert.ToDateTime("01/01/1900"))
                    {
                        _fecha_servicio_inicio = busqueda.fecha_servicio_inicio;
                    }
                    if (busqueda.fecha_servicio_fin != Convert.ToDateTime("01/01/1900"))
                    {
                        _fecha_servicio_fin = busqueda.fecha_servicio_fin;
                    }

                    if (busqueda.fecha_creacion_inicio != Convert.ToDateTime("01/01/1900"))
                    {
                        _fecha_creacion_inicio = busqueda.fecha_creacion_inicio;
                    }
                    if (busqueda.fecha_creacion_fin != Convert.ToDateTime("01/01/1900"))
                    {
                        _fecha_creacion_fin = busqueda.fecha_creacion_fin;
                    }

                    var item = (from a in _context.Servicio
                                join c in _context.Clientes on a.id_cliente equals c.id
                                join z in _context.Visita on a.id equals z.id_servicio
                                join tv in _context.rel_tecnico_visita on z.id equals tv.id_vista
                                join d in _context.Cat_Direccion on z.id_direccion equals d.id
                                join e in _context.Cat_Estado on d.id_estado equals e.id
                                join f in _context.Cat_Municipio on d.id_municipio equals f.id
                                join k in _context.Cat_estatus_servicio on a.id_estatus_servicio equals k.id
                                where //EF.Functions.Like(a.no_servicio + ' ' + c.telefono + ' ' + d.calle_numero + d.colonia + e.desc_estado + f.desc_municipio + ' ' + c.nombre + c.paterno + c.materno, "%" + busqueda.desc_busqueda + "%")
                                      //where a.no_servicio == busqueda.desc_busqueda
                                (a.fecha_servicio >= _fecha_servicio_inicio && a.fecha_servicio <= _fecha_servicio_fin)
                                && (a.creado >= _fecha_creacion_inicio && a.creado <= _fecha_creacion_fin)
                                && tv.id_tecnico == (busqueda.tecnico == 0 ? tv.id_tecnico : busqueda.tecnico) && z.pre_diagnostico == false
                                orderby z.fecha_visita descending
                                select new
                                {
                                    id_servicio = a.id,
                                    //id_vista = z.id,
                                    numero_servicio = a.no_servicio,
                                    ibs = a.IBS,
                                    cliente = c.nombre + " " + c.paterno + " " + c.materno,
                                    estatus_servicio = k.desc_estatus_servicio,
                                    fecha_servicio = z.fecha_visita.ToShortDateString(),
                                    pre_diagnostico = z.pre_diagnostico,
                                    z.asignacion_refacciones
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

        // POST: api/Servicios/Busqueda_Servicio_ultimos_field_services
        [Route("Busqueda_Servicio_ultimos_field_services")]
        [HttpPost("{Busqueda_Servicio_ultimos_field_services}")]
        public IActionResult Busqueda_Servicio_ultimos_field_services([FromBody] busqueda busqueda)
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
                    DateTime _fecha_servicio_inicio = Convert.ToDateTime("01/01/1900");
                    DateTime _fecha_servicio_fin = Convert.ToDateTime("01/01/3000");

                    DateTime _fecha_creacion_inicio = Convert.ToDateTime("01/01/1900");
                    DateTime _fecha_creacion_fin = Convert.ToDateTime("01/01/3000");

                    if (busqueda.fecha_servicio_inicio != Convert.ToDateTime("01/01/1900"))
                    {
                        _fecha_servicio_inicio = busqueda.fecha_servicio_inicio;
                    }
                    if (busqueda.fecha_servicio_fin != Convert.ToDateTime("01/01/1900"))
                    {
                        _fecha_servicio_fin = busqueda.fecha_servicio_fin;
                    }

                    if (busqueda.fecha_creacion_inicio != Convert.ToDateTime("01/01/1900"))
                    {
                        _fecha_creacion_inicio = busqueda.fecha_creacion_inicio;
                    }
                    if (busqueda.fecha_creacion_fin != Convert.ToDateTime("01/01/1900"))
                    {
                        _fecha_creacion_fin = busqueda.fecha_creacion_fin;
                    }

                    var item = (from a in _context.Servicio
                                join c in _context.Clientes on a.id_cliente equals c.id
                                join z in _context.Visita on a.id equals z.id_servicio
                                join tv in _context.rel_tecnico_visita on z.id equals tv.id_vista
                                join d in _context.Cat_Direccion on z.id_direccion equals d.id
                                join e in _context.Cat_Estado on d.id_estado equals e.id
                                join f in _context.Cat_Municipio on d.id_municipio equals f.id
                                join k in _context.Cat_estatus_servicio on a.id_estatus_servicio equals k.id
                                where //EF.Functions.Like(a.no_servicio + ' ' + c.telefono + ' ' + d.calle_numero + d.colonia + e.desc_estado + f.desc_municipio + ' ' + c.nombre + c.paterno + c.materno, "%" + busqueda.desc_busqueda + "%")
                                      //where a.no_servicio == busqueda.desc_busqueda
                                (a.fecha_servicio >= _fecha_servicio_inicio && a.fecha_servicio <= _fecha_servicio_fin)
                                && (a.creado >= _fecha_creacion_inicio && a.creado <= _fecha_creacion_fin)
                                && tv.id_tecnico == (busqueda.tecnico == 0 ? tv.id_tecnico : busqueda.tecnico) && z.asignacion_refacciones == false
                                orderby z.fecha_visita descending
                                select new
                                {
                                    id_servicio = a.id,
                                    //id_vista = z.id,
                                    numero_servicio = a.no_servicio,
                                    ibs = a.IBS,
                                    cliente = c.nombre + " " + c.paterno + " " + c.materno,
                                    estatus_servicio = k.desc_estatus_servicio,
                                    fecha_servicio = z.fecha_visita.ToShortDateString(),
                                    pre_diagnostico = z.pre_diagnostico,
                                    z.asignacion_refacciones
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

        // POST: api/Servicios/GetTecnicoById
        [Route("Refacciones_Tecnico")]
        [HttpPost("{Refacciones_Tecnico}")]
        public IActionResult Refacciones_Tecnico([FromBody] tecnicobyid item)
        {
            IActionResult response;

            try
            {
                if (item == null)
                {
                    return NotFound();
                }
                else
                {

                    var _item = (from a in _context.Cat_Materiales_Tecnico
                                 join b in _context.Cat_Materiales on a.id_material equals b.id
                                 where a.id_tecnico == item.id && a.cantidad > 0
                                 group new { a, b } by a.id_material into gb
                                 select new
                                 {
                                     id = gb.First().b.id,
                                     cantidad = gb.Sum(s => s.a.cantidad),
                                     descripcion = gb.First().b.descripcion,
                                     no_material = gb.First().b.no_material
                                 }).ToList();

                    if (_item.Count == 0)
                    {
                        return response = Ok(new { item = "No hay resultado para este técnico" });
                    }
                    return new ObjectResult(_item);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }

        // POST: api/Servicios/Producto_Categoria_Info
        [Route("Producto_Categoria_Info")]
        [HttpPost("{Producto_Categoria_Info}")]
        public IActionResult Producto_Categoria_Info([FromBody] Productos_Servicio item)
        {
            IActionResult response;
            List<int> myInClause = new List<int>() { 14, 15 };
            try
            {
                if (item == null)
                {
                    return NotFound();
                }
                else
                {
                    var _item = (from a in _context.Cliente_Productos
                                 join b in _context.Cat_Productos on a.Id_Producto equals b.id
                                 join d in _context.Rel_Categoria_Producto_Tipo_Producto on b.id_sublinea equals d.id_categoria
                                 where d.id_tipo_servicio == item.id_tipo && a.Id == item.id_cliente && !myInClause.Contains(b.id_linea.Value) && d.horas_tecnicos != 0.ToString()
                                 select new
                                 {
                                     id = b.id,
                                     no_tecnicos = d.no_tecnicos,
                                     horas_tecnico = d.horas_tecnicos,
                                     d.precio_hora_tecnico,
                                     d.precio_visita
                                 }).Distinct().ToList();

                    if (_item.Count == 0)
                    {
                        return response = Ok(new { item = "No hay resultado" });
                    }
                    return new ObjectResult(_item);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }

        // POST: api/Servicios/Productos_Servicio_Solicitado
        [Route("Productos_Servicio_Solicitado")]
        [HttpPost("{Productos_Servicio_Solicitado}")]
        public IActionResult Productos_Servicio_Solicitado([FromBody] Productos_Servicio producto)
        {
            List<long> myInClause = new List<long>() { 1009, 1010 };
            var item = (from a in _context.Cliente_Productos
                        join b in _context.Cat_Productos on a.Id_Producto equals b.id
                        join rel in _context.Rel_Categoria_Producto_Tipo_Producto on b.id_sublinea equals rel.id_categoria into _Rel_Categoria_Producto_Tipo_Producto
                        from Rel_Categoria_Producto_Tipo_Producto in _Rel_Categoria_Producto_Tipo_Producto.DefaultIfEmpty()
                            //join visita in _context.Visita on a.Id_Cliente equals visita.servicio.id_cliente into _visita
                            //from vis in _visita.DefaultIfEmpty()
                        join c in _context.CatEstatus_Producto on a.Id_EsatusCompra equals c.id
                        //join rel_pro in _context.Rel_servicio_producto on vis.id equals rel_pro.id_vista
                        where a.Id_Cliente == producto.id_cliente && Rel_Categoria_Producto_Tipo_Producto.id_tipo_servicio == producto.id_tipo && !myInClause.Contains(a.Id_EsatusCompra)//&& rel_pro.primera_visita == true && rel_pro.id_producto == a.Id_Producto && rel_pro.id_vista == vis.id
                        orderby a.FechaCompra ascending
                        select new
                        {
                            a.Id,
                            id_prodcuto = b.id,
                            modelo = b.modelo,
                            sku = b.sku,
                            //no_visitas = (vis == null) ? 0 : (from count in _context.Rel_servicio_producto
                            //                                  where count.id_vista == vis.id && count.id_producto == a.Id_Producto
                            //                                  select new
                            //                                  {
                            //                                      count.id
                            //                                  }).Count(),
                            tipo_equipo = b.nombre,
                            id_categoria = b.id_sublinea,
                            garantia = a.FinGarantia.ToShortDateString(),
                            poliza = a.NoPoliza,
                            estatus = c.desc_estatus_producto,
                            id_estutus = c.id,
                            hora_tecnico = Rel_Categoria_Producto_Tipo_Producto.horas_tecnicos == "" ? "0" : Rel_Categoria_Producto_Tipo_Producto.horas_tecnicos,
                            precio_hora = Rel_Categoria_Producto_Tipo_Producto.precio_hora_tecnico == null ? 0 : Rel_Categoria_Producto_Tipo_Producto.precio_hora_tecnico,
                            precio_visita = Rel_Categoria_Producto_Tipo_Producto.precio_visita == null ? 0 : Rel_Categoria_Producto_Tipo_Producto.precio_visita,
                            no_tecnico = Rel_Categoria_Producto_Tipo_Producto.no_tecnicos == null ? 0 : Rel_Categoria_Producto_Tipo_Producto.no_tecnicos

                        }).Distinct().ToList();

            if (item.Count == 0)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/Productos_Servicio_Solicitado_cliente
        [Route("Productos_Servicio_Solicitado_cliente")]
        [HttpPost("{Productos_Servicio_Solicitado_cliente}")]
        public IActionResult Productos_Servicio_Solicitado_cliente([FromBody] Productos_Servicio producto)
        {
            var item = (from a in _context.Cliente_Productos
                        join b in _context.Cat_Productos on a.Id_Producto equals b.id
                        join visita in _context.Visita on a.Id_Cliente equals visita.servicio.id_cliente into _visita
                        from vis in _visita.DefaultIfEmpty()
                        join c in _context.CatEstatus_Producto on a.Id_EsatusCompra equals c.id into _CatEstatus_Producto
                        from CatEstatus_Producto in _CatEstatus_Producto.DefaultIfEmpty()
                            //join rel_pro in _context.Rel_servicio_producto on vis.id equals rel_pro.id_vista
                        where a.Id_Cliente == producto.id_cliente &&
                        a.Id_Producto == producto.id_tipo //&& !myInClause.Contains(a.Id_EsatusCompra)//&& rel_pro.primera_visita == true && rel_pro.id_producto == a.Id_Producto && rel_pro.id_vista == vis.id
                        orderby a.FechaCompra ascending
                        select new
                        {
                            id = b.id,
                            modelo = b.modelo,
                            sku = b.sku,
                            no_visitas = (vis == null) ? 0 : (from count in _context.Visita
                                                              where count.id == vis.id
                                                              select new
                                                              {
                                                                  count.id
                                                              }).Count(),
                            tipo_equipo = b.nombre,
                            id_categoria = b.id_sublinea,
                            garantia = a.FinGarantia.ToShortDateString(),
                            poliza = a.NoPoliza,
                            estatus = CatEstatus_Producto.desc_estatus_producto,
                            id_estutus = CatEstatus_Producto.id

                        }).Distinct().ToList();

            if (item.Count == 0)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/Productos_Servicio_Detalle
        [Route("Productos_Servicio_Detalle")]
        [HttpPost("{Productos_Servicio_Detalle}")]
        public IActionResult Productos_Servicio_Detalle([FromBody] Productos_Servicio producto)
        {
            var myInClause = new int[] { 1, 4 };
            var item = ((from a in _context.Clientes
                         join b in _context.Cliente_Productos on a.id equals b.Id_Cliente
                         join c in _context.Cat_Productos on b.Id_Producto equals c.id
                         join d in _context.CatEstatus_Producto on b.Id_EsatusCompra equals d.id
                         //join e in _context.rel_certificado_producto on b.Id_Producto equals e.id_producto
                         //join f in _context.Cer_producto_cliente on e.id_certificado equals f.id
                         where a.id == producto.id_cliente //&& myInClause.Contains(c.tipo) //c.tipo == producto.id_cliente
                         select new
                         {
                             id = b.Id,
                             id_producto = c.id,
                             id_estatus = d.id,
                             modelo = c.modelo,
                             sku = c.sku,
                             garantia = b.FinGarantia.ToShortDateString(),
                             poliza = (from e in _context.Clientes
                                       join f in _context.Cliente_Productos on e.id equals f.Id_Cliente
                                       join g in _context.rel_certificado_producto on f.Id_Producto equals g.id_producto
                                       join h in _context.Cer_producto_cliente on g.id_certificado equals h.id
                                       where e.id == producto.id_cliente && h.id_cliente == producto.id_cliente && f.Id_Producto == b.Id_Producto
                                       select new
                                       {
                                           h.folio
                                       }).ToList(),
                             estatus = d.desc_estatus_producto,
                             c.nombre,
                             c.id_sublinea

                         })).ToList();
            //.Union(
            //from a in _context.Cotizaciones
            //join b in _context.Cotizacion_Producto on a.Id equals b.Id_Cotizacion
            //join c in _context.Cat_Estatus_Cotizacion on a.Estatus equals c.id
            //join d in _context.Cat_Productos on b.Id_Producto equals d.id
            //where a.Id_Cliente == producto.id_cliente && myInClause.Contains(c.id)
            ////orderby a.FechaCompra ascending
            //select new
            //{
            //    id = d.id,
            //    id_estatus = c.id,
            //    modelo = d.modelo,
            //    sku = d.sku,
            //    garantia = "01/01/1900",
            //    poliza = (from a in _context.Clientes
            //              join b in _context.Cliente_Productos on a.id equals b.Id_Cliente
            //              join c in _context.Cat_Productos on b.Id_Producto equals c.id
            //              join d in _context.CatEstatus_Producto on b.Id_EsatusCompra equals d.id
            //              join e in _context.rel_certificado_producto on b.Id_Producto equals e.id_producto
            //              join f in _context.Cer_producto_cliente on e.id_certificado equals f.id
            //              where a.id == producto.id_cliente && f.id_cliente == producto.id_cliente
            //              select new
            //              {
            //                  folio = f.folio == null ? "N/A" : f.folio
            //              }).ToList(),
            //    estatus = c.Estatus_es,
            //    d.nombre,
            //    d.id_sublinea

            //})).ToList();


            if (item.Count == 0)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/Productos_Servicio_Perfil
        [Route("Productos_Servicio_Perfil")]
        [HttpPost("{Productos_Servicio_Perfil}")]
        public IActionResult Productos_Servicio_Perfil([FromBody] Productos_Servicio producto)
        {
            var myInClause = new int[] { 3, 4 };
            var item = ((from a in _context.Cliente_Productos
                         join c in _context.Cat_Productos on a.Id_Producto equals c.id
                         join d in _context.CatEstatus_Producto on a.Id_EsatusCompra equals d.id into _CatEstatus_Producto
                         from CatEstatus_Producto in _CatEstatus_Producto.DefaultIfEmpty()
                             //from rel_certificado_producto in _rel_certificado_producto.DefaultIfEmpty()
                             //join e in _context.rel_certificado_producto on a.Id equals e.id_producto into _rel_certificado_producto
                             //from rel_certificado_producto in _rel_certificado_producto.DefaultIfEmpty()
                             //join f in _context.Cer_producto_cliente on rel_certificado_producto.id_certificado equals f.id
                         where a.Id_Cliente == producto.id_cliente //&& f.id_cliente == producto.id_cliente
                         select new
                         {
                             id_cliente_producto = Convert.ToInt32(a.Id),
                             id = c.id,
                             id_estatus = CatEstatus_Producto.id == null ? 0 : CatEstatus_Producto.id,
                             modelo = c.modelo == null ? "S/M" : c.modelo,
                             sku = c.sku,
                             garantia = a.FinGarantia.ToShortDateString(),
                             //poliza = f.folio,
                             poliza = (from e in _context.Cer_producto_cliente
                                       join g in _context.rel_certificado_producto on e.id equals g.id_certificado
                                       where e.id_cliente == producto.id_cliente && g.id_producto == a.Id
                                       select new
                                       {
                                           folio = e.folio == null ? "N/A" : e.folio
                                       }).Distinct(),
                             estatus = CatEstatus_Producto.desc_estatus_producto == "0" ? "S/E" : CatEstatus_Producto.desc_estatus_producto,
                             c.nombre,
                             c.id_sublinea

                         })
            //.Union(
            //from a in _context.Cotizaciones
            //join b in _context.Cotizacion_Producto on a.Id equals b.Id_Cotizacion
            //join c in _context.Cat_Estatus_Cotizacion on a.Estatus equals c.id
            //join d in _context.Cat_Productos on b.Id_Producto equals d.id
            //where a.Id_Cliente == producto.id_cliente && myInClause.Contains(c.id)
            ////orderby a.FechaCompra ascending
            //select new
            //{
            //    id_cliente_producto = 0,
            //    id = d.id,
            //    id_estatus = c.id,
            //    modelo = d.modelo,
            //    sku = d.sku,
            //    garantia = "01/01/1900",
            //    //poliza = "",
            //    poliza = (from e in _context.Cliente_Productos
            //              join g in _context.rel_certificado_producto on e.Id equals g.id_producto
            //              join h in _context.Cer_producto_cliente on g.id_certificado equals h.id
            //              where e.Id_Cliente == producto.id_cliente
            //              select new
            //              {
            //                  folio = h.folio == null ? "N/A" : h.folio
            //              }).Distinct(),
            //    estatus = c.Estatus_es,
            //    d.nombre,
            //    d.id_sublinea

            //})
            ).ToList();

            return new ObjectResult(item);
        }

        // POST: api/Servicios/Direcciones_cliente
        [Route("Direcciones_cliente")]
        [HttpPost("{Direcciones_cliente}")]
        public IActionResult Direcciones_cliente([FromBody] tecnicobyid id)
        {
            var item = (from a in _context.Cat_Direccion
                        join b in _context.Clientes on a.id_cliente equals b.id
                        join c in _context.Cat_Estado on a.id_estado equals c.id
                        join d in _context.Cat_Municipio on a.id_municipio equals d.id
                        join e in _context.Cat_Localidad on a.id_localidad equals e.id
                        where b.id == id.id && a.tipo_direccion == 1
                        orderby a.creado ascending
                        select new
                        {
                            id = a.id,
                            calle = a.calle_numero,
                            numExt = a.numExt == null ? "" : a.numExt,
                            numInt = a.numInt == null ? "" : a.numInt,
                            colonia = a.colonia,
                            estado = c.desc_estado,
                            municipio = d.desc_municipio,
                            e.desc_localidad,
                            cp = a.cp
                        }).ToList();

            if (item.Count == 0)
            {
                return new ObjectResult(item);
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/Home_program
        [Route("Home_program")]
        [HttpPost("{Home_program}")]
        public IActionResult Home_program([FromBody] tecnicobyid id)
        {
            var item = (from a in _context.home_producto_cliente
                        where a.id_cliente == id.id && a.estatus_activo == true
                        select new
                        {
                            id = a.id,
                            a.estatus_activo,
                            a.estatus_venta,
                            a.folio,
                            a.horas,
                            a.no_visitas,
                            a.costo
                        }).ToList();

            if (item.Count() == 0)
            {
                return new ObjectResult(item);
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/Actualizar_home_program
        [Route("Actualizar_home_program")]
        [HttpPost("{Actualizar_home_program}")]
        public IActionResult Actualizar_home_program([FromBody] tecnicobyid id)
        {
            var item = _context.home_producto_cliente.FirstOrDefault(x => x.id == id.id);

            item.no_visitas = 1;
            item.estatus_activo = false;
            _context.home_producto_cliente.Update(item);
            _context.SaveChanges();

            if (item != null)
            {
                return new ObjectResult(item);
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/tecnicos_visita
        [Route("tecnicos_visita")]
        [HttpPost("{tecnicos_visita}")]
        public IActionResult tecnicos_visita([FromBody] Visita visita)
        {
            var tecnicos = (from a in _context.Visita
                            join b in _context.rel_tecnico_visita on a.id equals b.id_vista
                            join c in _context.Users on b.id_tecnico equals c.id
                            where a.id == visita.id
                            select new
                            {
                                c.id,
                                tecnico = (from a in _context.Visita
                                           join b in _context.rel_tecnico_visita on a.id equals b.id_vista
                                           join c in _context.Users on b.id_tecnico equals c.id
                                           where a.id == visita.id
                                           select new
                                           {
                                               c.id,
                                               tecnico = c.name + " " + c.paterno + " " + (c.materno == null ? "" : c.materno),
                                               b.tecnico_responsable
                                           }).ToList(),
                                b.tecnico_responsable
                            }).Distinct().ToList();

            if (tecnicos.Count == 0)
            {
                return new ObjectResult(tecnicos);
            }
            return new ObjectResult(tecnicos);
        }

        // POST: api/Servicios/Agregar_Direccion
        [Route("Agregar_Direccion")]
        [HttpPost("{Agregar_Direccion}")]
        public IActionResult Agregar_Direccion([FromBody] Cat_Direccion item)
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
                    _context.Cat_Direccion.Add(item);
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

        // POST: api/Servicios/Agregar_Cliente_Productos
        [Route("Agregar_Cliente_Productos")]
        [HttpPost("{Agregar_Cliente_Productos}")]
        public IActionResult Agregar_Cliente_Productos([FromBody] Cliente_Productos item)
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
                    _context.Cliente_Productos.Add(item);
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

        // POST: api/Servicios/Agregar_visita
        [Route("Agregar_visita")]
        [HttpPost("{Agregar_visita}")]
        public IActionResult Agregar_visita([FromBody] Visita item)
        {
            IActionResult response;

            try
            {
                if (item == null)
                {
                    return BadRequest();
                }
                else
                {
                    _context.Visita.Add(item);
                    _context.SaveChanges();

                    var _visita = _context.Visita.FirstOrDefault(t => t.id == item.id);

                    if (_visita != null)
                    {
                        _visita.pre_diagnostico = true;
                        _context.Visita.Update(_visita);

                        _context.SaveChanges();
                    }

                    CreatePDFOordenServicio(item.id_servicio, item.id);

                    return response = Ok(new { response = "OK", item });
                }
            }
            catch (Exception ex)
            {
                return response = Ok(new { item = ex.ToString() });
            }
        }

        // POST: api/Servicios/Editar_Direccion
        [Route("Editar_Direccion")]
        [HttpPost("{Editar_Direccion}")]
        public IActionResult Editar_Direccion([FromBody] Cat_Direccion item)
        {
            var result = new Models.Response();

            try
            {
                var direccion = _context.Cat_Direccion.FirstOrDefault(t => t.id == item.id);
                var cliente = _context.Clientes.FirstOrDefault(t => t.id == item.id_cliente);

                if (direccion == null)
                {
                    _context.Cat_Direccion.Add(item);
                    _context.SaveChanges();

                    result = new Models.Response
                    {
                        response = "OK"
                    };
                }
                else
                {
                    direccion.calle_numero = item.calle_numero;
                    direccion.cp = item.cp;
                    direccion.id_estado = item.id_estado;
                    direccion.id_municipio = item.id_municipio;
                    direccion.colonia = item.colonia;
                    direccion.id_localidad = Convert.ToInt32(item.colonia);
                    direccion.telefono = item.telefono;
                    direccion.numExt = item.numExt;
                    direccion.numInt = item.numInt;

                    cliente.telefono = item.telefono;
                    cliente.telefono_movil = item.telefono_movil;
                    cliente.referencias = item.Fecha_Estimada;

                    _context.Clientes.Update(cliente);
                    _context.Cat_Direccion.Update(direccion);
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

        // POST: api/Servicios/Editar_Tecnico
        [Route("Editar_Tecnico")]
        [HttpPost("{Editar_Tecnico}")]
        public IActionResult Editar_Tecnico([FromBody] Tecnicos_Usuario item)
        {
            var result = new Models.Response();

            try
            {
                var todo = _context.Tecnicos.FirstOrDefault(t => t.id == item.id);
                if (todo == null)
                {
                    return NotFound();
                }

                var actividad = _context.Tecnicos_Actividad.Where(t => t.id_user == item.id).ToList();
                if (actividad != null)
                {
                    for (int j = 0; j < actividad.Count(); j++)
                    {
                        _context.Tecnicos_Actividad.Remove(actividad[j]);
                        //_context.SaveChanges();
                    }
                }

                var producto = _context.Tecnicos_Producto.Where(t => t.id_user == item.id).ToList();
                if (producto != null)
                {
                    for (int j = 0; j < producto.Count(); j++)
                    {
                        _context.Tecnicos_Producto.Remove(producto[j]);
                        //_context.SaveChanges();
                    }
                }

                var user = _context.Users.FirstOrDefault(x => x.id == item.id);

                user.name = item.name;
                user.paterno = item.paterno;
                user.materno = item.materno;
                user.telefono = item.telefono;
                user.telefono_movil = item.movil;
                todo.noalmacen = item.noalmacen;
                if (item.tecnicos_actividad.Count() > 0)
                {
                    for (int i = 0; i < item.tecnicos_actividad.Count(); i++)
                    {
                        _context.Tecnicos_Actividad.Add(item.tecnicos_actividad[i]);
                    }
                }

                if (item.tecnicos_producto.Count() > 0)
                {
                    for (int i = 0; i < item.tecnicos_producto.Count(); i++)
                    {
                        _context.Tecnicos_Producto.Add(item.tecnicos_producto[i]);
                    }
                }

                _context.Tecnicos.Update(todo);
                _context.Users.Update(user);
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

        // POST: api/Servicios/Actualizar_Folio
        [Route("Actualizar_Folio")]
        [HttpPost("{Actualizar_Folio}")]
        public IActionResult Actualizar_Folio([FromBody] tecnicobyid id)
        {
            var result = new Models.Response();

            try
            {
                var item = _context.Servicio.FirstOrDefault(t => t.id == id.id);
                if (item == null)
                {
                    return BadRequest();
                }
                else
                {
                    item.no_servicio = id.id.ToString();
                    _context.Servicio.Update(item);
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

        // POST: api/Servicios/Actualizar_ibs
        [Route("Actualizar_ibs")]
        [HttpPost("{Actualizar_ibs}")]
        public IActionResult Actualizar_ibs([FromBody] ibs ibs)
        {
            var result = new Models.Response();

            try
            {
                var item = _context.Servicio.FirstOrDefault(t => t.id == ibs.id);
                if (item == null)
                {
                    return BadRequest();
                }
                else
                {
                    item.IBS = ibs.numero;
                    _context.Servicio.Update(item);
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

        [Route("Actualizar_direccion")]
        [HttpPost("{Actualizar_direccion}")]
        public IActionResult Actualizar_direccion([FromBody] ibs ibs)
        {
            var result = new Models.Response();

            try
            {
                var item = _context.Visita.FirstOrDefault(t => t.id == ibs.id);
                if (item == null)
                {
                    return BadRequest();
                }
                else
                {
                    item.id_direccion = Convert.ToInt32(ibs.numero);
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

        // POST: api/Servicios/Actualizar_estatus
        [Route("Actualizar_estatus")]
        [HttpPost("{Actualizar_estatus}")]
        public IActionResult Actualizar_estatus([FromBody] ibs id)
        {
            var result = new Models.Response();

            try
            {
                var item = _context.Servicio.FirstOrDefault(t => t.id == id.id);
                if (item == null)
                {
                    return BadRequest();
                }
                else
                {
                    item.id_estatus_servicio = Convert.ToInt32(id.numero);
                    _context.Servicio.Update(item);
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

        // POST: api/Servicios/Actualizar_estatus
        [Route("Actualizar_estatus_servicio_visita")]
        [HttpPost("{Actualizar_estatus_servicio_visita}")]
        public IActionResult Actualizar_estatus_servicio_visita([FromBody] ibs id)
        {
            var result = new Models.Response();

            try
            {
                var servicio = _context.Servicio.FirstOrDefault(s => s.id == id.id);
                var visita = _context.Visita.FirstOrDefault(v => v.id_servicio == id.id);
                if (servicio == null)
                {
                    return BadRequest();
                }
                else
                {
                    servicio.id_estatus_servicio = Convert.ToInt32(4);
                    visita.estatus = Convert.ToInt32(2);
                    _context.Servicio.Update(servicio);
                    _context.Visita.Update(visita);
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

        // POST: api/Servicios/Agendar_servicio_visita
        [Route("Agendar_servicio_visita")]
        [HttpPost("{Agendar_servicio_visita}")]
        public IActionResult Agendar_servicio_visita([FromBody] Visita id)
        {
            var result = new Models.Response();

            try
            {
                var servicio = _context.Servicio.FirstOrDefault(s => s.id == id.id_servicio);
                var visita = _context.Visita.FirstOrDefault(v => v.id == id.id);
                if (servicio == null)
                {
                    return BadRequest();
                }
                else
                {
                    servicio.id_estatus_servicio = Convert.ToInt32(4);
                    visita.estatus = Convert.ToInt32(2);
                    _context.Servicio.Update(servicio);
                    _context.Visita.Update(visita);
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

        // POST: api/Servicios/Actualizar_estatus
        [Route("Actualizar_estatus_final")]
        [HttpPost("{Actualizar_estatus_final}")]
        public IActionResult Actualizar_estatus_final([FromBody] ibs id)
        {
            var result = new Models.Response();

            try
            {
                var servicio = _context.Servicio.FirstOrDefault(s => s.id == id.id);
                var visita = _context.Visita.FirstOrDefault(v => v.id_servicio == id.id);
                if (servicio == null)
                {
                    return BadRequest();
                }
                else
                {
                    servicio.id_estatus_servicio = Convert.ToInt32(17);
                    visita.estatus = Convert.ToInt32(4);
                    _context.Servicio.Update(servicio);
                    _context.Visita.Update(visita);
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

        // POST: api/Servicios/Actualizar_estatus
        [Route("CancelarVisita")]
        [HttpPost("{CancelarVisita}")]
        public IActionResult CancelarVisita([FromBody] ibs id)
        {
            var result = new Models.Response();

            try
            {
                //var servicio = _context.Servicio.FirstOrDefault(s => s.id == id.id);
                var visita = _context.Visita.FirstOrDefault(v => v.id == id.id);
                if (visita == null)
                {
                    return BadRequest();
                }
                else
                {
                    visita.estatus = Convert.ToInt32(id.numero);
                    _context.Visita.Update(visita);
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

        // POST: api/Servicios/Actualizar_estatus
        [Route("Actualizar_estatus_cancelado")]
        [HttpPost("{Actualizar_estatus_cancelado}")]
        public IActionResult Actualizar_estatus_cancelado([FromBody] cancela_visita id)
        {
            var result = new Models.Response();

            try
            {
                var servicio = _context.Servicio.Include(c => c.cliente).FirstOrDefault(s => s.id == id.id_servicio);
                var visita = _context.Visita.FirstOrDefault(v => v.id_servicio == id.id_servicio);
                if (servicio == null)
                {
                    return BadRequest();
                }
                else
                {
                    int? _estus_visita = 0;
                    servicio.id_estatus_servicio = id.id_motivo;
                    if (id.id_motivo == 13) { _estus_visita = 5; } else { _estus_visita = 1002; }
                    visita.estatus = _estus_visita;
                    visita.motiva_cancelación = id.motivo_cancelacion;
                    visita.fecha_cancelacion = DateTime.Now;

                    _context.Servicio.Update(servicio);
                    _context.Visita.Update(visita);
                    _context.SaveChanges();
                    result = new Models.Response
                    {
                        response = "OK"
                    };

                    if (result.response == "OK")
                    {
                        //var path_template = Path.GetFullPath("TemplateMail/Email_cambio_estatus_servicio.html");
                        //StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_cambio_estatus_servicio.html"));
                        //string body = string.Empty;
                        //body = reader.ReadToEnd();
                        //body = body.Replace("{username}", "Estimado (a) cliente: " + servicio.cliente.nombre + " " + servicio.cliente.paterno);
                        //body = body.Replace("{no_servicio}", servicio.id.ToString());

                        //EmailModel _email = new EmailModel();
                        //_email.To = "linelsk@gmail.com";
                        //_email.Body = body;
                        //_email.Subject = "Miele - Cambio de estatus número:" + servicio.id.ToString();
                        //_email.IsBodyHtml = true;
                        //_email.id_app = 1;

                        //_emailRepository.SendMailAsync(_email);

                        var path_template = Path.GetFullPath("TemplateMail/Email_cambio_estatus_servicio.html");
                        StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_cambio_estatus_servicio.html"));
                        string body = string.Empty;
                        body = reader.ReadToEnd();
                        body = body.Replace("{username}", "Estimado (a) cliente: " + servicio.cliente.nombre + " " + servicio.cliente.paterno);
                        body = body.Replace("{no_servicio}", servicio.id.ToString());
                        body = body.Replace("{tipo}", "Cancelado");
                        body = body.Replace("{motivo}", visita.motiva_cancelación);

                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential("rodrigo.stps@gmail.com", "bostones01")
                        };
                        using (var message = new MailMessage
                        {
                            From = new MailAddress("no-reply@miele.com.mx", "Miele Service"),
                            To = { "linelsk@gmail.com", servicio.cliente.email },
                            Subject = "Miele - Confirmación de servicio " + servicio.id,
                            IsBodyHtml = true,
                            Body = body

                        })
                        {
                            smtp.Send(message);
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

        // POST: api/Servicios/Actualizar_estatus
        [Route("Actualizar_estatus_completado")]
        [HttpPost("{Actualizar_estatus_completado}")]
        public IActionResult Actualizar_estatus_completado([FromBody] cancela_visita id)
        {
            var result = new Models.Response();

            try
            {
                var servicio = _context.Servicio.Include(c => c.cliente).FirstOrDefault(s => s.id == id.id_servicio);
                var visita = _context.Visita.FirstOrDefault(v => v.id_servicio == id.id_servicio);
                if (servicio == null)
                {
                    return BadRequest();
                }
                else
                {
                    servicio.id_estatus_servicio = id.id_motivo;
                    visita.estatus = 4;
                    visita.motiva_completado = id.motivo_cancelacion;
                    visita.fecha_completado = DateTime.Now;

                    _context.Servicio.Update(servicio);
                    _context.Visita.Update(visita);
                    _context.SaveChanges();
                    result = new Models.Response
                    {
                        response = "OK"
                    };

                    if (result.response == "OK")
                    {
                        //var path_template = Path.GetFullPath("TemplateMail/Email_cambio_estatus_servicio.html");
                        //StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_cambio_estatus_servicio.html"));
                        //string body = string.Empty;
                        //body = reader.ReadToEnd();
                        //body = body.Replace("{username}", "Estimado (a) cliente: " + servicio.cliente.nombre + " " + servicio.cliente.paterno);
                        //body = body.Replace("{no_servicio}", servicio.id.ToString());

                        //EmailModel _email = new EmailModel();
                        //_email.To = "linelsk@gmail.com";
                        //_email.Body = body;
                        //_email.Subject = "Miele - Cambio de estatus número:" + servicio.id.ToString();
                        //_email.IsBodyHtml = true;
                        //_email.id_app = 1;

                        //_emailRepository.SendMailAsync(_email);
                        var path_template = Path.GetFullPath("TemplateMail/Email_cambio_estatus_servicio.html");
                        StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_cambio_estatus_servicio.html"));
                        string body = string.Empty;
                        body = reader.ReadToEnd();
                        body = body.Replace("{username}", "Estimado (a) cliente: " + servicio.cliente.nombre + " " + servicio.cliente.paterno);
                        body = body.Replace("{no_servicio}", servicio.id.ToString());
                        body = body.Replace("{tipo}", "Completado");
                        body = body.Replace("{motivo}", visita.motiva_completado);

                        var smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential("rodrigo.stps@gmail.com", "bostones01")
                        };
                        using (var message = new MailMessage
                        {
                            From = new MailAddress("no-reply@miele.com.mx", "Miele Service"),
                            To = { "linelsk@gmail.com", servicio.cliente.email },
                            Subject = "Miele - Confirmación de servicio " + servicio.id,
                            IsBodyHtml = true,
                            Body = body

                        })
                        {
                            smtp.Send(message);
                        } // subject del correo
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

        // POST: api/Servicios/Actualizar_estatus
        [Route("Actualizar_visita")]
        [HttpPost("{Actualizar_visita}")]
        public IActionResult Actualizar_visita([FromBody] Visita id)
        {
            var result = new Models.Response();

            try
            {
                var visita = _context.Visita.Include(v => v.rel_tecnico_visita).FirstOrDefault(v => v.id == id.id);
                if (visita == null)
                {
                    return BadRequest();
                }
                else
                {
                    visita.fecha_visita = id.fecha_visita;
                    //visita.id_tecnico = id.id_tecnico;
                    visita.hora = id.hora;
                    visita.hora_fin = id.hora_fin;
                    if (visita.rel_tecnico_visita.Count == id.rel_tecnico_visita.Count)
                    {
                        for (int i = 0; i < visita.rel_tecnico_visita.Count; i++)
                        {
                            if (visita.rel_tecnico_visita[i].id_tecnico != id.rel_tecnico_visita[i].id_tecnico)
                            {
                                visita.rel_tecnico_visita[i].id_tecnico = id.rel_tecnico_visita[i].id_tecnico;
                                _context.rel_tecnico_visita.Update(visita.rel_tecnico_visita[i]);
                            }
                        }
                    }
                    else
                    {
                        _context.rel_tecnico_visita.RemoveRange(visita.rel_tecnico_visita);
                        _context.rel_tecnico_visita.AddRange(id.rel_tecnico_visita);
                    }
                    _context.Visita.Update(visita);
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

        // POST: api/Servicios/Visita_Id
        [Route("Visita_Id")]
        [HttpPost("{Visita_Id}")]
        public IActionResult Visita_Id([FromBody] tecnicobyid id)
        {
            var item = (from x in _context.Visita
                        join d in _context.Cat_Direccion on x.id_direccion equals d.id
                        join e in _context.Cat_Estado on d.id_estado equals e.id
                        join f in _context.Cat_Municipio on d.id_municipio equals f.id
                        join lo in _context.Cat_Localidad on d.id_localidad equals lo.id
                        join tv in _context.rel_tecnico_visita on x.id equals tv.id_vista
                        join i in _context.Tecnicos on tv.id_tecnico equals i.id
                        join j in _context.Users on i.id equals j.id
                        where x.id == id.id
                        select new
                        {
                            id = x.id,
                            actividades_realizar = x.actividades_realizar,
                            fecha_visita = x.fecha_visita.ToShortDateString(),
                            hora = x.hora,
                            fecha_inicio_visita = Convert.ToDateTime(x.fecha_inicio_visita).ToShortDateString(),
                            estatus = x.estatus,
                            direccion = d.calle_numero + ", " + lo.desc_localidad + ", " + e.desc_estado + ", " + f.desc_municipio,
                            productos = (from x in _context.Rel_servicio_producto
                                         join c_p in _context.Cliente_Productos on x.id_producto equals c_p.Id
                                         join z in _context.Visita on x.id_vista equals z.id
                                         join e in _context.Cat_Productos on c_p.Id_Producto equals e.id
                                         join f in _context.Cat_SubLinea_Producto on e.id_sublinea equals f.id
                                         join g in _context.Cat_Linea_Producto on e.id_linea equals g.id
                                         join h in _context.Cat_SubLinea_Producto on e.id_sublinea equals h.id
                                         where x.id_vista == id.id
                                         select new
                                         {
                                             id = e.id,
                                             sku = e.sku,
                                             modelo = e.modelo,
                                             nombre = e.nombre,
                                             estatus = e.estatus,
                                             descripcion_corta = e.descripcion_corta,
                                             descripcion_larga = e.descripcion_larga,
                                             atributos = e.atributos,
                                             precio_sin_iva = e.precio_sin_iva,
                                             precio_con_iva = e.precio_con_iva,
                                             categoria = f.descripcion,
                                             linea = g.descripcion,
                                             sublinea = h.descripcion,
                                             ficha_tecnica = e.ficha_tecnica,
                                             cat_imagenes_producto = (from x in _context.Cat_Imagenes_Producto
                                                                      where x.id_producto == x.id_producto
                                                                      select new
                                                                      {
                                                                          id = x.id,
                                                                          url = x.url
                                                                      }).ToList()
                                         }).ToList()
                        }).ToList();

            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/EstatusServicio
        [Route("EstatusServicioMantenimiento")]
        [HttpPost("{EstatusServicioMantenimiento}")]
        public IActionResult EstatusServicioMantenimiento()
        {
            var myInClause = new int[] { 0, 1 };
            var item = _context.Cat_estatus_servicio.Where(x => myInClause.Contains(x.id_tipo_servicio)).ToList();

            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/EstatusServicio
        [Route("EstatusProductosMantenimiento")]
        [HttpPost("{EstatusProductosMantenimiento}")]
        public IActionResult EstatusProductosMantenimiento([FromBody] estatus id)
        {
            var myInClause = new int[] { id.id_primario, id.id_secundario };
            var item = _context.CatEstatus_Producto.Where(x => myInClause.Contains(x.id_tipo_servicio)).ToList();

            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/TecnicoAgenda
        [Route("TecnicoAgenda")]
        [HttpPost("{TecnicoAgenda}")]
        public IActionResult TecnicoAgenda([FromBody] tecnico_agenda tecnico)
        {

            //List<long> myInClause = new List<long>() { };
            //for (int i = 0; i < tecnico.productos.Split(',').Count(); i++)
            //{
            //    if (tecnico.productos.Split(',')[i] != "")
            //    {
            //        myInClause.Add(Convert.ToInt64(tecnico.productos.Split(',')[i]));
            //    }
            //}

            //var _sub = ((from Tecnicos_Producto in _context.Tecnicos_Producto
            //             where
            //               (myInClause).Contains(Tecnicos_Producto.id_categoria_producto)
            //             select new
            //             {
            //                 Tecnicos_Producto.id_user
            //             }).Distinct().ToList());

            //List<long> _myInClause = new List<long>() { };
            //for (int i = 0; i < _sub.Count(); i++)
            //{
            //    _myInClause.Add(Convert.ToInt64(_sub[i].id_user));
            //}

            List<long> _actividades = new List<long>() { };
            var _Tecnicos_Actividad = (from a in _context.Tecnicos_Actividad
                                       where a.id_actividad == tecnico.id
                                       select new
                                       {
                                           a.id_user

                                       }).ToList();
            for (int i = 0; i < _Tecnicos_Actividad.Count(); i++)
            {
                if (_Tecnicos_Actividad[i] != null)
                {
                    _actividades.Add(Convert.ToInt64(_Tecnicos_Actividad[i].id_user));
                }
            }

            var item = ((
                from a in _context.Tecnicos
                where
                  (_actividades).Contains(a.id)
                select new
                {
                    a.id,
                    nombre = (a.users.name ?? ""),
                    paterno = (a.users.paterno ?? ""),
                    materno = (a.users.materno ?? ""),
                    tecnico_color = a.color
                })).ToList();
            //).Union
            //(
            //    from a in _context.Tecnicos
            //    where
            //      (_myInClause).Contains(a.id)
            //    select new
            //    {
            //        a.id,
            //        nombre = (a.users.name ?? ""),
            //        paterno = (a.users.paterno ?? ""),
            //        materno = (a.users.materno ?? ""),
            //        tecnico_color = a.color
            //    }
            //)).ToList();

            if (item.Count == 0)
            {
                return new ObjectResult(item);
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/TecnicoCalendario
        [Route("TecnicoCalendario")]
        [HttpPost("{TecnicoCalendario}")]
        public IActionResult TecnicoCalendario([FromBody] tecnico_agenda tecnico)
        {
            List<long> myInClause = new List<long>() { };
            if (tecnico.productos != null)
            {
                for (int i = 0; i < tecnico.productos.Split(',').Count(); i++)
                {
                    if (tecnico.productos.Split(',')[i] != "")
                    {
                        myInClause.Add(Convert.ToInt64(tecnico.productos.Split(',')[i]));
                    }
                }

            }

            var item = (from a in _context.Servicio
                        join b in _context.Visita on a.id equals b.id_servicio
                        join tv in _context.rel_tecnico_visita on b.id equals tv.id_vista
                        join c in _context.Cat_tipo_servicio on a.id_tipo_servicio equals c.id
                        join d in _context.Users on tv.id_tecnico equals d.id
                        join e in _context.Tecnicos on d.id equals e.id
                        where (myInClause).Contains(e.id)
                        orderby a.creado ascending
                        select new
                        {
                            id_tecnico = d.id,
                            desc_tipo_servicio = c.desc_tipo_servicio,
                            tecnico = d.name + ' ' + d.paterno + ' ' + (d.materno == null ? "" : d.materno),
                            hora_inicio = b.hora,
                            hora_fin = b.hora_fin,
                            fecha_visita = b.fecha_visita,
                            tecnico_color = e.color
                        }).ToList();


            //List<long> myInClause = new List<long>() { };
            //for (int i = 0; i < tecnico.productos.Split(',').Count(); i++)
            //{
            //    if (tecnico.productos.Split(',')[i] != "")
            //    {
            //        myInClause.Add(Convert.ToInt64(tecnico.productos.Split(',')[i]));
            //    }
            //}

            //var _sub = ((from Tecnicos_Producto in _context.Tecnicos_Producto
            //             where
            //               (myInClause).Contains(Tecnicos_Producto.id_categoria_producto)
            //             select new
            //             {
            //                 Tecnicos_Producto.id_user
            //             }).Distinct().ToList());

            //List<long> _myInClause = new List<long>() { };
            //for (int i = 0; i < _sub.Count(); i++)
            //{
            //    _myInClause.Add(Convert.ToInt64(_sub[i].id_user));
            //}

            //var item = ((from a in _context.Servicio
            //             join b in _context.Visita on a.id equals b.id_servicio
            //             join c in _context.Cat_tipo_servicio on a.id_tipo_servicio equals c.id
            //             join d in _context.Users on b.id_tecnico equals d.id
            //             join e in _context.Tecnicos on d.id equals e.id
            //             where e.id_tipo_tecnico == tecnico.id
            //             orderby a.creado ascending
            //             select new
            //             {
            //                 id_tecnico = d.id,
            //                 desc_tipo_servicio = c.desc_tipo_servicio,
            //                 tecnico = d.name + ' ' + d.paterno + ' ' + d.materno,
            //                 hora_inicio = b.hora,
            //                 hora_fin = b.hora_fin,
            //                 fecha_visita = b.fecha_visita,
            //                 tecnico_color = e.color
            //             }).Union
            //            (from a in _context.Servicio
            //             join b in _context.Visita on a.id equals b.id_servicio
            //             join c in _context.Cat_tipo_servicio on a.id_tipo_servicio equals c.id
            //             join d in _context.Users on b.id_tecnico equals d.id
            //             join e in _context.Tecnicos on d.id equals e.id
            //             where (_myInClause).Contains(e.id)
            //             orderby a.creado ascending
            //             select new
            //             {
            //                 id_tecnico = d.id,
            //                 desc_tipo_servicio = c.desc_tipo_servicio,
            //                 tecnico = d.name + ' ' + d.paterno + ' ' + d.materno,
            //                 hora_inicio = b.hora,
            //                 hora_fin = b.hora_fin,
            //                 fecha_visita = b.fecha_visita,
            //                 tecnico_color = e.color
            //             })).ToList();

            if (item.Count == 0)
            {
                return new ObjectResult(item);
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/TecnicoCalendario
        [Route("TecnicoCalendarioHome")]
        [HttpPost("{TecnicoCalendarioHome}")]
        public IActionResult TecnicoCalendarioHome()
        {
            var item = (from a in _context.Servicio
                        join b in _context.Visita on a.id equals b.id_servicio
                        join vs in _context.CatEstatus_Visita on b.estatus equals vs.id into _CatEstatus_Visita
                        from CatEstatus_Visita in _CatEstatus_Visita.DefaultIfEmpty()
                        join tv in _context.rel_tecnico_visita on b.id equals tv.id_vista
                        join c in _context.Cat_tipo_servicio on a.id_tipo_servicio equals c.id
                        join d in _context.Users on tv.id_tecnico equals d.id
                        join e in _context.Tecnicos on d.id equals e.id
                        //join ser in _context.Servicio on b.id_servicio equals ser.id
                        //join cli in _context.Clientes on ser.id_cliente equals cli.id
                        //orderby a.creado ascending
                        select new
                        {
                            id_tecnico = d.id,
                            desc_tipo_servicio = c.desc_tipo_servicio,
                            tecnico = d.name + ' ' + d.paterno + ' ' + d.materno,
                            hora_inicio = b.hora,
                            hora_fin = b.hora_fin,
                            fecha_visita = b.fecha_visita,
                            tecnico_color = e.color,
                            a.no_servicio,
                            desc_estatus_visita = CatEstatus_Visita.desc_estatus_visita == null ? "No iniciada" : CatEstatus_Visita.desc_estatus_visita,
                            cliente = b.servicio.cliente.nombre + " " + b.servicio.cliente.paterno + " " + (b.servicio.cliente.materno == null ? "" : b.servicio.cliente.materno),
                            b.pagado,
                            b.pago_pendiente
                        }).Distinct().ToList();

            if (item.Count == 0)
            {
                return new ObjectResult(item);
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/Tecnico_id
        [Route("Tecnico_id")]
        [HttpPost("{Tecnico_id}")]
        public IActionResult Tecnico_id([FromBody] tecnico_in id)
        {
            IActionResult response;
            List<long> myInClause = new List<long>() { };

            for (int i = 0; i < id.id.Split(',').Count(); i++)
            {
                if (id.id.Split(',')[i] != "")
                {
                    myInClause.Add(Convert.ToInt64(id.id.Split(',')[i]));
                }
            }

            var item = (from b in _context.Visita
                        join tv in _context.rel_tecnico_visita on b.id equals tv.id_vista
                        join e in _context.Tecnicos on tv.id_tecnico equals e.id
                        where
                          (myInClause).Contains(e.id)
                        select new
                        {
                            id_tecnico = e.users.id,
                            b.servicio.tipo_servicio.desc_tipo_servicio,
                            tecnico = (e.users.name + " " + e.users.paterno + " " + (e.users.materno == null ? "" : e.users.materno)),
                            hora_inicio = b.hora,
                            hora_fin = b.hora_fin,
                            b.fecha_visita,
                            tecnico_color = e.color
                        }).Distinct().ToList();

            if (item.Count == 0)
            {
                response = Ok(new { item });
            }
            else
            {
                response = Ok(new { item });
            }

            return new ObjectResult(response);
        }

        // POST: api/Servicios/Upload
        //[Route("Troubleshooting_Producto")]
        //[HttpPost("{Troubleshooting_Producto}")]
        //public IActionResult Troubleshooting_Producto([FromBody] Cat_Productos_Preguntas_Troubleshooting id)
        //{
        //    var item = (from x in _context.Cat_Productos_Preguntas_Troubleshooting
        //                where x.modelo.ToLower() == id.modelo.ToLower()
        //                select new
        //                {
        //                    x.id,
        //                    x.modelo,
        //                    x.pregunta,
        //                    respuestas = (from z in _context.Cat_Productos_Respuestas_Troubleshooting
        //                                  where z.modelo.ToLower() == x.modelo.ToLower()
        //                                  select new
        //                                  {
        //                                      z.falla,
        //                                      z.solucion
        //                                  }).ToList()

        //                }).ToList();

        //    if (item == null)
        //    {
        //        return NotFound();
        //    }
        //    return new ObjectResult(item);
        //}

        // GET: api/Catalogos/Tipo_Refaccion

        [HttpGet("problema_troubleshooting")]
        public IEnumerable<Cat_Productos_Problema_Troubleshooting> GetProblema(string modelo = "")
        {
            return _context.Cat_Productos_Problema_Troubleshooting.Where(x => x.modelo == modelo);
        }

        [Route("Pregunta_troubleshooting")]
        [HttpPost("{Pregunta_troubleshooting}")]
        public IActionResult Pregunta_troubleshooting([FromBody] Cat_Productos_Preguntas_Troubleshooting id)
        {
            var item = (from a in _context.Cat_Productos_Preguntas_Troubleshooting
                        where a.id_problema == id.id_problema
                        select new
                        {
                            id = a.id,
                            pregunta = a.pregunta,
                            respuestas = (from b in _context.Cat_Productos_Respuestas_Troubleshooting
                                          where b.id_pregunta == a.id
                                          select new
                                          {
                                              b.id,
                                              b.falla,
                                              b.solucion
                                          }).ToList()
                        }).ToList();

            if (item == null)
            {
                return BadRequest();
            }
            return new ObjectResult(item);
        }

        [Route("Respuesta_troubleshooting")]
        [HttpPost("{Respuesta_troubleshooting}")]
        public IActionResult Respuesta_troubleshooting([FromBody] Cat_Productos_Respuestas_Troubleshooting id)
        {
            var item = (from b in _context.Cat_Productos_Respuestas_Troubleshooting
                        where b.id_pregunta == id.id_pregunta
                        select new
                        {
                            b.id,
                            b.falla,
                            b.solucion
                        }).ToList();


            if (item == null)
            {
                return BadRequest();
            }
            return new ObjectResult(item);
        }


        private Image redimensionar(FileStream imagen)
        {
            Image img = Image.FromStream(imagen);
            //var res = img.GetPropertyItem(274).Value[0];
            //.PropertyIdList.Contains(Orientation);
            var orientation = (int)img.GetPropertyItem(274).Value[0];
            switch (orientation)
            {
                case 1:
                    // No rotation required.
                    break;
                case 2:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case 3:
                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case 4:
                    img.RotateFlip(RotateFlipType.Rotate180FlipX);
                    break;
                case 5:
                    img.RotateFlip(RotateFlipType.Rotate90FlipX);
                    break;
                case 6:
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case 7:
                    img.RotateFlip(RotateFlipType.Rotate270FlipX);
                    break;
                case 8:
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }
            // This EXIF data is now invalid and should be removed.
            img.RemovePropertyItem(274);


            const int max = 1024;
            int h = img.Height;
            int w = img.Width;
            int newH, newW;



            if (h > w && h > max)
            {
                // Si la imagen es vertical y la altura es mayor que max,
                // se redefinen las dimensiones.
                newH = max;
                newW = (w * max) / h;
            }
            else if (w > h && w > max)
            {
                // Si la imagen es horizontal y la anchura es mayor que max,
                // se redefinen las dimensiones.
                newW = max;
                newH = (h * max) / w;
            }
            else
            {
                newH = h;
                newW = w;
            }
            if (h != newH && w != newW)
            {
                // Si las dimensiones cambiaron, se modifica la imagen
                Bitmap newImg = new Bitmap(img, newW, newH);
                Graphics g = Graphics.FromImage(newImg);
                g.InterpolationMode =
                  System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                g.DrawImage(img, 0, 0, newImg.Width, newImg.Height);
                return newImg;
            }
            else
                return img;
        }

        private void recortar(string imagen)
        {
            //using (var fileimg = File.OpenRead(imagen))
            //{

            //}
            Image img = new Bitmap(@"D:\AreaTrabajo\netcore\miele\MyFirstProject\api_miele\WebApplication\WebApplication\Imagenes\carga_xls\_del7d366132-cbd9-4109-a594-57de34419292.jpg");
            //oldImg.PropertyItems
            //Image img = Image.FromStream(imagen);
            const int max = 1024;
            int h = img.Height;
            int w = img.Width;
            int newH, newW;

            if (h > w && h > max)
            {
                // Si la imagen es vertical y la altura es mayor que max,
                // se redefinen las dimensiones.
                newH = max;
                newW = (w * max) / h;
            }
            else if (w > h && w > max)
            {
                // Si la imagen es horizontal y la anchura es mayor que max,
                // se redefinen las dimensiones.
                newW = max;
                newH = (h * max) / w;
            }
            else
            {
                newH = h;
                newW = w;
            }
            if (h != newH && w != newW)
            {
                // Si las dimensiones cambiaron, se modifica la imagen
                Bitmap newImg = new Bitmap(img, newW, newH);
                Graphics g = Graphics.FromImage(newImg);
                g.InterpolationMode =
                  System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
                g.DrawImage(img, 0, 0, newImg.Width, newImg.Height);
                //return newImg;
                newImg.Save(imagen, System.Drawing.Imaging.ImageFormat.Jpeg);
            }

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
                var path = "/Imagenes/Cierre_Servicio/" + _guid + "." + extencion[extencion.Length - 1].ToLower();

                foreach (var formFile in file)
                {
                    if (formFile.Length > 0)
                    {
                        if (formFile.ContentType.StartsWith("image"))
                        {
                            using (var stream = new FileStream(filePath + "/Imagenes/Cierre_Servicio/_del" + _guid + "." + extencion[extencion.Length - 1], FileMode.Create))
                            {
                                formFile.CopyTo(stream);
                                Image image = redimensionar(stream);
                                image.Save(filePath + path, System.Drawing.Imaging.ImageFormat.Jpeg);

                            }
                        }
                        else
                        {
                            using (var stream = new FileStream(filePath + path, FileMode.Create))
                            {
                                formFile.CopyTo(stream);
                            }
                        }


                    }
                }

                result = new Models.Response
                {
                    response = path
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

        [Route("Upload_Comprobante")]
        [HttpPost("{Upload_Comprobante}")]
        public async Task<IActionResult> Upload_Comprobante(List<IFormFile> file)
        {

            var result = new Models.Response();

            try
            {
                long size = file.Sum(f => f.Length);

                var filePath = Environment.CurrentDirectory;
                var extencion = file[0].FileName.Split(".");
                var _guid = Guid.NewGuid();
                var path = "/Imagenes/Imagenes_Comprobantes/" + _guid + "." + extencion[extencion.Length - 1].ToLower();

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

                result = new Models.Response
                {
                    response = path
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

        [Route("UploadImgSuc")]
        [HttpPost("{UploadImgSuc}")]
        public async Task<IActionResult> UploadImgSuc(List<IFormFile> file)
        {
            var result = new Models.Response();

            try
            {

                var selRuta = _context.parametro_Archivos.FirstOrDefault(p => p.id == 2);
                string ruta;
                if (selRuta == null) ruta = "Error";
                else
                {
                    long size = file.Sum(f => f.Length);

                    var filePath = Environment.CurrentDirectory;
                    var extencion = file[0].FileName.Split(".");
                    var _guid = Guid.NewGuid();
                    var path = selRuta.ruta + _guid + "." + extencion[extencion.Length - 1].ToLower();

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
                    ruta = selRuta.funcion + _guid + "." + extencion[1];
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

        [Route("Upload_xls")]
        [HttpPost("{Upload_xls}")]
        //public async Task<IActionResult> Upload_xls(List<IFormFile> file)
        public IActionResult Upload_xls(List<IFormFile> file)
        {
            var result = new Models.Response();

            try
            {
                var selRuta = _context.parametro_Archivos.FirstOrDefault(p => p.id == 3);
                string ruta;
                if (selRuta == null) ruta = "Error";
                else
                {
                    long size = file.Sum(f => f.Length);

                    var filePath = Environment.CurrentDirectory;
                    var extencion = file[0].FileName.Split(".");
                    var _guid = Guid.NewGuid();
                    var path = "/Imagenes/carga_xls/" + _guid + "." + extencion[extencion.Length - 1].ToLower();

                    foreach (var formFile in file)
                    {


                        if (formFile.Length > 0)
                        {
                            //using (var stream = new FileStream(filePath + path, FileMode.Create))
                            //{
                            //    await formFile.CopyToAsync(stream);
                            //}

                            if (formFile.ContentType.StartsWith("image"))
                            {
                                using (var stream = new FileStream(filePath + "/Imagenes/carga_xls/_del" + _guid + "." + extencion[extencion.Length - 1], FileMode.Create))
                                {
                                    formFile.CopyTo(stream);
                                    Image image = redimensionar(stream);
                                    image.Save(filePath + path, System.Drawing.Imaging.ImageFormat.Jpeg);
                                }
                                //recortar(filePath + "/Imagenes/carga_xls/_del_" + _guid + "." + extencion[extencion.Length - 1]);
                            }
                            else
                            {
                                using (var stream = new FileStream(filePath + path, FileMode.Create))
                                {
                                    formFile.CopyTo(stream);
                                }
                            }
                        }
                    }
                    ruta = path;
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

        // POST: api/Servicios/Excel_Producto
        [Route("Excel_Producto")]
        [HttpPost("{Excel_Producto}")]
        public IActionResult Excel_Producto([FromBody] checklist ruta)
        {
            var result = new Models.Response();
            List<EntidadHojaExcel> _EntidadHojaExcel = new List<EntidadHojaExcel>();
            var filePath = Environment.CurrentDirectory;
            HSSFWorkbook hssfwb;
            //string rutaUbicacion = @"C:\inetpub\wwwroot\MieleDev\" +ruta.sku;
            string rutaUbicacion = filePath + ruta.sku;
            try
            {
                using (FileStream file = new FileStream(rutaUbicacion, FileMode.Open, FileAccess.Read))
                {
                    hssfwb = new HSSFWorkbook(file);
                }

                ISheet sheet = hssfwb.GetSheet("Hoja1");
                for (int row = 0; row <= sheet.LastRowNum; row++)
                {
                    if (sheet.GetRow(row) != null) //null is when the row only contains empty cells 
                    {
                        _EntidadHojaExcel.Add(new EntidadHojaExcel
                        {
                            descripcion = sheet.GetRow(row).GetCell(0).StringCellValue.ToString(),
                            estatus = sheet.GetRow(row).GetCell(1).NumericCellValue.ToString(),
                            id_grupo_precio = sheet.GetRow(row).GetCell(2).NumericCellValue.ToString(),
                            no_material = sheet.GetRow(row).GetCell(3).StringCellValue.ToString(),
                            cantidad = sheet.GetRow(row).GetCell(4).NumericCellValue.ToString(),
                        });
                    }
                }
                if (_EntidadHojaExcel.Count > 0)
                {
                    var del_inf = _context.Informe_parte_recibida.Where(a => a.id > 0);
                    _context.Informe_parte_recibida.RemoveRange(del_inf);
                    _context.SaveChanges();
                }

                return new ObjectResult(_EntidadHojaExcel);
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex.ToString());
            }
        }

        [Route("Guardar_Imagen_Producto")]
        [HttpPost("{Guardar_Imagen_Producto}")]
        public IActionResult Guardar_Imagen_Producto([FromBody] Rel_Imagen_Producto_Visita item)
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
                    _context.Rel_Imagen_Producto_Visita.Add(item);
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

        // POST: api/Servicios
        [HttpPost]
        public IActionResult Post([FromBody]Servicio item)
        {
            IActionResult response;
            try
            {
                if (item == null)
                {
                    return BadRequest();
                }
                else
                {
                    _context.Servicio.Add(item);
                    if (item.id_categoria_servicio == 1)
                    {
                        for (int i = 0; i < item.visita[0].servicio_producto.Count(); i++)
                        {

                            var certificado = (from a in _context.Cer_producto_cliente
                                               join b in _context.rel_certificado_producto on a.id equals b.id_certificado
                                               where a.id_cliente == item.id_cliente && b.estatus_activo == true && a.estatus_venta == true && b.id_sub_linea == item.visita[0].servicio_producto[i].id_categoria
                                               select new
                                               {
                                                   a.id,
                                                   id_rel = b.id,
                                                   b.id_producto
                                               }).ToList();//
                            var _certificado = _context.rel_certificado_producto.FirstOrDefault(t => t.id == certificado[0].id_rel);
                            _certificado.no_visitas = 1;
                            _certificado.fecha_visita_1 = DateTime.Now;
                            _context.rel_certificado_producto.Update(_certificado);
                        }

                    }
                    if (item.id_categoria_servicio == 2)
                    {
                        //var certificado = _context.Cer_producto_cliente.FirstOrDefault(t => t.id_cliente == item.id_cliente && t.estatus_activo == true && t.no_visitas == 1);
                        //certificado.no_visitas = 2;
                        //certificado.estatus_activo = false;
                        //_context.Cer_producto_cliente.Update(certificado);
                        for (int i = 0; i < item.visita[0].servicio_producto.Count(); i++)
                        {

                            var certificado = (from a in _context.Cer_producto_cliente
                                               join b in _context.rel_certificado_producto on a.id equals b.id_certificado
                                               where a.id_cliente == item.id_cliente && b.estatus_activo == true && a.estatus_venta == true && b.id_sub_linea == item.visita[0].servicio_producto[i].id_categoria
                                               select new
                                               {
                                                   a.id,
                                                   id_rel = b.id,
                                                   b.id_producto
                                               }).ToList();//
                            var _certificado = _context.rel_certificado_producto.FirstOrDefault(t => t.id == certificado[0].id_rel);
                            _certificado.no_visitas = 2;
                            _certificado.fecha_visita_2 = DateTime.Now;
                            _certificado.estatus_activo = false;
                            _context.rel_certificado_producto.Update(_certificado);
                        }
                    }
                    _context.SaveChanges();
                    var cliente = _context.Clientes.FirstOrDefault(t => t.id == item.id_cliente);
                    long _visita = item.visita[0].id;
                    if (cliente != null)
                    {
                        if (item.id_estatus_servicio != 1)
                        {
                            CreatePDFOordenServicio(item.id, item.visita[0].id);
                            //Se comenta distribuidor porque ya se envia en la misma función
                            //if (item.contacto != null)
                            //{
                            //    CreatePDFOordenServicio_Distribuidor(item.id, item.visita[0].id);
                            //}
                            _visita = item.visita[0].id;
                        }

                    }
                    response = Ok(new { response = item.id, id_visita = _visita });
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { response = ex.ToString() });
            }
            return new ObjectResult(response);
        }

        // POST: api/Servicios/No_visitas
        [Route("No_visitas")]
        [HttpPost("{No_visitas}")]
        public IActionResult Visita_Id([FromBody] Visita id)
        {
            var item = (from x in _context.Visita
                        join d in _context.Servicio on x.id_servicio equals d.id
                        where x.id_servicio == id.id_servicio
                        select new
                        {
                            no_visitas = (from _servicio in _context.Visita where _servicio.id_servicio == id.id_servicio select _servicio.id).Count()

                        }).ToList();

            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/Pendientes_Partes
        [HttpGet("Pendientes_Partes")]
        public IActionResult Pendientes_Partes(long? no_servicio = null, string fecha_in = "01/01/1900", string fecha_fi = "01/01/1900")
        {
            DateTime f1 = Convert.ToDateTime(fecha_in);
            DateTime f2 = Convert.ToDateTime(fecha_fi);

            f1.ToString("dd-MM-yyyy");
            f2.ToString("dd-MM-yyyy");

            var item = (from x in _context.Servicio
                        join d in _context.Visita on x.id equals d.id_servicio
                        join c in _context.Cat_estatus_servicio on x.id_estatus_servicio equals c.id
                        join e in _context.Clientes on x.id_cliente equals e.id
                        join f in _context.Cliente_Productos on e.id equals f.Id_Cliente
                        join g in _context.Rel_servicio_Refaccion on d.id equals g.id_vista
                        join h in _context.Piezas_Repuesto on g.id equals h.id_rel_servicio_refaccion
                        //join i in _context.rel_tecnico_visita on d.id equals i.id_tecnico
                        where x.id_estatus_servicio == 14 //&& i.tecnico_responsable == true//&& f.Id_EsatusCompra == 1 || f.Id_EsatusCompra == 2
                        select new
                        {
                            x.id,
                            x.IBS,
                            id_visita = d.id,
                            id_cliente = e.id,
                            id_servicio = x.id,
                            cliente = e.nombre.ToUpper() + " " + e.paterno.ToUpper() + " " + (e.materno == null ? "" : e.materno.ToUpper()),
                            desc_estatus_servicio = c.desc_estatus_servicio.ToUpper(),
                            fecha_visita = d.fecha_visita.ToString("dd/MM/yyyy"),
                            //i.id_tecnico

                        }).Distinct();

            if (no_servicio != null)
            {
                item = item.Where(x => x.id == no_servicio);
            }

            if (fecha_in != "01/01/1900" && fecha_fi != "01/01/1900")
            {
                item = item.Where(x => Convert.ToDateTime(x.fecha_visita) >= f1.Date && Convert.ToDateTime(x.fecha_visita).Date <= f2.Date);
            }

            //if (tecnico != null)
            //{
            //    item = item.Where(x => x.id_tecnico == tecnico);
            //}

            if (item.Count() == 0)
            {
                return new ObjectResult(item);
            }
            return new ObjectResult(item);
        }

        // POST: api/Servicios/Pendientes_Partes_refacciones
        [Route("Pendientes_Partes_refacciones")]
        [HttpPost("{Pendientes_Partes_refacciones}")]
        public IActionResult Pendientes_Partes_refacciones([FromBody] Visita id)
        {
            var item = (from a in _context.Rel_servicio_Refaccion
                        join c_p in _context.Cliente_Productos on a.id_producto equals c_p.Id
                        join b in _context.CatEstatus_Producto on a.estatus equals b.id
                        //join c in _context.Cat_Materiales on b.id_material equals c.id
                        join e in _context.Cat_Productos on c_p.Id_Producto equals e.id
                        where a.id_vista == id.id //&& a.estatus == 2 || a.estatus == 1
                        select new
                        {
                            a.id,
                            id_producto = e.id,
                            e.nombre,
                            b.desc_estatus_producto,
                            a.comentarios,
                            id_rel = a.id,
                            refacciones = (from aa in _context.Rel_servicio_Refaccion
                                           join bb in _context.Piezas_Repuesto on aa.id equals bb.id_rel_servicio_refaccion
                                           join cc in _context.Cat_Materiales on bb.id_material equals cc.id
                                           //join e in _context.Visita_solicitada on a.id equals e.id_servicio into _scts
                                           //from scts in _scts.DefaultIfEmpty()
                                           where aa.id_vista == id.id && aa.id_producto == a.id_producto
                                           select new
                                           {
                                               // = b.id,
                                               id_rel = bb.id,
                                               bb.id,
                                               id_material = cc.id,
                                               cc.no_material,
                                               cc.descripcion,
                                               bb.cantidad,
                                               bb.solicitada,
                                               bb.llegada,
                                               comentarios = bb.comentarios == null ? "" : bb.comentarios
                                           }).ToList()

                        }).ToList();

            if (item.Count() == 0)
            {
                return new ObjectResult(item); ;
            }
            return new ObjectResult(item);
        }

        [Route("Visita_solicitada")]
        [HttpPost("{Visita_solicitada}")]
        public IActionResult Visita_solicitada([FromBody] Piezas_Repuesto item)
        {
            var result = new Models.Response();

            try
            {
                var refaccion = _context.Piezas_Repuesto.FirstOrDefault(t => t.id == item.id);

                if (refaccion == null)
                {

                }
                else
                {
                    refaccion.solicitada = item.solicitada;

                    _context.Piezas_Repuesto.Update(refaccion);
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

        [HttpGet("servicios_sin_pagos")]
        public IActionResult editaComprobante()
        {

            try
            {
                var lista = _context.Servicio_Sin_Pagos.Where(t => t.Estatus == true).OrderBy(t => t.nombre);

                if (lista == null)
                {
                    return Ok(new { response = "Error" });
                }
                else
                {
                    return Ok(new { response = "Success", lista });
                }
            }
            catch (Exception ex)
            {
                return Ok(new { response = "Error", detalle = ex.Message });
            }

        }



        [Route("Comprobante_Pago")]
        [HttpPost("{Comprobante_Pago}")]
        public IActionResult Comprobante_Pago([FromBody] Visita item)
        {
            var result = new Models.Response();

            try
            {
                var visita = _context.Visita.FirstOrDefault(t => t.id == item.id);

                if (visita == null)
                {
                    result = new Models.Response
                    {
                        response = "Error"
                    };
                }
                else
                {
                    visita.fecha_deposito = item.fecha_deposito;
                    visita.comprobante = item.comprobante;

                    _context.Visita.Update(visita);
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



        [Route("Visita_llegada")]
        [HttpPost("{Visita_llegada}")]
        public IActionResult Visita_llegada([FromBody] Piezas_Repuesto item)
        {
            var result = new Models.Response();

            try
            {
                var refaccion = _context.Piezas_Repuesto.FirstOrDefault(t => t.id == item.id);

                if (refaccion == null)
                {

                }
                else
                {
                    refaccion.llegada = item.llegada;

                    _context.Piezas_Repuesto.Update(refaccion);
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

        [Route("Editar_refaccion_visita")]
        [HttpPost("{Editar_refaccion_visita}")]
        public IActionResult Editar_refaccion_visita([FromBody] Visita item)
        {
            var result = new Models.Response();

            try
            {
                var refaccion = _context.Piezas_Repuesto.FirstOrDefault(t => t.id == item.id);

                if (refaccion == null)
                {
                    return NotFound();
                }
                else
                {
                    refaccion.id_material = Convert.ToInt64(item.no_operacion);

                    _context.Piezas_Repuesto.Update(refaccion);
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

        [Route("Actualizar_comentario")]
        [HttpPost("{Actualizar_comentario}")]
        public IActionResult Actualizar_comentario([FromBody] Piezas_Repuesto item)
        {
            var result = new Models.Response();

            try
            {
                var refaccion = _context.Piezas_Repuesto.FirstOrDefault(t => t.id == item.id);

                if (refaccion == null)
                {

                }
                else
                {
                    refaccion.comentarios = item.comentarios;

                    _context.Piezas_Repuesto.Update(refaccion);
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

        [Route("Actualizar_estatus_solicitud")]
        [HttpPost("{Actualizar_estatus_solicitud}")]
        public IActionResult Actualizar_estatus_solicitud([FromBody] Rel_servicio_visita_Refaccion item)
        {
            var result = new Models.Response();

            try
            {
                var refaccion = _context.Rel_servicio_Refaccion.FirstOrDefault(t => t.id == item.id);
                var cliente = _context.Cliente_Productos.FirstOrDefault(t => t.Id == refaccion.id_producto);
                if (refaccion == null)
                {

                }
                else
                {
                    refaccion.estatus = item.estatus;
                    cliente.Id_EsatusCompra = item.estatus;

                    _context.Rel_servicio_Refaccion.Update(refaccion);
                    _context.Cliente_Productos.Update(cliente);
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

        // POST: api/Servicios/Busqueda_refaccion
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
                                where EF.Functions.Like(a.no_material, "%" + busqueda.desc_busqueda + "%")
                                orderby a.no_material descending
                                select new
                                {
                                    a.id,
                                    a.descripcion,
                                    a.no_material
                                }).Distinct().Take(1).ToList();

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
                            cliente = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(c.nombre) + " " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(c.paterno) + " " + (c.materno != "" ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(c.materno) : ""),
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
                                           direccion = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(d.calle_numero) + ", " + (d.numExt != null ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(d.numExt) : "S/N"),
                                           calle = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(d.calle_numero) + ", " + (d.numExt != null ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(d.numExt) : "S/N"),
                                           colonia = g.desc_localidad != null ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g.desc_localidad) : "S/N",
                                           cp = d.cp,
                                           desc_estado = e.desc_estado != null ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(e.desc_estado) : "S/N",
                                           d.telefono,
                                           d.telefono_movil,
                                           id_direccion = d.id,
                                           id_servicio = x.id_servicio,
                                           fecha_inicio_visita = Convert.ToDateTime(x.fecha_inicio_visita).ToString("dd/MM/yyyy"),
                                           fecha_visita = x.fecha_visita.ToString("dd/MM/yyyy"),
                                           x.imagen_firma,
                                           x.cantidad,
                                           fecha_limite = x.fecha_limite_pago,
                                           x.hora,
                                           productos = (from rel in _context.Rel_servicio_producto
                                                        join c_p in _context.Cliente_Productos on rel.id_producto equals c_p.Id
                                                        join e in _context.Cat_Productos on c_p.Id_Producto equals e.id
                                                        join f in _context.Cat_SubLinea_Producto on e.id_sublinea equals f.id
                                                        join estatus in _context.CatEstatus_Producto on rel.estatus equals estatus.id into t
                                                        from e_estatus in t.DefaultIfEmpty()
                                                        where rel.id_vista == x.id
                                                        select new
                                                        {
                                                            id = e.id,
                                                            sku = e.sku,
                                                            modelo = e.modelo,
                                                            no_serie = c_p.no_serie == null ? "" : c_p.no_serie,
                                                            nombre = e.nombre != null ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(e.nombre) : "S/N",
                                                            categoria = (f.descripcion == null) ? "" : f.descripcion.ToUpper()
                                                        }).Distinct().ToList(),
                                           tecnico = (from visita_tecnico in _context.rel_tecnico_visita
                                                          //join tv in _context.rel_tecnico_visita on visita_tecnico.id equals tv.id_vista
                                                      join i in _context.Tecnicos on visita_tecnico.id_tecnico equals i.id
                                                      join j in _context.Users on i.id equals j.id
                                                      where visita_tecnico.id_vista == x.id
                                                      select new
                                                      {
                                                          nombre = j.name != null ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(j.name) : "S/N",
                                                          visita_tecnico.tecnico_responsable
                                                      }),
                                           checklist = (from w in _context.Producto_Check_List_Respuestas
                                                        join serie in _context.Rel_servicio_producto on w.id_vista equals serie.id_vista
                                                        join c_p in _context.Cliente_Productos on serie.id_producto equals c_p.Id
                                                        join prod in _context.Cat_Productos on c_p.Id_Producto equals prod.id
                                                        join ec in _context.Cat_Checklist_Producto on w.estatus equals ec.id
                                                        where w.id_vista == id_visita //&& visita.id_servicio == id && serie.id_vista == id_visita
                                                        select new
                                                        {
                                                            estatus_prodcuto_checklist = ec.desc_checklist_producto,
                                                            prod.nombre,
                                                            prod.modelo,
                                                            c_p.no_serie,
                                                            serie.garantia,
                                                            imagenes_checklist = (from xz in _context.Rel_Imagen_Producto_Visita
                                                                                  where xz.id_visita == id_visita && xz.id_producto == w.id_producto && xz.checklist == true
                                                                                  select new
                                                                                  {
                                                                                      id = xz.id,
                                                                                      url = xz.path,
                                                                                      xz.actividad
                                                                                  }).ToList(),
                                                            preguntas = (from check in _context.Producto_Check_List_Respuestas
                                                                         join respuesta in _context.Check_List_Respuestas on check.id equals respuesta.id_producto_check_list_respuestas
                                                                         join pregunta in _context.Check_List_Preguntas on respuesta.id_pregunta equals pregunta.id
                                                                         where check.id_vista == id_visita && check.id_producto == c_p.Id_Producto //&& visita_c.id_servicio == id && check.id_producto == prod.id && check.id_vista == id_visita
                                                                         select new
                                                                         {
                                                                             pregunta = pregunta.pregunta,
                                                                             pregunta_en = pregunta.pregunta_en,
                                                                             respuesta = respuesta.respuesta,
                                                                             comentario = respuesta.comentarios,
                                                                         }).Distinct().ToList()
                                                        }).ToList(),

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
                                                        <img heigth='20%' width='20%' src='" + selRuta.funcion + @"/mieletickets/Imagenes_Productos/miele-logo-immer-besser.png'>
                                                    </td>
                                                 </tr>
                                              </table>
                                              <table style='width: 100%; text-align: left'>
                                                  <tr>
                                                    <td>
                                                        <h3>Confirmación de Orden de Servicio
                                                            <span style = 'color:#A5000D'>" + item[0].id); sb.Append(@" </span>
                                                        </h3>
                                                    </td>
                                                 </tr>
                                              </table>
                                            </div>
                                            <div class='col-md-12'>
                                                 <h4>Estimado (a) cliente, por este medio le enviamos la confirmación del servicio solicitado, en caso de alguna duda, error en sus datos o cancelación por favor contactenos al teléfono 01-800 MIELE 00 (01 800 64353 00)</h4>
                                                 
                                            </div>
                                            <div class='col-md-12'>
                                                 <hr style='border-color:black 4px;'>
                                            <div>
                                            <div class='col-md-12'>
                                                 
                                                 <h4><span style = 'color:#A5000D'>Tipo de servicio: </span>" + item[0].tipo_servicio);
            sb.Append(@" 
                                                 <h4><span style = 'color:#A5000D'>Fecha de atención: </span>" + item[0].visitas[0].fecha_visita); sb.Append(@" </h4>
                                                 
                                                 <h4><span style = 'color:#A5000D'>Cliente: </span>" + item[0].cliente); sb.Append(@" </h4>                                                
                                            </div>
                                            <div class='col-md-12'>
                                                <table style='width: 90%; text-align: left'>
                                                  <tr>
                                                    <td>
                                                        <h4><span style = 'color:#A5000D'> Calle y número: </span><span style = 'color:#000000'>" + item[0].visitas[0].calle); sb.Append(@"</span></h4>
                                                        <h4><span style = 'color:#A5000D'> Colonia: </span><span>" + item[0].visitas[0].colonia); sb.Append(@"</span></h4>
                                                        <h4><span style = 'color:#A5000D'> C.P: </span><span>" + item[0].visitas[0].cp); sb.Append(@"</span></h4>
                                                        <h4><span style = 'color:#A5000D'> Estado: </span><span>" + item[0].visitas[0].desc_estado); sb.Append(@"</span></h4>
                                                    </td>
                                                    <td>
                                                        <h4><span style = 'color:#A5000D'> Teléfono: </span><span>" + item[0].cliente_telefono); sb.Append(@"</span></h4>                                                        
                                                        <h4><span style = 'color:#A5000D'> Cel: </span><span>" + item[0].cliente_movil); sb.Append(@"</span></h4>
                                                        <h4><span style = 'color:#A5000D'> e-mail de cliente: </span><span>" + item[0].email); sb.Append(@"</span></h4>
                                                        <h4><span style = 'color:#A5000D'> e-mail de contacto: </span><span>" + item[0].contacto); sb.Append(@"</span></h4>
                                                    </td>
                                                 </tr>
                                              </table>
                                             </div>
<div class='col-md-12'>
                                                 <hr style='border-color:black 4px;'>
                                            <div>");
            if (item[0].visitas[0].productos.Count() != 0)
            {
                sb.Append(@"
                                                                                                                                    <div class='col-md-12' style='margin-bottom: 20px; padding-left: 0'>
                                                                                                                                    <h3>Equipos a revisar</h2>");

                sb.Append(@" <table style = 'width: 100%; text-align: left; margin-bottom: 10px;'>
                    <tr>
                            <td><h4><span style = 'color:#A5000D'>Producto</span></h4></td>
                            <td><h4><span style = 'color:#A5000D'>Modelo</span></h4></td>
                            <td><h4><span style = 'color:#A5000D'>No. Serie</span></h4></td>
                        </tr>
                ");
                foreach (var emp in item[0].visitas[0].productos)
                {
                    sb.Append(@"
                        
                        <tr>
                                                                                                                                            <td>
                                                                                                                                                  <h4>" + emp.nombre); sb.Append(@" </h4>
                                                                                                                                            </td>
                                                                                                                                             <td>
                                                                                                                                                  <h4>" + emp.modelo); sb.Append(@" </h4>
                                                                                                                                             </td>
                                                                                                                                             <td>
                                                                                                                                                  <h4>" + emp.no_serie); sb.Append(@" </h4>
                                                                                                                                             </td>
                                                                                                                                           </tr>
                        ");
                }
                sb.Append(@"</table>
                                                                                                                                 </div>");

            }

            sb.Append(@"<hr style='border-color:black 4px;'>
                        <div class='col-md-12' style='padding-left: 0'>
                                                                                                    <h3>Consideraciones:</h3><br/>");
            //consideraciones version anterior a mejoras abril 2019
            //                                                                                        <span>- En el caso de aplicar garantía, el servicio no tendra costo.</span><br/>");
            //if (item[0].id_tipo_servicio == 1)
            //{
            //    sb.Append(@"<span> - El costo por visita de servicio fuera de garantia es de $890 pesos por un equipo y $490 pesos a partir del segundo.no incluye refacciones </span><br/>");
            //}
            //else
            //{
            //    sb.Append(@"<span>- El costo por visita de servicio fuera de garantia es de $890 pesos por un equipo y $690 pesos a partir del segundo, no incluye refacciones </span><br/>");
            //    sb.Append(@"<span>- Las refacciones necesarias para la reparación seran cotizadas por separado asi como la mano de obra</span><br/>");
            //}
            //sb.Append(@"


            //                                                                                        <span>- Durante la visita solo se atenderán los equipos listados en esta confirmación de servicio</span><br/>
            //                                                                                        <span>- Metodo de pago: Efectivo, Transferencia, Tarjeta Bancaria / American Express</span>");
            sb.Append(@"<div class='col-md-12' style='text-align: left; font-size:12px; letter-spacing:0.6pt; vertical-align:top'> 
						<span>
 - El presupuesto para la atención de este servicio es de " + item[0].visitas[0].cantidad.ToString("C", new CultureInfo("es-MX")) + @" pesos
<br /> - Si Usted no realizó su pago en línea con Miele, le pedimos realizarlo mediante depósito bancario, antes del día " + item[0].visitas[0].fecha_limite + @" con los siguientes datos:
<table style='width:100%; border: 1px solid #000;'>
<tr bgcolor='#D3D3D3'>
<td><h> Banco </h></td>
<td><h> Cliente</h></td>
<td><h> A nombre de </h></td>
<td><h> Sucursal </h></td>
<td><h> Cuenta </h></td>
<td><h> CLABE </h></td>
<td><h> Destinado a </h></td>
<td><h> Referencia </h></td>
</tr>
<tr>
<td><h> Banamex </h></td>
<td><h> 62442172 </h></td>
<td><h> Miele S.A. de C.V. </h></td>
<td><h> 6503 </h></td>
<td><h> 4124307 </h></td>
<td><h> 002180650341243072 </h></td>
<td><h> Servicio Técnico </h></td>
<td><h> " + item[0].id.ToString() + @"</h></td>
</tr>
</table>
- Una vez realizado el depósito, por favor envíe el comprobante de pago al correo info@miele.com.mx de otra manera su servicio no podrá ser atendido.
<br /> - El servicio inicial de revisión y/o diagnóstico, no incluye el costo de las refacciones que pudieran ser necesarias, así como la mano de obra.
<br /> - Durante la visita del técnico, solo se atenderán los servicios listados en esta confirmación. En caso de requerir que un equipo adicional sea atendido, por favor solicítelo llamando a nuestro Contact Center para realizar una nueva programación.
						</span>
                                                </div>");

            sb.Append(@"</div>
                                      </div>
                                 </div>
                            </body>
                        </html>");

            return sb.ToString();
        }

        //[Route("test_confservi")]
        //[HttpPost("{test_confservi}")]
        //public IActionResult test_confservi([FromBody] busqueda busqueda)
        //{
        //    var ss = CreatePDFOordenServicio_Distribuidor(18, 11225);
        //    return Ok("Success");
        //}

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
            var _pdf_envio = _context.Visita.SingleOrDefault(x => x.id == id_visita);

            var objectSettings = new ObjectSettings
            {
                PagesCount = true,

                HtmlContent = TemplateOrdenServicioPDF(id, id_visita),
                //Page = "https://www.amigosdetechodigital.com.mx//#/", //USE THIS PROPERTY TO GENERATE PDF CONTENT FROM AN HTML PAGE
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Line = true, Right = "Página [page] de [toPage]" },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Av. Santa Fe 170, Col. Lomas de Santa Fe, C.P. 01210, Ciudad de México. Contacto.  Tel: 01 800 MIELE 00 (01 800 64353 00)" }
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
                            nombre = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(b.nombre) + " " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(b.paterno) + " " + (b.materno != null ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(b.materno) : ""),
                            mail = b.email,
                            mail2 = b.nombre_comercial,
                            contacto = a.contacto
                        }).ToList();

            _pdf_envio.url_pdf_confirmacion_visita = path;
            var path_template = Path.GetFullPath("TemplateMail/Email_orden_servicio.html");
            StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_orden_servicio.html"));
            string body = string.Empty;
            body = reader.ReadToEnd();
            body = body.Replace("{username}", "Estimado (a) cliente: " + item[0].nombre);
            body = body.Replace("{no_servicio}", item[0].id.ToString());

            var fs = new FileStream(filePath + path, FileMode.Open);
            List<Attachment> adjuntos = new List<Attachment>();
            adjuntos.Add(new Attachment(fs, "Confirmación de orden de Servicio.pdf", "text/pdf"));


            //var send_email = item[0].mail;
            //email uno para el cliente
            //_emailRepository.SendAttachment(item[0].mail, "Miele - Confirmación de servicio 1" + item[0].id, true, body, 1, adjuntos);
            List<MailAddress> mails = new List<MailAddress>();
            mails.Add(new MailAddress(item[0].mail));
            mails.Add(new MailAddress("info@miele.com"));
            if (item[0].contacto != "")
            {
                bool valida = IsValidEmail(item[0].contacto);
                if (valida)
                {
                    mails.Add(new MailAddress(item[0].contacto));
                    //send_email = send_email + "," + item[0].contacto;
                    //email 2 para el distribuidor
                    //_emailRepository.SendAttachment(item[0].contacto, "Miele - Confirmación de servicio 2" + item[0].id, true, body, 1, adjuntos); // subject del correo
                    //send_email = item[0].mail;
                }
            }
            if (item[0].mail2 != "")
            {
                bool valida = IsValidEmail(item[0].mail2);
                if (valida)
                    mails.Add(new MailAddress(item[0].mail2));
            }

            //email 3 de respaldo para miele
           
            _emailRepository.SendMultipleMails(mails, "Miele - Confirmación de servicio " + item[0].id, true, body, 1, adjuntos); // subject del correo
            _context.Visita.Update(_pdf_envio);
            _context.SaveChanges();
            return "Successfully created PDF document.";

        }

        public string CreatePDFOordenServicio_Distribuidor(long id, long id_visita)
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
                HeaderSettings = { FontName = "Arial", FontSize = 9, Line = true, Right = "Página [page] de [toPage]" },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Av. Santa Fe 170, Col. Lomas de Santa Fe, C.P. 01210, Ciudad de México. Contacto.  Tel: 01 800 MIELE 00 (01 800 64353 00)" }
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
                            a.contacto,
                            nombre = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(b.nombre) + " " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(b.paterno) + " " + (b.materno != "" ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(b.materno) : ""),
                            mail = b.email
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
            //    To = { item[0].contacto },
            //    Subject = "Miele - Confirmación de servicio " + item[0].id,
            //    IsBodyHtml = true,
            //    Body = body

            //})


            //var smtp = new SmtpClient
            //{
            //    Host = _emailSettings.PrimaryDomain,
            //    Port = _emailSettings.PrimaryPort,
            //    EnableSsl = _emailSettings.EnableSsl,
            //    DeliveryMethod = SmtpDeliveryMethod.Network,
            //    UseDefaultCredentials = false,
            //    Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword)
            //};
            //using (var message = new MailMessage
            //{
            //    From = new MailAddress("no-reply@miele.com.mx", _emailSettings.MailName),
            //    To = { item[0].contacto },
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
            adjuntos.Add(new Attachment(fs, "Confirmación de orden de Servicio.pdf", "text/pdf"));

            _emailRepository.SendAttachment(item[0].contacto, "Miele - Confirmación de servicio " + item[0].id, true, body, 1, adjuntos); // subject del correo

            return "Successfully created PDF document.";

        }

        //Certificados de mantenimiento 
        // POST: api/Servicios/consumibles_por_producto
        [Route("consumibles_por_producto")]
        [HttpPost("{consumibles_por_producto}")]
        public IActionResult consumibles_por_producto([FromBody] Cer_consumibles rel)
        {
            IActionResult response;

            try
            {
                if (rel == null)
                {
                    return NotFound();
                }
                else
                {
                    //var myInClause = new int[] { 52, 21, 6, 51 };
                    List<int> myInClause = new List<int>() { };
                    for (int i = 0; i < rel.consumible.Split(',').Count(); i++)
                    {
                        if (rel.consumible.Split(',')[i] != "")
                        {
                            myInClause.Add(Convert.ToInt32(rel.consumible.Split(',')[i]));
                        }
                    }
                    var item = (from a in _context.rel_consumible_sublinea
                                join b in _context.Cer_consumibles on a.id_consumible equals b.id
                                where myInClause.Contains(a.id_sublinea)
                                select new
                                {
                                    b.id,
                                    b.consumible,
                                    b.costo_unitario
                                }).Distinct().ToList();

                    if (item.Count == 0)
                    {
                        return response = Ok(new { item = "No hay consumibles" });
                    }
                    return new ObjectResult(item);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }

        // POST: api/Servicios/Nuevo_certificado
        [Route("Nuevo_certificado")]
        [HttpPost("{Nuevo_certificado}")]
        public IActionResult Nuevo_certificado([FromBody]Cer_producto_cliente item)
        {
            IActionResult response;
            //for (int i = 0; i < item.rel_certificado_producto.Count(); i++)
            //{

            //}
            var count = _context.Cer_producto_cliente.OrderByDescending(x => x.id).Take(1).ToList();
            int numeroCm = 0;
            if (count.Count() == 0)
            {
                numeroCm = 0 + 1;
            }
            else
            {
                numeroCm = count[0].id + 1;
            }
            item.folio = "CM" + DateTime.Now.ToString("ddMMyy") + string.Format("{0:000000}", numeroCm);
            try
            {
                if (item == null)
                {
                    response = Ok(new { response = "Error, intenta mas tarde" });
                }
                else
                {
                    _context.Cer_producto_cliente.Add(item);
                    _context.SaveChanges();

                    response = Ok(new { response = "Certificado creado correctamente" });
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { id = "0", response = ex.ToString() });
            }
            return new ObjectResult(response);
        }

        // POST: api/Servicios/cer_detalle
        [Route("cer_detalle")]
        [HttpPost("{cer_detalle}")]
        public IActionResult cer_detalle([FromBody] Cer_producto_cliente rel)
        {
            IActionResult response;
            List<int> myInClause = new List<int>() { 1, 4 };
            try
            {
                if (rel == null)
                {
                    return NotFound();
                }
                else
                {
                    var item = (from a in _context.Cer_producto_cliente
                                where a.id_cliente == rel.id_cliente //&& f.id_cliente == producto.id_cliente
                                select new
                                {
                                    a.id,
                                    a.folio,
                                    a.costo,
                                    estatus_activo = a.estatus_venta == false ? "Sin pagar" : "Pagado",
                                    poliza = (from e in _context.Cat_Productos
                                              join c_p in _context.Cliente_Productos on e.id equals c_p.Id_Producto
                                              join g in _context.rel_certificado_producto on c_p.Id equals g.id_producto
                                              where g.id_certificado == a.id
                                              select new
                                              {
                                                  e.id,
                                                  e.nombre,
                                                  g.no_visitas,
                                                  estatus_activo = g.estatus_activo == false ? "Inactivo" : "Activo"
                                              }),
                                    sublinea = (from e in _context.Cat_Productos
                                                join g in _context.rel_certificado_producto on e.id_sublinea equals g.id_sub_linea
                                                where g.id_certificado == a.id && myInClause.Contains(e.tipo)
                                                select new
                                                {
                                                    id_rel_cer = g.id,
                                                    e.id,
                                                    e.nombre,
                                                    e.modelo
                                                })
                                }).ToList();

                    if (item.Count == 0)
                    {
                        return response = Ok(new { id = 0, folio = "", costo = 0 });
                    }
                    return new ObjectResult(item);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }

        // POST: api/Servicios/cer_detalle
        [Route("cer_producto_detalle")]
        [HttpPost("{cer_producto_detalle}")]
        public IActionResult cer_producto_detalle([FromBody] Cer_producto_cliente rel)
        {
            IActionResult response;

            try
            {
                if (rel == null)
                {
                    return NotFound();
                }
                else
                {
                    var poliza = (from e in _context.Cat_Productos
                                  join g in _context.rel_certificado_producto on e.id equals g.id_producto
                                  join h in _context.Cer_producto_cliente on g.id_certificado equals h.id
                                  where g.id_certificado == rel.id
                                  select new
                                  {
                                      e.id,
                                      e.nombre,
                                      g.no_visitas,
                                      g.estatus_activo,
                                  }).ToList();

                    if (poliza.Count == 0)
                    {
                        return response = Ok(new { item = "No hay productos" });
                    }
                    return new ObjectResult(poliza);
                }
            }
            catch (Exception ex)
            {
                return new ObjectResult(ex);
            }
        }

        // POST: api/Servicios/asignar_producto_cer
        [Route("asignar_producto_cer")]
        [HttpPost("{asignar_producto_cer}")]
        public IActionResult asignar_producto_cer([FromBody] rel_certificado_producto_productos item)
        {
            var result = new Models.Response();

            try
            {
                var cer = _context.rel_certificado_producto.FirstOrDefault(t => t.id == item.id);
                var cliente = (from a in _context.Cliente_Productos
                               join b in _context.Cat_Productos on a.Id_Producto equals b.id
                               join c in _context.rel_certificado_producto on a.Id equals c.id_producto into _rel_certificado_producto
                               from rel_certificado_producto in _rel_certificado_producto.DefaultIfEmpty()
                               where a.Id_Cliente == item.Id_Cliente
                               select new
                               {
                                   a.Id,
                                   b.id_sublinea,
                                   id_producto_cer = rel_certificado_producto.id_producto == null ? 0 : rel_certificado_producto.id_producto,
                                   no_visita = rel_certificado_producto.no_visitas == null ? 0 : rel_certificado_producto.no_visitas
                               }).ToList();

                bool _is_true = false;
                for (int i = 0; i < cliente.Count(); i++)
                {
                    if (cliente[i].id_sublinea == cer.id_sub_linea)
                    {
                        if (cliente[i].id_producto_cer == 0)
                        {
                            cer.id_producto = Convert.ToInt32(cliente[i].Id);
                            _context.rel_certificado_producto.Update(cer);
                            _context.SaveChanges();
                            _is_true = true;
                            break;
                        }
                        else
                        {
                            if (cliente[i].no_visita == 2)
                            {
                                cer.id_producto = Convert.ToInt32(cliente[i].Id);
                                _context.rel_certificado_producto.Update(cer);
                                _context.SaveChanges();
                                _is_true = true;
                                break;
                            }
                        }

                    }
                }


                if (!_is_true)
                {
                    Cliente_Productos cliente_producto = new Cliente_Productos();
                    cliente_producto.Id_Cliente = item.Id_Cliente;
                    cliente_producto.Id_EsatusCompra = item.Id_EsatusCompra;
                    cliente_producto.Id_Producto = item.id_producto;

                    _context.Cliente_Productos.Add(cliente_producto);
                    _context.SaveChanges();

                    cer.id_producto = Convert.ToInt32(cliente_producto.Id);
                    _context.rel_certificado_producto.Update(cer);
                    _context.SaveChanges();
                }

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

        // POST: api/Servicios/actualizar_certificado
        [Route("actualizar_certificado")]
        [HttpPost("{actualizar_certificado}")]
        public IActionResult actualizar_certificado([FromBody] Cat_Productos item)
        {
            IActionResult response;

            try
            {
                var cer = _context.Cer_producto_cliente.FirstOrDefault(t => t.id == item.id);
                var rel_cer = (from a in _context.rel_certificado_producto
                               where a.id_certificado == cer.id
                               select new
                               {
                                   a.id,
                                   a.estatus_activo
                               }).ToList();

                if (cer == null)
                {
                    return NotFound();
                }
                else
                {
                    cer.estatus_venta = true;
                    foreach (var ce in rel_cer)
                    {
                        var cer_rel_ce = _context.rel_certificado_producto.FirstOrDefault(t => t.id == ce.id);
                        cer_rel_ce.estatus_activo = true;
                        _context.rel_certificado_producto.Update(cer_rel_ce);
                    }

                    _context.Cer_producto_cliente.Update(cer);
                    _context.SaveChanges();

                    response = Ok(new { response = "Pago exitoso", });
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { response = ex.ToString() });
            }

            return new ObjectResult(response);
        }

        // POST: api/Servicios/Queja_servicio
        [Route("Queja_servicio")]
        [HttpPost("{Queja_servicio}")]
        public IActionResult Queja_servicio([FromBody]quejas_servicios item)
        {
            IActionResult response;

            try
            {
                if (item == null)
                {
                    response = Ok(new { response = "Error, intenta mas tarde" });
                }
                else
                {
                    _context.quejas_servicios.Add(item);
                    _context.SaveChanges();

                    response = Ok(new { response = "OK" });
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { id = "0", response = ex.ToString() });
            }
            return new ObjectResult(response);
        }

        // POST: api/Servicios/Cliente_certificado
        [Route("Cliente_certificado")]
        [HttpPost("{Cliente_certificado}")]
        public IActionResult Cliente_certificado([FromBody]Cer_producto_cliente item)
        {
            IActionResult response;

            try
            {
                if (item == null)
                {
                    return NotFound();
                }
                else
                {
                    List<long> myInClause = new List<long>() { };
                    for (int i = 0; i < item.folio.Split(',').Count(); i++)
                    {
                        if (item.folio.Split(',')[i] != "")
                        {
                            myInClause.Add(Convert.ToInt64(item.folio.Split(',')[i]));
                        }
                    }

                    var respuesta = (from a in _context.Cer_producto_cliente
                                     join b in _context.rel_certificado_producto on a.id equals b.id_certificado
                                     where a.id_cliente == item.id_cliente && a.estatus_venta == true && b.estatus_activo == true && (myInClause).Contains(b.id_sub_linea)
                                     select new
                                     {
                                         a.id,
                                         a.id_cliente,
                                         b.estatus_activo,
                                         b.no_visitas
                                     }).Distinct().ToList();

                    if (respuesta.Count == 0)
                    {
                        return new ObjectResult(respuesta);
                    }

                    return new ObjectResult(respuesta);
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { id = "0", response = ex.ToString() });
            }
            return new ObjectResult(response);
        }

        // POST: api/Servicios/Busqueda_Quejas
        [Route("Busqueda_Quejas")]
        [HttpPost("{Busqueda_Quejas}")]
        public IActionResult Busqueda_Quejas([FromBody] busqueda busqueda)
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
                    var item = (from a in _context.Quejas
                                    //join pr in _context.Propuestas on a.Id equals pr.QuejaId
                                join b in _context.Clientes on a.ClienteId equals b.id
                                where EF.Functions.Like(a.Folio + " " + b.nombre + " " + b.paterno, "%" + busqueda.desc_busqueda + "%")
                                orderby a.Folio descending
                                select new
                                {
                                    a.Id,
                                    a.Folio,
                                    cliente = a.Cliente.nombre + " " + a.Cliente.paterno + " " + (a.Cliente.materno == null ? "" : a.Cliente.materno),
                                    canal = a.Canal.Nombre,
                                    a.Atendio,
                                    a.DetalleReclamo,
                                    Propuestas = (from pro in _context.Propuestas
                                                  where pro.QuejaId == a.Id
                                                  select new
                                                  {
                                                      pro.Solucion,
                                                      estatus_queja = (pro.FechaCierre != null ? "Cerrada" : "Abierta"),
                                                      pro.DetalleCierre,
                                                      pro.FechaCierre
                                                  }).Distinct().ToList(), //a.Propuestas.Select(x=>x.Solucion).Distinct().ToList(),
                                    a.Telefono,
                                    tipo_queja = a.TipoQueja.Nombre,
                                    //estatus_queja = (pr.FechaCierre != null ? "Cerrada" : "Abierta"),
                                    servicio = (from c in _context.quejas_servicios
                                                join d in _context.Servicio on c.id_servicio equals d.id
                                                where c.id_queja == a.Id
                                                select new
                                                {
                                                    d.id,
                                                    productos = (from e in _context.Rel_servicio_producto
                                                                 join c_p in _context.Cliente_Productos on e.id_producto equals c_p.Id
                                                                 join f in _context.Cat_Productos on c_p.Id_Producto equals f.id
                                                                 join g in _context.Cliente_Productos on e.id_producto equals g.Id_Producto
                                                                 join h in _context.CatEstatus_Producto on g.Id_EsatusCompra equals h.id
                                                                 where g.Id_Cliente == b.id && e.visita.id_servicio == c.id_servicio
                                                                 select new
                                                                 {
                                                                     f.nombre,
                                                                     no_serie = c_p.no_serie == null ? "" : c_p.no_serie,
                                                                     e.estatus,
                                                                     g.Id_EsatusCompra,
                                                                     h.desc_estatus_producto
                                                                 }).ToList()
                                                }).ToList()
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

        [Route("test")]
        [HttpPost("test")]
        public IActionResult test([FromBody] Visita vi)
        {
            int id_visita = 11221;
            int id = 14;
            IActionResult response;
            var selRuta = _context.parametro_Archivos.FirstOrDefault(p => p.id == 6);
            string path;
            if (selRuta == null) path = "/Imagenes/";
            else
            {
                path = selRuta.ruta;
            }
            try
            {
                if (vi == null)
                {
                    return NotFound();
                }
                else
                {
                    //var item = (from a in _context.Servicio
                    //            join c in _context.Clientes on a.id_cliente equals c.id
                    //            join j in _context.Cat_tipo_servicio on a.id_tipo_servicio equals j.id
                    //            join k in _context.Cat_estatus_servicio on a.id_estatus_servicio equals k.id into tu
                    //            from fe in tu.DefaultIfEmpty()
                    //            where a.id == id
                    //            select new
                    //            {
                    //                a.id,
                    //                cliente = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(c.nombre) + " " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(c.paterno) + " " + (c.materno == "" ? "" : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(c.materno)),
                    //                cliente_telefono = c.telefono,
                    //                cliente_movil = c.telefono_movil,
                    //                tipo_servicio = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(j.desc_tipo_servicio),
                    //                estatus_servicio = (fe == null) ? "" : fe.desc_estatus_servicio,
                    //                contacto = a.contacto,
                    //                IBS = a.IBS,
                    //                actividades_realizar = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(a.descripcion_actividades),
                    //                a.descripcion_actividades,
                    //                piezas_cotizacion = (from y in _context.Piezas_Repuesto
                    //                                     join pc in _context.Rel_servicio_Refaccion on y.id_rel_servicio_refaccion equals pc.id
                    //                                     join u in _context.Cat_Materiales on y.id_material equals u.id
                    //                                     join pre in _context.Cat_Lista_Precios on u.id_grupo_precio equals Convert.ToInt32(pre.grupo_precio) //into g
                    //                                     where pc.id_vista == id_visita
                    //                                     select new
                    //                                     {
                    //                                         id = y.id,
                    //                                         refaccion = u.descripcion,
                    //                                         cantidad = y.cantidad,
                    //                                         precio_sin_iva = pre.precio_sin_iva * y.cantidad,
                    //                                         total_cantidad = (from a in _context.Piezas_Repuesto
                    //                                                           join b in _context.Rel_servicio_Refaccion on a.id_rel_servicio_refaccion equals b.id
                    //                                                           where b.id_vista == id_visita
                    //                                                           select a.cantidad).Sum(),
                    //                                         total_precio = (from a in _context.Piezas_Repuesto
                    //                                                         join aa in _context.Rel_servicio_Refaccion on a.id_rel_servicio_refaccion equals aa.id
                    //                                                         join b in _context.Cat_Materiales on a.id_material equals b.id
                    //                                                         join c in _context.Cat_Lista_Precios on b.id_grupo_precio equals Convert.ToInt32(c.grupo_precio)
                    //                                                         where aa.id_vista == id_visita
                    //                                                         select c.precio_sin_iva).Sum()
                    //                                     }).ToList(),
                    //                visitas = (from x in _context.Visita
                    //                           join d in _context.Cat_Direccion on x.id_direccion equals d.id
                    //                           join e in _context.Cat_Estado on d.id_estado equals e.id
                    //                           join f in _context.Cat_Municipio on d.id_municipio equals f.id
                    //                           join g in _context.Cat_Localidad on d.id_localidad equals g.id
                    //                           join kk in _context.CatEstatus_Visita on x.estatus equals kk.id into t
                    //                           from fee in t.DefaultIfEmpty()
                    //                           where x.id_servicio == id && x.id == id_visita
                    //                           select new
                    //                           {
                    //                               id = x.id,
                    //                               direccion = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(d.calle_numero + " " + (d.numExt == null ? "" : d.numExt) + " " + (d.numInt == null ? "" : d.numInt)) + ", " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g.desc_localidad) + ", " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(e.desc_estado) + ", " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(f.desc_municipio),
                    //                               fecha_inicio_visita = Convert.ToDateTime(x.fecha_inicio_visita).ToString("dd/MM/yyyy"),
                    //                               x.imagen_firma,
                    //                               persona_recibe = x.persona_recibe == null ? "" : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x.persona_recibe),
                    //                               tecnico = (from visita_tecnico in _context.rel_tecnico_visita
                    //                                          join i in _context.Tecnicos on visita_tecnico.id_tecnico equals i.id
                    //                                          join j in _context.Users on i.id equals j.id
                    //                                          where visita_tecnico.id_vista == id_visita && visita_tecnico.tecnico_responsable == true
                    //                                          select new
                    //                                          {
                    //                                              nombre = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(j.name) + " " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(j.paterno) + " " + (j.materno == null ? "" : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(j.materno)),
                    //                                              visita_tecnico.tecnico_responsable
                    //                                          }).Distinct().ToList(),
                    //                               checklist = (from w in _context.Producto_Check_List_Respuestas
                    //                                            join c_p in _context.Cliente_Productos on w.id_producto equals c_p.Id
                    //                                            join prod in _context.Cat_Productos on c_p.Id_Producto equals prod.id
                    //                                            join ec in _context.Cat_Checklist_Producto on w.estatus equals ec.id
                    //                                            join serie in _context.Rel_servicio_producto on c_p.Id equals serie.id_producto
                    //                                            where w.id_vista == id_visita //&& serie.id_vista == id_visita
                    //                                            select new
                    //                                            {
                    //                                                w.id_vista,
                    //                                                estatus_prodcuto_checklist = ec.desc_checklist_producto,
                    //                                                prod.nombre,
                    //                                                prod.modelo,
                    //                                                c_p.no_serie,
                    //                                                serie.garantia,
                    //                                                imagenes_checklist = (from xz in _context.Rel_Imagen_Producto_Visita
                    //                                                                      where xz.id_producto == c_p.Id && xz.checklist == true && xz.id_visita == id_visita
                    //                                                                      select new
                    //                                                                      {
                    //                                                                          id = xz.id,
                    //                                                                          url = xz.path,
                    //                                                                          xz.actividad
                    //                                                                      }).Distinct().ToList(),
                    //                                                preguntas = (from check in _context.Producto_Check_List_Respuestas
                    //                                                             join c_p in _context.Cliente_Productos on check.id_producto equals c_p.Id
                    //                                                             join respuesta in _context.Check_List_Respuestas on check.id equals respuesta.id_producto_check_list_respuestas
                    //                                                             join pregunta in _context.Check_List_Preguntas on respuesta.id_pregunta equals pregunta.id
                    //                                                             where check.id_vista == id_visita//&& check.id_producto == c_p.Id_Producto //&& visita_c.id_servicio == id && check.id_producto == prod.id && check.id_vista == id_visita
                    //                                                             select new
                    //                                                             {
                    //                                                                 pregunta = pregunta.pregunta,
                    //                                                                 respuesta = respuesta.respuesta,
                    //                                                                 comentario = respuesta.comentarios,
                    //                                                             }).Distinct().ToList()
                    //                                            }).Distinct().ToList(),
                    //                               reporte = (from a in _context.Visita
                    //                                          join c in _context.Servicio on a.id_servicio equals c.id
                    //                                          join e in _context.Cat_estatus_servicio on c.id_estatus_servicio equals e.id into es
                    //                                          from fd in es.DefaultIfEmpty()
                    //                                          where a.id == x.id
                    //                                          orderby a.fecha_visita ascending
                    //                                          select new
                    //                                          {
                    //                                              id_servicio = a.id_servicio,
                    //                                              estatus_servicio = (fd == null) ? "" : fd.desc_estatus_servicio,
                    //                                              id_visita = a.id,
                    //                                              productos = (from xe in _context.Rel_servicio_Refaccion
                    //                                                           join c_p in _context.Cliente_Productos on xe.id_producto equals c_p.Id
                    //                                                           join sev in _context.CatEstatus_Producto on xe.estatus equals sev.id
                    //                                                           join prod in _context.Cat_Productos on c_p.Id_Producto equals prod.id
                    //                                                           join serie in _context.Rel_servicio_producto on c_p.Id equals serie.id_producto //&& serie.id_vista equals 
                    //                                                           join repa in _context.cat_reparacion on serie.reparacion equals repa.id into _cat_reparacion
                    //                                                           from cat_reparacion in _cat_reparacion.DefaultIfEmpty()
                    //                                                           where xe.id_vista == id_visita && serie.id_vista == id_visita
                    //                                                           select new
                    //                                                           {
                    //                                                               id = prod.id,
                    //                                                               nombre_prodcuto = prod.nombre,
                    //                                                               estatus_producto = sev.desc_estatus_producto,
                    //                                                               id_estatus = xe.estatus,
                    //                                                               //estatus = sev.desc_estatus_producto_en,
                    //                                                               actividades = xe.actividades,
                    //                                                               fallas = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(xe.fallas.ToLower()),
                    //                                                               c_p.no_serie,
                    //                                                               serie.garantia,
                    //                                                               prod.modelo,
                    //                                                               cat_reparacion.desc_reparacion_es,
                    //                                                               imagenes_reparacion = (from xz in _context.Rel_Imagen_Producto_Visita
                    //                                                                                      where xz.id_visita == id_visita && xz.id_producto == c_p.Id && xz.checklist == false
                    //                                                                                      select new
                    //                                                                                      {
                    //                                                                                          id = xz.id,
                    //                                                                                          url = xz.path,
                    //                                                                                          xz.actividad
                    //                                                                                      }).ToList(),
                    //                                                               piezas_repuesto = (from y in _context.Piezas_Repuesto
                    //                                                                                  join u in _context.Cat_Materiales on y.id_material equals u.id
                    //                                                                                  join o in _context.Cat_Lista_Precios on u.id_grupo_precio equals Convert.ToInt32(o.grupo_precio)
                    //                                                                                  where y.id_rel_servicio_refaccion == xe.id
                    //                                                                                  select new
                    //                                                                                  {
                    //                                                                                      id = y.id,
                    //                                                                                      u.no_material,
                    //                                                                                      refaccion = u.descripcion,
                    //                                                                                      cantidad = y.cantidad,
                    //                                                                                      precio = Math.Round(o.precio_sin_iva, 2)

                    //                                                                                  }).ToList(),
                    //                                                               piezas_tecnico = (from y in _context.Piezas_Repuesto_Tecnico
                    //                                                                                 join u in _context.Cat_Materiales on y.id_material equals u.id
                    //                                                                                 where y.id_rel_servicio_refaccion == xe.id
                    //                                                                                 select new
                    //                                                                                 {
                    //                                                                                     id = y.id,
                    //                                                                                     refaccion = u.descripcion,
                    //                                                                                     cantidad = y.cantidad,
                    //                                                                                     u.no_material,
                    //                                                                                     precio = u.grupo_precio.precio_sin_iva
                    //                                                                                 }).ToList(),
                    //                                                               mano_de_obra = (from mo in _context.Rel_Categoria_Producto_Tipo_Producto
                    //                                                                               where mo.id_categoria == prod.id_sublinea && mo.id_tipo_servicio == xe.visita.servicio.id_tipo_servicio
                    //                                                                               select new
                    //                                                                               {
                    //                                                                                   mo.precio_hora_tecnico,
                    //                                                                                   mo.precio_visita
                    //                                                                               }).Take(1).ToList()


                    //                                                           }).ToList()
                    //                                          }).ToList()
                    //                           }).Distinct().ToList()
                    //            }).Distinct().ToList();

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
                                    cliente = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(c.nombre) + " " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(c.paterno) + " " + (c.materno != "" ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(c.materno) : ""),
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
                                                   direccion = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(d.calle_numero) + ", " + (d.numExt != null ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(d.numExt) : "S/N"),
                                                   calle = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(d.calle_numero) + ", " + (d.numExt != null ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(d.numExt) : "S/N"),
                                                   colonia = g.desc_localidad != null ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g.desc_localidad) : "S/N",
                                                   cp = d.cp,
                                                   desc_estado = e.desc_estado != null ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(e.desc_estado) : "S/N",
                                                   d.telefono,
                                                   d.telefono_movil,
                                                   id_direccion = d.id,
                                                   id_servicio = x.id_servicio,
                                                   fecha_inicio_visita = Convert.ToDateTime(x.fecha_inicio_visita).ToString("dd/MM/yyyy"),
                                                   fecha_visita = x.fecha_visita.ToString("dd/MM/yyyy"),
                                                   x.imagen_firma,
                                                   x.cantidad,
                                                   fecha_limite = x.fecha_limite_pago,
                                                   x.hora,
                                                   productos = (from rel in _context.Rel_servicio_producto
                                                                join c_p in _context.Cliente_Productos on rel.id_producto equals c_p.Id
                                                                join e in _context.Cat_Productos on c_p.Id_Producto equals e.id
                                                                join f in _context.Cat_SubLinea_Producto on e.id_sublinea equals f.id
                                                                join estatus in _context.CatEstatus_Producto on rel.estatus equals estatus.id into t
                                                                from e_estatus in t.DefaultIfEmpty()
                                                                where rel.id_vista == x.id
                                                                select new
                                                                {
                                                                    id = e.id,
                                                                    sku = e.sku,
                                                                    modelo = e.modelo,
                                                                    no_serie = c_p.no_serie == null ? "" : c_p.no_serie,
                                                                    nombre = e.nombre != null ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(e.nombre) : "S/N",
                                                                    categoria = (f.descripcion == null) ? "" : f.descripcion.ToUpper()
                                                                }).Distinct().ToList(),
                                                   tecnico = (from visita_tecnico in _context.rel_tecnico_visita
                                                                  //join tv in _context.rel_tecnico_visita on visita_tecnico.id equals tv.id_vista
                                                              join i in _context.Tecnicos on visita_tecnico.id_tecnico equals i.id
                                                              join j in _context.Users on i.id equals j.id
                                                              where visita_tecnico.id_vista == x.id
                                                              select new
                                                              {
                                                                  nombre = j.name != null ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(j.name) : "S/N",
                                                                  visita_tecnico.tecnico_responsable
                                                              }),
                                                   checklist = (from w in _context.Producto_Check_List_Respuestas
                                                                join serie in _context.Rel_servicio_producto on w.id_vista equals serie.id_vista
                                                                join c_p in _context.Cliente_Productos on serie.id_producto equals c_p.Id
                                                                join prod in _context.Cat_Productos on c_p.Id_Producto equals prod.id
                                                                join ec in _context.Cat_Checklist_Producto on w.estatus equals ec.id
                                                                where w.id_vista == id_visita //&& visita.id_servicio == id && serie.id_vista == id_visita
                                                                select new
                                                                {
                                                                    estatus_prodcuto_checklist = ec.desc_checklist_producto,
                                                                    prod.nombre,
                                                                    prod.modelo,
                                                                    c_p.no_serie,
                                                                    serie.garantia,
                                                                    imagenes_checklist = (from xz in _context.Rel_Imagen_Producto_Visita
                                                                                          where xz.id_visita == id_visita && xz.id_producto == w.id_producto && xz.checklist == true
                                                                                          select new
                                                                                          {
                                                                                              id = xz.id,
                                                                                              url = xz.path,
                                                                                              xz.actividad
                                                                                          }).ToList(),
                                                                    preguntas = (from check in _context.Producto_Check_List_Respuestas
                                                                                 join respuesta in _context.Check_List_Respuestas on check.id equals respuesta.id_producto_check_list_respuestas
                                                                                 join pregunta in _context.Check_List_Preguntas on respuesta.id_pregunta equals pregunta.id
                                                                                 where check.id_vista == id_visita && check.id_producto == c_p.Id_Producto //&& visita_c.id_servicio == id && check.id_producto == prod.id && check.id_vista == id_visita
                                                                                 select new
                                                                                 {
                                                                                     pregunta = pregunta.pregunta,
                                                                                     pregunta_en = pregunta.pregunta_en,
                                                                                     respuesta = respuesta.respuesta,
                                                                                     comentario = respuesta.comentarios,
                                                                                 }).Distinct().ToList()
                                                                }).ToList(),

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
                                                        <img heigth='20%' width='20%' src='" + selRuta.funcion + @"/mieletickets/Imagenes_Productos/miele-logo-immer-besser.png'>
                                                    </td>
                                                 </tr>
                                              </table>
                                              <table style='width: 100%; text-align: left'>
                                                  <tr>
                                                    <td>
                                                        <h3>Confirmación de Orden de Servicio
                                                            <span style = 'color:#A5000D'>" + item[0].id); sb.Append(@" </span>
                                                        </h3>
                                                    </td>
                                                 </tr>
                                              </table>
                                            </div>
                                            <div class='col-md-12'>
                                                 <h4>Estimado (a) cliente, por este medio le enviamos la confirmación del servicio solicitado, en caso de alguna duda, error en sus datos o cancelación por favor contactenos al teléfono 01-800 MIELE 00 (01 800 64353 00)</h4>
                                                 
                                            </div>
                                            <div class='col-md-12'>
                                                 <hr style='border-color:black 4px;'>
                                            <div>
                                            <div class='col-md-12'>
                                                 
                                                 <h4><span style = 'color:#A5000D'>Tipo de servicio: </span>" + item[0].tipo_servicio);
                    sb.Append(@" 
                                                 <h4><span style = 'color:#A5000D'>Fecha de atención: </span>" + item[0].visitas[0].fecha_visita); sb.Append(@" </h4>
                                                 
                                                 <h4><span style = 'color:#A5000D'>Cliente: </span>" + item[0].cliente); sb.Append(@" </h4>                                                
                                            </div>
                                            <div class='col-md-12'>
                                                <table style='width: 90%; text-align: left'>
                                                  <tr>
                                                    <td>
                                                        <h4><span style = 'color:#A5000D'> Calle y número: </span><span style = 'color:#000000'>" + item[0].visitas[0].calle); sb.Append(@"</span></h4>
                                                        <h4><span style = 'color:#A5000D'> Colonia: </span><span>" + item[0].visitas[0].colonia); sb.Append(@"</span></h4>
                                                        <h4><span style = 'color:#A5000D'> C.P: </span><span>" + item[0].visitas[0].cp); sb.Append(@"</span></h4>
                                                        <h4><span style = 'color:#A5000D'> Estado: </span><span>" + item[0].visitas[0].desc_estado); sb.Append(@"</span></h4>
                                                    </td>
                                                    <td>
                                                        <h4><span style = 'color:#A5000D'> Teléfono: </span><span>" + item[0].cliente_telefono); sb.Append(@"</span></h4>                                                        
                                                        <h4><span style = 'color:#A5000D'> Cel: </span><span>" + item[0].cliente_movil); sb.Append(@"</span></h4>
                                                        <h4><span style = 'color:#A5000D'> e-mail de cliente: </span><span>" + item[0].email); sb.Append(@"</span></h4>
                                                        <h4><span style = 'color:#A5000D'> e-mail de contacto: </span><span>" + item[0].contacto); sb.Append(@"</span></h4>
                                                    </td>
                                                 </tr>
                                              </table>
                                             </div>
<div class='col-md-12'>
                                                 <hr style='border-color:black 4px;'>
                                            <div>");
                    if (item[0].visitas[0].productos.Count() != 0)
                    {
                        sb.Append(@"
                                                                                                                                    <div class='col-md-12' style='margin-bottom: 20px; padding-left: 0'>
                                                                                                                                    <h3>Equipos a revisar</h2>");

                        sb.Append(@" <table style = 'width: 100%; text-align: left; margin-bottom: 10px;'>
                    <tr>
                            <td><h4><span style = 'color:#A5000D'>Producto</span></h4></td>
                            <td><h4><span style = 'color:#A5000D'>Modelo</span></h4></td>
                            <td><h4><span style = 'color:#A5000D'>No. Serie</span></h4></td>
                        </tr>
                ");
                        foreach (var emp in item[0].visitas[0].productos)
                        {
                            sb.Append(@"
                        
                        <tr>
                                                                                                                                            <td>
                                                                                                                                                  <h4>" + emp.nombre); sb.Append(@" </h4>
                                                                                                                                            </td>
                                                                                                                                             <td>
                                                                                                                                                  <h4>" + emp.modelo); sb.Append(@" </h4>
                                                                                                                                             </td>
                                                                                                                                             <td>
                                                                                                                                                  <h4>" + emp.no_serie); sb.Append(@" </h4>
                                                                                                                                             </td>
                                                                                                                                           </tr>
                        ");
                        }
                        sb.Append(@"</table>
                                                                                                                                 </div>");

                    }

                    sb.Append(@"<hr style='border-color:black 4px;'>
                        <div class='col-md-12' style='padding-left: 0'>
                                                                                                    <h3>Consideraciones:</h3><br/>");
                    //consideraciones version anterior a mejoras abril 2019
                    //                                                                                        <span>- En el caso de aplicar garantía, el servicio no tendra costo.</span><br/>");
                    //if (item[0].id_tipo_servicio == 1)
                    //{
                    //    sb.Append(@"<span> - El costo por visita de servicio fuera de garantia es de $890 pesos por un equipo y $490 pesos a partir del segundo.no incluye refacciones </span><br/>");
                    //}
                    //else
                    //{
                    //    sb.Append(@"<span>- El costo por visita de servicio fuera de garantia es de $890 pesos por un equipo y $690 pesos a partir del segundo, no incluye refacciones </span><br/>");
                    //    sb.Append(@"<span>- Las refacciones necesarias para la reparación seran cotizadas por separado asi como la mano de obra</span><br/>");
                    //}
                    //sb.Append(@"


                    //                                                                                        <span>- Durante la visita solo se atenderán los equipos listados en esta confirmación de servicio</span><br/>
                    //                                                                                        <span>- Metodo de pago: Efectivo, Transferencia, Tarjeta Bancaria / American Express</span>");
                    sb.Append(@"<div class='col-md-12' style='text-align: left; font-size:12px; letter-spacing:0.6pt; vertical-align:top'> 
						<span>
 - El presupuesto para la atención de este servicio es de " + item[0].visitas[0].cantidad.ToString("C", new CultureInfo("es-MX")) + @" pesos
<br /> - Si Usted no realizó su pago en línea con Miele, le pedimos realizarlo mediante depósito bancario, antes del día " + item[0].visitas[0].fecha_limite + @" con los siguientes datos:
<table style='width:100%; border: 1px solid #000;'>
<tr bgcolor='#D3D3D3'>
<td><h> Banco </h></td>
<td><h> Cliente</h></td>
<td><h> A nombre de </h></td>
<td><h> Sucursal </h></td>
<td><h> Cuenta </h></td>
<td><h> CLABE </h></td>
<td><h> Destinado a </h></td>
<td><h> Referencia </h></td>
</tr>
<tr>
<td><h> Banamex </h></td>
<td><h> 62442172 </h></td>
<td><h> Miele S.A. de C.V. </h></td>
<td><h> 6503 </h></td>
<td><h> 4124307 </h></td>
<td><h> 2180650341243070 </h></td>
<td><h> Servicio Técnico </h></td>
<td><h> " + item[0].id.ToString() + @"</h></td>
</tr>
</table>
- Una vez realizado el depósito, por favor envíe el comprobante de pago al correo info@miele.com.mx de otra manera su servicio no podrá ser atendido.
<br /> - El servicio inicial de revisión y/o diagnóstico, no incluye el costo de las refacciones que pudieran ser necesarias, así como la mano de obra.
<br /> - Durante la visita del técnico, solo se atenderán los servicios listados en esta confirmación. En caso de requerir que un equipo adicional sea atendido, por favor solicítelo llamando a nuestro Contact Center para realizar una nueva programación.
						</span>
                                                </div>");

                    sb.Append(@"</div>
                                      </div>
                                 </div>
                            </body>
                        </html>");

                    //return sb.ToString();

                    if (item.Count() == 0)
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

        #region Dashboard
        [HttpGet("ResumenDashboard")]
        public IActionResult ResumenDashboard(bool? Garantia = null,
            string fechain = "01/01/1900",
            string fechafin = "01/01/3000",
            int? TipoServicio = null,
            int? SubTipoServicio = null,
            int? TecnicoId = null,
            int? TipoTecnico = null,
            int? SubTipoTecnico = null
            )
        {
            List<int> myInClause = new List<int>() { 15, 16, 13 };
            IActionResult response;
            DateTime f1 = Convert.ToDateTime(fechain);
            DateTime f2 = Convert.ToDateTime(fechafin);

            f1.ToString("dd-MM-yyyy");
            f2.ToString("dd-MM-yyyy");

            var requests = (from se in _context.Servicio
                            join vi in _context.Visita on se.id equals vi.id_servicio
                            join te in _context.rel_tecnico_visita on vi.id equals te.id_vista
                            join tec in _context.Tecnicos on te.id_tecnico equals tec.id
                            join rel in _context.Rel_servicio_producto on vi.id equals rel.id_vista
                            where se.creado.Date >= f1.Date && se.creado.Date <= f2.Date &&
                            te.tecnico_responsable == true
                            select new
                            {
                                se.id,
                                se.id_tipo_servicio,
                                se.creado,
                                se.id_sub_tipo_servicio,
                                se.id_estatus_servicio,
                                id_visita = vi.id,
                                no_visitas = se.visita.Count(),
                                te.tecnico.id_tipo_tecnico,
                                te.tecnico.id_cat_tecnicos_sub_Tipo,
                                id_tecnico = te.id_tecnico,
                                rel.garantia,
                                vi.fecha_visita,
                                vi.fecha_inicio_visita,
                                vi.fecha_fin_visita
                            });

            if (Garantia != null)
            {
                requests = (from a in requests
                                //join b in _context.Rel_servicio_producto on a.id_visita equals b.id_vista
                            select new
                            {
                                a.id,
                                a.id_tipo_servicio,
                                a.creado,
                                a.id_sub_tipo_servicio,
                                a.id_estatus_servicio,
                                a.id_visita,
                                a.no_visitas,
                                a.id_tipo_tecnico,
                                a.id_cat_tecnicos_sub_Tipo,
                                a.id_tecnico,
                                a.garantia,
                                a.fecha_visita,
                                a.fecha_inicio_visita,
                                a.fecha_fin_visita
                            }).Where(x => x.garantia == Garantia);
            }
            if (TipoServicio != null)
            {
                requests = requests.Where(x => x.id_tipo_servicio == TipoServicio);
            }

            if (SubTipoServicio != null)
            {
                requests = requests.Where(x => x.id_sub_tipo_servicio == SubTipoServicio);
            }

            if (TipoTecnico != null)
            {
                requests = requests.Where(x => x.id_tipo_tecnico == TipoTecnico);
            }

            if (SubTipoTecnico != null)
            {
                requests = requests.Where(x => x.id_cat_tecnicos_sub_Tipo == SubTipoTecnico);
            }

            if (TecnicoId != null)
            {
                requests = requests.Where(x => x.id_tecnico == TecnicoId);
            }

            //Pendientes
            var request = requests.OrderBy(x => x.creado).ToList();
            var totalPendientes = (from a in request
                                   where !(myInClause).Contains(a.id_estatus_servicio.Value)
                                   select new
                                   {
                                       a.id
                                   }).Count();
            //primera visita
            var total_primera_visita = request.Where(x => x.no_visitas == 1 && x.id_estatus_servicio == 15).Count();
            var dias_visita = request.Where(x => x.no_visitas == 1 && x.id_estatus_servicio == 15).ToList();
            var total_dias = 0;
            decimal porcentajeServiciosResultosPrimeraVisita = 0;
            if (dias_visita.Count() != 0)
            {
                //for (int i = 0; i < dias_visita.Count(); i++)
                //{
                //    if (dias_visita[i].creado != null)
                //    {
                //        total_dias = total_dias + EF.Functions.DateDiffDay(dias_visita[i].fecha_fin_visita, dias_visita[i].fecha_visita).Value;
                //    }
                //}
                var primer_dia = dias_visita.First();
                var ultimo_dias = dias_visita.Last();
                porcentajeServiciosResultosPrimeraVisita = Math.Round((Convert.ToDecimal(EF.Functions.DateDiffDay(primer_dia.creado, ultimo_dias.creado) / Convert.ToDecimal(total_primera_visita))) * 100, 2);
            }

            //Tiempo respuesta
            decimal promedioTiempoRespuesta = 0;
            var total_servicios_respuesta = request.Where(x => x.fecha_inicio_visita != null).Count();
            if (total_servicios_respuesta != 0)
            {
                for (int i = 0; i < request.Count(); i++)
                {
                    if (request[i].creado != null)
                    {
                        if (request[i].fecha_inicio_visita != null)
                        {

                            DateTime econvertedDate = Convert.ToDateTime(request[i].fecha_inicio_visita.Value.Date);
                            DateTime sconvertedDate = Convert.ToDateTime(request[i].creado.Date);

                            TimeSpan age = econvertedDate.Subtract(sconvertedDate);
                            Int32 diff = Convert.ToInt32(age.TotalDays);
                            int var = EF.Functions.DateDiffDay(request[i].creado.Date, request[i].fecha_inicio_visita.Value.Date);
                            total_dias = total_dias + diff;//((TimeSpan)(request[i].fecha_inicio_visita - request[i].creado.Date)).Days; //EF.Functions.DateDiffDay(Convert.ToDateTime(request[i].creado.ToShortDateString()), Convert.ToDateTime(request[i].fecha_inicio_visita.Value.ToShortDateString()));
                        }
                        else
                        {
                            total_dias = total_dias + 0;
                        }
                    }
                    else
                    {
                        total_dias = total_dias + 0;
                    }
                }

                //var primer_dia_respuesta = request.First();
                //var ultimo_dias_respuesta = request.Last();
                promedioTiempoRespuesta = Math.Round((Convert.ToDecimal(Math.Abs(total_dias)) / Convert.ToDecimal(Math.Abs(total_servicios_respuesta))), 2);
            }


            //Tiempo solucion
            var dias_visita_solucion = request.Where(x => x.id_estatus_servicio == 15).ToList();
            decimal promedioTiempoSolucion = 0;
            var total_dias_solucion = 0;
            if (dias_visita_solucion.Count() != 0)
            {
                for (int i = 0; i < dias_visita_solucion.Count(); i++)
                {
                    if (dias_visita_solucion[i].creado != null)
                    {
                        if (dias_visita_solucion[i].fecha_fin_visita != null)
                            total_dias_solucion = total_dias_solucion + EF.Functions.DateDiffDay(dias_visita_solucion[i].fecha_fin_visita, dias_visita_solucion[i].creado).Value;
                    }
                }

                //if (total_dias_solucion < 0)
                //{
                //    total_dias_solucion = (total_dias_solucion * -1);
                //}
                //var primer_dia_solucion = dias_visita_solucion.First();
                //var ultimo_dias_solucion = dias_visita_solucion.Last();
                promedioTiempoSolucion = Math.Round((Convert.ToDecimal(Math.Abs(total_dias_solucion)) / Convert.ToDecimal(Math.Abs(dias_visita_solucion.Count()))), 2);
            }

            //Grafica casos pendientes
            DateTime tres = DateTime.Today.AddDays(-3);
            tres.ToString("dd-MM-yyyy");
            DateTime seis = DateTime.Today.AddDays(-6);
            seis.ToString("dd-MM-yyyy");
            DateTime cinco = DateTime.Today.AddDays(-5);
            cinco.ToString("dd-MM-yyyy");
            DateTime diez = DateTime.Today.AddDays(-10);
            diez.ToString("dd-MM-yyyy");
            DateTime quince = DateTime.Today.AddDays(-15);
            quince.ToString("dd-MM-yyyy");
            DateTime veinte = DateTime.Today.AddDays(-20);
            veinte.ToString("dd-MM-yyyy");
            DateTime venticino = DateTime.Today.AddDays(-25);
            venticino.ToString("dd-MM-yyyy");

            var uno = (from r in request
                       where !(myInClause).Contains(r.id_estatus_servicio.Value) && r.creado.Date >= cinco.Date && r.creado.Date <= DateTime.Now
                       select new
                       {
                           r.id
                       }).Count();
            var dos = (from r in request
                       where !(myInClause).Contains(r.id_estatus_servicio.Value) && r.creado.Date >= diez.Date && r.creado.Date < cinco.Date
                       select new
                       {
                           r.id
                       }).Count();
            var tress = (from r in request
                         where !(myInClause).Contains(r.id_estatus_servicio.Value) && r.creado.Date >= quince.Date && r.creado.Date < diez.Date
                         select new
                         {
                             r.id
                         }).Count();
            var cuatro = (from r in request
                          where !(myInClause).Contains(r.id_estatus_servicio.Value) && r.creado.Date >= veinte.Date && r.creado.Date < quince.Date
                          select new
                          {
                              r.id
                          }).Count();
            var cincos = (from r in request
                          where !(myInClause).Contains(r.id_estatus_servicio.Value) && r.creado.Date >= venticino.Date && r.creado.Date < veinte.Date
                          select new
                          {
                              r.id
                          }).Count();
            var seiss = (from r in request
                         where !(myInClause).Contains(r.id_estatus_servicio.Value) && r.creado.Date < venticino.Date
                         select new
                         {
                             r.id
                         }).Count();

            var totalPendientesgrafica = (uno, dos, tress, cuatro, cincos, seiss);

            //Grafica reparacion primera vista;
            var total_segunda_visita = request.Where(x => x.no_visitas > 1 && x.id_estatus_servicio == 15).Count();
            var totalPrimeravisitagrafica = (total_primera_visita, total_segunda_visita);

            //Grafica tiempo de respuesta
            var uno_respuesta = (from r in request
                                 where r.id_estatus_servicio == 15 && r.creado.Date >= tres.Date && r.creado.Date <= DateTime.Now
                                 select new
                                 {
                                     r.id
                                 }).Count();
            var dos_respuesta = (from r in request
                                 where r.id_estatus_servicio == 15 && r.creado.Date >= seis.Date && r.creado.Date < tres.Date
                                 select new
                                 {
                                     r.id
                                 }).Count();
            var tres_respuesta = (from r in request
                                  where r.id_estatus_servicio == 15 && r.creado.Date < seis.Date
                                  select new
                                  {
                                      r.id
                                  }).Count();

            var totalTiempoespuestagrafica = (uno_respuesta, dos_respuesta, tres_respuesta);

            //tipoSoluciongrafica
            var uno_solucion = (from r in request
                                where r.id_estatus_servicio == 15 && r.creado.Date >= cinco.Date && r.creado.Date <= DateTime.Now
                                select new
                                {
                                    r.id
                                }).Count();
            var dos_solucion = (from r in request
                                where r.id_estatus_servicio == 15 && r.creado.Date >= diez.Date && r.creado.Date < cinco.Date
                                select new
                                {
                                    r.id
                                }).Count();
            var tres_solucion = (from r in request
                                 where r.id_estatus_servicio == 15 && r.creado.Date >= quince.Date && r.creado.Date < diez.Date
                                 select new
                                 {
                                     r.id
                                 }).Count();
            var cuatro_solucion = (from r in request
                                   where r.id_estatus_servicio == 15 && r.creado.Date >= veinte.Date && r.creado.Date < quince.Date
                                   select new
                                   {
                                       r.id
                                   }).Count();
            var cinco_solucion = (from r in request
                                  where r.id_estatus_servicio == 15 && r.creado.Date >= venticino.Date && r.creado.Date < veinte.Date
                                  select new
                                  {
                                      r.id
                                  }).Count();
            var seis_solucion = (from r in request
                                 where r.id_estatus_servicio == 15 && r.creado.Date < venticino.Date
                                 select new
                                 {
                                     r.id
                                 }).Count();

            var totalTiemposoluciongrafica = (uno_solucion, dos_solucion, tres_solucion, cuatro_solucion, cinco_solucion, seis_solucion);

            response = Ok(new { totalPendientes, porcentajeServiciosResultosPrimeraVisita, promedioTiempoRespuesta, promedioTiempoSolucion, totalPendientesgrafica, totalPrimeravisitagrafica, totalTiempoespuestagrafica, totalTiemposoluciongrafica });

            return new ObjectResult(response);
        }

        [HttpGet("ReporteDashboard")]
        public IActionResult ReporteDashboard(bool? Garantia = null,
            string fechain = "01/01/1900",
            string fechafin = "01/01/3000",
            int? TipoServicio = null,
            int? SubTipoServicio = null,
            int? TecnicoId = null,
            int? TipoTecnico = null,
            int? SubTipoTecnico = null,
            int? calculo = null
            )
        {
            List<int> myInClause = new List<int>() { 15, 16, 13 };
            List<int> Clause_pendiente = new List<int>() { 4 };
            IActionResult response;
            DateTime f1 = Convert.ToDateTime(fechain);
            DateTime f2 = Convert.ToDateTime(fechafin);

            f1.ToString("dd-MM-yyyy");
            f2.ToString("dd-MM-yyyy");

            decimal result = 0;
            var _result = "";
            var promotores = 0;
            var detractores = 0;
            var requests = (from se in _context.Servicio
                            join te in _context.rel_tecnico_visita on se.visita[0].id equals te.id_vista
                            join es in _context.Cat_estatus_servicio on se.id_estatus_servicio equals es.id
                            where se.creado.Date >= f1.Date && se.creado.Date <= f2.Date &&
                            te.tecnico_responsable == true
                            select new
                            {
                                se.id,
                                se.id_tipo_servicio,
                                se.creado,
                                fecha_creacion = se.creado.ToString("dd/MM/yyyy"),
                                folio = se.id,
                                tipo_servicio = se.tipo_servicio.desc_tipo_servicio,
                                subtiposervicio = se.sub_tipo_servicio.sub_desc_tipo_servicio,
                                estatus_servicio = es.desc_estatus_servicio,
                                se.IBS,
                                se.id_sub_tipo_servicio,
                                se.id_estatus_servicio,
                                id_visita = se.visita[0].id,
                                no_visitas = se.visita.Count(),
                                se.visita[0].fecha_visita,
                                se.visita[0].fecha_fin_visita,
                                te.tecnico.id_tipo_tecnico,
                                te.tecnico.id_cat_tecnicos_sub_Tipo,
                                te.id_tecnico,
                                hora_inicio = se.visita[0].hora,
                                se.visita[0].hora_fin,
                                se.visita[0].fecha_inicio_visita,
                                se.visita[0].garantia
                            }).ToList();

            if (Garantia != null)
            {
                requests = (from a in requests
                            join b in _context.Rel_servicio_producto on a.id_visita equals b.id_vista
                            select new
                            {
                                a.id,
                                a.id_tipo_servicio,
                                a.creado,
                                fecha_creacion = a.creado.ToString("dd/MM/yyyy"),
                                folio = a.id,
                                a.tipo_servicio,
                                a.subtiposervicio,
                                a.estatus_servicio,
                                a.IBS,
                                a.id_sub_tipo_servicio,
                                a.id_estatus_servicio,
                                id_visita = a.id,
                                no_visitas = a.no_visitas,
                                a.fecha_visita,
                                a.fecha_fin_visita,
                                a.id_tipo_tecnico,
                                a.id_cat_tecnicos_sub_Tipo,
                                a.id_tecnico,
                                a.hora_inicio,
                                a.hora_fin,
                                a.fecha_inicio_visita,
                                a.garantia
                            }).Where(x => x.garantia == Garantia).ToList();
            }
            if (TipoServicio != null)
            {
                requests = requests.Where(x => x.id_tipo_servicio == TipoServicio).ToList();
            }

            if (SubTipoServicio != null)
            {
                requests = requests.Where(x => x.id_sub_tipo_servicio == SubTipoServicio).ToList();
            }

            if (TipoTecnico != null)
            {
                requests = requests.Where(x => x.id_tipo_tecnico == TipoTecnico).ToList();
            }

            if (SubTipoTecnico != null)
            {
                requests = requests.Where(x => x.id_cat_tecnicos_sub_Tipo == SubTipoTecnico).ToList();
            }

            if (TecnicoId != null)
            {
                requests = requests.Where(x => x.id_tecnico == TecnicoId).ToList();
            }

            int total_dias = 0;
            DateTime tres_dias = DateTime.Today.AddDays(-3);
            tres_dias.ToString("dd-MM-yyyy HH:mm:ss");
            DateTime seis_dias = DateTime.Today.AddDays(-6);
            seis_dias.ToString("dd-MM-yyyy HH:mm:ss");
            DateTime cinco_dias = DateTime.Today.AddDays(-5);
            cinco_dias.ToString("dd-MM-yyyy");
            DateTime diez_dias = DateTime.Today.AddDays(-10);
            diez_dias.ToString("dd-MM-yyyy");
            DateTime quince_dias = DateTime.Today.AddDays(-15);
            quince_dias.ToString("dd-MM-yyyy");
            DateTime veinte_dias = DateTime.Today.AddDays(-20);
            veinte_dias.ToString("dd-MM-yyyy");
            DateTime venticino_dias = DateTime.Today.AddDays(-25);
            venticino_dias.ToString("dd-MM-yyyy");
            DateTime dia_actual = DateTime.Now;
            dia_actual.ToString("dd-MM-yyyy HH:mm:ss");

            switch (calculo)
            {
                case 1: //Tiempo de respuesta
                    for (int i = 0; i < requests.Count(); i++)
                    {
                        if (requests[i].fecha_visita != null)
                        {
                            if (requests[i].fecha_fin_visita != null)
                            {
                                total_dias = total_dias + EF.Functions.DateDiffDay(requests[i].fecha_fin_visita, requests[i].fecha_visita).Value;
                            }
                        }
                    }
                    if (total_dias != 0)
                    {
                        result = Math.Round((Convert.ToDecimal(total_dias) / Convert.ToDecimal(requests.Count)), 2);
                    }
                    else
                    {
                        result = 0;
                    }
                    break;
                case 2: // <= 3
                    var total_3 = (from a in requests
                                   where !(myInClause).Contains(a.id_estatus_servicio.Value) && a.creado.Date > tres_dias.Date && a.creado.Date <= dia_actual
                                   select new
                                   {
                                       a.id
                                   }).Count();
                    var total_servicios = (from a in requests
                                           where !(myInClause).Contains(a.id_estatus_servicio.Value)
                                           select new
                                           {
                                               a.id
                                           }).Count();

                    if (total_3 != 0)
                    {
                        if (total_servicios != 0)
                        {
                            result = (Convert.ToDecimal(total_3) / Convert.ToDecimal(total_servicios)) * 100;
                        }
                    }
                    else
                    {
                        result = 0;
                    }

                    break;
                case 3: // > 6
                    var total_6 = (from a in requests
                                   where !(myInClause).Contains(a.id_estatus_servicio.Value) && a.creado.Date < seis_dias.Date
                                   select new
                                   {
                                       a.id
                                   }).Count();
                    var total_servicios_6 = (from a in requests
                                             where !(myInClause).Contains(a.id_estatus_servicio.Value)
                                             select new
                                             {
                                                 a.id
                                             }).Count();
                    if (total_6 != 0)
                    {
                        if (total_servicios_6 != 0)
                        {
                            result = (Convert.ToDecimal(total_6) / Convert.ToDecimal(total_servicios_6)) * 100;
                        }
                    }
                    else
                    {
                        result = 0;
                    }

                    break;
                case 4: //Tiempo de solucion 
                    for (int i = 0; i < requests.Where(x => x.id_estatus_servicio == 15).Count(); i++)
                    {
                        if (requests[i].fecha_visita != null)
                        {
                            if (requests[i].fecha_fin_visita != null)
                            {
                                total_dias = total_dias + EF.Functions.DateDiffDay(requests[i].fecha_fin_visita, requests[i].fecha_visita).Value;
                            }
                        }
                    }
                    if (total_dias != 0)
                    {
                        result = Math.Round((Convert.ToDecimal(total_dias) / Convert.ToDecimal(requests.Count)), 2) * 100;
                    }
                    else
                    {
                        result = 0;
                    }
                    break;
                case 5: //First time fix rate [%] 
                    var fix = (from a in requests
                               where a.fecha_visita == a.fecha_fin_visita
                               select new
                               {
                                   a.id
                               }).Count();
                    result = Math.Round((Convert.ToDecimal(fix) / Convert.ToDecimal(requests.Count)), 2) * 100;
                    break;
                case 6: //Satisfacción de cliente (Servicio Tecnico) [puntos]

                    var total_encuestas = (from a in requests
                                           join b in _context.encuesta_general on a.id equals b.id_servicio
                                           //where !(myInClause).Contains(a.id_estatus_servicio.Value)
                                           select new
                                           {
                                               total = Convert.ToInt32(b.pregunta_2) + Convert.ToInt32(b.pregunta_3) + Convert.ToInt32(b.pregunta_4) + Convert.ToInt32(b.pregunta_5) + Convert.ToInt32(b.pregunta_6) + Convert.ToInt32(b.pregunta_7) + Convert.ToInt32(b.pregunta_8) + Convert.ToInt32(b.pregunta_9)
                                           }).ToList();
                    for (int i = 0; i < total_encuestas.Count(); i++)
                    {
                        total_dias = total_dias + total_encuestas[i].total;
                    }
                    if (total_dias != 0)
                    {
                        result = Math.Round((Convert.ToDecimal(total_dias) / Convert.ToDecimal(total_encuestas.Count())) / 8, 2);
                    }
                    else
                    {
                        result = 0;
                    }
                    break;
                case 7: //Net promoter score (NPS) Servicio Tecnico [puntos]

                    var nps = (from a in requests
                               join b in _context.encuesta_general on a.id equals b.id_servicio
                               //where !(myInClause).Contains(a.id_estatus_servicio.Value)
                               select new
                               {
                                   b.pregunta_1
                               }).ToList();
                    for (int i = 0; i < nps.Count(); i++)
                    {
                        if (Convert.ToInt32(nps[i].pregunta_1) > 8)
                        {
                            promotores = promotores + 1;
                        };
                        if (Convert.ToInt32(nps[i].pregunta_1) < 7)
                        {
                            detractores = detractores + 1;
                        };
                    }

                    result = (promotores - detractores) * 100;

                    break;
                case 8: //Estadisticas

                    var instalacion = (from a in requests
                                       where a.id_tipo_servicio == 1
                                       select new
                                       {
                                           a.id
                                       }).Count();
                    var mantenimiento = (from a in requests
                                         where a.id_tipo_servicio == 2
                                         select new
                                         {
                                             a.id
                                         }).Count();
                    var reparacion = (from a in requests
                                      where a.id_tipo_servicio == 3
                                      select new
                                      {
                                          a.id
                                      }).Count();
                    var diagnostico = (from a in requests
                                       where a.id_tipo_servicio == 5
                                       select new
                                       {
                                           a.id
                                       }).Count();
                    var Troubleshooting = (from a in requests
                                           where a.id_tipo_servicio == 6
                                           select new
                                           {
                                               a.id
                                           }).Count();
                    var home = (from a in requests
                                where a.id_tipo_servicio == 7
                                select new
                                {
                                    a.id
                                }).Count();

                    _result = ("Instalación:" + instalacion.ToString() + ", Mantenimiento:" + mantenimiento.ToString() + ", Reparación:" + reparacion.ToString() + ", Diagnóstico en taller:" + diagnostico.ToString() + ", Troubleshooting:" + Troubleshooting.ToString() + ", Home program:" + home.ToString());
                    break;
                case 9: //Aging

                    var uno = (from r in requests
                               where r.id_estatus_servicio == 4 && r.creado.Date >= cinco_dias.Date && r.creado.Date <= DateTime.Now
                               select new
                               {
                                   r.id
                               }).Count();
                    var dos = (from r in requests
                               where r.id_estatus_servicio == 4 && r.creado.Date >= diez_dias.Date && r.creado.Date < cinco_dias.Date
                               select new
                               {
                                   r.id
                               }).Count();
                    var tres = (from r in requests
                                where r.id_estatus_servicio == 4 && r.creado.Date >= quince_dias.Date && r.creado.Date < diez_dias.Date
                                select new
                                {
                                    r.id
                                }).Count();
                    var cuatro = (from r in requests
                                  where r.id_estatus_servicio == 4 && r.creado.Date >= veinte_dias.Date && r.creado.Date < quince_dias.Date
                                  select new
                                  {
                                      r.id
                                  }).Count();
                    var cinco = (from r in requests
                                 where r.id_estatus_servicio == 4 && r.creado.Date >= venticino_dias.Date && r.creado.Date < veinte_dias.Date
                                 select new
                                 {
                                     r.id
                                 }).Count();
                    var seis = (from r in requests
                                where r.id_estatus_servicio == 4 && r.creado.Date < venticino_dias.Date
                                select new
                                {
                                    r.id
                                }).Count();


                    _result = ("1-5:" + uno.ToString() + ", 6-10:" + dos.ToString() + ", 11-15:" + tres.ToString() + ", 16-20:" + cuatro.ToString() + ", 21-25:" + cinco.ToString() + ", +26:" + seis.ToString());

                    break;
                case 10: //Incumplimientos
                    for (int i = 0; i < requests.Count; i++)
                    {
                        string time = Convert.ToDateTime(requests[i].fecha_inicio_visita).ToString("hh:mm");
                        var segments = time.Split(':');
                        TimeSpan hora_inicio = TimeSpan.FromHours(Convert.ToDouble(requests[i].hora_inicio));
                        TimeSpan hora_fin = TimeSpan.FromHours(Convert.ToDouble(segments[0] + "." + segments[1]));
                        //TimeSpan fromTimeActual = DateTime.Now.TimeOfDay;
                        TimeSpan resta = hora_fin.Subtract(hora_inicio);

                        if (resta.TotalMinutes > 30.0)
                        {
                            total_dias = total_dias + 1;
                        }
                    }
                    result = total_dias;
                    break;
                case 11: //Servicios pendientes
                    var instalacion_i = (from a in requests
                                         where a.id_estatus_servicio.Value == 4 && a.id_tipo_servicio == 1
                                         select new
                                         {
                                             a.id
                                         }).Count();
                    var mantenimiento_i = (from a in requests
                                           where a.id_estatus_servicio.Value == 4 && a.id_tipo_servicio == 2
                                           select new
                                           {
                                               a.id
                                           }).Count();
                    var reparacion_i = (from a in requests
                                        where a.id_estatus_servicio.Value == 4 && a.id_tipo_servicio == 3
                                        select new
                                        {
                                            a.id
                                        }).Count();
                    var Troubleshooting_i = (from a in requests
                                             where a.id_estatus_servicio.Value == 4 && a.id_tipo_servicio == 5
                                             select new
                                             {
                                                 a.id
                                             }).Count();
                    var diagnostico_i = (from a in requests
                                         where a.id_estatus_servicio.Value == 4 && a.id_tipo_servicio == 6
                                         select new
                                         {
                                             a.id
                                         }).Count();
                    var home_i = (from a in requests
                                  where a.id_estatus_servicio.Value == 4 && a.id_tipo_servicio == 7
                                  select new
                                  {
                                      a.id
                                  }).Count();

                    _result = ("Instalación:" + instalacion_i.ToString() + ", Mantenimiento:" + mantenimiento_i.ToString() + ", Reparación:" + reparacion_i.ToString() + ", Diagnóstico en taller:" + diagnostico_i.ToString() + ", Troubleshooting:" + Troubleshooting_i.ToString() + ", Home program:" + home_i.ToString());
                    break;
                case 12: //Satisfacción de cliente (Contact Center) [puntos]
                    var total_cc = (from a in requests
                                    join b in _context.encuesta_general on a.id equals b.id_servicio
                                    //where !(myInClause).Contains(a.id_estatus_servicio.Value)
                                    select new
                                    {
                                        total = Convert.ToInt32(b.pregunta_14)
                                    }).ToList();
                    for (int i = 0; i < total_cc.Count(); i++)
                    {
                        total_dias = total_dias + total_cc[i].total;
                    }
                    if (total_dias != 0)
                    {
                        if (total_cc.Count() != 0)
                        {
                            result = Math.Round((Convert.ToDecimal(total_dias) / Convert.ToDecimal(total_cc.Count())), 2);
                        }
                    }
                    else
                    {
                        result = 0;
                    }
                    break;
                case 13: //Net promoter score (NPS) Contact Center [puntos]

                    var nps_contact_center = (from a in requests
                                              join b in _context.encuesta_general on a.id equals b.id_servicio
                                              //where !(myInClause).Contains(a.id_estatus_servicio.Value)
                                              select new
                                              {
                                                  b.pregunta_14
                                              }).ToList();
                    for (int i = 0; i < nps_contact_center.Count(); i++)
                    {
                        if (Convert.ToInt32(nps_contact_center[i].pregunta_14) > 8)
                        {
                            promotores = promotores + 1;
                        };
                        if (Convert.ToInt32(nps_contact_center[i].pregunta_14) < 7)
                        {
                            detractores = detractores + 1;
                        };
                    }

                    result = (promotores - detractores) * 100;

                    break;
                case 14: //Encuestas realizadas

                    var cantidad_encuestas = (from a in requests
                                              join b in _context.encuesta_general on a.id equals b.id_servicio
                                              //where !(myInClause).Contains(a.id_estatus_servicio.Value)
                                              select new
                                              {
                                                  a.id
                                              }).Count();

                    if (cantidad_encuestas != 0)
                    {
                        if (requests.Count() != 0)
                        {
                            result = Math.Round((Convert.ToDecimal(cantidad_encuestas) / Convert.ToDecimal(requests.Count())), 2) * 100;
                        }
                    }
                    else
                    {
                        result = 0;
                    }

                    break;
                case 15://Nivel de Servicio Quejas [%]
                    var total_q = (from a in requests
                                   join b in _context.quejas_servicios on a.id equals b.id_servicio
                                   join c in _context.Quejas on b.id_queja equals c.Id
                                   join d in _context.Propuestas on c.Id equals d.QuejaId
                                   where c.Fecha.Date > tres_dias.Date && c.Fecha.Date <= dia_actual && d.Fecha != null
                                   select new
                                   {
                                       a.id
                                   }).Distinct().Count();
                    var total_servicios_q = (from a in _context.Quejas
                                             select new
                                             {
                                                 a.Id
                                             }).Count();
                    if (total_q != 0)
                    {
                        if (total_servicios_q != 0)
                        {
                            result = Math.Round((Convert.ToDecimal(total_q) / Convert.ToDecimal(total_servicios_q)) * 100, 2);
                        }
                    }
                    else
                    {
                        result = 0;
                    }

                    break;
                case 16: //Tiempo de reacción promedio Quejas [dias]
                    var total_quejas = (from a in requests
                                        join b in _context.quejas_servicios on a.id equals b.id_servicio
                                        join c in _context.Quejas on b.id_queja equals c.Id
                                        join d in _context.Propuestas on c.Id equals d.QuejaId
                                        select new
                                        {
                                            a.id,
                                            fecha_queja = c.Fecha,
                                            fecha_solucion = d.Fecha
                                        }).Distinct().ToList();
                    for (int i = 0; i < total_quejas.Where(x => x.fecha_solucion != null).Count(); i++)
                    {
                        if (total_quejas[i].fecha_queja != null)
                        {
                            total_dias = total_dias + EF.Functions.DateDiffDay(total_quejas[i].fecha_queja, total_quejas[i].fecha_solucion);
                        }
                    }
                    if (total_dias != 0)
                    {
                        if (total_quejas.Where(x => x.fecha_solucion != null).Count() != 0)
                        {
                            result = Math.Round((Convert.ToDecimal(total_dias) / Convert.ToDecimal(total_quejas.Where(x => x.fecha_solucion != null).Count())), 2) * 100;
                        }
                        else
                        {
                            result = 0;
                        }
                    }
                    else
                    {
                        result = 0;
                    }
                    break;
                case 17: //Tiempo de solución Quejas [dias]
                    var total_quejas_cierre = (from a in requests
                                               join b in _context.quejas_servicios on a.id equals b.id_servicio
                                               join c in _context.Quejas on b.id_queja equals c.Id
                                               join d in _context.Propuestas on c.Id equals d.QuejaId
                                               select new
                                               {
                                                   a.id,
                                                   fecha_queja = c.Fecha,
                                                   fecha_cierre = d.FechaCierre
                                               }).Distinct().ToList();
                    for (int i = 0; i < total_quejas_cierre.Count(); i++)
                    {
                        if (total_quejas_cierre[i].fecha_queja != null)
                        {
                            if (total_quejas_cierre[i].fecha_cierre != null)
                            {
                                total_dias = total_dias + EF.Functions.DateDiffDay(total_quejas_cierre[i].fecha_queja, total_quejas_cierre[i].fecha_cierre).Value;
                            }
                        }
                    }
                    if (total_dias != 0)
                    {
                        result = Math.Round((Convert.ToDecimal(total_dias) / Convert.ToDecimal(total_quejas_cierre.Count())), 2) * 100;
                    }
                    else
                    {
                        result = 0;
                    }
                    break;
                case 18: //Satisfacción de cliente (Quejas) [puntos]
                    var total_satisfaccion_quejas = (from a in _context.Quejas
                                                     join b in _context.encuesta_queja on a.Id equals b.id_queja
                                                     //where !(myInClause).Contains(a.id_estatus_servicio.Value)
                                                     select new
                                                     {
                                                         total = Convert.ToInt32(b.pregunta_1) + Convert.ToInt32(b.pregunta_2) + Convert.ToInt32(b.pregunta_3) + Convert.ToInt32(b.pregunta_4) + Convert.ToInt32(b.pregunta_5) + Convert.ToInt32(b.pregunta_6) + Convert.ToInt32(b.pregunta_7) + Convert.ToInt32(b.pregunta_8) + Convert.ToInt32(b.pregunta_9)
                                                     }).ToList();
                    for (int i = 0; i < total_satisfaccion_quejas.Count(); i++)
                    {
                        total_dias = total_dias + total_satisfaccion_quejas[i].total;
                    }
                    if (total_dias != 0)
                    {
                        result = Math.Round((Convert.ToDecimal(total_dias) / Convert.ToDecimal(total_satisfaccion_quejas.Count())) / 9, 2);
                    }
                    else
                    {
                        result = 0;
                    }
                    break;
                case 19: //Net promoter score (NPS) Quejas [puntos]

                    var nps_queja = (from a in _context.Quejas
                                     join b in _context.encuesta_queja on a.Id equals b.id_queja
                                     //where !(myInClause).Contains(a.id_estatus_servicio.Value)
                                     select new
                                     {
                                         b.pregunta_10
                                     }).ToList();
                    for (int i = 0; i < nps_queja.Count(); i++)
                    {
                        if (Convert.ToInt32(nps_queja[i].pregunta_10) > 8)
                        {
                            promotores = promotores + 1;
                        };
                        if (Convert.ToInt32(nps_queja[i].pregunta_10) < 7)
                        {
                            detractores = detractores + 1;
                        };
                    }

                    result = (promotores - detractores) * 100;

                    break;
                case 20: //Satisfacción de cliente (Instalación) [puntos]
                    var total_satisfaccion_cliente = (from a in requests
                                                      join b in _context.encuesta_general on a.id equals b.id_servicio
                                                      where a.id_tipo_servicio == 1
                                                      select new
                                                      {
                                                          total = Convert.ToInt32(b.pregunta_2) + Convert.ToInt32(b.pregunta_3) + Convert.ToInt32(b.pregunta_4) + Convert.ToInt32(b.pregunta_5) + Convert.ToInt32(b.pregunta_6) + Convert.ToInt32(b.pregunta_7) + Convert.ToInt32(b.pregunta_8) + Convert.ToInt32(b.pregunta_9)
                                                      }).ToList();
                    for (int i = 0; i < total_satisfaccion_cliente.Count(); i++)
                    {
                        total_dias = total_dias + total_satisfaccion_cliente[i].total;
                    }
                    if (total_dias != 0)
                    {
                        result = Math.Round((Convert.ToDecimal(total_dias) / Convert.ToDecimal(total_satisfaccion_cliente.Count())) / 8, 2);
                    }
                    else
                    {
                        result = 0;
                    }
                    break;
                case 21: //Net promoter score (NPS) Instalacion [puntos]

                    var nps_instalacion = (from a in requests
                                           join b in _context.encuesta_general on a.id equals b.id_servicio
                                           where a.id_tipo_servicio == 1
                                           select new
                                           {
                                               b.pregunta_1
                                           }).ToList();
                    for (int i = 0; i < nps_instalacion.Count(); i++)
                    {
                        if (Convert.ToInt32(nps_instalacion[i].pregunta_1) > 8)
                        {
                            promotores = promotores + 1;
                        };
                        if (Convert.ToInt32(nps_instalacion[i].pregunta_1) < 7)
                        {
                            detractores = detractores + 1;
                        };
                    }

                    result = (promotores - detractores) * 100;

                    break;
                case 22: //Ordenes de servicio Instalados en primera visita (Fecha Primera Agenda = Fecha Completado) / Ordenes de Servicio Completadas de Instalacion
                    var total_ins_primera = (from a in requests
                                             where a.fecha_visita.Date == (a.fecha_fin_visita == null ? Convert.ToDateTime("1900-01-01").Date : a.fecha_fin_visita.Value.Date) && a.id_tipo_servicio == 1
                                             select new
                                             {
                                                 a.id
                                             }).Count();

                    if (total_ins_primera != 0)
                    {
                        if (requests.Where(x => x.id_tipo_servicio == 1).Count() != 0)
                        {
                            result = Math.Round((Convert.ToDecimal(total_ins_primera) / Convert.ToDecimal(requests.Where(x => x.id_tipo_servicio == 1).Count())), 2);
                        }
                    }
                    else
                    {
                        result = 0;
                    }

                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }

            if (calculo == 8 || calculo == 9 || calculo == 11)
            {
                response = Ok(new { requests, result = _result });
            }
            else
            {
                response = Ok(new { requests, result });
            }


            return new ObjectResult(response);
        }
        #endregion

        #region Encuestas
        [HttpGet("Encuestas_Cliente")]
        public IActionResult Encuestas_Cliente(int id_cliente)
        {
            var cliente = _context.Clientes.SingleOrDefault(x => x.id == id_cliente);

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

            var item = ((from a in _context.Clientes
                         join b in _context.Servicio on a.id equals b.id_cliente
                         join c in _context.Cat_estatus_servicio on b.id_estatus_servicio equals c.id
                         join d in _context.encuesta_general on b.id equals d.id_servicio into _encuesta_general
                         from encuesta_general in _encuesta_general.DefaultIfEmpty()
                         where b.id_estatus_servicio == 15 && a.id == id_cliente //&& !(from o in _context.encuesta_general
                                                                                 //select o.id_servicio).Contains(b.id)
                         select new
                         {
                             id = Convert.ToInt32(b.id),
                             folio = a.id.ToString(),
                             fecha = b.fecha_servicio,
                             estatus = c.desc_estatus_servicio,
                             tipo = "General",
                             id_encuesta = encuesta_general.id == null ? 0 : encuesta_general.id,
                             estatus_activo = encuesta_general.estatus_activo == null ? false : encuesta_general.estatus_activo,
                             estatus_encuesta = encuesta_general.estatus_encuesta == null ? 0 : encuesta_general.estatus_encuesta,
                             intentos = encuesta_general.intentos == null ? 0 : encuesta_general.intentos
                         }).Union
                        (from e in _context.Clientes
                         join f in _context.Quejas on e.id equals f.ClienteId
                         join g in _context.Propuestas on f.Id equals g.QuejaId
                         join d in _context.encuesta_queja on f.Id equals d.id_queja into _encuesta_queja
                         from encuesta_queja in _encuesta_queja.DefaultIfEmpty()
                         where f.ClienteId == id_cliente && g.FechaCierre != null //&& !(from o in _context.encuesta_queja
                                                                                  //select o.id_queja).Contains(f.Id)
                         select new
                         {
                             id = f.Id,
                             folio = f.Folio,
                             fecha = f.Fecha,
                             estatus = "Cerrada",
                             tipo = "Queja",
                             id_encuesta = encuesta_queja.id == null ? 0 : encuesta_queja.id,
                             estatus_activo = encuesta_queja.estatus_activo == null ? false : encuesta_queja.estatus_activo,
                             estatus_encuesta = encuesta_queja.estatus_encuesta == null ? 0 : encuesta_queja.estatus_encuesta,
                             intentos = encuesta_queja.intentos == null ? 0 : encuesta_queja.intentos
                         }
                )).Where(a => a.id_encuesta == 0).ToList();

            if (item.Count() != 0)
            {
                if (item[0].id_encuesta != 0)
                {
                    item = item.Where(x => x.estatus_activo == true && x.estatus_encuesta == 2 && x.intentos <= 3).ToList();
                }
            }

            return new ObjectResult(Ok(new { cliente = cliente.nombre + " " + cliente.paterno + " " + cliente.materno, item }));
        }

        // POST: api/Servicios/Nueva_encuesta_general
        [Route("Nueva_encuesta_general")]
        [HttpPost("{Nueva_encuesta_general}")]
        public IActionResult Nueva_encuesta_general([FromBody]encuesta_general item)
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

                    var encuesta = _context.encuesta_general.FirstOrDefault(x => x.id_servicio == item.id_servicio && x.id_cliente == item.id_cliente && x.estatus_activo == true);

                    if (encuesta != null)
                    {
                        if (item.estatus_encuesta == 2)
                        {
                            if (encuesta.intentos < 4)
                            {

                                item.estatus_activo = true;
                                item.intentos = item.intentos + 1;
                            }
                            if (encuesta.intentos >= 2)
                            {

                                item.estatus_activo = false;
                                item.intentos = 3;
                            }

                            _context.encuesta_general.Update(item);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        var count = _context.encuesta_general.OrderByDescending(x => x.id).Take(1).ToList();
                        int numeroCm = 0;
                        if (count.Count() == 0)
                        {
                            numeroCm = 0 + 1;
                        }
                        else
                        {
                            numeroCm = count[0].id + 1;
                        }
                        item.folio = "VCG" + DateTime.Now.ToString("ddMMyy") + string.Format("{0:000000}", numeroCm);
                        if (item.estatus_encuesta == 2)
                        {
                            item.estatus_activo = true;
                            item.intentos = 1;
                        }
                        else
                        {
                            item.estatus_activo = false;
                            item.intentos = 1;
                        }

                        item.fecha = DateTime.Now;
                        _context.encuesta_general.Add(item);
                        _context.SaveChanges();
                    }

                    response = Ok(new { success = true });
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { success = false });
            }
            return new ObjectResult(response);
        }

        // POST: api/Servicios/Nueva_encuesta_queja
        [Route("Nueva_encuesta_queja")]
        [HttpPost("{Nueva_encuesta_queja}")]
        public IActionResult Nueva_encuesta_queja([FromBody]encuesta_queja item)
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

                    var encuesta = _context.encuesta_queja.FirstOrDefault(x => x.id_queja == item.id_queja && x.id_cliente == item.id_cliente && x.estatus_activo == true);

                    if (encuesta != null)
                    {
                        if (item.estatus_encuesta == 2)
                        {
                            if (encuesta.intentos < 4)
                            {

                                item.estatus_activo = true;
                                item.intentos = item.intentos + 1;
                            }
                            if (encuesta.intentos >= 2)
                            {

                                item.estatus_activo = false;
                                item.intentos = 3;
                            }

                            _context.encuesta_queja.Update(item);
                            _context.SaveChanges();
                        }
                    }
                    else
                    {
                        var count = _context.encuesta_queja.OrderByDescending(x => x.id).Take(1).ToList();
                        int numeroCm = 0;
                        if (count.Count() == 0)
                        {
                            numeroCm = 0 + 1;
                        }
                        else
                        {
                            numeroCm = count[0].id + 1;
                        }
                        item.folio = "VCQ" + DateTime.Now.ToString("ddMMyy") + string.Format("{0:000000}", numeroCm);
                        if (item.estatus_encuesta == 2)
                        {
                            item.estatus_activo = true;
                            item.intentos = 1;
                        }
                        else
                        {
                            item.estatus_activo = false;
                            item.intentos = 1;
                        }

                        item.fecha = DateTime.Now;
                        _context.encuesta_queja.Add(item);
                        _context.SaveChanges();
                    }

                    response = Ok(new { success = true });
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { success = false });
            }
            return new ObjectResult(response);
        }

        [HttpGet("Historial_encuentas")]
        public IActionResult Historial_encuentas(string desc_busqueda = "", string fechain = "01/01/1900",
            string fechafin = "01/01/3000")
        {
            DateTime f1 = Convert.ToDateTime(fechain);
            DateTime f2 = Convert.ToDateTime(fechafin);

            f1.ToString("dd-MM-yyyy");
            f2.ToString("dd-MM-yyyy");

            //var enc_cte = (from a in _context.encuesta_general
            //               join b in _context.Clientes on a.id_cliente equals b.id
            //               join c in _context.Servicio on b.id equals c.id_cliente
            //               select new
            //               {
            //                   id_encuesta = a.id,
            //                   folio = a.folio,
            //                   cliente = b.nombre + " " + b.paterno + " " +(b.materno == null ? "" : b.materno),
            //                   fecha = c.fecha_servicio,
            //                   tipo = "General"
            //               });
            //var queja_cte = (from e in _context.encuesta_queja
            //                 join f in _context.Clientes on e.id_cliente equals f.id
            //                 join g in _context.Servicio on f.id equals g.id_cliente
            //                 select new
            //                 {

            //                     id_encuesta = e.id,
            //                     folio = e.folio,
            //                     cliente = f.nombre + " " + f.paterno + " " + (f.materno == null ? "" : f.materno),
            //                     fecha = g.fecha_servicio,
            //                     tipo = "Queja"
            //                 });

            //var item = enc_cte.Union(queja_cte);

            var item = ((from a in _context.encuesta_general
                         join c in _context.Servicio on a.id_servicio equals c.id
                         join b in _context.Clientes on c.id_cliente equals b.id
                         select new
                         {

                             id_encuesta = a.id,
                             folio = a.folio,
                             cliente = b.nombre + " " + b.paterno + " " + (b.materno == null ? "" : b.materno),
                             fecha = c.fecha_servicio,
                             tipo = "General"
                         })
                         .Union
                        (from e in _context.encuesta_queja
                         join h in _context.quejas_servicios on e.id equals h.id_queja
                         join g in _context.Servicio on h.id_servicio equals g.id
                         join f in _context.Clientes on g.id_cliente equals f.id
                         select new
                         {
                             id_encuesta = e.id,
                             folio = e.folio,
                             cliente = f.nombre + " " + f.paterno + " " + (f.materno == null ? "" : f.materno),
                             fecha = g.fecha_servicio,
                             tipo = "Queja"
                         })
            ).Where(x => x.fecha >= f1 && x.fecha <= f2);

            if (desc_busqueda != "")
            {
                item = item.Where(x => EF.Functions.Like(x.cliente + " " + x.folio, "%" + desc_busqueda + "%"));
            }

            return new ObjectResult(Ok(new { item }));
        }

        [HttpGet("Encuestas_por_id")]
        public IActionResult Encuestas_por_id(string folio)
        {
            IActionResult response;
            var encuesta_general = _context.encuesta_general.Where(x => x.folio == folio).ToList();
            var encuesta_queja = _context.encuesta_queja.Where(x => x.folio == folio).ToList();

            try
            {
                //Se cambia a mayo a cero para que muestre la encuesta general si existe porque es una lista
                if (encuesta_general.Count > 0)
                {
                    response = Ok(new { encuesta = encuesta_general });
                }
                else
                {
                    response = Ok(new { encuesta = encuesta_queja });
                }
            }
            catch (Exception ex)
            {
                response = Ok(new { encuesta = ex.ToString() });
            }

            return new ObjectResult(response);

        }
        #endregion

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match)
                {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    var domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e)
            {
                return false;
            }
            catch (ArgumentException e)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        public class estatus
        {
            public int id_primario { get; set; }
            public int id_secundario { get; set; }
        }

        public class cancela_visita
        {
            public int id_servicio { get; set; }
            public int id_motivo { get; set; }
            public string motivo_cancelacion { get; set; }

        }
        public class ibs
        {
            public int id { get; set; }
            public string numero { get; set; }
        }

        public class tecnicobyid
        {
            public int id { get; set; }
        }

        public class inicio_servicio
        {
            public int id { get; set; }
            public string latitud { get; set; }
            public string longitud { get; set; }
        }

        public class Productos_Servicio
        {
            public int id_cliente { get; set; }
            public int id_tipo { get; set; }
        }

        public class tecnico_agenda
        {
            public int id { get; set; }
            public string productos { get; set; }
        }

        public class tecnico_in
        {
            public string id { get; set; }
        }

        public class checklist
        {
            public string sku { get; set; }
        }

        public class busqueda
        {
            public string desc_busqueda { get; set; }
            public DateTime fecha_creacion_inicio { get; set; }
            public DateTime fecha_creacion_fin { get; set; }
            public DateTime fecha_servicio_inicio { get; set; }
            public DateTime fecha_servicio_fin { get; set; }
            public int tecnico { get; set; }
        }

        public class cerrar_servicio
        {
            public int id_servicio { get; set; }
            public int estatus { get; set; }
            public int id_visita { get; set; }
            public string modelo { get; set; }
            public string url { get; set; }
            public string latitud { get; set; }
            public string longitud { get; set; }
            public int id_categoria { get; set; }
            public string imagen_pago_referenciado { get; set; }
            public string persona_recibe { get; set; }

            public List<Rel_servicio_visita_Refaccion> servicio_refaccion { get; set; }
            public List<Rel_servicio_visita_producto_cat_producto> servicio_productos { get; set; }
        }

        public class cerrar_checklist
        {
            public int id_servicio { get; set; }
            public int estatus { get; set; }
            public int id_visita { get; set; }
            public string no_serie { get; set; }

            public List<Producto_Check_List_Respuestas> producto { get; set; }
        }

        public class Tecnicos_Usuario
        {
            public long id { get; set; }
            public Users users { get; set; }
            public string noalmacen { get; set; }
            public int id_tipo_tecnico { get; set; }
            public string color { get; set; }
            public Cat_Tecnicos_Tipo tecnicos_Tipo { get; set; }
            public DateTime creado { get; set; }
            public long creadopor { get; set; }
            public DateTime actualizado { get; set; }
            public long actualizadopor { get; set; }
            public string name { get; set; }
            public string paterno { get; set; }
            public string materno { get; set; }
            public string telefono { get; set; }
            public string movil { get; set; }

            public List<Tecnicos_Actividad> tecnicos_actividad { get; set; }
            public List<Tecnicos_Cobertura> tecnicos_cobertura { get; set; }
            public List<Tecnicos_Producto> tecnicos_producto { get; set; }
            public List<Visita> visita { get; set; }
        }

        public class rel_certificado_producto_productos
        {
            public int id { get; set; }
            public int id_certificado { get; set; }
            public Cer_producto_cliente certificado { get; set; }
            public int id_producto { get; set; }
            public bool estatus_activo { get; set; }
            public int no_visitas { get; set; }
            public int id_sub_linea { get; set; }
            public DateTime creado { get; set; }
            public DateTime fecha_visita_1 { get; set; }
            public DateTime fecha_visita_2 { get; set; }

            public long Id_Cliente { get; set; }
            public long Id_Producto { get; set; }
            public DateTime FinGarantia { get; set; }
            public DateTime FechaCompra { get; set; }
            public string NoPoliza { get; set; }
            public long Id_EsatusCompra { get; set; }
            public string NoOrdenCompra { get; set; }
            public int id_visita { get; set; }
            public int no_producto { get; set; }
            public DateTime actualizado { get; set; }
            public long actualizadopor { get; set; }
            public long creadopor { get; set; }
            public int id_cotizacion { get; set; }
        }

    }
}
