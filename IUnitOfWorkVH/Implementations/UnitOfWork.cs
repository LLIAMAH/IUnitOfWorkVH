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
        protected ILogger<T>? _logger = null;

        public IDbContextTransaction BeginTransaction()
        {
            return this._ctx.Database.BeginTransaction();
        }

        public async Task<IResultBool> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await this.BeforeSaveAsync(cancellationToken);
                await this._ctx.SaveChangesAsync(cancellationToken);
                await this.AfterSaveAsync(cancellationToken);

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
                this.BeforeSave();
                this._ctx.SaveChanges();
                this.AfterSave();

                return new ResultBool(true);
            }
            catch (Exception ex)
            {
                this._logger?.LogError(ex, ex.Message);
                return new ResultBool(ex.Message);
            }
        }

        #region Virtual Methods

        protected virtual void BeforeSave() { }

        protected virtual Task BeforeSaveAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        protected virtual void AfterSave() { }

        protected virtual Task AfterSaveAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region IDisposable Support

        public void Dispose()
        {
            this._logger = null;
            this._ctx?.Dispose();
        }

        #endregion
    }
}
