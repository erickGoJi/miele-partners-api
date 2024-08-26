using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;
using WebApplication.Repositoriy.Generic;

namespace WebApplication.Repository
{
    public class ProductSubLineTypeRepository : GenericRepository<Rel_Categoria_Producto_Tipo_Producto>, IProductSubLineTypeRepository
    {
        public ProductSubLineTypeRepository(MieleContext context): base(context) { }
    }
}
