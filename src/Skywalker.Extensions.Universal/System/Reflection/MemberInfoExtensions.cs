// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace System.Reflection;

public static class MemberInfoExtensions
{
    public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo memberInfo, bool inherit = true)
    {
        if (inherit)
        {
            return memberInfo.GetCustomAttributes(true).OfType<Attribute>().ToArray();
        }
        return memberInfo.GetCustomAttributes(false).OfType<Attribute>().ToArray();
    }

    public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this MemberInfo memberInfo, bool inherit = true)
    {
        return GetCustomAttributes(memberInfo, inherit).OfType<TAttribute>();
    }

    public static TAttribute? GetCustomAttribute<TAttribute>(this MemberInfo memberInfo, bool inherit = true)
    {
        return GetCustomAttributes(memberInfo, inherit).OfType<TAttribute>().FirstOrDefault();
    }

    /// <summary>
    /// Gets a single attribute for a member.
    /// </summary>
    /// <typeparam name="TAttribute">Type of the attribute</typeparam>
    /// <param name="memberInfo">The member that will be checked for the attribute</param>
    /// <param name="inherit">Include inherited attributes</param>
    /// <returns>Returns the attribute object if found. Returns null if not found.</returns>
    public static TAttribute? GetSingleAttributeOrNull<TAttribute>(this MemberInfo memberInfo, bool inherit = true)
        where TAttribute : Attribute
    {
        if (memberInfo == null)
        {
            throw new ArgumentNullException(nameof(memberInfo));
        }

        var attrs = memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).ToArray();
        if (attrs.Length > 0)
        {
            return (TAttribute)attrs[0];
        }

        return default;
    }


    public static TAttribute? GetSingleAttributeOfTypeOrBaseTypesOrNull<TAttribute>(this Type type, bool inherit = true) where TAttribute : Attribute
    {
        var attr = type.GetTypeInfo().GetSingleAttributeOrNull<TAttribute>();
        if (attr != null)
        {
            return attr;
        }

        if (type.GetTypeInfo().BaseType == null)
        {
            return null;
        }

        return type.GetTypeInfo().BaseType!.GetSingleAttributeOfTypeOrBaseTypesOrNull<TAttribute>(inherit);
    }
}
