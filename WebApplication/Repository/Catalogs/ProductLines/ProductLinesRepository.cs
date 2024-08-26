using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApplication.Models;
using WebApplication.Repositoriy.Generic;

namespace WebApplication.Repository
{
    public class ProductLinesRepository : GenericRepository<Cat_Linea_Producto>, IProductLinesRepository
    {
        public ProductLinesRepository(MieleContext context) : base(context) { }
    }
}
