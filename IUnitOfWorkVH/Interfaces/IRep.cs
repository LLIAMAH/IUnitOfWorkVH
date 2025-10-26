using System.Linq.Expressions;

namespace IUnitOfWorkVH.Interfaces
{
    public interface IRepBase<T> where T : class
    {
        IQueryable<T> Get(Expression<Func<T, bool>>? filter = null, string? include = null, bool asNoTracking = false);
    }

    public interface IRep<T> : IRepBase<T> where T : class
    {
        void Add(T entity);
        void Remove(T entity);
    }
}
