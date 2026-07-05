using Microsoft.EntityFrameworkCore.Storage;
using ResultsVH.Interfaces;

namespace IUnitOfWorkVH.Abstractions;

public interface IUnitOfWorkBase : IDisposable
{
    IDbContextTransaction BeginTransaction();
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task<IResultBool> SaveChangesAsync(CancellationToken cancellationToken = default);
    IResultBool SaveChanges();
}
