using WebApplication.Models;
using WebApplication.Repositoriy.Generic;

namespace WebApplication.Repository
{
    public interface IQuejasRepository : IGenericRepository<Quejas>
    {
        void UpdateStatus(int quejaId);
    }
}
