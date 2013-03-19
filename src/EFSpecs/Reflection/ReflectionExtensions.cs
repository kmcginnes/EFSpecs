using System;
using System.Collections.Generic;
using System.Reflection;

namespace EFSpecs.Reflection
{
    public static class ReflectionExtensions
    {
        static readonly Dictionary<Type, Dictionary<string, PropertyAccessor>> PropertyAccessorCache =
            new Dictionary<Type, Dictionary<string, PropertyAccessor>>();
        
        public static PropertyAccessor ToAccessor(this PropertyInfo propertyInfo)
        {
            return new PropertyAccessor(propertyInfo);
        }

        public static PropertyAccessor GetPropertyAccessor(this Type type, string propertyName)
        {
            if (!PropertyAccessorCache.ContainsKey(type))
            {
                PropertyAccessorCache[type] = new Dictionary<string, PropertyAccessor>();
            }

            if (!PropertyAccessorCache[type].ContainsKey(propertyName))
            {
                PropertyAccessorCache[type][propertyName] = 
                    type.GetProperty(propertyName).ToAccessor();
            }

            return PropertyAccessorCache[type][propertyName];
        }
    }
}
