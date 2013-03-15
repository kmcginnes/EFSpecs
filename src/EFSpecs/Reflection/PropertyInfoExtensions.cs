using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace EFSpecs.Reflection
{
    public static class PropertyInfoExtensions
    {
        public static PropertyAccessor ToAccessor(this PropertyInfo propertyInfo)
        {
            return new PropertyAccessor(propertyInfo);
        }
    }
}
