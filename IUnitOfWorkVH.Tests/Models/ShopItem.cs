using System.ComponentModel.DataAnnotations;

namespace IUnitOfWorkVH.Tests.Models
{
    public class ShopItem
    {
        public long Id { get; set; }
        [Required, MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; } = 0;
    }
}
