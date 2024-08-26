using System.Linq;
using WebApplication.Models;
using WebApplication.Repositoriy.Generic;

namespace WebApplication.Repository
{
    public class CanalesRepository : GenericRepository<Canales>, ICanalesRepository
    {
        public CanalesRepository(MieleContext context) : base(context) { }
    }
}
