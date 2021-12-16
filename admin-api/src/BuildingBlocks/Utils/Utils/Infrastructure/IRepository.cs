using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Utils
{
    public interface IRepositoryBase
    {
        List<Task> GetAllTask();
    }
    public interface IRepository<T> : IRepositoryBase where T : class
    {
        Task<List<T>> FromSql(string sql, params object[] parameters);
        T Find(params object[] id);
        T Find(Expression<Func<T, bool>> predicate);
        Task<T> FindAsync(params object[] id);
        Task<T> FindAsync(Expression<Func<T, bool>> predicate);
        bool Any(params object[] id);
        Task<bool> AnyAsync(params object[] id);
        int Count();
        int Count(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
        long LongCount();
        long LongCount(Expression<Func<T, bool>> predicate);
        Task<long> LongCountAsync();
        Task<long> LongCountAsync(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> predicate);
        void AddRange(IEnumerable<T> entities);
        void AddRange(params T[] entities);
        void DeleteRange(params T[] entities);
        void DeleteRange(IEnumerable<T> entities);
        void DeleteRange(Expression<Func<T, bool>> predicate);
        IQueryable<T> GetAll();
        IQueryable<T> Get(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate);
        DbSet<T> DbSet();
    }
}