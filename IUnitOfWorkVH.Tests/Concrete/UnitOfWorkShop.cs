using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using IUnitOfWorkVH.Interfaces;

namespace IUnitOfWorkVH.Tests.Concrete
{
    public class UnitOfWorkShop<TCtx> : IUnitOfWorkVH.Implementations.UnitOfWorkBaseAbstract<TCtx>, IUnitOfWorkBase
        where TCtx : DbContext
    {
        public IRepShopItems RepShopItems { get; init; }
        public IRepShopItemTypes RepShopItemTypes { get; init; }

        public UnitOfWorkShop(TCtx ctx, IRepShopItems repShopItems, IRepShopItemTypes repShopItemTypes, ILogger<TCtx>? logger = null)
        {
            _ctx = ctx;
            _logger = logger;
            RepShopItems = repShopItems;
            RepShopItemTypes = repShopItemTypes;
        }
    }
}
