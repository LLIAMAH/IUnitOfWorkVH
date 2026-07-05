# IUnitOfWorkVH solution

Consist of 2 libraries: 
- IUnitOfWorkVH.Abstractions - contains only interfaces that are needed to implement the Repository and Unit of Work patterns in the application.
- IUnitOfWorkVH - contains the implementation of the Repository and Unit of Work patterns based on the interfaces from the IUnitOfWorkVH.Abstractions library.

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

## IUnitOfWorkVH.Abstractions project

### Description

This library is a set of interfaces that are needed to simplify the formation of Repository and Unit of Work patterns in client applications.

### References

Lib uses:
- ResultVH NuGet package for returning results from the functions.
- Microsoft.EntityFrameworkCore.Relational 10.0 - cause of used IDbContextTransaction which needed for BeginTransaction() method.

### Content for Repositories

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

### Content for UnitOfWork

```
public interface IUnitOfWorkBase : IDisposable
{
    IDbContextTransaction BeginTransaction();
    Task<IResultBool> SaveChangesAsync(CancellationToken cancellationToken = default);
    IResultBool SaveChanges();
}
```

### Usage

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

## IUnitOfWorkVH project

### Description

Define application or class library, which will extend the base of the implemented Unit of Work pattern.
The basic idea of this library is to provide a set of abstractions that can be used in the application to implement the Repository and Unit of Work patterns without having to write boilerplate code.

### References

Lib uses:
- ResultVH NuGet package for returning results from the functions.
- IUnitOfWorkVH.Abstractions - for base interfaces.
- Microsoft.EntityFrameworkCore 10.0 - for EF core functionality.

### Note

Repository pattern implementation in this lib been splitted on 2 separate interfaces:

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

It was done to simplify the implementation of repositories in the application in case if you need only read-only functionality for some entities (for e.g. CQRS architectures):
- Read only repositories can be implemented by using only the IRepBase<T> interface.
- Read/Write repositories can be implemented by using the IRep<T> interface.

### Usage

1. Define application DB context definition base on DbContext 

```
// Entity
public class YourEntity1
{
    public int Id { get; set; }
    public string Name { get; set; }
}

// Entity
public class YourEntity2
{
    public int Id { get; set; }
    public string Description { get; set; }
}


using IUnitOfWorkVH;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    // Define your DbSets here
    public DbSet<YourEntity1> YourEntities1 { get; set; }
    public DbSet<YourEntity2> YourEntities2 { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }
}
```

2. Prepare **local** interfaces and repositories for your entities, which will be used in the application:

```
public interface IRepYourEntity1 : IRepBase<YourEntity1>;

public interface IRepYourEntity2 : IRepBase<YourEntity2>
{
    // place here additional properties or methods if needed
}

```

3. Create a new **local** interface that implements the `IUnitOfWorkBase` interface

```
public interface IUnitOfWork : IUnitOfWorkBase
{
    // dont forget to put your local interfaces here as properties
    IRepYourEntity1 YourEntityRep1 { get; }
    IRepYourEntity2 YourEntityRep2 { get; }

    // place here additional properties or methods if needed
}

```

4. Prepare repository classes: create file/s with interfaces and repositories base on your entities

```
public class RepYourEntity1(ApplicationDbContext context) : RepBase<ApplicationDbContext, YourEntity1>(context), IRepYourEntity1
public class RepYourEntity2(ApplicationDbContext context) : RepBase<ApplicationDbContext, YourEntity2>(context), IRepYourEntity2
```


5. Create class UnitOfWork and inherit from the UnitOfWorkAbstract class and your **local** IUnitOfWork interface

Don't forget initialize base._ctx [Required] and base._logger [Optional] in the constructor

```
public class UnitOfWork : UnitOfWorkAbstract<ApplicationDbContext>, IUnitOfWork
{
    IRepYourEntity1 YourEntityRep1 { get; }
    IRepYourEntity2 YourEntityRep2 { get; }

    public UnitOfWork(ApplicationDbContext context, ILogger<ApplicationDbContext> logger) : base(context)
    {
        /// Required initializations
        this._logger = logger;
        this._ctx = context;

        // Initialize your repositories here
        YourEntityRep1 = new RepYourEntity1(context);
        YourEntityRep2 = new RepYourEntity2(context);
    }

    // Implement additional methods or properties if needed
    // Define additional methods or properties if needed  
}
```

6. Enjoy of use of the UnitOfWork class in your application.

### Added few **protected virtual** methods for SaveChanges/SaveChangesAsync functions

Added functions providing fluent Pre-/Postfix calls, which will allow to configure flexible calls before and after data saved in DB. 
They are empty by default.

* For SaveChanges()
    * BeforeSave()
    * AfterSave()

```
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
```

* For SaveChangesAsync(...)
    * BeforeSaveAsync(...)
    * AfterSaveAsync(...)

```
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
```

In case you need override the default behavior of these functions - keep in mind, that **in minimal** implementation is required override only:
* BeforeSave() and AfterSave()

The async functions are calling the BeforeSave() and AfterSave() function by default - is was done to simplify overriding. 

```
#region Virtual Methods
protected virtual void BeforeSave() { }
protected virtual Task BeforeSaveAsync(CancellationToken cancellationToken = default)
{
    this.BeforeSave();
    return Task.CompletedTask;
}

protected virtual void AfterSave() { }
protected virtual Task AfterSaveAsync(CancellationToken cancellationToken = default)
{
    this.AfterSave();
    return Task.CompletedTask;
}
#endregion
```

Don't forget to register the UnitOfWork class in the DI container of your application.
