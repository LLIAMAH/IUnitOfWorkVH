using Microsoft.EntityFrameworkCore;
using IUnitOfWorkVH.Tests.Models;

namespace IUnitOfWorkVH.Tests
{
    public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public DbSet<ShopItem> ShopItems { get; set; } = null!;
        public DbSet<ShopItemType> ShopItemTypes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Ensure EF knows about the entities (DbSet properties are enough, but keep this for clarity)
            modelBuilder.Entity<ShopItem>();
            modelBuilder.Entity<ShopItemType>();
        }
    }
}
