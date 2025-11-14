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
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
        public virtual ShopItemType ShopItemType { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    }
}
