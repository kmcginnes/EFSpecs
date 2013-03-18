using System.Data.Entity;
using EFSpecs.Reflection;

namespace EFSpecs
{
    public class Property : IEntityValueAsserter
    {
        private readonly PropertyAccessor _accessor;
        private readonly object _expectedValue;

        public Property(PropertyAccessor accessor, object expectedValue)
        {
            _accessor = accessor;
            _expectedValue = expectedValue;
        }

        public void SetValue(object entity)
        {
            _accessor.SetValue(entity, _expectedValue);
        }

        public void AssertValue(DbContext ctx, object entity)
        {
            var actualValue = _accessor.GetValue(entity);
            if (!_expectedValue.Equals(actualValue))
            {
                throw new AssertionException(_expectedValue.ToString(), actualValue.ToString());
            }
        }
    }
}