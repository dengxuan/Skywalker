using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace System.Reflection;

public static class TypeHelper
{
    private static readonly HashSet<Type> s_floatingTypes = new()
    {
        typeof(float),
        typeof(double),
        typeof(decimal)
    };

    private static readonly HashSet<Type> s_nonNullablePrimitiveTypes = new()
    {
        typeof(byte),
        typeof(short),
        typeof(int),
        typeof(long),
        typeof(sbyte),
        typeof(ushort),
        typeof(uint),
        typeof(ulong),
        typeof(bool),
        typeof(float),
        typeof(decimal),
        typeof(DateTime),
        typeof(DateTimeOffset),
        typeof(TimeSpan),
        typeof(Guid)
    };

    public static bool IsNonNullablePrimitiveType(Type type)
    {
        return s_nonNullablePrimitiveTypes.Contains(type);
    }

    public static bool IsFunc(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        var type = obj.GetType();
        if (!type.GetTypeInfo().IsGenericType)
        {
            return false;
        }

        return type.GetGenericTypeDefinition() == typeof(Func<>);
    }

    public static bool IsFunc<TReturn>(object obj)
    {
        return obj != null && obj.GetType() == typeof(Func<TReturn>);
    }

    public static bool IsPrimitiveExtended(Type type, bool includeNullables = true, bool includeEnums = false)
    {
        if (IsPrimitiveExtendedInternal(type, includeEnums))
        {
            return true;
        }

        if (includeNullables && IsNullable(type))
        {
            return IsPrimitiveExtendedInternal(type.GenericTypeArguments[0], includeEnums);
        }

        return false;
    }

    public static bool IsNullable(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
    }

    public static Type? GetFirstGenericArgumentIfNullable(this Type t)
    {
        if (t.GetGenericArguments().Length > 0 && t.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return t.GetGenericArguments().FirstOrDefault();
        }

        return t;
    }

    public static bool IsEnumerable(Type type, out Type? itemType, bool includePrimitives = true)
    {
        if (!includePrimitives && IsPrimitiveExtended(type))
        {
            itemType = null;
            return false;
        }

        var enumerableTypes = ReflectionHelper.GetImplementedGenericTypes(type, typeof(IEnumerable<>));
        if (enumerableTypes.Count == 1)
        {
            itemType = enumerableTypes[0].GenericTypeArguments[0];
            return true;
        }

        if (typeof(IEnumerable).IsAssignableFrom(type))
        {
            itemType = typeof(object);
            return true;
        }

        itemType = null;
        return false;
    }

    public static bool IsDictionary(Type type, out Type? keyType, out Type? valueType)
    {
        var dictionaryTypes = ReflectionHelper
            .GetImplementedGenericTypes(
                type,
                typeof(IDictionary<,>)
            );

        if (dictionaryTypes.Count == 1)
        {
            keyType = dictionaryTypes[0].GenericTypeArguments[0];
            valueType = dictionaryTypes[0].GenericTypeArguments[1];
            return true;
        }

        if (typeof(IDictionary).IsAssignableFrom(type))
        {
            keyType = typeof(object);
            valueType = typeof(object);
            return true;
        }

        keyType = null;
        valueType = null;

        return false;
    }

    private static bool IsPrimitiveExtendedInternal(Type type, bool includeEnums)
    {
        if (type.IsPrimitive)
        {
            return true;
        }

        if (includeEnums && type.IsEnum)
        {
            return true;
        }

        return type == typeof(string) ||
               type == typeof(decimal) ||
               type == typeof(DateTime) ||
               type == typeof(DateTimeOffset) ||
               type == typeof(TimeSpan) ||
               type == typeof(Guid);
    }

    public static T? GetDefaultValue<T>()
    {
        return default;
    }

    public static object? GetDefaultValue(Type type)
    {
        if (type.IsValueType)
        {
            return Activator.CreateInstance(type);
        }

        return null;
    }

