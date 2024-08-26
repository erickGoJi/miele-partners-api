using System.Collections.Generic;
using WebApplication.Models;
using WebApplication.Repositoriy.Generic;
using WebApplication.ViewModels;

namespace WebApplication.Repository
{
    public interface IClientsRepository : IGenericRepository<Clientes>
    {
        List<Clientes> FindAllSearch(ClientsSearchDto clientsSearchDto);
    }
}
