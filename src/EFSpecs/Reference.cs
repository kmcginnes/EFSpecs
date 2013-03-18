using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using EFSpecs.Mapping;
using EFSpecs.Reflection;
using ServiceStack.Text;

namespace EFSpecs
{
    public class Reference : IEntityValueAsserter
    {
        public PropertyAccessor Accessor { get; private set; }
        public object ExpectedEntity { get; private set; }

        public Reference(PropertyAccessor propertyInfo, object value)
        {
            Accessor = propertyInfo;
            ExpectedEntity = value;
        }

        public void SetValue(object instance)
        {
            Accessor.SetValue(instance, ExpectedEntity);
        }

        public void AssertValue(DbContext ctx, object actual)
        {
            var propertyType = Accessor.MemberType;

            var objectContext = ((IObjectContextAdapter)ctx).ObjectContext;
            var entitySet = objectContext.GetEntitySet(propertyType);
            var keyMembers = entitySet.ElementType.KeyMembers;

            ctx.Entry(actual).Reference(Accessor.Name).Load();

            var actualEntity = Accessor.GetValue(actual);
            if (actualEntity == null)
            {
                throw new AssertionException(ExpectedEntity.Dump(), "NULL");
            }
            foreach (var keyMember in keyMembers)
            {
                var accessor = new PropertyAccessor(propertyType.GetProperty(keyMember.Name));

                var actualKeyValue = accessor.GetValue(actualEntity);
                var expectedKeyValue = accessor.GetValue(ExpectedEntity);

                if (!expectedKeyValue.Equals(actualKeyValue))
                {
                    throw new AssertionException(ExpectedEntity.Dump(), actualEntity.Dump());
                }
            }
        }
    }
}