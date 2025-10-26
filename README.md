# IUnitOfWorkVH

## Description

Define application or class library, which will extend the base of the implemented Unit of Work pattern.

- Include to the project NuGetPackage: **IUnitOfWorkVH** latest version

## Usage

### Application context fdefinition base on DbContext 

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

### Creete files with interfaces and repositories base on your entities
```
using IUnitOfWorkVH;

/// this repository will have only Get method - perfect for classifiers
public interface IRepYourEntity1 : IRepBase<YourEntity1>
{
    // Define additional methods if needed
}

/// this repository will have Get/Add/Remove methods - perfect for general purpose entities
public interface IRepYourEntity2 : IRep<YourEntity2>
{
    // Define additional methods if needed
}

///.....

public class RepYourEntity1(ApplicationDbContext context) : RepBase<YourEntity1>(context), IRepYourEntity1
public class RepYourEntity2(ApplicationDbContext context) : RepBase<YourEntity2>(context), IRepYourEntity2
```

### Define local interface IUnitOfWork and inherit from the IUnitOfWorkBase interface

```
using IUnitOfWorkVH;
public interface IUnitOfWork : IUnitOfWorkBase<ApplicationDbContext>
{
    // Define additional methods or properties if needed
    IRepYourEntity1 YourEntityRep1 { get; }
    IRepYourEntity2 YourEntityRep2 { get; }
}
```

### Create class UnitOfWork and inherit from the UnitOfWorkAbstract class and your local IUnitOfWork interface

Dont forget initialize base._ctx and base._logger in the constructor

```
public class UnitOfWork : UnitOfWorkAbstract<ApplicationDbContext>
{
    public UnitOfWork(ApplicationDbContext context, ILogger<UnitOfWork> logger) : base(context)
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

Don't forget to register the UnitOfWork class in the DI container of your application.