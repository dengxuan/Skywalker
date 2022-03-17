using System.Collections.Concurrent;

namespace System.Reflection;

public static class CustomAttributeAccessor
{
    private static readonly ConcurrentDictionary<object, Attribute[]> s_attributes = new();
    private static readonly ConcurrentDictionary<object, Attribute[]> s_ownAttributes = new();
    public static IEnumerable<Attribute> GetCustomAttributes(MemberInfo memberInfo, bool inherit = true)
    {
        Attribute[]? attributes;
        if (inherit)
        {
            return s_attributes.TryGetValue(memberInfo, out attributes)
             ? attributes
             : s_attributes[memberInfo] = memberInfo.GetCustomAttributes(true).OfType<Attribute>().ToArray();
        }

        return s_ownAttributes.TryGetValue(memberInfo, out attributes)
          ? attributes
          : s_attributes[memberInfo] = memberInfo.GetCustomAttributes(false).OfType<Attribute>().ToArray();
    }
    public static IEnumerable<Attribute> GetCustomAttributes(Type type, bool inherit = true)
    {
        var typeInfo = type.GetTypeInfo();
        Attribute[]? attributes;
        if (inherit)
        {
            return s_attributes.TryGetValue(typeInfo, out attributes)
             ? attributes
             : s_attributes[type] = typeInfo.GetCustomAttributes(true).OfType<Attribute>().ToArray();
        }

        return s_ownAttributes.TryGetValue(typeInfo, out attributes)
          ? attributes
          : s_attributes[type] = typeInfo.GetCustomAttributes(false).OfType<Attribute>().ToArray();
    }

    public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(MemberInfo memberInfo, bool inherit = true)
    {
        return GetCustomAttributes(memberInfo, inherit).OfType<TAttribute>();
    }

    public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(Type type, bool inherit = true)
    {
        return GetCustomAttributes(type, inherit).OfType<TAttribute>();
    }

    public static TAttribute? GetCustomAttribute<TAttribute>(MemberInfo memberInfo, bool inherit = true)
    {
        return GetCustomAttributes(memberInfo, inherit).OfType<TAttribute>().FirstOrDefault();
    }

    public static TAttribute? GetCustomAttribute<TAttribute>(Type type, bool inherit = true)
    {
        return GetCustomAttributes(type, inherit).OfType<TAttribute>().FirstOrDefault();
    }
}
