using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebApplication.Repositoriy.Generic
{
    public interface IGenericRepository<T>
    {
        void Save();
        void Dispose();
        Task<int> SaveAsync();

        T Add(T t);
        Task<T> AddAsyn(T t);

        T Get(int id);
        Task<T> GetAsync(int id);

        T Get(long id);
        Task<T> GetAsync(long id);

        int Count();
        Task<int> CountAsync();

        void Delete(T entity);
        Task<int> DeleteAsyn(T entity);

        T Update(T t, object key);
        Task<T> UpdateAsyn(T t, object key);

        T Find(Expression<Func<T, bool>> match);
        Task<T> FindAsync(Expression<Func<T, bool>> match);

        bool Exists(int id);
        Task<bool> ExistsAsync(int id);

        bool Exists(long id);
        Task<bool> ExistsAsync(long id);

        bool Exists(Expression<Func<T, bool>> match);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> match);

        ICollection<T> FindAll(Expression<Func<T, bool>> match);
        Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match);

        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        Task<ICollection<T>> FindByAsyn(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetAll();
        Task<ICollection<T>> GetAllAsyn();
        IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties);
    }
}
