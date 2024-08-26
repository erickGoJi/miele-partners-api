using WebApplication.Models;
using WebApplication.Repositoriy.Generic;

namespace WebApplication.Repository
{
    public interface IProductLinesRepository : IGenericRepository<Cat_Linea_Producto>
    {
        bool Exists(int? id_linea);
    }
}
