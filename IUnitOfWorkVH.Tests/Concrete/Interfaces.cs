using IUnitOfWorkVH.Interfaces;
using IUnitOfWorkVH.Tests.Models;

namespace IUnitOfWorkVH.Tests.Concrete
{
    public interface IRepShopItems : IRep<ShopItem> { }
    public interface IRepShopItemTypes : IRepBase<ShopItemType> { }
}
