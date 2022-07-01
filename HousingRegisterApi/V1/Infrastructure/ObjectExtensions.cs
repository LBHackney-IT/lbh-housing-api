using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HousingRegisterApi.V1.Infrastructure
{
    public static class ObjectExtensions
    {
        public static List<string> GetChangedProperties<T>(object firstObject, object secondObject)
        {
            if (firstObject != null && secondObject != null)
            {
                var type = typeof(T);
                var allProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                var allSimpleProperties = allProperties.Where(pi => pi.PropertyType.IsSimpleType());
                var unequalProperties =
                       from pi in allSimpleProperties
                       let AValue = type.GetProperty(pi.Name).GetValue(firstObject, null)
                       let BValue = type.GetProperty(pi.Name).GetValue(secondObject, null)
                       where AValue != BValue && (AValue == null || !AValue.Equals(BValue))
                       select pi.Name;
                return unequalProperties.ToList();
            }
            else
            {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
                throw new ArgumentNullException("You need to provide 2 non-null objects");
#pragma warning restore CA2208 // Instantiate argument exceptions correctly
            }
        }

        public static bool IsSimpleType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return type.GetGenericArguments()[0].IsSimpleType();
            }
            return type.IsPrimitive
              || type.IsEnum
              || type.Equals(typeof(string))
              || type.Equals(typeof(decimal));
        }
    }
}
