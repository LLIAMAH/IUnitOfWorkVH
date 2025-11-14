using Microsoft.EntityFrameworkCore;
using IUnitOfWorkVH.Interfaces;
using IUnitOfWorkVH.Implementations;
using Xunit;

namespace IUnitOfWorkVH.Tests
{
    public class ReflectionTests
    {
        [Fact]
        public void Interfaces_Exist_With_Expected_Members()
        {
            var iRepBase = typeof(IRepBase<>);
            Assert.True(iRepBase.IsInterface);

            var getMethod = iRepBase.GetMethod("Get");
            Assert.NotNull(getMethod);
            Assert.Equal("Get", getMethod.Name);
            Assert.True(getMethod.ReturnType.IsGenericType);
        }

        [Fact]
        public void IRep_Extends_IRepBase_And_Has_Add_Remove()
        {
            var irep = typeof(IRep<>);
            Assert.True(irep.IsInterface);

            var interfaces = irep.GetInterfaces();
            Assert.Contains(interfaces, t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IRepBase<>));

            var add = irep.GetMethod("Add");
            var remove = irep.GetMethod("Remove");
            Assert.NotNull(add);
            Assert.NotNull(remove);
        }

        [Fact]
        public void IUnitOfWorkBase_Has_Expected_Methods()
        {
            var uow = typeof(IUnitOfWorkBase);
            Assert.True(uow.IsInterface);
            Assert.NotNull(uow.GetMethod("BeginTransaction"));
            Assert.NotNull(uow.GetMethod("SaveChangesAsync"));
            Assert.NotNull(uow.GetMethod("SaveChanges"));
        }

        [Fact]
        public void RepBase_And_Rep_Types_Exist()
        {
            var repBase = typeof(RepBase<,>);
            var rep = typeof(Rep<,>);
            Assert.True(repBase.IsClass);
            Assert.True(rep.IsClass);

            var constructedRep = rep.MakeGenericType(typeof(DbContext), typeof(object));
            Assert.NotNull(constructedRep);

            var implemented = constructedRep.GetInterfaces();
            Assert.Contains(implemented, t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IRep<>));
        }

        [Fact]
        public void UnitOfWorkBaseAbstract_Implements_IUnitOfWorkBase()
        {
            var uowBase = typeof(UnitOfWorkBaseAbstract<>);
            Assert.True(uowBase.IsClass);

            var g = uowBase.MakeGenericType(typeof(DbContext));
            Assert.Contains(g.GetInterfaces(), t => t == typeof(IUnitOfWorkBase));

            Assert.NotNull(g.GetMethod("SaveChanges"));
            Assert.NotNull(g.GetMethod("SaveChangesAsync"));
            Assert.NotNull(g.GetMethod("BeginTransaction"));
        }
    }
}
