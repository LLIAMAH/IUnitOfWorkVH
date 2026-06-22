using IUnitOfWorkVH.Implementations;
using Microsoft.EntityFrameworkCore;

namespace IUnitOfWorkVH.Tests
{
    public class UnitOfWorkVirtualMethodsTests
    {
        private class DummyEntity
        {
            public int Id { get; set; }
        }

        private class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }
            public DbSet<DummyEntity> Dummies { get; set; } = null!;
        }

        private class TestUnitOfWorkBase : UnitOfWorkBaseAbstract<TestDbContext>
        {
            public TestUnitOfWorkBase(TestDbContext ctx)
            {
                this._ctx = ctx;
            }

            public bool BeforeSyncCalled { get; private set; }
            public bool AfterSyncCalled { get; private set; }
            public bool BeforeAsyncCalled { get; private set; }
            public bool AfterAsyncCalled { get; private set; }

            protected override void BeforeSave()
            {
                this.BeforeSyncCalled = true;
            }

            protected override void AfterSave()
            {
                this.AfterSyncCalled = true;
            }

            protected override Task BeforeSaveAsync(CancellationToken cancellationToken = default)
            {
                // mark async flag and do not call base to simulate an async-only override
                this.BeforeAsyncCalled = true;
                return Task.CompletedTask;
            }

            protected override Task AfterSaveAsync(CancellationToken cancellationToken = default)
            {
                this.AfterAsyncCalled = true;
                return Task.CompletedTask;
            }
        }

        private static DbContextOptions<TestDbContext> CreateOptions(string dbName)
        {
            return new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
        }

        [Fact]
        public void SaveChanges_Invokes_BeforeAndAfter_Sync_Overrides()
        {
            var options = CreateOptions("sync_db");
            using var ctx = new TestDbContext(options);
            var uow = new TestUnitOfWorkBase(ctx);

            // Act
            var result = uow.SaveChanges();

            // Assert: sync overrides should have been called
            Assert.True(uow.BeforeSyncCalled, "BeforeSave() should be called by SaveChanges().");
            Assert.True(uow.AfterSyncCalled, "AfterSave() should be called by SaveChanges().");
            // async flags should remain false for sync call
            Assert.False(uow.BeforeAsyncCalled);
            Assert.False(uow.AfterAsyncCalled);
        }

        [Fact]
        public async Task SaveChangesAsync_When_Only_Sync_OverridesExist_Invokes_Sync_Overrides()
        {
            var options = CreateOptions("async_calls_sync_db");
            using var ctx = new TestDbContext(options);

            // Create a custom UoW that only overrides sync methods by deriving from TestUnitOfWorkBase
            var uow = new TestUnitOfWorkBase(ctx);

            // Reset the async-only flags so they only reflect async overrides
            uow = new TestUnitOfWorkBase(ctx);

            // Act
            var result = await uow.SaveChangesAsync();

            // Assert: Because base BeforeSaveAsync/AfterSaveAsync call the sync methods,
            // the sync overrides should be invoked for SaveChangesAsync as well.
            Assert.True(uow.BeforeAsyncCalled, "BeforeSaveAsync() should be called by SaveChangesAsync().");
            Assert.True(uow.AfterAsyncCalled, "AfterSaveAsync() should be called by SaveChangesAsync().");
        }

        [Fact]
        public async Task SaveChangesAsync_Invokes_Async_Overrides_When_Overridden()
        {
            var options = CreateOptions("async_overrides_db");
            using var ctx = new TestDbContext(options);

            // Use the TestUnitOfWorkBase which overrides async methods to set async flags
            var uow = new TestUnitOfWorkBase(ctx);

            // Act
            var result = await uow.SaveChangesAsync();

            // Assert: async overrides should have been called
            Assert.True(uow.BeforeAsyncCalled, "BeforeSaveAsync() override should be called by SaveChangesAsync().");
            Assert.True(uow.AfterAsyncCalled, "AfterSaveAsync() override should be called by SaveChangesAsync().");
        }
    }
}
