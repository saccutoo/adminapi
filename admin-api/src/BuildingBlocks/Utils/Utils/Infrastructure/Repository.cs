using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Utils
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DbSet<T> _dbset;
        private DbContext _dataContext;
        private readonly string Prefix;

        public List<Task> ListEsTask = new List<Task>();

        public Repository(DbContext dataContext, string prefix)
        {
            Prefix = prefix;
            _dataContext = dataContext;
            _dbset = _dataContext.Set<T>();
        }
        protected IDatabaseFactory DatabaseFactory { get; }
        protected DbContext DataContext => _dataContext ?? (_dataContext = DatabaseFactory.GetDbContext());
        public List<Task> GetAllTask()
        {
            return ListEsTask;
        }
        public virtual Task<List<T>> FromSql(string sql, params object[] parameters)
        {
            return _dbset.FromSql(sql, parameters).ToListAsync();
        }
        public virtual T Find(params object[] id)
        {
            return _dbset.Find(id);
        }
        public virtual T Find(Expression<Func<T, bool>> predicate)
        {
            return _dbset.Where(predicate).FirstOrDefault();
        }
        public virtual Task<T> FindAsync(params object[] id)
        {
            return _dbset.FindAsync(id);
        }
        public virtual async Task<T> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbset.Where(predicate).FirstOrDefaultAsync();
        }
        public virtual bool Any(params object[] id)
        {
            var entity = _dbset.Find(id);
            if (entity == null) return false;
            _dataContext.Entry(entity).State = EntityState.Detached;
            return true;
        }
        public virtual async Task<bool> AnyAsync(params object[] id)
        {
            var entity = await _dbset.FindAsync(id);
            if (entity == null) return false;
            _dataContext.Entry(entity).State = EntityState.Detached;
            return true;
        }
        public virtual int Count()
        {
            return _dbset.Count();
        }
        public virtual int Count(Expression<Func<T, bool>> predicate)
        {
            var objects = _dbset.AsNoTracking().Where(predicate);
            return objects.Count();
        }
        public virtual Task<int> CountAsync()
        {
            return _dbset.CountAsync();
        }
        public virtual Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            var objects = _dbset.AsNoTracking().Where(predicate);
            return objects.CountAsync();
        }
        public virtual long LongCount()
        {
            return _dbset.LongCount();
        }
        public virtual long LongCount(Expression<Func<T, bool>> predicate)
        {
            var objects = _dbset.AsNoTracking().Where(predicate);
            return objects.LongCount();
        }
        public virtual Task<long> LongCountAsync()
        {
            return _dbset.LongCountAsync();
        }
        public virtual Task<long> LongCountAsync(Expression<Func<T, bool>> predicate)
        {
            var objects = _dbset.AsNoTracking().Where(predicate);
            return objects.LongCountAsync();
        }
        public virtual void Add(T entity)
        {
            _dbset.Add(entity);

        }
        public virtual void Update(T entity)
        {
            _dbset.Attach(entity);
            _dataContext.Entry(entity).State = EntityState.Modified;

        }
        public virtual void Delete(T entity)
        {
            _dbset.Remove(entity);
        }
        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            var objects = _dbset.Where(predicate).FirstOrDefault();
            if (objects != null)
            {
                _dbset.Remove(objects);

            }
        }
        public virtual void AddRange(IEnumerable<T> entities)
        {
            //foreach (var entity in entities)
            //{
            //    _dbset.Add(entity);
            //}
            var enumerable = entities as T[] ?? entities.ToArray();
            _dbset.AddRange(enumerable);

        }
        public virtual void AddRange(params T[] entities)
        {
            _dbset.AddRange(entities);

        }
        public virtual void DeleteRange(params T[] entities)
        {
            _dbset.RemoveRange(entities);

        }
        public virtual void DeleteRange(IEnumerable<T> entities)
        {
            var enumerable = entities as T[] ?? entities.ToArray();
            _dbset.RemoveRange(enumerable);

        }
        public virtual void DeleteRange(Expression<Func<T, bool>> predicate)
        {
            var objects = _dbset.Where(predicate);
            if (objects.Any())
            {
                _dbset.RemoveRange(objects);

            }
        }
        public virtual IQueryable<T> GetAll()
        {
            return _dbset.AsQueryable();
        }
        public virtual IQueryable<T> Get(Expression<Func<T, bool>> predicate)
        {
            return _dbset.Where(predicate);
        }
        public virtual Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate)
        {
            return _dbset.Where(predicate).ToListAsync();
        }
        public virtual DbSet<T> DbSet()
        {
            return _dbset;
        }
    }
}