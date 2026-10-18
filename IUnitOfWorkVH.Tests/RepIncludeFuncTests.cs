using Microsoft.EntityFrameworkCore;
using IUnitOfWorkVH.Tests.Models;
using IUnitOfWorkVH.Tests.Concrete;

namespace IUnitOfWorkVH.Tests
{
    public class RepIncludeFuncTests
    {
        [Fact]
        public void Get_with_include_func_includes_navigation_property()
        {
            var dbName = "rep_include_nav_" + Guid.NewGuid();
            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(dbName).Options;

            using var ctx = new TestDbContext(options);
            var repItems = new RepShopItems<TestDbContext>(ctx);

            var type = new ShopItemType { Name = "Food" };
            ctx.ShopItemTypes.Add(type);
            var item = new ShopItem { Name = "Apple", Price = 1m, StockQuantity = 10, ShopItemType = type };
            repItems.Add(item);
            ctx.SaveChanges();

            var result = repItems.Get(
                queryPredicate: q => q.Include(i => i.ShopItemType)).ToList();
            Assert.Single(result);
            Assert.NotNull(result[0].ShopItemType);
            Assert.Equal("Food", result[0].ShopItemType.Name);
        }

        [Fact]
        public void Get_with_filter_and_include_func_applies_both()
        {
            var dbName = "rep_include_filter_" + Guid.NewGuid();
            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(dbName).Options;

            using var ctx = new TestDbContext(options);
            var repItems = new RepShopItems<TestDbContext>(ctx);

            var type = new ShopItemType { Name = "Tool" };
            ctx.ShopItemTypes.Add(type);
            repItems.Add(new ShopItem { Name = "Hammer", Price = 10m, StockQuantity = 5, ShopItemType = type });
            repItems.Add(new ShopItem { Name = "Nail", Price = 0.1m, StockQuantity = 100, ShopItemType = type });
            ctx.SaveChanges();

            var hammers = repItems
                .Get(queryPredicate: q =>
                    q.Include(i => i.ShopItemType)
                        .Where(s => s.Name == "Hammer"))
                .ToList();
            Assert.Single(hammers);
            Assert.Equal("Hammer", hammers[0].Name);
            Assert.NotNull(hammers[0].ShopItemType);
            Assert.Equal("Tool", hammers[0].ShopItemType.Name);
        }

        [Fact]
        public void Get_with_asNoTracking_and_include_func_still_includes_navigation()
        {
            var dbName = "rep_include_no_tracking_" + Guid.NewGuid();
            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(dbName).Options;

            using var ctx = new TestDbContext(options);
            var repItems = new RepShopItems<TestDbContext>(ctx);

            var type = new ShopItemType { Name = "Drink" };
            ctx.ShopItemTypes.Add(type);
            repItems.Add(new ShopItem { Name = "Water", Price = 0.5m, StockQuantity = 50, ShopItemType = type });
            ctx.SaveChanges();

            var list = repItems.Get(
                    queryPredicate: q => q.Include(i => i.ShopItemType),
                    asNoTracking: true)
                .ToList();
            Assert.Single(list);
            Assert.NotNull(list[0].ShopItemType);
            Assert.Equal("Drink", list[0].ShopItemType.Name);
        }

        [Fact]
        public void Get_with_include_func_can_apply_ordering()
        {
            var dbName = "rep_include_order_" + Guid.NewGuid();
            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(dbName).Options;

            using var ctx = new TestDbContext(options);
            var repItems = new RepShopItems<TestDbContext>(ctx);

            var type = new ShopItemType { Name = "Misc" };
            ctx.ShopItemTypes.Add(type);
            repItems.Add(new ShopItem { Name = "A", Price = 1m, StockQuantity = 1, ShopItemType = type });
            repItems.Add(new ShopItem { Name = "B", Price = 5m, StockQuantity = 1, ShopItemType = type });
            repItems.Add(new ShopItem { Name = "C", Price = 3m, StockQuantity = 1, ShopItemType = type });
            ctx.SaveChanges();

            var ordered = repItems
                .Get(queryPredicate: q => q.OrderByDescending(i => i.Price)
                    .Include(i => i.ShopItemType))
                .ToList();
            Assert.Equal(3, ordered.Count);
            Assert.Equal("B", ordered[0].Name);
            Assert.Equal("C", ordered[1].Name);
            Assert.Equal("A", ordered[2].Name);
        }

        [Fact]
        public void Get_with_include_func_can_apply_ordering_and_where()
        {
            var dbName = "rep_include_order_where_" + Guid.NewGuid();
            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(dbName).Options;

            using var ctx = new TestDbContext(options);
            var repItems = new RepShopItems<TestDbContext>(ctx);

            var type = new ShopItemType { Name = "Misc" };
            ctx.ShopItemTypes.Add(type);
            repItems.Add(new ShopItem { Name = "A", Price = 1m, StockQuantity = 1, ShopItemType = type });
            repItems.Add(new ShopItem { Name = "B", Price = 5m, StockQuantity = 1, ShopItemType = type });
            repItems.Add(new ShopItem { Name = "C", Price = 3m, StockQuantity = 1, ShopItemType = type });
            ctx.SaveChanges();

            var ordered = repItems.Get(queryPredicate: q =>
                q.OrderByDescending(i => i.Price)
                    .Include(i => i.ShopItemType)
                    .Where(o => o.Name == "A")).ToList();
            Assert.Single(ordered);
            Assert.Equal("A", ordered[0].Name);
        }

        [Fact]
        public void Get_with_include_func_where_and_filter_combines_predicates()
        {
            var dbName = "rep_include_where_filter_" + Guid.NewGuid();
            var options = new DbContextOptionsBuilder<TestDbContext>().UseInMemoryDatabase(dbName).Options;

            using var ctx = new TestDbContext(options);
            var repItems = new RepShopItems<TestDbContext>(ctx);

            var type = new ShopItemType { Name = "Combo" };
            ctx.ShopItemTypes.Add(type);
            repItems.Add(new ShopItem { Name = "Keep", Price = 10m, StockQuantity = 2, ShopItemType = type });
            repItems.Add(new ShopItem { Name = "Drop", Price = 1m, StockQuantity = 2, ShopItemType = type });
            ctx.SaveChanges();

            // include function applies an additional where (price > 5)
            var res = repItems
                .Get(queryPredicate: q =>
                    q.Where(s => s.Price > 5)
                        .Include(i => i.ShopItemType))
                .ToList();
            Assert.Single(res);
            Assert.Equal("Keep", res[0].Name);
        }
    }
}
