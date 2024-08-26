using WebApplication.Models;
using WebApplication.Repositoriy.Generic;

namespace WebApplication.Repository
{
    public class BranchOfficesRepository : GenericRepository<Cat_Sucursales>, IBranchOfficesRepository
    {
        public BranchOfficesRepository(MieleContext context) : base(context) { }
    }
}
