using System.Collections;
using WebApplication.Models;
using WebApplication.Repositoriy.Generic;
using WebApplication.ViewModels;


namespace WebApplication.Repository
{
    public interface IProductSubLinesRepository : IGenericRepository<Cat_SubLinea_Producto>
    {
        bool Exists(int? id_sublinea);
        IList FindAllSearch(ProductSubLineSearchDto dto);
    }
}