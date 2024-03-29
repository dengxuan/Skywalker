﻿// Licensed to the Gordon under one or more agreements.
// Gordon licenses this file to you under the MIT license.

using System.Collections;
using System.Reflection;

namespace Skywalker.Extensions.Linq;

/// <summary>
/// Define extensions on <see cref="IEnumerable"/>.
/// </summary>
public static class DynamicEnumerableExtensions
{
    private static readonly MethodInfo s_toDynamicArrayGenericMethod;

    static DynamicEnumerableExtensions()
    {
        s_toDynamicArrayGenericMethod = typeof(DynamicEnumerableExtensions).GetTypeInfo().GetDeclaredMethods("ToDynamicArray").First(x => x.IsGenericMethod);
    }

    /// <summary>
    /// Creates an array of dynamic objects from a <see cref="IEnumerable"/>.
    /// </summary>
    /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
    /// <returns>An array that contains the elements from the input sequence.</returns>
    public static dynamic[] ToDynamicArray(this IEnumerable source)
    {
        Check.NotNull(source, nameof(source));
        return CastToArray<dynamic>(source);
    }

    /// <summary>
    /// Creates an array of dynamic objects from a <see cref="IEnumerable"/>.
    /// </summary>
    /// <typeparam name="T">The generic type.</typeparam>
    /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
    /// <returns>An Array{T} that contains the elements from the input sequence.</returns>
    public static T[] ToDynamicArray<T>(this IEnumerable source)
    {
        Check.NotNull(source, nameof(source));
        return CastToArray<T>(source);
    }

    /// <summary>
    /// Creates an array of dynamic objects from a <see cref="IEnumerable"/>.
    /// </summary>
    /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
    /// <param name="type">A <see cref="Type"/> cast to.</param>
    /// <returns>An Array that contains the elements from the input sequence.</returns>
    public static dynamic[] ToDynamicArray(this IEnumerable source, Type type)
    {
        Check.NotNull(source, nameof(source));
        Check.NotNull(type, nameof(type));

        var result = (IEnumerable)s_toDynamicArrayGenericMethod.MakeGenericMethod(type).Invoke(source, new object[] { source })!;
        return CastToArray<dynamic>(result);
    }

    /// <summary>
    /// Creates a list of dynamic objects from a <see cref="IEnumerable"/>.
    /// </summary>
    /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
    /// <returns>A List that contains the elements from the input sequence.</returns>
    public static List<dynamic> ToDynamicList(this IEnumerable source)
    {
        Check.NotNull(source, nameof(source));
        return CastToList<dynamic>(source);
    }

    /// <summary>
    /// Creates a list of dynamic objects from a <see cref="IEnumerable"/>.
    /// </summary>
    /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
    /// <param name="type">A <see cref="Type"/> cast to.</param>
    /// <returns>A List that contains the elements from the input sequence.</returns>
    public static List<dynamic> ToDynamicList(this IEnumerable source, Type type)
    {
        Check.NotNull(source, nameof(source));
        Check.NotNull(type, nameof(type));

        return ToDynamicArray(source, type).ToList();
    }

    /// <summary>
    /// Creates a list of dynamic objects from a <see cref="IEnumerable"/>.
    /// </summary>
    /// <typeparam name="T">Generic Type</typeparam>
    /// <param name="source">A <see cref="IEnumerable"/> to create an array from.</param>
    /// <returns>A List{T} that contains the elements from the input sequence.</returns>
    public static List<T> ToDynamicList<T>(this IEnumerable source)
    {
        Check.NotNull(source, nameof(source));
        return CastToList<T>(source);
    }

    internal static T[] CastToArray<T>(IEnumerable source)
    {
        return source.Cast<T>().ToArray();
    }

    internal static List<T> CastToList<T>(IEnumerable source)
    {
        return source.Cast<T>().ToList();
    }
}
