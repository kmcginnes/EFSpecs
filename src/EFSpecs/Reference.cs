using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using EFSpecs.Mapping;
using EFSpecs.Reflection;

namespace EFSpecs
{
    public class Reference
    {
        public PropertyAccessor PropertyInfo { get; private set; }
        public object ExpectedEntity { get; private set; }

        public Reference(PropertyAccessor propertyInfo, object value)
        {
            PropertyInfo = propertyInfo;
            ExpectedEntity = value;
        }

        public void SetValue(object instance)
        {
            PropertyInfo.SetValue(instance, ExpectedEntity);
        }

        public void AssertValue(DbContext ctx, object actual)
        {
            var propertyType = PropertyInfo.MemberType;

            var objectContext = ((IObjectContextAdapter)ctx).ObjectContext;
            var entitySet = objectContext.GetEntitySet(propertyType);
            var keyMembers = entitySet.ElementType.KeyMembers;

            var actualEntity = PropertyInfo.GetValue(actual);
            foreach (var keyMember in keyMembers)
            {
                var accessor = new PropertyAccessor(propertyType.GetProperty(keyMember.Name));

                var actualKeyValue = accessor.GetValue(actualEntity);
                var expectedKeyValue = accessor.GetValue(ExpectedEntity);

                Debug.Assert(actualKeyValue.Equals(expectedKeyValue),
                             string.Format("Assertion failed! Expected: {0} Actual: {1}", ExpectedEntity, actualEntity));
            }
        }
    }
}