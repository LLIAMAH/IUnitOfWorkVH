using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using ResultsVH.Interfaces;
using Xunit;
using IUnitOfWorkVH.Implementations;
using IUnitOfWorkVH.Interfaces;
using IUnitOfWorkVH.Tests.Models;
using IUnitOfWorkVH.Tests.Concrete;

namespace IUnitOfWorkVH.Tests
{
    public class UnitOfWorkConcreteTests
    {
        [Fact]
        public void UnitOfWorkShop_initializes_properties()
        {
            var mockCtx = new Mock<DbContext>(new DbContextOptions<DbContext>());
            var mockRepsShopItems = new Mock<IRepShopItems>();
            var mockRepsShopItemTypes = new Mock<IRepShopItemTypes>();

            var uow = new UnitOfWorkShop<DbContext>(mockCtx.Object, mockRepsShopItems.Object, mockRepsShopItemTypes.Object, NullLogger<DbContext>.Instance);

            Assert.Same(mockRepsShopItems.Object, uow.RepShopItems);
            Assert.Same(mockRepsShopItemTypes.Object, uow.RepShopItemTypes);
        }

        [Fact]
        public async Task SaveChangesAsync_returns_result_success_when_save_succeeds()
        {
            var options = new DbContextOptionsBuilder<DbContext>().UseInMemoryDatabase("uow_async_success").Options;
            using var ctx = new DbContext(options);
            var repShopItems = new RepShopItems<DbContext>(ctx);
            var repShopItemTypes = new RepBaseShopItemTypes<DbContext>(ctx);

            var uow = new UnitOfWorkShop<DbContext>(ctx, repShopItems, repShopItemTypes, NullLogger<DbContext>.Instance);

            var result = await uow.SaveChangesAsync(CancellationToken.None);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public void SaveChanges_returns_result_success_when_save_succeeds()
        {
            var options = new DbContextOptionsBuilder<DbContext>().UseInMemoryDatabase("uow_sync_success").Options;
            using var ctx = new DbContext(options);
            var repShopItems = new RepShopItems<DbContext>(ctx);
            var repShopItemTypes = new RepBaseShopItemTypes<DbContext>(ctx);

            var uow = new UnitOfWorkShop<DbContext>(ctx, repShopItems, repShopItemTypes, NullLogger<DbContext>.Instance);

            var result = uow.SaveChanges();
            Assert.True(result.IsSuccess);
        }
    }
}
