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
}
