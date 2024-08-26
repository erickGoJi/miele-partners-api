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

namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class Partners_ProductosController : Controller
    {
        private IConfiguration _config;
        private readonly MieleContext _context;

        public Partners_ProductosController(IConfiguration config, MieleContext context)
        {
            _config = config;
            _context = context;
        }

        // GET: api/Partners_Productos
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Partners_Productos/5
        [HttpGet("{id}", Name = "Get_ProductosLibre")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/Partners_Productos
        [Route("ProductosLibre")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult BuscarProductosLibre([FromBody]ModeloBusquedaCot busqueda)
        {
         
            IActionResult response = Unauthorized();
            
            var grp_productos = from prod in _context.Cat_Productos
                                join sl in _context.Cat_SubLinea_Producto on prod.id_sublinea equals sl.id
                                join l in _context.Cat_Linea_Producto on sl.id_linea_producto equals l.id
                                where prod.visible_partners == true
                                select new ResultadosBuscador
                                {
                                    resultado = "PRODUCTO: " + prod.nombre + " | SKU: " + prod.sku + " | MOD: " + prod.modelo + " | LÍNEA: " + l.descripcion ,
                                    //cantidad = 1,
                                    // tipo = "Producto",
                                    id = 0
                                };
            
            var grp_lineas = from prod in _context.Cat_Productos
                             join linea in _context.Cat_Linea_Producto on prod.id_linea equals linea.id
                             where prod.visible_partners == true
                             group linea by prod.id_linea into g
                                 select new ResultadosBuscador
                                 {
                                     resultado = " LÍNEA: " + g.First().descripcion + " (" + g.Count() + ")",
                                     cantidad = g.Count(),
                                     tipo = "Línea",
                                     id = 0,
                                 };

            var grp_sublineas = from prod in _context.Cat_Productos
                                join sublinea in _context.Cat_SubLinea_Producto on prod.id_sublinea equals sublinea.id
                                join l in _context.Cat_Linea_Producto on sublinea.id_linea_producto equals l.id
                                where prod.visible_partners == true
                                group sublinea by prod.id_sublinea into g
                                select new ResultadosBuscador
                                {
                                 resultado = "SUBLÍNEA: " + g.First().descripcion + " (" + g.Count() + ")" + " | LÍNEA: " + _context.Cat_Linea_Producto.FirstOrDefault(s => s.id == g.First().id_linea_producto).descripcion ,//.ToList().Select(d => new Cat_Linea_Producto { descripcion = d.descripcion })  ,
                                 cantidad = g.Count(),
                                 tipo = "Sublínea",
                                 id = 0,
                              
                             };

            //grp_productos = grp_productos.Union(grp_modelo);
            //grp_productos = grp_productos.Union(grp_sku);
            grp_lineas = grp_lineas.Union(grp_sublineas);
            grp_lineas = grp_lineas.Union(grp_productos);
            if (grp_productos == null)
            {
                return response = Ok(new { token = "Error", grp_lineas });
            }
            return response = Ok(new { token = "Success", grp_lineas });
        }


        // POST: api/Partners_Productos
        [Route("ProductosPopulares")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ProductosPopulares([FromBody]ModeloBusquedaCot busqueda)
        {
            IActionResult response = Unauthorized();
            string ixi = "";
            var item = (from a in _context.Cat_Productos
                        orderby a.nombre ascending
                        where a.tipo == 1
                        select new
                        {
                            id =  a.id,
                            sku =  a.sku,
                            modelo = a.modelo,
                            nombre =  a.nombre,
                            descripcion_corta =  a.descripcion_corta,
                            descripcion_larga = "<ul><li>" + a.descripcion_larga.Replace(",", "</li><li>") + "</ul>",
                            descripcion_largaR = a.descripcion_larga.Trim().Substring(0, 150),
                            atributos =  a.atributos,
                            precio_sin_iva =  a.precio_sin_iva,
                            precio_con_iva =  a.precio_con_iva,
                            id_categoria =  a.id_categoria,
                            id_linea =  a.linea,
                            id_sublinea =  a.id_sublinea,
                            ficha_tecnica =  a.ficha_tecnica,
                            estatus =  a.estatus,
                            cat_imagenes_producto = (from x in _context.Cat_Imagenes_Producto
                                                     where x.id_producto == a.id
                                                     select x)
                                                         .Select(d => new Cat_Imagenes_Producto
                                                         {
                                                             id = d.id,
                                                             id_producto = d.id_producto,
                                                             url = d.url
                                                         }).ToList()
                        }).ToList().Take(3);
            if (item == null)
            {
                return response = Ok(new { token = "Error", item });
            }
            return response = Ok(new { token = "Success", item });
        }


        // POST: api/Partners_Productos
        [Route("AccesoriosPopulares")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult AccesoriosPopulares([FromBody]ModeloBusquedaCot busqueda)
        {
            IActionResult response = Unauthorized();
            string ixi = "";
            var item = (from a in _context.Cat_Productos
                        orderby a.nombre ascending
                        where a.tipo == 2
                        select new
                        {
                            id = a.id,
                            sku = a.sku,
                            modelo = a.modelo,
                            nombre = a.nombre,
                            descripcion_corta = a.descripcion_corta,
                            descripcion_larga = "<ul><li>" + a.descripcion_larga.Replace(",", "</li><li>") + "</ul>",
                            descripcion_largaR = a.descripcion_larga.Trim().Substring(0, 150),
                            atributos = a.atributos,
                            precio_sin_iva = a.precio_sin_iva,
                            precio_con_iva = a.precio_con_iva,
                            id_categoria = a.id_categoria,
                            id_linea = a.linea,
                            id_sublinea = a.id_sublinea,
                            ficha_tecnica = a.ficha_tecnica,
                            estatus = a.estatus,
                            cat_imagenes_producto = (from x in _context.Cat_Imagenes_Producto
                                                     where x.id_producto == a.id
                                                     select x)
                                                         .Select(d => new Cat_Imagenes_Producto
                                                         {
                                                             id = d.id,
                                                             id_producto = d.id_producto,
                                                             url = d.url
                                                         }).ToList()
                        }).ToList().OrderByDescending(a => a.precio_con_iva).Take(4);
            if (item == null)
            {
                return response = Ok(new { token = "Error", item });
            }
            else
            {
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
            }
            return response = Ok(new { token = "Success", item });
        }

        // POST: api/Partners_Productos
        [Route("AccesoriosRecomendados")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult AccesoriosRecomendados([FromBody]ModBusPorId busqueda)
        {
            IActionResult response = Unauthorized();
            var id_pr = (from a in _context.productos_relacionados
                         where (a.id_producto == busqueda.Id) || (a.id_producto_2 == busqueda.Id)
                         select new
                         {
                             id_producto = (a.id_producto == busqueda.Id ? a.id_producto_2 : a.id_producto),
                         }).ToList();


            var item = (from a in _context.Cat_Productos
                        orderby a.nombre ascending
                        where a.tipo == 2 && id_pr.Any(prods => prods.id_producto == a.id)
                        select new
                        {
                            id = a.id,
                            sku = a.sku,
                            modelo = a.modelo,
                            nombre = a.nombre,
                            descripcion_corta = a.descripcion_corta,
                            descripcion_larga = "<ul><li>" + a.descripcion_larga.Replace(",", "</li><li>") + "</ul>",
                            descripcion_largaR = a.descripcion_larga.Trim().Substring(0, 150),
                            atributos = a.atributos,
                            precio_sin_iva = a.precio_sin_iva,
                            precio_con_iva = a.precio_con_iva,
                            id_categoria = a.id_categoria,
                            id_linea = a.linea,
                            id_sublinea = a.id_sublinea,
                            ficha_tecnica = a.ficha_tecnica,
                            estatus = a.estatus,
                            cat_imagenes_producto = (from x in _context.Cat_Imagenes_Producto
                                                     where x.id_producto == a.id
                                                     select x)
                                                         .Select(d => new Cat_Imagenes_Producto
                                                         {
                                                             id = d.id,
                                                             id_producto = d.id_producto,
                                                             url = d.url
                                                         }).ToList()
                        }).ToList().OrderByDescending(a => a.precio_con_iva).Take(4);

            if (item == null)
            {
                return response = Ok(new { token = "Error", item });
            }
            else
            {
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
            }
            return response = Ok(new { token = "Success", item });
        }

        [Route("ServiciosRelacionados")]
        [HttpPost]
        public IActionResult ServiciosRelacionados([FromBody]ModBusPorId busqueda )
        {
            IActionResult response = Unauthorized();
            try
            {
                var id_pr = (from a in _context.productos_relacionados
                             where (a.id_producto == busqueda.Id) || (a.id_producto_2 == busqueda.Id)
                             select new
                             {
                                 id_producto = (a.id_producto == busqueda.Id ? a.id_producto_2 : a.id_producto),
                             }).ToList();

                var tipo_prod = _context.Cat_Productos.FirstOrDefault(p => p.id == busqueda.Id);
                if (tipo_prod != null)
                {
                    if (tipo_prod.tipo == 5)
                    {
                        var prodsrel = (from a in _context.Cat_Productos
                                        orderby a.nombre ascending
                                        where a.tipo == 5 && id_pr.Any(prods => prods.id_producto == a.id)
                                        select new
                                        {
                                            id = a.id,
                                            sku = a.sku,
                                            modelo = a.modelo,
                                            nombre = a.nombre,
                                            descripcion_corta = a.descripcion_corta,
                                            descripcion_larga = "<ul><li>" + a.descripcion_larga.Replace(",", "</li><li>") + "</ul>",
                                            descripcion_largaR = a.descripcion_larga.Trim().Substring(0, 150),
                                            atributos = "<ul><li>" + a.atributos.Replace(",", "</li><li>") + "</ul>",
                                            precio_sin_iva = a.precio_sin_iva,
                                            precio_con_iva = a.precio_con_iva,
                                            id_categoria = a.id_categoria,
                                            id_linea = a.linea,
                                            id_sublinea = a.id_sublinea,
                                            ficha_tecnica = a.ficha_tecnica,
                                            estatus = a.estatus,

                                        }).ToList().Take(3);

                        return response = Ok(new { token = "Success", productos_all = prodsrel });
                    }
                    return response = Ok(new { token = "Error", detalle = "no es un servicio" });
                }
                return response = Ok(new { token = "Error", detalle = "no existe producto" });

            }
            catch (Exception ex)
            {
                return response = Ok(new { token = "Error", detalle = "error en servidor" + ex.Message });
            }
        }

        // POST: api/Partners_Productos
        [Route("ProductosRelacionadosAll")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ProductosRelacionadosAll([FromBody]ModBusPorId busqueda)
        {
            IActionResult response = Unauthorized();
            var id_pr = (from a in _context.productos_relacionados
                         where (a.id_producto == busqueda.Id) || (a.id_producto_2 == busqueda.Id)
                         select new
                         {
                             id_producto = (a.id_producto == busqueda.Id ? a.id_producto_2 : a.id_producto),
                         }).ToList();

            var productos = (from a in _context.Cat_Productos
                        orderby a.nombre ascending
                        where (a.tipo == 1 || a.tipo == 4) && id_pr.Any(prods => prods.id_producto == a.id)
                        select new
                        {
                            id = a.id,
                            sku = a.sku,
                            modelo = a.modelo,
                            nombre = a.nombre,
                            descripcion_corta = a.descripcion_corta,
                            descripcion_larga = "<ul><li>" + a.descripcion_larga.Replace(",", "</li><li>") + "</ul>",
                            descripcion_largaR = a.descripcion_larga.Trim().Substring(0, 150),
                            atributos = "<ul><li>" + a.atributos.Replace(",", "</li><li>") + "</ul>",
                            precio_sin_iva = a.precio_sin_iva,
                            precio_con_iva = a.precio_con_iva,
                            id_categoria = a.id_categoria,
                            id_linea = a.linea,
                            id_sublinea = a.id_sublinea,
                            ficha_tecnica = a.ficha_tecnica,
                            estatus = a.estatus,
                            url_guia = a.url_guia,
                            url_manual = a.url_manual,
                            url_impresion = a.url_impresion,
                            cat_imagenes_producto = (from x in _context.Cat_Imagenes_Producto
                                                     where x.id_producto == a.id
                                                     select x)
                                                         .Select(d => new Cat_Imagenes_Producto
                                                         {
                                                             id = d.id,
                                                             id_producto = d.id_producto,
                                                             url = d.url
                                                         }).ToList()
                        }).ToList().Take(3);

            foreach (var prod in productos)
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

            var accesorios = (from a in _context.Cat_Productos
                             orderby a.precio_con_iva descending
                             where a.tipo == 2 && id_pr.Any(prods => prods.id_producto == a.id)
                             select new
                             {
                                 id = a.id,
                                 sku = a.sku,
                                 modelo = a.modelo,
                                 nombre = a.nombre,
                                 descripcion_corta = a.descripcion_corta,
                                 descripcion_larga = "<ul><li>" + a.descripcion_larga.Replace(",", "</li><li>") + "</ul>",
                                 descripcion_largaR = a.descripcion_larga.Trim().Substring(0, 150),
                                 atributos = a.atributos,
                                 precio_sin_iva = a.precio_sin_iva,
                                 precio_con_iva = a.precio_con_iva,
                                 id_categoria = a.id_categoria,
                                 id_linea = a.linea,
                                 id_sublinea = a.id_sublinea,
                                 ficha_tecnica = a.ficha_tecnica,
                                 estatus = a.estatus,
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

            foreach (var prod in accesorios)
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

            var consumibles = (from a in _context.Cat_Productos
                              orderby a.precio_con_iva descending
                              where a.tipo == 3 && id_pr.Any(prods => prods.id_producto == a.id)
                              select new
                              {
                                  id = a.id,
                                  sku = a.sku,
                                  modelo = a.modelo,
                                  nombre = a.nombre,
                                  descripcion_corta = a.descripcion_corta,
                                  descripcion_larga = "<ul><li>" + a.descripcion_larga.Replace(",", "</li><li>") + "</ul>",
                                  descripcion_largaR = a.descripcion_larga.Trim().Substring(0, 150),
                                  atributos = a.atributos,
                                  precio_sin_iva = a.precio_sin_iva,
                                  precio_con_iva = a.precio_con_iva,
                                  id_categoria = a.id_categoria,
                                  id_linea = a.linea,
                                  id_sublinea = a.id_sublinea,
                                  ficha_tecnica = a.ficha_tecnica,
                                  estatus = a.estatus,
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

            foreach (var prod in consumibles)
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

            var productos_all = new List<IQueryable>()
            {
                productos.AsQueryable(),//0
               accesorios.AsQueryable(),//1
               consumibles.AsQueryable()//2
            };

            if (productos_all == null)
            {
                return response = Ok(new { token = "Error", productos_all });
            }
            else
            {
              return response = Ok(new { token = "Success", productos_all });
            }
            
        }

        // POST: api/Partners_Productos
        [Route("ProductosPopularesAll")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ProductosPopularesAll([FromBody]ModBusPorId busqueda)
        {
            IActionResult response = Unauthorized();
            var productos = (from a in _context.Cat_Productos
                             orderby a.nombre ascending
                             where a.tipo == 1
                             && a.id_caracteristica_base == 25
                             && a.id > 517
                             select new
                             {
                                 id = a.id,
                                 sku = a.sku,
                                 modelo = a.modelo,
                                 nombre = a.nombre,
                                 descripcion_corta = a.descripcion_corta,
                                 //descripcion_larga = a.descripcion_larga,
                                 descripcion_larga = "<ul><li>" + a.descripcion_larga.Replace(",", "</li><li>") + "</ul>",
                                 atributos = "<ul><li>" + a.atributos.Replace(",", "</li><li>") + "</ul>",
                                 precio_con_iva = a.precio_con_iva,
                                 id_caracteristica_base = a.id_caracteristica_base,
                                 url_guia = a.url_guia,
                                 url_manual = a.url_manual,
                                 url_impresion = a.url_impresion,
                                 cat_imagenes_producto = (from x in _context.Cat_Imagenes_Producto
                                                          where x.id_producto == a.id
                                                          select x)
                                                              .Select(d => new Cat_Imagenes_Producto
                                                              {
                                                                  id = d.id,
                                                                  url = d.url
                                                              }).ToList()
                             }).ToList().Take(3);

            var grp_lineas = from prod in _context.Cat_Productos
                             join linea in _context.Cat_Linea_Producto on prod.id_linea equals linea.id
                             where prod.visible_partners == true
                             group linea by prod.id_linea into g
                             select new ResultadosBuscador
                             {
                                 resultado = " LÍNEA: " + g.First().descripcion + " (" + g.Count() + ")",
                                 cantidad = g.Count(),
                                 tipo = "Línea",
                                 id = 0,
                             };

            //var ppp = from p in productos
            //          group p by p.id_caracteristica_base into g
            //          select new
            //          {
            //              id = g.First().id_caracteristica_base
            //          };

            
                             
            foreach (var prod in productos)
            {
                if (prod.cat_imagenes_producto.Count == 0)
                {
                    Cat_Imagenes_Producto cip = new Cat_Imagenes_Producto();
                    cip.url = "../assets/img/img_prod_no_dips.png";
                    cip.id = 0;
                    prod.cat_imagenes_producto.Add(cip);
                }
            }

            var productos1 = (from c in _context.caracteristicas_Bases
                              join p in (from p in productos
                                         group p by p.id_caracteristica_base into g
                                         select new
                                         {
                                             id = g.First().id_caracteristica_base
                                         }) on c.id equals p.id
                              orderby c.descripcion ascending
                              select new
                              {
                                  car_base = c.descripcion,
                                  productos = (from pr in productos
                                               where pr.id_caracteristica_base == c.id
                                               select pr).ToList()
                              }).ToList();

            var accesorios = (from a in _context.Cat_Productos
                              orderby a.nombre ascending
                              where a.tipo == 2 
                              select new
                              {
                                  id = a.id,
                                  sku = a.sku,
                                  modelo = a.modelo,
                                  nombre = a.nombre,
                                  descripcion_corta = a.descripcion_corta,
                                  descripcion_larga = "<ul><li>" + a.descripcion_larga.Replace(",", "</li><li>") + "</ul>",
                                  atributos = a.atributos,
                                  precio_con_iva = a.precio_con_iva,
                                  id_categoria = a.id_categoria,
                                  id_linea = a.linea,
                                  cat_imagenes_producto = (from x in _context.Cat_Imagenes_Producto
                                                           where x.id_producto == a.id
                                                           select x)
                                                               .Select(d => new Cat_Imagenes_Producto
                                                               {
                                                                   id = d.id,
                                                                   url = d.url
                                                               }).ToList()
                              }).ToList().OrderByDescending(a=> a.precio_con_iva).Take(4);

            foreach (var prod in accesorios)
            {
                if (prod.cat_imagenes_producto.Count == 0)
                {
                    Cat_Imagenes_Producto cip = new Cat_Imagenes_Producto();
                    cip.url = "../assets/img/img_prod_no_dips.png";
                    cip.id = 0;
                    prod.cat_imagenes_producto.Add(cip);
                }
            }

            var consumibles = (from a in _context.Cat_Productos
                               orderby a.nombre ascending
                               where a.tipo == 3 
                               select new
                               {
                                   id = a.id,
                                   sku = a.sku,
                                   modelo = a.modelo,
                                   nombre = a.nombre,
                                   descripcion_corta = a.descripcion_corta,
                                   descripcion_larga = "<ul><li>" + a.descripcion_larga.Replace(",", "</li><li>") + "</ul>",
                                   atributos = a.atributos,
                                   precio_con_iva = a.precio_con_iva,
                                   cat_imagenes_producto = (from x in _context.Cat_Imagenes_Producto
                                                            where x.id_producto == a.id
                                                            select x)
                                                                .Select(d => new Cat_Imagenes_Producto
                                                                {
                                                                    id = d.id,
                                                                    url = d.url
                                                                }).ToList()
                               }).ToList().OrderByDescending(a => a.precio_con_iva).Take(4);

            foreach (var prod in consumibles)
            {
                if (prod.cat_imagenes_producto.Count == 0)
                {
                    Cat_Imagenes_Producto cip = new Cat_Imagenes_Producto();
                    cip.url = "../assets/img/img_prod_no_dips.png";
                    cip.id = 0;
                    prod.cat_imagenes_producto.Add(cip);
                }
            }

            var productos_all = new List<IQueryable>()
            {
               productos.AsQueryable(),//0
               accesorios.AsQueryable(),//1
               consumibles.AsQueryable(),//2
               productos1.AsQueryable()
            };

            if (productos_all == null)
            {
                return response = Ok(new { token = "Error", productos_all });
            }
            else
            {
                return response = Ok(new { token = "Success", productos_all });
            }

        }

        [Route("ConsultaHPbyCp")]
        [HttpPost]
        public IActionResult BuscarHPbyCp([FromBody]ModeloBusquedaCot item)
        {
            IActionResult response = Unauthorized();
            var cp_ = Convert.ToInt32(item.TextoLibre);
            var edo = (from a in _context.Cat_Localidad
                       join b in _context.Cat_Municipio on a.municipio.id equals b.id
                       join c in _context.Cat_Estado on b.estado.id equals c.id
                       join hpe in _context.home_Producto_Estados on c.id equals hpe.id_estado
                       where a.cp == cp_
                       select new
                       {
                           id_producto = hpe.id_producto_home,
                           id_estado = c.id,
                           estado = c.desc_estado
                       }).FirstOrDefault();

            if (edo == null)
            {
                return response = Ok(new { result = "Error", detalle = "Codigo postal no existe" });
            }

            //var hp_sublineas = (from a in _context.home_producto_cliente
            //                    join b in _context.rel_homep_productos on a.id equals b.id_homep
            //                    join c in _context.Cat_SubLinea_Producto on b.id_sub_linea equals c.id
            //                    select new
            //                    {
            //                        b.id,
            //                        b.id_sub_linea,
            //                        cantidad = b.cantidad / c.hp_horas //cantidad de horas totales entre horas para obtener el numero de servicios que requiere
            //                    }).ToList();
            //int rels = 0;
            //int id_rels = 0;
            //float suma_ = hp_sublineas.Sum(a => a.cantidad);
            //if (hp_sublineas.Sum(s => s.cantidad) >= 3)
            //{
            //    id_rels = (from a in _context.productos_relacionados
            //                 where (a.id_producto == edo.id_producto)
            //                 select a.id_producto_2
            //                  ).FirstOrDefault();
            //    //rels = 2;
            //}
            ////else rels = 1;

            var hp = (from p in _context.Cat_Productos
                          //where p.id == edo.id_producto || p.id == (id_rels > 0 ? id_rels : edo.id_producto)
                      where p.id == edo.id_producto
                      select new
                      {
                          p.id,
                          p.sku,
                          p.nombre,
                          p.descripcion_corta,
                          p.precio_con_iva
                          //}).ToList();
                      }).FirstOrDefault();

            if (hp == null)
            {
                return response = Ok(new { result = "Error", detalle = "Producto no encontrado" });
            }
            //return response = Ok(new { result = "Success", item = hp, hp_sublineas = hp_sublineas });
            return response = Ok(new { result = "Success", item = hp });
        }

        [Route("getSubineasHomeProgram")]
        [HttpGet]
        public IActionResult CargaLineasHomeProgram()
        {
            IActionResult response = Unauthorized();

            try
            {

                //var car_prod = (from cp in _context.Productos_Carrito
                //                join p in _context.Cat_Productos on cp.Id_Producto equals p.id
                //                where cp.id_usuario == ids.Id2 && cp.Id_Producto != 388
                //                group new { p, cp } by p.id_sublinea into gp
                //                select new ids_cantidad
                //                {
                //                    id = Convert.ToInt32(gp.First().p.id_sublinea),
                //                    cantidad = gp.Sum(a => a.cp.cantidad)
                //                }).ToList();

                var sublineas = (from sl in _context.Cat_SubLinea_Producto
                                 join l in _context.Cat_Linea_Producto on sl.id_linea_producto equals l.id
                                 join p in _context.Cat_Productos on sl.id equals p.id_sublinea
                                 //where p.tipo == 1 || p.tipo == 2 || p.tipo == 4
                                 where l.id_superlinea < 4
                                 group new { sl, l, p } by p.id_sublinea into gp
                                 select new
                                 {
                                     id = gp.First().p.id_sublinea,
                                     descripcion = gp.First().l.descripcion + " | " + gp.First().sl.descripcion,
                                     hrs = gp.First().sl.hp_horas,
                                     estatus = true,
                                     a = gp.OrderBy(p => p.sl.descripcion),
                                     des1 = gp.First().sl.descripcion
                                 }).OrderBy(s => s.descripcion).ThenBy(s => s.des1).ToList();

                //var sublineas = (from sl in _context.Cat_SubLinea_Producto
                //                 join l in _context.Cat_Linea_Producto on sl.id_linea_producto equals l.id
                //                 join ssl in _context.cat_SuperLineas on l.id_superlinea equals ssl.id
                //                 where l.id_superlinea < 3
                //                 orderby sl.descripcion ascending
                //                 select new
                //                 {
                //                     sl.id,
                //                     sublinea = sl.descripcion,
                //                     linea = l.descripcion,
                //                     cantidad = 0
                //                 });
                if (sublineas != null)
                {
                    return response = Ok(new { token = "Success", item = sublineas });
                }

                return response = Ok(new { token = "Error", detalle = "Instruccion no válida" });
            }
            catch (Exception ex)
            {
                return response = Ok(new { token = "Error", detalle = "Error en el servidor: " + ex.Message });
            }

        }

        [Route("getLineasCertificados")]
        [HttpGet]
        public IActionResult CargaLineasCertificados()
        {
            IActionResult response = Unauthorized();

            try
            {
                var sublineas = (from a in _context.producto_certificado_sublinea
                                 join b in _context.Cat_SubLinea_Producto on a.Id_sublinea equals b.id
                                 join c in _context.Cat_Linea_Producto on b.id_linea_producto equals c.id
                                 orderby c.descripcion, b.descripcion ascending
                                 select new
                                 {
                                     b.id,
                                     sublinea = c.descripcion + " | " + b.descripcion,
                                     linea = c.descripcion,
                                     cantidad = 0
                                 });
                //var sublineas2 = (from sl in _context.Cat_SubLinea_Producto
                //                  join l in _context.Cat_Linea_Producto on sl.id_linea_producto equals l.id
                //                  join ssl in _context.cat_SuperLineas on l.id_superlinea equals ssl.id
                //                  where l.id_superlinea < 3
                //                  orderby sl.descripcion ascending
                //                  select new
                //                  {
                //                      sl.id,
                //                      sublinea = sl.descripcion,
                //                      linea = l.descripcion,
                //                      cantidad = 0
                //                  });
                if (sublineas != null)
                {
                    return response = Ok(new { token = "Success", item = sublineas });
                }

                return response = Ok(new { token = "Error", detalle = "Instruccion no válida" });
            }
            catch (Exception ex)
            {
                return response = Ok(new { token = "Error", detalle = "Error en el servidor: " + ex.Message });
            }

        }

        // POST: api/Partners_Productos
        [Route("CargarProducto")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult CargarProducto([FromBody]ModeloBusquedaCot busqueda)
        {
            IActionResult response = Unauthorized();
            var item = (from a in _context.Cat_Productos
                        orderby a.nombre ascending
                        where EF.Functions.Like(a.nombre+a.sku, "%" + busqueda.TextoLibre + "%")

                        select new
                        {
                            id = a.id,
                            sku = a.sku,
                            modelo = a.modelo,
                            nombre = a.nombre,
                            descripcion_corta = a.descripcion_corta,
                            descripcion_larga = "<ul><li>" + a.descripcion_larga.Replace(",", "</li><li>") + "</ul>",
                            descripcion_largaR = a.descripcion_larga.Trim().Substring(0, 400),
                            descripcion_largaM = a.descripcion_larga.Trim().Substring(0, 600),
                            atributos = "<ul><li>" + a.atributos.Replace(",", "</li><li>") + "</ul>",
                            precio_sin_iva = a.precio_sin_iva,
                            precio_con_iva = a.precio_con_iva,
                            id_categoria = a.id_categoria,
                            id_linea = a.linea,
                            id_sublinea = a.id_sublinea,
                            ficha_tecnica = a.ficha_tecnica,
                            estatus = a.estatus,
                            url_guia = a.url_guia,
                            url_manual = a.url_manual,
                            url_impresion = a.url_impresion,
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
            
            return response = Ok(new { token = "Success", item });
        }

        [Route("CargarResultadosMenu")]
        [HttpPost]
        public IActionResult CargarResultadosMenu([FromBody] ModBusPorDosId busqueda)
        {
            IActionResult response = Unauthorized();
            try
            {
                var prods = (from a in _context.Cat_Productos
                            join l in _context.Cat_Linea_Producto on a.id_linea equals l.id
                            join s in _context.Cat_SubLinea_Producto on a.id_sublinea equals s.id
                            
                            orderby a.nombre ascending
                            where l.id == (busqueda.Id1 == 1 ? busqueda.Id2 : l.id)
                            && s.id == (busqueda.Id1 == 2 ? busqueda.Id2 : s.id)
                            && a.id == (busqueda.Id1 == 3 ? busqueda.Id2 : a.id)
                            && a.visible_partners == true
                            select new
                            {
                                id = a.id,
                                sku = a.sku,
                                modelo = a.modelo,
                                nombre = a.nombre,
                                descripcion_corta = a.descripcion_corta,
                                descripcion_larga = "<ul><li>" + a.descripcion_larga.Replace(",", "</li><li>") + "</ul>",
                                prostributos = a.atributos,
                                descripcion_largaR = a.descripcion_larga.Trim().Substring(0, 400),
                                descripcion_largaM = a.descripcion_larga.Trim().Substring(0, 600),
                                atributos = "<ul><li>" + a.atributos.Replace(",", "</li><li>") + "</ul>",
                                precio_sin_iva = a.precio_sin_iva,
                                precio_con_iva = a.precio_con_iva,
                                id_categoria = a.id_categoria,
                                id_linea = a.linea,
                                id_sublinea = a.id_sublinea,
                                id_caracteristica_base = a.id_caracteristica_base,
                                ficha_tecnica = a.ficha_tecnica,
                                estatus = a.estatus,
                                tipo = a.tipo,
                                url_guia = a.url_guia,
                                url_manual = a.url_manual,
                                url_impresion = a.url_impresion,
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
                    return response = Ok(new { token = "Error", item = prods });
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

                    var item = (from c in _context.caracteristicas_Bases
                                join p in (from p in prods
                                           group p by p.id_caracteristica_base into g
                                           select new
                                           {
                                               id = g.First().id_caracteristica_base
                                           }) on c.id equals p.id
                                orderby c.descripcion ascending
                                select new
                                {
                                    car_base = c.descripcion,
                                    productos = (from pr in prods
                                                 where pr.id_caracteristica_base == c.id
                                                 select pr).ToList()
                                }).ToList();
                    return response = Ok(new { token = "Success", item });
                }
                
            }
            catch (Exception ex)
            {
                return response = Ok(new { token = "Error", detalle = ex.Message });
            }
            
        }

        // POST: api/Partners_Productos
        [Route("CargarResultados")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult CargarResultados([FromBody]ModeloBusquedaCot busqueda)
        {
            string TipoBusqueda = "";
            string txtsku = "";
            string txtlinea = "";
            string txtsubline = "";
            string busquedalibre = busqueda.TextoLibre;

            int k = busqueda.TextoLibre.IndexOf("SKU");
            if (k > 0)
            {
                TipoBusqueda = "SKU";
                int i = busqueda.TextoLibre.IndexOf("SKU: ");
                int f = busqueda.TextoLibre.IndexOf(" | MOD: ");
                txtsku = busqueda.TextoLibre.Substring(i + 5, f-i-5);
                busquedalibre = "";
            }
            else
            {
                int tl = busqueda.TextoLibre.IndexOf("SUBLÍNEA");
                if (tl < 0)
                {
                    int l = busqueda.TextoLibre.IndexOf("LÍNEA"); // si no trae linea es busqueda libre
                    if(l > 0)
                    {
                        TipoBusqueda = "LINEA";
                        int i = busqueda.TextoLibre.IndexOf("LÍNEA: ");
                        int f = busqueda.TextoLibre.IndexOf(" (");
                        txtlinea = busqueda.TextoLibre.Substring(i + 7, f - i - 7);
                        busquedalibre = "";

                    }
                }
                else
                {
                    TipoBusqueda = "SUBLINEA";
                    int i = busqueda.TextoLibre.IndexOf("SUBLÍNEA: ");
                    int f = busqueda.TextoLibre.IndexOf(" (");
                    txtsubline = busqueda.TextoLibre.Substring(i+10, f-i-10);
                    busquedalibre = "";
                }
            }


        IActionResult response = Unauthorized();

        var prods = (from a in _context.Cat_Productos
                    join l in _context.Cat_Linea_Producto on a.id_linea equals l.id
                    join s in _context.Cat_SubLinea_Producto on a.id_sublinea equals s.id
                    
                    orderby a.nombre ascending
                        where        EF.Functions.Like(l.descripcion, "%" + txtlinea + "%") 
                                  && EF.Functions.Like(s.descripcion, "%" + txtsubline + "%") 
                                  && EF.Functions.Like(a.sku, "%" + txtsku + "%") 
                                  && EF.Functions.Like(a.sku + a.modelo + a.nombre + l.descripcion + s.descripcion, "%" + busquedalibre + "%")
                                  && a.visible_partners == true
                                select new
                                    {
                                        id = a.id,
                                        sku = a.sku,
                                        modelo = a.modelo,
                                        nombre = a.nombre,
                                        descripcion_corta = a.descripcion_corta,
                                    descripcion_larga = "<ul><li>" + a.descripcion_larga.Replace(",", "</li><li>") + "</ul>",
                                    prostributos = a.atributos,
                                        descripcion_largaR = a.descripcion_larga.Trim().Substring(0, 400),
                                        descripcion_largaM = a.descripcion_larga.Trim().Substring(0, 600),
                                        atributos = "<ul><li>" + a.atributos.Replace(",", "</li><li>") + "</ul>",
                                        precio_sin_iva = a.precio_sin_iva,
                                        precio_con_iva = a.precio_con_iva,
                                        id_categoria = a.id_categoria,
                                        id_linea = a.linea,
                                        id_sublinea = a.id_sublinea,
                                        id_caracteristica_base = a.id_caracteristica_base,
                                        ficha_tecnica = a.ficha_tecnica,
                                        estatus = a.estatus,
                                        tipo= a.tipo,
                                         url_guia = a.url_guia,
                                         url_manual = a.url_manual,
                                        url_impresion = a.url_impresion,
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
                return response = Ok(new { token = "Error", item = prods });
            }
            else
            {
                foreach (var prod in prods)
                {
                    if ( prod.cat_imagenes_producto.Count == 0)
                   {
                        Cat_Imagenes_Producto cip = new Cat_Imagenes_Producto();
                        cip.url = "../assets/img/img_prod_no_dips.png";
                        cip.id = 0;
                        cip.id_producto = prod.id;
                        prod.cat_imagenes_producto.Add(cip);
                   }
                }

                var item = (from c in _context.caracteristicas_Bases
                            join p in (from p in prods
                                       group p by p.id_caracteristica_base into g
                                       select new
                                       {
                                           id = g.First().id_caracteristica_base
                                       }) on c.id equals p.id
                            orderby c.descripcion ascending
                            select new
                            {
                                car_base = c.descripcion,
                                productos = (from pr in prods
                                             where pr.id_caracteristica_base == c.id
                                             select pr).ToList()
                            }).ToList();
                return response = Ok(new { token = "Success", item });
            }
            
        }

        [Route("CaracteristicaBase")]
        [HttpGet]
        public IActionResult Get_Caracteristica_Base()
        {
            IActionResult response = Unauthorized();
            try
            {
                var item = _context.caracteristicas_Bases.Where(c => c.estatus == true);
                if (item == null)
                {
                    return response = Ok(new { result = "Error" });
                }

                return response = Ok(new { result = "Success", item });
            }
            catch (Exception ex)
            {
                return response = Ok(new { result = "Error", detalle = ex.Message });
            }
        }

        // POST: api/Partners_Productos
        [Route("ProductosRelacionados")]
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ProductosRelacionados([FromBody]ModBusPorId busqueda)
        {
            IActionResult response = Unauthorized();
            var id_pr = (from a in _context.productos_relacionados
                         where (a.id_producto == busqueda.Id) || (a.id_producto_2 == busqueda.Id)
                         select  new {
                                          id_producto = (a.id_producto == busqueda.Id ? a.id_producto_2 : a.id_producto),
                                     }).ToList();


            var item = (from a in _context.Cat_Productos
                        orderby a.nombre ascending
                        where a.tipo == 1 && id_pr.Any(prods => prods.id_producto == a.id)
                        select new
                        {
                            id = a.id,
                            sku = a.sku,
                            modelo = a.modelo,
                            nombre = a.nombre,
                            descripcion_corta = a.descripcion_corta,
                            precio_con_iva = a.precio_con_iva,
                            cat_imagenes_producto = (from x in _context.Cat_Imagenes_Producto
                                                     where x.id_producto == a.id
                                                     select x)
                                                         .Select(d => new Cat_Imagenes_Producto
                                                         {
                                                             id = d.id,
                                                             id_producto = d.id_producto,
                                                             url = d.url
                                                         }).ToList()
                        }).ToList().Take(3);
            if (item == null)
            {
                return response = Ok(new { token = "Error", item });
            }
            else
            {
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
            }
            return response = Ok(new { token = "Success", item });
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //////////////////////////////////////////     CERTIFICADO DE MANTENIMIENTO         ////////////////////////////////////
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        // POST: api/get_productos_certificado
        [Route("get_productos_certificado")]
        [HttpPost]
        public IActionResult get_productos_certificado([FromBody] lista_certificado lista)
        {
            IActionResult response = Unauthorized();
            decimal costo_viaticos = -1;
            string localidad = "Localidad no encontrada";
            decimal costo_labor = 0;
            decimal costo_consumibles = 0;
            int n_prods = 0;
            var certificados = (from c in _context.producto_certificado_sublinea
                                join p in lista.lista_sublineas on c.Id_sublinea equals p.id
                                join cp  in _context.Cat_Productos on c.id_producto equals cp.id
                                select new 
                                {
                                    c.id_producto , cp.nombre, p.cantidad, sublinea_id = p.id
                                }).ToList();

            if (certificados.Count > 0)
            {
                var certificados_ = (from c in certificados
                                     group c by c.id_producto into s2
                                     select new
                                     {
                                         id_producto = s2.First().id_producto,
                                         nombre = s2.First().nombre,
                                         cantidad = s2.Sum(s => s.cantidad)
                                     }).ToList();

                n_prods = certificados.Sum(m => m.cantidad);
                costo_labor = _context.Cer_labor.FirstOrDefault(l => l.cantidad_equipos == n_prods).anual;

                var distinct_sl = certificados.GroupBy(p => p.sublinea_id).Select(g => g.First()).ToList();

                var _consumibles = (from c in _context.rel_consumible_sublinea
                                    join p in distinct_sl on c.id_sublinea equals p.sublinea_id
                                    join cp in _context.Cer_consumibles on c.id_consumible equals cp.id
                                    select new
                                    { cp.consumible, cp.id, cp.costo_unitario }).ToList();


                var consumibles = (from c in _consumibles
                                   group c by c.id into s2
                                   select new
                                   {
                                       consumible = s2.First().consumible,
                                       id = s2.First().id,
                                       costo_unitario = s2.First().costo_unitario
                                   }).ToList();


                if (consumibles.Count > 0)
                    costo_consumibles = consumibles.Sum(m => m.costo_unitario);

                var costo_viaticos_l = (from c in _context.Cer_viaticos
                                        join l in _context.Cat_Localidad on c.id_cat_localidad equals l.id
                                        where l.cp == Convert.ToUInt32(lista.cp)
                                        select new
                                        {
                                            c.costo_unitario,
                                            localidad = l.desc_localidad + ", Municipio " + l.municipio.desc_municipio }).ToList();

                if (costo_viaticos_l.Count > 0)
                {
                    costo_viaticos = costo_viaticos_l[0].costo_unitario;
                    localidad = costo_viaticos_l[0].localidad;
                }
                    

                return response = Ok(new { token = "Success", certificados_, costo_labor, costo_consumibles, costo_viaticos, localidad });
            }
         
            return response = Ok(new { token = "Success", certificados , costo_labor, costo_consumibles, costo_viaticos, localidad });
        }

        public int save_certificados(guardar_certificados lista)
        {

            decimal costo_viaticos = 0;
            decimal costo_labor = 0;
            decimal costo_consumibles = 0;
            int id_viaticos = 0;
            int n_prods = 0;

            int respuesta = 0;
            var certificados = (from c in _context.producto_certificado_sublinea
                                join p in lista.lista_sublineas on c.Id_sublinea equals p.id
                                join cp in _context.Cat_Productos on c.id_producto equals cp.id
                                select new
                                {
                                    c.id_producto,
                                    cp.nombre,
                                    p.cantidad,
                                    sublinea_id = p.id
                                }).ToList();

            if (certificados.Count > 0)
            {
               
                var certificados_ = (from c in certificados
                                     group c by c.id_producto into s2
                                     select new
                                     {
                                         id_producto = s2.First().id_producto,
                                         nombre = s2.First().nombre,
                                         cantidad = s2.Sum(s => s.cantidad)
                                     }).ToList();

                n_prods = certificados.Sum(m => m.cantidad);
                costo_labor = _context.Cer_labor.FirstOrDefault(l => l.cantidad_equipos == n_prods).anual;

                var distinct_sl = certificados.GroupBy(p => p.sublinea_id).Select(g => g.First()).ToList();

                var _consumibles = (from c in _context.rel_consumible_sublinea
                                   join p in distinct_sl on c.id_sublinea equals p.sublinea_id
                                   join cp in _context.Cer_consumibles on c.id_consumible equals cp.id
                                   select new
                                   { cp.consumible,  cp.id, cp.costo_unitario  }).ToList();


                var consumibles = (from c in _consumibles
                                   group c by c.id into s2
                                     select new
                                     {
                                         consumible = s2.First().consumible,
                                         id = s2.First().id,
                                         costo_unitario = s2.First().costo_unitario
                                     }).ToList();


                if (consumibles.Count > 0)
                    costo_consumibles = consumibles.Sum(m => m.costo_unitario);

                var costo_viaticos_l = (from c in _context.Cer_viaticos
                                        join l in _context.Cat_Localidad on c.id_cat_localidad equals l.id
                                        where l.cp == Convert.ToUInt32(lista.cp)
                                        select new
                                        { c.costo_unitario,c.id }).ToList();

                if (costo_viaticos_l.Count > 0)
                {
                    costo_viaticos = costo_viaticos_l[0].costo_unitario;
                    id_viaticos = costo_viaticos_l[0].id;
                }


                if (costo_viaticos < 0)
                    respuesta = 0;
                else
                {


                    decimal costo_viaticos_div = 0;
                    decimal costo_labor_div = 0;
                    decimal costo_consumibles_div = 0;
                    float total_div = 0;
                    float total_sn_iva_div = 0;
                    float total_iva_div = 0;
                    int n_cert = certificados_.Sum(m => m.cantidad);

                    if (lista.cotizacion_id > 0)
                    {
                        var id_clie_cot = _context.Cotizaciones.FirstOrDefault(h => h.Id == lista.cotizacion_id).Id_Cliente;
                        ////////// hay que borrar todo lo que exista de Certificados en carrito y en cotizacion
                        eliminar_registros_certificado(lista.id_usuario, id_clie_cot, lista.cotizacion_id);
                        foreach (var producto in certificados_)
                        {
                            costo_viaticos_div = (costo_viaticos / n_cert);// * producto.cantidad;
                            costo_labor_div = (costo_labor / n_cert);//* producto.cantidad;
                            costo_consumibles_div = (costo_consumibles / n_cert) ;//* producto.cantidad;
                            total_div = (float)costo_viaticos_div + (float)costo_labor_div + (float)costo_consumibles_div;

                            total_sn_iva_div = (float)(total_div / 1.16);
                            total_sn_iva_div = (float)(Math.Round((double)total_sn_iva_div, 2));

                            total_iva_div = (total_div - (float)(total_div / 1.16));
                            total_iva_div = (float)(Math.Round((double)total_iva_div, 2));

                            Cotizacion_Producto cm = new Cotizacion_Producto();
                            cm.cantidad = producto.cantidad;
                            cm.Id_Cotizacion = lista.cotizacion_id;
                            cm.precio_condiciones_com = total_sn_iva_div;
                            cm.iva_cond_comerciales = total_iva_div;
                            cm.precio_descuento = total_sn_iva_div;
                            cm.iva_precio_descuento = total_iva_div;
                            cm.precio_lista = total_sn_iva_div;
                            cm.iva_precio_lista = total_iva_div;
                            cm.Id_Producto = producto.id_producto;
                            _context.Cotizacion_Producto.Add(cm);
                            // _context.SaveChanges();
                            var sublineas = certificados.Where(f => f.id_producto == producto.id_producto).ToList();
                            foreach (var sl in sublineas)
                                save_registros_tickets_certificado(id_viaticos, 0, id_clie_cot, sl.cantidad, sl.sublinea_id, false, lista.cotizacion_id);
                        }

                        _context.SaveChanges();
                        respuesta = 1;
                    }
                    else
                    {
                        eliminar_registros_certificado(lista.id_usuario, 0, 0);

                        foreach (var producto in certificados_)
                        {
                            costo_viaticos_div = (costo_viaticos / n_cert);// * producto.cantidad;
                            costo_labor_div = (costo_labor / n_cert);// * producto.cantidad;
                            costo_consumibles_div = (costo_consumibles / n_cert);// * producto.cantidad;
                            total_div = (float)costo_viaticos_div + (float)costo_labor_div + (float)costo_consumibles_div;

                            total_sn_iva_div = (float)(total_div / 1.16);
                            total_sn_iva_div = (float)(Math.Round((double)total_sn_iva_div, 2));

                            total_iva_div = (total_div - (float)(total_div / 1.16));
                            total_iva_div = (float)(Math.Round((double)total_iva_div, 2));
                            
                            Productos_Carrito cm = new Productos_Carrito();
                            cm.cantidad = producto.cantidad;
                            cm.id_usuario = lista.id_usuario;
                            cm.precio_condiciones_com = total_sn_iva_div;
                            cm.iva_cond_comerciales = total_iva_div;
                            cm.precio_descuento = total_sn_iva_div;
                            cm.iva_precio_descuento = total_iva_div;
                            cm.precio_lista = total_sn_iva_div;
                            cm.iva_precio_lista = total_iva_div;
                            cm.Id_Producto = producto.id_producto;
                            _context.Productos_Carrito.Add(cm);
                            var sublineas = certificados.Where(f => f.id_producto == producto.id_producto).ToList();

                            long id_cliente_c = _context.Cotizaciones.FirstOrDefault(b => b.Id_Cliente == lista.id_usuario).Id;
                            foreach (var sl in sublineas)
                                save_registros_tickets_certificado(id_viaticos, 0, lista.id_usuario, sl.cantidad, sl.sublinea_id, false, Convert.ToInt32(id_cliente_c));
                        }
                        _context.SaveChanges();
                        crear_cotizacion_carrito(lista.id_usuario);
                    }
                   
                    respuesta = 1;
                }
            }
            else
            {
                respuesta = 0;
                if (lista.cotizacion_id > 0)   ////////// hay que borrar todo lo que exista de Certificados en carrito y en cotizacion
                {
                    var id_clie_cot = _context.Cotizaciones.FirstOrDefault(h => h.Id == lista.cotizacion_id).Id_Cliente;
                    eliminar_registros_certificado(lista.id_usuario, id_clie_cot, lista.cotizacion_id);
                }
                else
                {
                    eliminar_registros_certificado(lista.id_usuario, 0, 0);
                }
            }
            return respuesta;
        }

        

        public void eliminar_registros_certificado(int id_usuario, long id_cliente, long id_cotizacion)
        {
            var sublineas_crt = _context.Cat_SubLinea_Producto.Where(u => u.id_linea_producto == 36).ToList();
            var prods_crt = _context.Cat_Productos.Where(u => sublineas_crt.Any(k => k.id == u.id_sublinea)).ToList(); // se obtienenlos productos del certificado del catalogo

            if (id_cotizacion == 0)//cuando e id_cotizacion es == 0 es por que se envio el de usuario 
            {
                /////  hay que eliminar esto 
                var pc = _context.Productos_Carrito.Where(u => u.id_usuario == id_usuario && prods_crt.Any(k => k.id == u.Id_Producto)).ToList();
                var cpc = _context.Cer_producto_cliente.Where(u => u.id_cliente == id_usuario).ToList(); // cuando son produtos en carrito el cliente es el mismo usuario
                var rcp = _context.rel_certificado_producto.Where(u => cpc.Any(k => k.id == u.id_certificado)).ToList();
                foreach (var P_C in pc)
                    _context.Productos_Carrito.Remove(P_C);

                foreach (var CPC in cpc)
                    _context.Cer_producto_cliente.Remove(CPC);

                foreach (var RCP in rcp)
                    _context.rel_certificado_producto.Remove(RCP);

            }
            else //cuando e id_cotizacion es != 0 son productos de cat_cotizacion
            {
                /////  hay que eliminar esto 
                var pc = _context.Cotizacion_Producto.Where(u => u.Id_Cotizacion == id_cotizacion && prods_crt.Any(k => k.id == u.Id_Producto)).ToList();
                var cpc = _context.Cer_producto_cliente.Where(u => u.id_cliente == id_cliente).ToList();
                var rcp = _context.rel_certificado_producto.Where(u => cpc.Any(k => k.id == u.id_certificado)).ToList();
                foreach (var P_C in pc)
                    _context.Cotizacion_Producto.Remove(P_C);

                foreach (var CPC in cpc)
                    _context.Cer_producto_cliente.Remove(CPC);

                foreach (var RCP in rcp)
                    _context.rel_certificado_producto.Remove(RCP);
            }
            _context.SaveChanges();

        }


        public long save_registros_tickets_certificado(int id_viaticos, int costo, long id_cliente, int cantidad, int id_sublinea, bool estatus, int id_cotizacion)
        {
            for (int i = 0; i < cantidad; i++)
            {
                Cer_producto_cliente cpc = new Cer_producto_cliente();
                cpc.actualizado = DateTime.Now;
                cpc.actualizadopor = 0;
                cpc.creado = DateTime.Now;
                cpc.creadopor = 0;
                cpc.id_cliente = id_cliente;
                cpc.id_labor = 1;
                cpc.id_viaticos = id_viaticos;
                cpc.costo = costo;
                cpc.folio = "CM000" + id_viaticos.ToString() + id_sublinea.ToString() + DateTime.Now.Millisecond ;
                cpc.estatus_venta = estatus;
                cpc.id_cotizacion = id_cotizacion;
                _context.Cer_producto_cliente.Add(cpc);

                rel_certificado_producto rcp = new rel_certificado_producto();
                rcp.id_certificado = cpc.id;
                rcp.id_producto = 0;
                rcp.estatus_activo = estatus;
                rcp.no_visitas = 0;
                rcp.id_sub_linea = id_sublinea;
                rcp.creado = DateTime.Now;
                _context.rel_certificado_producto.Add(rcp);
            }

            _context.SaveChanges();
            return cantidad;
        }

        public long crear_cotizacion_carrito(int id_vendedor)
        {
             long current_C = 0;
            List<Cotizaciones> List_C = _context.Cotizaciones.Where(CL => CL.Id_Cliente == id_vendedor).ToList();
           
            if (List_C.Count < 1)
            {
                var C = new Cotizaciones();
                var vendedor = _context.Users.FirstOrDefault(U => U.id == id_vendedor);
                var sucursal = _context.Cat_Sucursales.FirstOrDefault(S => S.Id == vendedor.id_Sucursales);
                var cuenta = _context.Cat_Cuentas.FirstOrDefault(R => R.Id == sucursal.Id_Cuenta);


                C.Numero = "";
                C.Id_Cliente = id_vendedor;
                C.Id_Vendedor = id_vendedor;
                C.fecha_cotiza = DateTime.Now;
                C.Estatus = 1;
                C.Acciones = 0;
                C.Id_Estado_Instalacion = 0;
                C.Observaciones = "Esta es una cotización de carrito, sin cliente asignado aún.";
                C.creadopor = id_vendedor;
                C.Id_Canal = cuenta.Id_Canal;
                C.Id_Cuenta = cuenta.Id;
                C.Id_sucursal = sucursal.Id;
                _context.Cotizaciones.Add(C);

                _context.SaveChanges();
                current_C = C.Id;
            }
            else
            {
                current_C = List_C[0].Id;
            }

            List<Cotizacion_Producto> CP_L = _context.Cotizacion_Producto.Where(CP => CP.Id_Cotizacion == current_C).ToList(); // se cargan todos os productos de la cotizacion/carrito
            foreach (Cotizacion_Producto CP in CP_L)
                _context.Cotizacion_Producto.Remove(CP);

            List<Productos_Carrito> PC_L = _context.Productos_Carrito.Where(CP => CP.id_usuario == id_vendedor).ToList();
            foreach (var PC in PC_L)
            {
                Cotizacion_Producto PD = new Cotizacion_Producto();
                PD.Id_Cotizacion = current_C;
                PD.Id_Producto = PC.Id_Producto;
                PD.cantidad = PC.cantidad;
                PD.precio_lista = PC.precio_lista;
                PD.iva_precio_lista = (float)(PC.iva_precio_lista);
                PD.precio_descuento = (float)PC.precio_descuento;
                PD.iva_precio_descuento = (float)PC.iva_precio_descuento;
                PD.precio_condiciones_com = (float)PC.precio_condiciones_com;
                PD.iva_cond_comerciales = (float)PC.iva_cond_comerciales;
                PD.margen_cc = PC.margen_cc;
                _context.Cotizacion_Producto.Add(PD);
            }
            _context.SaveChanges();


            return current_C;
        }


        // POST: api/get_productos_certificado
        [Route("save_productos_certificado")]
        [HttpPost]
        public IActionResult save_productos_certificado([FromBody] guardar_certificados lista)
        {
            IActionResult response = Unauthorized();
            
            try
            {
                if(save_certificados(lista) == 1)
                    return response = Ok(new { token = "Success", detalle = "Productos incertados en la cotizacion/carrito" });
                else
                    return response = Ok(new { token = "Error", detalle = "No hay Certificados asociados a la sublineas" });
            }
            catch(Exception ex)
            {
                return response = Ok(new { token = "Error", detalle = ex.Message });
            }
            
        }

        // POST: api/get_productos_certificado
        [Route("save_productos_certificado_carrito")]
        [HttpPost]
        public IActionResult save_productos_certificado_carrito([FromBody] ModBusPorDosId ids)  
        {

            IActionResult response = Unauthorized();
            try
            {
                var car_prod = (from cp in _context.Productos_Carrito
                                join p in _context.Cat_Productos on cp.Id_Producto equals p.id
                                where cp.id_usuario == ids.Id2 && cp.Id_Producto != 388
                                group new { p, cp } by p.id_sublinea into gp
                                select new ids_cantidad
                                {
                                    id = Convert.ToInt32(gp.First().p.id_sublinea),
                                    cantidad = gp.Sum(a => a.cp.cantidad)
                                }).ToList();

                guardar_certificados lista = new guardar_certificados();
                string cp1 = ids.Id1.ToString().Length < 5 ? "0" + ids.Id1.ToString() : ids.Id1.ToString();
                lista.cp = cp1;
                lista.cotizacion_id = 0;
                lista.id_usuario = ids.Id2;
                lista.lista_sublineas = car_prod;

                if (save_certificados(lista) == 1)
                    return response = Ok(new { token = "Success", detalle = "Productos incertados en la cotizacion/carrito" });
                else
                    return response = Ok(new { token = "Error", detalle = "No hay Certificados asociados a la sublineas" });
            }
            catch (Exception ex)
            {
                return response = Ok(new { token = "Error", detalle = ex.Message });
            }

        }

        // PUT: api/Partners_Productos/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        //////////////////////////  DEFINICIONES ///////////////////

        public class ModeloBusquedaCot
        {
            public string TextoLibre { get; set; }
        }

        public class ModBusPorId
        {
            public int Id { get; set; }
        }

        public class ModBusPorDosId
        {
            public int Id1 { get; set; }
            public int Id2 { get; set; }
        }

        public class ids_cantidad
        {
            public int id { get; set; }
            public int cantidad { get; set; }
        }

        public class lista_certificado
        {
            public string cp { get; set; }
            public List<ids_cantidad> lista_sublineas { get; set; }
        }

        public class guardar_home_program
        {
            public string cp { get; set; }
            public int cotizacion_id { get; set; }
            public int id_usuario { get; set; }
            public List<Productos_Carrito> prods_home { get; set; }
            public List<ids_cantidad> lista_sublineas { get; set; }
        }

        public class guardar_certificados
        {
            public string cp { get; set; }
            public int cotizacion_id { get; set; }
            public int id_usuario { get; set; }
            public List<ids_cantidad> lista_sublineas { get; set; }
        }


        public class ModeloResultadoCot
        {
            public long Id { get; set; }
            public string Modelo { get; set; }
            public string Nombre { get; set; }
        }

        public class inicio_servicio
        {
            public int id { get; set; }
            public string latitud { get; set; }
            public string longitud { get; set; }
        }
    }
}
