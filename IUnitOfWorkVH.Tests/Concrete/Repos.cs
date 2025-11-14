using Microsoft.EntityFrameworkCore;
using IUnitOfWorkVH.Implementations;
using IUnitOfWorkVH.Tests.Models;

namespace IUnitOfWorkVH.Tests.Concrete
{
    public class RepShopItems<TCtx>(TCtx ctx) : Rep<TCtx, ShopItem>(ctx), IRepShopItems
        where TCtx : DbContext;

    public class RepBaseShopItemTypes<TCtx>(TCtx ctx) : RepBase<TCtx, ShopItemType>(ctx), IRepShopItemTypes
        where TCtx : DbContext;
}
