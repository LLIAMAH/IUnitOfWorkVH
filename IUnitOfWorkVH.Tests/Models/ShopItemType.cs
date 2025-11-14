using System.ComponentModel.DataAnnotations;

namespace IUnitOfWorkVH.Tests.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class ShopItemType
    {
        public long Id { get; set; }
        [Required, MaxLength(256)]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<ShopItem> ShopItems { get; set; } = new HashSet<ShopItem>();
    }
}
