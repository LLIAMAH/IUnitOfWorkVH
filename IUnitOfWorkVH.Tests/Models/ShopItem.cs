using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IUnitOfWorkVH.Tests.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ShopItem
    {
        public long Id { get; set; }
        [Required, MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        public decimal Price { get; set; }

        public int StockQuantity { get; set; } = 0;

        [ForeignKey(nameof(ShopItemType))]
        public long ShopItemTypeId { get; set; }
        public virtual ShopItemType ShopItemType { get; set; }
    }
}
