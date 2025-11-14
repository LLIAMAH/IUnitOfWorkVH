using Microsoft.EntityFrameworkCore;
using IUnitOfWorkVH.Implementations;
using IUnitOfWorkVH.Interfaces;

namespace IUnitOfWorkVH.Tests
{
    public class RepTests
    {
        [Fact]
        public void Rep_generic_types_can_be_constructed()
        {
            var repBaseType = typeof(RepBase<,>);
            var repType = typeof(Rep<,>);

            var constructedRepBase = repBaseType.MakeGenericType(typeof(DbContext), typeof(object));
            var constructedRep = repType.MakeGenericType(typeof(DbContext), typeof(object));

            Assert.NotNull(constructedRepBase);
            Assert.NotNull(constructedRep);

            Assert.True(constructedRep.IsClass);
            Assert.True(constructedRepBase.IsClass);
        }

        [Fact]
        public void Rep_implements_IRep()
        {
            var repType = typeof(Rep<,>).MakeGenericType(typeof(DbContext), typeof(object));
            var interfaces = repType.GetInterfaces();
            Assert.Contains(interfaces, t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IRep<>));
        }
    }
}
