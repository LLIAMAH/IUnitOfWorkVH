using IUnitOfWorkVH.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IUnitOfWorkVH.Implementations
{
    public abstract class AbstractUnitOfWork<T> : IUnitOfWork where T : DbContext
    {
        protected T _ctx;
        protected Logger<T> _logger;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = this._ctx.SaveChangesAsync(cancellationToken);
                return result;
            }
            catch(Exception ex)
            {
                this._logger?.LogError(ex, ex.Message);
                return Task.FromResult(0);
            }
        }

        public int SaveChanges()
        {
            try
            {
                var result = this._ctx.SaveChanges();
                return result;
            }
            catch (Exception ex)
            {
                this._logger?.LogError(ex, ex.Message);
                return 0;
            }
        }

        public void Dispose()
        {
            this._ctx?.Dispose();
        }
    }
}
