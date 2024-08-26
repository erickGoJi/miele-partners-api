using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using WebApplication.Models;
using WebApplication.Repositoriy.Generic;
using System.Collections.Generic;
using WebApplication.ViewModels;
using System.Collections;

namespace WebApplication.Repository
{
    public class ProductoRepository : GenericRepository<Cat_Productos>, IProductoRepository
    {
        public ProductoRepository(MieleContext context) : base(context) { }

        public override Cat_Productos Get(int id)
        {
            return _context.Cat_Productos.Where(p => p.id == id)
                .Include(p => p.sublinea)
                .Include(p => p.linea)
                .Include(p => p.sublinea)
                .Include(p => p.cat_imagenes_producto)
                .FirstOrDefault();
        }

        public IList FindAllSearch(ProductsSearchDto productsSearchDto)
        {
            if (productsSearchDto.text == "string")
                productsSearchDto.text = "";
            List<Cat_Productos> prods = _context.Cat_Productos.Where(p => p.tipo == (productsSearchDto.id_categoria == 0 ? p.tipo : productsSearchDto.id_categoria)
                && p.id_linea == (productsSearchDto.id_linea == 0 ? p.id_linea : productsSearchDto.id_linea)
                && p.id_sublinea == (productsSearchDto.id_sublinea == 0 ? p.id_sublinea : productsSearchDto.id_sublinea)
                && (p.nombre.Contains(productsSearchDto.text) || p.sku.Contains(productsSearchDto.text)
                || p.no_serie.Contains(productsSearchDto.text) || p.descripcion_corta.Contains(productsSearchDto.text)
                || p.descripcion_larga.Contains(productsSearchDto.text) || p.atributos.Contains(productsSearchDto.text))).ToList();

            var item = (from a in prods
                        select new
                        {
                            a.id,
                            a.sku, 
                            a.tipo, 
                            a.no_serie, 
                            a.modelo,
                            a.nombre, 
                            a.descripcion_corta,
                            a.precio_sin_iva, 
                            a.precio_con_iva, 
                            //a.categoria, 
                            a.id_categoria,
                            linea = (from x in _context.Cat_Linea_Producto
                                       where x.id == a.id_linea
                                       select x)
                                         .Select(c => new 
                                         {
                                             c.id,
                                             c.descripcion,
                                             c.estatus
                                         }).FirstOrDefault(),
                            a.id_linea, 
                            sublinea = (from x in _context.Cat_SubLinea_Producto
                                                     where x.id == a.id_sublinea
                                                     select x)
                                         .Select(c => new
                                         {
                                             c.id,
                                             c.descripcion,
                                             c.estatus,
                                             c.id_linea_producto
                                         }).FirstOrDefault(),
                            a.id_sublinea
                        }).ToList();

            return item;
        }
    }
}
