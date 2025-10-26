using IUnitOfWorkVH.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using ResultsVH.Implementations;
using ResultsVH.Interfaces;

namespace IUnitOfWorkVH.Implementations
{
    public abstract class UnitOfWorkBaseAbstract<T> : IUnitOfWorkBase where T : DbContext
    {
        // ReSharper disable once UnassignedField.Global
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        protected T _ctx;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        // ReSharper disable once UnassignedField.Global
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        protected ILogger<T> _logger;
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.

        public IDbContextTransaction BeginTransaction()
        {
            return this._ctx.Database.BeginTransaction();
        }

        public async Task<IResultBool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await this._ctx.SaveChangesAsync(cancellationToken);
                return new ResultBool(true);
            }
            catch (Exception ex)
            {
                this._logger?.LogError(ex, ex.Message);
                return new ResultBool(ex.Message);
            }
        }

        public IResultBool SaveChanges()
        {
            try
            {
                var result = this._ctx.SaveChanges();
                return new ResultBool(true);
            }
            catch (Exception ex)
            {
                this._logger?.LogError(ex, ex.Message);
                return new ResultBool(ex.Message);
            }
        }

        public void Dispose()
        {
            this._ctx?.Dispose();
        }
    }
}