    public static string GetFullNameHandlingNullableAndGenerics(Type type)
    {
        type.NotNull(nameof(type));

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return type.GenericTypeArguments[0].FullName + "?";
        }

        if (type.IsGenericType)
        {
            var genericType = type.GetGenericTypeDefinition();
            return $"{genericType.FullName!.Left(genericType.FullName!.IndexOf('`'))}<{type.GenericTypeArguments.Select(GetFullNameHandlingNullableAndGenerics).JoinAsString(",")}>";
        }

        return type.FullName!;
    }

    public static string GetSimplifiedName(Type type)
    {
        type.NotNull(nameof(type));

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return GetSimplifiedName(type.GenericTypeArguments[0]) + "?";
        }

        if (type.IsGenericType)
        {
            var genericType = type.GetGenericTypeDefinition();
            return $"{genericType.FullName!.Left(genericType.FullName!.IndexOf('`'))}<{type.GenericTypeArguments.Select(GetSimplifiedName).JoinAsString(",")}>";
        }

        if (type == typeof(string))
        {
            return "string";
        }
        else if (type == typeof(int))
        {
            return "number";
        }
        else if (type == typeof(long))
        {
            return "number";
        }
        else if (type == typeof(bool))
        {
            return "boolean";
        }
        else if (type == typeof(char))
        {
            return "string";
        }
        else if (type == typeof(double))
        {
            return "number";
        }
        else if (type == typeof(float))
        {
            return "number";
        }
        else if (type == typeof(decimal))
        {
            return "number";
        }
        else if (type == typeof(DateTime))
        {
            return "string";
        }
        else if (type == typeof(DateTimeOffset))
        {
            return "string";
        }
        else if (type == typeof(TimeSpan))
        {
            return "string";
        }
        else if (type == typeof(Guid))
        {
            return "string";
        }
        else if (type == typeof(byte))
        {
            return "number";
        }
        else if (type == typeof(sbyte))
        {
            return "number";
        }
        else if (type == typeof(short))
        {
            return "number";
        }
        else if (type == typeof(ushort))
        {
            return "number";
        }
        else if (type == typeof(uint))
        {
            return "number";
        }
        else if (type == typeof(ulong))
        {
            return "number";
        }
        else if (type == typeof(IntPtr))
        {
            return "number";
        }
        else if (type == typeof(UIntPtr))
        {
            return "number";
        }

        return type.FullName!;
    }

    public static object? ConvertFromString<TTargetType>(string value)
    {
        return ConvertFromString(typeof(TTargetType), value);
    }

    public static object? ConvertFromString(Type targetType, string value)
    {
        if (value == null)
        {
            return null;
        }

        var converter = TypeDescriptor.GetConverter(targetType);

        if (IsFloatingType(targetType))
        {
            using (CultureHelper.Use(CultureInfo.InvariantCulture))
            {
                return converter.ConvertFromString(value.Replace(',', '.'));
            }
        }

        return converter.ConvertFromString(value);
    }

    public static bool IsFloatingType(Type type, bool includeNullable = true)
    {
        if (s_floatingTypes.Contains(type))
        {
            return true;
        }

        if (includeNullable && IsNullable(type) && s_floatingTypes.Contains(type.GenericTypeArguments[0]))
        {
            return true;
        }

        return false;
    }

    public static object? ConvertFrom<TTargetType>(object value)
    {
        return ConvertFrom(typeof(TTargetType), value);
    }

    public static object? ConvertFrom(Type targetType, object value)
    {
        return TypeDescriptor.GetConverter(targetType).ConvertFrom(value);
    }

    public static Type StripNullable(Type type)
    {
        return IsNullable(type) ? type.GenericTypeArguments[0] : type;
    }

    public static bool IsDefaultValue(object obj)
    {
        if (obj == null)
        {
            return true;
        }

        return obj.Equals(GetDefaultValue(obj.GetType()));
    }

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
