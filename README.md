# IUnitOfWorkVH

## Description

Define application or class library, which will extend the base of the implemented Unit of Work pattern.

- Include to the project NuGetPackage: **IUnitOfWorkVH** latest version

## Usage

1. Define application DB context definition base on DbContext 

```
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

2. Create files with interfaces and repositories base on your entities
```
using IUnitOfWorkVH;

/// this repository will have only Get method - perfect for classifiers
public interface IRepYourEntity1 : IRepBase<YourEntity1>
{
    // Define additional methods if needed
}

/// this repository will have Get/Add/Remove methods - for general purpose entities
public interface IRepYourEntity2 : IRep<YourEntity2>
{
    // Define additional methods if needed
}

///.....

public class RepYourEntity1(ApplicationDbContext context) : RepBase<ApplicationDbContext, YourEntity1>(context), IRepYourEntity1
public class RepYourEntity2(ApplicationDbContext context) : RepBase<ApplicationDbContext, YourEntity2>(context), IRepYourEntity2
```

3. Define local interface IUnitOfWork and inherit from the IUnitOfWorkBase interface

```
using IUnitOfWorkVH;
public interface IUnitOfWork : IUnitOfWorkBase<ApplicationDbContext>
{
    // Define additional methods or properties if needed
    IRepYourEntity1 YourEntityRep1 { get; }
    IRepYourEntity2 YourEntityRep2 { get; }
}
```

4. Create class UnitOfWork and inherit from the UnitOfWorkAbstract class and your local IUnitOfWork interface

Don't forget initialize base._ctx [Required] and base._logger [Optional] in the constructor

```
public class UnitOfWork : UnitOfWorkAbstract<ApplicationDbContext>
{
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
    IRepYourEntity1 YourEntityRep1 { get; }
    IRepYourEntity2 YourEntityRep2 { get; }
}
```

## Added few **protected virtual** methods for SaveChanges/SaveChangesAsync functions

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
