using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication.Models;
using WebApplication.Repositoriy.Generic;

namespace WebApplication.Repository
{
    public class PropuestasRepository : GenericRepository<Propuestas>, IPropuestasRepository
    {
        public PropuestasRepository(MieleContext context) : base(context) { }

        public override Propuestas Add(Propuestas propuesta)
        {
            propuesta.Fecha = DateTime.Now;
            return base.Add(propuesta);
        }

        public List<Propuestas> AddOrModify(List<Propuestas> propuestas)
        {
            var entities = new List<Propuestas>();

            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in propuestas)
                    {
                        if (item.Id == 0)
                        {
                            item.Fecha = DateTime.Now;

                            if (!string.IsNullOrEmpty(item.DetalleCierre))
                                item.FechaCierre = DateTime.Now;

                            _context.Propuestas.Add(item);
                            entities.Add(item);
                        }
                        else
                        {
                            var entity = _context.Propuestas.Where(c => c.Id == item.Id).FirstOrDefault();

                            if (!string.IsNullOrEmpty(item.DetalleCierre))
                                entity.FechaCierre = DateTime.Now;

                            if (!string.IsNullOrEmpty(item.Solucion))
                                entity.Solucion = item.Solucion;

                            if (!string.IsNullOrEmpty(item.DetalleCierre))
                                entity.DetalleCierre = item.DetalleCierre;

                            _context.Propuestas.Update(entity);
                            entities.Add(entity);
                        }
                    }
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }

                return entities;
            }
        }

        public void CorreoEnviado(int id)
        {
            var propuesta = _context.Propuestas.Where(c => c.Id == id).SingleOrDefault();

            if (propuesta != null)
            {
                propuesta.Email = true;
                _context.SaveChanges();
            }
        }
    }
}
