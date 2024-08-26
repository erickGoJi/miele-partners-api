using WebApplication.Models;
using WebApplication.Repositoriy.Generic;

namespace WebApplication.Repository
{
    public class UsersRepository : GenericRepository<Users>, IUsersRepository
    {
        public UsersRepository(MieleContext context) : base(context) { }

    }
}
