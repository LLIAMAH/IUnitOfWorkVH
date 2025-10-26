using System.Linq.Expressions;
using IUnitOfWorkVH.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IUnitOfWorkVH.Implementations
{
    public class RepBase<TCtx, T>(TCtx ctx) : IRepBase<T>
        where T : class
        where TCtx : DbContext
    {
        protected readonly TCtx _ctx = ctx;
        protected readonly DbSet<T> _dbSet = ctx.Set<T>();

        public IQueryable<T> Get(Expression<Func<T, bool>>? filter = null, string? include = null,
            bool asNoTracking = false)
        {
            IQueryable<T> query = _dbSet;
            if (asNoTracking)
                query = query.AsNoTracking();

            if (!string.IsNullOrEmpty(include))
                query = include.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Aggregate(query, (current, inc) => current.Include(inc.Trim()));

            if (filter != null)
                query = query.Where(filter);

            return query;
        }
    }

    public class Rep<TCtx, T>(TCtx ctx) : RepBase<TCtx, T>(ctx), IRep<T>
        where T : class
        where TCtx : DbContext
    {
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
