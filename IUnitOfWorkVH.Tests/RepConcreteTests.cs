using Microsoft.EntityFrameworkCore;
using IUnitOfWorkVH.Tests.Models;
using IUnitOfWorkVH.Tests.Concrete;

namespace IUnitOfWorkVH.Tests
{
    public class RepConcreteTests
    {
        [Fact]
        public void RepShopItems_Add_and_Get_work_with_inmemory_db()
        {
            var dbName = "rep_items_add_get_" + Guid.NewGuid();
            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(dbName).Options;

            using var ctx = new TestDbContext(options);
            var repItems = new RepShopItems<TestDbContext>(ctx);
            var repTypes = new RepBaseShopItemTypes<TestDbContext>(ctx);
            var uow = new UnitOfWorkShop<TestDbContext>(ctx, repItems, repTypes, null);

            var item = new ShopItem { Name = "Item1", Price = 9.99m, StockQuantity = 5 };
            repItems.Add(item);

            var save = uow.SaveChanges();
            Assert.True(save.IsSuccess);

            var all = repItems.Get().ToList();
            Assert.Single(all);
            Assert.Equal("Item1", all[0].Name);
        }

        [Fact]
        public void RepShopItems_Remove_removes_entity()
        {
            var dbName = "rep_items_remove_" + Guid.NewGuid();
            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(dbName).Options;

            using var ctx = new TestDbContext(options);
            var repItems = new RepShopItems<TestDbContext>(ctx);
            var repTypes = new RepBaseShopItemTypes<TestDbContext>(ctx);
            var uow = new UnitOfWorkShop<TestDbContext>(ctx, repItems, repTypes, null);

            var item1 = new ShopItem { Name = "ItemA", Price = 1m, StockQuantity = 1 };
            var item2 = new ShopItem { Name = "ItemB", Price = 2m, StockQuantity = 2 };
            repItems.Add(item1);
            repItems.Add(item2);
            Assert.True(uow.SaveChanges().IsSuccess);

            var allBefore = repItems.Get().ToList();
            Assert.Equal(2, allBefore.Count);

            // remove one
            var toRemove = allBefore.First(i => i.Name == "ItemA");
            repItems.Remove(toRemove);
            Assert.True(uow.SaveChanges().IsSuccess);

            var allAfter = repItems.Get().ToList();
            Assert.Single(allAfter);
            Assert.Equal("ItemB", allAfter[0].Name);
        }

        [Fact]
        public void RepShopItems_Get_supports_filter_and_asNoTracking()
        {
            var dbName = "rep_items_get_filter_" + Guid.NewGuid();
            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(dbName).Options;

            using var ctx = new TestDbContext(options);
            var repItems = new RepShopItems<TestDbContext>(ctx);
            var repTypes = new RepBaseShopItemTypes<TestDbContext>(ctx);
            var uow = new UnitOfWorkShop<TestDbContext>(ctx, repItems, repTypes, null);

            repItems.Add(new ShopItem { Name = "Apple", Price = 1m, StockQuantity = 10 });
            repItems.Add(new ShopItem { Name = "Banana", Price = 2m, StockQuantity = 20 });
            Assert.True(uow.SaveChanges().IsSuccess);

            var apples = repItems.Get(filter: s => s.Name == "Apple").ToList();
            Assert.Single(apples);
            Assert.Equal("Apple", apples[0].Name);

            var noTracking = repItems.Get(asNoTracking: true).ToList();
            Assert.Equal(2, noTracking.Count);
        }
    }
}
