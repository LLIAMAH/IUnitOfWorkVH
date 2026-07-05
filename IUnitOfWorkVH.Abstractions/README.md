# IUnitOfWorkVH

## Description

This library is a set of interfaces that are needed to simplify the formation of Repository and Unit of Work patterns in client applications.

## References

Lib uses:
- ResultVH NuGet package for returning results from the functions.
- Microsoft.EntityFrameworkCore.Relational 10.0 - for separate BeginTransaction() method.

## Content for Repositories

```
public interface IRepBase<T> where T : class
{
    IQueryable<T> Get(Expression<Func<T, bool>>? filter = null, string? include = null, bool asNoTracking = false);
}

public interface IRep<T> : IRepBase<T> where T : class
{
    void Add(T entity);
    void Remove(T entity);
}
```

## Content for UnitOfWork

```
public interface IUnitOfWorkBase : IDisposable
{
    IDbContextTransaction BeginTransaction();
    Task<IResultBool> SaveChangesAsync(CancellationToken cancellationToken = default);
    IResultBool SaveChanges();
}
```

## Usage

1. Create local repository interfaces and classes based on your entities

```
// Entity
public class YourEntity1
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public interface IRepYourEntity1 : IRepBase<YourEntity1>;

// or

public interface IRepYourEntity1 : IRepBase<YourEntity1>
{
    // place here additional properties or methods if needed
}

```

2. Create a new local interface that implements the `IUnitOfWorkBase` interface
```
public interface IUnitOfWork : IUnitOfWorkBase
{
    // dont forget to put your local interfaces here as properties
    IRepYourEntity1 YourEntityRep1 { get; }

    // place here additional properties or methods if needed
}

```

3. Use prepared abstractions in your local UnitOfWork class (see details of description in the README of the IUnitOfWorkVH package)

Don't forget to register the local IUnitOfWork interface in the DI container of your application.
