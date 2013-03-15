using System.Diagnostics;
using EFSpecs.Reflection;

namespace EFSpecs
{
    public class Property
    {
        private readonly PropertyAccessor _accessor;
        private object ExpectedValue { get; set; }

        public Property(PropertyAccessor accessor, object expectedValue)
        {
            _accessor = accessor;
            ExpectedValue = expectedValue;
        }

        public void SetValue(object entity)
        {
            _accessor.SetValue(entity, ExpectedValue);
        }

        public void AssertValue(object entity)
        {
            var actualValue = _accessor.GetValue(entity);
            Debug.Assert(actualValue.Equals(ExpectedValue),
                         string.Format("Assertion failed! Expected: {0} Actual: {1}", ExpectedValue, actualValue));
        }
    }
}