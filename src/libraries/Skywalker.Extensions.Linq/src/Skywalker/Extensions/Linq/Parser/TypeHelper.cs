using System.Reflection;
using System.Reflection.Emit;

namespace Skywalker.Extensions.Linq.Parser;

internal static class TypeHelper
{
    public static Type? FindGenericType(Type generic, Type? type)
    {
        while (type != null && type != typeof(object))
        {
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == generic)
            {
                return type;
            }

            if (generic.GetTypeInfo().IsInterface)
            {
                foreach (var intfType in type.GetInterfaces())
                {
                    var found = FindGenericType(generic, intfType);
                    if (found != null) return found;
                }
            }

            type = type.GetTypeInfo().BaseType;
        }

        return null;
    }

    public static bool IsCompatibleWith(Type source, Type target)
    {
        if (source == target)
        {
            return true;
        }

        if (!target.IsValueType)
        {
            return target.IsAssignableFrom(source);
        }

        var st = GetNonNullableType(source);
        var tt = GetNonNullableType(target);

        if (st != source && tt == target)
        {
            return false;
        }

        var sc = st.GetTypeInfo().IsEnum ? TypeCode.Object : Type.GetTypeCode(st);
        var tc = tt.GetTypeInfo().IsEnum ? TypeCode.Object : Type.GetTypeCode(tt);
        switch (sc)
        {
            case TypeCode.SByte:
                switch (tc)
                {
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        return true;
                }
                break;
            case TypeCode.Byte:
                switch (tc)
                {
                    case TypeCode.Byte:
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        return true;
                }
                break;
            case TypeCode.Int16:
                switch (tc)
                {
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        return true;
                }
                break;
            case TypeCode.UInt16:
                switch (tc)
                {
                    case TypeCode.UInt16:
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        return true;
                }
                break;
            case TypeCode.Int32:
                switch (tc)
                {
                    case TypeCode.Int32:
                    case TypeCode.Int64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        return true;
                }
                break;
            case TypeCode.UInt32:
                switch (tc)
                {
                    case TypeCode.UInt32:
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        return true;
                }
                break;
            case TypeCode.Int64:
                switch (tc)
                {
                    case TypeCode.Int64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        return true;
                }
                break;
            case TypeCode.UInt64:
                switch (tc)
                {
                    case TypeCode.UInt64:
                    case TypeCode.Single:
                    case TypeCode.Double:
                    case TypeCode.Decimal:
                        return true;
                }
                break;
            case TypeCode.Single:
                switch (tc)
                {
                    case TypeCode.Single:
                    case TypeCode.Double:
                        return true;
                }
                break;
            default:
                if (st == tt)
                {
                    return true;
                }
                break;
        }
        return false;
    }

    public static bool IsClass(Type type)
    {
        var result = false;
        if (type.GetTypeInfo().IsClass)
        {
            // Is Class or Delegate
            if (type != typeof(Delegate))
            {
                result = true;
            }
        }
        return result;
    }

    public static bool IsStruct(Type type)
    {
        var nonNullableType = GetNonNullableType(type);
        if (nonNullableType.GetTypeInfo().IsValueType)
        {
            if (!nonNullableType.GetTypeInfo().IsPrimitive)
            {
                if (nonNullableType != typeof(decimal) && nonNullableType != typeof(DateTime) && nonNullableType != typeof(Guid))
                {
                    if (!nonNullableType.GetTypeInfo().IsEnum)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public static bool IsEnumType(Type type)
    {
        return GetNonNullableType(type).GetTypeInfo().IsEnum;
    }

    public static bool IsNumericType(Type type)
    {
        return GetNumericTypeKind(type) != 0;
    }

    public static bool IsNullableType(Type type)
    {
        Check.NotNull(type, nameof(type));

        return type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    public static bool TypeMaybeNull(Type type)
    {
        Check.NotNull(type, nameof(type));

        return !type.GetTypeInfo().IsValueType || IsNullableType(type);
    }

    public static Type ToNullableType(Type type)
    {
        Check.NotNull(type, nameof(type));

        return IsNullableType(type) ? type : typeof(Nullable<>).MakeGenericType(type);
    }

    public static bool IsSignedIntegralType(Type type)
    {
        return GetNumericTypeKind(type) == 2;
    }

    public static bool IsUnsignedIntegralType(Type type)
    {
        return GetNumericTypeKind(type) == 3;
    }

    private static int GetNumericTypeKind(Type type)
    {
        type = GetNonNullableType(type);

        if (type.GetTypeInfo().IsEnum)
        {
            return 0;
        }

        return Type.GetTypeCode(type) switch
        {
            TypeCode.Char or TypeCode.Single or TypeCode.Double or TypeCode.Decimal => 1,
            TypeCode.SByte or TypeCode.Int16 or TypeCode.Int32 or TypeCode.Int64 => 2,
            TypeCode.Byte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 => 3,
            _ => 0,
        };
    }

    public static string GetTypeName(Type type)
    {
        var baseType = GetNonNullableType(type);

        var name = baseType.Name;
        if (type != baseType)
        {
            name += '?';
        }

        return name;
    }

    public static Type GetNonNullableType(Type type)
    {
        Check.NotNull(type, nameof(type));

        return IsNullableType(type) ? type.GetTypeInfo().GetGenericTypeArguments()[0] : type;
    }

    public static Type GetUnderlyingType(Type? type)
    {
        Check.NotNull(type, nameof(type));

        var genericTypeArguments = type!.GetGenericArguments();
        if (genericTypeArguments.Any())
        {
            var outerType = GetUnderlyingType(genericTypeArguments.LastOrDefault());
            return Nullable.GetUnderlyingType(type) == outerType ? type : outerType;
        }

        return type;
    }

    public static IEnumerable<Type> GetSelfAndBaseTypes(Type type)
    {
        if (type.GetTypeInfo().IsInterface)
        {
            var types = new List<Type>();
            AddInterface(types, type);
            return types;
        }
        return GetSelfAndBaseClasses(type);
    }

    private static IEnumerable<Type> GetSelfAndBaseClasses(Type? type)
    {
        while (type != null)
        {
            yield return type;
            type = type.GetTypeInfo().BaseType;
        }
    }

    private static void AddInterface(List<Type> types, Type type)
    {
        if (!types.Contains(type))
        {
            types.Add(type);
            foreach (var t in type.GetInterfaces())
            {
                AddInterface(types, t);
            }
        }
    }

    public static object? ParseEnum(string value, Type type)
    {
        if (type.GetTypeInfo().IsEnum && Enum.IsDefined(type, value))
        {
            return Enum.Parse(type, value, true);
        }

        return null;
    }
}
