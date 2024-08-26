using WebApplication.Models;
using WebApplication.Repositoriy.Generic;
using WebApplication.ViewModels;
using System.Collections.Generic;
using System.Collections;

namespace WebApplication.Repository
{
    public interface IProductoRepository : IGenericRepository<Cat_Productos>
    {
        IList FindAllSearch(ProductsSearchDto productsSearchDto);
    }
}
