using System.Linq;
using WebApplication.Models;
using WebApplication.Repositoriy.Generic;

namespace WebApplication.Repository
{
    public class TipoQuejasRepository : GenericRepository<TipoQueja>, ITipoQuejaRepository
    {
        public TipoQuejasRepository(MieleContext context) : base(context) { }
    }
}
