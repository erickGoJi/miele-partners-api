using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using WebApplication.Models;
using WebApplication.Repositoriy.Generic;
using System.Collections.Generic;
using WebApplication.ViewModels;
using System;

namespace WebApplication.Repository
{
    public class ClientsRepository : GenericRepository<Clientes>, IClientsRepository
    {
        public ClientsRepository(MieleContext context) : base(context) { }

        public override Clientes Add(Clientes cliente)
        {
            Users user = _context.Users.Where(u => u.id == cliente.creadopor).FirstOrDefault();
            cliente.creado = DateTime.Now;
            cliente.Id_sucursal = user.id_Sucursales;

            if (cliente.referidopor != 0 && user.id_canal == 3)
                cliente.vigencia_ref = DateTime.Now.AddMonths(6);
            else
                cliente.referidopor = 0;
            return base.Add(cliente);
        }

        public List<Clientes> FindAllSearch(ClientsSearchDto clientsSearchDto)
        {
            string[] words = clientsSearchDto.text.Split(' ');
            var branchOffice = _context.Cat_Sucursales.Where(b =>
                b.Id == (clientsSearchDto.Id_sucursal == 0 ? b.Id : clientsSearchDto.Id_sucursal)
                && b.Id_Cuenta == (clientsSearchDto.id_account == 0 ? b.Id_Cuenta : clientsSearchDto.id_account)
                && b.Cuenta.Id_Canal == (clientsSearchDto.id_channel == 0 ? b.Cuenta.Id_Canal : clientsSearchDto.id_channel)).Select(b => b.Id);

            //var item = _context.Clientes.Where(c => c.nombre.Any(words) || words.Any(c.paterno.Contains) || words.Any(c.materno.Contains)
            //|| words.Any(c.email.Contains));

            var item = _context.Clientes.Where(c => c.Id_sucursal == (clientsSearchDto.Id_sucursal == 0 ? c.Id_sucursal : clientsSearchDto.Id_sucursal)
               || branchOffice.Contains(c.Id_sucursal));
            var item2 = _context.Clientes.Take(0);

            foreach (string word in words)
                item2 = item2.Union(item.Where(c => c.nombre.Contains(word) || c.paterno.Contains(word) 
                || c.materno.Contains(word) || c.email.Contains(word)));

            return item2.ToList();
        }
    }
}
