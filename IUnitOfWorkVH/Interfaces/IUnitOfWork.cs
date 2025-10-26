using Microsoft.EntityFrameworkCore.Storage;
using ResultsVH.Interfaces;

namespace IUnitOfWorkVH.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IDbContextTransaction BeginTransaction();
        Task<IResultBool> SaveChangesAsync(CancellationToken cancellationToken = default);
        IResultBool SaveChanges();
    }
}
