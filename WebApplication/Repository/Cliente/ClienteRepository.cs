using System.Linq;
using WebApplication.Models;
using WebApplication.Repositoriy.Generic;

namespace WebApplication.Repository
{
    public class ClienteRepository : GenericRepository<Clientes>, IClienteRepository
    {
        public ClienteRepository(MieleContext context) : base(context) { }
    }
}
