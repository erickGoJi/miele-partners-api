using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using WebApplication.Models;
using WebApplication.Repositoriy.Generic;

namespace WebApplication.Repository
{
    public class QuejasRepository : GenericRepository<Quejas>, IQuejasRepository
    {
        public QuejasRepository(MieleContext context) : base(context) { }

        public override Quejas Add(Quejas queja)
        {
            int numeroQuejas = Count() + 1;
            queja.Folio = "VOC" + DateTime.Now.ToString("ddMMyy") + string.Format("{0:000000}", numeroQuejas);
            queja.Fecha = DateTime.Now;

            if (queja.Propuestas != null
                && queja.Propuestas.Count > 0)
            {
                foreach (var item in queja.Propuestas)
                {
                    item.Fecha = DateTime.Now;

                    if (!string.IsNullOrEmpty(item.DetalleCierre))
                        item.FechaCierre = DateTime.Now;
                }
            }

            return base.Add(queja);
        }

        public override Quejas Get(int id)
        {
            return _context.Quejas.Where(c => c.Id == id)
                .Include(c => c.Canal)
                .Include(c => c.ProductosQuejas)
                .ThenInclude(x => x.Producto)
                .Include(c => c.Propuestas)
                .Include(c => c.Cliente)
                .Include(c => c.TipoQueja)
                .FirstOrDefault();
        }

        public override Quejas Find(Expression<Func<Quejas, bool>> match)
        {
            return _context.Quejas.Where(match)
                .Include(c => c.Canal)
                .Include(c => c.ProductosQuejas)
                .ThenInclude(x => x.Producto)
                .Include(c => c.Propuestas)
                .Include(c => c.Cliente)
                .Include(c => c.TipoQueja)
                .FirstOrDefault();
        }

        public void UpdateStatus(int quejaId)
        {
            var queja = _context.Quejas
                .Where(c => c.Id == quejaId)
                .Include(c => c.Propuestas)
                .FirstOrDefault();

            if (queja.Propuestas != null 
                && queja.Propuestas.Count > 0)
            {
                if(queja.Propuestas.Any(c => c.FechaCierre == null))
                    queja.Estatus = true;
                else
                    queja.Estatus = false;
            }
            else
                queja.Estatus = true;

            _context.SaveChanges();
        }
    }
}
