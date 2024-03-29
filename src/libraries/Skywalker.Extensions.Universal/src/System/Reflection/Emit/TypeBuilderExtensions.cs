﻿namespace System.Reflection.Emit;

public static class TypeBuilderExtensions
{
    public static Type? CreateType(this TypeBuilder builder)
    {
        return builder.CreateTypeInfo()?.AsType();
    }

    public static PropertyBuilder DefineProperty(this TypeBuilder tb, string name, PropertyAttributes attributes, CallingConventions callingConvention, Type returnType, Type[] parameterTypes)
    {
        return tb.DefineProperty(name, attributes, callingConvention, returnType, parameterTypes);
    }

    // https://github.com/castleproject/Core/blob/netcore/src/Castle.Core/Compatibility/CustomTypeBuilderExtensions.cs
    // TypeBuilder and GenericTypeParameterBuilder no longer inherit from Type but TypeInfo,
    // so there is now an AsType method to get the Type which we are providing here to shim to itself.
    public static Type AsType(this TypeBuilder builder)
    {
        return builder;
    }
}
