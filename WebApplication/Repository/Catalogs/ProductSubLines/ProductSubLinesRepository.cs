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
    public class ProductSubLinesRepository : GenericRepository<Cat_SubLinea_Producto>, IProductSubLinesRepository
    {
        public ProductSubLinesRepository(MieleContext context) : base(context) { }

        public IList FindAllSearch(ProductSubLineSearchDto dto)
        {
            if (dto.text == "string")
                dto.text = "";
            var temp = (dto.id_linea_producto == 0 ? -1 : dto.id_linea_producto);
            List<Cat_SubLinea_Producto> res = _context.Cat_SubLinea_Producto
                .Where(sl => sl.id_linea_producto == (dto.id_linea_producto == 0 ? sl.id_linea_producto : dto.id_linea_producto)
                && sl.descripcion.Contains(dto.text) && sl.id_linea_producto != 36 && sl.id_linea_producto != 38).ToList();
            var item = (from a in res
                        join l in _context.Cat_Linea_Producto on a.id_linea_producto equals l.id
                        select new
                        {
                            id = a.id,
                            descripcion = a.descripcion,
                            //id_linea_producto = sl.id_linea_producto,
                            linea_producto_desc = l.descripcion,
                            estatus = a.estatus
                        }).ToList();
            return item;
        }
    }


}