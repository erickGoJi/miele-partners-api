using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.Service
{
    public class CotizacionPDF : ICotizacionPDF
    {
        private IConverter _converter;
        private readonly MieleContext _context;

        public CotizacionPDF(IConverter converter, MieleContext context)
        {
            _converter = converter;
            _context = context;
        }
        public string CrearDocumento(long id, bool cliente)
        {
            var selRuta = _context.parametro_Archivos.FirstOrDefault(p => p.id == 4);
            string path;
            if (selRuta == null) path = "/Imagenes/pdf_estimates/";
            else
            {
                path = selRuta.ruta;
            }

            StringBuilder estimateTemplate = new StringBuilder();
            StringBuilder headerTemplate = new StringBuilder();

            string filePath2 = Environment.CurrentDirectory;
            var estimateInfo = _context.Cotizaciones.Where(e => e.Id == id).FirstOrDefault();
            var clientInfo = _context.Clientes.Where(c => c.id == estimateInfo.Id_Cliente).FirstOrDefault();

            var shippingAddress = (from d in _context.direcciones_cotizacion
                                   join e in _context.Cat_Estado on d.id_estado equals e.id
                                   //join f in _context.Cat_Municipio on d.id_municipio equals f.id
                                   join g in _context.Cat_Localidad on d.id_localidad equals g.id
                                   where d.id_cotizacion == estimateInfo.Id && d.tipo_direccion == 2
                                   select new
                                   {
                                       d.cp,
                                       d.nombrecontacto,
                                       d.telefono,
                                       d.telefono_movil,
                                       d.calle_numero,
                                       d.numExt,
                                       d.numInt,
                                       d.Fecha_Estimada,
                                       g.desc_localidad,
                                       e.desc_estado,
                                       g.municipio.desc_municipio,
                                       id_direccion = d.id,
                                   }).FirstOrDefault();
            var installAddress = (from d in _context.direcciones_cotizacion
                                  join e in _context.Cat_Estado on d.id_estado equals e.id
                                  //join f in _context.Cat_Municipio on d.id_municipio equals f.id
                                  join g in _context.Cat_Localidad on d.id_localidad equals g.id
                                  where d.id_cotizacion == estimateInfo.Id && d.tipo_direccion == 1
                                  select new
                                  {
                                      d.cp,
                                      d.nombrecontacto,
                                      d.telefono,
                                      d.telefono_movil,
                                      d.calle_numero,
                                      d.numExt,
                                      d.numInt,
                                      d.Fecha_Estimada,
                                      g.desc_localidad,
                                      e.desc_estado,
                                      g.municipio.desc_municipio,
                                      id_direccion = d.id,
                                  }).FirstOrDefault();
            var cer_prods_rels = (from cp in _context.Cotizacion_Producto
                                  join p in _context.Cat_Productos on cp.Id_Producto equals p.id
                                  join slp in _context.producto_certificado_sublinea on p.id_sublinea equals slp.Id_sublinea
                                  where cp.Id_Cotizacion == id
                                  group slp by slp.id_producto into sq1
                                  select new
                                  {
                                      id_producto = sq1.First().id_producto,
                                      id_sublinea = sq1.Max().Id_sublinea
                                  });

            var estimate = (from c in _context.Cotizaciones
                            join ce in _context.Cat_Estatus_Cotizacion on c.Estatus equals ce.id
                            join usr in _context.Users on c.Id_Vendedor equals usr.id
                            where c.Id == id
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
                                cambio_ord_comp_generada = c.cambio_ord_comp_generada,

                                //////////////////////////////////////MONTOS
                                ///
                                importe_precio_lista = c.importe_precio_lista,
                                iva_precio_lista = c.iva_precio_lista,
                                importe_condiciones_com = c.importe_condiciones_com,
                                iva_condiciones_com = c.iva_condiciones_com,
                                importe_promociones = c.importe_promociones,
                                iva_promociones = c.iva_promociones,
                                descuento_acumulado = c.descuento_acumulado,
                                descuento_acumulado_cond_com = c.descuento_acumulado_cond_com,
                                motivo_rechazo = c.motivo_rechazo,
                                rechazada = c.rechazada,
                                requiere_fact = c.requiere_fact,
                                id_cotizacion_padre = c.id_cotizacion_padre,
                                //cambio orden pot ripo
                                productos = (from cp in _context.Cotizacion_Producto
                                             join a in _context.Cat_Productos on cp.Id_Producto equals a.id
                                             join sq1 in cer_prods_rels on cp.Id_Producto equals sq1.id_producto into sq2
                                             orderby a.tipo
                                             where cp.Id_Cotizacion == id
                                             from s2 in sq2.DefaultIfEmpty()
                                             select new
                                             {
                                                 id_superlinea_orden = _context.Cat_SubLinea_Producto.FirstOrDefault(ss => ss.id == (s2.id_sublinea > 0 ? s2.id_sublinea : a.id_sublinea)).cat_linea_producto.id_superlinea,
                                                 nombre_linea_orden = _context.Cat_SubLinea_Producto.FirstOrDefault(ss => ss.id == (s2.id_sublinea > 0 ? s2.id_sublinea : a.id_sublinea)).cat_linea_producto.descripcion,

                                                 id = a.id,
                                                 sku = a.sku,
                                                 modelo = a.modelo,
                                                 nombre = a.nombre,
                                                 descripcion_corta = a.descripcion_corta,
                                                 cantidad = cp.cantidad,
                                                 margen_cc = cp.margen_cc,
                                                 cp.precio_lista,
                                                 cp.iva_precio_lista,
                                                 cp.precio_descuento,
                                                 cp.iva_precio_descuento,
                                                 importe_precio_lista = cp.precio_lista + cp.iva_precio_lista,
                                                 importe_total_bruto = (cp.precio_lista + cp.iva_precio_lista) * cp.cantidad,
                                                 importe_condiciones_com = (cp.precio_condiciones_com + cp.iva_cond_comerciales) * cp.cantidad,
                                                 importe_con_descuento = (cp.precio_descuento + cp.iva_precio_descuento),
                                                 descuento = (cp.precio_lista + cp.iva_precio_lista) - (cp.precio_descuento + cp.iva_precio_descuento),
                                                 descuento_cc = (cp.precio_lista + cp.iva_precio_lista) - (cp.precio_condiciones_com + cp.iva_cond_comerciales),
                                                 importetotal = (cp.precio_descuento + cp.iva_precio_descuento) * cp.cantidad,
                                                 importetotal_cc = (cp.precio_condiciones_com + cp.iva_cond_comerciales) * cp.cantidad,

                                                 cat_imagenes_producto = (from x in _context.Cat_Imagenes_Producto
                                                                          where x.id_producto == a.id
                                                                          select x)
                                                                              .Select(d => new Cat_Imagenes_Producto
                                                                              {
                                                                                  id = d.id,
                                                                                  id_producto = d.id_producto,
                                                                                  url = d.url
                                                                              }).ToList()

                                             }).OrderBy(a => a.id_superlinea_orden).ThenBy(d => d.nombre_linea_orden).ToList(),
                            }).FirstOrDefault();


            var suc = _context.Cat_Sucursales.FirstOrDefault(su => su.Id == estimateInfo.Id_sucursal);
            var billingAddressSuc = _context.DatosFiscales_Sucursales.Where(dfs => dfs.id_Sucursal == estimateInfo.Id_sucursal)
                .Join(_context.Cat_Municipio.Include(mu => mu.estado), df => df.id_municipio, mu => mu.id,
                (df, mu) => new
                {
                    razon_social = df.razon_social,
                    rfc = df.rfc,
                    correo = df.email,
                    calle_numero = df.calle_numero,
                    exterior = df.Ext_fact,
                    interior = df.Int_fact,
                    colonia = df.colonia,
                    cp = df.cp,
                    nombre_municipio = mu.desc_municipio,
                    nombre_estado = mu.estado.desc_estado,
                }).FirstOrDefault();
            var billingAddressCte = _context.DatosFiscales.Where(dc => dc.id_cliente == estimateInfo.Id_Cliente)
                .Join(_context.Cat_Municipio.Include(mu => mu.estado), dfc => dfc.id_municipio, mu => mu.id,
                (dfc, mu) => new
                {
                    razon_social = dfc.razon_social,
                    rfc = dfc.rfc,
                    correo = dfc.email,
                    calle_numero = dfc.calle_numero,
                    exterior = dfc.Ext_fact,
                    interior = dfc.Int_fact,
                    colonia = dfc.colonia,
                    cp = dfc.cp,
                    nombre_municipio = mu.desc_municipio,
                    nombre_estado = mu.estado.desc_estado,
                }).FirstOrDefault();

            var dirSuc = _context.cat_direccion_sucursales.Where(ds => ds.id_sucursales == estimateInfo.Id_sucursal && ds.tipo_direccion == 1)
                .Join(_context.Cat_Localidad.Include(lo => lo.municipio).ThenInclude(mu => mu.estado), dis => dis.id_localidad, es => es.id,
               (dis, es) => new
               {
                   calle_numero = dis.calle_numero + " " + dis.numExt + (String.IsNullOrEmpty(dis.numInt) ? "" : dis.numInt),
                   colonia = es.desc_localidad,
                   cp = dis.cp,
                   nombre_municipio = es.municipio.desc_municipio,
                   nombre_estado = es.municipio.estado.desc_estado,
                   telefono = dis.telefono,
               }).FirstOrDefault();


            var fuente = _context.parametro_Archivos.FirstOrDefault(a => a.id == 5);

            string letra = fuente.funcion != null ? fuente.funcion : "Arial";


            headerTemplate.Append(@"<img heigth = '120px' width = '250px' src = 'https://www.miele.com.mx/media/common/img/Miele_Logo.svg'>");

            estimateTemplate.Append(@"<html>
                                        <head align='center'>
                                            <link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css' integrity='sha384-BVYiiSIFeK1dGmJRAkycuHAHRg32OmUcww7on3RYdg4Va+PmSTsz/K68vbdEjh4u' crossorigin='anonymous'>
                                        </head>
                                        <body style='font-family:Arial'>
<!-- Tabla 1, datos de sucursal, datos del cliente y datos de la cotizacion -->
                                                        <table style='height:140px; width:100%; text-align:left; font-size:12px; font-family:" + letra + "'>");
            if (estimateInfo.Id_Canal == 2)
                //Agreagar imagen en blanco
                estimateTemplate.Append("<tr><td width='30%' valign='top'><img heigth='90' width='190' src=''>");
            else
                estimateTemplate.Append("<tr><td width='30%' valign='top'><img heigth='90' width='190' src='" + suc.url_logo + @"'>
                                        <br><h><strong>" + suc.Sucursal + @"</strong></h>");
            if (dirSuc != null)
            {
                estimateTemplate.Append(@"<br>" + dirSuc.calle_numero + @",
                                                <br>" + dirSuc.colonia + ", " + dirSuc.cp + @",
                                                <br>" + dirSuc.nombre_municipio + ", " + dirSuc.nombre_estado + @"
                                                <br>" + dirSuc.telefono);
            }
            if (estimateInfo.Estatus == 1)
            {
                estimateTemplate.Append(@"</td>
                                            <td valign='top' style='padding-left:20px; text-align:left;' width='40%'>
                                                <h4><b>Datos del cliente:</b></h4>
                                                <h><strong>Nombre: </strong>" + clientInfo.nombre + @"</h><br />
                                                <h><strong>Apellidos: </strong>" + clientInfo.paterno + (String.IsNullOrEmpty(clientInfo.materno) ? "" : " " + clientInfo.materno) + @" </h><br />
                                                <h><strong>Correo: </strong>" + clientInfo.email + @" </h><br />
                                                <h><strong>Teléfono fijo: </strong>" + clientInfo.telefono + @" </h><br />
                                                <h><strong>Teléfono Cel: </strong>" + clientInfo.telefono_movil + @" </h><br />
                                            </td>");
            }
            estimateTemplate.Append(@"<td width='30%' style='text-align: right;' valign='top'>
                            <table width='100%' style='text-align: right; font-size:15px;' height='100%'>
                                <tr height='80%'>
                                    <td style='text-align:right; vertical-align:top' width='100%'>");
            if (estimateInfo.ibs != null)
            {
                estimateTemplate.Append(@"<strong>IBS: <span style = 'color:#A5000D'>" + estimateInfo.ibs + @"</span></strong>");
            }
            string tipoEstatus;
            bool bandera = false;
            switch (estimateInfo.Estatus)
            {
                case 0:
                    tipoEstatus = "Cotización";
                    bandera = true;
                    break;
                case 1:
                    tipoEstatus = "Cotización";
                    bandera = true;
                    break;
                case 2:
                    tipoEstatus = "Órden de compra";
                    bandera = true;
                    break;
                case 3:
                    tipoEstatus = "Órden de compra";
                    bandera = false;
                    break;
                case 4:
                    tipoEstatus = "Órden de venta";
                    bandera = false;
                    break;
                case 5:
                    tipoEstatus = "Órden de venta";
                    bandera = false;
                    break;
                default:
                    tipoEstatus = "Cotización";
                    bandera = false;
                    break;
            }
            estimateTemplate.Append(@"<br><strong>" + tipoEstatus + ":<span style='color:#A5000D'> " + estimateInfo.Numero + (bandera ? "*" : "") + @"</span></strong>");
            estimateTemplate.Append(@"<br><span> Fecha: " + estimateInfo.fecha_cotiza.ToString("dd/MM/yyyy") + @"</span>
                                            </td>
                                        </tr>");
            var vendedor = _context.Users.FirstOrDefault(v => v.id == estimateInfo.Id_Vendedor);
            if (vendedor != null)
            {
                estimateTemplate.Append("<tr><td><strong>Vendedor: </strong>" + vendedor.name + " " + vendedor.paterno + @"</td></tr>");
            }
            estimateTemplate.Append(@"</table>
                                            </td>            
                                          </tr>
                                        </table>");
            int prodpag1 = 11;
            bool banderaDirecciones = false;
            //Datos de facturacion del cliente y direcciones de envio e instalacion
            if (estimateInfo.Estatus > 1 && ((billingAddressCte != null || billingAddressSuc != null) || (installAddress != null || shippingAddress != null)))
            {
                estimateTemplate.Append(@"<table width='100%' style='font-size:12px; font-family:" + letra + @"; height:222px'>
                                            <tr>");
                banderaDirecciones = true;
            }
            string anchoCols = "32";
            //Datos de facturacion verificar alto de tabla
            if (estimateInfo.Id_Canal == 2 && billingAddressCte != null && estimateInfo.Estatus > 1)
            {
                prodpag1 -= 3;
                estimateTemplate.Append(@"<td width='32%' valign='top' style='border: 1px solid #000;'>
                                            <table style='width:100%; font-size:12px; font-family:" + letra + @";'>
                                                <tr bgcolor='#D3D3D3' style='text-align:center'>    
                                                    <td colspan='2' style='height:30px;'>    
                                                        <h5><b>Datos de facturación</b></h5>
                                                    </td>    
                                                </tr>    
                                                <tr>
                                                    <td style='width:32%; padding-left:5px; padding-top:15px; line-height:13px; vertical-align:top;'><strong>Razón Social: </strong></td>
                                                    <td style='width:68%;padding-left:2px; padding-top:15px; padding-right:5px;'>" + billingAddressCte.razon_social + @"</td>
                                                </tr>
                                                <tr>
                                                    <td style='padding-left:5px; line-height:16px; '><strong>RFC: </strong></td>
                                                    <td style='padding-left:2px; padding-right:5px;'>" + billingAddressCte.rfc + @"</td>
                                                </tr>
                                                <tr>
                                                    <td style='padding-left:5px; line-height:16px; vertical-align:top;'><strong>Correo: </strong></td>
                                                    <td style='padding-left:2px; padding-right:5px;'>" + billingAddressCte.correo + @"</td>
                                                </tr>
                                                <tr>
                                                    <td style='padding-left:5px; line-height:16px;'><strong>Dirección: </strong></td>
                                                    <td>
                                                    <table>
                                                        <tr>
                                                            <td style='padding-left:16px; padding-left:2px; width:100%; font-size:12px; font-family:" + letra + @"; line-height:16px;'>
                                                                <h>" + billingAddressCte.calle_numero + "," + billingAddressCte.exterior + (String.IsNullOrEmpty(billingAddressCte.interior) ? "" : ", " + billingAddressCte.interior) + @" </h><br />
                                                                <h>" + billingAddressCte.colonia + @"</h>
                                                                <h>, " + billingAddressCte.cp + @"</h><br />
                                                                <h>" + billingAddressCte.nombre_municipio + @" </h><br />
                                                                <h>" + billingAddressCte.nombre_estado + @" </h><br />
                                                                <h>México</h><br />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                  </td>
                                              </tr>
                                          </table>    
                                       </td><td width='2%'></td>");
            }
            else if (estimateInfo.Id_Canal != 2 && billingAddressSuc != null && estimateInfo.Estatus > 1)
            {
                prodpag1 -= 3;
                estimateTemplate.Append(@"<td width='32%' valign='top' style='border: 1px solid #000;'>
                                            <table style='width:100%; font-size:12px; font-family:" + letra + @";'>
                                                <tr bgcolor='#D3D3D3' style='text-align:center'>    
                                                    <td colspan='2' style='height:30px;'>    
                                                        <h5><b>Datos de facturación</b></h5>
                                                    </td>    
                                                </tr>    
                                                <tr>
                                                    <td style='width:32%; padding-left:5px; padding-top:15px; line-height:13px; vertical-align:top;'><strong>Razón Social: </strong></td>
                                                    <td style='width:68%;padding-left:2px; padding-top:15px; padding-right:5px;'>" + billingAddressSuc.razon_social + @"</td>
                                                </tr>
                                                <tr>
                                                    <td style='padding-left:5px; line-height:16px; '><strong>RFC: </strong></td>
                                                    <td style='padding-left:2px; padding-right:5px;'>" + billingAddressSuc.rfc + @"</td>
                                                </tr>
                                                <tr>
                                                    <td style='padding-left:5px; line-height:16px; vertical-align:top;'><strong>Correo: </strong></td>
                                                    <td style='padding-left:2px; padding-right:5px;'>" + billingAddressSuc.correo + @"</td>
                                                </tr>
                                                <tr>
                                                    <td style='padding-left:5px; line-height:16px; vertical-align:top;'><strong>Dirección: </strong></td>
                                                    <td>
                                                      <table>
                                                        <tr>
                                                            <td style='width:100%; padding-left:2px; font-size:12px; font-family:" + letra + @"; line-height:16px;'>
                                                                <h>" + billingAddressSuc.calle_numero + ", " + billingAddressSuc.exterior + (String.IsNullOrEmpty(billingAddressSuc.interior) ? "" : ", " + billingAddressSuc.interior) + @"</h><br />
                                                                <h>" + billingAddressSuc.colonia + ", " + billingAddressSuc.cp + @"</h><br />
                                                                <h>" + billingAddressSuc.nombre_municipio + @" </h><br />
                                                                <h>" + billingAddressSuc.nombre_estado + @" </h><br />
                                                                <h>México</h><br />
                                                            </td>
                                                        </tr>
                                                      </table>
                                                  </td>
                                              </tr>
                                          </table>
                                          </td><td width='2%'></td>");
            }
            else
            {
                if (banderaDirecciones)
                {
                    prodpag1 -= 3;
                    //estimateTemplate.Append(@"<td width='32%' valign='top' style='border: 1px solid #000; padding-left:5px; padding-right:5px'>
                    //                                <h4>Datos de facturación Cliente</h4>
                    //                                <h>Información no proporcionada </h><br />
                    //                          </td><td width='2%'></td>");
                    anchoCols = "49";
                }
            }

            //Direccion de instalacion
            if (installAddress != null && estimateInfo.Estatus > 1)
            {
                DateTime fec_est_ins = DateTime.Parse(installAddress.Fecha_Estimada, CultureInfo.GetCultureInfo("en-US").DateTimeFormat);
                estimateTemplate.Append(@"<td width='" + anchoCols + @"%' valign='top' style='border: 1px solid #000;'>
                                            <table style='width:100%; font-size:12px; font-family:" + letra + @";'>
                                                <tr bgcolor='#D3D3D3' style='text-align:center'>    
                                                    <td colspan='2' style='height:30px;'>    
                                                        <h5><b>Datos de instalación</b></h5>
                                                    </td>    
                                                </tr>    
                                                <tr>
                                                    <td style='width:25%; padding-left:5px; padding-top:15px; line-height:13px; vertical-align:top;'><strong>Contacto: </strong></td>
                                                    <td style='width:75%;padding-left:5px; padding-top:15px; padding-right:5px;'>" + installAddress.nombrecontacto + @"</td>
                                                </tr>
                                                <tr>
                                                    <td style='padding-left:5px; line-height:16px; '><strong>Telefono: </strong></td>
                                                    <td style='padding-left:5px; padding-right:5px;'>" + installAddress.telefono + @"</td>
                                                </tr>
                                                <tr>
                                                    <td style='padding-left:5px; line-height:16px; vertical-align:top;'><strong>Movil: </strong></td>
                                                    <td style='padding-left:5px; padding-right:5px;'>" + installAddress.telefono_movil + @"</td>
                                                </tr>
                                                <tr>
                                                    <td style='padding-left:5px; line-height:16px; vertical-align:top;'><strong>Fecha: </strong></td>
                                                    <td style='padding-left:5px; padding-right:5px;'>" + fec_est_ins.ToString("dd/MM/yyyy") + @"</td>
                                                </tr>
                                                <tr>
                                                    <td style='padding-left:5px; line-height:16px; vertical-align:top;'><strong>Dirección: </strong></td>
                                                    <td>
                                                      <table style=''>
                                                        <tr>
                                                            <td style='width:100%; padding-left:5px; font-size:12px; font-family:" + letra + @"; line-height:16px;'>
                                                                <h>" + installAddress.calle_numero + " " + installAddress.numExt + (String.IsNullOrEmpty(installAddress.numInt) ? "" : ", " + installAddress.numInt) + @" </h><br />
                                                                <h>" + installAddress.desc_localidad + ", " + installAddress.cp + @"</h><br />
                                                                <h>" + installAddress.desc_municipio + @" </h><br />
                                                                <h>" + installAddress.desc_estado + @" </h><br />
                                                                <h>México</h><br />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                  </td>
                                              </tr>
                                          </table>    
                                            </td><td width='2%'></td>");
            }
            else
            {
                if (banderaDirecciones)
                    //estimateTemplate.Append("<td style='color:white;' width='34%' valign='top'>sin direccion de instalacion</td>");
                    estimateTemplate.Append(@"<td width='" + anchoCols + @"%' valign='top' style='border: 1px solid #000; padding-left:5px; padding-right:5px'>
                                                <h4> Dirección de instalación</h4>
                                                    <h>Información no proporcionada </h><br />
                                               </td><td width='2%'></td>");
            }
            //Datos de envío
            if (shippingAddress != null && estimateInfo.Estatus > 1)
            {
                DateTime fec_est_env = DateTime.Parse(shippingAddress.Fecha_Estimada, CultureInfo.GetCultureInfo("en-US").DateTimeFormat);
                estimateTemplate.Append(@"<td width:'" + anchoCols + @"%' valign='top' style='border: 1px solid #000;'>
                                            <table style='width:100%; font-size:12px; font-family:" + letra + @";'>
                                                <tr bgcolor='#D3D3D3' style='text-align:center'>    
                                                    <td colspan='2' style='height:30px;'>    
                                                        <h5><b>Datos de envío</b></h5>
                                                    </td>    
                                                </tr>    
                                                <tr>
                                                    <td style='width:25%; padding-left:5px; padding-top:15px; line-height:13px; vertical-align:top;'><strong>Contacto: </strong></td>
                                                    <td style='width:75%;padding-left:5px; padding-top:15px; padding-right:5px;'>" + shippingAddress.nombrecontacto + @"</td>
                                                </tr>
                                                <tr>
                                                    <td style='padding-left:5px; line-height:16px; '><strong>Telefono: </strong></td>
                                                    <td style='padding-left:5px; padding-right:5px;'>" + shippingAddress.telefono + @"</td>
                                                </tr>
                                                <tr>
                                                    <td style='padding-left:5px; line-height:16px; vertical-align:top;'><strong>Movil: </strong></td>
                                                    <td style='padding-left:5px; padding-right:5px;'>" + shippingAddress.telefono_movil + @"</td>
                                                </tr>
                                                <tr>
                                                    <td style='padding-left:5px; line-height:16px; vertical-align:top;'><strong>Fecha: </strong></td>
                                                    <td style='padding-left:5px; padding-right:5px;'>" + fec_est_env.ToString("dd/MM/yyyy") + @"</td>
                                                </tr>
                                                <tr>
                                                   <td style='padding-left:5px; line-height:16px; vertical-align:top;'><strong>Dirección: </strong></td>
                                                   <td>
                                                    <table style=''>
                                                        <tr>
                                                            <td style='width:100%; padding-left:5px; font-size:12px; font-family:" + letra + @"; line-height:16px;'>
                                                                <h>" + shippingAddress.calle_numero + " " + shippingAddress.numExt + (String.IsNullOrEmpty(shippingAddress.numInt) ? "" : ", " + shippingAddress.numInt) + @" </h><br />
                                                                <h>" + shippingAddress.desc_localidad + @", " + shippingAddress.cp + @"</h><br />
                                                                <h>" + shippingAddress.desc_municipio + @" </h><br />
                                                                <h>" + shippingAddress.desc_estado + @" </h><br />
                                                                <h>México</h><br />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                  </td>
                                              </tr>
                                          </table>
                                       </td>");
            }
            else
            {
                if (banderaDirecciones)
                    //estimateTemplate.Append("<td style='color:white;' width='34%' valign='top'>sin direccion de envío</td>");
                    estimateTemplate.Append(@"<td width='" + anchoCols + @"%' valign='top' style='border: 1px solid #000; padding-left:5px; padding-right:5px'>
                                                <h4> Dirección de instalación:</h4>
                                                    <h>Información no proporcionada </h><br />
                                               </td>");
            }
            //Cerrar la tabla en las mismas condiciones en que se inicia arriba
            if (banderaDirecciones)
            {
                estimateTemplate.Append(@"
                                </tr>
                             </table>");

            }

            estimateTemplate.Append(@"<p style='height:2px'></p>
            <!-- Termina tabla de la primera parte -->");
            //estimates.products.Count
            int productos = estimate.productos.Count;
            //Productos por pagina
            int prodsxPag = 13;
            //Productos en pgina 1
            //int prodpag1 = 9;
            //paginas adicionales a la primera
            int pagres = (productos - prodpag1) / prodsxPag; // 22 - 6 = 18 / 20 = 0
            //se obtiene el residuo para saber si hay una pagina adicional
            int res = (productos - prodpag1) % prodsxPag;   // 22-6=18/20 = 18 residuo
            //Numero depaginas reales que tendra el reporte con productos
            int pags = (productos < prodpag1 ? 1 : 1 + pagres + (res > 0 ? 1 : 0)); //22 < 6? No, entonces 1+ 0 (18 > 0 ?, si entonces 1) = 2
            int ini = 0;
            int fin = 0;
            bool mismaHoja = false;
            //ciclo de paginas
            for (int i = 1; i <= pags; i++)
            {
                ini = (i == 1 ? 0 : prodpag1 + prodsxPag * (i - 2));
                fin = (i == pags ? productos : prodpag1 + prodsxPag * (i - 1));
                Console.WriteLine("pagina " + i + " inicio: " + ini + " fin:" + fin);

                //Se requiere que tenga el equivalente a tres productos para que las observaciones quepan en la misma hora
                int minimoP = (pags == 1 ? prodpag1 - 2 : prodsxPag - 2);
                //Valida si es la ultima hoja y si caben las observaciones
                if (i == pags && (fin - ini) <= minimoP && !String.IsNullOrEmpty(estimate.Observaciones))
                {
                    decimal alto = Math.Round(Convert.ToDecimal(100 / ((pags == 1 ? 2 : pags) - 1)));
                    string altura = (pags == 1 ? (banderaDirecciones ? "63" : "84") : alto.ToString());
                    estimateTemplate.Append(@"<table style='vertical-align:top; text-align: left; width:100%; position:relative; height:" + altura + @"%!important'>
                                                <tr style='width:100%'>
                                                  <td valign='top'>");

                    mismaHoja = true;
                }
                estimateTemplate.Append(@"<table style='text-align: left; width: 100%; border: 1px solid #000; font-size:12px; font-family:" + letra + @";'>
                                            <tr bgcolor='#D3D3D3' style='height:45px;'>
                                                <td style='width: 11%;'><h5><b> </b></h5></td>
                                                <td style='text-align: center; width: 8%;'><h5><b>SKU</b></h5></td>
                                                <td style='text-align: center; width: 8%;'><h5><b>Modelo</b></h5></td>
                                                <td style='text-align: center; width: 22%;'><h5><b>Producto</b></h5></td>
                                                <td style='text-align: center; width: 7%;'><h5><b>Cantidad</b></h5></td>
                                                <td style='text-align: center; width: 11%;'><h5><b>Precio <p>Unitario</p></b></h5></td>
                                                <td style='text-align: center; width: 11%;'><h5><b>Monto</b></h5></td>");
                //Validacion: si es venta directa o cotizacion se muestran descuentos al cliente final, delo contrario se muestran los descuentos al intermediario KS y DS
                if (estimateInfo.Id_Canal == 2 || estimateInfo.Estatus == 1 || cliente)
                {
                    estimateTemplate.Append(@"<td style='text-align:center; width: 11%;'><h5><b>Descuento</b></h5></td>");
                }
                else
                {
                    estimateTemplate.Append(@"<td style='text-align:center; width: 11%;'><h5><b>Descuento Comercial</b></h5></td>");
                }

                estimateTemplate.Append(@"<td style='text-align: center; width: 11%;'><h5><b>Total <p>con IVA</p></b></h5></td>
                                            </tr>");
                //ciclo de productos
                for (int j = ini; j < fin; j++)
                {
                    if (estimate.productos[j].cat_imagenes_producto.Count == 0)
                    {
                        Cat_Imagenes_Producto cip = new Cat_Imagenes_Producto();
                        cip.url = selRuta.funcion + "img_prod_no_dips.png";
                        cip.id = 0;
                        cip.id_producto = estimate.productos[j].id;
                        estimate.productos[j].cat_imagenes_producto.Add(cip);
                        //product.cat_imagenes_producto[0].url = "../assets/img/img_prod_no_dips.png";
                    }


                    estimateTemplate.Append(@"<tr style='height:76px;'>
                                                <td style='max-width:100px; text-align:center;'><img height='75' style='padding: 1px;' src='" + estimate.productos[j].cat_imagenes_producto[0].url + @"'></td>        
                                                <td align='center' style='letter-spacing:0.6pt;'>" + estimate.productos[j].sku + @"</td>
                                                <td align='center' style='letter-spacing:0.6pt;'>" + estimate.productos[j].modelo + @"</td>
                                                <td style='padding-left:5px; letter-spacing:0.6pt;'>" + estimate.productos[j].nombre + @"</td>
                                                <td align='center'>" + estimate.productos[j].cantidad + @"</td>
                                                <td align='right' style='padding-right:17px; padding-left:7px;'><span style='float:left;'>$</span>" + largoMoneda(Convert.ToDouble(estimate.productos[j].importe_precio_lista).ToString("C", new CultureInfo("es-MX"))) + @" </td>
                                                <td align='right' style='padding-right:12px; padding-left:12px'><span style='float:left;'>$</span>" + largoMoneda((Convert.ToDouble(estimate.productos[j].importe_precio_lista) * estimate.productos[j].cantidad).ToString("C", new CultureInfo("es-MX"))) + @" </td>");
                    //Validacion: si es venta directa o cotizacion se muestran descuentos al cliente final, delo contrario se muestran los descuentos al intermediario KS y DS
                    if (estimateInfo.Id_Canal == 2 || estimateInfo.Estatus == 1 || cliente)
                    {
                        estimateTemplate.Append(@"<td align='right' style='padding-right:12px; padding-left:12px'><span style='float:left;'>$</span>" + largoMoneda((Convert.ToDouble(estimate.productos[j].descuento) * Convert.ToDouble(estimate.productos[j].cantidad)).ToString("C", new CultureInfo("es-MX"))) + @" </td>
                                                <td align='right' style='padding-right: 7px; padding-left:17px;'><span style='float:left;'>$</span>" + largoMoneda(Convert.ToDouble(estimate.productos[j].importetotal).ToString("C", new CultureInfo("es-MX"))) + @" </td>                    
                                            </tr>");
                    }
                    else
                    {
                        estimateTemplate.Append(@"<td align='right' style='padding-right:12px; padding-left:12px'><span style='float:left;'>$</span>" + largoMoneda((Convert.ToDouble(estimate.productos[j].descuento_cc) * Convert.ToDouble(estimate.productos[j].cantidad)).ToString("C", new CultureInfo("es-MX"))) + @" </td>
                                                <td align='right' style='padding-right: 7px; padding-left:17px;'><span style='float:left;'>$</span>" + largoMoneda(Convert.ToDouble(estimate.productos[j].importetotal_cc).ToString("C", new CultureInfo("es-MX"))) + @" </td>
                                            </tr>");
                    }
                }
                estimateTemplate.Append("</table>");
                if (i < pags)
                {
                    estimateTemplate.Append("<p style='page-break-after:always'></p>");
                }
            }

            if (!mismaHoja && !String.IsNullOrEmpty(estimate.Observaciones))
            {
                estimateTemplate.Append(@"<p style='page-break-after:always'></p>
                                            <table style='vertical-align:top; text-align: left; width:100%; position:relative; height:100%!important'>
                                                <tr style='width:100%'>
                                                  <td valign='top'>");
            }
            estimateTemplate.Append(@"<br />
                                        <table style='text-align: right; width: 100%; font-size:12px; font-family:" + letra + @";'>
                                        <tr style='vertical-align:top'>
                                            <td width='65%'>");
            //var discgroup1 = from discount in discounts
            //                 group discount by discount.offer into d
            //                 select new
            //                 {
            //                     id_promocion = d.First().id_promocion,
            //                     promo = d.First().offer, 
            //                     discSum = d.Sum(p => p.discount)
            //                 };
            //nuevo
            var offers = _context.Cotizacion_Promocion.Include(cp => cp.promocion).Where(cp => cp.id_cotizacion == estimateInfo.Id);
            var offer2 = from offer in offers
                         group offer by offer.id_promocion into cp
                         select new
                         {
                             id_promo = cp.First().id_promocion,
                             nombre_promo = cp.First().promocion.nombre
                         };

            int tiene_meses = 0;
            if (offer2.Count() != 0)
            {
                estimateTemplate.Append(@"<table style='text-align: left; width: 100%; font-size:12px; font-family:" + letra + @"'>
                                            <tr>
                                                <td>Promociones aplicadas:</td>
                                            </tr>");

                foreach (var discount in offer2)
                {
                    

                    string sBeneficios = beneficios_promo_cadena(discount.id_promo);
                    if (sBeneficios.Contains("Meses Sin"))
                    {
                        var arr = sBeneficios.Split(" ");
                        string num = arr[arr.Length - 2];
                        tiene_meses = Convert.ToInt32(num);
                        var tarjeta = _context.cat_Tarjetas.FirstOrDefault(t => t.id == estimateInfo.id_tarjeta);
                        sBeneficios += tarjeta != null ? " " + tarjeta.nombre : " ";
                    }
                    //sBeneficios = sBeneficios.Substring(0, sBeneficios.Length - 3);
                    estimateTemplate.Append(@"<tr>
                                                <td><strong>" + discount.nombre_promo + @"</strong> &nbsp; - " + sBeneficios + @"</td>
                                          </tr>");
                }

            }
            else
                estimateTemplate.Append(@"<table style='text-align: left; width: 100%;'>
                                            <tr>
                                                <td style='color:white'>Promociones aplicadas:</td>
                                            </tr>");

            
            double desc_sin_iva;
            double subtotal_sin_iva;
            double iva;
            double total_pagar;
            //Validacion: si es venta directa o cotizacion se muestran descuentos al cliente final, delo contrario se muestran los descuentos al intermediario KS y DS
            if (estimateInfo.Id_Canal == 2 || estimateInfo.Estatus == 1 || cliente)
            {
                desc_sin_iva = Convert.ToDouble(estimate.descuento_acumulado);
                subtotal_sin_iva = Convert.ToDouble(estimate.importe_precio_lista) - Convert.ToDouble(estimate.descuento_acumulado);
                iva = Convert.ToDouble(estimate.iva_promociones);
                total_pagar = (Convert.ToDouble(estimate.importe_promociones) + Convert.ToDouble(estimate.iva_promociones));
            }
            else
            {
                desc_sin_iva = Convert.ToDouble(estimate.descuento_acumulado_cond_com);
                subtotal_sin_iva = Convert.ToDouble(estimate.importe_precio_lista) - Convert.ToDouble(estimate.descuento_acumulado_cond_com);
                iva = Convert.ToDouble(estimate.iva_condiciones_com);
                double imp_cc = Convert.ToDouble(estimate.importe_condiciones_com);
                double iva_cc = Convert.ToDouble(estimate.iva_condiciones_com);
                total_pagar = (Convert.ToDouble(estimate.importe_condiciones_com) + Convert.ToDouble(estimate.iva_condiciones_com));
            }
            if (tiene_meses > 0)
            {
                //for (int i = 0; i < tiene_meses; i++)
                //{
                //    estimateTemplate.Append(@"<tr>
                //                                <td><strong> Pago " + (i + 1) + @": </strong> &nbsp; - " + (total_pagar/tiene_meses).ToString("C", new CultureInfo("es-MX")) + @"</td>
                //                          </tr>");
                //}
                estimateTemplate.Append(@"<tr>
                                                <td><strong> Promocion " + tiene_meses + @" Meses: </strong> &nbsp; - " + ((Convert.ToDouble(estimate.importe_precio_lista) - Convert.ToDouble(estimate.descuento_acumulado) + Convert.ToDouble(estimate.iva_promociones)) / tiene_meses).ToString("C", new CultureInfo("es-MX")) + @" mensuales</td>
                                          </tr>");
            }
            estimateTemplate.Append(@"</table>
                                    </td>
                                    <td width='35%'>
                                        <table style='text-align: right; width: 100%; font-size:13px'>
                                            <tr>
                                                <td style='width: 50%; height:26px; vertical-align:top;'><strong>Importe total</strong></td>
                                                <td style='width: 50%; height:26px; vertical-align:top; padding-right: 10px; padding-left: 25px;'><strong><span style='float:left;'>$</span>" + largoMoneda(total_pagar.ToString("C", new CultureInfo("es-MX"))) + @"</strong></td>
                                            </tr>");
            estimateTemplate.Append(@"<tr>
                                        <td style='width: 50%' >Importe de lista sin IVA</td>
                                        <td style='width: 50%; padding-right: 10px; padding-left: 25px;'><span style='float:left;'>$</span>" + largoMoneda(Convert.ToDouble(estimate.importe_precio_lista).ToString("C", new CultureInfo("es-MX"))) + @"</td>
                                      </tr>");
            estimateTemplate.Append(@"<tr style='border-bottom: 1px solid #000'>
                                        <td style='width: 50%;' >Descuento sin IVA</td>
                                        <td style='width: 50%; padding-right: 10px; padding-left: 25px;'><span style='float:left;'>$</span>" + largoMoneda(desc_sin_iva.ToString("C", new CultureInfo("es-MX"))) + @"</td>
                                      </tr>");
            estimateTemplate.Append(@"<tr>
                                        <td style='width: 50%' >Sub total sin IVA</td>
                                        <td style='width: 50%; padding-right: 10px; padding-left: 25px;'><span style='float:left;'>$</span>" + largoMoneda(subtotal_sin_iva.ToString("c", new CultureInfo("es-MX"))) + @"</td>
                                      </tr>");
            estimateTemplate.Append(@"<tr style='border-bottom: 1px solid #000;'>
                                        <td style='width: 50%;' >IVA</td>
                                        <td style='width: 50%; padding-right: 10px; padding-left: 25px;'><span style='float:left;'>$</span>" + largoMoneda(iva.ToString("C", new CultureInfo("es-MX"))) + @"</td>
                                      </tr>");
            estimateTemplate.Append(@"<tr>
                                        <td style='width: 50%'><strong>Total a pagar</strong></td>
                                        <td style='width: 50%; padding-right: 10px; padding-left: 25px;'><strong><span style='float:left;'>$</span> " + largoMoneda(total_pagar.ToString("C", new CultureInfo("es-MX"))) + @"</strong></td>
                                      </tr>
                                    </table>
                                  </td>
                                </tr>
                              </table>");
            if (!String.IsNullOrEmpty(estimate.Observaciones))
            {
                estimateTemplate.Append(@"</td>
                                            </tr>
                                            <tr>
                                                    <tr style='font-size:12px; font-family:" + letra + @"; text-align: left; line-height:20px; text-align:left; position:absolute; bottom:3px; width:100%'>
                                                        <td><strong style='text-align: left;'>Observaciones:  </strong>" + estimate.Observaciones + @"
                                                    </td>
                                                </tr>
                                            </table>");
            }
            estimateTemplate.Append(@"
                      </body>
                    </html>");

            try
            {
                //string servePath = Request.IsHttps ? "https://" : "http://" + Request.Host.ToUriComponent();
                
                string filePath = Environment.CurrentDirectory;
                Guid _guid = Guid.NewGuid();
                Guid _guid_cotizacion = Guid.NewGuid();
                path = path + _guid + ".pdf";
                GlobalSettings globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.Letter,
                    Margins = new MarginSettings { Top = 19, Bottom = 38, Left = 7, Right = 7 },
                    DocumentTitle = "Cotizacion " + id,
                    Out = filePath + path  //USE THIS PROPERTY TO SAVE PDF TO A PROVIDED LOCATION
                };
                //Modificar footer en base a bandera
                string urlFooter;
                if (bandera)
                    urlFooter = selRuta.funcion +  "FooterCotizacionAvisos.html";
                else
                    urlFooter = selRuta.funcion + "FooterCotizacion.html";

                ObjectSettings objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = estimateTemplate.ToString(),
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css") },
                    HeaderSettings = { FontName = letra, FontSize = 9, HtmUrl = selRuta.funcion + "EncabezadoCotizacion.html", Spacing = 0.1 },
                    FooterSettings = { HtmUrl = urlFooter, FontName = letra }
                };

                var pdf = new HtmlToPdfDocument()
                {

                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                _converter.Convert(pdf);

                var item =  _guid + ".pdf";

                if (item == null)
                {
                    return "vacio";
                }

                return item;
            }
            catch (Exception ex)
            {
                return "error:" + ex;
            }
        }



        public string beneficios_promo_cadena(int id_promo)
        {
            var descuento = (from b in _context.beneficio_desc
                             where b.id_promocion == id_promo
                             select new
                             {
                                 descripcion = (b.es_porcentaje ? " % " : " $ "),
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
                beneficios = beneficios + " | Meses Sin Int. " + msi[0].descripcion;
            if (regalos.Count > 0)
                beneficios = beneficios + " | Regalo " + regalos[0].descripcion;
            return beneficios;
        }


        public string largoMoneda(string monedatx)
        {
            int largo = monedatx.Length;
            return monedatx.Replace("$", "");
            //if (largo < 15)
            //{
            //    int espacios = 15 - largo;
            //    string esp = "";
            //    //for (int i = 0; i < espacios; i++)
            //    //{
            //    //    esp = esp + " ";
            //    //}
            //    nuevo = monedatx.Replace("$", esp);
            //}
            //else nuevo = monedatx;
            //return nuevo;
        }

    }
}
