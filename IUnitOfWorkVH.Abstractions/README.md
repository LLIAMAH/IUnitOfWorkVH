# IUnitOfWorkVH.Abstractions

## Description

This library is a set of interfaces that are needed to simplify the formation of Repository and Unit of Work patterns in client applications.

The basic project IUnitOfWorkVH been splitted to 2 separate libraries:
- IUnitOfWorkVH.Abstractions - contains only interfaces that are needed to implement the Repository and Unit of Work patterns in the application.
- IUnitOfWorkVH - contains the implementation of the Repository and Unit of Work patterns based on the interfaces from the IUnitOfWorkVH.Abstractions library.

It was done to provide flexible usage in Clean Architecture, where you can use only the interfaces from the IUnitOfWorkVH.Abstractions library in many projects/layers of your solution, but implement the full IUnitOfWorkVH library only in the infrastructure layer of your application.

E.g:
- Projects/layers of your solution that can use only the interfaces from the IUnitOfWorkVH.Abstractions library:
  - Application layer - ref to *.Abstractions if needed
  - Domain layer - ref to *.Abstractions if needed
  - Infrastucture layer - ref to full IUnitOfWorkVH
  - etc.

## References

Lib uses:
- ResultVH NuGet package for returning results from the functions.
- Microsoft.EntityFrameworkCore.Relational 10.0 - cause of used IDbContextTransaction which needed for BeginTransaction() method.

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
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
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
