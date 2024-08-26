using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PDF_Generator.Utility;
using WebApplication.Models;
using WebApplication.Repository;

namespace WebApplication.Controllers
{
    [Produces("application/json")]
    [Route("api/PDFCreator")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class PDFCreatorController : Controller
    {
        private IConverter _converter;
        private IConfiguration _config;
        //private EmailSettings _emailSettings;
        private IEmailRepository _emailRepository;
        private readonly MieleContext _context;

        public PDFCreatorController(IConverter converter, IConfiguration config, IEmailRepository emailRepository, MieleContext context)
        {
            _converter = converter;
            _config = config;
            //_emailSettings = emailSettings;
            _emailRepository = emailRepository;
            _context = context;
        }

        public string[] TemplatePDF(long id, long id_visita)
        {
            var selRuta = _context.parametro_Archivos.FirstOrDefault(p => p.id == 6);
            string path;
            if (selRuta == null) path = "/Imagenes/";
            else
            {
                path = selRuta.ruta;
            }
            var item = (from a in _context.Servicio
                        join c in _context.Clientes on a.id_cliente equals c.id
                        join j in _context.Cat_tipo_servicio on a.id_tipo_servicio equals j.id
                        join k in _context.Cat_estatus_servicio on a.id_estatus_servicio equals k.id into tu
                        from fe in tu.DefaultIfEmpty()
                        where a.id == id
                        select new
                        {
                            a.id,
                            cliente = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(c.nombre) + " " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(c.paterno) + " " + (c.materno == "" ? "" : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(c.materno)),
                            cliente_telefono = c.telefono,
                            cliente_movil = c.telefono_movil,
                            tipo_servicio = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(j.desc_tipo_servicio),
                            estatus_servicio = (fe == null) ? "" : fe.desc_estatus_servicio,
                            contacto = a.contacto,
                            IBS = a.IBS,
                            actividades_realizar = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(a.descripcion_actividades),
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
                                           direccion = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(d.calle_numero + " " + (d.numExt == null ? "" : d.numExt) + " " + (d.numInt == null ? "" : d.numInt)) + ", " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g.desc_localidad) + ", " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(e.desc_estado) + ", " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(f.desc_municipio),
                                           fecha_inicio_visita = Convert.ToDateTime(x.fecha_inicio_visita).ToString("dd/MM/yyyy"),
                                           x.imagen_firma,
                                           persona_recibe = x.persona_recibe == null ? "" : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x.persona_recibe),
                                           tecnico = (from visita_tecnico in _context.rel_tecnico_visita
                                                      join i in _context.Tecnicos on visita_tecnico.id_tecnico equals i.id
                                                      join j in _context.Users on i.id equals j.id
                                                      where visita_tecnico.id_vista == id_visita && visita_tecnico.tecnico_responsable == true
                                                      select new
                                                      {
                                                          nombre = (j.name == null ? "" : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(j.name))  + " " + (j.paterno == null ? "" : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(j.paterno))   + " " + (j.materno == null ? "" : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(j.materno)),
                                                          visita_tecnico.tecnico_responsable
                                                      }).Distinct().ToList(),
                                           checklist = (from w in _context.Producto_Check_List_Respuestas
                                                        join c_p in _context.Cliente_Productos on w.id_producto equals c_p.Id
                                                        join prod in _context.Cat_Productos on c_p.Id_Producto equals prod.id
                                                        join ec in _context.Cat_Checklist_Producto on w.estatus equals ec.id
                                                        join serie in _context.Rel_servicio_producto on c_p.Id equals serie.id_producto
                                                        where w.id_vista == id_visita //&& serie.id_vista == id_visita
                                                        select new
                                                        {
                                                            w.id_vista,
                                                            estatus_prodcuto_checklist = ec.desc_checklist_producto,
                                                            prod.nombre,
                                                            prod.modelo,
                                                            c_p.no_serie,
                                                            serie.garantia,
                                                            imagenes_checklist = (from xz in _context.Rel_Imagen_Producto_Visita
                                                                                  where xz.id_producto == c_p.Id && xz.checklist == true && xz.id_visita == id_visita
                                                                                  select new
                                                                                  {
                                                                                      id = xz.id,
                                                                                      url = xz.path,
                                                                                      xz.actividad
                                                                                  }).Distinct().ToList(),
                                                            preguntas = (from check in _context.Producto_Check_List_Respuestas
                                                                         join c_p in _context.Cliente_Productos on check.id_producto equals c_p.Id
                                                                         join respuesta in _context.Check_List_Respuestas on check.id equals respuesta.id_producto_check_list_respuestas
                                                                         join pregunta in _context.Check_List_Preguntas on respuesta.id_pregunta equals pregunta.id
                                                                         where check.id_vista == id_visita//&& check.id_producto == c_p.Id_Producto //&& visita_c.id_servicio == id && check.id_producto == prod.id && check.id_vista == id_visita
                                                                         select new
                                                                         {
                                                                             pregunta = pregunta.pregunta,
                                                                             respuesta = respuesta.respuesta,
                                                                             comentario = respuesta.comentarios,
                                                                         }).Distinct().ToList()
                                                        }).Distinct().ToList(),
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
                                                                       join c_p in _context.Cliente_Productos on xe.id_producto equals c_p.Id
                                                                       join sev in _context.CatEstatus_Producto on xe.estatus equals sev.id
                                                                       join prod in _context.Cat_Productos on c_p.Id_Producto equals prod.id
                                                                       join serie in _context.Rel_servicio_producto on c_p.Id equals serie.id_producto //&& serie.id_vista equals 
                                                                       join repa in _context.cat_reparacion on serie.reparacion equals repa.id into _cat_reparacion
                                                                       from cat_reparacion in _cat_reparacion.DefaultIfEmpty()
                                                                       where xe.id_vista == id_visita && serie.id_vista == id_visita
                                                                       select new
                                                                       {
                                                                           id = prod.id,
                                                                           nombre_prodcuto = prod.nombre,
                                                                           estatus_producto = sev.desc_estatus_producto,
                                                                           id_estatus = xe.estatus,
                                                                           //estatus = sev.desc_estatus_producto_en,
                                                                           actividades = xe.actividades,
                                                                           fallas = xe.fallas == null? "" : CultureInfo.CurrentCulture.TextInfo.ToTitleCase(xe.fallas.ToLower()),
                                                                           c_p.no_serie,
                                                                           serie.garantia,
                                                                           prod.modelo,
                                                                           cat_reparacion.desc_reparacion_es,
                                                                           imagenes_reparacion = (from xz in _context.Rel_Imagen_Producto_Visita
                                                                                                  where xz.id_visita == id_visita && xz.id_producto == c_p.Id && xz.checklist == false
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
                                                                                                  u.no_material,
                                                                                                  refaccion = u.descripcion,
                                                                                                  cantidad = y.cantidad,
                                                                                                  precio = Math.Round(o.precio_sin_iva, 2)

                                                                                              }).ToList(),
                                                                           piezas_tecnico = (from y in _context.Piezas_Repuesto_Tecnico
                                                                                             join u in _context.Cat_Materiales on y.id_material equals u.id
                                                                                             where y.id_rel_servicio_refaccion == xe.id
                                                                                             select new
                                                                                             {
                                                                                                 id = y.id,
                                                                                                 refaccion = u.descripcion,
                                                                                                 cantidad = y.cantidad,
                                                                                                 u.no_material,
                                                                                                 precio = u.grupo_precio.precio_sin_iva
                                                                                             }).ToList(),
                                                                           mano_de_obra = (from mo in _context.Rel_Categoria_Producto_Tipo_Producto
                                                                                           where mo.id_categoria == prod.id_sublinea && mo.id_tipo_servicio == xe.visita.servicio.id_tipo_servicio
                                                                                           select new
                                                                                           {
                                                                                               precio_hora_tecnico = Math.Round(Convert.ToDecimal(mo.precio_hora_tecnico/1.16),2),
                                                                                               mo.precio_visita
                                                                                           }).Take(1).ToList()


                                                                       }).ToList()
                                                      }).ToList()
                                       }).Distinct().ToList()
                        }).Distinct().ToList();

            //var todo = DataStorage.
            var sb_cotizacion = new StringBuilder();
            bool iscotizacion = false;
            bool isutilizadas = false;
            double sub_total = 0;
            double iva = 0;
            var sb = new StringBuilder();
            var sb_checklist = new StringBuilder();
            sb.Append(@"
                        <html>
                            <head>
                                <link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css' integrity='sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u' crossorigin='anonymous'>
                            </head>
                            <body>
                                </br>
                                <div class='row'>
                                        <div class='content prueba'>
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
                                                        <h3>Reporte de Visita: Servicio
                                                            <span style = 'color:#A5000D'>" + item[0].id); sb.Append(@" </span>
                                                        </h3>
                                                    </td>
                                                 </tr>
                                              </table>
                                            </div>
                                            <div class='col-md-12'>
                                                 <h3>Información del Servicio:</h3>
                                                 <h4><span style = 'color:#A5000D'>Técnico (s) que diagnóstica </span>");
            foreach (var emp in item[0].visitas[0].tecnico)
            {
                sb.Append(@"<span>" + emp.nombre + " </span>");
            }
            sb.Append(@"</h4> 
                                                 <h4><span style = 'color:#A5000D'>Fecha de atención: </span>" + item[0].visitas[0].fecha_inicio_visita); sb.Append(@" </h4>
                                                 <h4><span style = 'color:#A5000D'>IBS: </span>" + item[0].IBS); sb.Append(@" </h4>
                                                 <h4><span style = 'color:#A5000D'>Tipo de servicio: </span>" + item[0].tipo_servicio); sb.Append(@" </h4>
                                                 <h4><span style = 'color:#A5000D'>Estatus de servicio: </span>" + item[0].estatus_servicio); sb.Append(@" </h4>
                                            </div>
                                            <div class='col-md-12'>
                                                <h3>Información del cliente:</h3>
                                                <h4><span style = 'color:#A5000D'>Nombre: </span>" + item[0].cliente); sb.Append(@" </h4>
                                                <h4><span style = 'color:#A5000D'>Domicilio: </span>" + item[0].visitas[0].direccion); sb.Append(@" </h4>
                                                <h4><span style = 'color:#A5000D'>Teléfono: </span>" + item[0].cliente_telefono); sb.Append(@" </h4>
                                                <h4><span style = 'color:#A5000D'>Actividades a realizar: </span>" + item[0].actividades_realizar); sb.Append(@" </h4>
                                             </div>");
            if (item[0].visitas[0].checklist.Count() == 0)
            {
                sb.Append(@"<div class='col-md-12'>
                                                                                                    <hr style='border-color:black 4px;'>
                                                                                                </div>
                                                                                                <div class='col-md-12'>
                                                                                                    <h3>Problema reportado por el cliente:</h3>
                                                                                                    <h4>" + item[0].actividades_realizar); sb.Append(@" </h4>
                                                                                                     
                                                                                                </div>");
            }
            sb.Append(@"
                                            <div class='col-md-12'>
                                                <hr style='border-color:black 4px;'>
                                            </div>");
            if (item[0].visitas[0].reporte[0].productos.Count() != 0)
            {
                sb.Append(@"
                                                                                                                                    <div class='col-md-12' style='margin-bottom: 20px'>
                                                                                                                                    <h3>Actividades realizadas</h3>
                                                                                                                                    <table style='width: 100%; text-align: left'>");
                foreach (var emp in item[0].visitas[0].reporte[0].productos)
                {
                    sb.Append(@"
                                                <tr>
                                                    <td>
                                                            <h4><span style = 'color:#A5000D'>Producto: </span>" + emp.nombre_prodcuto); sb.Append(@" </h4>
                                                            <h4><span style = 'color:#A5000D'>Modelo: </span>" + emp.modelo); sb.Append(@" </h4>
                                                            <h4><span style = 'color:#A5000D'>No. Serie: </span>" + emp.no_serie); sb.Append(@" </h4>
                                                            <h4><span style = 'color:#A5000D'>Dictamen técnico: </span>" + emp.fallas); sb.Append(@" </h4>
                                                            <h4><span style = 'color:#A5000D'>Actividad realizada: </span>" + emp.desc_reparacion_es);
                    //sb.Append(@" </h4>
                    //                                        <h4><span style = 'color:#A5000D'>Garantía: </span>"); if (emp.garantia)
                    //{
                    //    sb.Append(@"Si");
                    //}
                    //else
                    //{
                    //    sb.Append(@"No");
                    //}
                    sb.Append(@" </h4>
                                                                                                                                                    
                                                            <h4><span style = 'color:#A5000D'>Diagnóstico técnico: </span>" + emp.actividades); sb.Append(@" </h4>
                                                            <h4><span style = 'color:#A5000D'>Estatus de producto: </span>" + emp.estatus_producto); sb.Append(@" </h4>");
                    sb.Append(@" </td></tr>");
                    for (int i = 0; i < item[0].visitas[0].reporte[0].productos[0].piezas_tecnico.Count(); i++)
                    {

                        if (item[0].visitas[0].reporte[0].productos[0].piezas_tecnico.Count() != 0)
                        {
                            isutilizadas = true;
                            break;
                        }
                    }
                    if (isutilizadas)
                    {
                        sb.Append(@" <tr><td>
                            <h4><span style = 'color:#A5000D; width: 700px;'> Refacciones utilizadas:" + item[0].visitas[0].reporte[0].productos[0].piezas_tecnico[0].refaccion + "</span> <span style = 'left: 570px; position: fixed'> Cantidad </span> <span style='left: 850px; position: fixed'> Total</span> </h4>");
                        foreach (var refacciones in emp.piezas_tecnico)
                        {
                            sb.Append(@" 
                                                                                    <table style='width: 100%;'>
                                                                                      <tr>                                                                                       
                                                                                        <td style='padding:0 10px 0 0; width: 600px;'> 
                                                                                           <h4>" + refacciones.refaccion + "  -  " + refacciones.no_material); sb.Append(@"</h4>
                                                                                        </td>
                                                                                        <td style='text-align: right'>
                                                                                             <table>
                                                                                                <tr>
                                                                                                    <td style='padding:0 10px 0 10px; width: 120px;'><h4>" + refacciones.cantidad); sb.Append(@"</h4>
                                                                                                    </td>
                                                                                                    <td style='padding:0 10px 0 10px; width: 85px; text-align: -webkit-right;'>
                                                                                                        <h4><span></span>"); sb.Append(@"</h4>
                                                                                                    </td>
                                                                                                    <td style='padding:0 10px 0 10px; width: 100px; text-align: -webkit-right;'>");
                            if (emp.garantia)
                            {
                                sb.Append(@"<h4><span>$0.00</span>");
                            }
                            else
                            {
                                sb.Append(@"<h4><span>$" + Convert.ToDouble(refacciones.precio).ToString("0.00") + "</span>");
                            }
                            sb.Append(@""); sb.Append(@"</h4>
                                                                                                    </td>
                                                                                                </tr>
                                                                                             </table>
                                                                                             
                                                                                        </td>
                                                                                     </tr></table>");

                        }
                        sb.Append(@"</tr></td>");
                    }
                    sb.Append(@"<tr><td><h3>Evidencia de la Visita</h3></td></tr>");


                    sb.Append(@"<table style='width: 100%; text-align: center'>");
                    if (emp.imagenes_reparacion.Count() == 1)
                    {
                        sb.Append(@"<tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[0].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[0].actividad + "</span>");
                        sb.Append(@"</td></tr>");
                    }
                    if (emp.imagenes_reparacion.Count() == 2)
                    {
                        sb.Append(@"<tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[0].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[0].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[1].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[1].actividad + "</span>" +
                                    "</td></tr>");
                    }
                    if (emp.imagenes_reparacion.Count() == 3)
                    {
                        sb.Append(@"<tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[0].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[0].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[1].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[1].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[2].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[2].actividad + "</span>" +
                                    "</td></tr>");
                    }
                    if (emp.imagenes_reparacion.Count() == 4)
                    {
                        sb.Append(@"<tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[0].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[0].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[1].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[1].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[2].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[2].actividad + "</span>");
                        sb.Append(@"</td></tr>
                                    <tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[3].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[3].actividad + "</span>" +
                                    "</td></tr>");
                    }
                    if (emp.imagenes_reparacion.Count() == 5)
                    {
                        sb.Append(@"<tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[0].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[0].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[1].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[1].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[2].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[2].actividad + "</span>");
                        sb.Append(@"</td></tr>
                                    <tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[3].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[3].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[4].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[4].actividad + "</span>" +
                                    "</td></tr>");
                    }
                    if (emp.imagenes_reparacion.Count() == 6)
                    {
                        sb.Append(@"<tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[0].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[0].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[1].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[1].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[2].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[2].actividad + "</span>");
                        sb.Append(@"</td></tr>
                                    <tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[3].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[3].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[4].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[4].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[5].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[5].actividad + "</span>" +
                                    "</td></tr>");
                    }
                    if (emp.imagenes_reparacion.Count() == 7)
                    {
                        sb.Append(@"<tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[0].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[0].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[1].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[1].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[2].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[2].actividad + "</span>");
                        sb.Append(@"</td></tr>
                                    <tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[3].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[3].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[4].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[4].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[5].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[5].actividad + "</span>");
                        sb.Append(@"</td></tr>
                                    <tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[6].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[6].actividad + "</span>");
                        sb.Append(@"</td></tr>");
                    }
                    if (emp.imagenes_reparacion.Count() == 8)
                    {
                        sb.Append(@"<tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[0].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[0].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[1].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[1].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[2].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[2].actividad + "</span>");
                        sb.Append(@"</td></tr>
                                    <tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[3].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[3].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[4].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[4].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[5].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[5].actividad + "</span>");
                        sb.Append(@"</td></tr>
                                    <tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[6].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[6].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[7].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[7].actividad + "</span>" +
                                   "</td></tr>");
                    }
                    if (emp.imagenes_reparacion.Count() == 9)
                    {
                        sb.Append(@"<tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[0].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[0].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[1].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[1].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[2].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[2].actividad + "</span>");
                        sb.Append(@"</td></tr>
                                    <tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[3].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[3].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[4].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[4].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[5].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[5].actividad + "</span>");
                        sb.Append(@"</td></tr>
                                    <tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[6].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[6].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[7].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[7].actividad + "</span>");
                        sb.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + emp.imagenes_reparacion[8].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + emp.imagenes_reparacion[8].actividad + "</span>" +
                                  "</td></tr>");
                    }
                    sb.Append(@"</table><table><hr style='border-color:black 4px;'></table>");
                }
                sb.Append(@"
                                                                                                                                 </table></div>");

            }

            sb.Append(@"
                                                 <div class='col-md-12' style='text-align:center;'> 

                                                    <img heigth='180px' width='180px' src='"+ selRuta.funcion + item[0].visitas[0].imagen_firma + "'><hr style='border-color:black 4px; width: 300px;'>" +
                                                    "<h3>" + item[0].visitas[0].persona_recibe + "</h3>");
            sb.Append(@"
                                                </div>
                                      </div>
                                 </div>
                            </body>
                        </html>");

            for (int i = 0; i < item[0].visitas[0].reporte[0].productos.Count(); i++)
            {
                if (item[0].visitas[0].reporte[0].productos[i].piezas_repuesto.Count() != 0)
                {
                    iscotizacion = true;
                    break;
                }
            }
            if (iscotizacion)
            {
                sb_cotizacion.Append(@"
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
                                                        <h3>Cotización: Servicio
                                                            <span style = 'color:#A5000D'>" + item[0].id); sb_cotizacion.Append(@" </span>
                                                        </h3>
                                                    </td>
                                                 </tr>
                                              </table>
                                            </div>
                                            <div class='col-md-12'>
                                                 <table style='width: 100%; text-align: left'>
                                                  <tr>
                                                    <td style='width: 670px'>
                                                       
                                                    </td>
                                                    <td text-align: rigth'>
                                                        <span style = 'color:#A5000D'>Fecha de atención: </span > 
                                                        " + item[0].visitas[0].fecha_inicio_visita); sb_cotizacion.Append(@"
                                                    </td>
                                                 </tr>                                                 
                                                </table>");
                sb_cotizacion.Append(@"
                                            </div>
                                            <div class='col-md-12'>
                                                <h3>Información del cliente:</h2>
                                                <h4><span style = 'color:#A5000D'>Nombre: </span>" + item[0].cliente); sb_cotizacion.Append(@" </h4>
                                                <h4><span style = 'color:#A5000D'>Domicilio: </span>" + item[0].visitas[0].direccion); sb_cotizacion.Append(@" </h4>
                                                <h4><span style = 'color:#A5000D'>Teléfono: </span>" + item[0].cliente_telefono); sb_cotizacion.Append(@" </h4>
                                                                                        
                                            
                                            </div>");

                sb_cotizacion.Append(@"
                                            <div class='col-md-12'>
                                                <hr style='border-color:black 4px;'>
                                            </div>");
                if (item[0].visitas[0].reporte[0].productos.Count() != 0)
                {
                    sb_cotizacion.Append(@"
                                                                        <div class='col-md-12'>
                                                                            <h3>Productos y refacciones asociadas:</h3>
                                                                            <table style = 'width: 100%; text-align: left'> ");
                    var num_visitas = _context.Visita.Where(a => a.id_servicio == id).ToList();
                    int _re = 0;
                    double costo_visita = 0;
                    foreach (var emp in item[0].visitas[0].reporte[0].productos)
                    {

                        if (emp.garantia)
                        {
                            //sub_total += Convert.ToDouble(0) + Convert.ToDouble(item[0].visitas[0].reporte[0].productos[_re].mano_de_obra[0].precio_visita);
                            //iva += ((Convert.ToDouble(0) + item[0].visitas[0].reporte[0].productos[_re].mano_de_obra[0].precio_visita.Value) * 0.16);
                        }
                        else
                        {
                            
                            //sub_total += Convert.ToDouble(item[0].visitas[0].reporte[0].productos[_re].piezas_repuesto[_re].cantidad * item[0].visitas[0].reporte[0].productos[_re].piezas_repuesto[_re].precio) + Convert.ToDouble(item[0].visitas[0].reporte[0].productos[_re].mano_de_obra[0].precio_visita);
                            //iva +=     ((Convert.ToDouble(item[0].visitas[0].reporte[0].productos[_re].piezas_repuesto[_re].cantidad * item[0].visitas[0].reporte[0].productos[_re].piezas_repuesto[_re].precio) + item[0].visitas[0].reporte[0].productos[_re].mano_de_obra[0].precio_hora_tecnico.Value + costo_visita) * 0.16);
                            sub_total += Convert.ToDouble(item[0].visitas[0].reporte[0].productos[_re].mano_de_obra[0].precio_hora_tecnico) + costo_visita;
                            double iva2 = sub_total * 0.16;
                        }

                        if (emp.piezas_repuesto.Count() != 0)
                        {
                            //revisar si es la primera visita y el primer producto y en funion a eso el costo de viaticos vale diferente
                            if (num_visitas.Count == 1)
                            {
                                if (costo_visita == 0)
                                    costo_visita = Convert.ToDouble(item[0].visitas[0].reporte[0].productos[_re].mano_de_obra[0].precio_visita);
                                else
                                    costo_visita = Convert.ToDouble(item[0].visitas[0].reporte[0].productos[_re].mano_de_obra[0].precio_visita) - 200;
                            }
                            sb_cotizacion.Append(@"
                                                        <tr>
                                                            <td>
                                                                <h4><span style = 'color:#A5000D'>Producto: </span>" + emp.nombre_prodcuto); sb_cotizacion.Append(@" </h4>
                                                                <h4><span style = 'color:#A5000D'>Modelo: </span>" + emp.modelo); sb_cotizacion.Append(@" </h4>
                                                                <h4><span style = 'color:#A5000D'>No. Serie: </span>" + emp.no_serie);
                            //sb_cotizacion.Append(@" </h4>
                            //                                    <h4><span style = 'color:#A5000D'>Garantía: </span>");

                            //if (emp.garantia)
                            //{
                            //    sb_cotizacion.Append(@"Si");
                            //}
                            //else
                            //{
                            //    sb_cotizacion.Append(@"No");

                            //}
                            sb_cotizacion.Append(@" </h4>
                                                        
                                                                <h4><span style = 'color:#A5000D; width: 700px;'>Refacción solicitada: </span> <span style='left: 550px; position: fixed'>Cantidad</span> <span style='left: 670px; position: fixed'> Precio unitario</span> <span style='left: 850px; position: fixed'> Total</span> </h4>");

                            foreach (var refacciones in emp.piezas_repuesto)
                            {
                                sb_cotizacion.Append(@" 
                                                                                    <table style='width: 100%;'>
                                                                                      <tr>
                                                                                       
                                                                                        <td style='padding:0 10px 0 0; width: 600px;'> 
                                                                                           <h4>" + refacciones.refaccion + "  -  " + refacciones.no_material); sb_cotizacion.Append(@"</h4>
                                                                                        </td>
                                                                                        <td style='text-align: right'>
                                                                                             <table>
                                                                                                <tr>
                                                                                                    <td style='padding:0 10px 0 10px; width: 120px;'><h4>" + refacciones.cantidad); sb_cotizacion.Append(@"</h4>
                                                                                                    <td>
                                                                                                    <td style='padding:0 10px 0 10px; width: 85px; text-align: -webkit-right;'>
                                                                                                        <h4>" + Convert.ToDouble(refacciones.precio).ToString("c", new CultureInfo("es-MX"))); sb_cotizacion.Append(@"</h4>
                                                                                                    </td>
                                                                                                    <td style='padding:0 10px 0 10px; width: 145px; text-align: -webkit-right;'>");
                                if (emp.garantia)
                                {
                                    sb_cotizacion.Append(@"<h4><span>$</span>0.00</h4>");
                                }
                                else
                                {
                                    sub_total += Convert.ToDouble(refacciones.cantidad * refacciones.precio);
                                    sb_cotizacion.Append(@"<h4><span>" + Convert.ToDouble(refacciones.cantidad * refacciones.precio).ToString("c", new CultureInfo("es-MX"))); sb_cotizacion.Append(@"</h4>");

                                }


                                sb_cotizacion.Append(@" </td>
                                                                                                <tr>
                                                                                             </table>
                                                                                             
                                                                                        </td>
                                                                                     </tr>                                                                                    
                                                                                    </table>");



                            }
                            sb_cotizacion.Append(@"
                                                                    </td>
                                                          </tr>");

                            sb_cotizacion.Append(@"</table>
                                                    <table style='width: 100%;'>
                                                                                      <tr>
                                                                                        <td style='padding:0 10px 0 0; width: 600px;'> 
                                                                                            <h4>Costo por " + item[0].tipo_servicio.ToLower() + @"</h4>
                                                                                        </td>
                                                                                        <td style='text-align: left'>
                                                                                             <table>
                                                                                                <tr>
                                                                                                    <td style='padding:0 10px 0 10px; width: 120px;'>"); sb_cotizacion.Append(@"</h4>
                                                                                                    <td>
                                                                                                    <td style='padding:0 10px 0 10px; width: 80px; text-align: -webkit-right;'><h4>");
                            
                            //se sustituye  if de abajo comentado por esta linea simple,
                            sb_cotizacion.Append(Convert.ToDouble(emp.mano_de_obra[0].precio_hora_tecnico).ToString("c", new CultureInfo("es-MX")));
                            
                            
                            //if (emp.mano_de_obra[0].precio_hora_tecnico.ToString().IndexOf(".") != -1)
                            //{
                            //    if (emp.mano_de_obra[0].precio_hora_tecnico.ToString().Split(".")[0].Length == 2)
                            //    {
                            //        sb_cotizacion.Append(@"$" + emp.mano_de_obra[0].precio_visita);
                            //    }
                            //    else
                            //    {
                            //        sb_cotizacion.Append(@"$" + emp.mano_de_obra[0].precio_visita + "0");
                            //    }
                            //}
                            //else
                            //{
                            //    sb_cotizacion.Append(@"$" + emp.mano_de_obra[0].precio_visita + ".00");
                            //}
                            sb_cotizacion.Append(@" </h4>
                                                                                                    </td>
                                                                                                    <td style='padding:0 10px 0 10px; width: 145px; text-align: -webkit-right;'><h4>");
                            if (emp.garantia)
                            {
                                sb_cotizacion.Append(@"<span>$0.00</span>");
                            }
                            else
                            {
                                //sb_cotizacion.Append(Convert.ToDouble(emp.mano_de_obra[0].precio_visita).ToString("c", new CultureInfo("es-MX")));
                                sb_cotizacion.Append(Convert.ToDouble(1 * emp.mano_de_obra[0].precio_hora_tecnico).ToString("c", new CultureInfo("es-MX")));
                                //if ((1 * emp.mano_de_obra[0].precio_hora_tecnico).ToString().IndexOf(".") != -1)
                                //{
                                //    if ((1 * emp.mano_de_obra[0].precio_hora_tecnico).ToString().Split(".")[0].Length == 2)
                                //    {
                                //        sb_cotizacion.Append(@"$" + (1 * emp.mano_de_obra[0].precio_visita));
                                //    }
                                //    else
                                //    {
                                //        sb_cotizacion.Append(@"$" + (1 * emp.mano_de_obra[0].precio_visita) + "0");
                                //    }
                                //}
                                //else
                                //{
                                //    sb_cotizacion.Append(@"$" + (1 * emp.mano_de_obra[0].precio_visita) + ".00");
                                //}

                            }

                            sb_cotizacion.Append(@"</h4>
                                                                                                    </td>
                                                                                                <tr>
                                                                                             </table>
                                                                                        </td>
                                                                                     </tr>");
                            //se agrega costo de visita solo en la primera visita
                            //if (num_visitas.Count >= 1 )
                            //{
                            //    sb_cotizacion.Append(@"<tr>
                            //                               <td style='padding:0 10px 0 0; width: 600px;'>
                            //                                   <h4>Costo por diagnóstico</h4>
                            //                               </td>
                            //                               <td style='text-align: right'>
                            //                                   <table>
                            //                                       <tr>
                            //                                           <td style='padding:0 10px 0 10px; width: 120px;'></td>
                            //                                           <td style='padding:0 10px 0 10px; width: 80px; text-align: -webkit-right;'><h4>"+ Convert.ToDouble(emp.mano_de_obra[0].precio_visita).ToString("c", new CultureInfo("es-MX")) + @" </h4></td>
                            //                                           <td style='padding:0 10px 0 10px; width: 145px; text-align: -webkit-right;'><h4>" + costo_visita.ToString("c", new CultureInfo("es-MX")) + @"</h4></td>
                            //                                       <tr>
                            //                                   </table>
                            //                               </td>    
                            //                           </tr>");
                                

                            //}
                            sb_cotizacion.Append(@"</table>");
                            sb_cotizacion.Append(@" <div class='col-md-12' style='padding: 10px 0 10px 0'>
                                                                                        <hr style='border-color:black 4px;'>
                                                                                    </div>");
                        }
                        _re = _re + 1;
                    }


                    sb_cotizacion.Append(@"
                                           <div class='col-md-12'>
                                                 <table style='width: 100%;'>
                                                  <tr>
                                                    <td style='width: 850px'>
                                                       
                                                    </td>
                                                    <td style='text-align: -webkit-right; width: 145px'>
                                                        <h4>Subtotal:</h4>
                                                    </td>
                                                    <td  style='text-align: -webkit-right; width: 145px'>
                                                        <h4>");
                    double _sub_total = sub_total;
                    sb_cotizacion.Append(_sub_total.ToString("c", new CultureInfo("es-MX")));
                    //if (_sub_total.ToString().IndexOf(".") != -1)
                    //{
                    //    if (_sub_total.ToString().Split(".")[0].Length == 2)
                    //    {
                    //        sb_cotizacion.Append(@"$" + _sub_total);
                    //    }
                    //    else
                    //    {
                    //        sb_cotizacion.Append(@"$" + _sub_total.ToString() + "0");
                    //    }
                    //}
                    //else
                    //{
                    //    sb_cotizacion.Append(@"$" + _sub_total.ToString() + ".00");
                    //}
                    sb_cotizacion.Append(@"</h4>
                                                    <td>
                                                 </tr>
                                                  <tr>
                                                    <td style='width: 850px'>
                                                       
                                                    </td>
                                                    <td style='text-align: -webkit-right; width: 145px'>
                                                        <h4>IVA:</h4>
                                                    </td>
                                                    <td  style='text-align: -webkit-right; width: 145px'>
                                                       <h4>");
                    iva = sub_total * 0.16;
                    double _iva = iva;
                    sb_cotizacion.Append(_iva.ToString("c", new CultureInfo("es-MX")));
                    //if (_iva.ToString().IndexOf(".") != -1)
                    //{
                    //    if (_iva.ToString().Split(".")[0].Length == 2)
                    //    {
                    //        sb_cotizacion.Append(@"$" + _iva);
                    //    }
                    //    else
                    //    {
                    //        sb_cotizacion.Append(@"$" + _iva.ToString() + "0");
                    //    }
                    //}
                    //else
                    //{
                    //    sb_cotizacion.Append(@"$" + _iva.ToString() + ".00");
                    //}
                    sb_cotizacion.Append(@"</h4>
                                                    <td>
                                                 </tr>
                                                  <tr>
                                                    <td style='width: 850px'>
                                                       
                                                    </td>
                                                    <td style='text-align: -webkit-right; width: 145px'>
                                                        <h4>Total:</h4>
                                                    </td>
                                                    <td  style='text-align: -webkit-right; width: 145px'>
                                                         <h4>");
                    double _total = Math.Round(sub_total +iva, 2);
                    sb_cotizacion.Append(_total.ToString("c", new CultureInfo("es-MX"))); 
                    //if ((_total).ToString().IndexOf(".") != -1)
                    //{
                    //    if ((_total).ToString().Split(".")[0].Length == 2)
                    //    {
                    //        sb_cotizacion.Append(@"$" + _total);
                    //    }
                    //    else
                    //    {
                    //        sb_cotizacion.Append(@"$" + _total.ToString() + "0");
                    //    }
                    //}
                    //else
                    //{
                    //    sb_cotizacion.Append(@"$" + _total.ToString() + ".00");
                    //}
                    sb_cotizacion.Append(@"</h4>
                                                    <td>
                                                 </tr>
                                                </table>");
                    sb_cotizacion.Append(@"<div class='col-md-12' style='text - align: left; font-size:12px; letter-spacing:0.5pt; vertical-align:top'> 
						<span><strong> IMPORTANTE </strong>
                            <br /> -Si Usted está de acuerdo con este presupuesto, por favor envíenos su confirmación al correo info@miele.com.mx para que el servicio de reparación pueda ser programado. También puede confirmar llamándonos al 01800 MIELE 00(01 800 64353 00)
                            <br /> -Los tiempos de reparación están sujetos a la disponibilidad de las refacciones necesarias
                            <br /> -Después de la reparación, refacciones adicionales pueden ser necesarias para completar un funcionamiento óptimo del equipo. Esto como consecuencia de las funcionalidades del equipo que hayan sido afectadas por la falla inicial. Usted podría recibir una cotización adicional de manera posterior
                            <br /> -La vigencia de esta cotización es de 15 días naturales a partir de la fecha de su envío
                      </span></div>");
                    sb_cotizacion.Append(@"
                                            </div>");
                }
                sb_cotizacion.Append(@"                                               
                                         </div>
                                      </div>
                                    </body>
                                </html>");
            }
            else
            {
                sb_cotizacion.Append(@"");
            }

            if (item[0].visitas[0].checklist.Count() != 0)
            {
                {
                    sb_checklist.Append(@"
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
                                                        <img heigth='20%' width='20%' src='"+ selRuta.funcion +@"/mieletickets/Imagenes_Productos/miele-logo-immer-besser.png'>
                                                    </td>
                                                 </tr>
                                              </table>
                                              <table style='width: 100%; text-align: left'>
                                                  <tr>
                                                    <td>
                                                        <h3>Reporte de Visita: Servicio
                                                            <span style = 'color:#A5000D'>" + item[0].id); sb_checklist.Append(@" </span>
                                                        </h3>
                                                    </td>
                                                 </tr>
                                              </table>
                                            </div>
                                            <div class='col-md-12'>
                                                 <h3>Información del Servicio:</h3>
                                                 <h4><span style = 'color:#A5000D'>Técnico (s) que diagnóstica </span>");
                    foreach (var emp in item[0].visitas[0].tecnico)
                    {
                        sb_checklist.Append(@"<span>" + emp.nombre + " </span>");
                    }
                    sb_checklist.Append(@"</h4> 
                                                 <h4><span style = 'color:#A5000D'>Fecha de atención: </span>" + item[0].visitas[0].fecha_inicio_visita); sb_checklist.Append(@" </h4>
                                                 <h4><span style = 'color:#A5000D'>IBS: </span>" + item[0].IBS); sb_checklist.Append(@" </h4>
                                                 <h4><span style = 'color:#A5000D'>Tipo de servicio: </span>" + item[0].tipo_servicio); sb_checklist.Append(@" </h4>
                                                 <h4><span style = 'color:#A5000D'>Estatus de servicio: </span>" + item[0].estatus_servicio); sb_checklist.Append(@" </h4>
                                            </div>
                                            <div class='col-md-12'>
                                                <h3>Información del cliente:</h3>
                                                <h4><span style = 'color:#A5000D'>Nombre: </span>" + item[0].cliente); sb_checklist.Append(@" </h4>
                                                <h4><span style = 'color:#A5000D'>Domicilio: </span>" + item[0].visitas[0].direccion); sb_checklist.Append(@" </h4>
                                                <h4><span style = 'color:#A5000D'>Teléfono: </span>" + item[0].cliente_telefono); sb_checklist.Append(@" </h4>
                                             </div>");
                    sb_checklist.Append(@"
                                            <div class='col-md-12'>
                                                <hr style='border-color:black 4px;'>
                                            </div>");

                    sb_checklist.Append(@"
                                                                                                                                    <div class='col-md-12' style='margin-bottom: 20px'>
                                                                                                                                   
                                                                                                                                    <table style='width: 100%; text-align: left'>");
                    sb_checklist.Append(@"</div><div class='col-md-12' style='padding:0;'><h3>Evidencia de la visita</h3></div>");
                    int xi = 0;
                    foreach (var emp in item[0].visitas[0].checklist)
                    {
                        sb_checklist.Append(@"
                                                                                                                                       <tr>
                                                                                                                                            <td>
                                                                                                                                                  <h4><span style = 'color:#A5000D'>Producto: </span>" + emp.nombre); sb_checklist.Append(@" </h4>
                                                                                                                                                  <h4><span style = 'color:#A5000D'>Modelo: </span>" + emp.modelo); sb_checklist.Append(@" </h4>
                                                                                                                                                    <h4><span style = 'color:#A5000D'>Estatus de producto: </span>" + emp.estatus_prodcuto_checklist); sb_checklist.Append(@" </h4>
                            <h4><span style = 'color:#A5000D'>Garantía: </span>");
                        if (emp.garantia)
                        {
                            sb_checklist.Append(@"Si");
                        }
                        else
                        {
                            sb_checklist.Append(@"No");
                        }
                        sb_checklist.Append(@" </h4>");


                        sb_checklist.Append(@" </td></tr>");
                        sb_checklist.Append(@"<table style='width: 100%; text-align: center'><tr><td style='text-align: left; width: 50%;'>
    <h4><span style = 'color:#A5000D'>Punto de revisión </span></h4>
</td>
<td style='text-align: left; width: 40%;'>
    <h4><span style = 'color:#A5000D;'>Comentario</span></h4>
</td>
<td style='text-align: center; width: 10%;'>
    <h4><span style = 'color:#A5000D; width: 50%;'>Estatus</span></h4>
</td>
</tr>");

                        for (var z = 0; z < item[0].visitas[0].checklist[xi].preguntas.Count(); z++)
                        {
                            sb_checklist.Append(@"<tr>
<td style='text-align: left; padding-top: 10px; padding-bottom: 10px;'>"
+ (z + 1) + "- " + item[0].visitas[0].checklist[xi].preguntas[z].pregunta); sb_checklist.Append(@"
</td>
<td style='text-align: left; padding-top: 10px; padding-bottom: 10px;'>"
+ item[0].visitas[0].checklist[xi].preguntas[z].comentario); sb_checklist.Append(@"    
</td>
<td style='text-align: center; padding-top: 10px; padding-bottom: 10px;'>");
                            if (item[0].visitas[0].checklist[xi].preguntas[z].respuesta)
                            {
                                sb_checklist.Append(@"Si");
                            }
                            else
                            {
                                sb_checklist.Append(@"No");
                            }
                            sb_checklist.Append(@"
</td>
</tr>
<tr>
<td style='width: 50%;'>
    <hr style='border-color:black 4px;'>
</td>
<td style='width: 40%;'>
    <hr style='border-color:black 4px;'>
</td>
<td style='width: 10%;'>
    <hr style='border-color:black 4px;'>
</td>
</tr>");
                        }

                        sb_checklist.Append(@" <table style='width: 100%; text-align: center'>");
                        //for (var i = 0; i < emp.imagenes_reparacion.Count(); i++)
                        //{


                        //}
                        if (item[0].visitas[0].checklist[xi].imagenes_checklist.Count() == 1)
                        {
                            sb_checklist.Append(@"<tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[0].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[0].actividad + "</span>");
                            sb_checklist.Append(@"</td></tr>");
                        }
                        if (item[0].visitas[0].checklist[xi].imagenes_checklist.Count() == 2)
                        {
                            sb_checklist.Append(@"<tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[0].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[0].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[1].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[1].actividad + "</span>" +
                                        "</td></tr>");
                        }
                        if (item[0].visitas[0].checklist[xi].imagenes_checklist.Count() == 3)
                        {
                            sb_checklist.Append(@"<tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[0].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[0].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[1].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[1].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[2].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[2].actividad + "</span>" +
                                        "</td></tr>");
                        }
                        if (item[0].visitas[0].checklist[xi].imagenes_checklist.Count() == 4)
                        {
                            sb_checklist.Append(@"<tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[0].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[0].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[1].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[1].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[2].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[2].actividad + "</span>");
                            sb_checklist.Append(@"</td></tr>
                                    <tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[3].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[3].actividad + "</span>" +
                                        "</td></tr>");
                        }
                        if (item[0].visitas[0].checklist[xi].imagenes_checklist.Count() == 5)
                        {
                            sb_checklist.Append(@"<tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[0].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[0].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[1].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[1].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[2].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[2].actividad + "</span>");
                            sb_checklist.Append(@"</td></tr>
                                    <tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[3].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[3].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[4].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[4].actividad + "</span>" +
                                        "</td></tr>");
                        }
                        if (item[0].visitas[0].checklist[xi].imagenes_checklist.Count() == 6)
                        {
                            sb_checklist.Append(@"<tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[0].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[0].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[1].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[1].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[2].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[2].actividad + "</span>");
                            sb_checklist.Append(@"</td></tr>
                                    <tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[3].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[3].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[4].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[4].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[5].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[5].actividad + "</span>" +
                                        "</td></tr>");
                        }
                        if (item[0].visitas[0].checklist[xi].imagenes_checklist.Count() == 7)
                        {
                            sb_checklist.Append(@"<tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[0].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[0].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[1].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[1].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[2].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[2].actividad + "</span>");
                            sb_checklist.Append(@"</td></tr>
                                    <tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[3].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[3].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[4].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[4].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[5].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[5].actividad + "</span>");
                            sb_checklist.Append(@"</td></tr>
                                    <tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[6].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[6].actividad + "</span>");
                            sb_checklist.Append(@"</td></tr>");
                        }
                        if (item[0].visitas[0].checklist[xi].imagenes_checklist.Count() == 8)
                        {
                            sb_checklist.Append(@"<tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[0].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[0].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[1].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[1].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[2].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[2].actividad + "</span>");
                            sb_checklist.Append(@"</td></tr>
                                    <tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[3].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[3].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[4].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[4].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[5].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[5].actividad + "</span>");
                            sb_checklist.Append(@"</td></tr>
                                    <tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[6].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[6].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[7].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[7].actividad + "</span>" +
                                       "</td></tr>");
                        }
                        if (item[0].visitas[0].checklist[xi].imagenes_checklist.Count() == 9)
                        {
                            sb_checklist.Append(@"<tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[0].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[0].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[1].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[1].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[2].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[2].actividad + "</span>");
                            sb_checklist.Append(@"</td></tr>
                                    <tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[3].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[3].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[4].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[4].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[5].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[5].actividad + "</span>");
                            sb_checklist.Append(@"</td></tr>
                                    <tr><td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[6].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[6].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[7].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[7].actividad + "</span>");
                            sb_checklist.Append(@"</td>
                                    <td style='width: 33.3333%;'>
                                        <img src='"+ selRuta.funcion + item[0].visitas[0].checklist[xi].imagenes_checklist[8].url + "' style='padding: 10px 10px 10px 10px' height='300' width='300'><br/>" + "<span style='padding: 10px 10px 10px 10px; width:300px;'>" + item[0].visitas[0].checklist[0].imagenes_checklist[8].actividad + "</span>" +
                                      "</td></tr>");
                        }
                        sb_checklist.Append(@"</table><table><hr style='border-color:black 4px;'></table>");
                        xi = xi + 1;
                    }
                    sb_checklist.Append(@"
                                                                                                                                 </div>");

                    sb_checklist.Append(@"
                                                 <div class='col-md-12' style='text-align:center;'> 

                                                    <img heigth='180px' width='180px' src='"+ selRuta.funcion + item[0].visitas[0].imagen_firma + "'><hr style='border-color:black 4px; width: 300px;'>" +
                                                            "<h3>" + item[0].visitas[0].persona_recibe + "</h3>");
                    sb_checklist.Append(@"
                                                </div>
                                      </div>
                                 </div>
                            </body>
                        </html>");
                }
            }
            else
            {
                sb_checklist.Append(@"");
            }
            string[] _array = new string[] { };
            if (item[0].visitas[0].reporte[0].productos.Count() != 0)
            {
                _array = new string[] { sb.ToString(), sb_cotizacion.ToString(), sb_checklist.ToString(), "1" };
            }
            else
            {
                _array = new string[] { sb.ToString(), sb_cotizacion.ToString(), sb_checklist.ToString(), "0" };
            }
            return _array;
        }

        [Route("api/PDFCreator/{id}/{id_visita}")]
        [HttpGet("{id}/{id_visita}", Name = "Get_pdf")]
        public IActionResult CreatePDFAsync(long id, long id_visita)
        {
            string[] _pdf = TemplatePDF(id, id_visita);
            var filePath = Environment.CurrentDirectory;
            var _guid = Guid.NewGuid();
            var _guid_cotizacion = Guid.NewGuid();
            var _guid_checklist = Guid.NewGuid();
            var path = "/Imagenes/pdf_reportes/" + _guid + ".pdf";
            var path_cotizacion = "/Imagenes/pdf_reportes/" + _guid_cotizacion + ".pdf";
            var path_checklist = "/Imagenes/pdf_reportes/" + _guid_checklist + ".pdf";
            var _pdf_envio = _context.Visita.SingleOrDefault(x => x.id == id_visita);
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

                HtmlContent = _pdf[0],
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

            var globalSettings_cotizacion = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report",
                Out = filePath + path_cotizacion  //USE THIS PROPERTY TO SAVE PDF TO A PROVIDED LOCATION
            };

            var objectSettings_cotizacion = new ObjectSettings
            {
                PagesCount = true,

                HtmlContent = _pdf[1],
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Line = true, Right = "Página [page] de [toPage]" },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Av. Santa Fe 170, Col. Lomas de Santa Fe, C.P. 01210, Ciudad de México. Contacto.  Tel: 01 800 MIELE 00 (01 800 64353 00)" }
            };

            var pdf_cotizacion = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings_cotizacion,
                Objects = { objectSettings_cotizacion }
            };

            var globalSettings_checklist = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle = "PDF Report",
                Out = filePath + path_checklist  //USE THIS PROPERTY TO SAVE PDF TO A PROVIDED LOCATION
            };

            var objectSettings_checklist = new ObjectSettings
            {
                PagesCount = true,

                HtmlContent = _pdf[2],
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Line = true, Right = "Página [page] de [toPage]" },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Av. Santa Fe 170, Col. Lomas de Santa Fe, C.P. 01210, Ciudad de México. Contacto.  Tel: 01 800 MIELE 00 (01 800 64353 00)" }
            };

            var pdf_checklist = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings_checklist,
                Objects = { objectSettings_checklist }
            };

            //_converter.Convert(pdf); IF WE USE Out PROPERTY IN THE GlobalSettings CLASS, THIS IS ENOUGH FOR CONVERSION

            var file = _converter.Convert(pdf);
            var file_cotizacion = _converter.Convert(pdf_cotizacion);
            var file_checklist = _converter.Convert(pdf_checklist);

            var item = (from a in _context.Servicio
                        join b in _context.Clientes on a.id_cliente equals b.id
                        where a.id == id
                        select new
                        {
                            nombre = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(b.nombre) + " " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(b.paterno) + " " + (b.materno != "" ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(b.materno) : ""),
                            mail = b.email,
                            a.contacto

                        }).ToList();

            var path_template = Path.GetFullPath("TemplateMail/Email_reporte.html");
            StreamReader reader = new StreamReader(Path.GetFullPath("TemplateMail/Email_reporte.html"));
            string body = string.Empty;
            body = reader.ReadToEnd();
            body = body.Replace("{username}", item[0].nombre);

            

            List<Attachment> adjuntos = new List<Attachment>();
            if (_pdf[3] == "1")
            {
                var fs = new FileStream(filePath + path, FileMode.Open);
                adjuntos.Add(new Attachment(fs, "Reporte de Actividades.pdf", "text/pdf"));
            }
            if (_pdf[1] != "")
            {
                _pdf_envio.url_pdf_cotizacion = path_cotizacion;
                var fs_cotizacion = new FileStream(filePath + path_cotizacion, FileMode.Open);
                adjuntos.Add(new Attachment(fs_cotizacion, "Cotización.pdf", "text/pdf"));
            }
            if (_pdf[2] != "")
            {
                _pdf_envio.url_pdf_checklist = path_checklist;
                var fs_checklist = new FileStream(filePath + path_checklist, FileMode.Open);
                adjuntos.Add(new Attachment(fs_checklist, "Reporte de Checklist.pdf", "text/pdf"));
            }

            List<MailAddress> mails = new List<MailAddress>();
            mails.Add(new MailAddress(item[0].mail));
            mails.Add(new MailAddress("info@miele.com"));

            var send_email = item[0].mail;

            if (item[0].contacto != "")
            {
                bool valida = IsValidEmail(item[0].contacto);
                if (valida)
                {
                    mails.Add(new MailAddress(item[0].contacto));
                }
            }
            _emailRepository.SendMultipleMails(mails, "Reporte Miele Servicio # " + id.ToString(), true, body, 1, adjuntos); //parametro 5 es id_app: 1 tickets, 2 partners

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
            //    To = { send_email },
            //    Subject = "Reporte Miele",
            //    IsBodyHtml = true,
            //    Body = body

            //})
            //    var smtp = new SmtpClient
            //    {
            //        Host = _emailSettings.PrimaryDomain,
            //        Port = _emailSettings.PrimaryPort,
            //        EnableSsl = _emailSettings.EnableSsl,
            //        DeliveryMethod = SmtpDeliveryMethod.Network,
            //        UseDefaultCredentials = false,
            //        Credentials = new NetworkCredential(_emailSettings.UsernameEmail, _emailSettings.UsernamePassword)
            //    };
            //using (var message = new MailMessage
            //{
            //    From = new MailAddress("no-reply@miele.com.mx", _emailSettings.MailName),
            //    To = { send_email },
            //    Subject = "Reporte Miele",
            //    IsBodyHtml = true,
            //    Body = body

            //})
            //{


            //    if (_pdf[3] == "1")
            //    {
            //        var fs = new FileStream(filePath + path, FileMode.Open);
            //        message.Attachments.Add(new Attachment(fs, "Reporte de Actividades.pdf", "text/pdf"));
            //    }
            //    if (_pdf[1] != "")
            //    {
            //        var fs_cotizacion = new FileStream(filePath + path_cotizacion, FileMode.Open);
            //        message.Attachments.Add(new Attachment(fs_cotizacion, "Cotización.pdf", "text/pdf"));
            //    }
            //    if (_pdf[2] != "")
            //    {
            //        var fs_checklist = new FileStream(filePath + path_checklist, FileMode.Open);
            //        message.Attachments.Add(new Attachment(fs_checklist, "Reporte de Checklist.pdf", "text/pdf"));
            //    }

            //    smtp.Send(message);
            //}

            //var fs = new FileStream(filePath + path, FileMode.Open);
            //    var fs_cotizacion = new FileStream(filePath + path_cotizacion, FileMode.Open);
            //    var fs_checklist = new FileStream(filePath + path_checklist, FileMode.Open);
            //    List<Attachment> adjuntos = new List<Attachment>();
            //    adjuntos.Add(new Attachment(fs, "Confirmación de orden de Servicio.pdf", "text/pdf"));

            //    adjuntos.Add(new Attachment(fs_cotizacion, "Cotización.pdf", "text/pdf"));
            //    adjuntos.Add(new Attachment(fs_checklist, "Reporte de Checklist.pdf", "text/pdf"));

            //    _emailRepository.SendAttachment(send_email, "Reporte Miele", true, body, adjuntos); // subject del correo

            _pdf_envio.url_ppdf_reporte = path;
            _context.Visita.Update(_pdf_envio);
            _context.SaveChanges();

            return Ok("Successfully created PDF document.");
            //return File(file, "application/pdf", "EmployeeReport.pdf"); //USE THIS RETURN STATEMENT TO DOWNLOAD GENERATED PDF DOCUMENT
            //return File(file, "application/pdf");

        }

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

        //[Route("emtest")]
        //[HttpPost("emtest/{email}")]
        //public IActionResult test_email(string email)
        //{
        //    bool respuesta = IsValidEmail(email);
        //    return new ObjectResult(respuesta);
        //}

        // POST: api/PDFCreator/visitas
        [Route("visitas")]
        [HttpPost("{visitas}")]
        public IActionResult Visita_Id([FromBody] Visita id)
        {
            var item = (from a in _context.Servicio
                        join b in _context.Cat_SubLinea_Producto on a.id_categoria_servicio equals b.id into _cat_servicio
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
                        where a.id == id.id_servicio
                        select new
                        {
                            id = a.id,
                            id_cliente = c.id,
                            cliente = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(c.nombre) + " " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(c.paterno) + " " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(c.materno),
                            cliente_telefono = c.telefono,
                            cliente_movil = c.telefono_movil,
                            categoria_servicio = (cat_servicio == null) ? "" : cat_servicio.descripcion,
                            distribuidor_autorizado = (fd == null) ? "" : fd.desc_distribuidor,
                            solicitado_por = h.desc_solicitado_por,
                            solicitud_via = hh.desc_solicitud_via,
                            id_tipo_servicio = j.id,
                            tipo_servicio = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(j.desc_tipo_servicio),
                            sub_tipo_servicio = sub.sub_desc_tipo_servicio,
                            id_estatus = fe == null ? 0 : fe.id,
                            estatus_servicio = (fe == null) ? "" : fe.desc_estatus_servicio,
                            contacto = a.contacto,
                            IBS = a.IBS,
                            actividades_realizar = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(a.descripcion_actividades),
                            a.descripcion_actividades,
                            piezas_cotizacion = (from y in _context.Piezas_Repuesto
                                                 join pc in _context.Rel_servicio_Refaccion on y.id_rel_servicio_refaccion equals pc.id
                                                 join u in _context.Cat_Materiales on y.id_material equals u.id
                                                 join pre in _context.Cat_Lista_Precios on u.id_grupo_precio equals Convert.ToInt32(pre.grupo_precio) //into g
                                                 where pc.id_vista == id.id
                                                 select new
                                                 {
                                                     id = y.id,
                                                     refaccion = u.descripcion,
                                                     cantidad = y.cantidad,
                                                     precio_sin_iva = pre.precio_sin_iva * y.cantidad,
                                                     total_cantidad = (from a in _context.Piezas_Repuesto
                                                                       join b in _context.Rel_servicio_Refaccion on a.id_rel_servicio_refaccion equals b.id
                                                                       where b.id_vista == id.id
                                                                       select a.cantidad).Sum(),
                                                     total_precio = (from a in _context.Piezas_Repuesto
                                                                     join aa in _context.Rel_servicio_Refaccion on a.id_rel_servicio_refaccion equals aa.id
                                                                     join b in _context.Cat_Materiales on a.id_material equals b.id
                                                                     join c in _context.Cat_Lista_Precios on b.id_grupo_precio equals Convert.ToInt32(c.grupo_precio)
                                                                     where aa.id_vista == id.id
                                                                     select c.precio_sin_iva).Sum()
                                                 }).ToList(),
                            visitas = (from x in _context.Visita
                                       join d in _context.Cat_Direccion on x.id_direccion equals d.id
                                       join e in _context.Cat_Estado on d.id_estado equals e.id
                                       join f in _context.Cat_Municipio on d.id_municipio equals f.id
                                       join g in _context.Cat_Localidad on d.id_localidad equals g.id
                                       join kk in _context.CatEstatus_Visita on x.estatus equals kk.id into t
                                       from fee in t.DefaultIfEmpty()
                                       where x.id_servicio == id.id_servicio && x.id == id.id
                                       select new
                                       {
                                           id = x.id,
                                           direccion = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(d.calle_numero + " " + d.numExt) + ", " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g.desc_localidad) + ", " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(e.desc_estado) + ", " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(f.desc_municipio),
                                           id_direccion = d.id,
                                           id_servicio = x.id_servicio,
                                           fecha_inicio_visita = Convert.ToDateTime(x.fecha_inicio_visita).ToString("dd/MM/yyyy"),
                                           x.imagen_firma,
                                           persona_recibe = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(x.persona_recibe),
                                           tecnico = (from visita_tecnico in _context.rel_tecnico_visita
                                                          //join tv in _context.rel_tecnico_visita on visita_tecnico.id equals tv.id_vista
                                                      join i in _context.Tecnicos on visita_tecnico.id_tecnico equals i.id
                                                      join j in _context.Users on i.id equals j.id
                                                      where visita_tecnico.id_vista == id.id && visita_tecnico.tecnico_responsable == true
                                                      select new
                                                      {
                                                          nombre = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(j.name) + " " + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(j.paterno) + " " + (j.materno != "" ? CultureInfo.CurrentCulture.TextInfo.ToTitleCase(j.materno) : ""),
                                                          visita_tecnico.tecnico_responsable
                                                      }).Distinct().ToList(),
                                           checklist = (from w in _context.Producto_Check_List_Respuestas
                                                        join c_p in _context.Cliente_Productos on w.id_producto equals c_p.Id
                                                        join prod in _context.Cat_Productos on c_p.Id_Producto equals prod.id
                                                        join visita in _context.Visita on w.id_vista equals visita.id
                                                        join ec in _context.Cat_Checklist_Producto on w.estatus equals ec.id
                                                        where visita.id == id.id && visita.id_servicio == id.id_servicio
                                                        select new
                                                        {
                                                            id_visita = visita.id,
                                                            id_estatus_prodcuto_checklist = w.estatus,
                                                            estatus_prodcuto_checklist = ec.desc_checklist_producto,
                                                            prod.nombre,
                                                            prod.modelo,
                                                            imagenes_checklist = (from xz in _context.Rel_Imagen_Producto_Visita
                                                                                  where xz.id_visita == id.id && xz.id_producto == w.id_producto && xz.checklist == true
                                                                                  select new
                                                                                  {
                                                                                      id = xz.id,
                                                                                      url = xz.path,
                                                                                      xz.actividad
                                                                                  }).ToList(),
                                                            preguntas = (from check in _context.Producto_Check_List_Respuestas
                                                                             //join prod in _context.Cat_Productos on check.id_producto equals prod.id
                                                                         join visita_c in _context.Visita on check.id_vista equals visita_c.id
                                                                         join respuesta in _context.Check_List_Respuestas on check.id equals respuesta.id_producto_check_list_respuestas
                                                                         join pregunta in _context.Check_List_Preguntas on respuesta.id_pregunta equals pregunta.id
                                                                         where visita_c.id == id.id && visita_c.id_servicio == id.id_servicio && check.id_producto == prod.id
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
                                                          productos = (from xe in _context.Rel_servicio_Refaccion
                                                                       join sev in _context.CatEstatus_Producto on xe.estatus equals sev.id
                                                                       join prod in _context.Cat_Productos on xe.id_producto equals prod.id
                                                                       join serie in _context.Rel_servicio_producto on prod.id equals serie.id_producto //&& serie.id_vista equals 
                                                                       where xe.id_vista == id.id && serie.id_vista == id.id
                                                                       select new
                                                                       {
                                                                           id = xe.id_producto,
                                                                           nombre_prodcuto = prod.nombre,
                                                                           estatus_producto = sev.desc_estatus_producto,
                                                                           id_estatus = xe.estatus,
                                                                           //estatus = sev.desc_estatus_producto_en,
                                                                           actividades = xe.actividades,
                                                                           fallas = xe.fallas,
                                                                           serie.no_serie,
                                                                           serie.garantia,
                                                                           prod.modelo,
                                                                           imagenes_reparacion = (from xz in _context.Rel_Imagen_Producto_Visita
                                                                                                  where xz.id_visita == id.id && xz.id_producto == xe.id_producto && xz.checklist == false
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
                                                                                                  u.no_material,
                                                                                                  refaccion = u.descripcion,
                                                                                                  cantidad = y.cantidad,
                                                                                                  precio = Math.Round(o.precio_sin_iva, 2)

                                                                                              }).ToList(),
                                                                           piezas_tecnico = (from y in _context.Piezas_Repuesto_Tecnico
                                                                                             join u in _context.Cat_Materiales on y.id_material equals u.id
                                                                                             where y.id_rel_servicio_refaccion == xe.id
                                                                                             select new
                                                                                             {
                                                                                                 id = y.id,
                                                                                                 refaccion = u.descripcion,
                                                                                                 cantidad = y.cantidad,
                                                                                                 u.no_material
                                                                                             }).ToList(),
                                                                           mano_de_obra = (from mo in _context.Rel_Categoria_Producto_Tipo_Producto
                                                                                           where mo.id_categoria == prod.id_sublinea && mo.id_tipo_servicio == xe.visita.servicio.id_tipo_servicio
                                                                                           select new
                                                                                           {
                                                                                               mo.precio_visita
                                                                                           }).Take(1).ToList()


                                                                       }).ToList()
                                                      }).ToList()
                                       }).Distinct().ToList()
                        }).Distinct().ToList();

            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }
    }
}
