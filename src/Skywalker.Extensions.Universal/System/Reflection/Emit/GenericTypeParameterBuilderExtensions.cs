// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

namespace System.Reflection.Emit;

public static class GenericTypeParameterBuilderExtensions
{
    /// <summary>
    /// https://github.com/castleproject/Core/blob/netcore/src/Castle.Core/Compatibility/CustomTypeBuilderExtensions.cs
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static Type AsType(this GenericTypeParameterBuilder builder)
    {
        return builder;
    }
}
