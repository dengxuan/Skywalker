using System.Reflection;
using Skywalker.Ddd.Uow.Abstractions;

namespace Skywalker.Ddd.Uow;

public static class UnitOfWorkHelper
{
    public static bool IsUnitOfWorkType(TypeInfo implementationType)
    {
        //Explicitly defined UnitOfWorkAttribute
        if (HasUnitOfWorkAttribute(implementationType) || AnyMethodHasUnitOfWorkAttribute(implementationType))
        {
            return true;
        }

        //Conventional classes
        if (typeof(IUnitOfWorkEnabled).GetTypeInfo().IsAssignableFrom(implementationType))
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Todo:从Handler中判断是否需要启用
    /// </summary>
    /// <param name="methodInfo"></param>
    /// <param name="unitOfWorkAttribute"></param>
    /// <returns></returns>
    public static bool IsUnitOfWorkMethod(Type methodInfo, out UnitOfWorkAttribute? unitOfWorkAttribute)
    {
        methodInfo.NotNull(nameof(methodInfo));

        //Method declaration
        var attrs = methodInfo.GetCustomAttributes(true).OfType<UnitOfWorkAttribute>().ToArray();
        if (attrs.Any())
        {
            unitOfWorkAttribute = attrs.First();
            return !unitOfWorkAttribute.IsDisabled;
        }

        if (methodInfo.DeclaringType != null)
        {
            //Class declaration
            attrs = methodInfo.DeclaringType.GetTypeInfo().GetCustomAttributes(true).OfType<UnitOfWorkAttribute>().ToArray();
            if (attrs.Any())
            {
                unitOfWorkAttribute = attrs.First();
                return !unitOfWorkAttribute.IsDisabled;
            }

            //Conventional classes
            if (typeof(IUnitOfWorkEnabled).GetTypeInfo().IsAssignableFrom(methodInfo.DeclaringType))
            {
                unitOfWorkAttribute = null;
                return true;
            }
        }

        unitOfWorkAttribute = null;
        return true;
    }

    public static UnitOfWorkAttribute? GetUnitOfWorkAttributeOrNull(MethodInfo methodInfo)
    {
        var attrs = methodInfo.GetCustomAttributes(true).OfType<UnitOfWorkAttribute>().ToArray();
        if (attrs.Length > 0)
        {
            return attrs[0];
        }

        attrs = methodInfo.DeclaringType?.GetTypeInfo().GetCustomAttributes(true).OfType<UnitOfWorkAttribute>().ToArray();
        if (attrs?.Length > 0)
        {
            return attrs[0];
        }

        return null;
    }

    private static bool AnyMethodHasUnitOfWorkAttribute(TypeInfo implementationType)
    {
        return implementationType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .Any(HasUnitOfWorkAttribute);
    }

    private static bool HasUnitOfWorkAttribute(MemberInfo methodInfo)
    {
        return methodInfo.IsDefined(typeof(UnitOfWorkAttribute), true);
    }
}
