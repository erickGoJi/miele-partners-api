using WebApplication.Models;
using WebApplication.Repositoriy.Generic;

namespace WebApplication.Repository
{
    public class ProductCategoriesRepository : GenericRepository<Cat_Categoria_Producto>, IProductCategoriesRepository
    {
        public ProductCategoriesRepository (MieleContext context) : base (context) { }
    }
}
