using System.Collections.Generic;
using WebApplication.Models;
using WebApplication.Repositoriy.Generic;

namespace WebApplication.Repository
{
    public interface IPropuestasRepository : IGenericRepository<Propuestas>
    {
        void CorreoEnviado(int id);
        List<Propuestas> AddOrModify(List<Propuestas> list);
    }
}
